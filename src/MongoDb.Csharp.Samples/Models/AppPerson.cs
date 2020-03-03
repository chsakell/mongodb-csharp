using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace MongoDb.Csharp.Samples.Models
{
    public class AppPerson
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public Gender Gender { get; set; }
        public string FirstName {get; set; }
        public string LastName {get; set; }
        public string UserName {get; set; }
        public string Avatar {get; set; }
        public string Email {get; set; }
        public DateTime DateOfBirth {get; set; }
        public AppCardAddress Address {get; set; }
        public string Phone {get; set; }
        public string Website {get; set; }
        public AppCardCompany Company {get; set; }
        public decimal Salary { get; set; }
    }
}
