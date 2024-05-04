using Microsoft.Extensions.DependencyInjection;
using NXTK.GrpcServer.Interfaces;
using NXTK.GrpcServer.Services;

namespace NXTK.GrpcServer
{
    public static class ServiceCollections
    {
        public static IServiceCollection AddGrpcServerService(this IServiceCollection services)
        {
            services.AddSingleton<IOutrightIncDataServiceInvoker, GrpcDataService>();

            return services;
        }
    }
}
