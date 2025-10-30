using MedSchedule.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Application.Responses
{
    public class AppointmentPaginatedResponse
    {
        public List<AppointmentShortResponse> Appointments { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }

    public class AppointmentShortResponse
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public Guid PatientId { get; set; }
        public ScheduleWorkResponse ScheduleWork { get; set; }
    }
    
    public class AppointmentResponse
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public Guid PatientId { get; set; }
        public ScheduleWorkResponse ScheduleWork { get; set; }
        public string SpecialtyName { get; set; }
        public decimal Duration { get; set; }
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
