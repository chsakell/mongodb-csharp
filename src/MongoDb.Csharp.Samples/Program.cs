using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
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
                RegisterClasses();

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

        static void RegisterClasses()
        {
            BsonClassMap.RegisterClassMap<AppPerson>();
        }
    }
}
