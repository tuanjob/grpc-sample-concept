using DataServicePackage;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using gRPCSampleServer.Services;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;
using static DataServicePackage.DataService;

// Implement gRPC service in Server1
public class DataServiceImpl : DataServiceBase, IDataServiceInvoker
{
    private static Dictionary<string, IServerStreamWriter<DataResponse>> _clientStreams = new Dictionary<string, IServerStreamWriter<DataResponse>>();
    private static Dictionary<string, Queue<DataResponse>> _messageQueues = new Dictionary<string, Queue<DataResponse>>();

    private readonly object _lock = new object();


    public async override Task SubscribeToUpdates(DataRequest request, IServerStreamWriter<DataResponse> responseStream, ServerCallContext context)
    {
        Console.WriteLine($"Client \"{request.ClientId}\" is connected.");

        lock (_lock)
        {
            _clientStreams[request.ClientId] = responseStream;
            if (!_messageQueues.TryGetValue(request.ClientId, out _))
            {
                _messageQueues[request.ClientId] = new Queue<DataResponse>();
            }
        }

        // #1 FULL DATA at the initial connection
        // TODO: Call service to get (Prepare) FULL DATA

        // Send the full data first
        await responseStream.WriteAsync(new DataResponse { Message = $"Full Data Response To {request.ClientId}"});


        // #2 WAITING for incomming INC data and send to Client by calling InvokeGetIncrementalData1
        try
        {
            // Keep the call alive or handle incoming data updates, depending on your scenario.
            // For example, listen for cancellation requests:
            while (!context.CancellationToken.IsCancellationRequested)
            {
                // Here, the method can wait for new incremental data to be invoked or just be responsive to cancellation
                await Task.Delay(5000); // This delay is just to prevent a tight loop; adjust according to your needs.
                FlushQueue(request.ClientId, responseStream);
            }
        }
       
        finally
        {
            // Clean up on disconnect
            lock (_lock)
            {
                _clientStreams.Remove(request.ClientId);
                _messageQueues.Remove(request.ClientId);
            }
        }
    }

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

    public async Task InvokeGetIncrementalData1(string incData)
    {
        // TODO
    }

    public async Task InvokeGetIncrementalData1(string clientId, string incData)
    {
        await SendData(clientId, incData);
    }

    private async Task SendData(string clientId, string incData)
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
                await localStream.WriteAsync(new DataResponse { Message = $"[CLIENT]: {clientId}, [DATA]: {incData}" });
            }
            catch (RpcException ex)// when (ex.StatusCode == StatusCode.Cancelled)
            {
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
                _clientStreams.Remove(clientId);
                Console.WriteLine($"Failed to send message to client {clientId}: {ex.Message}. Removed.");
            }
            finally
            {
                Console.WriteLine($"Finally send data to [{clientId}]");
            }
        }
        else
        {
            lock (_lock)
            {
                if (_messageQueues.TryGetValue(clientId, out Queue<DataResponse> queue))
                {
                    queue.Enqueue(new DataResponse { Message = incData });
                }
                else
                {
                    _messageQueues[clientId] = new Queue<DataResponse>();
                    _messageQueues[clientId].Enqueue(new DataResponse { Message = $"[Added to Queue] Data to client {clientId}, data: {incData}" });
                    Console.WriteLine($"Add clientId \"{clientId}\" into _messageQueues ");
                }
            }
        }
    }
}
