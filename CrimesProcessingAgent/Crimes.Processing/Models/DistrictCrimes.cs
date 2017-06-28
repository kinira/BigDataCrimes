using System;
using System.Collections.Generic;
using System.Text;

namespace Crimes.Processing.Models
{
    public class DistrictCrimes
    {
        public int Year { get; set; }
        public int District { get; set; }
        public string CrimeType { get; set; }
        public int CrimAvg { get; set; }
    }
}
