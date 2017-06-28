using CrimesProcessing.Contracts;
using Grpc.Core;
using System.Threading.Tasks;
using static CrimesProcessing.Contracts.CrimesService;

namespace CrimesProcessingAgent
{
    public class Service : CrimesServiceBase
    {
        public override Task CalculateCrimes(CalculateAvgRequest request, IServerStreamWriter<CalculateAvgResponse> responseStream, ServerCallContext context)
        {
            return base.CalculateCrimes(request, responseStream, context);
        }

        public override Task<CalculatePredictionResponse> GetProbability(CalculatePredictionRequest request, ServerCallContext context)
        {
            return base.GetProbability(request, context);
        }

        public override Task<CrimesResponse> SayHello(CrimesRequest request, ServerCallContext context)
        {
            return Task.FromResult(new CrimesResponse { Result = "some data" });
        }
    }
}
