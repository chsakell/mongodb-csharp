using System.Linq;
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
            await GroupStageOperations();
        }

        private async Task GroupStageOperations()
        {
            var collectionName = "users";
            var database = Client.GetDatabase(Databases.Persons);
            var collection = database.GetCollection<User>(collectionName);
            var bsonCollection = database.GetCollection<BsonDocument>(collectionName);
            #region Prepare data

            await collection.InsertManyAsync(RandomData.GenerateUsers(1000));

            #endregion

            #region Typed classes commands

            var aggregate = collection.Aggregate()
                .Group(u => u.Profession,
                    ac => new { profession = ac.Key, total = ac.Sum(u => 1)} );

            var groupedProfessions = await aggregate.ToListAsync();

            foreach (var group in groupedProfessions)
            {
                Utils.Log($"{group.profession}: {group.total}");
            }
            
            #endregion

            #region BsonDocument commands


            #endregion

            #region Shell commands

#if false
            db.users.aggregate([
             { 
                $match : { $and: [ { salary: { $gte: 3500 }}, { salary: { $lte: 5000 }}  ] } 
             }])

#endif

            #endregion
        }
    }
}
