using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
        public static IConfiguration Configuration;
        public static BsonClassMap<User> DefaultUserClassMap;
        public static BsonClassMap<Order> DefaultOrderClassMap;
        public static BsonClassMap<ShipmentDetails> DefaultShipmentDetailsClassMap;
        public static BsonClassMap<Traveler> DefaultTravelerClassMap;
        public static BsonClassMap<VisitedCountry> DefaultVisitedCountryClassMap;
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
                        try
                        {
                            Utils.Log($"Running {instance.Sample} sample..");
                            await instance.Run();
                        }
                        catch (Exception e)
                        {
                            Utils.Log($"Exception running {instance.Sample} sample");
                            Console.WriteLine(e);
                        }
                    }
                }
            }
            
            Utils.Log("Press any key to exit...");
            Console.ReadKey();
        }

        static void ConfigureServices(ServiceCollection services)
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
        }

        static void SetCamelCaseConventionPack()
        {
            var pack = new ConventionPack {new CamelCaseElementNameConvention()};
            ConventionRegistry.Register("camel case", pack, t => true);
        }

        static void RegisterClasses()
        {
            DefaultUserClassMap = BsonClassMap.RegisterClassMap<User>();
            DefaultOrderClassMap = BsonClassMap.RegisterClassMap<Order>();
            DefaultShipmentDetailsClassMap = BsonClassMap.RegisterClassMap<ShipmentDetails>();
            DefaultTravelerClassMap = BsonClassMap.RegisterClassMap<Traveler>();
            DefaultVisitedCountryClassMap = BsonClassMap.RegisterClassMap<VisitedCountry>();
        }

        static void RegisterSerializers()
        {
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
            BsonSerializer.RegisterSerializer(typeof(decimal?),
                new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));
        }
    }
}
