using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using Utils = MongoDb.Csharp.Samples.Core.Utils;

namespace MongoDb.Csharp.Samples.QuickStart
{
    public class IdMember : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Crud_Insert_IdMember;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Chat);
        }

        public async Task Run()
        {
            await IdMemberSamples();
        }
        private async Task IdMemberSamples()
        {
            var chatDatabase = Client.GetDatabase(Databases.Chat);

            #region Prepare data

            // Will create the users collection on the fly if it doesn't exists
            var messagesCollection = chatDatabase.GetCollection<Message>(" messages");

            #endregion

            #region Typed classes commands
          

            var message = new Message { Text = "hello world" };
            await messagesCollection.InsertOneAsync(message);
            Utils.Log(message.ToBsonDocument());

            #endregion

            #region BsonDocument commands

            #endregion

            #region Shell commands

            #endregion
        }
    }

    // Sample 1
    // MongoDB.Driver.MongoWriteException: A write operation resulted in an error.
    // E11000 duplicate key error collection: Chat.messages index: _id_ dup key: { _id: null }
    //public class Message
    //{
    //    public string Id { get; set; }
    //    public string Text { get; set; }
    //}

    // Sample 2
    // Requires the attribute
    //public class Message
    //{
    //[BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    //public string Id { get; set; }
    //public string Text { get; set; }
    //}

    // Sample 3_1
    // Works without attributes
    //public class Message
    //{
    //    public Guid Id { get; set; }
    //    public string Text { get; set; }
    //}

    // Sample 3_2
    //public class Message
    //{
    //    [BsonId(IdGenerator = typeof(GuidGenerator))]
    //    public Guid MyCustomId { get; set; }
    //    public string Text { get; set; }
    //}

    // Sample 4
    //public class Message
    //{
    //    public BsonObjectId Id { get; set; }
    //    public string Text { get; set; }
    //}

    public class Message
    {
        [BsonId]
        public BsonObjectId MyCustomId { get; set; }
        public string Text { get; set; }
    }
}
