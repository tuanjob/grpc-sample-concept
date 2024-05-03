using Microsoft.Extensions.DependencyInjection;
using NXTK.GrpcServer.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace NXTK.GrpcServer
{
    public static class ServiceCollections
    {
        public static IServiceCollection AddGrpcServerService(this IServiceCollection services)
        {
            services.AddSingleton<IDataServiceInvoker, DataServiceImpl>();

            return services;
        }
    }
}
