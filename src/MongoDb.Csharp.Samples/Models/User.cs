using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDb.Csharp.Samples.Models
{
    public class User
    {
        [BsonId]
        [BsonIgnoreIfDefault] // required for replace documents 
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
        
        [BsonIgnoreIfDefault]
        public string Website {get; set; }
        public CompanyCard Company {get; set; }
        public decimal Salary { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public List<string> FavoriteSports { get; set; }
        public string Profession { get; set; }
    }
}
