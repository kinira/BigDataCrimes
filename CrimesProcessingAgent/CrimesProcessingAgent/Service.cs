using CrimesProcessing.Contracts;
using Grpc.Core;
using System.Threading.Tasks;
using static CrimesProcessing.Contracts.CrimesService;

namespace CrimesProcessingAgent
{
    public class Service : CrimesServiceBase
    {
        public override Task<CrimesResponse> SayHello(CrimesRequest request, ServerCallContext context)
        {
            return Task.FromResult(new CrimesResponse { Result = "some data" });
        }
    }
}
