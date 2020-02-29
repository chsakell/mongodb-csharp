using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDb.Csharp.Samples.Models
{
    public class Database
    {
        public string Name { get; set; }
        public Decimal SizeOnDisk { get; set; }
        public bool Empty { get; set; }
    }
}
