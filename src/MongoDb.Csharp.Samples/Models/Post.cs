using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDb.Csharp.Samples.Models
{
    public class Post
    {
        public ObjectId Id { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public List<Comment> Comments { get; set; }
    }

    public class Comment
    {
        public string Text { get; set; }
        public string Author { get; set; }
        public int Votes { get; set; }
        public bool Hidden { get; set; }
    }
}
