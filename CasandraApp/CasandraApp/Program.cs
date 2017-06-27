using CasandraApp.Extensions;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace CasandraApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start insert into Cassandra db");
            ISession session = SetUpCassandra();

            //CreateCrimeTableIfNotExists(session);

            var httpClient = new HttpClient();


            //for (int i = 2001; i <= 2017; i++)
            //{
            //    List<CrimesJson> allCrimes = DownloadCrimes(httpClient, i);
            //    InsertCrimes(session, allCrimes);
            //}


            session.Execute("DROP INDEX crimes_year");

            Console.WriteLine();
        }


        private static void InsertCrimes(ISession session, IEnumerable<CrimesJson> allCrimes)
        {
            IMapper mapper = new Mapper(session);


            var mapptoCraimsDb = allCrimes.Select(x =>
            new Crimes()
            {
                Block = x.Block,
                CaseNumber = x.CaseNumber,
                CrimeDate = new LocalDate(x.Date.Year, x.Date.Month, x.Date.Day),
                District = x.District,
                Hour = x.Date.Hour,
                Minute = x.Date.Minute,
                Id = x.Id,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                LocationDescription = x.LocationDescription,
                PrimaryType = x.PrimaryType,
                Updated_On = new LocalDate(x.Updated_On.Year, x.Updated_On.Month, x.Updated_On.Day),
                X_Coordinate = x.X_Coordinate,
                Year = x.Year,
                Y_Coordinate = x.Y_Coordinate
            });

            var batchesForInsert = mapptoCraimsDb.Batch(100);

            foreach (var crimesBatch in batchesForInsert)
            {
                var batch = mapper.CreateBatch(BatchType.Unlogged);

                foreach (var crime in crimesBatch)
                    batch.Insert(crime);

                mapper.ExecuteAsync(batch).Wait();
                Console.WriteLine("Batch inserted");
            }
        }

        private static List<CrimesJson> DownloadCrimes(HttpClient httpClient, int year)
        {
            var limitParam = "$limit=65000";
            var uriParams = $"year={year}";
            var apitoken = $"$$app_token=cvJt8qTg2K2iQ0OfYmoH4vsgx";
            var uri = $"https://data.cityofchicago.org/resource/6zsd-86xi.json?{uriParams}&{limitParam}&{apitoken}";

            List<CrimesJson> allCrimes;
            using (HttpResponseMessage response = httpClient.GetAsync(uri).Result)
            {
                using (HttpContent content = response.Content)
                {
                    var json = content.ReadAsStringAsync().Result;
                    allCrimes = JsonConvert.DeserializeObject<List<CrimesJson>>(json);
                }
            }

            return allCrimes;
        }

        private static void CreateCrimeTableIfNotExists(ISession session)
        {
            //session.Execute("DROP TABLE crimes");
            session.Execute(@"
                    CREATE TABLE IF NOT EXISTS crimes(
                            id int PRIMARY KEY,
                             CaseNumber ascii,
                                CrimeDate date,
                            Hour int,
                            Minute int,
                            Block ascii,
                            PrimaryType ascii,
                            LocationDescription ascii,
                            District int,
                            X_Coordinate double,
                            Y_Coordinate double,
                            Year int,
                            Updated_On date,
                            Latitude double,
                            Longitude double
                            )");
        }

        public static ISession SetUpCassandra()
        {
            Cluster cluster = Cluster.Builder()
                .AddContactPoints("35.158.163.178", "35.158.217.185", "35.158.151.241")
                .WithAuthProvider(new PlainTextAuthProvider("iccassandra", "c24a0e043b2f439225898c5bdcb61a49"))
                .WithPort(9042)
                .WithLoadBalancingPolicy(new DCAwareRoundRobinPolicy("AWS_VPC_EU_CENTRAL_1"))
                .Build();

            ISession session = cluster.Connect();

            session.CreateKeyspaceIfNotExists("demo");
            session.ChangeKeyspace("demo");
            return session;
        }
    }
}