using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDb.Csharp.Samples.Models
{
    public class StoreItem
    {
        public BsonObjectId Id { get; set; }

        [BsonIgnoreIfDefault]
        public List<string> PcGames { get; set; }
        [BsonIgnoreIfDefault]
        public List<string> XboxGames { get; set; }
    }
}
