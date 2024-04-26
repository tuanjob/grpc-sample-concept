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


Console.ResetColor();

string clientName = Guid.NewGuid().ToString();// Client contextId
MyConsole.WriteLine($"<<<<<<<<<<<<  CLIENT {clientName} Started  >>>>>>>>>>");


#region Initila connection to SERVER

var target = "http://localhost:50051";

/*
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
httpclient.timeout = timespan.fromseconds(10);
await httpclient.sendasync(new httprequestmessage(httpmethod.head, target)).continuewith(task =>
{
    if (task.iscompletedsuccessfully)
    {
        // connection established successfully
        console.writeline("connection established successfully.");
    }
    else if (task.isfaulted)
    {
        // error occurred while establishing connection
        console.writeline($"error occurred while establishing connection: {task.exception?.message}");
    }
    else if (task.iscanceled)
    {
        // connection attempt was canceled
        console.writeline("connection attempt was canceled.");
    }
});

var channel = GrpcChannel.ForAddress(target, new GrpcChannelOptions { HttpClient = httpClient, DisposeHttpClient = true });

*/

var channel = GrpcChannel.ForAddress(target);
var _client = new DataServiceClient(channel);


#endregion


// Request FULL
await RequestOutrightFullAsync(clientName, CancellationToken.None);

// FULL should be finished first, then Request INC
await SubsribeToGetOutrightIncAsync(clientName, CancellationToken.None);


#region Get Full / Inc Data

async Task RequestOutrightFullAsync(string clientName, CancellationToken cancellationToken)
{
    MyConsole.WriteLine("#1 Request FULL =============");

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
                               MyConsole.WriteLine($"Subscribe Data Server FULL response OFF... Retrying in {delay.TotalSeconds} seconds.");
                               retryCount++;
                           });

    await retryPolicy.ExecuteAsync(async () =>
    {
        if (retryCount == maxRetries)
        {
            MyConsole.WriteLine("Maximum retry attempts reached. Unable to connect to the server.");
        }

        var respnose = _client.RequestOutrightFull(new DataRequest { ClientId = clientName });
        var receivedDataFull = await StreamHelper.DeserializeFromByteStringAsync<List<FullOdds>>(respnose.Data);

        MyConsole.WriteLine("#2 Reponse FULL:");
        MyConsole.WriteLine(ConsoleColor.Green, $"{JsonSerializer.Serialize(receivedDataFull)}");

    });
}

async Task SubsribeToGetOutrightIncAsync(string clientName, CancellationToken cancellationToken)
{
    MyConsole.WriteLine("#3 Request INC =============");
    var retryConnectedToServer = 0;

    // long-running tasks (for Long live Streaming)
    while (!cancellationToken.IsCancellationRequested)
    {
        try
        {
            using (var call = _client.SubscribeToOutrightInc(new DataRequest { ClientId = clientName }))
            {
                await foreach (var updateMessage in call.ResponseStream.ReadAllAsync())
                {
                    var receivedData = await StreamHelper.DeserializeFromByteStringAsync<HDPOUIncOdds>(updateMessage.Data);

                    if (retryConnectedToServer > 0)
                    {
                        MyConsole.WriteLine(ConsoleColor.Yellow, $"Client has been trying to re-connect \"{retryConnectedToServer} TIMEs\". (TODO) Need to check data response for make decission to get FULL data or not?");
                        retryConnectedToServer = 0;
                    }


                    MyConsole.WriteLine("#4 Reponse INC:");
                    MyConsole.WriteLine(ConsoleColor.Green, $" {JsonSerializer.Serialize(receivedData)}");
                }
            }
        }
        catch (RpcException ex)
        {
            MyConsole.WriteLine($"Failed to receive message: {ex.Status.Detail}");
        }

        await Task.Delay(TimeSpan.FromSeconds(1));
        retryConnectedToServer++;
    }
}

#endregion


Console.ReadLine();