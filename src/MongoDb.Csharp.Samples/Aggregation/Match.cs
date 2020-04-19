using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Aggregation
{
    public class MatchStage : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Aggregation_Stages_Match;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Persons);
            Utils.DropDatabase(Client, Databases.Trips);
        }

        public async Task Run()
        {
            await MatchStageSamples();
        }

        private async Task MatchStageSamples()
        {
            var usersCollection = "users";
            var tripsCollection = "trips";
            var personsDatabase = Client.GetDatabase(Databases.Persons);
            var travelersDatabase = Client.GetDatabase(Databases.Trips);
            var personsCollection = personsDatabase.GetCollection<User>(usersCollection);
            var personsBsonCollection = personsDatabase.GetCollection<BsonDocument>(usersCollection);
            var travelersCollection = travelersDatabase.GetCollection<Traveler>(tripsCollection);
            var travelersBsonCollection = travelersDatabase.GetCollection<BsonDocument>(tripsCollection);
            #region Prepare data

            await personsCollection.InsertManyAsync(RandomData.GenerateUsers(500));
            await travelersCollection.InsertManyAsync(RandomData.GenerateTravelers(500));
            #endregion

            #region Typed classes commands

            var aggregate = personsCollection.Aggregate()
                .Match(Builders<User>.Filter.Gte(u => u.Salary, 3500) &
                       Builders<User>.Filter.Lte(u => u.Salary, 5000));

            var highSalaryUsers = await aggregate.ToListAsync();
            Utils.Log($"{highSalaryUsers.Count} users found with high salary");

            var visitedGreeceExactly3Times = Builders<Traveler>.Filter
                .ElemMatch(t => t.VisitedCountries,
                country => country.Name == "Greece" && country.TimesVisited == 3);

            // match with project
            var simpleProjection = Builders<Traveler>.Projection
                .Include(t => t.Name)
                .Include(t => t.Age)
                .Include(t => t.VisitedCountries);

            var greeceAggregate = travelersCollection
                .Aggregate()
                .Match(visitedGreeceExactly3Times)
                .Project(simpleProjection);

            var greeceVisited3Times = await greeceAggregate.ToListAsync();

            #endregion

            #region BsonDocument commands

            var bsonAggregate = personsBsonCollection.Aggregate()
                .Match(Builders<BsonDocument>.Filter.Gte("salary", 3500) &
                       Builders<BsonDocument>.Filter.Lte("salary", 5000));

            var bsonHighSalaryUsers = await bsonAggregate.ToListAsync();

            var bsonSimpleProjection = Builders<BsonDocument>.Projection
                .Include("name")
                .Include("age")
                .Include("visitedCountries");

            var bsonVisitedGreeceExactly3Times = Builders<BsonDocument>.Filter
                .ElemMatch<BsonValue>("visitedCountries", 
                    new BsonDocument { { "name", "Greece" }, { "timesVisited", 3 } });

            var bsonGreeceAggregate = travelersBsonCollection
                .Aggregate()
                .Match(bsonVisitedGreeceExactly3Times)
                .Project(bsonSimpleProjection);

            var bsonGreeceVisited3Times = await bsonGreeceAggregate.ToListAsync();

            #endregion

            #region Shell commands

#if false
            db.users.aggregate([
             { 
                $match : { $and: [ { salary: { $gte: 3500 }}, { salary: { $lte: 5000 }}  ] } 
             }])

            db.trips.aggregate([
               {
                  "$match":{
                     "visitedCountries":{
                        "$elemMatch":{
                           "name":"Greece",
                           "timesVisited":3
                        }
                     }
                  }
               },
               {
                  "$project":{
                     "name":1,
                     "age":1,
                     "visitedCountries":1
                  }
               }
            ])
#endif

            #endregion
        }
    }
}
