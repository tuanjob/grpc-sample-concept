// See https://aka.ms/new-console-template for more information
using DataServicePackage;
using Grpc.Core;
using Grpc.Net.Client;
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

const int Port = 50051;
var server = new Server
{
    Services = { DataService.BindService(new DataServiceImpl()) },
    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
};
server.Start();
Console.WriteLine($"======= Server listening on port ${Port} =============");

#endregion

#region Client

// [INC] Sample for another service will call for data INC
var _clientFactory = host.Services.GetRequiredService<IDataServiceInvoker>();
int i = 0;
do
{
    Thread.Sleep(5000);
    await _clientFactory.InvokeGetIncrementalData1("MCOutright001", $"INC data at {i}");

    Thread.Sleep(5000);
    await _clientFactory.InvokeGetIncrementalData1("MCOutright002", $"INC data at {i}");

    i++;
} while (i < 11);



#endregion


Console.WriteLine("Press any key to stop the server...");
Console.ReadKey();

server.ShutdownAsync().Wait();
