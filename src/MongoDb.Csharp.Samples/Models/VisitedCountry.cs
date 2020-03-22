using System;

namespace MongoDb.Csharp.Samples.Models
{
    public class VisitedCountry
    {
        public string Name { get; set; }
        public int TimesVisited { get; set; }
        public DateTime LastDateVisited { get; set; }
        public GeoLocation Coordinates { get; set; }
    }

    public class GeoLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
