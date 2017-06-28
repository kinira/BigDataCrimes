using Crimes.Processing.Models;
using Crimes.Processing.Predictions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crimes.Processing
{
    public interface IStatisticProvider
    {
        Task<IEnumerable<CaseSimple>> GetCrimesOneMonthBack();

        IEnumerable<DistrictCrimes> CalculateAllCrimesByDistrcts(IEnumerable<CrimesDb> allCrimesByYears);

        Task<IEnumerable<CaseSimple>> CalculateAllCrimesByDistrctsByYear(int year);

        Task<IEnumerable<DistrictCrimes>> CalculateAllCrimesInDisctrictsByYear(int year);

        int GetScore(string primaryType);

        void DisplayCrimesByDistricts(Dictionary<int, Dictionary<Tuple<int, string>, int>> allCrimesByDistricts);

        List<DisctrictScore> CalculateDistrictScores(Dictionary<int, Dictionary<Tuple<int, string>, int>> allCrimesByDistricts);
    }
}
