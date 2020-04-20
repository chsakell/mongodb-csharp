using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Aggregation
{
    public class Bucket : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Aggregation_Stages_Bucket;
        protected override void Init()
        {
            // Create a mongodb client using a connection string
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Trips);
        }

        public async Task Run()
        {
            await BucketSamples();
        }

        private async Task BucketSamples()
        {
            var travelersCollectionName = "travelers";
            var tripsDatabase = Client.GetDatabase(Databases.Trips);
            var travelersCollection = tripsDatabase.GetCollection<Traveler>(travelersCollectionName);
            var travelersQueryableCollection = tripsDatabase.GetCollection<Traveler>(travelersCollectionName).AsQueryable();
            var travelersBsonCollection = tripsDatabase.GetCollection<BsonDocument>(travelersCollectionName);
            #region Prepare data

            await travelersCollection.InsertManyAsync(RandomData.GenerateTravelers(40, 5));

            #endregion

            #region Typed

            var aggregate = travelersCollection.Aggregate();

            var bucket = aggregate.Bucket(
                t => t.Age,
                new[] { 20, 32, 45, 60, 80 },
                g => new
                {
                    _id = default(int),
                    averageAge = g.Average(e => e.Age),
                    totalTravelers = g.Count()
                });

            var bucketResults = await bucket.ToListAsync();

            var autoBucket = aggregate.BucketAuto(t => t.Age, 4);

            var autoBucketResults = await autoBucket.ToListAsync();

            #endregion

            #region BsonDocument commands

            var bsonAggregate = travelersBsonCollection.Aggregate();
            var bsonGroupBy = (AggregateExpressionDefinition<BsonDocument, BsonValue>)"$age";
            var bsonBuckets = 4;
            var bsonOutput = (ProjectionDefinition<BsonDocument, BsonDocument>)
                "{ averageAge : { $avg : \"$age\" }, totalTravelers : { $sum : 1 } }";
            var bsonBucketAuto = bsonAggregate.BucketAuto(bsonGroupBy, bsonBuckets, bsonOutput);

            var bsonBucketResults = await bsonBucketAuto.ToListAsync();

            #endregion

            #region Shell commands

            #if false
            db.travelers.aggregate([
               {
                  "$bucket":{
                     "groupBy":"$age",
                     "boundaries":[20,32, 45,60,80],
                     "output":{
                        "averageAge":{
                           "$avg":"$age"
                        },
                        "totalTravelers":{
                           "$sum":1
                        }
                     }
                  }
               }
            ])
            #endif

            #endregion

        }
    }
}
