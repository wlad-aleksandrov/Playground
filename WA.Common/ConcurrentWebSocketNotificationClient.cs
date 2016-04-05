using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace WA.Notification
{
    public class ConcurrentWebSocketNotificationClient : INotificationClient, IDisposable
    {
        bool _disposed;
        readonly WebSocketNotificationClient _notificationClient;
        readonly BlockingCollection<NotificationMessage> _queue;

        public ConcurrentWebSocketNotificationClient()
        {
            _notificationClient = new WebSocketNotificationClient();
            _queue = new BlockingCollection<NotificationMessage>();

            Task.Run(() =>
            {
                while (!_disposed)
                {
                    var notification = _queue.Take();
                    try
                    {
                        _notificationClient.Publish(notification);
                    }
                    catch (Exception exc)
                    {
                        // add back for later processing
                        //TODO: it probably should not be added to the end of the queue...
                        //LOGGING To be added!!
                        _queue.Add(notification);
                    }
                }

            });
        }

        public bool Setup(string url) => _notificationClient.Setup(url);
        public bool Setup(string url, TimeSpan timeout) => _notificationClient.Setup(url, timeout);
        public void Publish(NotificationMessage notification)
        {
            _queue.Add(notification);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_notificationClient != null)
                    _notificationClient.Dispose();
            }
            _disposed = true;
        }
    }
}
