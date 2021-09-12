using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Tharga.Remote.Server.Client;
using Tharga.Remote.Server.Console;

namespace Tharga.Remote.Server
{
    public static class Builder
    {
        public static IServiceCollection AddThargaRemoteServer(this IServiceCollection services)
        {
            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(3);
            });

            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IConsoleService, ConsoleService>();
            services.AddSingleton<IConsoleStore, ConsoleStore>();
            services.AddSingleton<ISelectStore, SelectStore>();

            return services;
        }

        public static IApplicationBuilder UseThargaRemoteServer(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ClientHub>($"/{Constants.ClientHubName}", options =>
                {
                    options.ApplicationMaxBufferSize = 0;
                    options.TransportMaxBufferSize = 0;
                });

                endpoints.MapHub<ConsoleHub>($"/{Constants.ConsoleHubName}", options =>
                {
                    options.ApplicationMaxBufferSize = 0;
                    options.TransportMaxBufferSize = 0;
                });

                //endpoints.MapControllerRoute(
                //    "default",
                //    "{controller=Home}/{action=Index}/{id?}");
            });

            return app;
        }
    }
}