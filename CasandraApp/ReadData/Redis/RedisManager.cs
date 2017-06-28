using System;
using System.Collections.Generic;
using System.Text;
using ReadData.Models;

namespace ReadData.Redis
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

            var temp = store.Get("2001_4");
           
        }
    }
}
