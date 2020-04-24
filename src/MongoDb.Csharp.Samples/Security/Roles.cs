using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDb.Csharp.Samples.Expressions
{
    public class Roles : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Security_Roles;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Constants.SamplesDatabase);
        }

        public async Task Run()
        {
            await RolesSamples();
        }

        private async Task RolesSamples()
        {
            var tripsDatabase = Client.GetDatabase(Constants.SamplesDatabase);
            var travelersCollection = tripsDatabase.GetCollection<Traveler>(Constants.TravelersCollection);
            var travelersQueryableCollection = tripsDatabase.GetCollection<Traveler>(Constants.TravelersCollection).AsQueryable();
            var travelersBsonCollection = tripsDatabase.GetCollection<BsonDocument>(Constants.TravelersCollection);
            #region Prepare data

            await travelersCollection.InsertManyAsync(RandomData.GenerateTravelers(10, 5));

            #endregion

            #region Linq

            #endregion


            #region Shell commands

#if false
#endif

            #endregion

        }
    }
}