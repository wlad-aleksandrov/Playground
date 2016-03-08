using SuperSocket.WebSocket;
using System.Collections.Concurrent;
using WA.Notification;

namespace WA.NotificationService
{
    public class NotificationWebSocketServer : WebSocketServer<NotificationSession>, INotificationServer
    {
        readonly ConcurrentDictionary<string, NotificationMessage> _lastNotifications;
        readonly object _lock = new object();

        public NotificationWebSocketServer()
        {
            _lastNotifications = new ConcurrentDictionary<string, NotificationMessage>();
        }

        protected override void OnNewSessionConnected(NotificationSession session)
        {
            session.NotificationServer = this;
            base.OnNewSessionConnected(session);
        }

        public void RecordNotification(NotificationMessage notification)
        {
            lock (_lock)
            {
                if (!_lastNotifications.ContainsKey(notification.AuctionId) ||
                    _lastNotifications[notification.AuctionId].HighestBid < notification.HighestBid)
                {
                    _lastNotifications[notification.AuctionId] = notification;
                }
            }
        }

        public NotificationMessage GetLastNotification(string auctionId) => _lastNotifications.ContainsKey(auctionId) ? _lastNotifications[auctionId] : null;

    }
}