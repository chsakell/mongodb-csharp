using System;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDb.Csharp.Samples.Models
{
    public class Order
    {
        [BsonId]
        public int OrderId { get; set; }
        public string Item { get; set; }
        public  int Price { get; set; }
        public int Quantity { get; set; }

        [BsonIgnoreIfDefault]
        public int? LotNumber { get; set; }
        public ShipmentDetails ShipmentDetails { get; set; }
    }

    public class ShipmentDetails
    {
        [BsonIgnoreIfDefault]
        public DateTime? ShippedDate { get; set; }
        public string ShipAddress { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
    }
}
