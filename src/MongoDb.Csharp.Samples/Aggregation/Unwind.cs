﻿using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDb.Csharp.Samples.Aggregation
{
    public class UnwindStage : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Aggregation_Stages_Unwind;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Constants.SamplesDatabase);
        }

        public async Task Run()
        {
            await UnWindSamples();
        }

        private async Task UnWindSamples()
        {
            var tripsDatabase = Client.GetDatabase(Constants.SamplesDatabase);
            var travelersCollection = tripsDatabase.GetCollection<Traveler>(Constants.TravelersCollection);
            var travelersQueryableCollection = tripsDatabase.GetCollection<Traveler>(Constants.TravelersCollection).AsQueryable();
            var travelersBsonCollection = tripsDatabase.GetCollection<BsonDocument>(Constants.TravelersCollection);
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
