using System;
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
    public class Unwind : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Aggregation_Stages_Unwind;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Trips);
        }

        public async Task Run()
        {
            await ProjectionOperations();
        }

        private async Task ProjectionOperations()
        {
            var travelersCollectionName = "travelers";
            var tripsDatabase = Client.GetDatabase(Databases.Trips);
            var travelersCollection = tripsDatabase.GetCollection<Traveler>(travelersCollectionName);
            var travelersQueryableCollection = tripsDatabase.GetCollection<Traveler>(travelersCollectionName).AsQueryable();
            var travelersBsonCollection = tripsDatabase.GetCollection<BsonDocument>(travelersCollectionName);
            #region Prepare data

            await travelersCollection.InsertManyAsync(RandomData.GenerateTravelers(10, 5));

            #endregion

            #region Typed classes commands

            //var pipeLineDefinition = new PipelineStagePipelineDefinition<Traveler, object>(
            //{
            //    new 
            //});

            //travelersCollection.Aggregate()

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
                Utils.Log($"Age: {result.GetValue("_id").AsInt32} - activities: {result.GetValue("activities").AsBsonArray.ToString()}");
            }

            #endregion

            #region Linq

            var c = from t in travelersQueryableCollection
                from a in t.Activities
                group t by t.Age into g
                select new
                {
                    age = g.Key, activities = g.SelectMany(t => t.Activities)
                };

            var re = c.ToList();

            var query = travelersCollection.Aggregate()
                .Unwind(t => t.Activities)
                .Group(t => "$age", ac => new { age = ac.Key, activities = "ctivities") });

            var queryR = await query.ToListAsync();

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
