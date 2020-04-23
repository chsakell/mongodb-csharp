using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Update
{
    public class BasicOperators : RunnableSample, IRunnableSample
    {

        public override Core.Samples Sample => Core.Samples.Crud_Update_BasicOperators;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Constants.SamplesDatabase);
        }

        public async Task Run()
        {
            await UpdateDocumentsDefinitions();
        }

        private async Task UpdateDocumentsDefinitions()
        {
            var database = Client.GetDatabase(Constants.SamplesDatabase);
            var collection = database.GetCollection<User>(Constants.UsersCollection);
            var bsonCollection = database.GetCollection<BsonDocument>(Constants.UsersCollection);
            #region Prepare data

            await collection.InsertManyAsync(RandomData.GenerateUsers(1000));

            #endregion

            #region Typed classes commands

            #region set

            var firstUserFilter = Builders<User>.Filter.Empty;
            var allusers = await collection.Find(firstUserFilter).ToListAsync();
            // update a single field
            var updateNameDefinition = Builders<User>.Update.Set(u => u.FirstName, "Chris");
            var updateNameResult = await collection.UpdateOneAsync(firstUserFilter, updateNameDefinition);
            Utils.Log($"{updateNameResult.ModifiedCount} user's name was updated");

            // update multiple fields
            var multiUpdateDefinition = Builders<User>.Update
                .Set(u => u.Phone, "123-456-789")
                .Set(u => u.Website, "https://chsakell.com")
                .Set(u => u.FavoriteSports, new List<string> { "Soccer", "Basketball" });

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
                .Update.Set(u => u.FirstName, "Chris")
                .Inc(u => u.Salary, 450);
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

            #region unset

            // remove the website field for the first user
            var removeWebsiteDefinition = Builders<User>.Update.Unset(u => u.Website);
            var removeWebsiteFieldUpdateResult =
                await collection.UpdateOneAsync(firstUserFilter, removeWebsiteDefinition);
            Utils.Log("Website field entirely removed from the first document");


            #region caution

            //var removeSalaryDefinition = Builders<User>.Update.Unset(u => u.Salary);
            //var removeSalaryFieldUpdateResult =
            //    await collection.UpdateOneAsync(firstUserFilter, removeSalaryDefinition);

            //var firstUser = await collection.Find(firstUserFilter).FirstOrDefaultAsync();

            #endregion
            #endregion

            #region rename

            // rename the phone field to phoneNumber for the first user
            var renamePhoneDefinition = Builders<User>.Update.Rename(u => u.Phone, "phoneNumber");
            var renamePhoneFieldUpdateResult =
                await collection.UpdateOneAsync(firstUserFilter, renamePhoneDefinition);
            Utils.Log("Phone field renamed to phone");

            // Switch back to phone
            await collection.UpdateOneAsync(firstUserFilter, Builders<User>.Update.Rename("phoneNumber", "phone"));


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
            // would not update if the new salary was < 3500
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

            #region unset

            // remove the website field for the first user
            var bsonRemoveWebsiteDefinition = Builders<BsonDocument>.Update.Unset("website");
            var bsonRemoveWebsiteFieldUpdateResult =
                await bsonCollection.UpdateOneAsync(bsonFirstUserFilter, bsonRemoveWebsiteDefinition);

            #endregion

            #region rename

            // rename the phone field to phoneNumber for the first user
            var bsonRenamePhoneDefinition = Builders<BsonDocument>.Update.Rename("phone", "phoneNumber");
            var bsonRenamePhoneFieldUpdateResult =
                await bsonCollection.UpdateOneAsync(bsonFirstUserFilter, bsonRenamePhoneDefinition);

            // Switch back to phone
            await bsonCollection.UpdateOneAsync(bsonFirstUserFilter, 
                Builders<BsonDocument>.Update.Rename("phoneNumber", "phone"));


            #endregion

            #endregion

            #region Shell commands

#if false
            db.users.updateOne({}, { $set: { firstName: "Chris"  }});
            db.users.updateOne({}, { $set: { 
                phone: "123-456-789", 
                website: "https://chsakell.com", 
                favoriteSports: ["Soccer", "Basketball"]  
            }});
            db.users.updateOne({}, { $set: { firstName: "Chris"  }, $inc: { salary: NumberDecimal("450") }});
            db.users.updateOne({}, { $min: { salary: NumberDecimal("2000") } })
            db.users.updateOne({}, { $inc: { salary: NumberDecimal("450.00") }});
            db.users.updateOne({}, { $mul: { salary: 2 } })
            db.users.updateOne({}, { $unset: { website: ""  }})
            db.users.updateOne({}, { $rename: { phone: "phoneNumber" } })
#endif

            #endregion
        }
    }
}
