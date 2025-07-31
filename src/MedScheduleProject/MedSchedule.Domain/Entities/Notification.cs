using MedSchedule.Domain.Enums;
using MedSchedule.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Entities
{
    public class Notification : Entity
    {
        public Guid UserId { get; set; }
        public required string Content { get; set; }
        public DateTime SentAt { get; set; }
        public ENotificationStatus Status { get; set; }
        public IList<NotificationChannel> Channels { get; set; } = [];

        public void AddChannel(NotificationChannel notificationChannel)
        {
            if (Channels.Any(d => d.Type == notificationChannel.Type))
                throw new DomainException("Notification already has this type of channel");

            Channels.Add(notificationChannel);
        }
    }

    public enum ENotificationStatus
    {
        PENDING,
        SENT,
        DELIVERED,
        FAILED
    }

    public class NotificationChannel
    {
        public Guid Id { get; set; }
        public Guid NotificationId { get; set; }
        public Notification Notification { get; set; }
        public EChannelType Type { get; set; }
    }
}
