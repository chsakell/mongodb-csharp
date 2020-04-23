using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Insert
{
    public class OrderedInsert : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Crud_Insert_OrderedInsert;
        protected override void Init()
        {
            // Register Classes
            BsonClassMap.RegisterClassMap<Sport>();
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Constants.SamplesDatabase);
        }

        public async Task Run()
        {
            await OrderedInsertSamples();
        }

        private async Task OrderedInsertSamples()
        {
            var bettingDatabase = Client.GetDatabase(Constants.SamplesDatabase);

            var sportsCollection = bettingDatabase.GetCollection<Sport>(Constants.SportsCollection);
            #region Prepare data

            var sports = new List<Sport>
            {
                new Sport { Title = "Soccer", TotalEvents = 100 },
                new Sport { Title = "Basketball", TotalEvents = 50 },
                new Sport { Title = "Tennis", TotalEvents = 60 },
            };

            await sportsCollection.InsertManyAsync(sports);

            #endregion

            #region Typed classes commands

            var sportsToAdd = new List<Sport>
            {
                new Sport { Title = "Volleyball", TotalEvents = 12 },
                new Sport { Title = "Basketball", TotalEvents = 44 }, // This should cause an error
                new Sport { Title = "Formula 1", TotalEvents = 67 },
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
                // clean the collection
                await sportsCollection.DeleteManyAsync(Builders<Sport>.Filter.Empty);

                await sportsCollection.InsertManyAsync(sports);

                var sportsToAddWithRollback = new List<Sport>
                {
                    new Sport { Title = "Volleyball", TotalEvents = 12 },
                    new Sport { Title = "Basketball", TotalEvents = 11 }, // This should cause an error
                    new Sport { Title = "Baseball", TotalEvents = 44 }, // But this will be inserted as well
                    new Sport { Title = "Tennis", TotalEvents = 67 }, // This should cause an error
                    new Sport { Title = "Moto GP", TotalEvents = 12 } // But this will be inserted as well
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
