using Crimes.Processing.Models;
using Crimes.Processing.Predictions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimes.Processing.Redis
{
    public interface IRedisManager
    {
        void InsertScore(int year, IEnumerable<DisctrictScore> disctrictscores);
        IEnumerable<DisctrictScore> HasScores(string key);
        IEnumerable<DistrictCrimes> HasCrimes(string key);
        void InsertCrimes(int year, IEnumerable<DistrictCrimes> crimes);
        IEnumerable<CaseSimple> HasCaseSimple(string key);
        void InsertCaseSimple(int year, IEnumerable<CaseSimple> crimes);

    }
}
