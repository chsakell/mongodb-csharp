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
            
            // newUser.Id = ObjectId.GenerateNewId(); this would cause an exception
            newUser.FirstName = "Chris";
            newUser.LastName = "Sakellarios";
            newUser.Website = "https://github.com/chsakell";
            var replaceOneResult = await collection.ReplaceOneAsync(Builders<User>.Filter.Empty, newUser);
            Utils.Log($"First user document has been replaced with new user");
            
            // if id is available then set it on the new doc before replacing the old one
            var firstDbUser = await collection.Find(Builders<User>.Filter.Empty).FirstOrDefaultAsync();
            newUser.Id = firstDbUser.Id;
            newUser.Website = "https://chsakell.com";

            var firstUser = await collection.FindOneAndReplaceAsync(
                Builders<User>.Filter.Eq(u => u.Id, firstDbUser.Id), newUser,
                new FindOneAndReplaceOptions<User> {ReturnDocument = ReturnDocument.After});

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

            var addOrReplaceSatyaNadellaUser = await collection
                .FindOneAndReplaceAsync<User>(u => u.Company.Name == "Microsoft", 
                    microsoftCeo, new FindOneAndReplaceOptions<User>()
                    {
                        IsUpsert = true, 
                        ReturnDocument = ReturnDocument.After
                    });

            var addOrReplaceSatyaNadellaResult = await collection
                .ReplaceOneAsync<User>(u => u.Company.Name == "Microsoft Corp",
                    microsoftCeo, new ReplaceOptions()
                    {
                        IsUpsert = true
                    });

            #endregion

            #endregion

            #region BsonDocument commands

            newUser.FirstName = "Christos";
            newUser.LastName = "Sakellarios";
            newUser.Website = "https://github.com/chsakell";
            var bsonReplaceOneResult = await bsonCollection
                .ReplaceOneAsync(new BsonDocument(), newUser.ToBsonDocument());

            var bsonAddOrReplaceSatyaNadellaUser = await bsonCollection
                .FindOneAndReplaceAsync(
                    Builders<BsonDocument>.Filter.Eq("company.name", "Microsoft Corp"), 
                        microsoftCeo.ToBsonDocument(), 
                    new FindOneAndReplaceOptions<BsonDocument>()
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                });

            var bsonFirstUser = await bsonCollection.FindOneAndReplaceAsync(
                    Builders<BsonDocument>.Filter.Eq("_id", firstDbUser.Id), 
                    newUser.ToBsonDocument(),
                    new FindOneAndReplaceOptions<BsonDocument> 
                        { ReturnDocument = ReturnDocument.After });

            #endregion

            #region Shell commands

#if false
            db.users.replaceOne({},
            {
	            "gender" : 1,
	            "firstName" : "Chris",
	            "lastName" : "Sakellarios",
	            "userName" : "Elsie.VonRueden72",
	            "avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/miguelmendes/128.jpg",
	            "email" : "Elsie.VonRueden@yahoo.com",
	            "dateOfBirth" : ISODate("1965-08-26T15:55:31.907+02:00"),
	            "address" : {
		            "street" : "8902 Baumbach Burg",
		            "suite" : "Apt. 717",
		            "city" : "West Carlieton",
		            "state" : "South Carolina",
		            "zipCode" : "85642-3703",
		            "geo" : {
			            "lat" : -69.6681,
			            "lng" : -116.3583
		            }
	            },
	            "phone" : "(222) 443-5341 x35825",
	            "website" : "https://github.com/chsakell",
	            "company" : {
		            "name" : "Abshire Inc",
		            "catchPhrase" : "Function-based mission-critical budgetary management",
		            "bs" : "monetize holistic eyeballs"
	            },
	            "salary" : 2482,
	            "monthlyExpenses" : 2959,
	            "favoriteSports" : [
		            "Basketball",
		            "Baseball",
		            "Table Tennis",
		            "Ice Hockey",
		            "Handball",
		            "Formula 1",
		            "American Football"
	            ],
	            "profession" : "Model"
            })

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
