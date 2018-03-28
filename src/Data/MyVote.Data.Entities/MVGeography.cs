using System;
using System.Collections.Generic;

namespace MyVote.Data.Entities
{
    public partial class Mvgeography
    {
        public int GeographyKey { get; set; }
        public string Zip { get; set; }
        public string PrimaryCity { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public string TimeZone { get; set; }
        public string AreaCodes { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string EstimatedPopulation { get; set; }
    }
}
