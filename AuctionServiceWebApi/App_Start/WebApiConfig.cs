using AuctionServiceWebApi.Models;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using WA.Notification;
using WA.Repository;

namespace AuctionServiceWebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            // Enable CORS
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            var container = new UnityContainer();
            var notificationClient = new ConcurrentWebSocketNotificationClient(); 
             var started = notificationClient.Setup(ConfigurationManager.AppSettings["wsNotificationUrl"]);
            var repository = new RedisRepository(ConfigurationManager.AppSettings["redisConfiguration"]);

            container.RegisterInstance<IRepository>(repository);
            container.RegisterInstance<INotificationClient>(notificationClient);
            config.DependencyResolver = new UnityResolver(container);
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "AuctionService/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}