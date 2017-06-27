using CasandraApp;
using Cassandra;
using System;
using System.Collections.Generic;

namespace ReadData
{
    class Program
    {
        static void Main(string[] args)
        {
            ISession session = SetUpCassandra();

            var allData = ReadEverything(session);

            Console.WriteLine("End");
        }

        private static Dictionary<int, IEnumerable<Crimes>> ReadEverything(ISession session)
        {
            var allData = new Dictionary<int, IEnumerable<Crimes>>();
            for (int i = 2001; i < 2017; i++)
            {
                var rowDbData = session.Execute($"select * from crimes where \"year\"={i} Allow Filtering");

                var readedData = MappedToCrimes(rowDbData);

                allData.Add(i, readedData);               
            }

            return allData;
        }

        private static IEnumerable<Crimes> MappedToCrimes(RowSet rowData)
        {
            foreach (var r in rowData.GetRows())
            {
                yield return new Crimes()
                {
                    Block = r.GetValue<string>("block"),
                    CaseNumber = r.GetValue<string>("casenumber"),
                    CrimeDate = r.GetValue<LocalDate>("crimedate"),
                    District = r.GetValue<int>("district"),
                    Hour = r.GetValue<int>("hour"),
                    Id = r.GetValue<int>("id"),
                    Latitude = r.GetValue<double>("latitude"),
                    LocationDescription = r.GetValue<string>("locationdescription"),
                    Longitude = r.GetValue<double>("longitute"),
                    Minute = r.GetValue<int>("minute"),
                    PrimaryType = r.GetValue<string>("primarytype"),
                    Updated_On = r.GetValue<LocalDate>("updated_on"),
                    X_Coordinate = r.GetValue<double>("x_coordinate"),
                    Year = r.GetValue<int>("year"),
                    Y_Coordinate = r.GetValue<double>("y_coordinate")
                };
            }
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