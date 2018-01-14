using Halcyon.HAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aleksandrov.BasketService {
    public static class MyHttpContext {
        private static IHttpContextAccessor m_httpContextAccessor;

        public static HttpContext Current => m_httpContextAccessor.HttpContext;

        public static Uri AppBaseUrl => new Uri($"{Current.Request.Scheme}://{Current.Request.Host}{Current.Request.PathBase}");

        public static IHALModelConfig HalConfig => new HALModelConfig { LinkBase = AppBaseUrl.ToString() };

        internal static void Configure(IHttpContextAccessor contextAccessor) {
            m_httpContextAccessor = contextAccessor;
        }
    }

    public static class HttpContextExtensions {
        public static void AddHttpContextAccessor(this IServiceCollection services) {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static IApplicationBuilder UseHttpContext(this IApplicationBuilder app) {
            MyHttpContext.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
            return app;
        }
    }
}
