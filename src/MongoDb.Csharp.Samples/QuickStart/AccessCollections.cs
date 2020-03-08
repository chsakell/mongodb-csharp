using System.Threading.Tasks;
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
        }

        public async Task Run()
        {
            await CollectionOperations();
        }
        private async Task CollectionOperations()
        {
            var usersDatabase = Client.GetDatabase(Core.Databases.Persons);

            #region Typed classes commands

            // Will create the users collection on the fly if it doesn't exists
            var personsTypedCollection = usersDatabase.GetCollection<User>("users");

            User typedUser = RandomData.GeneratePerson();
            await personsTypedCollection.InsertOneAsync(typedUser);

            // Create another collection
            var loginsCollectionName = "logins";
            await usersDatabase.CreateCollectionAsync(loginsCollectionName);

            // list collections
            var collections = (await usersDatabase.ListCollectionsAsync()).ToList();
            Utils.Log(collections, "List Collections");

            // remove collection
            await usersDatabase.DropCollectionAsync(loginsCollectionName);

            #endregion
           
        }
    }
}
