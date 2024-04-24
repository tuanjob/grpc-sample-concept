// See https://aka.ms/new-console-template for more information
using DataServicePackage;
using Grpc.Core;
using gRPCSample.Core.Configurations;
using gRPCSample.Core.Models;
using gRPCSampleServer.FakeData;
using gRPCSampleServer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

#region MAIN SERVICE HOST

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
using IHost host = Host.CreateDefaultBuilder(args).ConfigureServices((_, services) =>
{
    // Register your gRPC service implementation
    services.AddSingleton<DataServiceImpl>();

    services.AddSingleton<IDataServiceInvoker, DataServiceImpl>();
    services.AddScoped<IOutrightFullDataService, OutrightFullJsonData>();
    services.AddScoped<IOutrightIncDataService, OutrightIncJsonData>();
})
.Build();

#endregion


#region  Start gRPC Server

var configManager = new ConfigurationManager();
var serverSetting = configManager.GetSection<ServerSetting>("Server");

var server = new Server
{
    Services = { DataService.BindService(host.Services.GetRequiredService<DataServiceImpl>()) },
    Ports = { new ServerPort(serverSetting.Host, serverSetting.Port, ServerCredentials.Insecure) }
};
server.Start();

Console.WriteLine($"======= SERVER IS LISTENING AT PORT: ${serverSetting.Port} =============");

#endregion


#region INJECT SERVICES

// [INC] Sample for another service will call for data INC
var _clientFactory = host.Services.GetRequiredService<IDataServiceInvoker>();
var _outrightFullDataService = host.Services.GetRequiredService<IOutrightFullDataService>();
var _outrightIncDataService = host.Services.GetRequiredService<IOutrightIncDataService>();

#endregion

#region IMPLEMETATIONS

// ============ 1
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("1 => Press any key to send Increment data .... ");
Console.ReadLine();
Console.WriteLine("#1 - Add First Incremental data .... Processing.....");

var matchId = _outrightFullDataService.AddNew();
var incData = _outrightIncDataService.AddNew(matchId, ModType.ADD, MarketType.Early, OddsCommand.GameDesc);
await _clientFactory.InvokeSendIncrementalData(incData);

// ============ 2
Console.WriteLine("2=> Press any key to send Increment data .... ");
Console.ReadLine();
Console.WriteLine($"#2 - New Incremental data (Mode: Update for matchID: {matchId}) .... Processing.....");

_outrightFullDataService.Update(matchId);
incData = _outrightIncDataService.AddNew(matchId, ModType.MOD, MarketType.Early, OddsCommand.GameDesc);
await _clientFactory.InvokeSendIncrementalData(incData);

// ============ 3
Console.WriteLine("3=> Press any key to send Increment data .... ");
Console.ReadLine();
Console.WriteLine($"#3 - New Incremental data (Mode: Delete for matchID: {matchId}) .... Processing.....");


_outrightFullDataService.Delete(matchId);
incData = _outrightIncDataService.AddNew(matchId, ModType.DEL, MarketType.Early, OddsCommand.GameDesc);
await _clientFactory.InvokeSendIncrementalData(incData);

#endregion


Console.WriteLine("Press any key to stop the server...");
Console.ReadKey();

server.ShutdownAsync().Wait();
