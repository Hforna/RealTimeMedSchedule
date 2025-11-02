using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Application.Requests
{
    public class OverridePriorityRequest
    {
        public int PriorityScore { get; set; }
        public required string Reason { get; set; }
    }
}
