using MCServiceFake.ConsoleApp.RegisterServices;
using Microsoft.Extensions.DependencyInjection;
using NXTK.GrpcServer;
using NXTK.GrpcServer.Interfaces;
using System;
using System.Threading.Tasks;

namespace MCServiceFake.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Started Application ........");

            //#1- Register Services
            NXTKServiceCollections.RegisterServices();
            var externalDataService = DependencyResolver.ServiceProvider.GetService<IOutrightFullDataServiceInvoker>();

            //#1- Register Services
            var grpcServer = new NXTKGrpcServer(externalDataService);
            grpcServer.StartServer("localhost", 50051);// TODO


            var _v = "n";
            Console.WriteLine("Do you want to send Inc Data (y/n)");
            _v = Console.ReadLine();

            while (_v.Equals("y"))
            {
                await IvokeIncData();

                Console.WriteLine("Conintue? (y/n)");
                _v = Console.ReadLine();
            }
            

            Console.ReadLine();
            grpcServer.ShutdownServer();
        }

        static void InvokeFullData()
        {
            // Call service OutrightFullDataServiceBuilder
        }

        static async Task IvokeIncData()
        {
            var dataServiceInvoker = DependencyResolver.ServiceProvider.GetService<IOutrightIncDataServiceInvoker>();

            await dataServiceInvoker.InvokeSendIncrementalData(new NXTK.GrpcServer.Models.HDPOUIncOdds { Data = "ababw", FTSocOddsId = DateTime.Now.Millisecond });

        }
    }
}
