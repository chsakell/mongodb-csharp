using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Read.Query
{
    public class LogicalOperators : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Crud_Read_Query_LogicalOperators;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Persons);
        }

        public async Task Run()
        {
            await LogicalOperatorsOperations();
        }

        private async Task LogicalOperatorsOperations()
        {
            var collectionName = "users";
            var database = Client.GetDatabase(Databases.Persons);
            var collection = database.GetCollection<User>(collectionName);
            var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

            #region Prepare data

            var users = new List<User>();
            for (int i = 0; i < 1000; i++)
            {
                users.Add(RandomData.GeneratePerson());
            }

            await collection.InsertManyAsync(users);

            #endregion

            #region Typed classes commands

            #region and

            // and on simple properties - all male doctors
            var maleFilter = Builders<User>.Filter.Eq(u => u.Gender, Gender.Male);
            var doctorFilter = Builders<User>.Filter.Eq(u => u.Profession, "Doctor");
            var maleDoctorsFilter = Builders<User>.Filter.And(maleFilter, doctorFilter);

            var maleDoctors = await collection.Find(maleDoctorsFilter).ToListAsync();
            Utils.Log($"{maleDoctors.Count} total users are male Doctors");

            //////////////////////////////////////////////////////////////////////////////////////

            // and combined with other operators
            // - all a) female witch are b) either teacher or nurse AND c) having salary between 2000-3200  
            var femaleFilter = Builders<User>.Filter.Eq(u => u.Gender, Gender.Female);
            var teacherOrNurseFilter = Builders<User>.Filter
                .In(u => u.Profession, new[] { "Teacher", "Nurse", "Dentist" });
            var salaryFilter = Builders<User>.Filter.And(
                Builders<User>.Filter.Gte(u => u.Salary, 2000),
                Builders<User>.Filter.Lte(u => u.Salary, 3200));
            var combinedFilter = Builders<User>.Filter.And(femaleFilter, teacherOrNurseFilter, salaryFilter);
            var matchedUsers = await collection.Find(combinedFilter).ToListAsync();
            Utils.Log($"{matchedUsers.Count} total users are female Teacher or Nurse or Dentist, with 2000 < salary < 3200");


            #endregion

            #endregion

            #region BsonDocument commands

            #region and

            var bsonMaleFilter = Builders<BsonDocument>.Filter.Eq("gender", Gender.Male);
            var bsonDoctorFilter = Builders<BsonDocument>.Filter.Eq("profession", "Doctor");
            var bsonMaleDoctorsFilter = Builders<BsonDocument>.Filter.And(bsonMaleFilter, bsonDoctorFilter);
            var bsonMaleDoctors = await bsonCollection.Find(bsonMaleDoctorsFilter).ToListAsync();

            //////////////////////////////////////////////////////////////////////////////////////

            var bsonFemaleFilter = Builders<BsonDocument>.Filter.Eq("gender", Gender.Female);
            var bsonTeacherOrNurseFilter = Builders<BsonDocument>.Filter
                .In("profession", new[] { "Teacher", "Nurse", "Dentist" });
            var bsonSalaryFilter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Gte("salary", 2000),
                Builders<BsonDocument>.Filter.Lte("salary", 3200));
            var bsonCombinedFilter = Builders<BsonDocument>.Filter
                .And(bsonFemaleFilter, bsonTeacherOrNurseFilter, bsonSalaryFilter);
            var bsonMatchedUsers = await bsonCollection.Find(bsonCombinedFilter).ToListAsync();

            #endregion

            #endregion

            #region Shell commands

#if false
            db.users.find({ $and: [{ profession: { $eq: "Doctor"} }, {gender: { $eq: 0} }] })

            db.users.find( { $and: [
                                    { "gender" : 1 },
                                    { profession: { $in: ["Teacher", "Nurse", "Dentist"]}},
                                    { $and : [ 
                                                { salary: { $gte: 2000 } }, 
                                                { salary: { $lte: 3200 }} ] } 
                                    ] })

#endif

            #endregion
        }
    }
}
