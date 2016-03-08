using SuperSocket.WebSocket.SubProtocol;

namespace WA.NotificationService
{
    public class Unsubscribe : JsonSubCommandBase<NotificationSession, string>
    {
        protected override void ExecuteJsonCommand(NotificationSession session, string auctionId)
        {
            session.NotificationId = string.Empty;
        }
    }
}