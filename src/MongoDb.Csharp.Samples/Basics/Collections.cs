using System.Threading.Tasks;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using Utils = MongoDb.Csharp.Samples.Core.Utils;

namespace MongoDb.Csharp.Samples.Basics
{
    public class Collections : RunnableSample, IRunnableSample
    {
        public bool Enabled => true;
        protected override Core.Samples Sample => Core.Samples.Basic_Collections;
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
            var personsTypedCollection = usersDatabase.GetCollection<AppPerson>("users");

            AppPerson typedUser = RandomData.GeneratePerson();
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
