using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Update
{
    public class UpdatingArrays : RunnableSample, IRunnableSample
    {

        public override Core.Samples Sample => Core.Samples.Crud_Update_UpdatingArrays;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Trips);
        }

        public async Task Run()
        {
            await UpdatingArraysDefinitions();
        }

        private async Task UpdatingArraysDefinitions()
        {
            var collectionName = "travelers";
            var database = Client.GetDatabase(Databases.Trips);
            var collection = database.GetCollection<Traveler>(collectionName);
            var bsonCollection = database.GetCollection<BsonDocument>(collectionName);
            #region Prepare data

            await collection.InsertManyAsync(RandomData.GenerateTravelers(1000));

            #endregion

            #region Typed classes commands

            #region update matched array elements
            
            var visitedGreeceExactly3Times = Builders<Traveler>.Filter
                .ElemMatch(t => t.VisitedCountries,
                    country => country.Name == "Greece" && country.TimesVisited == 3);

            var updateDefinition = Builders<Traveler>.Update.Set(t => t.VisitedCountries[-1].Name, "Hellas");

            // this will update only the first matching array element! ($) refers to the first match
            var updateHellasResult = await collection.UpdateManyAsync(visitedGreeceExactly3Times, updateDefinition);
            Utils.Log($"{updateHellasResult.ModifiedCount} visited countries have been updated");

            #endregion

            #region update all matched array elements

            var visitedHellasExactly3Times = Builders<Traveler>.Filter
                .ElemMatch(t => t.VisitedCountries,
                    country => country.Name == "Hellas" && country.TimesVisited == 3);

            // TODO : more with projection
            var updateGreeceDefinition = Builders<Traveler>.Update.Inc("visitedCountries.$[].timesVisited", 10);
            var updateGreeceResult = await collection.UpdateManyAsync(visitedHellasExactly3Times, updateGreeceDefinition);

            #endregion

            #endregion

            #region BsonDocument commands

            #region update matched array elements
            var bsonVisitedGreeceExactly3Times = Builders<BsonDocument>.Filter
                .ElemMatch<BsonValue>("visitedCountries", new BsonDocument { { "name", "Greece" }, { "timesVisited", 3 } });
            
            var bsonUpdateDefinition = Builders<BsonDocument>.Update.Set("visitedCountries.$.name", "Hellas");

            var bsonUpdateHellasResult = await bsonCollection
                .UpdateManyAsync(bsonVisitedGreeceExactly3Times, bsonUpdateDefinition);

            #endregion

            #region update all matched array elements

            var bsonVisitedHellasExactly3Times = Builders<BsonDocument>.Filter
                .ElemMatch<BsonValue>("visitedCountries", new BsonDocument { { "name", "Hellas" }, { "timesVisited", 3 } });

            // TODO : more with projection
            var bsonUpdateGreeceDefinition = Builders<BsonDocument>.Update.Inc("visitedCountries.$[].timesVisited", 10);
            var bsonUpdateGreeceResult = await bsonCollection
                .UpdateManyAsync(bsonVisitedHellasExactly3Times, bsonUpdateGreeceDefinition);

            #endregion

            #endregion

            #region Shell commands

#if false
            db.travelers.updateMany(
                { visitedCountries: { $elemMatch: { name: "Hellas", timesVisited: 3 }}},
                { $set: { "visitedCountries.$.name": "Greece" } }
            )
            
            db.travelers.updateMany(
                { visitedCountries: { $elemMatch: { name: "Hellas", timesVisited: 10 }}},
                { $inc: { "visitedCountries.$[].timesVisited": 100 } }
            )
#endif

            #endregion
        }
    }
}
