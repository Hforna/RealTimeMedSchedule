using MedSchedule.Domain.AggregatesModel.UserAggregate;
using MedSchedule.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Application.Responses
{
    public class StaffResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; }
        public int MaxPriorityLevel { get; set; }
        public required WorkShift WorkShift { get; set; }
        public Guid? SpecialtyId { get; set; }
        public Specialty? Specialty { get; set; }
        public int TotalServices { get; set; } = 0;
        public int? AvgConsultationTime { get; set; }
    }

    public class StaffsResponse
    {
        public int Count { get; set; }
        public int PageNumber { get; set; }
        public List<StaffResponse> Staffs { get; set; }
        public bool IsFirstPage { get; set; }
        public bool IsLastPage { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}
