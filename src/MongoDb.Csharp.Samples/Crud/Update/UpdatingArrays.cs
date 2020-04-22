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
            Utils.DropDatabase(Client, Databases.GenericUseDb);
        }

        public async Task Run()
        {
            await UpdatingArraysDefinitions();
        }

        private async Task UpdatingArraysDefinitions()
        {
            var travelerCollectionName = "travelers";
            var storesCollectionName = "stores";
            var tripsDatabase = Client.GetDatabase(Databases.Trips);
            var genericDatabase = Client.GetDatabase(Databases.GenericUseDb);
            var travelersCollection = tripsDatabase.GetCollection<Traveler>(travelerCollectionName);
            var bsonTravelersCollection = tripsDatabase.GetCollection<BsonDocument>(travelerCollectionName);
            var storesCollection = genericDatabase.GetCollection<StoreItem>(storesCollectionName);
            var bsonStoresCollection = genericDatabase.GetCollection<BsonDocument>(storesCollectionName);
            #region Prepare data

            await travelersCollection.InsertManyAsync(RandomData.GenerateTravelers(2));

            #endregion

            #region Typed classes commands

            #region Adding array items

            var firstTraveler = Builders<Traveler>.Filter.Empty;
            var visitedCountry = RandomData.GenerateVisitedCountries(1).First();
            visitedCountry.Name = "South Korea";
            visitedCountry.TimesVisited = 5;
            visitedCountry.LastDateVisited = DateTime.UtcNow.AddYears(5);

            var pushCountryDefinition = Builders<Traveler>.Update.Push(t => t.VisitedCountries, visitedCountry);
            var addNewVisitedCountryResult = await travelersCollection.UpdateOneAsync(firstTraveler, pushCountryDefinition);
            Utils.Log("South Korea has been added to user's visited countries");

            var newVisitedCountries = RandomData.GenerateVisitedCountries(10);
            var pushCountriesDefinition = Builders<Traveler>.Update
                .PushEach(t => t.VisitedCountries, newVisitedCountries);

            var addNewVisitedCountriesResult = await travelersCollection
                .UpdateOneAsync(firstTraveler, pushCountriesDefinition);

            #endregion

            #region remove array items

            // add two items
            var storeItems = new List<StoreItem>
            {
                new StoreItem()
                {
                    PcGames = new List<string>
                    {
                        "Football Manager", "DOOM Eternal", 
                        "FIFA 20", "Grand Theft Auto", "NBA 2K17"
                    },
                    XboxGames = new List<string> 
                    { 
                        "Forza Horizon", "Call of Duty", 
                        "Mortal Kombat", "Gears 5"
                    }
                },
                new StoreItem()
                {
                    PcGames = new List<string>
                    {
                        "Assassin's Creed", "Final Fantasy", 
                        "The Sims", "Football Manager", "FIFA 20"
                    },
                    XboxGames = new List<string>
                    {
                        "Resident Evil", "Forza Motorsport", 
                        "Battlefield", "Halo 5 Guardians", "Mortal Kombat"
                    }
                }
            };

            await storesCollection.InsertManyAsync(storeItems);

            var storeEmptyFilter = Builders<StoreItem>.Filter.Empty;
            var removePcGames = new List<string> { "FIFA 20", "NBA 2K17" };
            var removeXboxGames = new List<string> { "Mortal Kombat" };

            var pullPcGamesDefinition = Builders<StoreItem>.Update.PullFilter(s => s.PcGames,
                game => removePcGames.Contains(game));
            var pullXboxGamesDefinition = Builders<StoreItem>.Update.PullFilter(s => s.XboxGames,
                game => removeXboxGames.Contains(game));

            var pullCombined = Builders<StoreItem>.Update
                .Combine(pullPcGamesDefinition, pullXboxGamesDefinition);

            var simplePullResult = await storesCollection
                .UpdateManyAsync(storeEmptyFilter, pullPcGamesDefinition);

            // reset collection
            await genericDatabase.DropCollectionAsync(storesCollectionName);
            await storesCollection.InsertManyAsync(storeItems);

            var removeUpdateResult = await storesCollection
                .UpdateManyAsync(storeEmptyFilter, pullCombined);

            // remove embedded document

            await travelersCollection.InsertManyAsync(RandomData.GenerateTravelers(10, 15));
            var visited8Times = Builders<Traveler>.Filter
                .ElemMatch(t => t.VisitedCountries, country =>  country.TimesVisited == 8);

            var totalDocVisited8Times = await travelersCollection
                    .Find(visited8Times).CountDocumentsAsync();

            var pullVisited8TimesDefinition = Builders<Traveler>.Update
                .PullFilter(t => t.VisitedCountries,
                country => country.TimesVisited == 8);

            var visited8TimesResult = await travelersCollection
                .UpdateManyAsync(visited8Times, pullVisited8TimesDefinition);

            Utils.Log($"{totalDocVisited8Times} document found with TimesVisited = 8 and {visited8TimesResult.ModifiedCount} removed");

            #endregion

            #region update matched array elements

            var visitedGreeceExactly3Times = Builders<Traveler>.Filter
                .ElemMatch(t => t.VisitedCountries,
                    country => country.Name == "Greece" && country.TimesVisited == 3);

            var updateDefinition = Builders<Traveler>.Update.Set(t => t.VisitedCountries[-1].Name, "Hellas");

            // this will update only the first matching array element! ($) refers to the first match
            var updateHellasResult = await travelersCollection.UpdateManyAsync(visitedGreeceExactly3Times, updateDefinition);
            Utils.Log($"{updateHellasResult.ModifiedCount} visited countries have been updated");

            #endregion

            #region update all matched array elements

            var visitedHellasExactly3Times = Builders<Traveler>.Filter
                .ElemMatch(t => t.VisitedCountries,
                    country => country.Name == "Greece" && country.TimesVisited == 3);

            // TODO : more with Aggregation Pipeline
            var updateGreeceDefinition = Builders<Traveler>.Update.Inc("visitedCountries.$[].timesVisited", 10);
            var updateGreeceResult = await travelersCollection.UpdateManyAsync(visitedHellasExactly3Times, updateGreeceDefinition);

            // TODO : more with Aggregation Pipeline

            var updateExactVisitedDefinition = Builders<Traveler>.Update.Inc("visitedCountries.$[el].timesVisited", 10);
            var updateExactVisitedResult = await travelersCollection.UpdateManyAsync(
                Builders<Traveler>.Filter
                    .ElemMatch(t => t.VisitedCountries, country => country.Name == "Hellas")
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

            var bsonAddNewVisitedCountryResult = await bsonTravelersCollection
                .UpdateOneAsync(bsonFirstUser, bsonPushCountryDefinition);

            var bsonNewVisitedCountries = RandomData.GenerateVisitedCountries(10);
            var bsonPushCountriesDefinition = Builders<BsonDocument>.Update
                .PushEach("visitedCountries", bsonNewVisitedCountries);

            var bsonAddNewVisitedCountries = await bsonTravelersCollection
                .UpdateOneAsync(bsonFirstUser, bsonPushCountriesDefinition);
            
            var bsonVisited9Times = Builders<BsonDocument>.Filter
                .ElemMatch<BsonValue>("visitedCountries", new BsonDocument { { "timesVisited", 9 } });

            var bsonTotalDocVisited9Times = await bsonTravelersCollection
                .Find(bsonVisited9Times).CountDocumentsAsync();

            var bsonPullVisited9TimesDefinition = Builders<BsonDocument>.Update
                .PullFilter<BsonValue>("visitedCountries",
                    new BsonDocument { { "timesVisited", 9 } });

            var bsonVisited9TimesResult = await bsonTravelersCollection
                .UpdateManyAsync(bsonVisited9Times, bsonPullVisited9TimesDefinition);

            #endregion

            #region update matched array elements
            var bsonVisitedGreeceExactly3Times = Builders<BsonDocument>.Filter
                .ElemMatch<BsonValue>("visitedCountries", new BsonDocument { { "name", "Greece" }, { "timesVisited", 3 } });

            var bsonUpdateDefinition = Builders<BsonDocument>.Update.Set("visitedCountries.$.name", "Hellas");

            var bsonUpdateHellasResult = await bsonTravelersCollection
                .UpdateManyAsync(bsonVisitedGreeceExactly3Times, bsonUpdateDefinition);

            #endregion

            #region update all matched array elements

            var bsonVisitedHellasExactly3Times = Builders<BsonDocument>.Filter
                .ElemMatch<BsonValue>("visitedCountries", new BsonDocument { { "name", "Hellas" }, { "timesVisited", 3 } });

            // TODO : more with projection
            var bsonUpdateGreeceDefinition = Builders<BsonDocument>.Update.Inc("visitedCountries.$[].timesVisited", 10);
            var bsonUpdateGreeceResult = await bsonTravelersCollection
                .UpdateManyAsync(bsonVisitedHellasExactly3Times, bsonUpdateGreeceDefinition);

            #endregion

            #endregion

            #region Shell commands

#if false
            db.travelers.updateMany(
                {
                    "visitedCountries": {
                        "$elemMatch": {
                            "timesVisited": 8
                        }
                    },
                },
                {
                    "$pull": {
                        "visitedCountries": {
                            "timesVisited": 8
                        }
                    }
                }
            )

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
