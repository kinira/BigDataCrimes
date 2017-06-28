using CrimesProcessing.Contracts;
using ServiceStack;
using System;

namespace CrimesWebApi.ServiceModel
{
    [Route("/prediction/")]
    public class PredictionRequest : IReturn<PredictionResponse>
    {
        public double X_coordinate { get; set; }

        public double Y_coordinate { get; set; }

        public int Month { get; set; }

        public int Date { get; set; }


        public CalculatePredictionRequest ToAgentRequest(int year)
        => new CalculatePredictionRequest
        {
            Month = this.Month,
            X = this.X_coordinate,
            Y = this.Y_coordinate,
            Year = year
        };
    }
}
