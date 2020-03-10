using MongoDB.Bson.Serialization.Attributes;

namespace MongoDb.Csharp.Samples.Models
{
    public class Order
    {
        [BsonId]
        public int OrderId { get; set; }
        public string Item { get; set; }
        public int Quantity { get; set; }

        [BsonIgnoreIfDefault]
        public int? LotNumber { get; set; }
    }
}
