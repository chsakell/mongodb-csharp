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
            await LogicalOperatorsSamples();
        }

        private async Task LogicalOperatorsSamples()
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

            #region not

            // all users that are females, hence not males

            var notMaleFilter = Builders<User>.Filter.Not(
                Builders<User>.Filter.Eq(u => u.Gender, Gender.Male)
            );

            var femaleUsers = await collection.Find(notMaleFilter).ToListAsync();
            Utils.Log($"{femaleUsers.Count} users have female gender");

            #endregion

            #region or

            // users with salary either < 1500 (too low) or > 4000 (too high)
            var orSalaryFilter = Builders<User>.Filter.Or(
                Builders<User>.Filter.Lt(u => u.Salary, 1500),
                Builders<User>.Filter.Gt(u => u.Salary, 4000));

            var lowOrHighSalaryUsers = await collection.Find(orSalaryFilter).ToListAsync();
            Utils.Log($"{lowOrHighSalaryUsers.Count} users have salary either too low or too high");
            
            #endregion

            #region nor

            // users with profession other than Doctor, salary other than < 4500
            // should fail all conditions

            var norFilter = Builders<User>.Filter.And(
                Builders<User>.Filter.Not(Builders<User>.Filter.Eq(u => u.Profession, "Doctor")),
                Builders<User>.Filter.Not(Builders<User>.Filter.Lt(u => u.Salary, 4500)));

            var norUsers = await collection.Find(norFilter).ToListAsync();
            Utils.Log($"{norUsers.Count} users aren't doctors and have salary greater than 4500");

            var firstFilterToFail = Builders<User>.Filter.Eq(u => u.Profession, "Doctor");
            var secondFilterToFail = Builders<User>.Filter.Lt(u => u.Salary, 4500);
            var extensionNorFilter = Builders<User>.Filter.Nor(firstFilterToFail, secondFilterToFail);

            var extensionUsers = await collection.Find(norFilter).ToListAsync();

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

            #region not

            // all users that are females, hence not females

            var bsonNotMaleFilter = Builders<BsonDocument>.Filter.Not(
                Builders<BsonDocument>.Filter.Eq("gender", Gender.Male)
            );

            var bsonFemaleUsers = await bsonCollection.Find(bsonNotMaleFilter).ToListAsync();

            #endregion

            #region or

            var bsonOrSalaryFilter = Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.Lt("salary", 1500),
                Builders<BsonDocument>.Filter.Gt("salary", 4000));

            var bsonLowOrHighSalaryUsers = await bsonCollection.Find(bsonOrSalaryFilter).ToListAsync();

            #endregion

            #region nor

            var bsonNorFilter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Not(Builders<BsonDocument>.Filter.Eq("profession", "Doctor")),
                Builders<BsonDocument>.Filter.Not(Builders<BsonDocument>.Filter.Lt("salary", 4500)));

            var bsonNorUsers = await bsonCollection.Find(bsonNorFilter).ToListAsync();

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
            
            db.users.find( { gender: { $not: { $eq: 0   } }} ) |  db.users.find( { gender: { $ne: 1 }} )
            db.users.find( { $or: [ { salary: { $lt: 1500 } }, { salary: { $gt: 4000 }}]})

            db.users.find( { $nor: [ 
                                    { profession: { $eq: "Doctor" } }, 
                                    { salary: { $lt: 4500 }}
                                ]})
#endif

            #endregion
        }
    }
}
