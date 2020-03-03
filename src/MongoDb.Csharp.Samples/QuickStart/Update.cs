using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using Utils = MongoDb.Csharp.Samples.Core.Utils;

namespace MongoDb.Csharp.Samples.QuickStart
{
    public class Update : RunnableSample, IRunnableSample
    {
        public bool Enabled => true;
        protected override Core.Samples Sample => Core.Samples.Basic_Update;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Core.Databases.Persons);
        }

        public async Task Run()
        {
            await UpdateOperations();
        }
        private async Task UpdateOperations()
        {
            var usersDatabase = Client.GetDatabase(Core.Databases.Persons);

            #region Prepare data

            // Will create the users collection on the fly if it doesn't exists
            var personsCollection = usersDatabase.GetCollection<AppPerson>("users");

            AppPerson appPerson = RandomData.GeneratePerson();
            // Insert one document
            await personsCollection.InsertOneAsync(appPerson);


            // Insert multiple documents
            var persons = new List<AppPerson>();
            for (int i = 0; i < 30; i++)
            {
                persons.Add(RandomData.GeneratePerson());
            }

            await personsCollection.InsertManyAsync(persons);
            #endregion

            #region Typed classes commands

            // Find a person using a class filter
            var filter = Builders<AppPerson>.Filter.Eq(person => person.Id, appPerson.Id);

            // update person
            var update = Builders<AppPerson>.Update.Set(person => person.Phone, "12345");
            var personUpdateResult = await personsCollection.UpdateOneAsync(filter, update);
            if (personUpdateResult.MatchedCount == 1)
            {
                Utils.Log( $"Document {appPerson.Id} Updated");
            }

            // Find multiple documents having 1200 < salary < 3500 

            var salaryFilter = Builders<AppPerson>.Filter
                .And(
                    Builders<AppPerson>.Filter.Gt(person => person.Salary, 1200),
                    Builders<AppPerson>.Filter.Lt(person => person.Salary, 3500)
                    );

            var totalPersons = await personsCollection.Find(salaryFilter).CountDocumentsAsync();

            var updateDefinition =
                Builders<AppPerson>.Update.Set(person => person.Salary, 4000);
            var updateResult = await personsCollection.UpdateManyAsync(salaryFilter, updateDefinition);
            if (updateResult.MatchedCount.Equals(totalPersons))
            {
                Utils.Log($"Salary has been updated for {totalPersons}");
            }

            #endregion

            #region BsonDocument commands
            // we need to get the BsonDocument schema based collection
            var bsonPersonCollection = usersDatabase.GetCollection<BsonDocument>("users");
            // Find a person using a class filter
            var bsonSingleFilter = Builders<BsonDocument>.Filter.Eq("_id", appPerson.Id);
            var bsonUpdate = Builders<BsonDocument>.Update.Set("phone", "123-456-678");
            var bsonPersonUpdateResult = await bsonPersonCollection.UpdateOneAsync(bsonSingleFilter, bsonUpdate);
            if (bsonPersonUpdateResult.MatchedCount == 1)
            {
                Utils.Log("Person updated");
            }

            #endregion

            #region Shell commands

            /*
            use Persons

            // update a single document
            db.users.updateOne({}, { $set: {  phone: "123-456-678" } })

            // update multiple documents
            db.users.updateMany(
                { $and: [{ salary: { $gt: 1200} }, {salary: { $lt: 3500} }] },
                { $set: { salary: 4000  } }
            )

             */

            #endregion
        }
    }
}
