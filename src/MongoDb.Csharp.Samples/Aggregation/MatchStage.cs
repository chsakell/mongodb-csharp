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
        }

        public async Task Run()
        {
            await MatchStageOperations();
        }

        private async Task MatchStageOperations()
        {
            var collectionName = "users";
            var database = Client.GetDatabase(Databases.Persons);
            var collection = database.GetCollection<User>(collectionName);
            var bsonCollection = database.GetCollection<BsonDocument>(collectionName);
            #region Prepare data

            await collection.InsertManyAsync(RandomData.GenerateUsers(1000));

            #endregion

            #region Typed classes commands

            var aggregate = collection.Aggregate()
                .Match(Builders<User>.Filter.Gte(u => u.Salary, 3500) &
                       Builders<User>.Filter.Lte(u => u.Salary, 5000));

            var highSalaryUsers = await aggregate.ToListAsync();
            Utils.Log($"{highSalaryUsers.Count} users found with high salary");

            #endregion

            #region BsonDocument commands

            var bsonAggregate = bsonCollection.Aggregate()
                .Match(Builders<BsonDocument>.Filter.Gte("salary", 3500) &
                       Builders<BsonDocument>.Filter.Lte("salary", 5000));

            var bsonHighSalaryUsers = await bsonAggregate.ToListAsync();

            #endregion

            #region Shell commands

#if false
            db.users.aggregate([
             { 
                $match : { $and: [ { salary: { $gte: 3500 }}, { salary: { $lte: 5000 }}  ] } 
             }])

#endif

            #endregion
        }
    }
}
