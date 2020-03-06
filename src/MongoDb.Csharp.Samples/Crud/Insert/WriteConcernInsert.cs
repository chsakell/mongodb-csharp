using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Insert
{
    public class WriteConcernInsert : RunnableSample, IRunnableSample
    {
        public bool Enabled => true;
        protected override Core.Samples Sample => Core.Samples.Crud_Write_Concern;
        protected override void Init()
        {
            // Register Classes
            BsonClassMap.RegisterClassMap<Sport>();
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Betting);
        }

        public async Task Run()
        {
            await WriteConcernOperations();
        }

        private async Task WriteConcernOperations()
        {
            var personsDatabase = Client.GetDatabase(Databases.Betting);
            

            #region Prepare data

            var user = RandomData.GeneratePerson();
            var usersCollection = personsDatabase.GetCollection<User>("users")
                .WithWriteConcern(WriteConcern.Unacknowledged);
 

            await usersCollection.InsertOneAsync(user);

            List<User> users = new List<User>();

            for (int i = 0; i < 3000; i++)
            {
                
            }


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
