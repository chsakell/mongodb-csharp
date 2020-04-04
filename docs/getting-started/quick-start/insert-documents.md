# Insert documents

## Insert one

You can insert a document using the `InsertOne` method on a collection reference. Depending on the collection type you can pass either your own class type or a `BsonDocument`. You can build the `BsonDocument` either manually or using the `BsonDocument.Parse` method.

{% tabs %}
{% tab title="Typed" %}
```csharp
var personsCollection = usersDatabase.GetCollection<User>("users");

User appPerson = RandomData.GenerateUsers(1).First();

// Insert one document
await personsCollection.InsertOneAsync(appPerson);
```
{% endtab %}

{% tab title="BsonDocument" %}
```csharp
var personsBsonCollection = usersDatabase.GetCollection<BsonDocument>("users");

var bsonPerson = new BsonDocument
{
    { "gender" , 1 },
    { "firstName" , "Johh" },
    { "lastName" , "Doe"},
    { "userName" , "Kimberly12"},
    { "avatar" , "https://api.adorable.io/avatars/285/abott@adorable.png" },
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
```
{% endtab %}

{% tab title="BsonDocument.Parse" %}
```csharp
var personsBsonCollection = usersDatabase.GetCollection<BsonDocument>("users");
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
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.insertOne({
	"gender" : 1,
	"firstName" : "Johh",
	"lastName" : "Doe",
	"userName" : "Kimberly12",
	"avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/benefritz/128.jpg",
	"email" : "Kimberly29@hotmail.com",
	"dateOfBirth" : ISODate("1995-04-04T22:13:06.020+03:00"),
	"address" : {
		"street" : "113 Al Points",
		"suite" : "Apt. 697",
		"city" : "South Murrayshire",
		"state" : "South Dakota",
		"zipCode" : "35038",
		"geo" : {
			"lat" : 87.333,
			"lng" : -116.99
		}
	},
	"phone" : "392-248-7338 x083",
	"website" : "terry.biz",
	"company" : {
		"name" : "Bode - Hills",
		"catchPhrase" : "Total composite service-desk",
		"bs" : "morph customized bandwidth"
	},
	"salary" : 1641,
	"monthlyExpenses" : 3009,
	"favoriteSports" : [
		"Basketball",
		"MMA"
	],
	"profession" : "Doctor"
})
```
{% endtab %}
{% endtabs %}

{% hint style="danger" %}
 Notice how overwhelming querying using `BsonDocument` can be. And it's not only that you have to carefully type all these in curly brackets, it is also dangerous that you might end up having wrong **type of data** in the database because MongoDB will use default data types for values that their type haven't explicitly defined. 
{% endhint %}

## Insert many

To add multiple documents at once, you can use the `InsertMany` collection method.

{% code title="InsertDocuments.cs" %}
```csharp
// generate 10 users
var persons = RandomData.GenerateUsers(10);

// Insert multiple documents
await personsCollection.InsertManyAsync(persons);
```
{% endcode %}

## The Id member

