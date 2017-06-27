using Cassandra;
using System;
using System.Linq;
using System.Net.Http;

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
            httpClient.BaseAddress = new Uri("https://data.cityofchicago.org/resource/6zsd-86xi.json");

            Console.WriteLine("Uspeh");
            //session.Execute("insert into users (lastname, age, city, email, firstname) values ('Jones', 35, 'Austin', 'bob@example.com', 'Bob')");
            //Row result = session.Execute("select * from users where lastname='Jones'").First();
            //Console.WriteLine("{0} {1}", result["firstname"], result["age"]);
        }

        private static void CreateCrimeTableIfNotExists(ISession session)
        {
            session.Execute(@"
                    CREATE TABLE IF NOT EXISTS crimes(
                            id int PRIMARY KEY,
                             CaseNumber ascii,
                                Date date,
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
                            Longitude double,
                            Location tuple<double,double>)");
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