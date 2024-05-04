using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;

namespace gRPCSample.Core.Helpers
{
    public class JsonRepository
    {
        private readonly string _filePath;

        public JsonRepository(string filePath)
        {
            _filePath = filePath;
        }

        public T Get<T>()
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"File {_filePath} not found.");
            }

            string jsonData = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<T>(jsonData);
        }

        public void Set<T>(T data)
        {
            string jsonData = JsonSerializer.Serialize<T>(data);
            File.WriteAllText(_filePath, jsonData);
        }
    }
}
