using Crimes.Processing.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimes.Processing
{
    public interface IStatisticProvider
    {
        Dictionary<int, Dictionary<Tuple<int, string>, int>> CalculateAllCrimesByDistrcts(Dictionary<int, IEnumerable<CrimesDb>> allCrimesByYears);
        int GetScore(string primaryType);
        void DisplayCrimesByDistricts(Dictionary<int, Dictionary<Tuple<int, string>, int>> allCrimesByDistricts);
        List<DisctrictScore> CalculateDistrictScores(Dictionary<int, Dictionary<Tuple<int, string>, int>> allCrimesByDistricts);
    }
}
