using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Csharp.Samples.Core
{
    public interface IRunnableSample
    {
        Task Run();
        bool Enabled { get; }
    }
}
