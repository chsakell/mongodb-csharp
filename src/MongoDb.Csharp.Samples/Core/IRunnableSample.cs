using System.Threading.Tasks;

namespace MongoDb.Csharp.Samples.Core
{
    public interface IRunnableSample
    {
        Task Run();
        bool Enabled { get; }
        Samples Sample { get; }
    }
}
