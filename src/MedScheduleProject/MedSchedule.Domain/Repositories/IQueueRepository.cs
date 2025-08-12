using MedSchedule.Domain.AggregatesModel.QueueAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Repositories
{
    public interface IQueueRepository
    {
        public Task<QueueRoot?> GetQueueRoot(Guid specialtyId, DateTime time);
    }
}
