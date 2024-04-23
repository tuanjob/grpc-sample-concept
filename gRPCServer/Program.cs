// See https://aka.ms/new-console-template for more information
using DataServicePackage;
using Grpc.Core;
using Grpc.Net.Client;
using gRPCSample.Core.Configurations;
using gRPCSample.Core.Models;
using gRPCSampleServer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static DataServicePackage.DataService;



HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
using IHost host = Host.CreateDefaultBuilder(args).ConfigureServices((_, services) =>
{
    services.AddScoped<IDataServiceInvoker, DataServiceImpl>();

})
.Build();

#region  Start gRPC Server
var configManager = new ConfigurationManager();
var serverSetting = configManager.GetSection<ServerSetting>("Server");

var server = new Server
{
    Services = { DataService.BindService(new DataServiceImpl()) },
    Ports = { new ServerPort(serverSetting.Host, serverSetting.Port, ServerCredentials.Insecure) }
};
server.Start();

Console.WriteLine($"======= SERVER IS LISTENING AT PORT: ${serverSetting.Port} =============");

#endregion

#region Client


// Get a simple list of strings
var clients = configManager.GetSection<List<string>>("Clients");

// [INC] Sample for another service will call for data INC
var _clientFactory = host.Services.GetRequiredService<IDataServiceInvoker>();
int i = 1;
do
{
    var jsonInc = new JsonIncModel { MatchId = i, Mode = "Update", TimeIndex = (i * 2), Message = $"Inc Data at {(i * 2)}" };
    foreach (var client in clients)
    {
        Thread.Sleep(5000);
        await _clientFactory.InvokeSendIncrementalData(client, jsonInc);
    }

    i++;
} while (i < 12);



#endregion


Console.WriteLine("Press any key to stop the server...");
Console.ReadKey();

server.ShutdownAsync().Wait();
