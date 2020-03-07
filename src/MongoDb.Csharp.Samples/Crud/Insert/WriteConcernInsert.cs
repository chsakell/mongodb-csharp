using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Insert
{
    public class WriteConcernInsert : RunnableSample, IRunnableSample
    {
        public bool Enabled => false;
        protected override Core.Samples Sample => Core.Samples.Crud_Write_Concern;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Persons);
        }

        public async Task Run()
        {
            await WriteConcernOperations();
        }

        private async Task WriteConcernOperations()
        {
            var personsDatabase = Client.GetDatabase(Databases.Persons);


            #region Prepare data
            var writeConcern = new WriteConcern(w: new Optional<WriteConcern.WValue>("0"), journal: false);
            var user = RandomData.GeneratePerson();
            var usersCollection = personsDatabase.GetCollection<User>("users")
                .WithWriteConcern(writeConcern);

            // Requests no acknowledgment of the write operation
            // no user id available!
            // Command insert failed: cannot use non-majority 'w' mode 0 when a host is not a member of a replica set.'
            await usersCollection.InsertOneAsync(user);

            List<User> users = new List<User>();

            for (int i = 0; i < 3000; i++)
            {
                users.Add(RandomData.GeneratePerson());
            }

            await usersCollection.InsertManyAsync(users);

            var totalUsers = await usersCollection.CountDocumentsAsync(Builders<User>.Filter.Empty);

            var unassigned = users.Where(u => u.Id.Equals(default(ObjectId)));

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
