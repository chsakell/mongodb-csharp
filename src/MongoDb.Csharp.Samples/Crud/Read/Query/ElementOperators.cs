using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Read.Query
{
    public class ElementOperators : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Crud_Read_Query_ElementOperators;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Persons);
        }

        public async Task Run()
        {
            await ElementOperatorsOperations();
        }

        private async Task ElementOperatorsOperations()
        {
            var collectionName = "users";
            var database = Client.GetDatabase(Databases.Persons);
            var collection = database.GetCollection<User>(collectionName);
            var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

            #region Prepare data

            var users = new List<User>();
            for (int i = 0; i < 1000; i++)
            {
                users.Add(RandomData.GeneratePerson());
            }

            await collection.InsertManyAsync(users);

            #endregion

            #region Typed classes commands

            #endregion

            #region BsonDocument commands

            #endregion

            #region Shell commands

#if false
#endif

            #endregion
        }
    }
}
