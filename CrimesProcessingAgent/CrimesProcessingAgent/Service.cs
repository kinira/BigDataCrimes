using Crimes.Processing;
using Crimes.Processing.Predictions;
using CrimesProcessing.Contracts;
using Grpc.Core;
using System;
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
            var data = await this.statisticProvider.CalculateAllCrimesInDisctrictsByYear(request.Year);

            foreach (var item in data)
                await responseStream.WriteAsync(new CalculateAvgResponse() { CrimesCount = item.CrimAvg, CrimeType = item.CrimeType, District = item.District, Year = request.Year});

        }

        public override async Task<CalculatePredictionResponse> GetProbability(CalculatePredictionRequest request, ServerCallContext context)
        {
            var calc = new PositionCalculator();
            var dbData = await statisticProvider.CalculateAllCrimesByDistrctsByYear(DateTime.Parse(request.Date).Year);

            var res = calc.CalculateAverageCrimes(new CaseSimple { X = double.Parse(request.Xcoordinates),
                                                          Y = double.Parse(request.Ycoordinates),
                                                            Year = DateTime.Now.Year, Month = DateTime.Now.Month },dbData);
            return new CalculatePredictionResponse() { Probability = res };
        }

        public override Task<CrimesResponse> SayHello(CrimesRequest request, ServerCallContext context)
        {
            return Task.FromResult(new CrimesResponse { Result = "some data" });
        }
    }
}
