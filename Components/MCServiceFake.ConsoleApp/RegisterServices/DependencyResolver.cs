using Autofac;
using System;

namespace MCServiceFake.ConsoleApp.RegisterServices
{
    public class DependencyResolver
    {
        public static IContainer Container { get; private set; }

        public static void Initialize(IContainer container)
        {
            Container = container;
        }
    }
}
