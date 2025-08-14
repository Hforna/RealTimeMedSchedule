using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Application.Requests
{
    public class SetSpecialtyToStaffRequest
    {
        public Guid StaffId { get; set; }
        public string SpecialtyName { get; set; }
    }
}
