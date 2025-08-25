using AutoMapper;
using MedSchedule.Application.Requests;
using MedSchedule.Application.Responses;
using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.AggregatesModel.AppointmentAggregate;
using MedSchedule.Domain.AggregatesModel.QueueAggregate;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using MedSchedule.Domain.Exceptions;
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

namespace MedSchedule.Application.Services
{
    public interface IAppointmentService
    {
        public Task<AppointmentResponse> CreateAppointment(AppointmentRequest request);
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AppointmentService> _logger;
        private readonly IQueueDomainService _queueDomain;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AppointmentService(IUnitOfWork uow, ITokenService tokenService, 
            ILogger<AppointmentService> logger, IQueueDomainService queueDomain, 
            IMapper mapper, IEmailService emailService)
        {
            _uow = uow;
            _tokenService = tokenService;
            _logger = logger;
            _queueDomain = queueDomain;
            _mapper = mapper;
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

            List<Staff>? staffs = await _uow.UserRepository.GetStaffsBySpecialty(specialty.Id);

            if (staffs is null)
            {
                _logger.LogError($"None staffs with {specialty.Name} specialty were found");

                throw new ResourceNotFoundException($"There aren't {specialty.Name} avaliable");
            }

            Staff? professionalLessAppointments;

            try
            {
                var avaliableStaffs = await _uow.UserRepository
                    .GetAllSpecialtyStaffAvaliableByIds(staffs.Select(d => d.Id).ToList(), request.Time)
                    ?? throw new UnavaliableException($"There aren't {specialty.Name} professionals avaliable at this time");

                professionalLessAppointments = await _uow.AppointmentRepository.GetStaffWithLessAppointments(avaliableStaffs)
                    ?? throw new ResourceNotFoundException("Professional with less appointments couldn't be found");
            }
            catch(ResourceNotFoundException ex)
            {
                _logger.LogError(ex, $"Professional with less appointments couldn't be found: {ex.Message}");

                throw;
            }
            catch(UnavaliableException ex)
            {
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error while trying to find a professional: {ex.Message}");

                throw new InternalServerException("Occured an error while trying to find a professional");
            }

            var schedule = new ScheduleWork(request.Time.Hour, request.Time.Minute);
            schedule.AppointmentDate = request.Time;
            schedule.CalculateTotalTimeForFinish(specialty.AvgConsultationTime);

            using var transaction = await _uow.BeginTransaction();

            try
            {
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
                    queuePosition = await _queueDomain.SetQueuePosition(queueRoot, appointment);

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
                        EstimatedMinutes = specialty.AvgConsultationTime,
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

                return new AppointmentResponse()
                {
                    Id = appointment.Id,
                    AppointmentStatus = appointment.AppointmentStatus,
                    SpecialtyName = specialty.Name,
                    Duration = queuePosition.EstimatedMinutes,
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
    }
}
