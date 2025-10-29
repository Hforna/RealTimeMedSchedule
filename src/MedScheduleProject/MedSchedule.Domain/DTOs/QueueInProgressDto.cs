using MedSchedule.Domain.AggregatesModel.AppointmentAggregate;
using MedSchedule.Domain.AggregatesModel.QueueAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.DTOs
{
    public class QueueInProgressDto
    {
        public QueueInProgressDto()
        {
        }

        public QueueInProgressDto(Guid appointmentId, string userName, string professionalName, string specialtyName, int rawPosition, int estimatedMinutes, DateTime lastUpdate)
        {
            AppointmentId = appointmentId;
            UserName = userName;
            ProfessionalName = professionalName;
            SpecialtyName = specialtyName;
            RawPosition = rawPosition;
            EstimatedMinutes = estimatedMinutes;
            LastUpdate = lastUpdate;
        }



        public Guid AppointmentId { get; set; }
        public string UserName { get; set; }
        public string ProfessionalName { get; set; }
        public string SpecialtyName { get; set; }
        public int RawPosition { get; set; }
        public int EstimatedMinutes { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
