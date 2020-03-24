using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDb.Csharp.Samples.Aggregation
{
    public class Slice : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Aggregation_Expressions_Slice;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Trips);
        }

        public async Task Run()
        {
            await SliceOperations();
        }

        private async Task SliceOperations()
        {
            var travelersCollectionName = "travelers";
            var tripsDatabase = Client.GetDatabase(Databases.Trips);
            var travelersCollection = tripsDatabase.GetCollection<Traveler>(travelersCollectionName);
            var travelersQueryableCollection = tripsDatabase.GetCollection<Traveler>(travelersCollectionName).AsQueryable();
            var travelersBsonCollection = tripsDatabase.GetCollection<BsonDocument>(travelersCollectionName);
            #region Prepare data

            await travelersCollection.InsertManyAsync(RandomData.GenerateTravelers(10, 5));

            #endregion

            #region Linq

            // $addToSet with Distinct
            var linqQuery = travelersQueryableCollection
                .SelectMany(t => t.Activities, (t, a) => new
                {
                    age = t.Age,
                    activity = a
                })
                .GroupBy(q => q.age)
                .Select(g => new { age = g.Key, activities = g.Select(a => a.activity).Distinct() })
                .OrderBy(r => r.age);

            var linqQueryResults = await linqQuery.ToListAsync();

            foreach (var result in linqQueryResults)
            {
                Utils.Log($"Age: {result.age} - activities: {string.Join(',', result.activities)}");
            }
            #endregion

            #region BsonDocument commands

            var bsonPipeline = new[]
            {
                new BsonDocument()
                {
                    {"$unwind", "$activities"}
                },
                new BsonDocument()
                {
                    {"$group", new BsonDocument()
                        {
                            {   "_id",  "$age" },
                            {
                                "activities", new BsonDocument()
                                {
                                    {"$addToSet", "$activities" }
                                }
                            }
                        }
                    }
                },
                new BsonDocument()
                {
                    {"$sort", new BsonDocument()
                    {
                        { "_id", 1 }
                    }}
                },
            };

            var results = await travelersBsonCollection.Aggregate<BsonDocument>(bsonPipeline).ToListAsync();

            foreach (var result in results)
            {
                //Utils.Log($"Age: {result.GetValue("_id").AsInt32} - activities: {result.GetValue("activities").AsBsonArray.ToString()}");
            }

            #endregion


            #region Shell commands

#if false
            db.travelers.aggregate()
                .unwind("$activities")
                .group({
                      _id: { age: "$age" }, activities: { $addToSet: "$activities" }
                })
#endif

            #endregion

        }
    }
}
