using MedSchedule.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.AggregatesModel.PriorityAssignment
{
    public class PriorityOverride : Entity
    {
        public Guid AppointmentId { get; set; }
        public int OriginalPriority { get; set; }
        public int NewPriority { get; set; }
        public required string Reason { get; set; }
        [ForeignKey("StaffWhoAuthorized")]
        public Guid AuthorizedBy { get; set; }
        public Staff StaffWhoAuthorized { get; set; }
    }
}
