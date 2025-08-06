using MedSchedule.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Application.Responses
{
    public class AppointmentResponse
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public Guid PatientId { get; set; }
        public ScheduleWorkResponse ScheduleWork { get; set; }
        public int Duration { get; set; }
        public EAppointmentStatus AppointmentStatus { get; set; }
    }

    public class ScheduleWorkResponse
    {
        public int StartHours { get; set; }
        public int StartMinutes { get; set; }
        public int EndHours { get; set; }
        public int EndMinutes { get; set; }
    }
}
