using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Read.Query
{
    public class ComparisonOperators : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Crud_Read_Query_ComparisonOperators;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Persons);
        }

        public async Task Run()
        {
            await ComparisonOperatorsOperations();
        }

        private async Task ComparisonOperatorsOperations()
        {
            var collectionName = "users";
            var database = Client.GetDatabase(Databases.Persons);
            var collection = database.GetCollection<User>(collectionName);
            var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

            #region Prepare data

            var users = RandomData.GenerateUsers(1000);

            await collection.InsertManyAsync(users);

            #endregion

            #region Typed classes commands

            // Greater than
            var filterGreaterThan = Builders<User>.Filter.Gt(u => u.Salary, 3500);
            var greaterThan3500 = await collection.Find(filterGreaterThan).ToListAsync();
            
            // Greater or equal than
            var filterGreaterOrEqualThan = Builders<User>.Filter.Gte(u => u.Salary, 4500);
            var greaterOrEqualThan4500 = await collection.Find(filterGreaterOrEqualThan).ToListAsync();

            // Less than
            var filterLessThan = Builders<User>.Filter.Lt(u => u.Salary, 2500);
            var lessThan2500 = await collection.Find(filterLessThan).ToListAsync();

            // Greater or equal than
            var filterLessOrEqualThan = Builders<User>.Filter.Lte(u => u.Salary, 1500);
            var lessThanOrEqual1500 = await collection.Find(filterLessOrEqualThan).ToListAsync();

            Utils.Log($"Salary Greater than 3500 total: {greaterThan3500.Count}{Environment.NewLine}" +
                      $"Salary Greater or equal to 4500 total: {greaterOrEqualThan4500.Count}{Environment.NewLine}" +
                      $"Salary Less than 2500 total: {lessThan2500.Count}{Environment.NewLine}" +
                      $"Salary Less than or equal to 1500 total: {lessThanOrEqual1500.Count}{Environment.NewLine}");

            var totalUsers = await collection.CountDocumentsAsync(Builders<User>.Filter.Empty);

            // Equal
            // Case sensitive matters!
            var equalPilotsFilter = Builders<User>.Filter.Eq(u => u.Profession, "Pilot");
            var pilots = await collection.Find(equalPilotsFilter).ToListAsync();
            Utils.Log($"Among {totalUsers} users, {pilots.Count} are pilots");

            // Not equal
            // Case sensitive matters!
            var notEqualDoctorsFilter = Builders<User>.Filter.Ne(u => u.Profession, "Doctor");
            var notDoctors = await collection.Find(notEqualDoctorsFilter).ToListAsync();
            Utils.Log($"Among {totalUsers} users, {notDoctors.Count} aren't doctors");

            // In
            var medicalProfessionsFilter = Builders<User>.Filter.In(u => u.Profession,
                new[] { "Dentist", "Pharmacist", "Nurse" });
            var medicalUsers = await collection.Find(medicalProfessionsFilter).ToListAsync();
            Utils.Log($"{medicalUsers.Count} users have medical related profession");

            // Not In
            var nonMedicalProfessionsFilter = Builders<User>.Filter.Nin(u => u.Profession,
                new[] { "Dentist", "Pharmacist", "Nurse" });
            var nonMedicalUsers = await collection.Find(nonMedicalProfessionsFilter).ToListAsync();
            Utils.Log($"{nonMedicalUsers.Count} users have no medical related profession");
            #endregion

            #region BsonDocument commands

            // Greater than
            var bsonFilterGreaterThan = Builders<BsonDocument>.Filter.Gt("salary", 3500);
            var bsonGreaterThan3500 = await bsonCollection.Find(bsonFilterGreaterThan).ToListAsync();

            // Greater or equal than
            var bsonFilterGreaterOrEqualThan = Builders<BsonDocument>.Filter.Gte("salary", 4500);
            var bsonGreaterOrEqualThan4500 = await bsonCollection.Find(bsonFilterGreaterOrEqualThan).ToListAsync();

            // Less than
            var bsonFilterLessThan = Builders<BsonDocument>.Filter.Lt("salary", 2500);
            var bsonLessThan2500 = await bsonCollection.Find(bsonFilterLessThan).ToListAsync();

            // Greater or equal than
            var bsonFilterLessOrEqualThan = Builders<BsonDocument>.Filter.Lte("salary", 1500);
            var bsonLessThanOrEqual1500 = await bsonCollection.Find(bsonFilterLessOrEqualThan).ToListAsync();

            // Equal
            // Case sensitive matters!
            var bsonEqualPilotsFilter = Builders<BsonDocument>.Filter.Eq("profession", "Pilot");
            var bsonPilots = await bsonCollection.Find(bsonEqualPilotsFilter).ToListAsync();

            // Not equal
            var bsonNotEqualDoctorsFilter = Builders<BsonDocument>.Filter.Ne("profession", "Doctor");
            var bsonNotDoctors = await bsonCollection.Find(bsonNotEqualDoctorsFilter).ToListAsync();

            // In
            var bsonMedicalProfessionsFilter = Builders<BsonDocument>.Filter.In("profession",
                new[] { "Dentist", "Pharmacist", "Nurse" });
            var bsonMedicalUsers = await bsonCollection.Find(bsonMedicalProfessionsFilter).ToListAsync();

            // Not In
            var bsonNotMedicalProfessionsFilter = Builders<BsonDocument>.Filter.Nin("profession",
                new[] { "Dentist", "Pharmacist", "Nurse" });
            var bsonNotMedicalUsers = await bsonCollection.Find(bsonNotMedicalProfessionsFilter).ToListAsync();
            #endregion

            #region Shell commands

#if false
            db.users.find({salary: { $gt: 3500}})
            db.users.find({salary: { $gte: 4500}})
            db.users.find({salary: { $lt: 2500}})
            db.users.find({salary: { $lte: 1500}})
            db.users.find({profession: { $eq: "Pilot"}})
            db.users.find({profession: { $ne: "Doctor"}})
            db.users.find({profession: { $in: ["Dentist", "Pharmacist", "Nurse"]}})
            db.users.find({profession: { $nin: ["Dentist", "Pharmacist", "Nurse"]}})
#endif

            #endregion
        }
    }
}
