using System.Linq;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.GettingStarted
{
    public class CreateDatabaseAndCollections  : RunnableSample, IRunnableSample
    {
        public bool Enabled => true;
        protected override Core.Samples Sample => Core.Samples.CreateDatabaseAndCollections;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Users);
        }

        public void Run()
        {
            DatabaseOperations();
        }

        private void DatabaseOperations()
        {
            // Lists the databases on the server
            // Default schema for each db: {"name":"admin","sizeOnDisk":40960.0,"empty":false}

            var databases = Client.ListDatabases();

            // iterate databases
            // Throws System.ObjectDisposedException: 'Cannot access a disposed object if ToList() or .Any() has been used
            // databases.ToList() returns a list containing all the documents returned by a cursor
            // databases.Any() determines whether the cursor contains any documents
            while (databases.MoveNext())
            {
                var currentBatch = databases.Current;
                Utils.Log(currentBatch.AsEnumerable());
            }

            var adminDatabase = Client.ListDatabases(new ListDatabasesOptions
            {
                Filter = Builders<BsonDocument>.Filter.Eq("name", "admin"),
                NameOnly = true
            }).FirstOrDefault();
            Utils.Log(adminDatabase);

            // Returns the names of the databases on the server.
            var databaseNames = Client.ListDatabaseNames().ToList();

            // Shell commands
            // show dbs | show databases

            // Gets a database
            IMongoDatabase adminDb = Client.GetDatabase("admin");

            // Check if database exists
            var dbNameFilter = Builders<BsonDocument>.Filter.Eq("name", "fictionDb");
            var fictionDbExists = Client
                                      .ListDatabases(new ListDatabasesOptions() {Filter = dbNameFilter})
                                      .FirstOrDefault() != null;
        }

        private void CollectionOperations()
        {
            var usersDatabase = Client.GetDatabase(Databases.Users);
            var usersDataCollection = usersDatabase.GetCollection<BsonDocument>("logins");

            // insert some documents in the collection - this will also create the database on the fly
            for (var i = 0; i < 5; i++)
            {
                var userLogin = new BsonDocument("name", "");
            }

        }
    }
}
