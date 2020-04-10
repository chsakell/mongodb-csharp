using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDb.Csharp.Samples.Core;
using MongoDB.Driver;
using Utils = MongoDb.Csharp.Samples.Core.Utils;

namespace MongoDb.Csharp.Samples.Crud.Insert
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
            
            var message = new Message {Text = "hello world"};
            await messagesCollection.InsertOneAsync(message);
            Utils.Log(message.ToBsonDocument());

            #endregion
        }

        // Sample 1
        // MongoDB.Driver.MongoWriteException: A write operation resulted in an error.
        // E11000 duplicate key error collection: Chat.messages index: _id_ dup key: { _id: null }
        public class Message
        {
            public string Id { get; set; }
            public string Text { get; set; }
        }

        // Sample 2
        // Requires the attribute
        //public class Message
        //{
        //    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        //    public string Id { get; set; }
        //    public string Text { get; set; }
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

        // Sample 3_3
        //public class Message
        //{
        //    [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        //    public Guid MyCustomId { get; set; }
        //    public string Text { get; set; }
        //}

        // Sample 4_1
        //public class Message
        //{
        //    public BsonObjectId Id { get; set; }
        //    public string Text { get; set; }
        //}

        // Sample 4_2
        //public class Message
        //{
        //    [BsonId]
        //    public BsonObjectId MyCustomId { get; set; }
        //    public string Text { get; set; }
        //}

        // Sample 4_3
        //public class Message
        //{
        //    [BsonId(IdGenerator = typeof(BsonObjectIdGenerator))]
        //    public BsonObjectId MyCustomId { get; set; }
        //    public string Text { get; set; }
        //}

        // Sample 5_1
        //public class Message
        //{
        //    public ObjectId Id { get; set; }
        //    public string Text { get; set; }
        //}

        // Sample 5_2
        //public class Message
        //{
        //    [BsonId]
        //    public ObjectId CustomId { get; set; }
        //    public string Text { get; set; }
        //}

        // Sample 5_3
        //public class Message
        //{
        //    [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        //    public ObjectId CustomId { get; set; }
        //    public string Text { get; set; }
        //}

        // Sample 6
        // Throws exception System.InvalidOperationException: Id cannot be null.
        //public class Message
        //{
        //    [BsonId(IdGenerator = typeof(NullIdChecker))]
        //    public object Id { get; set; }
        //    public string Text { get; set; }
        //}
    }
}
