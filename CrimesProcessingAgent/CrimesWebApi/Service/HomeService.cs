﻿using CrimesProcessing.Contracts;
using CrimesWebApi.ServiceModel;
using Grpc.Core;
using Service = ServiceStack.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CrimesProcessing.Contracts.CrimesService;
using Crimes.Processing.Predictions;

namespace CrimesWebApi
{
    public class HomeService : Service
    {
        private IReadOnlyList<CrimesServiceClient> agents;
        private PositionCalculator calculator;

        public HomeService(IReadOnlyList<CrimesServiceClient> agents, PositionCalculator calculator)
        {
            this.agents = agents;
            this.calculator = calculator;
        }

        public object Any(HelloRequest request)
        {
            return new HelloResponse { Result = $"Hello {request.Name}" };
        }

        public async Task<PredictionResponse> Post(PredictionRequest request)
        {
            var year = DateTime.Now.Year;
            var tasks = new Dictionary<int, AsyncUnaryCall<CalculatePredictionResponse>>();
            var yearAverages = new Dictionary<int , double>();

            for (int i = year; i >= year - 4; i--)
            {
                tasks.Add(i, agents[i % agents.Count].GetProbabilityAsync(new CalculatePredictionRequest
                {
                    Month = request.Month,
                    X = request.X_coordinate,
                    Y = request.Y_coordinate,
                    Year = year
                }));
            }

            foreach (var task in tasks)
                yearAverages.Add(task.Key, (await task.Value).Probability);

            var expectation = calculator.GetAverageOfPreviousYears(yearAverages);


            return new PredictionResponse();
        }

    }
}
