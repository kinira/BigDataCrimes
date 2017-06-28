using System;
using System.Collections.Generic;
using System.Text;
using Crimes.Processing.Models;
using System.Linq;
using Cassandra;
using Crimes.Cassandra;
using System.Threading.Tasks;
using Crimes.Processing.Predictions;

namespace Crimes.Processing
{
    public class StatisticProvider : IStatisticProvider, IDisposable
    {
        IDbCassandraProvider dbProvider { get; set; }

        ISession session { get; set; }

        public StatisticProvider()
        {
            this.dbProvider = new DbCassandraProvider();
            this.session = new SetUp().SetUpCassandra();
        }

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

        public async Task<IEnumerable<DistrictCrimes>> CalculateAllCrimesInDisctrictsByYear(int year)
        {
            var allCrimesByYear = await dbProvider.ReadCrimesByYear(session, year);

            var crimessummary = new Dictionary<(int, string), int>();
            foreach (var crime in allCrimesByYear)
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

        public async Task<IEnumerable<CaseSimple>> CalculateAllCrimesByDistrctsByYear(int year)
        {
            var task = await dbProvider.ReadCrimesByYear(session, year);
            var res = task.Select(x => new CaseSimple() { Year = x.Year, Type = x.PrimaryType, X = x.X_Coordinate, Y = x.Y_Coordinate, Month = x.CrimeDate.Month });
            return res;
        }

        public async Task<IEnumerable<CaseSimple>> GetCrimesOneMonthBack()
        {
            var task = await dbProvider.ReadCrimesByYear(session, DateTime.Now.Year);
            var res = task.Where(x => x.CrimeDate.Month == DateTime.Now.AddMonths(-1).Month);
            return res.Select(x => new CaseSimple() { Year = x.Year, Type = x.PrimaryType, X = x.X_Coordinate, Y = x.Y_Coordinate, Month = x.CrimeDate.Month });
        }

        public void Dispose()
        {
            //this.session.Dispose();
        }
    }
}
