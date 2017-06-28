using ServiceStack;

namespace CrimesWebApi.ServiceModel
{
    [Route("/prediction/")]
   public class PredictionRequest : IReturn<PredictionResponse>
    {
        public double X_coordinate { get; set; }

        public double Y_coordinate { get; set; }

        public int Month { get; set; }

        public int Date { get; set; }
    }
}
