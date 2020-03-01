


using System;
using System.Collections.Generic;
using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace MongoDb.Csharp.Samples.Core
{
    public class Utils
    {
        public static string DefaultConnectionString = "mongodb://localhost:27017";

        public static void Log(BsonDocument doc)
        {
            Debug.WriteLine(JsonConvert.SerializeObject(BsonTypeMapper.MapToDotNetValue(doc)));
        }

        public static void Log(IEnumerable<BsonDocument> docs)
        {
            foreach (var doc in docs)
            {
                Log(doc);
            }
        }

        public static void DropDatabase(MongoClient client, string database)
        {
            // Drops the database with the specified name
            client.DropDatabase(database);
        }
    }
}
