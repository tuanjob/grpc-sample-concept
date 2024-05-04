using MCServiceFake.ConsoleApp.Services;
using Microsoft.Extensions.DependencyInjection;
using NXTK.GrpcServer;
using NXTK.GrpcServer.Interfaces;

namespace MCServiceFake.ConsoleApp.RegisterServices
{
    public static class NXTKServiceCollections
    {
        /// <summary>
        /// Register all services for WebServerController
        /// </summary>
        public static void RegisterServices()
        {
            var services = new ServiceCollection();
            services.AddScoped<IOutrightFullDataServiceInvoker, OutrightFullDataServiceBuilder>();

            var serviceProvider = services.AddGrpcServerService()
                                          .BuildServiceProvider();

            DependencyResolver.Initialize(serviceProvider);
        }
    }
}
