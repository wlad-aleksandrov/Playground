using System.Collections.Generic;

namespace WA.NotificationService
{

    // Better to move the creation of NotificationServer into a dedicated factory, otherwise
    // it is confusing as to why there exist both NotificationServer and NotificationWebSocketServer
    public interface INotificationServerFactory
    {
        INotificationServer GetNotificationServer(IDictionary<string, string> parameter);
    }
}