using Grpc.Core;
using Polly;
using Polly.Retry;

namespace gRPCSample.Core
{
    public class PollyRetryHelper
    {
        public static AsyncRetryPolicy CreateRetryPolicy(int maxRetries, double backoffFactor, TimeSpan initialDelay, ref int retryCount)
        {
            retryCount = 0; // Initialize retry count

            return Policy.Handle<RpcException>() // Handle RpcException
                         .Or<HttpRequestException>() // Handle HTTP request exceptions
                         .WaitAndRetryAsync(maxRetries, // Maximum number of retries
                                            attempt => CalculateDelay(initialDelay, backoffFactor, attempt), // Exponential backoff
                                            (exception, delay) => // On retry
                                            {
                                                Console.WriteLine($"Retrying in {delay.TotalSeconds} seconds...");
                                                //retryCount++;
                                            });
        }

        private static TimeSpan CalculateDelay(TimeSpan initialDelay, double backoffFactor, int attempt)
        {
            return initialDelay * Math.Pow(backoffFactor, attempt);
        }
    }
}
