using System;
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

            await collection.InsertManyAsync(RandomData.GenerateTravelers(500));

            #endregion

            #region Typed classes commands

            #region Adding array items

            var firstTraveler = Builders<Traveler>.Filter.Empty;
            var visitedCountry = RandomData.GenerateVisitedCountries(1).First();
            visitedCountry.Name = "South Korea";
            visitedCountry.TimesVisited = 5;
            visitedCountry.LastDateVisited = DateTime.UtcNow.AddYears(5);

            var pushCountryDefinition = Builders<Traveler>.Update.Push(t => t.VisitedCountries, visitedCountry);
            var addNewVisitedCountryResult = await collection.UpdateOneAsync(firstTraveler, pushCountryDefinition);
            Utils.Log("South Korea has been added to user's visited countries");

            var newVisitedCountries = RandomData.GenerateVisitedCountries(10);
            var pushCountriesDefinition = Builders<Traveler>.Update
                .PushEach(t => t.VisitedCountries, newVisitedCountries);

            var addNewVisitedCountriesResult = await collection
                .UpdateOneAsync(firstTraveler, pushCountriesDefinition);

            #endregion

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

            // TODO : more with Aggregation Pipeline
            var updateGreeceDefinition = Builders<Traveler>.Update.Inc("visitedCountries.$[].timesVisited", 10);
            var updateGreeceResult = await collection.UpdateManyAsync(visitedHellasExactly3Times, updateGreeceDefinition);

            // TODO : more with Aggregation Pipeline
            
            var updateExactVisitedDefinition = Builders<Traveler>.Update.Inc("visitedCountries.$[el].timesVisited", 10);
            var updateExactVisitedResult = await collection.UpdateManyAsync(
                Builders<Traveler>.Filter
                    .ElemMatch(t => t.VisitedCountries,country => country.Name == "Hellas")
                , updateExactVisitedDefinition, 
                new UpdateOptions()
                {
                    ArrayFilters = new List<ArrayFilterDefinition<VisitedCountry>>()
                    {
                        "{ $and: [{ 'el.timesVisited': 13 }, { 'el.name': 'Hellas'} ] }"
                    }
                });

            #endregion



            // TODO: pull/pop

            #endregion

            #region BsonDocument commands

            #region Adding array items

            var bsonFirstUser = Builders<BsonDocument>.Filter.Empty;
            var bsonVisitedCountry = RandomData.GenerateVisitedCountries(1).First();
            visitedCountry.Name = "North Korea";
            visitedCountry.TimesVisited = 5;
            visitedCountry.LastDateVisited = DateTime.UtcNow.AddYears(5);

            var bsonPushCountryDefinition = Builders<BsonDocument>.Update
                .Push("visitedCountries", visitedCountry.ToBsonDocument());

            var bsonAddNewVisitedCountryResult = await bsonCollection
                .UpdateOneAsync(bsonFirstUser, bsonPushCountryDefinition);

            var bsonNewVisitedCountries = RandomData.GenerateVisitedCountries(10);
            var bsonPushCountriesDefinition = Builders<BsonDocument>.Update
                .PushEach("visitedCountries", bsonNewVisitedCountries);

            var bsonAddNewVisitedCountries = await bsonCollection
                .UpdateOneAsync(bsonFirstUser, bsonPushCountriesDefinition);

            #endregion

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


            db.travelers.updateOne( {}, { 
                $push: { visitedCountries: { name: "My Own Country", visitedTimes: 2, lastDateVisited: ISODate("2018-07-11T10:00:23.454Z") } }
            })

            db.travelers.updateOne({}, {
                $push: {
                    visitedCountries: {
                        $each: [
                            {
                                "name": "South Korea",
                                "timesVisited": 5,
                                "lastDateVisited": ISODate("2025-04-21T19:27:38.700+03:00"),
                                "coordinates": {
                                    "latitude": 4.2429,
                                    "longitude": -148.3179
                                }
                            },
                            {
                                "name": "Mozambique",
                                "timesVisited": 6,
                                "lastDateVisited": ISODate("2017-01-20T14:42:27.786+02:00"),
                                "coordinates": {
                                    "latitude": -45.9077,
                                    "longitude": -121.6868
                                }
                            }
                        ]
                    }
                }
            })

#endif

            #endregion
        }
    }
}
