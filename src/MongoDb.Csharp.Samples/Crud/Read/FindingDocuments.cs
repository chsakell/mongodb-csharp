using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Insert
{
    public class FindingDocuments : RunnableSample, IRunnableSample
    {

        public override Core.Samples Sample => Core.Samples.Crud_Read_FindingDocuments;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Persons);
        }

        public async Task Run()
        {
            await FindDocumentsOperations();
        }

        private async Task FindDocumentsOperations()
        {
            var collectionName = "users";
            var database = Client.GetDatabase(Databases.Persons);
            var collection = database.GetCollection<User>(collectionName);
            var bsonCollection = database.GetCollection<BsonDocument>(collectionName);
            #region Prepare data

            var users = new List<User>();
            for (int i = 0; i < 1000; i++)
            {
                var user = RandomData.GeneratePerson();

                if (i >= 30 && i < 50)
                {
                    user.Address.City = "Athens";
                }

                users.Add(user);
            }

            await collection.InsertManyAsync(users);

            var sampleUser = RandomData.GeneratePerson();
            sampleUser.Email = "sample@example.com";
            sampleUser.Phone = "123-456-789";
            await collection.InsertOneAsync(sampleUser);

            #endregion

            #region Typed classes commands

            // Get the very first document
            var firstUser = await collection.Find(Builders<User>.Filter.Empty).FirstOrDefaultAsync();

            // Get the first document with equality filter on a simple property
            var sampleUserFilter = Builders<User>.Filter.Eq(u => u.Email, sampleUser.Email);
            var dbSampleUser = await collection.Find(sampleUserFilter).FirstOrDefaultAsync();

            // Find multiple documents with equality filter on a simple property
            var doctorsFilter = Builders<User>.Filter.Eq(u => u.Profession, "Doctor");
            var doctors = await collection.Find(doctorsFilter).ToListAsync();
            // same result with .Find
            // doctors = collection.Find(doctorsFilter).ToCursor();
            Utils.Log($"{doctors.Count} doctors found");

            // Query on an embedded field, eg. address.city

            // find all users with address.city = Athens
            var athensCityFilter = Builders<User>.Filter.Eq(u => u.Address.City, "Athens");
            var athensUsers = await collection.Find(athensCityFilter).ToListAsync();
            Utils.Log($"{athensUsers.Count} total users live in Athens");

            // Query on array property

            // find all users that have Basketball on their favorite sports -
            // doesn't have to be the only item on the array, may have more favorite sports as well
            var basketballFilter = Builders<User>.Filter.AnyEq(u => u.FavoriteSports, "Basketball");
            var usersHaveBasketball = await collection.Find(basketballFilter).ToListAsync();
            Utils.Log($"{usersHaveBasketball.Count} have Basketball on their favorite sports collection");

            // find all users that have ONLY Soccer on their favorite sports
            var onlySoccerFilter = Builders<User>.Filter
                .Eq(u => u.FavoriteSports, new List<string> { "Soccer" });

            var soccerUsers = await collection.Find(onlySoccerFilter).ToListAsync();

            Utils.Log($"{soccerUsers.Count} have only Soccer on their favorite sports collection");

            #endregion

            #region BsonDocument commands

            var bsonFirstUser = await bsonCollection.Find(new BsonDocument()).FirstOrDefaultAsync();

            var sampleBsonUserFilter = Builders<BsonDocument>.Filter.Eq("email", sampleUser.Email);
            var dbBsonSampleUser = await bsonCollection.Find(sampleBsonUserFilter).FirstOrDefaultAsync();

            var bsonDoctorsFilter = Builders<BsonDocument>.Filter.Eq("profession", "Doctor");
            var bsonDoctors = await bsonCollection.Find(bsonDoctorsFilter).FirstOrDefaultAsync();

            var bsonAthensCityFilter = Builders<BsonDocument>.Filter.Eq("address.city", "Athens");
            var bsonAthensUsers = await bsonCollection.Find(bsonAthensCityFilter).ToListAsync();

            var bsonBasketballFilter = Builders<BsonDocument>.Filter.AnyEq("favoriteSports", "Basketball");
            var bsonUsersHaveBasketball = await bsonCollection.Find(bsonBasketballFilter).ToListAsync();

            var bsonOnlySoccerFilter = Builders<BsonDocument>.Filter
                .Eq("favoriteSports", new List<string>() { "Soccer" });

            var bsonSoccerUsers = await bsonCollection.Find(bsonOnlySoccerFilter).ToListAsync();
            #endregion

            #region Shell commands

#if false
            db.users.findOne({})
            db.users.findOne({ email: "sample@example.com" })
            db.users.find({ profession: "Doctor"})
            db.users.find({"address.city": { $eq: "Athens"}}
            db.users.find({"favoriteSports": "Basketball"})
            db.users.find({"favoriteSports": ["Basketball"]})
            db.users.find({"favoriteSports": ["Soccer"]})
#endif

            #endregion
        }
    }
}
