using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDb.Csharp.Samples.Aggregation
{
    public class Bucket : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Aggregation_Stages_Bucket;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Trips);
        }

        public async Task Run()
        {
            await BucketOperations();
        }

        private async Task BucketOperations()
        {
            var travelersCollectionName = "travelers";
            var tripsDatabase = Client.GetDatabase(Databases.Trips);
            var travelersCollection = tripsDatabase.GetCollection<Traveler>(travelersCollectionName);
            var travelersQueryableCollection = tripsDatabase.GetCollection<Traveler>(travelersCollectionName).AsQueryable();
            var travelersBsonCollection = tripsDatabase.GetCollection<BsonDocument>(travelersCollectionName);
            #region Prepare data

            await travelersCollection.InsertManyAsync(RandomData.GenerateTravelers(10, 5));

            #endregion

            #region Typed

            

            #endregion

            #region BsonDocument commands

            var bsonAggregate = travelersBsonCollection.Aggregate();
            var bsonGroupBy = (AggregateExpressionDefinition<BsonDocument, BsonValue>)"$age";
            var bsonBuckets = 4;
            var bsonOutput = (ProjectionDefinition<BsonDocument, BsonDocument>)"{ averageAge : { $avg : \"$age\" }, totalTravelers : { $sum : 1 } }";
            var bsonBucketAuto = bsonAggregate.BucketAuto(bsonGroupBy, bsonBuckets, bsonOutput);

            var bsonBucketResults = await bsonBucketAuto.ToListAsync();

            #endregion


            #region Shell commands

#if false

#endif

            #endregion

        }
    }
}
