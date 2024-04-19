// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");
// Implement gRPC client logic in Client1
using DataServicePackage;
using Grpc.Core;
using Grpc.Net.Client;
using static DataServicePackage.DataService;


Console.WriteLine("Started ............");

var channel = GrpcChannel.ForAddress("http://localhost:50051");
var _client = new DataServiceClient(channel);

#region Subscribe Data


Console.WriteLine("=======CLIENT Started ............");
try
{
    using (var call = _client.SubscribeToUpdates(new DataRequest()))
    {
        await foreach (var updateMessage in call.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine($"Message Response from Server: {updateMessage.Message}");
            // Process each incoming message
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Subscribe Data Server response OFFFFF ...");
}


#endregion


Console.ReadLine();