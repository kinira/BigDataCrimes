using System;
using System.Collections.Generic;
using System.Text;
using Cassandra;
using Crimes.Processing.Models;
using Cassandra.Mapping;
using System.Linq;
using Crimes.Processing.Extensions;
using System.Threading.Tasks;

namespace Crimes.Processing
{
    public class DbCassandraProvider : IDbCassandraProvider
    {
        public void CreateCrimeTableIfNotExists(ISession session)
        {
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

        public void InsertCrimes(ISession session, IEnumerable<CrimesJson> allCrimes)
        {
            IMapper mapper = new Mapper(session);


            var mapptoCraimsDb = allCrimes.Select(x =>
            new CrimesDb()
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

        public IEnumerable<CrimesDb> MappedToCrimes(RowSet rowData)
        {
            foreach (var r in rowData.GetRows())
            {
                yield return new CrimesDb()
                {
                    Block = r.GetValue<string>("block"),
                    CaseNumber = r.GetValue<string>("casenumber"),
                    CrimeDate = r.GetValue<LocalDate>("crimedate"),
                    District = r.GetValue<int>("district"),
                    Hour = r.GetValue<int>("hour"),
                    Id = r.GetValue<int>("id"),
                    Latitude = r.GetValue<double>("latitude"),
                    LocationDescription = r.GetValue<string>("locationdescription"),
                    Longitude = r.GetValue<double>("longitude"),
                    Minute = r.GetValue<int>("minute"),
                    PrimaryType = r.GetValue<string>("primarytype"),
                    Updated_On = r.GetValue<LocalDate>("updated_on"),
                    X_Coordinate = r.GetValue<double>("x_coordinate"),
                    Year = r.GetValue<int>("year"),
                    Y_Coordinate = r.GetValue<double>("y_coordinate")
                };
            }
        }

        public Dictionary<int, IEnumerable<CrimesDb>> ReadCrimesByYear(ISession session)
        {
            var allData = new Dictionary<int, IEnumerable<CrimesDb>>();
            for (int i = 2001; i < 2017; i++)
            {
                var rowDbData = session.Execute($"select * from crimes where \"year\"={i} Allow Filtering");

                var readedData = MappedToCrimes(rowDbData);

                allData.Add(i, readedData);
            }

            return allData;
        }

        public async Task<IEnumerable<CrimesDb>> ReadCrimesByYear(ISession session, int year)
        {
            var rowDbData = await session.ExecuteAsync(new SimpleStatement($"select * from crimes where \"year\"={year} Allow Filtering"));
            var readedData = MappedToCrimes(rowDbData);

            return readedData;
        }
    }
}
