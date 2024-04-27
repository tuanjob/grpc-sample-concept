using gRPCSample.Core.Helpers;
using System.Text.Json;

namespace gRPCSampleClient1.Processors
{
    public interface IRedisDatabase
    {
        void Set<T>(T data);
    }

    public class RedisDatabase : IRedisDatabase
    {
        public RedisDatabase()
        {
            
        }

        public void Set<T>(T data)
        {
            MyConsole.WriteLine(ConsoleColor.Blue, $"AT REDIS: {JsonSerializer.Serialize(data)}");
            //throw new NotImplementedException();
        }
    }
}
