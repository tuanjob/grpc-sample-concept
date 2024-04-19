using DataServicePackage;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using gRPCSampleServer.Services;
using System.Threading.Tasks.Dataflow;
using static DataServicePackage.DataService;

// Implement gRPC service in Server1
public class DataServiceImpl : DataServiceBase, IDataServiceInvoker
{
    private static IServerStreamWriter<DataResponse> _responseStream;
    private static Dictionary<string, IServerStreamWriter<DataResponse>> _clientStreams = new Dictionary<string, IServerStreamWriter<DataResponse>>();
    private static Dictionary<string, Queue<DataResponse>> _messageQueues = new Dictionary<string, Queue<DataResponse>>();

    private readonly object _lock = new object();
    private string CLIENTID = Guid.NewGuid().ToString();


    public async override Task SubscribeToUpdates(DataRequest request, IServerStreamWriter<DataResponse> responseStream, ServerCallContext context)
    {
        string clientId = CLIENTID;// context.Peer;

        Console.WriteLine($"Client.... {clientId}");

        lock (_lock)
        {
            _responseStream = responseStream;
            _clientStreams[clientId] = responseStream;
            _messageQueues[clientId] = new Queue<DataResponse>();
        }

        // Send the full data first
        await responseStream.WriteAsync(new DataResponse { Message = "Full Data Response at the first Time"});


        try
        {
            // Keep the call alive or handle incoming data updates, depending on your scenario.
            // For example, listen for cancellation requests:
            while (!context.CancellationToken.IsCancellationRequested)
            {
                // Here, the method can wait for new incremental data to be invoked or just be responsive to cancellation
                await Task.Delay(5000); // This delay is just to prevent a tight loop; adjust according to your needs.
                FlushQueue(clientId, responseStream);
            }
        }
        finally
        {
            // Clean up on disconnect
            lock (_lock)
            {
                _clientStreams.Remove(clientId);
                _messageQueues.Remove(clientId);
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
        var clientId = CLIENTID;
        IServerStreamWriter<DataResponse> localStream;
        lock (_lock)
        {
            _clientStreams.TryGetValue(clientId, out localStream);
        }

        if (localStream != null)
        {
            await localStream.WriteAsync(new DataResponse { Message = incData });
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
                    // Handle cases where there is no queue available, possibly due to a client that never connected or was cleaned up
                    Console.WriteLine("No client connection or queue available for clientId: " + clientId);
                }
            }
        }
    }
}
