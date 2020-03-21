using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDb.Csharp.Samples.Project
{
    public class Projection : RunnableSample, IRunnableSample
    {

        public override Core.Samples Sample => Core.Samples.Projection;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Persons);
        }

        public async Task Run()
        {
            await ProjectionOperations();
        }

        private async Task ProjectionOperations()
        {
            var collectionName = "users";
            var database = Client.GetDatabase(Databases.Persons);
            var collection = database.GetCollection<User>(collectionName);
            var queryableCollection = database.GetCollection<User>(collectionName).AsQueryable();
            var bsonCollection = database.GetCollection<BsonDocument>(collectionName);
            #region Prepare data

            await collection.InsertManyAsync(RandomData.GenerateUsers(1000));

            #endregion

            #region Typed classes commands

            // exclude id, return only gender and date of birth
            var simpleProjection = Builders<User>.Projection
                .Exclude(u => u.Id)
                .Include(u => u.Gender)
                .Include(u => u.DateOfBirth);

            var simpleProjectionResults = await collection.Find(Builders<User>.Filter.Empty)
                .Project(simpleProjection)
                .ToListAsync();

            // return full name (first & last name), gender ('Male' or 'Female'), age
            //var lastNameProjection = Builders<User>.Projection.Expression(u => u.FirstName + " " + u.LastName);
            var customProjection = Builders<User>.Projection.Expression(u =>
                new
                {
                    fullName = u.FirstName + " " + u.LastName,
                    gender = u.Gender.ToString(),
                    age = DateTime.Today.Year - u.DateOfBirth.Year
                });

            var results = await collection.Find(Builders<User>.Filter.Empty)
                .Project(customProjection)
                .ToListAsync();

            foreach (var result in results)
            {
                Utils.Log($"{result.fullName} {result.gender} {result.age}");
            }

            #endregion

            #region BsonDocument commands


            #endregion

            #region Linq

            var linqSimpleProjection = from u in queryableCollection
                                       select new
                                       {
                                           lastName = u.FirstName + " " + u.LastName,
                                           gender = u.Gender == Gender.Male ? "Male" : "Female",
                                           age = DateTime.Now.Year - u.DateOfBirth.Year
                                       };

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
        }
    }
}
