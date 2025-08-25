using MedSchedule.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Application.Requests
{
    public class AppointmentRequest
    {
        public string ReasonText { get; set; }
        public string SpecialtyName { get; set; }
        public DateTime Time { get; set; }
        public EPriorityLevel PriorityLevel { get; set; }
    }
}
