using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Insert
{
    public class WriteConcernInsert : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Crud_Insert_WriteConcern;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Constants.SamplesDatabase);
        }

        public async Task Run()
        {
            await WriteConcernSamples();
        }

        private async Task WriteConcernSamples()
        {
            var personsDatabase = Client.GetDatabase(Constants.SamplesDatabase);


            #region Prepare data
            var writeConcern = new WriteConcern(w: new Optional<WriteConcern.WValue>("0"), journal: false);
            var user = RandomData.GenerateUsers(1).First();
            var usersCollection = personsDatabase.GetCollection<User>(Constants.UsersCollection)
                .WithWriteConcern(writeConcern);

            // Requests no acknowledgment of the write operation
            // no user id available!
            // Command insert failed: cannot use non-majority 'w' mode 0 when a host is not a member of a replica set.'
            await usersCollection.InsertOneAsync(user);

            List<User> users = RandomData.GenerateUsers(3000);

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
