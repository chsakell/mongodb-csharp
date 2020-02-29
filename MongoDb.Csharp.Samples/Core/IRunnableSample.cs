using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDb.Csharp.Samples.Core
{
    public interface IRunnableSample
    {
        void Run();
        bool Enabled { get; }
    }
}
