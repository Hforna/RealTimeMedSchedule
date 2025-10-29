using MedSchedule.Domain.AggregatesModel.QueueAggregate;
using MedSchedule.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Hubs
{
    public interface IQueueHubService
    {
        public Task CurrentAppointmentInProgress(QueueInProgressDto queue);
    }
}
