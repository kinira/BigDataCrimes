using Cassandra;
using Crimes.Processing.Models;
using System.Collections.Generic;

namespace Crimes.Processing
{
    public interface IDbCassandraProvider
    {
        void InsertCrimes(ISession session, IEnumerable<CrimesJson> allCrimes);
        void CreateCrimeTableIfNotExists(ISession session);
        Dictionary<int, IEnumerable<CrimesDb>> ReadCrimesByYear(ISession session);
        IEnumerable<CrimesDb> MappedToCrimes(RowSet rowData);
    }
}
