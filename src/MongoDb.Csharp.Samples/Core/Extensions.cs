using System.Linq;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Core
{
    public static class Extensions
    {
        public static FilterDefinition<T> Nor<T>(this FilterDefinitionBuilder<T> builder, 
            params FilterDefinition<T>[] filters)
        {
            return builder.And(
                filters.Select(builder.Not)
            );
        }
    }
}
