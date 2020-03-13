using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Read.Query
{
    public class ArrayOperators : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Crud_Read_Query_ArrayOperators;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Trips);
        }

        public async Task Run()
        {
            await ArrayOperatorsOperations();
        }

        private async Task ArrayOperatorsOperations()
        {
            var collectionName = "travelers";
            var database = Client.GetDatabase(Databases.Trips);
            var collection = database.GetCollection<Traveler>(collectionName);
            var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

            #region Prepare data

            var travelers = RandomData.GenerateTravelers(1000);

            await collection.InsertManyAsync(travelers);

            #endregion

            #region Typed classes commands

            // Get all travelers that have visited Greece
            
            //same results
            var greeceTravelers = await collection.Find(t => t.VisitedCountries.Any(c => c.Name == "Greece")).ToListAsync();

            Utils.Log($"{greeceTravelers.Count} total travelers have visited Greece");

            // using filter - same results
            var stringGreeceVisitedFilter = Builders<Traveler>.Filter.AnyEq("visitedCountries.name", "Greece");
            greeceTravelers = await collection.Find(stringGreeceVisitedFilter).ToListAsync();

            #endregion

            #region BsonDocument commands
            var bsonGreeceVisitedFilter = Builders<BsonDocument>.Filter.AnyEq("visitedCountries.name", "Greece");

            var bsonGreeceTravelers = await bsonCollection.Find(bsonGreeceVisitedFilter).ToListAsync();


            #endregion

            #region Shell commands

#if false
            db.travelers.find({ "visitedCountries.name" : "Greece" })
#endif


            #endregion
        }

    }
    }
