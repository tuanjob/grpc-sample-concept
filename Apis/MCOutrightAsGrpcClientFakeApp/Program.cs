// #define useRun

using MCOutrightAsGrpcClientFakeApp.gRPCServices;
using MCOutrightAsGrpcClientFakeApp.ServiceCollections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add Log
builder.Services.AddLogger(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the gRPC client as a singleton
builder.Services.AddSingleton<WebApiDataServiceClient>();

// builder.Services.AddHostedService<OutrightFullHostedService>(); // => the app will be stuck ( stop until can connect to gRPC server )


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

/*
 * [NOTE]
 * We have 2 approaches to do:
 * #1 - Call directly InitializeAsync in this Program => this application can start without waiting for connect with gRPC server
 * #2 - Using HostedService for OutrightFullHostedService => this way will stuck the app if cannot connect to gRPC Server
 */

#if(useRun)
{
    // #1 Use app.Run
    await InitializeAsync(app); // Or use AddHostedService<OutrightFullHostedService> above instead of this

    app.Run();
}
#else
{
    // #2 Use app.StartAsync()
    //// Start the host (application)
    await app.StartAsync();

    await InitializeAsync(app);

    // Wait for the host to be stopped (if needed)
     await app.WaitForShutdownAsync();
}
#endif



async Task InitializeAsync(IHost host)
{
    // Get the MyGrpcClient instance from the service provider
    using (var scope = app.Services.CreateScope())
    {
        var _logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var _webApiDataServiceClient = scope.ServiceProvider.GetRequiredService<WebApiDataServiceClient>();

        _logger.LogInformation("RequestOutrightFullAsync Started ..........");
        await _webApiDataServiceClient.RequestOutrightFullAsync("WebApi001", CancellationToken.None);
        _logger.LogInformation("gRPC server connected successfully.");


        await _webApiDataServiceClient.SubsribeToGetOutrightIncAsync("WebApi001", CancellationToken.None);
        _logger.LogInformation("SubsribeToGetOutrightIncAsync has been triggered, and waiting for Server Streaming whenever have new data");
    }
}