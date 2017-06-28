using CrimesProcessing.Contracts;
using CrimesWebApi.ServiceModel;
using Grpc.Core;
using Service = ServiceStack.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrimesWebApi
{
    public class HomeService : Service
    {
        private CrimesService.CrimesServiceClient agent1;

        public HomeService()
        {
            var channel1 = new Channel($"127.0.0.1:50051", ChannelCredentials.Insecure);
            agent1 = new CrimesService.CrimesServiceClient(channel1);
        }

        public object Any(HelloRequest request)
        {
            return new HelloResponse { Result = $"Hello {request.Name}"};
        }

        public async Task<PredictionResponse> Post(PredictionRequest request)
        {
            await agent1.GetProbabilityAsync(new CalculatePredictionRequest { Date = request.Date.ToString(), Xcoordinates = request.X_coordinate.ToString(), Ycoordinates = request.Y_coordinate.ToString() });
        }

    }
}
