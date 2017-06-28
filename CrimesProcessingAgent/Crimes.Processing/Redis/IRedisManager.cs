using Crimes.Processing.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimes.Processing.Redis
{
    public interface IRedisManager
    {
        void InsertIntoRedis(List<DisctrictScore> disctrictscores);
    }
}
