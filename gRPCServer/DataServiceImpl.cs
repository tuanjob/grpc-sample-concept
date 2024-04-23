using DataServicePackage;
using Grpc.Core;
using gRPCSample.Core.Helpers;
using gRPCSample.Core.Models;
using gRPCSampleServer.Services;
using System.Collections.Concurrent;
using static DataServicePackage.DataService;

// Implement gRPC service in Server1
public class DataServiceImpl : DataServiceBase, IDataServiceInvoker
{
    private static ConcurrentDictionary<string, IServerStreamWriter<DataResponse>> _clientStreams = new ConcurrentDictionary<string, IServerStreamWriter<DataResponse>>();
    private static ConcurrentDictionary<string, Queue<DataResponse>> _messageQueues = new ConcurrentDictionary<string, Queue<DataResponse>>();
    private readonly object _lock = new object();


    /// <summary>
    /// Handle request FULL and prepare FullData response to Client
    /// </summary>
    /// <param name="request"></param>
    /// <param name="responseStream"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async override Task RequestOutrightFull(DataRequest request, IServerStreamWriter<DataResponse> responseStream, ServerCallContext context)
    {
        Console.WriteLine($"[FULL][Client \"{request.ClientId}\" is connected.]");

        // #1 FULL DATA at the initial connection
        var jsonFull = new JsonFullModel { MatchId = 1, SportType = "S", TimeIndex = 1 };
        var dataStream = await StreamHelper.SerializeToByteStringAsync(jsonFull);

        // Send the full data first
        await responseStream.WriteAsync(new DataResponse { Data = dataStream });
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
            _clientStreams[request.ClientId] = responseStream;
            if (!_messageQueues.TryGetValue(request.ClientId, out _))
            {
                _messageQueues[request.ClientId] = new Queue<DataResponse>();
            }
        }


        //  #WAITING for incomming INC data and send to Client by calling InvokeGetIncrementalData
        try
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(5000);
                FlushQueue(request.ClientId, responseStream);
            }
        }
       
        finally
        {
            Console.WriteLine($"=====> [Finally] Client {request.ClientId} Disconnected at {DateTime.Now}");

            // Clean up on disconnect
            lock (_lock)
            {
                _clientStreams.TryRemove(request.ClientId, out _);

                // TODO: should we remove frmo Queue or not?
                // _messageQueues.Remove(request.ClientId);
            }
        }
    }

    #region Private Methods


    private void FlushQueue(string clientId, IServerStreamWriter<DataResponse> stream)
    {
        lock (_lock)
        {
            if (_messageQueues.TryGetValue(clientId, out Queue<DataResponse> queue))
            {
                while (queue.Count > 0)
                {
                    var message = queue.Dequeue();
                    stream.WriteAsync(message).Wait(); // Consider the implications of using Wait() here
                }
            }
        }
    }

    /// <summary>
    /// Send to all active clients
    /// </summary>
    /// <param name="incData"></param>
    /// <returns></returns>
    public async Task InvokeSendIncrementalData(JsonIncModel jsonInc)
    {
        // Get all clientIds
        var clientIds = _clientStreams.Keys;
        foreach(var clientId in clientIds)
        {
            await SendData(clientId, jsonInc);
        }
    }

    /// <summary>
    /// Send to specific client
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="incData"></param>
    /// <returns></returns>
    public async Task InvokeSendIncrementalData(string clientId, JsonIncModel jsonInc)
    {
        await SendData(clientId, jsonInc);
    }

    private async Task SendData(string clientId, JsonIncModel jsonInc)
    {
        IServerStreamWriter<DataResponse> localStream;
        lock (_lock)
        {
            if (!_clientStreams.TryGetValue(clientId, out localStream))
            {
                Console.WriteLine($"[{clientId}] doesnot exist in Stream Response.");
            }
        }

        if (localStream != null)
        {
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
                // Optionally remove the client from the dictionary
                _clientStreams.TryRemove(clientId, out _);
                Console.WriteLine($"[Exception] Failed to send message to client {clientId}: {ex.Message}. Removed.");
            }
            finally
            {
                Console.WriteLine($"[Finally] Send data to [{clientId}]");
            }
        }
        else
        {
            lock (_lock)
            {
                jsonInc.Message += " => from [QUEUE]";
                var dataStream = StreamHelper.SerializeToByteString(jsonInc);

                if (_messageQueues.TryGetValue(clientId, out Queue<DataResponse> queue))
                {
                    queue.Enqueue(new DataResponse { Data = dataStream });
                }
                else
                {
                    _messageQueues[clientId] = new Queue<DataResponse>();
                    _messageQueues[clientId].Enqueue(new DataResponse { Data = dataStream });
                }

                Console.WriteLine($"Added clientId \"{clientId}\" into message Queues ");
            }
        }
    }

    #endregion

}
