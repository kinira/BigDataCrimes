using Newtonsoft.Json;
using System;

namespace CasandraApp
{
    public class CrimesJson
    {
        public int Id { get; set; }
        [JsonProperty("case_number")]
        public string CaseNumber { get; set; }
        public DateTime Date { get; set; }
        public string Block { get; set; }
        [JsonProperty("primary_type")]
        public string PrimaryType { get; set; }
        [JsonProperty("location_description")]
        public string LocationDescription { get; set; }
        public int District { get; set; }
        [JsonProperty("x_coordinate")]
        public double X_Coordinate { get; set; }
        [JsonProperty("y_coordinate")]
        public double Y_Coordinate { get; set; }
        public int Year { get; set; }
        public DateTime Updated_On { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
