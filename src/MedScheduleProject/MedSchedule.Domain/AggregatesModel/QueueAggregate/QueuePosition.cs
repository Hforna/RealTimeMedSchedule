using MedSchedule.Domain.AggregatesModel.AppointmentAggregate;
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
        public int RawPosition { get; set; }
        public int EstimatedMinutes { get; set; }
        public int EffectivePosition { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
