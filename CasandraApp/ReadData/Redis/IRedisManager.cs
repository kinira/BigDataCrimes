
using ReadData.Models;
using System.Collections.Generic;

namespace ReadData.Redis
{
    public interface IRedisManager
    {
        void InsertIntoRedis(List<DisctrictScore> disctrictscores);
    }
}
