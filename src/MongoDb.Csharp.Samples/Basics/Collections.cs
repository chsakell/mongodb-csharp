using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
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
            await usersDatabase.CreateCollectionAsync("logins");

            // list collections
            var collections = (await usersDatabase.ListCollectionsAsync()).ToList();
            Utils.Log(collections, "List Collections");

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

            await personsBsonCollection.InsertOneAsync(bsonUser);

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
