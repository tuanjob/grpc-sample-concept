// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");
// Implement gRPC client logic in Client1
using DataServicePackage;
using Grpc.Core;
using Grpc.Net.Client;
using gRPCSample.Core.Helpers;
using gRPCSample.Core.Models;
using Polly;
using System.Text.Json;
using static DataServicePackage.DataService;


const string clientName = "MCOutright002"; // => It should be a Client that allowed in gRPC Server
Console.WriteLine($"<<<<<<<<<<<<  CLIENT {clientName} Started  >>>>>>>>>>");

var channel = GrpcChannel.ForAddress("http://localhost:50051");
var _client = new DataServiceClient(channel);

// Request FULL
await RequestOutrightFullAsync(clientName);

// FULL should be finished first, then Request INC
await SubsribeToGetOutrightIncAsync(clientName);


#region Get Full / Inc Data

async Task RequestOutrightFullAsync(string clientName)
{
    Console.WriteLine("#1 Request FULL =============");

    const int maxRetries = 5;  // Maximum number of retries
    int retryCount = 0; // Counter for tracking retries
    double backoffFactor = 2.0;
    TimeSpan initialDelay = TimeSpan.FromSeconds(1);

    var retryPolicy = Policy
        .Handle<RpcException>() // Handle RpcException
        .WaitAndRetryAsync(maxRetries, // Maximum number of retries
                           attempt => initialDelay * Math.Pow(backoffFactor, attempt), // Exponential backoff
                           (exception, delay) => // On retry
                           {
                               Console.WriteLine($"Subscribe Data Server FULL response OFF... Retrying in {delay.TotalSeconds} seconds.");
                               retryCount++;
                           });

    await retryPolicy.ExecuteAsync(async () =>
    {
        if (retryCount == maxRetries)
        {
            Console.WriteLine("Maximum retry attempts reached. Unable to connect to the server.");
        }

        var respnose = _client.RequestOutrightFull(new DataRequest { ClientId = clientName });
        var receivedDataFull = await StreamHelper.DeserializeFromByteStringAsync<List<FullOdds>>(respnose.Data);

        Console.WriteLine("#2 Reponse FULL:");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($" {JsonSerializer.Serialize(receivedDataFull)}");
        Console.ResetColor();

    });
}

async Task SubsribeToGetOutrightIncAsync(string clientName)
{
    Console.WriteLine("#3 Request INC =============");

    const int maxRetries = 5;  // Maximum number of retries
    double backoffFactor = 2.0;
    int retryCount = 0; // Counter for tracking retries
    TimeSpan initialDelay = TimeSpan.FromSeconds(1);


    var retryPolicy = Policy
        .Handle<RpcException>() // Handle RpcException
        .WaitAndRetryAsync(maxRetries, // Maximum number of retries
                           attempt => initialDelay * Math.Pow(backoffFactor, attempt), // Exponential backoff
                           (exception, delay) => // On retry
                           {
                               Console.WriteLine($"Subscribe Data Server response OFF... Retrying in {delay.TotalSeconds} seconds.");
                               retryCount++;
                           });

    await retryPolicy.ExecuteAsync(async () =>
    {
        if (retryCount == maxRetries)
        {
            Console.WriteLine("Maximum retry attempts reached. Unable to connect to the server.");
        }

        using (var call = _client.SubscribeToOutrightInc(new DataRequest { ClientId = clientName }))
        {
            await foreach (var updateMessage in call.ResponseStream.ReadAllAsync())
            {
                var receivedData = await StreamHelper.DeserializeFromByteStringAsync<HDPOUIncOdds>(updateMessage.Data);

                Console.WriteLine("#4 Reponse INC:");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($" {JsonSerializer.Serialize(receivedData)}");
                Console.ResetColor();
            }
        }
    });
}

#endregion


Console.ReadLine();