using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;

namespace MongoDb.Csharp.Samples
{
    class Program
    {
        
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            await using (var serviceProvider = services.BuildServiceProvider())
            {
                SetCamelCaseConventionPack();
                RegisterClasses();
                RegisterSerializers();

                var samples = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(t => typeof(IRunnableSample).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var sample in samples)
                {
                    var instance = (IRunnableSample)Activator.CreateInstance(sample);
                    if (instance != null && instance.Enabled)
                    {
                        await instance.Run();
                    }
                }
            }
            

            Console.ReadKey();
        }

        static void ConfigureServices(ServiceCollection services)
        {
            
        }

        static void SetCamelCaseConventionPack()
        {
            var pack = new ConventionPack {new CamelCaseElementNameConvention()};
            ConventionRegistry.Register("camel case", pack, t => true);
        }

        static void RegisterClasses()
        {
            BsonClassMap.RegisterClassMap<AppPerson>();
        }

        static void RegisterSerializers()
        {
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
            BsonSerializer.RegisterSerializer(typeof(decimal?),
                new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));
        }
    }
}
