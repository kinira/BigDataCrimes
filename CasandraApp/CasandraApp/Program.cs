using Cassandra;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Cassandra.Mapping;


namespace CasandraApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start insert into Cassndra db");
            ISession session = SetUpCassandra();

            CreateCrimeTableIfNotExists(session);
            
            var httpClient = new HttpClient();
            var limitParam = "&$limit=5000";
            var uriParams = "year=2001";
            var uri = $"https://data.cityofchicago.org/resource/6zsd-86xi.json?{uriParams}{limitParam}";
            httpClient.BaseAddress = new Uri(uri);

            List<CrimesJson> allCrimes;

            using (HttpResponseMessage response = httpClient.GetAsync(uri).Result)
            {
                using (HttpContent content = response.Content)
                {
                    var json = content.ReadAsStringAsync().Result;
                    allCrimes = JsonConvert.DeserializeObject<List<CrimesJson>>(json);
                }
            }
            
            Console.WriteLine("Uspeh");
            IMapper mapper = new Mapper(session);

            var mapptoCraimsDb = allCrimes.Select(x =>
            new Crimes() {
                Block = x.Block,
                CaseNumber = x.CaseNumber,
                CrimeDate = new LocalDate(x.Date.Year, x.Date.Month,x.Date.Day),
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
                Y_Coordinate = x.Y_Coordinate});    

            foreach (var crime in mapptoCraimsDb)
            {
                mapper.Insert(crime);
                Console.WriteLine("Entry inserted");
            }

            Console.WriteLine("test");

            //session.Execute("insert into users (lastname, age, city, email, firstname) values ('Jones', 35, 'Austin', 'bob@example.com', 'Bob')");
            //Row result = session.Execute("select * from users where lastname='Jones'").First();
            //Console.WriteLine("{0} {1}", result["firstname"], result["age"]);
        }

        private static void CreateCrimeTableIfNotExists(ISession session)
        {
            session.Execute("DROP TABLE crimes");
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

        private static ISession SetUpCassandra()
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