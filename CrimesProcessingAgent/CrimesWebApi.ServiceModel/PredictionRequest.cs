using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrimesWebApi.ServiceModel
{
    [Route("/prediction/")]
   public class PredictionRequest : IReturn<PredictionResponse>
    {
        public DateTime Date { get; set; }
        public double X_coordinate { get; set; }
        public double Y_coordinate { get; set; }
        public int Month { get; set; }
    }
}
