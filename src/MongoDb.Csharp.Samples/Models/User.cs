using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace MongoDb.Csharp.Samples.Models
{
    public class User
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
        public AddressCard Address {get; set; }
        public string Phone {get; set; }
        public string Website {get; set; }
        public CompanyCard Company {get; set; }
        public decimal Salary { get; set; }
    }
}
