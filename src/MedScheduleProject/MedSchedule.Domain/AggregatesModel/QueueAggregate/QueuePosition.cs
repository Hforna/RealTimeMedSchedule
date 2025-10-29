using MedSchedule.Domain.AggregatesModel.AppointmentAggregate;
using MedSchedule.Domain.Enums;
using MedSchedule.Domain.Exceptions;
using MedSchedule.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.AggregatesModel.QueueAggregate
{
    public class QueuePosition : Entity
    {
        public Guid AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public Guid QueueId { get; set; }
        public QueueRoot Queue { get; set; }
        //Raw position is calculated based on date the user schedules appointment,
        //as soon as they schedule the appointment so the raw position becomes fixed
        public int RawPosition { get; set; }
        //Estimated minutes for user be called
        public int EstimatedMinutes { get; set; }
        public int EffectivePosition { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public interface IQueueDomainService
    {
        public Task<QueuePosition> SetQueuePosition(QueueRoot queueRoot, Appointment appointment);
    }

    public class QueueDomainService : IQueueDomainService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<QueueDomainService> _logger;

        public QueueDomainService(IUnitOfWork uow, ILogger<QueueDomainService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<QueuePosition> SetQueuePosition(QueueRoot queueRoot, Appointment appointment)
        {
            if (queueRoot.QueuePositions is null)
                throw new DomainException("Queue root must contains queue positions for set appointment position");

            var queuePosition = new QueuePosition()
            {
                Appointment = appointment,
                AppointmentId = appointment.Id,
                LastUpdate = DateTime.UtcNow,
                QueueId = queueRoot.Id
            };

            queueRoot.QueuePositions.Add(queuePosition);

            var rawPositions = queueRoot.QueuePositions
                .OrderBy(d => d.Appointment.Schedule.AppointmentDate)
                .ToList();

            queuePosition.RawPosition = rawPositions.IndexOf(queuePosition) + 1;

            var notChecked = rawPositions.Where(p => p.Appointment.CheckInDate == null).ToList();
            var checkedIn = rawPositions.Where(p => p.Appointment.CheckInDate != null).ToList();

            for (var i = 0; i < checkedIn.Count; i++)
            {
                var currPosition = checkedIn[i];
                if (currPosition.Appointment.CheckInDate is not null)
                {
                    currPosition.EffectivePosition = CalculatePosition(currPosition.Appointment.Schedule.AppointmentDate, 
                        currPosition.Appointment.PriorityLevel, (DateTime)currPosition.Appointment.CheckInDate, currPosition.RawPosition);

                    //Calculate time to wait on room until patient be called
                    var avgConsultTime = currPosition.Appointment.Specialty.AvgConsultationTime;
                    var patientsAhead = rawPositions[..i].Count;

                    currPosition.EstimatedMinutes = avgConsultTime * patientsAhead;
                    currPosition.LastUpdate = DateTime.UtcNow;
                }
            }

            if(notChecked.Count > 0)
            {
                var lastEffective = checkedIn.Count > 0 ? checkedIn[checkedIn.Count - 1].EffectivePosition : 0;
                for (var i = 0; i < notChecked.Count; i++)
                {
                    notChecked[i].EffectivePosition = lastEffective + 1;
                    checkedIn.Add(notChecked[i]);
                    lastEffective++;
                }
            }

            if (!checkedIn.Any(p => p.AppointmentId == queuePosition.AppointmentId))
                await _uow.GenericRepository.Add<QueuePosition>(queuePosition);

            _uow.GenericRepository.UpdateRange<QueuePosition>(checkedIn);
            await _uow.Commit();

            return queuePosition;
        }

        private int CalculatePosition(DateTime scheduled, EPriorityLevel priorityLevel, DateTime checkIn, int rawPosition)
        {
            var offset = (checkIn - scheduled).TotalMinutes;
            rawPosition += (int)(offset / 15);
            rawPosition -= GetPriorityLevel(priorityLevel);

            _logger.LogInformation($"Position calculated: {rawPosition}");

            return Math.Max(1, rawPosition);
        }

        private int GetPriorityLevel(EPriorityLevel priorityLevel) => priorityLevel switch
        {
            EPriorityLevel.EMERGENCY => 3,
            EPriorityLevel.URGENT => 2,
            EPriorityLevel.ROUTINE => 1,
            _ => 0
        };
    }
}
