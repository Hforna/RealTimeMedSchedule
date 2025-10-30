using AutoMapper;
using MedSchedule.Application.Requests;
using MedSchedule.Application.Responses;
using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.AggregatesModel.AppointmentAggregate;
using MedSchedule.Domain.AggregatesModel.QueueAggregate;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using MedSchedule.Domain.DTOs;
using MedSchedule.Domain.Exceptions;
using MedSchedule.Domain.Hubs;
using MedSchedule.Domain.Repositories;
using MedSchedule.Domain.Services;
using MedSchedule.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedSchedule.Domain.Enums;
using X.PagedList;

namespace MedSchedule.Application.Services
{
    public interface IAppointmentService
    {
        public Task<AppointmentResponse> CreateAppointment(AppointmentRequest request);
        public Task<AppointmentResponse> NextAppointment();

        public Task<AppointmentPaginatedResponse> FilterAppointments(int page, int perPage, DateTime? queueDay, string? specialtyName, Guid? staffId, EAppointmentStatus? status,
            EPriorityLevel? priorityLevel, Guid? patientId);
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AppointmentService> _logger;
        private readonly IQueueDomainService _queueDomain;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IQueueHubService _queueHub;

        public AppointmentService(IUnitOfWork uow, ITokenService tokenService, 
            ILogger<AppointmentService> logger, IQueueDomainService queueDomain, 
            IMapper mapper, IEmailService emailService, IQueueHubService queueHub)
        {
            _uow = uow;
            _tokenService = tokenService;
            _logger = logger;
            _queueDomain = queueDomain;
            _mapper = mapper;
            _queueHub = queueHub;
            _emailService = emailService;
        }

        public async Task<AppointmentResponse> CreateAppointment(AppointmentRequest request)
        {
            if (request.Time <= DateTime.UtcNow)
                throw new DomainException("Appointment time must be longer than today");

            var patientUid = _tokenService.GetUserGuidByToken() 
                ?? throw new RequestException("User must be authenticated for create an appointment");
            var patient = await _uow.UserRepository.GetUserById(patientUid);

            Specialty? specialty = await _uow.UserRepository.SpecialtyByName(request.SpecialtyName)
                ?? throw new RequestException("Specialty name not exists");

            Staff? professionalLessAppointments;
            try
            {
                var avaliableStaffs = await _uow.UserRepository
                    .GetAllSpecialtyStaffAvaliableByIds(specialty.Name, request.Time) 
                    ?? throw new UnavaliableException($"There aren't {specialty.Name} professionals avaliable at this time");

                if(avaliableStaffs!.Count == 0)
                    throw new UnavaliableException($"There aren't {specialty.Name} professionals avaliable at this time");

                professionalLessAppointments = await _uow.AppointmentRepository.GetStaffWithLessAppointments(avaliableStaffs);               
            }
            catch(ResourceNotFoundException ex)
            {
                _logger.LogError(ex, $"Professional with less appointments couldn't be found: {ex.Message}");

                throw;
            }
            catch(UnavaliableException ex)
            {
                _logger.LogError($"No specialty avaliable at: {request.Time} time");

                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error while trying to find a professional: {ex.Message}");

                throw new InternalServerException("Occured an error while trying to find a professional");
            }
            using var transaction = await _uow.BeginTransaction();

            try
            {
                var schedule = new ScheduleWork(request.Time.Hour, request.Time.Minute);
                schedule.AppointmentDate = request.Time;
                schedule.CalculateTotalTimeForFinish(specialty.AvgConsultationTime);

                var appointment = new Appointment()
                {
                    PatientId = patient.Id,
                    PriorityLevel = request.PriorityLevel,
                    SpecialtyId = specialty.Id,
                    StaffId = professionalLessAppointments.Id,
                    AppointmentStatus = Domain.Enums.EAppointmentStatus.Scheduled,
                    Schedule = schedule
                };
                await _uow.GenericRepository.Add<Appointment>(appointment);
                await _uow.Commit();

                var queueRoot = await _uow.QueueRepository.GetQueueRoot(specialty.Id, request.Time);
                QueuePosition queuePosition = null!;

                if (queueRoot is not null)
                {
                    queuePosition = await _queueDomain.SetQueuePosition(queueRoot, appointment);
                }

                if(queueRoot is null)
                {
                    queueRoot = new QueueRoot()
                    {
                        QueueDate = new DateTime(request.Time.Year, request.Time.Month, request.Time.Day),
                        SpecialtyId = specialty.Id,
                    };

                    await _uow.GenericRepository.Add<QueueRoot>(queueRoot);
                    await _uow.Commit();

                    queuePosition = new QueuePosition()
                    {
                        AppointmentId = appointment.Id,
                        RawPosition = 1,
                        QueueId = queueRoot.Id,
                        LastUpdate = DateTime.UtcNow,
                        EstimatedMinutes = 0,
                        EffectivePosition = 1
                    };

                    await _uow.GenericRepository.Add<QueuePosition>(queuePosition);
                    await _uow.Commit();
                }

                var professionalUser = professionalLessAppointments.User;
                var professionalFullName = professionalUser.FirstName + professionalUser.LastName;

                await _emailService.SendToPatientAppointmentCreated(
                    patient.Email!, 
                    professionalFullName, 
                    patient.UserName!, 
                    appointment.Schedule.AppointmentDate, 
                    appointment.Duration);

                await transaction.CommitAsync();

                return new AppointmentResponse()
                {
                    Id = appointment.Id,
                    AppointmentStatus = appointment.AppointmentStatus,
                    SpecialtyName = specialty.Name,
                    Duration = appointment.Duration,
                    PatientId = patient.Id,
                    StaffId = professionalLessAppointments.Id,
                    ScheduleWork = _mapper.Map<ScheduleWorkResponse>(appointment.Schedule),
                };
            }catch(DomainException ex)
            {
                _logger.LogError($"A domain exception was threw while trying to configure appointment: {ex.Message}");

                await transaction.RollbackAsync();

                throw;
            }catch(Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while trying to configure appointment: {ex.Message}");

                await transaction.RollbackAsync();

                throw new InternalServerException("Occured an error while trying to configure appointment to patient");
            }
        }

        public async Task<AppointmentResponse> NextAppointment()
        {
            var userUid = _tokenService.GetUserGuidByToken()
                ?? throw new RequestException("User must be authenticated for create an appointment");
            var user = await _uow.UserRepository.GetUserById(userUid);

            var staff = await _uow.StaffRepository.GetStaffByUserId(user.Id) ?? throw new UnavaliableException("Staff assigned to user not found");

            if (staff.ProfessionalInfos is null)
                throw new UnauthorizedException("Staff must be a doctor to manage appointments");

            var queueRoot = await _uow.QueueRepository.GetQueueRootToStaff(staff);
            var queuePositions = queueRoot!.QueuePositions.OrderByDescending(d => d.EffectivePosition).ToList();

            var inProgress = queuePositions.SingleOrDefault(d => d.Appointment.AppointmentStatus == Domain.Enums.EAppointmentStatus.InProgress);
            var next = queuePositions.FirstOrDefault(d => d.Appointment.AppointmentStatus == Domain.Enums.EAppointmentStatus.CheckedIn 
                                                       || d.Appointment.AppointmentStatus == Domain.Enums.EAppointmentStatus.Scheduled);
            if(inProgress is not null)
            {
                inProgress.Appointment.AppointmentStatus = Domain.Enums.EAppointmentStatus.Completed;

                var index = queuePositions.IndexOf(inProgress);
                if (index == queuePositions.Count - 1)
                    return new();

                var nextIndex = queuePositions.IndexOf(inProgress) + 1;
                next = queuePositions[nextIndex];
                while (next.Appointment.AppointmentStatus == Domain.Enums.EAppointmentStatus.Cancelled)
                {
                    if (index == queuePositions.Count - 1)
                        return new();
                    nextIndex++;
                    next = queuePositions[nextIndex];
                }
            }
            next.Appointment.AppointmentStatus = Domain.Enums.EAppointmentStatus.InProgress;
            var appointment = next.Appointment;

            _uow.GenericRepository.UpdateRange<QueuePosition>(queuePositions);
            await _uow.Commit();

            var patient = await _uow.UserRepository.GetUserById(appointment.PatientId);
            var messageToHub = _mapper.Map<QueueInProgressDto>(next);
            messageToHub.SpecialtyName = staff.ProfessionalInfos.Specialty!.Name;
            messageToHub.ProfessionalName = user.UserName!;
            messageToHub.UserName = patient.UserName!;
            await _queueHub.CurrentAppointmentInProgress(messageToHub);

            return new AppointmentResponse()
            {
                Id = appointment.Id,
                AppointmentStatus = appointment.AppointmentStatus,
                SpecialtyName = staff.ProfessionalInfos.Specialty!.Name,
                Duration = next.Appointment.Duration,
                PatientId = patient.Id,
                StaffId = staff.Id,
                ScheduleWork = _mapper.Map<ScheduleWorkResponse>(appointment.Schedule),
            };
        }

        public async Task<AppointmentPaginatedResponse> FilterAppointments(int page, int perPage, DateTime? queueDay, string? specialtyName, Guid? staffId, EAppointmentStatus? status,
            EPriorityLevel? priorityLevel, Guid? patientId)
        {
            if (perPage > 100)
                throw new RequestException("Limit per page is 100");

            var dto = new FilterAppointmentsDto(queueDay, specialtyName, staffId, status, priorityLevel, page, perPage, patientId);
            var appointments = await _uow.AppointmentRepository.FilterAppointmentsPaginated(dto);
            
            var response = _mapper.Map<AppointmentPaginatedResponse>(appointments);
            response.Appointments = appointments.Results.Select(appointment => _mapper.Map<AppointmentShortResponse>(appointment)).ToList();

            return response;
        }
    }
}
