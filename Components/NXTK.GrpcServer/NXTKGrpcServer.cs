using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using DataServicePackage;
using NXTK.GrpcServer.Services;

namespace NXTK.GrpcServer
{
    public class NXTKGrpcServer
    {
        private Server _server;

        public NXTKGrpcServer()
        {
            
        }

        public void StartServer(string addressHost, int port)
        {
            _server = new Server
            {
                Services = { DataService.BindService(new DataServiceImpl()) },
                Ports = { new ServerPort(addressHost, port, ServerCredentials.Insecure) },
            };

            _server.Start();

            Console.WriteLine($"======= SERVER IS LISTENING AT PORT: {port} =============");
        }

        public void ShutdownServer()
        {
            _server?.ShutdownAsync().Wait();
        }
    }
}
