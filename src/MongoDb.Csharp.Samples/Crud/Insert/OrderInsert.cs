using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Insert
{
    public class OrderInsert : RunnableSample, IRunnableSample
    {
        public bool Enabled => true;
        protected override Core.Samples Sample => Core.Samples.Crud_Insert_Ordered;
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
            await OrderedInsertOperations();
        }

        private async Task OrderedInsertOperations()
        {
            var bettingDatabase = Client.GetDatabase(Databases.Betting);
            var sportsCollection = bettingDatabase.GetCollection<Sport>("sports");
            #region Prepare data

            var sports = new List<Sport>
            {
                new Sport(1, "Soccer", 100),
                new Sport(2, "Basketball", 56),
                new Sport(3, "Tennis", 15)
            };

            await sportsCollection.InsertManyAsync(sports);

            #endregion

            #region Typed classes commands

            var sportsToAdd = new List<Sport>
            {
                new Sport(4, "Volleyball", 12),
                new Sport(2, "Baseball", 44), // This should cause an error
                new Sport(5, "Formula 1", 67)
            };

            try
            {
                // default behavior, doesn't rollback on failure but doesn't continue either - ORDERED INSERT
                await sportsCollection.InsertManyAsync(sportsToAdd);
            }
            catch (MongoBulkWriteException e)
            {
                Utils.Log(e.Message);
            }

            try
            {
                var sportsToAddWithRollback = new List<Sport>
                {
                    new Sport(5, "Volleyball", 12),
                    new Sport(6, "Baseball", 44), 
                    new Sport(1, "Formula 1", 67), // This should cause an error
                    new Sport(7, "Moto GP", 12) // But this will be inserted as well
                };

                await sportsCollection.InsertManyAsync(sportsToAddWithRollback, 
                    new InsertManyOptions
                    { 
                        IsOrdered = false
                    });
            }
            catch (MongoBulkWriteException e)
            {
                Utils.Log(e.Message);
            }

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
