using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Insert
{
    public class FindingDocuments : RunnableSample, IRunnableSample
    {
        public bool Enabled => true;
        protected override Core.Samples Sample => Core.Samples.Crud_Finding_Documents;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Persons);
        }

        public async Task Run()
        {
            await WriteConcernOperations();
        }

        private async Task WriteConcernOperations()
        {
            var collectionName = "users";
            var database = Client.GetDatabase(Databases.Persons);
            var collection = database.GetCollection<User>(collectionName);
            var bsonCollection = database.GetCollection<BsonDocument>(collectionName);
            #region Prepare data

            for (int i = 0; i < 1000; i++)
            {
                await collection.InsertOneAsync(RandomData.GeneratePerson());
            }

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
            #endregion

            #region BsonDocument commands

            var bsonFirstUser = await bsonCollection.Find(new BsonDocument()).FirstOrDefaultAsync();

            var sampleBsonUserFilter = Builders<BsonDocument>.Filter.Eq("email", sampleUser.Email);
            var dbBsonSampleUser = await bsonCollection.Find(sampleBsonUserFilter).FirstOrDefaultAsync();

            var bsonDoctorsFilter = Builders<BsonDocument>.Filter.Eq("profession", "Doctor");
            var bsonDoctors = await bsonCollection.Find(bsonDoctorsFilter).FirstOrDefaultAsync();

            #endregion

            #region Shell commands

#if false
            db.users.findOne({})
            db.users.findOne({ email: "sample@example.com" })
            db.users.find({ profession: "Doctor"})
#endif

            #endregion
        }
    }
}
