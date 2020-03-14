using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Update
{
    public class UpdatingDocuments : RunnableSample, IRunnableSample
    {

        public override Core.Samples Sample => Core.Samples.Crud_Update_UpdatingDocuments;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Persons);
        }

        public async Task Run()
        {
            await UpdateDocumentsOperations();
        }

        private async Task UpdateDocumentsOperations()
        {
            var collectionName = "users";
            var database = Client.GetDatabase(Databases.Persons);
            var collection = database.GetCollection<User>(collectionName);
            var bsonCollection = database.GetCollection<BsonDocument>(collectionName);
            #region Prepare data

            await collection.InsertManyAsync(RandomData.GenerateUsers(1000));

            #endregion

            #region Typed classes commands

            #region set

            var firstUserFilter = Builders<User>.Filter.Empty;

            // update a single field
            var updateNameOperation = Builders<User>.Update.Set(u => u.FirstName, "Chris");
            var updateNameResult = await collection.UpdateOneAsync(firstUserFilter, updateNameOperation);
            Utils.Log($"{updateNameResult.ModifiedCount} user's name was updated");

            // update multiple fields
            var multiUpdateDefinition = Builders<User>.Update
                .Set(u => u.Phone, "123-456-789")
                .Set(u => u.Website, "https://chsakell.com")
                .Set(u => u.FavoriteSports, new List<string> {"Soccer", "Basketball"});

            var multiUpdateResult = await collection.UpdateOneAsync(firstUserFilter, multiUpdateDefinition);
            Utils.Log($"Multiple user's fields were updated");

            #endregion

            #region inc

            // can be used with negative values to decrease
            var incrementSalaryOperation = Builders<User>.Update.Inc(u => u.Salary, 450);
            var incrementSalaryResult = await collection.UpdateOneAsync(firstUserFilter, incrementSalaryOperation);
            Utils.Log($"User's salary has been increased");

            #endregion

            // combine inc with set
            var incWithSetUpdateDefinition = Builders<User>
                .Update.Set(u => u.FirstName, "Chris").Inc(u => u.Salary, 450);
            var incWithSetUpdateResult = await collection.UpdateOneAsync(firstUserFilter, incWithSetUpdateDefinition);

            #endregion

            #region BsonDocument commands

            #region set

            var bsonFirstUserFilter = Builders<BsonDocument>.Filter.Empty;
            
            // single field
            var bsonUpdateNameOperation = Builders<BsonDocument>.Update.Set("firstName", "John");
            var bsonUpdateNameResult = await bsonCollection.UpdateOneAsync(bsonFirstUserFilter, bsonUpdateNameOperation);

            // update multiple fields
            var bsonMultiUpdateDefinition = Builders<BsonDocument>.Update
                .Set("phone", "123-456-789")
                .Set("website", "https://chsakell.com")
                .Set("favoriteSports", new List<string> { "Soccer", "Basketball" });

            var bsonMultiUpdateResult = await bsonCollection.UpdateOneAsync(bsonFirstUserFilter, bsonMultiUpdateDefinition);
            #endregion

            #region inc

            var bsonIncrementSalaryOperation = Builders<BsonDocument>.Update.Inc("salary", 450);
            var bsonIncrementSalaryResult = await bsonCollection
                .UpdateOneAsync(bsonFirstUserFilter, bsonIncrementSalaryOperation);

            #endregion

            // combine inc with set
            var bsonIncWithSetUpdateDefinition = Builders<BsonDocument>
                .Update.Set("firstName", "Chris").Inc("salary", 450);
            var bsonIncWithSetUpdateResult = await bsonCollection
                .UpdateOneAsync(bsonFirstUserFilter, bsonIncWithSetUpdateDefinition);

            #endregion

            #region Shell commands

#if false
            db.users.update({}, { $set: { firstName: "Chris"  }});
            db.users.update({}, { $set: { 
                phone: "123-456-789", 
                website: "https://chsakell.com", 
                favoriteSports: ["Soccer", "Basketball"]  
            }});
#endif

            #endregion
        }
    }
}
