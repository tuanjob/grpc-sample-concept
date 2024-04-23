// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");
// Implement gRPC client logic in Client1
using DataServicePackage;
using Grpc.Core;
using Grpc.Net.Client;
using gRPCSample.Core.Helpers;
using gRPCSample.Core.Models;
using System.Text.Json;
using static DataServicePackage.DataService;


const string clientName = "MCOutright002"; // => It should be a Client that allowed in gRPC Server
Console.WriteLine($"<<<<<<<<<<<<  CLIENT {clientName} Started  >>>>>>>>>>");

var channel = GrpcChannel.ForAddress("http://localhost:50051");
var _client = new DataServiceClient(channel);


await RequestOutrightFullAsync(clientName);

await SubsribeToGetOutrightIncAsync(clientName);

#region Subscribe Data

async Task RequestOutrightFullAsync(string clientName)
{
    Console.WriteLine($"#1 Request FULL =============");

    const int maxRetries = 5;  // Maximum number of retries
    int retryCount = 0;
    double backoffFactor = 2.0;
    TimeSpan initialDelay = TimeSpan.FromSeconds(1);

    while (retryCount < maxRetries)
    {
        try
        {
            using (var call = _client.RequestOutrightFull(new DataRequest { ClientId = clientName }))
            {
                await foreach (var updateMessageFull in call.ResponseStream.ReadAllAsync())
                {
                    var receivedDataFull = await StreamHelper.DeserializeFromByteStringAsync<JsonFullModel>(updateMessageFull.Data);
                    Console.WriteLine($"#2 Response FULL: {JsonSerializer.Serialize(receivedDataFull)}");
                }
                break;
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


async Task SubsribeToGetOutrightIncAsync(string clientName)
{

    Console.WriteLine($"#3 Request INC =============");

    const int maxRetries = 5;  // Maximum number of retries
    int retryCount = 0;
    double backoffFactor = 2.0;
    TimeSpan initialDelay = TimeSpan.FromSeconds(1);

    while (retryCount < maxRetries)
    {
        try
        {
            using (var call = _client.SubscribeToOutrightInc(new DataRequest { ClientId = clientName }))
            {

                await foreach (var updateMessage in call.ResponseStream.ReadAllAsync())
                {
                    var receivedData = await StreamHelper.DeserializeFromByteStringAsync<JsonIncModel>(updateMessage.Data);
                    Console.WriteLine($"#4 Reponse INC: {JsonSerializer.Serialize(receivedData)}");
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