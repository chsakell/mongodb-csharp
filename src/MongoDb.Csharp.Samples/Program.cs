using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples
{
    class Program
    {
        
        static void Main(string[] args)
        {
            RegisterClasses();

            var samples  = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => typeof(IRunnableSample).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var sample in samples)
            {
                var instance = (IRunnableSample)Activator.CreateInstance(sample);
                if (instance != null && instance.Enabled)
                {
                    instance.Run();
                }
            }

            Console.ReadKey();
        }

        static void RegisterClasses()
        {
            BsonClassMap.RegisterClassMap<AppPerson>();
        }
    }
}
