using NLog;
using NLog.Extensions.Logging;
using System.Runtime.InteropServices;

namespace gRPCClientApiApplication.ServiceCollections
{
    public static class LoggerCollection
    {
        public static IServiceCollection AddLogger(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure NLog
            LogManager.Configuration = new NLogLoggingConfiguration(configuration.GetSection("NLog"));

            if (OS.IsLinux())
            {
                // If this is running under Linux, store the logs in /var/log
                LogManager.Configuration.Variables["basedir"] = "/var/log/MerchantManager";
            }
            else if (OS.IsWindows())
            {
                // If this is running under Windows, store the logs in /logs
                var basedir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                LogManager.Configuration.Variables["basedir"] = basedir;
            }

            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddNLog();
            });

            return services;
        }

        private static class OS
        {
            public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }
    }
}
