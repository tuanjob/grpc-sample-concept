using Google.Protobuf;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace NXTK.GrpcServer.Helpers
{
    public static class StreamHelper
    {
        public static ByteString SerializeToByteString<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                JsonSerializer.Serialize(ms, obj);
                return ByteString.CopyFrom(ms.ToArray());
            }
        }

        public static T DeserializeFromByteString<T>(ByteString bytes)
        {
            using (var ms = new MemoryStream(bytes.ToByteArray()))
            {
                return JsonSerializer.Deserialize<T>(ms);
            }
        }

        public static async Task<ByteString> SerializeToByteStringAsync<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(ms, obj);
                return ByteString.CopyFrom(ms.ToArray());
            }
        }

        public static async Task<T> DeserializeFromByteStringAsync<T>(ByteString bytes)
        {
            using (var ms = new MemoryStream(bytes.ToByteArray()))
            {
                return await JsonSerializer.DeserializeAsync<T>(ms);
            }
        }
    }
}
