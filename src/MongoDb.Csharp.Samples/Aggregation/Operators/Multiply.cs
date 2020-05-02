using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDb.Csharp.Samples.Aggregation.Operators
{
    public class Multiply : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Aggregation_Operators_Multiply;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Constants.SamplesDatabase);
        }

        public async Task Run()
        {
            await MultiplySamples();
        }

        private async Task MultiplySamples()
        {
            var tripsDatabase = Client.GetDatabase(Constants.SamplesDatabase);
            var ordersCollection = tripsDatabase.GetCollection<Order>(Constants.OrdersCollection);
            var ordersBsonCollection = tripsDatabase.GetCollection<BsonDocument>(Constants.OrdersCollection);
            #region Prepare data

            await ordersCollection.InsertManyAsync(RandomData.GenerateOrders(100));

            #endregion

            #region Linq

            var multiplyQuery = from o in ordersCollection.AsQueryable()
                              select new
                              {
                                  o.OrderId,
                                  total = o.Quantity * o.Price
                              };

            var multiplyQueryResults = await multiplyQuery.ToListAsync();

            Utils.Log(multiplyQueryResults.Select(r => r.ToBsonDocument()));

            #endregion


            #region Shell commands

#if false
        
#endif

            #endregion

        }
    }
}
