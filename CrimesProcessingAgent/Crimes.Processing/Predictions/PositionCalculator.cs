using Crimes.Processing.Models;
using Crimes.Processing.Predictions;
using CrimesProcessing.Contracts;
using CrimesWebApi.ServiceModel;
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

        public int Year { get; set; }

        public int Month { get; set; }

        public int Date { get; set; }

        public string Type { get; set; }

        public double DistanceTo(CaseSimple other)
        {
            var dX = (this.X - other.X) * (this.X - other.X);
            var dY = (this.Y - other.Y) * (this.Y - other.Y);
            var dMonth = (double)(this.Month - other.Month) * (this.Month - other.Month);

            return Math.Sqrt(dX + dY + dMonth);
        }

        public static CaseSimple FromAgentRequest(CalculatePredictionRequest request)
         => new CaseSimple
         {
             X = request.X,
             Y = request.Y,
             Year = request.Year,
             Month = request.Month
         };

        public static CaseSimple FromApiRequest(PredictionRequest request)
          => new CaseSimple
          {
              X = request.X_coordinate,
              Y = request.Y_coordinate,
              Month = request.Month,
              Date = request.Date,
              Year = DateTime.Now.Year
          };

        public static CaseSimple FromDbModel(CrimesDb model)
          => new CaseSimple()
          {
              Year = model.Year,
              Type = model.PrimaryType,
              X = model.X_Coordinate,
              Y = model.Y_Coordinate,
              Month = model.CrimeDate.Month,
              Date = model.CrimeDate.Day
          };
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

        var len = Math.Min(coefficients.Length, yearlyAverages.Count);

        return Enumerable.Range(0, len).Sum(i => coefficients[i] * yearlyAverages[currentYear - i]);
    }

    public int FindDaysSinceLastCrime(CaseSimple given, IEnumerable<CaseSimple> lastMonthData)
    {
        var lastCase = lastMonthData.OrderBy(item => item.DistanceTo(given)).FirstOrDefault();
        if (lastCase == null) return 30;

        var time = new DateTime(given.Year, given.Month, given.Date) - new DateTime(lastCase.Year, lastCase.Month, lastCase.Date);
        return time.Days;
    }


    public double CalculateCrimeProbability(double averagePerDay, double daysSinceLastCrime)
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
