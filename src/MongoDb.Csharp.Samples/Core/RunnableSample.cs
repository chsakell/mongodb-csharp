using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Core
{
    public abstract class RunnableSample
    {
        public RunnableSample() => Init();
        protected MongoClient Client { get; set; }
        protected abstract Samples Sample { get; }

        protected abstract void Init();
    }
}
