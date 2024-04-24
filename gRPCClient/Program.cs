// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");
// Implement gRPC client logic in Client1
using DataServicePackage;
using Grpc.Core;
using Grpc.Net.Client;
using gRPCSample.Core.Helpers;
using gRPCSample.Core.Models;
using System.Collections.Concurrent;
using System.Text.Json;
using static DataServicePackage.DataService;


const string clientName = "MCOutright001"; // => It should be a Client that allowed in gRPC Server
Console.WriteLine($"<<<<<<<<<<<<  CLIENT {clientName} Started  >>>>>>>>>>");

var channel = GrpcChannel.ForAddress("http://localhost:50051");
var _client = new DataServiceClient(channel);

// Which is latest TimeIndex from succeed data before ....
ConcurrentDictionary<string, long> currentTimeIndex = new ConcurrentDictionary<string, long>();



// Request FULL
await RequestOutrightFullAsync(clientName);

// FULL should be finished first, then Request INC
await SubsribeToGetOutrightIncAsync(clientName);

#region Get Full / Inc Data

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
                    var receivedDataFull = await StreamHelper.DeserializeFromByteStringAsync<List<FullOdds>>(updateMessageFull.Data);


                    Console.WriteLine($"#2 Reponse FULL:");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($" {JsonSerializer.Serialize(receivedDataFull)}");
                    Console.ResetColor();

                    // TODO: Process data into JSON FULL and then return TimeIndex......

                    // TODO: SET current TimeIndex
                    currentTimeIndex[clientName] = 1; // TODO: will get from service ....
                    Console.WriteLine($"1- Current TimeIndex: {currentTimeIndex[clientName]}");
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
                Console.WriteLine("CONNECTED .............");
                await foreach (var updateMessage in call.ResponseStream.ReadAllAsync())
                {
                    var receivedData = await StreamHelper.DeserializeFromByteStringAsync<HDPOUIncOdds>(updateMessage.Data);

                    Console.WriteLine($"#4 Reponse INC:");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($" {JsonSerializer.Serialize(receivedData)}");
                    Console.ResetColor();

                    // TODO: Process data into JSON Incremental and then return TimeIndex......

                    //Console.WriteLine($"==> Current TimeIndex: {currentTimeIndex[clientName]} NEW => {receivedData.TimeIndex}");
                    //if(receivedData.TimeIndex == currentTimeIndex[clientName]  // at the first time
                    //|| receivedData.TimeIndex == currentTimeIndex[clientName] + 1 
                    //    )
                    //{
                    //   currentTimeIndex[clientName] = receivedData.TimeIndex;
                    //   Console.WriteLine($"OK .... Continue .... with Current TimeIndex: {currentTimeIndex[clientName]}");
                    //}
                    //else
                    //{
                    //    Console.WriteLine("BREAK this and call FULL again PLEASE....");
                    //    await RequestOutrightFullAsync(clientName);
                    //}
                }

                Console.WriteLine("CONNECTED .............");
                break; // Break the loop if the connection was successful and completed without interruption
            }
        }
        catch (RpcException ex)
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