using System;

namespace MongoDb.Csharp.Samples.Models
{
    public class VisitedCountry
    {
        public string Name { get; set; }
        public int TimesVisited { get; set; }
        public DateTime LastDateVisited { get; set; }
    }
}
