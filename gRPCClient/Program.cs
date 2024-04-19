// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");
// Implement gRPC client logic in Client1
using DataServicePackage;
using Grpc.Core;
using Grpc.Net.Client;
using static DataServicePackage.DataService;


Console.WriteLine("======= CLIENT Started =============");

var channel = GrpcChannel.ForAddress("http://localhost:50051");
var _client = new DataServiceClient(channel);

const string clientName = "MCOutright001";

#region Subscribe Data

/*
try
{
    using (var call = _client.SubscribeToUpdates(new DataRequest { ClientId = clientName, Name = $"Client1 Req"  }))
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
*/

await ConnectAndListenAsync(clientName);

async Task ConnectAndListenAsync(string clientName)
{
    const int maxRetries = 5;  // Maximum number of retries
    int retryCount = 0;
    double backoffFactor = 2.0;
    TimeSpan initialDelay = TimeSpan.FromSeconds(1);

    while (retryCount < maxRetries)
    {
        try
        {
            using (var call = _client.SubscribeToUpdates(new DataRequest { ClientId = clientName, Name = $"Client1 Req" }))
            {
                await foreach (var updateMessage in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine($"Message Response from Server: {updateMessage.Message}");
                }
                break; // Break the loop if the connection was successful and completed without interruption
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Attempt {retryCount + 1}: Subscribe Data Server response OFF... Retrying in {initialDelay.TotalSeconds} seconds.");
            await Task.Delay(initialDelay);
            initialDelay *= backoffFactor;  // Increase the delay for the next retry
            retryCount++;
        }
    }

    if (retryCount == maxRetries)
    {
        Console.WriteLine("Maximum retry attempts reached. Unable to connect to the server.");
    }
}

#endregion


Console.ReadLine();