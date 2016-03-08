using SuperSocket.WebSocket.SubProtocol;
using WA.Notification;

namespace WA.NotificationService
{
    public class Publish : JsonSubCommandBase<NotificationSession, NotificationMessage>
    {
        protected override void ExecuteJsonCommand(NotificationSession session, NotificationMessage notification)
        {
            session.NotificationServer.RecordNotification(notification);
            // We need to inform all subscribes about the changes related to the given AuctionId
            foreach (var subscriber in session.AppServer.GetSessions(s => s.NotificationId == notification.AuctionId))
            {
                subscriber.SendJsonMessage(null, notification);
            }
        }
    }
}