using DataServicePackage;
using Grpc.Core;
using Grpc.Net.Client;
using gRPCSample.Core.Helpers;
using gRPCSample.Core.Models;
using Polly;
using System.Text.Json;

namespace MCOutrightAsGrpcClientFakeApp.gRPCServices
{
    public class WebApiDataServiceClient
    {
        private readonly DataServicePackage.DataService.DataServiceClient _client;

        public WebApiDataServiceClient()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:50051");
            _client = new DataServicePackage.DataService.DataServiceClient(channel);
        }


        public async Task RequestOutrightFullAsync(string clientName, CancellationToken cancellationToken)
        {

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
                                       // MyConsole.WriteLine($"Subscribe Data Server FULL response OFF... Retrying in {delay.TotalSeconds} seconds.");
                                       retryCount++;
                                   });

            await retryPolicy.ExecuteAsync(async () =>
            {
                if (retryCount == maxRetries)
                {
                    // MyConsole.WriteLine("Maximum retry attempts reached. Unable to connect to the server.");
                }

                var respnose = await _client.RequestOutrightFullAsync(new DataRequest { ClientId = clientName });
                var receivedDataFull = await StreamHelper.DeserializeFromByteStringAsync<List<FullOdds>>(respnose.Data);

                MyConsole.WriteLine("#2 Reponse FULL:");
                MyConsole.WriteLine(ConsoleColor.Green, $"{JsonSerializer.Serialize(receivedDataFull)}");

            });
        }

        public async Task SubsribeToGetOutrightIncAsync(string clientName, CancellationToken cancellationToken)
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

                                //TODO:  Maybe we need to request full data first.
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
    }
}
