using DataServicePackage;
using Grpc.Core;
using NXTK.GrpcServer.Helpers;
using NXTK.GrpcServer.Interfaces;
using NXTK.GrpcServer.Models;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using static DataServicePackage.DataService;

namespace NXTK.GrpcServer.Services
{
    public class GrpcDataService : DataServiceBase, IOutrightIncDataServiceInvoker
    {
        private static ConcurrentDictionary<string, ClientStreamData> _clientStreams = new ConcurrentDictionary<string, ClientStreamData>();
        // private static ConcurrentDictionary<string, Queue<DataResponse>> _messageQueues = new ConcurrentDictionary<string, Queue<DataResponse>>();
        private readonly object _lock = new object();


        private readonly IOutrightFullDataServiceInvoker _externalDataService;
        // private readonly ILogger<GrpcDataService> _logger; // TODO
        public GrpcDataService(IOutrightFullDataServiceInvoker externalDataService)
        {
            _externalDataService = externalDataService;
        }


        /// <summary>
        /// Handle request FULL and prepare FullData response to Client
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<DataResponse> RequestOutrightFull(DataRequest request, ServerCallContext context)
        {
            Console.WriteLine($"[FULL][Client \"{request.ClientId}\" is connected.]");

            var outrightFullData = _externalDataService.GetOutrightFullData();

            // #1 FULL DATA at the initial connection
            var dataStream = await StreamHelper.SerializeToByteStringAsync(outrightFullData);

            // Reset MessageQueues (TODO)
            // InitialMessageQueues(request.ClientId);

            Console.WriteLine($"[FULL][Client \"{request.ClientId}\" is done for GETTING DATA.]");

            return new DataResponse { Data = dataStream };
        }


        /// <summary>
        /// Server Streaming, process data INC and Send to Client whenever has new data
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task SubscribeToOutrightInc(DataRequest request, IServerStreamWriter<DataResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine($"[INC][Client \"{request.ClientId}\" is connected.]");

            lock (_lock)
            {
                // REGISTER client for Server Streaming ...
                _clientStreams[request.ClientId] = new ClientStreamData(responseStream);

                // REGISTER client for add Queue Messages (use when the client is lost connection to server )
                // InitialMessageQueues(request.ClientId);
            }

            //  #WAITING for incomming INC data and send to Client by calling InvokeGetIncrementalData
            try
            {
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    // await Task.Delay(3000);
                    // FlushQueue(request.ClientId, responseStream);
                }
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"[INC] Exception... => {ex.GetType().Name}");
            }


            // Register a callback when the client disconnects
            context.CancellationToken.Register(() =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"=====> [INC] Client {request.ClientId} Disconnected at {DateTime.Now}");
                Console.ResetColor();

                lock (_lock)
                {
                    // Update status Connection ( Disconnected = true )
                    if (_clientStreams.TryGetValue(request.ClientId, out ClientStreamData client))
                    {
                        client.DisConnected = true;

                        _clientStreams[request.ClientId] = client;
                    }
                }
            });
        }

        #region Private Methods

        

        /// <summary>
        /// Send to all active clients
        /// </summary>
        /// <param name="incData"></param>
        /// <returns></returns>
        public async Task InvokeSendIncrementalData(HDPOUIncOdds jsonInc)
        {
            // Get all clientIds ( allowed clients )
            var clientIds = _clientStreams.Keys;
            foreach (var clientId in clientIds)
            {
                await SendData(clientId, jsonInc);
            }
        }

        private async Task SendData(string clientId, HDPOUIncOdds jsonInc)
        {
            IServerStreamWriter<DataResponse> localStream;
            ClientStreamData clientStreamData;

            lock (_lock)
            {
                if (!_clientStreams.TryGetValue(clientId, out clientStreamData))
                {
                    Console.WriteLine($"[{clientId}] doesnot exist in Stream Response.");
                }
            }

            if (clientStreamData != null && !clientStreamData.DisConnected)
            {
                localStream = clientStreamData.ServerStreamWriter;
                try
                {
                    var dataStream = await StreamHelper.SerializeToByteStringAsync(jsonInc);

                    await localStream.WriteAsync(new DataResponse { Data = dataStream });
                }
                catch (RpcException ex)
                {
                    Console.WriteLine($"[RpcException] Failed to send message to client {clientId}.");
                    // TODO #1 
                    switch (ex.StatusCode)
                    {
                        case StatusCode.Cancelled:
                            Console.WriteLine($"Client \"{clientId}\" cancelled the connection");
                            break;
                        case StatusCode.DeadlineExceeded:
                            Console.WriteLine($"Client \"{clientId}\" The call deadline was exceeded");
                            break;
                        case StatusCode.Unavailable:
                            Console.WriteLine($"Client \"{clientId}\" The service is currently unavailable");
                            break;
                        case StatusCode.Unknown:
                            Console.WriteLine($"Client \"{clientId}\" An unknown error occurred");
                            break;
                        case StatusCode.Internal:
                            Console.WriteLine($"Client \"{clientId}\" Internal server error");
                            break;
                        default:
                            Console.WriteLine($"An unexpected error occurred: {ex.StatusCode}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Exception] Failed to send message to client {clientId}: {ex.Message}. Removed.");
                }
                finally
                {
                    // Console.WriteLine($"[Finally] Send data to [{clientId}]");
                }
            }
            else if (clientStreamData != null && clientStreamData.DisConnected)
            {
                Console.WriteLine($"The clientId {clientId} lost connection already......");

                return;
            }
        }

        #endregion

    }

}
