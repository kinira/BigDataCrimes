using Grpc.Core;
using System;
using System.Threading.Tasks;
using Xunit;
using CrimesProcessing.Contracts;

namespace CrimesProcessingAgent.RpcTests
{
    public class ServiceTests : IDisposable
    {
        private Server server;
        private Channel channel;

        public ServiceTests()
        {
            server = Program.StartServer();
            channel = new Channel($"127.0.0.1:{Program.Port}", ChannelCredentials.Insecure);
        }

        public void Dispose()
        {
            server.ShutdownAsync().Wait();
            channel.ShutdownAsync().Wait();
        }

        [Fact]
        public async Task ShouldReturnAResponse()
        {
            var client = new CrimesService.CrimesServiceClient(channel);
            var reply = await client.SayHelloAsync(new CrimesRequest { Message = "hello" });
            Assert.Equal("some data", reply.Result);
        }
    }
}
