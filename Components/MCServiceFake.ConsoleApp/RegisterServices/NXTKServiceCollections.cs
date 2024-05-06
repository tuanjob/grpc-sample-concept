using Autofac;
using MCServiceFake.ConsoleApp.Services;
using NXTK.GrpcServer;
using NXTK.GrpcServer.Interfaces;
using NXTK.GrpcServer.Services;

namespace MCServiceFake.ConsoleApp.RegisterServices
{
    public static class NXTKServiceCollections
    {
        public static void RegisterServices()
        {
            var builder = new ContainerBuilder();

            // Register services with Autofac
            builder.RegisterType<GrpcDataService>().As<IOutrightIncDataServiceInvoker>().SingleInstance();
            builder.RegisterType<OutrightFullDataServiceBuilder>().As<IOutrightFullDataServiceInvoker>().InstancePerLifetimeScope();

            // Register additional services with Autofac if needed

            // Build the Autofac container
            var container = builder.Build();

            // Return the Autofac container
            // return container;
            DependencyResolver.Initialize(container);
        }
    }
}
