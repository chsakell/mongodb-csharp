using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using Utils = MongoDb.Csharp.Samples.Core.Utils;

namespace MongoDb.Csharp.Samples.QuickStart
{
    public class InsertDocuments : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.QuickStart_InsertDocuments;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Constants.SamplesDatabase);
        }

        public async Task Run()
        {
            await InsertSamples();
        }
        private async Task InsertSamples()
        {
            var database = Client.GetDatabase(Constants.SamplesDatabase);

            #region Typed classes commands

            // Will create the users collection on the fly if it doesn't exists
            var personsCollection = database.GetCollection<User>(Constants.UsersCollection);

            User appPerson = RandomData.GenerateUsers(1).First();
            // Insert one document
            await personsCollection.InsertOneAsync(appPerson);

            // Insert multiple documents
            var persons = RandomData.GenerateUsers(10);

            await personsCollection.InsertManyAsync(persons);

            #endregion

            #region BsonDocument commands

            var personsBsonCollection = database.GetCollection<BsonDocument>(Constants.UsersCollection);

            var bsonPerson = new BsonDocument
            {
                { "gender" , 1 },
                {"firstName" , "Johh" },
                { "lastName" , "Doe"},
                { "userName" , "Kimberly12"},
                { "avatar" , "https://s3.amazonaws.com/uifaces/faces/twitter/benefritz/128.jpg" },
                { "email" , "Kimberly29@hotmail.com"},
                { "dateOfBirth" , new BsonDateTime(DateTime.Now.AddYears(-25)) },
                { "address", new BsonDocument
                    {
                        {"street" , "113 Al Points" },
                        {"suite" , "Apt. 697" },
                        {"city" , "South Murrayshire" },
                        {"state" , "South Dakota" },
                        {"zipCode" , "35038" },
                        {
                            "geo", new BsonDocument()
                            {
                                { "lat", 87.333 },
                                { "lng", -116.99 }
                            }
                        }
                    }
                },
                { "phone" , "392-248-7338 x083" },
                { "website" , "terry.biz" },
                {
                    "company" , new BsonDocument()
                    {
                        {"name" , "Bode - Hills" },
                        {"catchPhrase" , "Total composite service-desk" },
                        {"bs" , "morph customized bandwidth"}
                    }
                },
                { "salary" , 1641 },
                { "monthlyExpenses" , 3009 },
                { "favoriteSports", new BsonArray{ "Basketball", "MMA" } },
                { "profession", "Doctor" }
            };

            await personsBsonCollection.InsertOneAsync(bsonPerson);

            var bsonUser = BsonDocument.Parse(@"{
	            'gender' : 1,
	            'firstName' : 'Kimberly',
	            'lastName' : 'Ernser',
	            'userName' : 'Kimberly12',
	            'avatar' : 'https://s3.amazonaws.com/uifaces/faces/twitter/benefritz/128.jpg',
	            'email' : 'Kimberly29@hotmail.com',
	            'dateOfBirth' : ISODate('1996-06-10T23:55:56.029+03:00'),
	            'address' : {
		            'street' : '113 Al Points',
		            'suite' : 'Apt. 697',
		            'city' : 'South Murrayshire',
		            'state' : 'South Dakota',
		            'zipCode' : '35038',
		            'geo' : {
			            'lat' : 87.4034,
			            'lng' : -116.5628
		            }
	            },
	            'phone' : '392-248-7338 x083',
	            'website' : 'terry.biz',
	            'company' : {
		            'name' : 'Bode - Hills',
		            'catchPhrase' : 'Total composite service-desk',
		            'bs' : 'morph customized bandwidth'
	            },
	            'salary' : 1641,
	            'monthlyExpenses' : 3009,
	            'favoriteSports' : [
		            'Basketball',
		            'MMA',
		            'Volleyball',
		            'Ice Hockey',
		            'Water Polo',
		            'Moto GP',
		            'Beach Volleyball'
	            ],
	            'profession' : 'Photographer'
            }");

            await personsBsonCollection.InsertOneAsync(bsonUser);

            await personsBsonCollection.InsertOneAsync(appPerson.ToBsonDocument());

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
                },
                'salary': NumberDecimal(3000)
            })
            */

            #endregion
        }
    }
}
