using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Read.Query
{
    public class ElementOperators : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Crud_Read_Query_ElementOperators;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Orders);
        }

        public async Task Run()
        {
            await ElementOperatorsOperations();
        }

        private async Task ElementOperatorsOperations()
        {
            var collectionName = "invoices";
            var database = Client.GetDatabase(Databases.Orders);
            var collection = database.GetCollection<Order>(collectionName);
            var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

            #region Prepare data

            var orders = RandomData.GenerateOrders(1000);

            await collection.InsertManyAsync(orders);

            #endregion

            #region Typed classes commands

            var lotNumberFilter = Builders<Order>.Filter.Exists(o => o.LotNumber, exists:true);
            var ordersWithLotNumber = await collection.Find(lotNumberFilter).ToListAsync();
            Utils.Log($"{ordersWithLotNumber.Count} orders have Lot number");

            // use exists in nested property
            var shippedOrdersFilter = Builders<Order>.Filter.Exists(o => o.ShipmentDetails.ShippedDate);
            var shippedOrders = await collection.Find(shippedOrdersFilter).ToListAsync();
            Utils.Log($"{shippedOrders.Count} orders have been already shipped");
            #endregion

            #region BsonDocument commands

            var bsonLotNumberFilter = Builders<BsonDocument>.Filter.Exists("lotNumber", exists: true);
            var bsonOrdersWithLotNumber = await collection.Find(lotNumberFilter).ToListAsync();


            var bsonShippedOrdersFilter = Builders<BsonDocument>.Filter.Exists("shipmentDetails.shippedDate");
            var bsonShippedOrders = await bsonCollection.Find(bsonShippedOrdersFilter).ToListAsync();
            #endregion

            #region Shell commands

#if false
            db.invoices.find({ lotNumber: { $exists: true } })
            db.invoices.find({"shipmentDetails.shippedDate" : { $exists: true }})
#endif

            #endregion
        }
    }
}
