namespace WA.Notification
{
    public interface INotificationClient
    {
        void Publish(NotificationMessage notification);
    }
}
