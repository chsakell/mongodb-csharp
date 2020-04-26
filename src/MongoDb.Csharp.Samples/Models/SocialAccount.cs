using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDb.Csharp.Samples.Models
{
public class SocialAccount
    {
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public RelationShips RelationShips { get; set; }

        [BsonIgnoreIfDefault]
        public List<Notification> LastNotifications { get; set; }
    }

    public class RelationShips
    {
        public List<string> Friends { get; set; }
        public List<string> Blocked { get; set; }
    }

    public class Notification
    {
        public string Text { get; set; }
        public string Link { get; set; }
    }
}
