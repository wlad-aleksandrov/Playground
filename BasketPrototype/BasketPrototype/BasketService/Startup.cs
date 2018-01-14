using Aleksandrov.BasketRepository;
using Aleksandrov.Inventory;
using BasketService.Utils;
using Halcyon.Web.HAL.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aleksandrov.BasketService {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc(o => {
                o.OutputFormatters.RemoveType<JsonOutputFormatter>();
                o.OutputFormatters.Add(new JsonHalOutputFormatter(new string[] {
                    "application/hal+json",
                    "application/vnd.example.hal+json",
                    "application/vnd.example.hal.vl+json"
                }));

            });

            // add repositories

            var catalogue = new DummyCatalogue();
            for (var i = 1; i <= 100; i++) {
                catalogue.Add($"Item {i}", $"Description Item {i}", $"picture{1}", i + .2m);
            }

            services.AddSingleton<IBasketRepository, DummyBasketRepository>();
            services.AddSingleton<ICatalogue>(catalogue);
            services.AddSingleton<IClock, LocalClock>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpContext();

            app.UseMvc();
        }
    }
}
