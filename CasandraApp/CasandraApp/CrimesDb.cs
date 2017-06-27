using Cassandra;
using System;

namespace CasandraApp
{
    public class Crimes
    {
        public int Id { get; set; }
        public string CaseNumber { get; set; }
        public LocalDate CrimeDate { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public string Block { get; set; }
        public string PrimaryType { get; set; }
        public string LocationDescription { get; set; }
        public int District { get; set; }
        public double X_Coordinate { get; set; }
        public double Y_Coordinate { get; set; }
        public int Year { get; set; }
        public LocalDate Updated_On { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
