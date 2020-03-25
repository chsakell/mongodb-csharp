using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDb.Csharp.Samples.Expressions
{
    public class Slice : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Expressions_Slice;
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

            // $slice with Distinct

            var sliceQuery = from t in travelersQueryableCollection
                select new {t.Name, visitedCountries = t.VisitedCountries.Take(1)};
            
            var sliceQueryResults = await sliceQuery.ToListAsync();

            foreach (var result in sliceQueryResults)
            {
                Utils.Log($"Age: {result.Name} - countries: {string.Join(',', result.visitedCountries.FirstOrDefault()?.Name)}");
            }
            
            var sliceProjection = Builders<Traveler>.Projection.Expression(u =>
                new
                {
                    name = u.Name,
                    visitedCountries = u.VisitedCountries.Take(1)
                });

            var sliceProjectionResults = await travelersCollection.Find(Builders<Traveler>.Filter.Empty)
                .Project(sliceProjection)
                .ToListAsync();


            // retrieve the last two visited countries
            var sliceQueryTwoLastCountries = from t in travelersQueryableCollection
                select new { t.Name, visitedCountries = t.VisitedCountries.Take(-2) };

            var sliceQueryTwoLastCountriesResults = await sliceQueryTwoLastCountries.ToListAsync();

            #endregion

            #region BsonDocument commands

            var bsonSlicePipeline = new[]
            {
                new BsonDocument()
                {
                    {"$project", new BsonDocument()
                        {
                            {   "name",  1 },
                            {
                                "visitedCountries", new BsonDocument()
                                {
                                    {"$slice", new BsonArray() { "$visitedCountries", 1 } }
                                }
                            }
                        }
                    }
                }
            };

            var bsonSlicePipelineResults = await travelersBsonCollection.Aggregate<BsonDocument>(bsonSlicePipeline).ToListAsync();

            foreach (var result in bsonSlicePipelineResults)
            {
                
            }

            #endregion


            #region Shell commands

#if false
               db.travelers.aggregate()
               .project({ name: 1, visitedCountries : { $slice: ["$visitedCountries", 1] } })
               .pretty()

               db.travelers.aggregate()
               .project({ name: 1, visitedCountries : { $slice: ["$visitedCountries", -2] } })
               .pretty()
#endif

            #endregion

        }
    }
}
