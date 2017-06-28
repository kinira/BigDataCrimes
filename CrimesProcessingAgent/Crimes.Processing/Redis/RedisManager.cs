using System;
using System.Collections.Generic;
using System.Text;
using Crimes.Processing.Models;
using StackExchange.Redis;
using Crimes.Processing.Predictions;

namespace Crimes.Processing.Redis
{
    public class RedisManager : IRedisManager
    {
        private ConnectionMultiplexer connection;
        private IDatabase db;
        private RedisObjectStore<IEnumerable<DisctrictScore>> storeScore;
        private RedisObjectStore<IEnumerable<DistrictCrimes>> storeCrimes;
        private RedisObjectStore<IEnumerable<CaseSimple>> storeCaseSimple;

        public RedisManager()
        {
            this.connection = RedisConnectionFactory.GetConnection();
            this.db = connection.GetDatabase();
            this.storeScore = new RedisObjectStore<IEnumerable<DisctrictScore>>(db);
            this.storeCrimes = new RedisObjectStore<IEnumerable<DistrictCrimes>>(db);
            this.storeCaseSimple = new RedisObjectStore<IEnumerable<CaseSimple>>(db);
        }

        public void InsertScore(int year, IEnumerable<DisctrictScore> disctrictscores)
        {
                storeScore.Save($"{year}", disctrictscores);
        }

        public IEnumerable<DisctrictScore> HasScores(string key)
        {
            try
            {
                return storeScore.Get(key);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<DistrictCrimes> HasCrimes(string key)
        {
            try
            {
                return storeCrimes.Get(key);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<CaseSimple> HasCaseSimple(string key)
        {
            try
            {
                return storeCaseSimple.Get(key);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void InsertCrimes(int year, IEnumerable<DistrictCrimes> crimes)
        {
            storeCrimes.Save($"{year}", crimes);
        }

        public void InsertCaseSimple(int year, IEnumerable<CaseSimple> crimes)
        {
            storeCaseSimple.Save($"{year}", crimes);
        }
    }
}
