using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using System;
using System.Collections.Generic;
using WA.NotificationService.Properties;

namespace WA.NotificationService
{
    public class NotificationServerFactory : INotificationServerFactory
    {
        public INotificationServer GetNotificationServer(IDictionary<string, string> parameters)
        {

            int port;

            if (!parameters.ContainsKey("WebSocket.Port") || !int.TryParse(parameters["WebSocket.Port"], out port))
                throw new ArgumentException("WebSocket.Port");
            var serverConfig = new ServerConfig
            {
                Name = "SecureSuperWebSocket",
                Ip = "Any",
                Port = port,
                Mode = SocketMode.Tcp,
                DisableSessionSnapshot = true
            };

            var appServer = new NotificationWebSocketServer();
            var status = appServer.Setup(new RootConfig(), serverConfig);

            if (!status)
                throw new Exception(string.Format(Resources.Str_ErrCannotCreateServer, serverConfig.Port));
            return appServer;
        }
    }
}