using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JwtWebDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<IJwtAlgorithm, HMACSHA256Algorithm>();
            services.AddSingleton<IJsonSerializer, JsonNetSerializer>();
            services.AddSingleton<IBase64UrlEncoder, JwtBase64UrlEncoder>();
            services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();
            services.AddSingleton<IJwtEncoder>(provider =>
            {
                var algorithm = provider.GetService<IJwtAlgorithm>();
                var jsonSerializer = provider.GetService<IJsonSerializer>();
                var urlEncoder = provider.GetService<IBase64UrlEncoder>();
                return new JwtEncoder(algorithm, jsonSerializer, urlEncoder);
            });
            services.AddSingleton<IJwtValidator>(provider =>
            {
                var jsonSerializer = provider.GetService<IJsonSerializer>();
                var dateTimeProvider = provider.GetService<IDateTimeProvider>();
                return new JwtValidator(jsonSerializer, dateTimeProvider);
            });
            services.AddSingleton<IJwtDecoder>(provider =>
            {
                var jsonSerializer = provider.GetService<IJsonSerializer>();
                var jwtValidator = provider.GetService<IJwtValidator>();
                var urlEncoder = provider.GetService<IBase64UrlEncoder>();
                return new JwtDecoder(jsonSerializer, jwtValidator, urlEncoder);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
