using Cassandra;
using Crimes.Processing.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crimes.Processing
{
    public interface IDbCassandraProvider
    {
        void InsertCrimes(ISession session, IEnumerable<CrimesJson> allCrimes);
        void CreateCrimeTableIfNotExists(ISession session);
        Dictionary<int, IEnumerable<CrimesDb>> ReadCrimesByYear(ISession session);
        Task<IEnumerable<CrimesDb>> ReadCrimesByYear(ISession session, int year);
        IEnumerable<CrimesDb> MappedToCrimes(RowSet rowData);
    }
}
