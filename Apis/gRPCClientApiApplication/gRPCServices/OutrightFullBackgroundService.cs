
namespace gRPCClientApiApplication.gRPCServices
{
    public class OutrightFullBackgroundService : BackgroundService
    {
        private readonly WebApiDataServiceClient _webApiDataServiceClient;
        private readonly ILogger<OutrightFullBackgroundService> _logger;
        public OutrightFullBackgroundService(ILogger<OutrightFullBackgroundService> logger, WebApiDataServiceClient webApiDataServiceClient)
        {
            _logger = logger;
            _webApiDataServiceClient = webApiDataServiceClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await InitializeAsync(stoppingToken);
                // Perform background task logic here
                // await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Example delay
                stoppingToken.WaitHandle.WaitOne();
            }
        }

        async Task InitializeAsync(CancellationToken token)
        {
            // Get the MyGrpcClient instance from the service provider

                _logger.LogInformation("RequestOutrightFullAsync Started ..........");
                await _webApiDataServiceClient.RequestOutrightFullAsync("WebApi001", token);
                _logger.LogInformation("gRPC server connected successfully.");


                await _webApiDataServiceClient.SubsribeToGetOutrightIncAsync("WebApi001", token);
                _logger.LogInformation("SubsribeToGetOutrightIncAsync has been triggered, and waiting for Server Streaming whenever have new data");
        }
    }
}
