using System;
using System.Configuration;
using System.ServiceModel;
using WA.Notification;

using WA.Repository;

namespace WA.AuctionService
{
    class Program
    {
        static void Main()
        {
            // 1. Create our Redis Repository
            var redisConfig = ConfigurationManager.AppSettings["redisConfiguration"];
            var webSocketUrl = ConfigurationManager.AppSettings["wsNotificationUrl"];

            if (string.IsNullOrWhiteSpace(redisConfig))
                redisConfig = "localhost";

            // Create our two dependencies - Repository and NotificationClient
            using (var repository = new RedisRepository(redisConfig))
            using (var notificationClient = new ConcurrentWebSocketNotificationClient())
            {

                var wsStatus = notificationClient.Setup(webSocketUrl);
                Console.WriteLine($"WebSocket-Notification Service at url {webSocketUrl}:  {(wsStatus ? "Online" : "Offline!")}");

                // Inject our dependencies via Property Injection
                var auctionService = new AuctionService { Repository = repository, NotificationService = notificationClient };

                using (ServiceHost host = new ServiceHost(auctionService))
                {
                    host.Open();
                    Console.WriteLine("Press <Enter> to stop the Auction Service.");
                    Console.ReadLine();
                    host.Close();
                }
            }
        }
    }
}