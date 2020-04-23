using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using Utils = MongoDb.Csharp.Samples.Core.Utils;

namespace MongoDb.Csharp.Samples.QuickStart
{
    public class AccessCollections : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.QuickStart_AccessCollections;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Constants.SamplesDatabase);
        }

        public async Task Run()
        {
            await CollectionSamples();
        }
        private async Task CollectionSamples()
        {
            var database = Client.GetDatabase(Constants.SamplesDatabase);

            #region Typed classes commands

            // Will create the users collection on the fly if it doesn't exists
            var personsCollection = database.GetCollection<User>(Constants.UsersCollection);
            var bsonPersonsCollection = database.GetCollection<BsonDocument>(Constants.UsersCollection);

            User typedUser = RandomData.GenerateUsers(1).First();
            await personsCollection.InsertOneAsync(typedUser);

            // Create another collection
            var loginsCollectionName = "logins";
            await database.CreateCollectionAsync(loginsCollectionName);

            // list collections
            var collections = (await database.ListCollectionsAsync()).ToList();
            Utils.Log(collections, "List Collections");

            // remove collection
            await database.DropCollectionAsync(loginsCollectionName);

            #region Capped collection

            // create a capped collection
            // 'size' field is required when 'capped' is true
            await database
                .CreateCollectionAsync(Constants.TravelersCollection,
                    new CreateCollectionOptions()
                    {
                        Capped = true, MaxDocuments = 3, MaxSize = 10000
                    });

            var travelers = RandomData.GenerateTravelers(3);
            travelers.First().Name = "Christos";

            var travelersCollection = database
                .GetCollection<Traveler>(Constants.TravelersCollection);

            await travelersCollection.InsertManyAsync(travelers);

            // Max documents reached - Now let's insert another one
            await travelersCollection.InsertManyAsync(RandomData.GenerateTravelers(1));

            // Read all the docs
            var dbTravelers = await travelersCollection.Find(Builders<Traveler>.Filter.Empty).ToListAsync();

            // First user 'Christos' has been removed from the collection so that the new one can fit in

            #endregion


            #endregion

        }
    }
}
