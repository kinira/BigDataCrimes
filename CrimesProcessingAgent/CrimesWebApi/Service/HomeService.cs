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
using Crimes.Processing;

namespace CrimesWebApi
{
    public class HomeService : Service
    {
        private IReadOnlyList<CrimesServiceClient> agents;
        private IStatisticProvider statisticProvider;
        private PositionCalculator calculator;

        public HomeService(IReadOnlyList<CrimesServiceClient> agents, PositionCalculator calculator, IStatisticProvider statisticProvider)
        {
            this.agents = agents;
            this.calculator = calculator;
            this.statisticProvider = statisticProvider;
        }

        public object Any(HelloRequest request)
        {
            return new HelloResponse { Result = $"Hello {request.Name}" };
        }

        public async Task<PredictionResponse> Post(PredictionRequest request)
        {
            var year = DateTime.Now.Year;
            var tasks = new Dictionary<int, AsyncUnaryCall<CalculatePredictionResponse>>();
            var yearAverages = new Dictionary<int, double>();

            for (int y = year; y >= year -4 ; y--)
                tasks.Add(y, agents[y % agents.Count].GetProbabilityAsync(request.ToAgentRequest(y)));

            foreach (var task in tasks)
                yearAverages.Add(task.Key, (await task.Value).Probability);

            var expectation = calculator.GetAverageOfPreviousYears(yearAverages);
            var recentCrimes = await statisticProvider.GetCrimesOneMonthBack();
            var lastMonthData = calculator.FindDaysSinceLastCrime(CaseSimple.FromApiRequest(request), recentCrimes);
            var probability = calculator.CalculateCrimeProbability(expectation, lastMonthData);

            return new PredictionResponse { Probability = probability };
        }

    }
}
