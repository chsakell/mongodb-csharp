using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using Utils = MongoDb.Csharp.Samples.Core.Utils;

namespace MongoDb.Csharp.Samples.Crud.Read.Query
{
    public class ArrayOperators : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Crud_Read_Query_ArrayOperators;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Constants.SamplesDatabase);
        }

        public async Task Run()
        {
            await ArrayOperatorsSamples();
        }

        private async Task ArrayOperatorsSamples()
        {
            var database = Client.GetDatabase(Constants.SamplesDatabase);
            var collection = database.GetCollection<Traveler>(Constants.TravelersCollection);
            var bsonCollection = database.GetCollection<BsonDocument>(Constants.TravelersCollection);

            #region Prepare data

            var travelers = RandomData.GenerateTravelers(500);
            await collection.InsertManyAsync(travelers);

            // ElemMatch
            var greeceAndItalyTravelers = RandomData.GenerateTravelers(15);
            foreach (var grcItTraveler in greeceAndItalyTravelers)
            {
                var firstCountry = RandomData.GenerateVisitedCountries(1).First();
                var secondCountry = RandomData.GenerateVisitedCountries(1).First();
                var random = new Faker().PickRandom(0, 1);
                switch (random)
                {
                    case 0:
                        firstCountry.Name = "Greece";
                        secondCountry.Name = "Italy";
                        break;
                    default:
                        firstCountry.Name = "Italy";
                        secondCountry.Name = "Greece";
                        break;
                }

                grcItTraveler.VisitedCountries = new List<VisitedCountry> { firstCountry, secondCountry };
            }

            await collection.InsertManyAsync(greeceAndItalyTravelers);

            #endregion

            #region Typed classes commands

            // Get all travelers that have visited Greece

            //same results
            var greeceTravelers = await collection.Find(t => t.VisitedCountries.
                Any(c => c.Name == "Greece")).ToListAsync();

            var italyTravelers = await collection.Find(t => t.VisitedCountries
                .Any(c => c.Name == "Italy")).ToListAsync();

            var greeceItalyTravelers = await collection.Find(t => t.VisitedCountries
                .Any(c => c.Name == "Greece" || c.Name == "Italy")).ToListAsync();
            // using filter - same results
            var greeceVisitedFilter = Builders<Traveler>.Filter.AnyEq("visitedCountries.name", "Greece");

            greeceTravelers = await collection.Find(greeceVisitedFilter).ToListAsync();
            Utils.Log($"{greeceTravelers.Count} total travelers have visited Greece");

            var visitedTimesFilter = Builders<Traveler>.Filter.AnyEq("visitedCountries.timesVisited", 3);

            var combinedFilter = Builders<Traveler>.Filter.And(greeceVisitedFilter, visitedTimesFilter);
            var wrongResult = await collection.Find(combinedFilter).ToListAsync();

            #region size

            var fiveVisitedCountriesFilter = await collection.Find(t => t.VisitedCountries.Count == 5).ToListAsync();
            Utils.Log($"{fiveVisitedCountriesFilter.Count} total travelers have visited 5 countries exactly");

            var moreThan10VisitedCountries = await collection.Find(t => t.VisitedCountries.Count > 10).ToListAsync();

            Utils.Log($"{moreThan10VisitedCountries.Count} total travelers have visited more than 10 countries");

            #endregion

            #region elemMatch

            var visitedGreeceExactly3Times = Builders<Traveler>.Filter.ElemMatch(t => t.VisitedCountries,
                country => country.Name == "Greece" && country.TimesVisited == 3);

            var visitedGreeceExactly3TimesTravelers = await collection.Find(visitedGreeceExactly3Times).ToListAsync();
            Utils.Log($"{visitedGreeceExactly3TimesTravelers.Count} total travelers have visited Greece exactly 3 times");

            #region multiple conditions

            var countryNameFilter = Builders<VisitedCountry>.Filter.In(c => c.Name, new[] {"Greece", "Italy"});
            var countryTimesVisitedFilter = Builders<VisitedCountry>.Filter.Eq(c => c.TimesVisited, 3);

            var visitedGreeceOrItalyExactly3Times = Builders<Traveler>.Filter
                .ElemMatch(t => t.VisitedCountries,
                        Builders<VisitedCountry>.Filter.And(countryNameFilter, countryTimesVisitedFilter));


            var visitedGreeceOrItalyExactly3TimesTravelers = await collection.Find(visitedGreeceOrItalyExactly3Times).ToListAsync();
            Utils.Log($"{visitedGreeceOrItalyExactly3TimesTravelers.Count} total travelers have visited Greece or Italy exactly 3 times");
            #endregion

            #endregion

            #region All

            // Order doesn't matter - items are included on the array
            var climbingAndBackpackingFilter = Builders<Traveler>.Filter
                .All(t => t.Activities, new List<string> { "Backpacking", "Climbing" });

            var climbingAndBackpackingTravelers = await collection.Find(climbingAndBackpackingFilter).ToListAsync();
            Utils.Log($"{climbingAndBackpackingTravelers.Count} total travelers have 'Backpacking' & 'Climbing' activities");
            #endregion

            #endregion

            #region BsonDocument commands
            var bsonGreeceVisitedFilter = Builders<BsonDocument>.Filter.AnyEq("visitedCountries.name", "Greece");
            var bsonGreeceTravelers = await bsonCollection.Find(bsonGreeceVisitedFilter).ToListAsync();

            #region size

            var bsonFiveVisitedCountriesFilter = await bsonCollection.Find(
                new BsonDocument
                { 
                    {"visitedCountries", new BsonDocument {{ "$size", 5 } }}
                }).ToListAsync();

            var bsonMoreThan10VisitedCountries = await bsonCollection
                .Find(new BsonDocument
                {
                    {"visitedCountries.10", new BsonDocument {{ "$exists", true } }}
                }).ToListAsync();

            Utils.Log($"{moreThan10VisitedCountries.Count} total travelers have visited more than 10 countries");

            #endregion

            #region elemmMatch

            var bsonVisitedGreeceExactly3Times = Builders<BsonDocument>.Filter
                .ElemMatch<BsonValue>("visitedCountries", new BsonDocument { { "name", "Greece" }, { "timesVisited", 3 } });

            var bsonVisitedGreeceExactly3TimesTravelers = await bsonCollection.Find(bsonVisitedGreeceExactly3Times).ToListAsync();

            #region multiple conditions

            var bsonVisitedGreeceOrItalyExactly3Times = Builders<BsonDocument>.Filter
                .ElemMatch<BsonValue>("visitedCountries", new BsonDocument
                {
                    { "name", new BsonDocument("$in", new BsonArray { "Greece", "Italy" }) }, 
                    { "timesVisited", 3 }
                });

            var bsonVisitedGreeceOrItalyExactly3TimesTravelers = await bsonCollection.Find(bsonVisitedGreeceOrItalyExactly3Times).ToListAsync();

            #endregion

            #endregion

            #region All

            var bsonClimbingAndBackpackingFilter = Builders<BsonDocument>.Filter
                .All("activities", new List<string> { "Backpacking", "Climbing" });

            var bsonClimbingAndBackpackingTravelers = await bsonCollection.Find(bsonClimbingAndBackpackingFilter).ToListAsync();

            #endregion

            #endregion

            #region Shell commands

#if false
            db.travelers.find({ "visitedCountries.name" : "Greece" })
            db.travelers.find({ visitedCountries : { $size: 5 } }).count()
            db.travelers.find({ "visitedCountries.10" : { "$exists" : true } })
            db.travelers.find({ activities: { $all : [ "Climbing", "Backpacking" ] } }
            db.travelers.find({
                visitedCountries: {
                    $elemMatch: {
                        name : "Greece",
                        timesVisited: 3
                    }
                }})
            db.travelers.find(
            {
                visitedCountries: {
                    $elemMatch: {
                        name : { $in: [ "Greece", "Italy" ] },
                        timesVisited: 3
                    }
                }
            })
#endif


            #endregion
        }

    }
}
