using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDb.Csharp.Samples.Aggregation
{
    public class LimitSkipStages : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Aggregation_Stages_Limit_Skip;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Constants.SamplesDatabase);
        }

        public async Task Run()
        {
            await LimitSkipSamples();
        }

        private async Task LimitSkipSamples()
        {
            var personsDatabase = Client.GetDatabase(Constants.SamplesDatabase);
            var usersCollection = personsDatabase.GetCollection<User>(Constants.UsersCollection);
            var usersQueryableCollection = personsDatabase.GetCollection<User>(Constants.UsersCollection).AsQueryable();
            var usersBsonCollection = personsDatabase.GetCollection<BsonDocument>(Constants.UsersCollection);
            #region Prepare data

            await usersCollection.InsertManyAsync(RandomData.GenerateUsers(100));

            var skipSize = 3;
            var limitSize = 3;
            #endregion

            #region Typed

            #region Top Level

            // In find order doesn't matter but in pipelines it does!

            // Order users by their birth date, older persons first

            var topLevelProjection = Builders<User>.Projection
                .Exclude(u => u.Id)
                .Include(u => u.UserName)
                .Include(u => u.DateOfBirth);

            var topLevelProjectionResults = await usersCollection.Find(Builders<User>.Filter.Empty)
                .Project(topLevelProjection)
                .SortBy(u => u.DateOfBirth)
                .Skip(skipSize)
                .Limit(limitSize)
                .ToListAsync();

            foreach (var topLevelProjectionResult in topLevelProjectionResults)
            {
                Utils.Log(topLevelProjectionResult.ToJson());
            }

            var linqTopLevelResults = await usersQueryableCollection
                .Select(u => new { u.UserName, u.DateOfBirth })
                .OrderBy(u => u.DateOfBirth)
                .Skip(skipSize)
                .Take(limitSize)
                .ToListAsync();

            #endregion

            #endregion

            #region BsonDocument commands

            var bsonTopLevelProjection = Builders<BsonDocument>.Projection
                .Exclude("_id")
                .Include("userName")
                .Include("dateOfBirth");

            var bsonTopLevelProjectionResults = await usersBsonCollection.Find(Builders<BsonDocument>.Filter.Empty)
                .Project(bsonTopLevelProjection)
                .SortBy(doc => doc["dateOfBirth"])
                .Skip(skipSize)
                .Limit(limitSize)
                .ToListAsync();

            #endregion

            #region Shell commands

            #if false
            db.users.aggregate([
                { "$project" : { _id: 0, userName: 1, dateOfBirth: 1 } },
                { "$sort" : { dateOfBirth: 1 } },
                { "$skip":  3 },
                { "$limit": 3 }
            ])
            #endif

            #endregion

            #region Tips

            // Let's paginate in the favoriteSports array field
            var user = await usersCollection.Find(u => u.FavoriteSports.Count > 10).FirstOrDefaultAsync();
            Utils.Log(user.Id.ToString());

            var sliceQuery = usersQueryableCollection
                .Where(u => u.Id == user.Id)
                .SelectMany(u => u.FavoriteSports, (u, s) => new
                {
                    id = u.Id,
                    sport = s
                })
                .OrderBy(u => u.sport)
                .Skip(skipSize)
                .Take(limitSize)
                .GroupBy(q => q.id)
                .Select(g => new
                {
                    id = g.Key,
                    sports = g.Select(a => a.sport)
                });


            var sliceQueryResults = await sliceQuery.FirstOrDefaultAsync();

            /*
            db.users.aggregate([
                { "$match" : { _id: ObjectId("5e7f6778760dd709946ee276") } },
                { "$unwind" : "$favoriteSports" },
                { "$sort" : { favoriteSports: 1 } },
                { "$group": { _id: '$_id', 'favoriteSports': {$push: '$favoriteSports'} } },
                { "$project" : { _id: 0, sports: { $slice: ["$favoriteSports", 2, 2] } } }
            ])
            */

            #endregion

        }
    }
}
