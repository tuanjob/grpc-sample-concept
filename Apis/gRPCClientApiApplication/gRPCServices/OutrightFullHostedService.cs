
using gRPCClientApiApplication.Controllers;

namespace gRPCClientApiApplication.gRPCServices
{
    public class OutrightFullHostedService : IHostedService
    {
        private readonly WebApiDataServiceClient _webApiDataServiceClient;
        private readonly ILogger<OutrightFullHostedService> _logger;


        public OutrightFullHostedService(WebApiDataServiceClient webApiDataServiceClient, ILogger<OutrightFullHostedService> logger)
        {
            _webApiDataServiceClient = webApiDataServiceClient;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting gRPC initialization...");

            // Delay initialization to allow other startup tasks to continue
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

            /*
            try
            {
                _logger.LogInformation("RequestOutrightFullAsync Started ..........");
                await _webApiDataServiceClient.RequestOutrightFullAsync("WebApi001", cancellationToken);
                _logger.LogInformation("gRPC server connected successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to connect to gRPC server: {ex.Message}");
                // Handle connection failure gracefully
            }
            */

            // Start gRPC connection in a separate background task
            await Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation("RequestOutrightFullAsync Started ..........");
                    await _webApiDataServiceClient.RequestOutrightFullAsync("WebApi001", cancellationToken);
                    _logger.LogInformation("gRPC server connected successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to connect to gRPC server: {ex.Message}");
                    // Handle connection failure gracefully
                }
            }, cancellationToken);



            // TODO: INC request
            //await _webApiDataServiceClient.SubsribeToGetOutrightIncAsync("WebApi001", cancellationToken);
            //_logger.LogInformation("SubsribeToGetOutrightIncAsync has been triggered, and waiting for Server Streaming whenever have new data");

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Cleanup or shutdown logic
            // Task.CompletedTask;
        }
    }
}
