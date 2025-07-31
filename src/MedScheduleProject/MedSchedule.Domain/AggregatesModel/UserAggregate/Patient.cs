using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Aggregates.UserAggregate
{
    public class Patient : Entity
    {
        public required Guid UserId { get; set; }
        public User? User { get; set; }
        public string? ContactPhone { get; set; }
        public string MedicalRecordNumber { get; set; }
        public bool ConsentSms { get; set; } = true;
        public bool ConsentEmail { get; set; } = true;
    }
}
