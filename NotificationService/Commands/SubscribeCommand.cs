using SuperSocket.WebSocket.SubProtocol;

namespace WA.NotificationService
{
    public class Subscribe : JsonSubCommandBase<NotificationSession, string>
    {
        protected override void ExecuteJsonCommand(NotificationSession session, string auctionId)
        {
            session.NotificationId = auctionId;
            var lastNotification = session.NotificationServer.GetLastNotification(auctionId);
            if (lastNotification != null)
            {
                session.SendJsonMessage(null, lastNotification);
            }
        }
    }
}