using Microsoft.Extensions.Configuration;

namespace gRPCSample.Core.Configurations
{
    public class ConfigurationLoader
    {
        public IConfigurationRoot Configuration { get; }

        public ConfigurationLoader()
        {
            // Setup the configuration builder
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Ensure the path is correct
                .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }

}