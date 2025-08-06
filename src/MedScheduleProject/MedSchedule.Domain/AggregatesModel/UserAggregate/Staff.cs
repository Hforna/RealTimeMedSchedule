using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.AggregatesModel.UserAggregate
{
    public class Staff : Entity
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public string Role { get; set; }
        public int MaxPriorityLevel { get; set; }
        public required ScheduleWork ScheduleWork { get; set; }
        public Guid? SpecialtyId { get; set; }
        public Specialty? Specialty { get; set; }
    }

    public class Specialty : Entity
    {
        public string Name { get; set; }
        public int AvgConsultationTime { get; set; }
        public int MinEmergencySlots { get; set; }
    }
}
