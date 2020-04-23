using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Read
{
    public class Basics : RunnableSample, IRunnableSample
    {

        public override Core.Samples Sample => Core.Samples.Crud_Read_Basics;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Constants.SamplesDatabase);
        }

        public async Task Run()
        {
            await FindDocumentsSamples();
        }

        private async Task FindDocumentsSamples()
        {
            var database = Client.GetDatabase(Constants.SamplesDatabase);
            var collection = database.GetCollection<User>(Constants.UsersCollection);
            var bsonCollection = database.GetCollection<BsonDocument>(Constants.UsersCollection);
            #region Prepare data

            var users = RandomData.GenerateUsers(1000);
            for (int i = 0; i < users.Count; i++)
            {
                if (i >= 30 && i < 50)
                {
                    users[i].Address.City = "Athens";
                }
            }

            await collection.InsertManyAsync(users);

            var sampleUser = RandomData.GenerateUsers(1).First();
            sampleUser.Email = "sample@example.com";
            sampleUser.Phone = "123-456-789";
            await collection.InsertOneAsync(sampleUser);

            #endregion

            #region Typed classes commands

            // empty filter
            var emptyFilter = Builders<User>.Filter.Empty;

            // first user
            var firstUser = await collection.Find(emptyFilter).FirstOrDefaultAsync();

            // all users
            var allUsers = await collection.Find(emptyFilter).ToListAsync();

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

            var bsonEmptyDocument = Builders<BsonDocument>.Filter.Empty;

            var bsonFirstUser = await bsonCollection.Find(new BsonDocument()).FirstOrDefaultAsync();
            var bsonAllUsers = await bsonCollection.Find(bsonEmptyDocument).ToListAsync();

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
            db.users.find({"address.city": { $eq: "Athens"}})
            db.users.find({"favoriteSports": "Basketball"})
            db.users.find({"favoriteSports": ["Basketball"]})
            db.users.find({"favoriteSports": ["Soccer"]})
#endif

            #endregion
        }
    }
}
