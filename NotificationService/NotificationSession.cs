using SuperSocket.WebSocket;

namespace WA.NotificationService
{
    public class NotificationSession : JsonWebSocketSession<NotificationSession>
    {
        //AuctionID for changes to watch for
        public string NotificationId { get; set; }

        //Reference back to NotificationServer to record/resend last notification
        public INotificationServer NotificationServer { set; get; }
    }
}