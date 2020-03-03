using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDB.Driver;
using Utils = MongoDb.Csharp.Samples.Core.Utils;

namespace MongoDb.Csharp.Samples.QuickStart
{
    public class Databases : RunnableSample, IRunnableSample
    {
        public bool Enabled => true;
        protected override Core.Samples Sample => Core.Samples.Basic_Databases;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Core.Databases.Persons);
        }

        public async Task Run()
        {
            await DatabaseOperations();
        }

        private async Task DatabaseOperations()
        {
            // Lists the databases on the server
            // Default schema for each db: {"name":"admin","sizeOnDisk":40960.0,"empty":false}

            var databases = await Client.ListDatabasesAsync();

            // iterate databases
            // Throws System.ObjectDisposedException: 'Cannot access a disposed object if ToList() or .Any() has been used
            // databases.ToList() returns a list containing all the documents returned by a cursor
            // databases.Any() determines whether the cursor contains any documents
            while (databases.MoveNext())
            {
                var currentBatch = databases.Current;
                Utils.Log(currentBatch.AsEnumerable(), "List databases");
            }

            var adminDatabase = (await Client.ListDatabasesAsync(new ListDatabasesOptions
            {
                Filter = Builders<BsonDocument>.Filter.Eq("name", "admin"),
                NameOnly = true
            })).FirstOrDefault();
            Utils.Log(adminDatabase);

            // Returns the names of the databases on the server.
            var databaseNames = (await Client.ListDatabaseNamesAsync()).ToList();

            // Shell commands
            // show dbs | show databases

            // Gets a database
            IMongoDatabase adminDb = Client.GetDatabase("admin");

            // Check if database exists
            var dbNameFilter = Builders<BsonDocument>.Filter.Eq("name", "fictionDb");
            var fictionDbExists = (await Client
                                      .ListDatabasesAsync(new ListDatabasesOptions() { Filter = dbNameFilter }))
                                      .FirstOrDefault() != null;
        }
    }
}
