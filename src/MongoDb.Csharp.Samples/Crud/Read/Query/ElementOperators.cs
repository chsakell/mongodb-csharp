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
            Utils.DropDatabase(Client, Constants.SamplesDatabase);
        }

        public async Task Run()
        {
            await ElementOperatorsSamples();
        }

        private async Task ElementOperatorsSamples()
        {
            var database = Client.GetDatabase(Constants.SamplesDatabase);
            var collection = database.GetCollection<Order>(Constants.OrdersCollection);
            var bsonCollection = database.GetCollection<BsonDocument>(Constants.OrdersCollection);

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

            var typeFilter = Builders<Order>.Filter.Type(o => o.ShipmentDetails.ShippedDate, BsonType.DateTime);
            shippedOrders = await collection.Find(typeFilter).ToListAsync();

            var nullContactPhoneFilter = Builders<Order>.Filter
                    .Type(o => o.ShipmentDetails.ContactPhone, BsonType.Null);
            
            var nullContactPhoneOrders = await collection
                    .Find(nullContactPhoneFilter).ToListAsync();
            Utils.Log($"{nullContactPhoneOrders.Count} orders don't contain Contact Phone number");

            #endregion

            #region BsonDocument commands

            var bsonLotNumberFilter = Builders<BsonDocument>.Filter.Exists("lotNumber", exists: true);
            var bsonOrdersWithLotNumber = await collection.Find(lotNumberFilter).ToListAsync();

            var bsonShippedOrdersFilter = Builders<BsonDocument>.Filter
                    .Exists("shipmentDetails.shippedDate");
            var bsonShippedOrders = await bsonCollection.Find(bsonShippedOrdersFilter).ToListAsync();

            var bsonTypeFilter = Builders<BsonDocument>.Filter.Type("shipmentDetails.shippedDate", BsonType.DateTime);
            bsonShippedOrders = await bsonCollection.Find(bsonTypeFilter).ToListAsync();

            var bsonNullContactPhoneFilter = Builders<BsonDocument>.Filter
                .Type("shipmentDetails.contactPhone", BsonType.Null);

            var bsonNullContactPhoneOrders = await bsonCollection
                    .Find(bsonNullContactPhoneFilter).ToListAsync();

            #endregion

            #region Shell commands

#if false
            db.invoices.find({ lotNumber: { $exists: true } })
            db.invoices.find({"shipmentDetails.shippedDate" : { $exists: true }})

            db.invoices.find({"shipmentDetails.shippedDate" : { $type: 9 }})
            db.invoices.find({"shipmentDetails.shippedDate" : { $type: "date" }})

            db.invoices.find({"shipmentDetails.contactPhone" : { $type: 10 }})
            db.invoices.find({"shipmentDetails.contactPhone" : { $type: "null" }})
#endif

            #endregion
        }
    }
}
