using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crimes.Processing.Predictions
{
    public class CaseSimple
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Year { get; set; }

        public double Month { get; set; }

        public string Type { get; set; }

        public double DistanceTo(CaseSimple other)
        {
            var dX = (this.X - other.X) * (this.X - other.X);
            var dY = (this.Y - other.Y) * (this.Y - other.Y);
            var dMonth = (this.Month - other.Month) * (this.Month - other.Month);

            return Math.Sqrt(dX + dY + dMonth);
        }
    }

    public class PositionCalculator
    {

        public double GetAverageOfPreviousYears(IDictionary<int, double> yearlyAverages)
        {
            var currentYear = DateTime.Now.Year;
            var coefficients = new[] {
                    0.5, // this year
                    0.3, // prev year
                    0.1,
                    0.05,
                    0.05
            };

            return Enumerable.Range(0, 5).Sum(i => coefficients[i] * yearlyAverages[currentYear - i]);
        }


        public double CalculateCrimeProbability(double averagePerDay, int daysSinceLastCrime)
            => 1 - Math.Pow((1 - averagePerDay), daysSinceLastCrime);  // some sort of geometric distribution, commulative formula

        public double CalculateAverageCrimes(CaseSimple given, IEnumerable<CaseSimple> yearlySamples)
            => CountIncidentsInTheArea(given, yearlySamples) / 365;


        public void PredictNextCrimeType(IEnumerable<CaseSimple> disctictScores, CaseSimple givenCase)
          => disctictScores
                .OrderBy(x => x.DistanceTo(givenCase))
                .Take(15)
                .GroupBy(x => x.Type)
                .ToDictionary(grp => grp.Key, grp => grp.Count())
                .OrderByDescending(x => x.Value)
                .First();


        private double CountIncidentsInTheArea(CaseSimple given, IEnumerable<CaseSimple> cases)
            => cases.Sum(item => GetSignificance(given, item));


        /// <summary>
        /// <para> The significance is a number between 0 and 1 </para>
        /// <para> The closer the distance is to 0, the closer the significance is to 1 </para>
        /// <para> Distances of 100 and above will have 0 significance </para>
        /// </summary>
        private double GetSignificance(CaseSimple first, CaseSimple second)
        {
            var distance = first.DistanceTo(second);
            return (distance > 100) ? 0 : Math.Cos(distance / 65);
        }

    }
}
