using AuctionServiceWebApi.Models;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WA.Notification;
using WA.Repository;

namespace AuctionServiceWebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            var container = new UnityContainer();
            var notificationClient = new ConcurrentWebSocketNotificationClient();
            notificationClient.Setup("ws://localhost:8089");
            var repository = new RedisRepository("localhost");

            container.RegisterInstance<IRepository>(repository);
            container.RegisterInstance<INotificationClient>(notificationClient);
            config.DependencyResolver = new UnityResolver(container);
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
