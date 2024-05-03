using NXTK.GrpcServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gRPCSampleServer.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("STArted ........");

            var grpcServer = new NXTKGrpcServer();
            grpcServer.StartServer("localhost", 50051);// TODO

            Console.ReadLine();

            grpcServer.ShutdownServer();
        }
    }
}
