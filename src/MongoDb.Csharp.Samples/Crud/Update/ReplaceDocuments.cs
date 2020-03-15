using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Update
{
    public class ReplaceDocuments : RunnableSample, IRunnableSample
    {

        public override Core.Samples Sample => Core.Samples.Crud_Update_ReplaceDocuments;
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

            #region Replace

            // replace the first document entirely
            // requires [BsonIgnoreIfDefault] if id not available
            var newUser = RandomData.GenerateUsers(1).First();
            newUser.FirstName = "Chris";
            newUser.LastName = "Sakellarios";
            newUser.Website = "https://github.com/chsakell";
            var replaceOneResult = await collection.ReplaceOneAsync(Builders<User>.Filter.Empty, newUser);
            Utils.Log($"First user document has been replaced with new user");

            // if id is available then set it on the new doc before replacing the old one
            var firstDbUser = await collection.Find(Builders<User>.Filter.Empty).FirstOrDefaultAsync();
            newUser.Id = firstDbUser.Id;
            newUser.Website = "https://chsakell.com";

            var firstUser = await collection.FindOneAndReplaceAsync(u => u.Id == firstDbUser.Id, newUser);

            #endregion

            #region upsert

            // update a document but if not found then insert it
            var microsoftCeo = RandomData.GenerateUsers(1).First();
            microsoftCeo.FirstName = "Satya";
            microsoftCeo.LastName = "Nadella";
            microsoftCeo.Company.Name = "Microsoft";

            // returns null without upsert true
            var satyaNadellaFirstAttemptResult = await collection
                .FindOneAndReplaceAsync<User>(u => u.Company.Name == "Microsoft", microsoftCeo);

            var satyaNadellaUser = await collection
                .FindOneAndReplaceAsync<User>(u => u.Company.Name == "Microsoft", 
                    microsoftCeo, new FindOneAndReplaceOptions<User>()
                    {
                        IsUpsert = true, 
                        ReturnDocument = ReturnDocument.After
                    });

            #endregion

            #endregion

            #region BsonDocument commands


            #endregion

            #region Shell commands

#if false
            db.users.updateOne({ firstName: "Chris", lastName: "Sakellarios"}, {
                $set: {
                    "gender" : 1,
                    "userName" : "Madeline_Lynch62",
                    "avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/mizhgan/128.jpg",
                    "email" : "chsakell@example.com",
                    "dateOfBirth" : ISODate("1979-09-16T08:59:47.010Z"),
                    "address" : {
                        "street" : "75507 Susana Union",
                        "suite" : "Apt. 764",
                        "city" : "Carolbury",
                        "state" : "Delaware",
                        "zipCode" : "06830-8499",
                        "geo" : {
                            "lat" : 3.6387,
                            "lng" : 5.3223
                        }
                    },
                    "phone" : "1-521-993-8838",
                    "website" : "https://chsakell.com",
                    "company" : {
                        "name" : "Daugherty, Halvorson and Brekke",
                        "catchPhrase" : "Monitored logistical flexibility",
                        "bs" : "synergize out-of-the-box networks"
                    },
                    "salary" : NumberDecimal("4386"),
                    "monthlyExpenses" : NumberDecimal("5658"),
                    "favoriteSports" : [
                        "Volleyball",
                        "Beach Volleyball",
                        "Handball",
                        "Formula 1",
                        "Cycling",
                        "Ice Hockey",
                        "MMA",
                        "Golf"
                    ],
                    "profession" : "Firefighter"
                }}, { upsert: true })

#endif

            #endregion
        }
    }
}
