using Microsoft.Extensions.Configuration;

namespace gRPCSample.Core.Configurations
{
    public class ConfigurationManager
    {
        private readonly IConfigurationRoot _configuration;

        public ConfigurationManager()
        {
            _configuration = new ConfigurationLoader().Configuration;
        }

        public T GetConfigurationValue<T>(string key)
        {
            // This will handle simple types, lists, and complex objects
            return _configuration.GetValue<T>(key);
        }

        public T GetSection<T>(string section) where T : new()
        {
            var model = new T();
            _configuration.GetSection(section).Bind(model);
            return model;
        }
    }

}
