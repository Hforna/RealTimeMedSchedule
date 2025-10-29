using MedSchedule.Domain.DTOs;
using MedSchedule.Domain.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;

namespace MedSchedule.WebApi.Hubs
{
    public class QueueHub : Hub
    {
        
    }

    public class QueueHubService : IQueueHubService
    {
        private readonly IHubContext<QueueHub> _context;
        private readonly ILogger<QueueHubService> _logger;

        public QueueHubService(IHubContext<QueueHub> context, ILogger<QueueHubService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CurrentAppointmentInProgress(QueueInProgressDto queue)
        {
            await _context.Clients.Group(queue.SpecialtyName).SendAsync("next-appointment", queue);
            _logger.LogInformation($"New appointment sent to hub: {queue.SpecialtyName}, {queue}");
        }
    }

}
