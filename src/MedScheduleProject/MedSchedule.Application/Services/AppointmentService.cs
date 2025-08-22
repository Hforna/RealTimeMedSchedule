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

        public AppointmentService(IUnitOfWork uow, ITokenService tokenService, 
            ILogger<AppointmentService> logger, IQueueDomainService queueDomain, IMapper mapper)
        {
            _uow = uow;
            _tokenService = tokenService;
            _logger = logger;
            _queueDomain = queueDomain;
            _mapper = mapper;
        }

        public async Task<AppointmentResponse> CreateAppointment(AppointmentRequest request)
        {
            if (request.Time < DateTime.UtcNow)
                throw new DomainException("Appointment time must be longer than today");

            var userUid = _tokenService.GetUserGuidByToken() 
                ?? throw new RequestException("User must be authenticated for create an appointment");

            var user = await _uow.UserRepository.GetUserById(userUid);

            Specialty? specialty = await _uow.UserRepository.SpecialtyByName(request.SpecialtyName)
                ?? throw new RequestException("Specialty name not exists");

            List<Staff>? staffs = await _uow.UserRepository.GetStaffsBySpecialty(specialty.Id);

            if (staffs is null == false)
            {
                _logger.LogError($"None staffs with {specialty.Name} specialty were found");

                throw new ResourceNotFoundException($"There aren't {specialty.Name} avaliable");
            }

            var avaliableStaffs = await _uow.UserRepository
                .GetAllSpecialtyStaffAvaliableByIds(staffs.Select(d => d.Id).ToList(), request.Time) 
                ?? throw new UnavaliableException($"There aren't {specialty.Name} professionals avaliable at this time");

            var professionalLessAppointments = avaliableStaffs.OrderBy(d => d.ProfessionalInfos.Appointments!.Count).First();

            var schedule = new ScheduleWork(request.Time.Hour, request.Time.Minute);
            schedule.AppointmentDate = request.Time;
            schedule.CalculateTotalTimeForFinish(specialty.AvgConsultationTime);

            var appointment = new Appointment()
            {
                PatientId = user.Id,
                PriorityLevel = request.PriorityLevel,
                SpecialtyId = specialty.Id,
                StaffId = professionalLessAppointments.Id,
                AppointmentStatus = Domain.Enums.EAppointmentStatus.Scheduled,
                Schedule = schedule
            };
            await _uow.GenericRepository.Add<Appointment>(appointment);
            await _uow.Commit();

            var queueRoot = await _uow.QueueRepository.GetQueueRoot(specialty.Id, request.Time);
            QueuePosition queuePosition;

            if (queueRoot is not null)
            {
                queueRoot.TotalPositions++;

                if (queueRoot.QueuePositions.Count > queueRoot.TotalPositions)
                    throw new DomainException("There aren't slots avaliable today");

                queuePosition = await _queueDomain.SetQueuePosition(queueRoot, appointment);
            }
            else
            {
                queueRoot = new QueueRoot() { 
                    QueueDate = new DateTime(request.Time.Year, request.Time.Month, request.Time.Day), 
                    SpecialtyId = specialty.Id, TotalPositions = 1 };

                queuePosition = new QueuePosition()
                {
                    AppointmentId = appointment.Id,
                    RawPosition = 1,
                    QueueId = queueRoot.Id,
                    LastUpdate = DateTime.UtcNow,
                    EstimatedMinutes = specialty.AvgConsultationTime,
                    EffectivePosition = 1
                };

                await _uow.GenericRepository.Add<QueueRoot>(queueRoot);
                await _uow.Commit();

                await _uow.GenericRepository.Add<QueuePosition>(queuePosition);
            }
            await _uow.Commit();

            return new AppointmentResponse()
            {
                Id = appointment.Id,
                AppointmentStatus = appointment.AppointmentStatus,
                Duration = queuePosition.EstimatedMinutes,
                PatientId = user.Id,
                StaffId = professionalLessAppointments.Id,
                ScheduleWork = _mapper.Map<ScheduleWorkResponse>(appointment.Schedule),
            };
        }
    }
}
