using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Application.Requests
{
    public class CreateNewStaffRequest
    {
        public Guid UserId { get; set; }
        public string? SpecialtyName { get; set; }
        public WorkShiftRequest WorkShift { get; set; }
    }

    public class WorkShiftRequest
    {
        public int StartHours { get; set; }
        public int StartMinutes { get; set; }
        public int EndHours { get; set; }
        public int EndMinutes { get; set; }
    }
}
