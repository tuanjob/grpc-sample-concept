using System;

namespace MCServiceFake.ConsoleApp.RegisterServices
{
    public class DependencyResolver
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}
