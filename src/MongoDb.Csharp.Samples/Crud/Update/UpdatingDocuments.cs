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
            await UpdateDocumentsDefinitions();
        }

        private async Task UpdateDocumentsDefinitions()
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
            var updateNameDefinition = Builders<User>.Update.Set(u => u.FirstName, "Chris");
            var updateNameResult = await collection.UpdateOneAsync(firstUserFilter, updateNameDefinition);
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
            var incrementSalaryDefinition = Builders<User>.Update.Inc(u => u.Salary, 450);
            var incrementSalaryResult = await collection.UpdateOneAsync(firstUserFilter, incrementSalaryDefinition);
            Utils.Log($"User's salary has been increased");

            #endregion

            // combine inc with set
            var incWithSetUpdateDefinition = Builders<User>
                .Update.Set(u => u.FirstName, "Chris").Inc(u => u.Salary, 450);
            var incWithSetUpdateResult = await collection.UpdateOneAsync(firstUserFilter, incWithSetUpdateDefinition);

            #region min

            // preparation - set current salary to 3000
            await collection.UpdateOneAsync(firstUserFilter, Builders<User>.Update.Set(u => u.Salary, 3000));
            // update only if the new value is less than the current
            // would not update if the new salary was > 3000
            var minUpdateDefinition = Builders<User>.Update.Min(u => u.Salary, 2000);
            var minUpdateResult = await collection.UpdateOneAsync(firstUserFilter, minUpdateDefinition);
            Utils.Log($"{minUpdateResult.ModifiedCount} user's salary has been updated (decreased - min)");

            #endregion

            #region max

            // preparation - set current salary to 3000
            await collection.UpdateOneAsync(firstUserFilter, Builders<User>.Update.Set(u => u.Salary, 3000));
            // update only if the new value is greater than the current
            // would not update if the new salary was < 3000
            var maxUpdateDefinition = Builders<User>.Update.Max(u => u.Salary, 3500);
            var maxUpdateResult = await collection.UpdateOneAsync(firstUserFilter, maxUpdateDefinition);
            Utils.Log($"{maxUpdateResult.ModifiedCount} user's salary has been updated (increased - max)");

            #endregion

            #region mul

            // preparation - set current salary to 1000
            await collection.UpdateOneAsync(firstUserFilter, Builders<User>.Update.Set(u => u.Salary, 1000));
            // set salary X 2
            var mulUpdateDefinition = Builders<User>.Update.Mul(u => u.Salary, 2);
            var mulUpdateResult = await collection.UpdateOneAsync(firstUserFilter, mulUpdateDefinition);
            Utils.Log($"{mulUpdateResult.ModifiedCount} user's salary has been doubled (mul)");

            #endregion

            #endregion

            #region BsonDocument commands

            #region set

            var bsonFirstUserFilter = Builders<BsonDocument>.Filter.Empty;
            
            // single field
            var bsonUpdateNameDefinition = Builders<BsonDocument>.Update.Set("firstName", "John");
            var bsonUpdateNameResult = await bsonCollection.UpdateOneAsync(bsonFirstUserFilter, bsonUpdateNameDefinition);

            // update multiple fields
            var bsonMultiUpdateDefinition = Builders<BsonDocument>.Update
                .Set("phone", "123-456-789")
                .Set("website", "https://chsakell.com")
                .Set("favoriteSports", new List<string> { "Soccer", "Basketball" });

            var bsonMultiUpdateResult = await bsonCollection.UpdateOneAsync(bsonFirstUserFilter, bsonMultiUpdateDefinition);
            #endregion

            #region inc

            var bsonIncrementSalaryDefinition = Builders<BsonDocument>.Update.Inc("salary", 450);
            var bsonIncrementSalaryResult = await bsonCollection
                .UpdateOneAsync(bsonFirstUserFilter, bsonIncrementSalaryDefinition);

            #endregion

            // combine inc with set
            var bsonIncWithSetUpdateDefinition = Builders<BsonDocument>
                .Update.Set("firstName", "Chris").Inc("salary", 450);
            var bsonIncWithSetUpdateResult = await bsonCollection
                .UpdateOneAsync(bsonFirstUserFilter, bsonIncWithSetUpdateDefinition);

            #region min
            // preparation - set current salary to 3000
            await bsonCollection.UpdateOneAsync(bsonFirstUserFilter, 
                Builders<BsonDocument>.Update.Set("salary", 3000));
            // update only if the new value is less than the current
            // would not update if the new salary was > 3000
            var bsonMinUpdateDefinition = Builders<BsonDocument>.Update.Min("salary", 2000);
            var bsonMinUpdateResult = await bsonCollection
                .UpdateOneAsync(bsonFirstUserFilter, bsonMinUpdateDefinition);

            #endregion

            #region max

            // preparation - set current salary to 3000
            await bsonCollection.UpdateOneAsync(bsonFirstUserFilter, Builders<BsonDocument>
                .Update.Set("salary", 3000));
            // update only if the new value is greater than the current
            // would not update if the new salary was < 3000
            var bsonMaxUpdateDefinition = Builders<BsonDocument>.Update.Max("salary", 3500);
            var bsonMaxUpdateResult = await bsonCollection
                .UpdateOneAsync(bsonFirstUserFilter, bsonMaxUpdateDefinition);

            #endregion

            #region mul

            // preparation - set current salary to 1000
            await bsonCollection.UpdateOneAsync(bsonFirstUserFilter, 
                Builders<BsonDocument>.Update.Set("salary", 1000));
            // set salary X 2
            var bsonMulUpdateDefinition = Builders<BsonDocument>.Update.Mul("salary", 2);
            var bsonMulUpdateResult = await bsonCollection.UpdateOneAsync(bsonFirstUserFilter, bsonMulUpdateDefinition);

            #endregion

            #endregion

            #region Shell commands

#if false
            db.users.update({}, { $set: { firstName: "Chris"  }});
            db.users.update({}, { $set: { 
                phone: "123-456-789", 
                website: "https://chsakell.com", 
                favoriteSports: ["Soccer", "Basketball"]  
            }});
            db.users.update({}, { $set: { firstName: "Chris"  }, $inc: { salary: 450 }});
            db.users.update({}, { $min: { salary: 2000 } })
            db.users.update({}, { $max: { salary: 3500 } })
            db.users.update({}, { $mul: { salary: 2 } })
#endif

            #endregion
        }
    }
}
