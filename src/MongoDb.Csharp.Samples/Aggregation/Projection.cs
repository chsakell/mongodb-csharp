using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDb.Csharp.Samples.Aggregation
{
    public class ProjectionStage : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Aggregation_Stages_Projection;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Persons);
        }

        public async Task Run()
        {
            await ProjectionSamples();
        }

        private async Task ProjectionSamples()
        {
            var usersCollectionName = "users";
            var personsDatabase = Client.GetDatabase(Databases.Persons);
            var collection = personsDatabase.GetCollection<User>(usersCollectionName);
            var usersQueryableCollection = personsDatabase.GetCollection<User>(usersCollectionName).AsQueryable();
            var bsonCollection = personsDatabase.GetCollection<BsonDocument>(usersCollectionName);

            var travelersCollectionName = "travelers";
            var tripsDatabase = Client.GetDatabase(Databases.Trips);
            var travelersCollection = tripsDatabase.GetCollection<Traveler>(travelersCollectionName);
            #region Prepare data

            await collection.InsertManyAsync(RandomData.GenerateUsers(500));
            await travelersCollection.InsertManyAsync(RandomData.GenerateTravelers(500));

            #endregion

            #region Typed classes commands

            // exclude id, return only gender and date of birth
            var simpleProjection = Builders<User>.Projection
                .Exclude(u => u.Id)
                .Include(u => u.Gender)
                .Include(u => u.DateOfBirth);

            var simpleProjectionResults = await collection
                .Find(Builders<User>.Filter.Empty)
                .Project(simpleProjection)
                .ToListAsync();

            // return full name (first & last name), gender ('Male' or 'Female'), age
            //var lastNameProjection = Builders<User>.Projection.Expression(u => u.FirstName + " " + u.LastName);
            var customProjection = Builders<User>.Projection.Expression(u =>
                new
                {
                    fullName = $"{u.FirstName} {u.LastName}",
                    fullNameUpper = ToUpperCase($"{u.FirstName} {u.LastName}"),
                    gender = u.Gender.ToString(),
                    age = DateTime.Today.Year - u.DateOfBirth.Year
                });

            var results = await collection.Find(Builders<User>.Filter.Empty)
                .Project(customProjection)
                .ToListAsync();

            foreach (var result in results)
            {
                Utils.Log(result.ToBsonDocument());
            }

            #endregion

            #region BsonDocument commands

            var bsonSimpleProjection = Builders<BsonDocument>.Projection
                .Exclude("_id")
                .Include("gender")
                .Include("dateOfBirth");

            var bsonSimpleProjectionResults = await bsonCollection
                .Find(Builders<BsonDocument>.Filter.Empty)
                .Project(bsonSimpleProjection)
                .ToListAsync();

            #endregion

            #region Linq

            var linqSimpleProjection = 
                from u in usersQueryableCollection
               select new
               {
                   fullName = u.FirstName + " " + u.LastName,
                   fullNameUpper = ToUpperCase($"{u.FirstName} {u.LastName}"),
                   gender = u.Gender == Gender.Male ? "Male" : "Female",
                   age = DateTime.Now.Year - u.DateOfBirth.Year
               };

            Utils.Log($"Query built: {linqSimpleProjection}");
            var linqSimpleProjectionResults = await linqSimpleProjection.ToListAsync();

            #endregion

            #region Shell commands

#if false
            db.users.aggregate([
             { $project:  { 
                    gender: {
                        $switch: {
                          branches: [
                            { case: { $eq: [ "$gender", 0 ] }, then: "Male" },
                          ],
                          default: "Female"
                       }
                    }, 
                    fullName: { $concat: [ "$firstName", " ", "$lastName" ]  },
                    age: { 
                        $toInt : {
                            $divide: [{$subtract: [ new Date(), "$dateOfBirth" ] }, (365 * 24*60*60*1000)]
                        }
                    } 
                } 
             }
            ])


            db.users.aggregate([
            { "$project" : 
                { 
                    "lastName" : { "$concat" : ["$firstName", " ", "$lastName"] }, 
                    "gender" : { 
                        "$cond" : [{ "$eq" : ["$gender", 0] }, "Male", "Female"] 
                     }, 
                    "age" : { "$subtract" : [2020, { "$year" : "$dateOfBirth" }] }, 
                    "_id" : 0 
                } 
            }])
#endif

            #endregion

            #region 
            // return the first user with address in one field

            var exercice_1_projection =
                from user in usersQueryableCollection
                select new
                {
                    fullName = user.FirstName + " " + user.LastName,
                    address = user.Address.Street + ", " + user.Address.City +", Zip Code:" +  user.Address.ZipCode
                };

            var exercice_1_result = await exercice_1_projection.FirstOrDefaultAsync();

            #endregion
        }

        private string ToUpperCase(string value)
        {
            return value.ToUpper();
        }
    }
}
