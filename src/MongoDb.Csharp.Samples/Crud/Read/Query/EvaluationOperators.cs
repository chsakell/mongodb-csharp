using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Read.Query
{
    public class EvaluationOperators : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Crud_Read_Query_EvaluationOperators;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Constants.SamplesDatabase);
        }

        public async Task Run()
        {
            await ElementOperatorsSamples();
        }

        private async Task ElementOperatorsSamples()
        {
            var collectionName = "users";
            var database = Client.GetDatabase(Constants.SamplesDatabase);
            var collection = database.GetCollection<User>(collectionName);
            var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

            #region Prepare data

            var users = RandomData.GenerateUsers(1000);

            await collection.InsertManyAsync(users);

            #endregion

            #region Typed classes commands

            #region regex

            var gmailFilter = Builders<User>.Filter.Regex(u => u.Email, new BsonRegularExpression("/gmail/"));
            var gmailUsers = await collection.Find(gmailFilter).ToListAsync();
            Utils.Log($"{gmailUsers.Count} users found to have gmail acounts");

            #endregion

            #region expression

            // needs projection

            #endregion

            #endregion

            #region BsonDocument commands

            #region regex

            var bsonGmailFilter = Builders<BsonDocument>.Filter
                .Regex("email", new BsonRegularExpression("/gmail/"));

            var bsonGmailUsers = await bsonCollection.Find(bsonGmailFilter).ToListAsync();

            #endregion

            #endregion

            #region Shell commands

#if false
        db.users.find({"email": { $regex : /gmail/ }})
#endif


            #endregion
        }
    }
}
