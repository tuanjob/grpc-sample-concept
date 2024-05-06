using Autofac;
using NXTK.GrpcServer.Interfaces;
using NXTK.GrpcServer.Services;

namespace NXTK.GrpcServer
{
    public static class ServiceCollections
    {
        public static ContainerBuilder RegisterNXTKGrpcServices(this ContainerBuilder builder)
        {
            // Register services with Autofac
            builder.RegisterType<GrpcDataService>().As<IOutrightIncDataServiceInvoker>().SingleInstance();

            // Add other service registrations here...

            return builder;
        }
    }
}
