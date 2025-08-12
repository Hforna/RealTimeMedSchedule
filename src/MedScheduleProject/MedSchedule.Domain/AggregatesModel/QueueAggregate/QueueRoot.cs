using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.AggregatesModel.QueueAggregate
{
    public class QueueRoot : Entity
    {
        public Guid SpecialtyId { get; set; }
        public DateTime QueueDate { get; set; }
        public int TotalPositions { get; set; }
        public List<QueuePosition> QueuePositions { get; set; }
    }
}
