using MongoDB.Bson;

namespace MongoDb.Csharp.Samples.Models
{
    public class Product
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}
