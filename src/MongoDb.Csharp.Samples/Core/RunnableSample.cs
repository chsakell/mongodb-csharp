using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Core
{
    public abstract class RunnableSample
    {
        public bool Enabled
        {
            get
            {
                var enabled = Program.Configuration[$"Samples:{Sample}"];
                if (bool.TryParse(enabled, out var result))
                {
                    return result;
                }

                return true;
            }
        }

        protected RunnableSample() => Init();
        protected MongoClient Client { get; set; }
        public abstract Samples Sample { get; }

        protected abstract void Init();
    }
}
