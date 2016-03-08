using System;
using WA.Notification;

namespace WA.NotificationService
{
    public interface INotificationServer : IDisposable
    {
        NotificationMessage GetLastNotification(string auctionId);
        void RecordNotification(NotificationMessage notification);

        void Stop();
        bool Start();
    }
}