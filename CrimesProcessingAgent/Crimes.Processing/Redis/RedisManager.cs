using System;
using System.Collections.Generic;
using System.Text;
using Crimes.Processing.Models;

namespace Crimes.Processing.Redis
{
    public class RedisManager : IRedisManager
    {
        public void InsertIntoRedis(List<DisctrictScore> disctrictscores)
        {
            var con = RedisConnectionFactory.GetConnection();
            var db = con.GetDatabase();

            var store = new RedisObjectStore<DisctrictScore>(db);

            foreach (var item in disctrictscores)
            {
                store.Save($"{item.Year}_{item.District}", item);
            }

           
        }
    }
}
