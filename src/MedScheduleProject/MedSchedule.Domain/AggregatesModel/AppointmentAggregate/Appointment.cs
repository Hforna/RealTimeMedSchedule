using MedSchedule.Domain.Enums;
using MedSchedule.Domain.Exceptions;
using MedSchedule.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.AggregatesModel.AppointmentAggregate
{
    public class Appointment : Entity
    {
        public Guid PatientId { get; set; }
        public Guid StaffId { get; set; }
        public ScheduleWork Schedule { get; set; }
        public decimal Duration => Schedule.DurationInHours();
        public EPriorityLevel PriorityLevel { get; set; }
        private int _priorityScore;
        public int PriorityScore
        {
            get => _priorityScore;
            set
            {
                if (value < 1 || value > 10)
                    throw new DomainException("Priority score must be between 1 and 10");

                _priorityScore = value;
            }
        }
        public EAppointmentStatus AppointmentStatus { get; set; }
    }
}
