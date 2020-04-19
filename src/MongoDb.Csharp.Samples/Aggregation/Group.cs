using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Aggregation
{
    public class GroupStage : RunnableSample, IRunnableSample
    {

        public override Core.Samples Sample => Core.Samples.Aggregation_Stages_Group;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Persons);
        }

        public async Task Run()
        {
            await GroupStageSamples();
        }

        private async Task GroupStageSamples()
        {
            var collectionName = "users";
            var database = Client.GetDatabase(Databases.Persons);
            var collection = database.GetCollection<User>(collectionName);
            var bsonCollection = database.GetCollection<BsonDocument>(collectionName);
            #region Prepare data

            await collection.InsertManyAsync(RandomData.GenerateUsers(1000));

            #endregion

            #region Typed classes commands

            #region group on single field

            // group by profession
            var singleFieldAggregate = collection.Aggregate()
                .Group(u => u.Profession,
                    ac => new { profession = ac.Key, total = ac.Sum(u => 1)} );

            var groupedProfessions = await singleFieldAggregate.ToListAsync();

            Utils.Log("Grouped by profession:");
            foreach (var group in groupedProfessions)
            {
                Utils.Log($"{group.profession}: {group.total}");
            }

            #endregion

            #region group by embedded document field

            // group by address state and sorted
            var embeddedDocFieldAggregate = collection.Aggregate()
                .Group(u => u.Address.State,
                    ac => new {state = ac.Key, total = ac.Sum(u => 1)})
                .SortBy(group => group.total); // ASC

            var groupedPerCountry = await embeddedDocFieldAggregate.ToListAsync();

            Utils.Log("Grouped by address state sorted:");
            foreach (var group in groupedPerCountry)
            {
                Utils.Log($"{group.state}: {group.total}");
            }

            #endregion

            #endregion

            #region excercices

            //      a) find users with 1500 < 3000 salary,
            //      b) group by gender & display average monthly expenses
            //      c) sort in descending order on avg monthly expenses
            //      truncation resulted in data loss => https://jira.mongodb.org/browse/CSHARP-2399
            var excercice1Aggregate = collection.Aggregate()
                .Match(Builders<User>.Filter.Gte(u => u.Salary, 1500) &
                       Builders<User>.Filter.Lte(u => u.Salary, 3000))
                .Group(u => u.Gender,
                    ac => new
                    {
                        gender = ac.Key,
                        averageMonthlyExpenses = ac.Average(u => u.MonthlyExpenses),
                        total = ac.Sum(u => 1)
                    })
                .Project(group => new 
                {
                    Gender = group.gender == 0 ? "Male" : "Female",
                    AverageMonthlyExpenses = group.averageMonthlyExpenses,
                    Total = group.total
                })
                .SortByDescending(group => group.AverageMonthlyExpenses);

            var excercice1Result = await excercice1Aggregate.ToListAsync();

            Utils.Log("Grouped by gender with average monthly expenses");
            
            
            var linqQuery = collection.AsQueryable()
                .Where(u => u.Salary > 1500 && u.Salary < 3000)
                .GroupBy(u => u.Gender)
                .Select(ac => new
                {
                    gender = ac.Key,
                    averageMonthlyExpenses = Math.Ceiling(ac.Average(u => u.MonthlyExpenses)),
                    total = ac.Sum(u => 1)
                })
                .OrderBy(group => group.total);

            var excercice1LinqResult = linqQuery.ToList();

            foreach (var group in excercice1LinqResult)
            {
                Utils.Log($"{group.gender}: total - {group.total}, average monthly expenses - {group.averageMonthlyExpenses}");
            }

            // count births by year
            var excercice2Aggregate = collection.Aggregate()
                .Group(u => u.DateOfBirth.Year,
                    ac => new
                    {
                        year = ac.Key,
                        total = ac.Sum(u => 1)
                    })
                .SortByDescending(group => group.year);

            var excercice2AggregateResult = await excercice2Aggregate.ToListAsync();

            #endregion


            #region Shell commands

#if false

            db.users.aggregate([
             { 
                $match : { $and: [ { salary: { $gte: 3500 }}, { salary: { $lte: 5000 }}  ] } 
             }])

           db.users.aggregate([
                { "$group" : { "_id" : "$profession", "total" : { "$sum" : 1 } } }
            ])
            
            db.users.aggregate([
                { "$group" : { "_id" : "$address.state", "total" : { "$sum" : 1 } } },
                { "$sort" : { "total" : 1 } }
            ])

            db.users.aggregate([
                { "$match" : { "salary" : { "$gte" : NumberDecimal("1500"), "$lte" : NumberDecimal("3000") } } }, 
                { "$group" : { "_id" : "$gender", 
                                "averageMonthlyExpenses" : { "$avg" : "$monthlyExpenses" }, 
                                "total" : { "$sum" : 1 } } }, 
                { "$sort" : { "averageMonthlyExpenses" : -1 } }
            ])

            db.users.aggregate([
                { "$group" : 
                    { "_id" : { "$year" : "$dateOfBirth" }, "total" : { "$sum" : 1 } } 
                }, 
                { "$sort" : { "_id" : -1 } }
            ])


#endif

            #endregion
        }
    }
}
