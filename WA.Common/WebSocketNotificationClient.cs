using Newtonsoft.Json;
using System;
using System.Threading;
using WA.Notification.Properties;
using WebSocket4Net;

namespace WA.Notification
{
    public class WebSocketNotificationClient : INotificationClient, IDisposable
    {
        bool _disposed;

        WebSocket _websocket;
        readonly TimeSpan _websocketOpenTimeout = TimeSpan.FromSeconds(5);

        // WaitHandle to wait for the WebSocket.Opened Event
        readonly EventWaitHandle _waitHandle = new AutoResetEvent(false);
        public bool Setup(string url) => Setup(url, _websocketOpenTimeout);

        public bool Setup(string url, TimeSpan timeout)
        {
            _websocket = new WebSocket(url);
            _websocket.Opened += webSocket_Opened;
            _websocket.Open();
            return _waitHandle.WaitOne(timeout);
        }

        private void webSocket_Opened(object sender, EventArgs e)
        {
            _waitHandle.Set();
        }

        public void Publish(NotificationMessage notification)
        {
            if (_websocket.State != WebSocketState.Open)
            {
                // Trying to reconnect
                _websocket.Open();
                _waitHandle.WaitOne(_websocketOpenTimeout);
            }

            if (_websocket.State == WebSocketState.Open)
                _websocket.Send($"Publish {JsonConvert.SerializeObject(notification)}");
            else throw new Exception(string.Format(Resources.Str_ErrServiceOffline, _websocket.State));
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
                if (_waitHandle != null)
                    _waitHandle.Dispose();
                if (_websocket != null)
                    _websocket.Dispose();
            }
            _disposed = true;
        }
    }
}