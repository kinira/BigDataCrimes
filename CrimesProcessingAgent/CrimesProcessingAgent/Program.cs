using CrimesProcessing.Contracts;
using Grpc.Core;
using System;
using static CrimesProcessing.Contracts.CrimesService;
using System.Threading.Tasks;

namespace CrimesProcessingAgent
{
    public class Service : CrimesServiceBase
    {
        public override Task<CrimesResponse> SayHello(CrimesRequest request, ServerCallContext context)
        {
            return Task.FromResult(new CrimesResponse { Result = "some data" });
        }
    }

    public static class Program
    {
        public const int Port = 50051;

        public static Server StartServer()
        {
            Server server = new Server
            {
                Services = { BindService(new Service()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Greeter server listening on port " + Port);
            return server;
        }

        public static void Main(string[] args)
        {
            var server = StartServer();
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}