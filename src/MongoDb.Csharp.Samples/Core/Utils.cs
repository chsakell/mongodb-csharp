


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
        private static string EmptySpace = "                     ";

        public static void Log(string message)
        {
            Debug.WriteLine(message);
            Debug.WriteLine(string.Empty);
        }
        public static void Log(BsonDocument doc)
        {
            Debug.WriteLine(JsonConvert.SerializeObject(BsonTypeMapper.MapToDotNetValue(doc), Formatting.Indented));
        }

        public static void Log(BsonDocument doc, string message)
        {
            Log(new List<BsonDocument>() {doc}, message);
        }

        public static void Log(IEnumerable<BsonDocument> docs, string message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Debug.WriteLine($"{EmptySpace}{message}");
            }
            foreach (var doc in docs)
            {
                Log(doc);
            }

            Debug.WriteLine(string.Empty);
        }

        public static void DropDatabase(MongoClient client, string database)
        {
            // Drops the database with the specified name
            client.DropDatabase(database);
        }
    }
}
