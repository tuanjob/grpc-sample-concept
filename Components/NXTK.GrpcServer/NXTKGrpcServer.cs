using DataServicePackage;
using Grpc.Core;
using NXTK.GrpcServer.Interfaces;
using NXTK.GrpcServer.Services;
using System;

namespace NXTK.GrpcServer
{
    public class NXTKGrpcServer
    {
        private Server _server;

        private readonly IOutrightFullDataServiceInvoker _externalDataService;
        public NXTKGrpcServer(IOutrightFullDataServiceInvoker externalDataService)
        {
            _externalDataService = externalDataService;
        }

        /// <summary>
        /// Must implement class inherit from <see cref="IOutrightFullDataServiceInvoker"/> 
        /// if you want to start using NXTKGrpcServer
        /// </summary>
        /// <param name="addressHost"></param>
        /// <param name="port"></param>
        public void StartServer(string addressHost, int port)
        {
            _server = new Server
            {
                Services = { DataService.BindService(new GrpcDataService(_externalDataService)) },
                Ports = { new ServerPort(addressHost, port, ServerCredentials.Insecure) }
            };

            _server.Start();

            Console.WriteLine($"======= SERVER IS LISTENING AT {addressHost}:{port} =============");
        }

        public void ShutdownServer()
        {
            _server?.ShutdownAsync().Wait();
        }
    }
}
