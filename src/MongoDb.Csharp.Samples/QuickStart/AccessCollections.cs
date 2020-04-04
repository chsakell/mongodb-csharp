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
            Utils.DropDatabase(Client, Core.Databases.Persons);
            Utils.DropDatabase(Client, Core.Databases.Trips);
        }

        public async Task Run()
        {
            await CollectionSamples();
        }
        private async Task CollectionSamples()
        {
            var usersDatabase = Client.GetDatabase(Databases.Persons);
            var tripsDatabase = Client.GetDatabase(Databases.Trips);

            #region Typed classes commands

            // Will create the users collection on the fly if it doesn't exists
            var personsCollection = usersDatabase.GetCollection<User>("users");
            var bsonPersonsCollection = usersDatabase.GetCollection<BsonDocument>("users");

            User typedUser = RandomData.GenerateUsers(1).First();
            await personsCollection.InsertOneAsync(typedUser);

            // Create another collection
            var loginsCollectionName = "logins";
            await usersDatabase.CreateCollectionAsync(loginsCollectionName);

            // list collections
            var collections = (await usersDatabase.ListCollectionsAsync()).ToList();
            Utils.Log(collections, "List Collections");

            // remove collection
            await usersDatabase.DropCollectionAsync(loginsCollectionName);

            #region Capped collection

            // create a capped collection
            // 'size' field is required when 'capped' is true
            var travelersCollectionName = "travelers";
            await tripsDatabase
                .CreateCollectionAsync(travelersCollectionName,
                    new CreateCollectionOptions()
                    {
                        Capped = true, MaxDocuments = 3, MaxSize = 10000
                    });

            var travelers = RandomData.GenerateTravelers(3);
            travelers.First().Name = "Christos";

            var travelersCollection = tripsDatabase
                .GetCollection<Traveler>(travelersCollectionName);

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
