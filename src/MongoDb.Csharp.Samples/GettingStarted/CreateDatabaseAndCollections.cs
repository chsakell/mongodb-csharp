using System.Linq;
using Bogus;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using Utils = MongoDb.Csharp.Samples.Core.Utils;

namespace MongoDb.Csharp.Samples.GettingStarted
{
    public class CreateDatabaseAndCollections : RunnableSample, IRunnableSample
    {
        public bool Enabled => true;
        protected override Core.Samples Sample => Core.Samples.CreateDatabaseAndCollections;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Persons);
        }

        public void Run()
        {
            DatabaseOperations();
            CollectionOperations();
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
                                      .ListDatabases(new ListDatabasesOptions() { Filter = dbNameFilter })
                                      .FirstOrDefault() != null;
        }

        private void CollectionOperations()
        {
            var usersDatabase = Client.GetDatabase(Databases.Persons);

            #region Typed classes commands

            var personsTypedCollection = usersDatabase.GetCollection<AppPerson>("users");

            AppPerson typedUser = RandomData.GeneratePerson();
            personsTypedCollection.InsertOne(typedUser);

            #endregion

            #region BsonDocument commands

            var personsBsonCollection = usersDatabase.GetCollection<BsonDocument>("users");
            var bsonUser = BsonDocument.Parse(@"{
                'firstName': 'Lee',
                'lastName': 'Brown',
                'userName': 'Lee_Brown3',
                'avatar': 'https://s3.amazonaws.com/uifaces/faces/twitter/ccinojasso1/128.jpg',
                'email': 'Lee_Brown369@yahoo.com',
                'dateOfBirth': '1984-01-16T21:31:27.87666',
                'address': {
                  'street': '2552 Bernard Rapid',
                  'suite': 'Suite 199',
                  'city': 'New Haskell side',
                  'zipCode': '78425-0411',
                  'geo': {
                    'lat': -35.8154,
                    'lng': -140.2044
                  }
                },
                'phone': '1-500-790-8836 x5069',
                'website': 'javier.biz',
                'company': {
                  'name': 'Kuphal and Sons',
                  'catchPhrase': 'Organic even-keeled monitoring',
                  'ns': 'open-source brand e-business'
                }
             }");

            personsBsonCollection.InsertOne(bsonUser);

            #endregion

            #region Shell commands

            /*
            use Persons
            db.users.insertOne({
                'firstName': 'Lee',
                'lastName': 'Brown',
                'userName': 'Lee_Brown3',
                'avatar': 'https://s3.amazonaws.com/uifaces/faces/twitter/ccinojasso1/128.jpg',
                'email': 'Lee_Brown369@yahoo.com',
                'dateOfBirth': '1984-01-16T21:31:27.87666',
                'address': {
                    'street': '2552 Bernard Rapid',
                    'suite': 'Suite 199',
                    'city': 'New Haskell side',
                    'zipCode': '78425-0411',
                    'geo': {
                        'lat': -35.8154,
                        'lng': -140.2044
                    }
                },
                'phone': '1-500-790-8836 x5069',
                'website': 'javier.biz',
                'company': {
                    'name': 'Kuphal and Sons',
                    'catchPhrase': 'Organic even-keeled monitoring',
                    'ns': 'open-source brand e-business'
                }
            })
            */

            #endregion
        }
    }
}
