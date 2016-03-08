using System;
using System.Collections.Generic;
using System.Configuration;
using WA.NotificationService;

namespace NotificationServiceHost
{
    class Program
    {
        static void Main()
        {
            var factory = new NotificationServerFactory();
            var parameters = new Dictionary<string, string>
            {
                ["WebSocket.Port"] = ConfigurationManager.AppSettings["WebSocket.Port"],
                ["WebSocket.Protocol"] = ConfigurationManager.AppSettings["WebSocket.Protocol"],
                ["Certificate.Thumbprint"] = ConfigurationManager.AppSettings["Certificate.Thumbprint"],
                ["Certificate.StoreLocation"] = ConfigurationManager.AppSettings["Certificate.StoreLocation"],
                ["Certificate.StoreName"] = ConfigurationManager.AppSettings["Certificate.StoreName"]
            };

            INotificationServer notificationServer;
            try
            {
                notificationServer = factory.GetNotificationServer(parameters);
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Failed to set up the server. Details: {exc.GetType()}: {exc.Message}");
                Console.WriteLine("Press any key to quit...");
                Console.ReadKey();
                return;
            }
            var started = notificationServer.Start();
            Console.WriteLine($"The server is started: {(started ? "YES" :"NO")}. Press any key to stop the server...");
            Console.ReadKey();
            notificationServer.Stop();
        }
    }
}