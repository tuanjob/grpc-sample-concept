using gRPCSample.Core.Helpers;
using gRPCSample.Core.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace gRPCSampleClient1.Processors
{
    public interface IOutrightDataProcessor
    {
        void StartProcessing(CancellationToken cancellationToken);
        void EnqueueData(HDPOUIncOdds data);
    }

    public class OutrightDataProcessor : IOutrightDataProcessor
    {
        private readonly ILogger<OutrightDataProcessor> _logger;
        private ConcurrentQueue<HDPOUIncOdds> _queue;
        private readonly IRedisDatabase _redisDatabase;
        private CancellationToken _cancellationToken;

        public OutrightDataProcessor(IRedisDatabase redisDatabase, ILogger<OutrightDataProcessor> logger)
        {
            _redisDatabase = redisDatabase;
            _queue = new ConcurrentQueue<HDPOUIncOdds>();
            _logger = logger;
        }

        public void EnqueueData(HDPOUIncOdds data)
        {
            MyConsole.WriteLine(color: ConsoleColor.Blue, "2================ [EnqueueData] Add data Inc to Queue which is called from gRPC or other services ...");
            //foreach (var item in data)
            //{
                _queue.Enqueue(data);
            //}
        }

        public void StartProcessing(CancellationToken cancellationToken)
        {
            MyConsole.WriteLine(color: ConsoleColor.Blue, "================ [AppStart] StartProcessing queue ...");
            _cancellationToken = cancellationToken;
            Task.Run(async () => await ProcessQueue());
        }

        private async Task ProcessQueue()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out HDPOUIncOdds item))
                {
                    // Process and save to Redis
                    _redisDatabase.Set($"OutrightInc:{item.FTSocOddsId}");
                    _logger.LogInformation($"Processed and saved item with ID: {item.FTSocOddsId}");
                }
                else
                {
                    await Task.Delay(100);  // Pause when queue is empty
                }
            }
        }
    }

}
