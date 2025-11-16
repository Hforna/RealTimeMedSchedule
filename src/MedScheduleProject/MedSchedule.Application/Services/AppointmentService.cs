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
using MedSchedule.Domain.AggregatesModel.PriorityAssignment;
using System.Reflection.Metadata.Ecma335;

namespace MedSchedule.Application.Services
{
    public interface IAppointmentService
    {
        public Task<AppointmentResponse> CreateAppointment(AppointmentRequest request);
        public Task<AppointmentResponse> NextAppointment();
        public Task<AppointmentResponse> OverridePriority(OverridePriorityRequest request, Guid appointmentId);
        public Task<AppointmentPaginatedResponse> FilterAppointments(int page, int perPage, DateTime? queueDay, string? specialtyName, Guid? staffId, EAppointmentStatus? status,
            EPriorityLevel? priorityLevel, Guid? patientId);

        public Task<AppointmentResponse> CheckInAppointment(Guid id);
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
                    PatientId = patient!.Id,
                    PriorityLevel = request.PriorityLevel,
                    SpecialtyId = specialty.Id,
                    StaffId = professionalLessAppointments!.Id,
                    AppointmentStatus = Domain.Enums.EAppointmentStatus.Scheduled,
                    Schedule = schedule,
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
                ?? throw new NotAuthenticatedException("User must be authenticated for create an appointment");
            
            var staff = await _uow.StaffRepository.GetStaffByUserId(userUid) 
                        ?? throw new UnauthorizedException("Staff assigned to user not found");

            if (staff.ProfessionalInfos is null)
                throw new UnauthorizedException("Staff must be a doctor to manage appointments");

            var queueRoot = await _uow.QueueRepository.GetQueueRootToStaff(staff) 
                            ?? throw new NotFoundException("There aren't any appointment scheduled today");
            var queuePositions = queueRoot!.QueuePositions.OrderByDescending(d => d.EffectivePosition).ToList();

            var inProgress = queuePositions.SingleOrDefault(d => d.Appointment.AppointmentStatus == Domain.Enums.EAppointmentStatus.InProgress);
            var next = queuePositions.FirstOrDefault(d => d.Appointment.AppointmentStatus == Domain.Enums.EAppointmentStatus.CheckedIn
                                                       || d.Appointment.AppointmentStatus == Domain.Enums.EAppointmentStatus.Scheduled);

            if(inProgress is not null)
            {
                inProgress.Appointment.AppointmentStatus = Domain.Enums.EAppointmentStatus.Completed;
                _uow.GenericRepository.Update<QueuePosition>(inProgress);
            }
            if (next is null)
            {
                await _uow.Commit();
                return new();
            }

            next.Appointment.AppointmentStatus = Domain.Enums.EAppointmentStatus.InProgress;
            var appointment = next.Appointment;

            _uow.GenericRepository.Update<QueuePosition>(next);
            await _uow.Commit();

            var patient = await _uow.UserRepository.GetUserById(appointment.PatientId);
            var messageToHub = _mapper.Map<QueueInProgressDto>(next);
            messageToHub.SpecialtyName = staff.ProfessionalInfos.Specialty!.Name;
            messageToHub.ProfessionalName = staff.User.UserName!;
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
                ScheduleWork = _mapper.Map<ScheduleWorkResponse>(appointment.Schedule)
            };
        }

        public async Task<AppointmentPaginatedResponse> FilterAppointments(int page, int perPage, DateTime? queueDay, 
            string? specialtyName, Guid? staffId, EAppointmentStatus? status,
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

        public async Task<AppointmentResponse> CheckInAppointment(Guid id)
        {
            var appointment = await _uow.AppointmentRepository.GetAppointmentById(id)
                ?? throw new NotFoundException("Appointment was not found");

            appointment.CheckIn();

            _uow.GenericRepository.Update<Appointment>(appointment);
            await _uow.Commit();

            return _mapper.Map<AppointmentResponse>(appointment);
        }

        public async Task<AppointmentResponse> OverridePriority(OverridePriorityRequest request, Guid appointmentId)
        {
            if (request.PriorityScore > 10)
                throw new RequestException("Priority score must be between 1 and 10");

            var appointment = await _uow.AppointmentRepository.GetAppointmentById(appointmentId)
                ?? throw new NotFoundException("Appointment was not found");

            var user = await _tokenService.GetUserByToken();
            var staff = await _uow.StaffRepository.GetStaffByUserId(user!.Id);

            if (staff.Id != appointment.StaffId || staff.Role != StaffRoles.AppointmentManager)
                throw new UnauthorizedException("User does not have accces to handle this appointment");

            var priorityOverride = new PriorityOverride()
            {
                Reason = request.Reason,
                NewPriority = request.PriorityScore,
                OriginalPriority = appointment.PriorityScore,
                AppointmentId = appointmentId,
                AuthorizedBy = staff.Id
            };
            appointment.PriorityScore = request.PriorityScore;

            try
            {
                var queueRoot = await _uow.QueueRepository.GetQueueRoot(appointment.SpecialtyId, appointment.Schedule.AppointmentDate) 
                    ?? throw new NotFoundException($"Queue root not found to appointment with id {appointment.Id}");

                await _queueDomain.SetQueuePosition(queueRoot, appointment);
                appointment.AppointmentStatus = EAppointmentStatus.Bumped;

                await _uow.GenericRepository.Add<PriorityOverride>(priorityOverride);
                _uow.GenericRepository.Update<Appointment>(appointment);
                await _uow.Commit();
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpectadly error occurred while trying to set appointment queue position");

                throw new InternalServerException("Unexpected error occurred while trying to set appointment position on queue");
            }

            return new AppointmentResponse()
            {
                Id = appointment.Id,
                AppointmentStatus = appointment.AppointmentStatus,
                SpecialtyName = appointment.Specialty.Name,
                Duration = appointment.Duration,
                PatientId = appointment.PatientId,
                StaffId = appointment.StaffId,
                ScheduleWork = _mapper.Map<ScheduleWorkResponse>(appointment.Schedule),
            };
        }
    }
}