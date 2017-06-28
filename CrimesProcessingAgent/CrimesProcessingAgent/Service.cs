using Crimes.Processing;
using Crimes.Processing.Predictions;
using CrimesProcessing.Contracts;
using Grpc.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using static CrimesProcessing.Contracts.CrimesService;

namespace CrimesProcessingAgent
{
    public class Service : CrimesServiceBase
    {
        public IStatisticProvider statisticProvider { get; set; }

        public PositionCalculator posCalculator { get; set; }

        public Service()
        {
            this.statisticProvider = new StatisticProvider();
            this.posCalculator = new PositionCalculator();
        }

        public override async Task CalculateCrimes(CalculateAvgRequest request, IServerStreamWriter<CalculateAvgResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine("Calculate average request recieved");
            var data = await this.statisticProvider.CalculateAllCrimesInDisctrictsByYear(request.Year);

            foreach (var item in data)
                await responseStream.WriteAsync(item.MapToAgentResponse());

        }

        public override async Task<CalculatePredictionResponse> GetProbability(CalculatePredictionRequest request, ServerCallContext context)
        {
            Console.WriteLine("GetProbability average request recieved");

            var dbData = await statisticProvider.CalculateAllCrimesByDistrctsByYear(request.Year);

            var res = posCalculator.CalculateAverageCrimes(CaseSimple.FromAgentRequest(request), dbData);
            return new CalculatePredictionResponse() { Probability = res };
        }

        public override Task<CrimesResponse> SayHello(CrimesRequest request, ServerCallContext context)
        {
            return Task.FromResult(new CrimesResponse { Result = "some data" });
        }
    }
}
