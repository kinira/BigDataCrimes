using CrimesProcessing.Contracts;

namespace Crimes.Processing.Models
{
    public class DistrictCrimes
    {
        public int Year { get; set; }
        public int District { get; set; }
        public string CrimeType { get; set; }
        public int CrimAvg { get; set; }

        public CalculateAvgResponse MapToAgentResponse()
           => new CalculateAvgResponse
           {
               CrimesCount = this.CrimAvg,
               CrimeType = this.CrimeType,
               District = this.District,
               Year = this.Year
           };

    }
}
