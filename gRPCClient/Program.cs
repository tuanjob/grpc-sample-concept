// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");
// Implement gRPC client logic in Client1
using DataServicePackage;
using Grpc.Core;
using Grpc.Net.Client;
using gRPCSample.Core.Helpers;
using gRPCSample.Core.Models;
using Polly;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.Json;
using static DataServicePackage.DataService;


const string clientName = "MCOutright001"; // => It should be a Client that allowed in gRPC Server
Console.WriteLine($"<<<<<<<<<<<<  CLIENT {clientName} Started  >>>>>>>>>>");


#region Initila connection to SERVER

var target = "http://localhost:50051";
// For .net 5+, .net core 3+
var handler = new SocketsHttpHandler
{
    // Configure keepalive settings
    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
    KeepAlivePingDelay = TimeSpan.FromSeconds(30),
    KeepAlivePingTimeout = TimeSpan.FromSeconds(15),
    KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
};

var httpClient = new HttpClient(handler);
// Log connection-related events
//httpClient.Timeout = TimeSpan.FromSeconds(10);
//await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, target)).ContinueWith(task =>
//{
//    if (task.IsCompletedSuccessfully)
//    {
//        // Connection established successfully
//        Console.WriteLine("Connection established successfully.");
//    }
//    else if (task.IsFaulted)
//    {
//        // Error occurred while establishing connection
//        Console.WriteLine($"Error occurred while establishing connection: {task.Exception?.Message}");
//    }
//    else if (task.IsCanceled)
//    {
//        // Connection attempt was canceled
//        Console.WriteLine("Connection attempt was canceled.");
//    }
//});

// var channel = GrpcChannel.ForAddress(target, new GrpcChannelOptions { HttpClient = httpClient, DisposeHttpClient = true });
var channel = GrpcChannel.ForAddress(target);
var _client = new DataServiceClient(channel);


#endregion

await SubsribeToGetOutrightIncAsync1(clientName);
async Task SubsribeToGetOutrightIncAsync1(string clientName)
{
    Console.WriteLine("#3 Request INC =============");

    while (true)
    {
        try
        {
            using (var call = _client.SubscribeToOutrightInc(new DataRequest { ClientId = clientName }))
            {
                Console.WriteLine($"CALLING .............: {call.GetType()}");
                await foreach (var response in call.ResponseStream.ReadAllAsync())
                {
                    var receivedData = await StreamHelper.DeserializeFromByteStringAsync<HDPOUIncOdds>(response.Data);

                    Console.WriteLine($"Client ID {clientName} got response: {receivedData.FTSocOddsId}");
                }
            }
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"Failed to receive message: {ex.Status.Detail}");
        }

        await Task.Delay(TimeSpan.FromSeconds(10));
    }
}



// Request FULL
// await RequestOutrightFullAsync(clientName);

// FULL should be finished first, then Request INC
// await SubsribeToGetOutrightIncAsync(clientName);


#region Get Full / Inc Data

async Task RequestOutrightFullAsync(string clientName)
{
    Console.WriteLine("#1 Request FULL =============");

    const int maxRetries = 15;  // Maximum number of retries
    int retryCount = 0; // Counter for tracking retries
    double backoffFactor = 2.0;
    TimeSpan initialDelay = TimeSpan.FromSeconds(1);

    var retryPolicy = Policy
        .Handle<RpcException>() // Handle RpcException
        .Or<HttpRequestException>() // Handle HTTP request exceptions
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

    const int maxRetries = 3;  // Maximum number of retries
    double backoffFactor = 2.0;
    int retryCount = 0; // Counter for tracking retries
    TimeSpan initialDelay = TimeSpan.FromSeconds(1);

    var retryPolicy = Policy
        .Handle<RpcException>() // Handle RpcException
        // .Or<HttpRequestException>() // Handle HTTP request exceptions
        .WaitAndRetryAsync(maxRetries, // Maximum number of retries
                           attempt => initialDelay * Math.Pow(backoffFactor, attempt), // Exponential backoff
                           (exception, delay) => // On retry
                           {
                               Console.WriteLine($"INC Subscribe Data Server response OFF... Retrying in {delay.TotalSeconds} seconds.");
                               retryCount++;
                           });



    await retryPolicy.ExecuteAsync(async () =>
    {
        if (retryCount == maxRetries)
        {
            Console.WriteLine("Maximum retry attempts reached. Unable to connect to the server.");
            // TODO: need to have another action to cover this case ...
            return;
        }

        Console.WriteLine($"=====> {retryCount}");

        if(retryCount > 0)
        {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"SHOULD call the process reConnection.......");
                    Console.ResetColor();
        }

        //try
        //{
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
        //}
        //catch (RpcException ex)
        //{
        //    Console.ForegroundColor = ConsoleColor.Red;
        //    Console.WriteLine("Error connected to server ....");
        //    Console.ResetColor();
        //}
        
    });
    
}

#endregion


Console.ReadLine();