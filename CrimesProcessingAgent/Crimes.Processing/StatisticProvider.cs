using System;
using System.Collections.Generic;
using System.Text;
using Crimes.Processing.Models;
using System.Linq;

namespace Crimes.Processing
{
    public class StatisticProvider : IStatisticProvider
    {
        public IEnumerable<DistrictCrimes> CalculateAllCrimesByDistrcts(IEnumerable<CrimesDb> allCrimesByYears)
        {
            var crimessummary = new Dictionary<(int, string), int>();
            foreach (var crime in allCrimesByYears)
            {
                var currentKey = (crime.District, crime.PrimaryType);

                if (!crimessummary.ContainsKey(currentKey))
                    crimessummary.Add(currentKey, 1);
                else
                    crimessummary[currentKey]++;
            }


            return crimessummary.Select(kvp => new DistrictCrimes { District = kvp.Key.Item1, CrimeType = kvp.Key.Item2, CrimAvg = kvp.Value });
        }

        public List<DisctrictScore> CalculateDistrictScores(Dictionary<int, Dictionary<Tuple<int, string>, int>> allCrimesByDistricts)
        {
            var res = new List<DisctrictScore>();
            foreach (var yearCrimes in allCrimesByDistricts)
            {
                Console.WriteLine($"Statistcs for year {yearCrimes.Key}");

                var districtSavety = new Dictionary<int, int>();



                foreach (var yearCrime in yearCrimes.Value)
                {
                    if (!districtSavety.ContainsKey(yearCrime.Key.Item1))
                    {

                        var alltypesOfCrimesInDistrict = yearCrimes.Value.Where(x => x.Key.Item1 == yearCrime.Key.Item1);

                        var score = 0;
                        foreach (var distrinctCrime in alltypesOfCrimesInDistrict)
                        {
                            score = GetScore(distrinctCrime.Key.Item2) * distrinctCrime.Value;
                        }

                        districtSavety.Add(yearCrime.Key.Item1, score);

                        res.Add(new DisctrictScore() { District = yearCrime.Key.Item1, Score = score, Year = yearCrimes.Key });
                    }
                }
            }
            return res;
        }

        public void DisplayCrimesByDistricts(Dictionary<int, Dictionary<Tuple<int, string>, int>> allCrimesByDistricts)
        {
            foreach (var yearCrimes in allCrimesByDistricts)
            {
                var displayYear = $"For year {yearCrimes.Key} the all crimes in all districts are:";
                Console.WriteLine(displayYear);
                foreach (var item in yearCrimes.Value)
                {
                    var displaycrimes = $"District: {item.Key.Item1} - Crime type: {item.Key.Item2}, total crimes: {item.Value}";
                    Console.WriteLine(displaycrimes);
                }
            }
        }

        public int GetScore(string primaryType)
        {
            switch (primaryType.ToUpper().Trim())
            {
                case "CRIMINAL DAMAGE": return 15;
                case "BURGLARY": return 10;
                case "DECEPTIVE PRACTICE": return 2;
                case "NARCOTICS": return 8;
                case "OTHER OFFENSE": return 2;
                case "MOTOR VEHICLE THEFT": return 6;
                case "ARSON": return 14;
                case "ASSAULT": return 15;
                case "BATTERY": return 10;
                case "CONCEALED CARRY LICENSE VIOLATION": return 3;
                case "CRIM SEXUAL ASSAULT": return 11;
                case "CRIMINAL TRESPASS": return 6;
                case "GAMBLING": return 3;
                case "HOMICIDE": return 15;
                case "HUMAN TRAFFICKING": return 10;
                case "INTERFERENCE WITH PUBLIC OFFICER": return 5;
                case "INTIMIDATION": return 8;
                case "KIDNAPPING": return 13;
                case "LIQUOR LAW VIOLATION": return 4;
                case "NON - CRIMINAL": return 1;
                case "NON-CRIMINAL": return 1;
                case "OBSCENITY": return 7;
                case "OFFENSE INVOLVING CHILDREN": return 12;
                case "OTHER NARCOTIC VIOLATION": return 8;
                case "PROSTITUTION": return 6;
                case "PUBLIC INDECENCY": return 10;
                case "PUBLIC PEACE VIOLATION": return 4;
                case "RITUALISM": return 2;
                case "ROBBERY": return 14;
                case "SEX OFFENSE": return 14;
                case "STALKING": return 6;
                case "THEFT": return 9;
                case "WEAPONS VIOLATION": return 11;
                default:
                    return 0;
            }
        }
    }
}
