using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Application.Requests
{
    public class StaffsPaginatedRequest
    {
        public string? SpecialtyName { get; set; }
        public WorkShiftRequest? WorkShift { get; set; }
        public int? PerPage { get; set; }
        public int? Page { get; set; }
        public int? MinTotalServices { get; set; }
        public int? MaxTotalServices { get; set; }
    }
}
