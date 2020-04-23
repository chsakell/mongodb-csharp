using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using Utils = MongoDb.Csharp.Samples.Core.Utils;

namespace MongoDb.Csharp.Samples.QuickStart
{
    public class ReadDocuments : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.QuickStart_ReadDocuments;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Constants.SamplesDatabase);
        }

        public async Task Run()
        {
            await ReadSamples();
        }
        private async Task ReadSamples()
        {
            var database = Client.GetDatabase(Constants.SamplesDatabase);

            #region Prepare data

            // Will create the users collection on the fly if it doesn't exists
            var personsCollection = database.GetCollection<User>(Constants.UsersCollection);

            User appPerson = RandomData.GenerateUsers(1).First();
            // Insert one document
            await personsCollection.InsertOneAsync(appPerson);

            // Insert multiple documents
            var persons = RandomData.GenerateUsers(30);

            await personsCollection.InsertManyAsync(persons);
            #endregion

            #region Typed classes commands

            // Find a person using a class filter
            var personFilter = Builders<User>.Filter.Eq(person => person.Id, appPerson.Id);
            var personFindResult = await personsCollection.Find(personFilter).FirstOrDefaultAsync();
            Utils.Log(personFindResult.ToBsonDocument(), "Document Find with filter");

            // Find multiple documents using a filter

            var femaleGenderFilter = Builders<User>.Filter.Eq(person => person.Gender, Gender.Female);
            var females = await personsCollection.Find(femaleGenderFilter).ToListAsync();
            Utils.Log($"Found {females.Count} female persons");

            #endregion

            #region BsonDocument commands
            // we need to get the BsonDocument schema based collection
            var bsonPersonCollection = database.GetCollection<BsonDocument>(Constants.UsersCollection);

            // Create a bson filter
            var bsonPersonFilter = Builders<BsonDocument>.Filter.Eq("_id", appPerson.Id);

            // Find a person using a class filter
            var bsonPersonFindResult = await bsonPersonCollection.Find(bsonPersonFilter).FirstOrDefaultAsync();
            bsonPersonFindResult = await bsonPersonCollection.Find(new BsonDocument("_id", appPerson.Id)).FirstOrDefaultAsync();
            Utils.Log(bsonPersonFindResult);

            var bsonFemaleGenderFilter = Builders<BsonDocument>.Filter.Eq("gender", Gender.Female);
            var bsonFemales = await bsonPersonCollection.Find(bsonFemaleGenderFilter).ToListAsync();

            #endregion

            #region Shell commands

            /*
            use Persons

            // find a single document
            db.users.findOne(
            {
                "_id": ObjectId("5e5d11fe152a428290f30245")
            })

            // find multiple documents
            db.users.find(
            {
                "gender": 1
            })
             */

            #endregion
        }
    }
}
