# Read documents

## Find a document

To search for a document in MongoDB you use the `Find` method on a `IMongoCollection<T>` reference. `Find` method accepts a `FilterDefinition<T>` parameter where `T` is the collection's type. 

{% tabs %}
{% tab title="Syntax" %}
```csharp
IMongoCollection<T>.Find(FilterDefinition<T> filter)
```
{% endtab %}
{% endtabs %}

Filters can be created using the `Builders<T>.Filter` definition builder which contain multiple filters. The following example finds a user document based on its Id. It does this using the **equality filter** `Eq<T>` on the id field.

{% tabs %}
{% tab title="C\#" %}
{% code title="ReadDocuments.cs" %}
```csharp
// Get the collection
var personsCollection = usersDatabase.GetCollection<User>("users");

// Create an Equality Filter Definition
var personFilter = Builders<User>.Filter
    .Eq(person => person.Id, appPerson.Id);

// Find the document in the collection    
var personFindResult = await personsCollection
    .Find(personFilter).FirstOrDefaultAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
// Get the collection
var bsonPersonCollection = usersDatabase.GetCollection<BsonDocument>("users");

// Create a bson filter
var bsonPersonFilter = Builders<BsonDocument>.Filter.Eq("_id", appPerson.Id);

// Find a person using a class filter
var bsonPersonFindResult = await bsonPersonCollection
    .Find(bsonPersonFilter).FirstOrDefaultAsync();

// alternative for passing a filter argument
bsonPersonFindResult = await bsonPersonCollection
.Find(new BsonDocument("_id", appPerson.Id)).FirstOrDefaultAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
// You need to know the Id's value and type

db.users.findOne(
{
    "_id": ObjectId("5e5d11fe152a428290f30245")
})
```
{% endtab %}

{% tab title="Document" %}
```javascript
{
	"_id" : ObjectId("5e5d11fe152a428290f30245"),
	"gender" : 1,
	"firstName" : "Joan",
	"lastName" : "Kovacek",
	"userName" : "Joan68",
	"avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/kohette/128.jpg",
	"email" : "Joan.Kovacek1@yahoo.com",
	"dateOfBirth" : ISODate("1961-02-04T21:38:05.235+02:00"),
	"address" : {
		"street" : "6266 Naomie Key",
		"suite" : "Suite 865",
		"city" : "East Brianton",
		"state" : "New York",
		"zipCode" : "72433",
		"geo" : {
			"lat" : 41.3698,
			"lng" : -57.0814
		}
	},
	"phone" : "123-456-789",
	"website" : "heidi.net",
	"company" : {
		"name" : "Hansen - Stiedemann",
		"catchPhrase" : "Versatile uniform orchestration",
		"bs" : "expedite proactive markets"
	},
	"salary" : 4000,
	"monthlyExpenses" : 3132,
	"favoriteSports" : [
		"Table Tennis",
		"Handball",
		"Water Polo",
		"Tennis",
		"Baseball",
		"Cricket",
		"Cycling"
	],
	"profession" : "Lawyer"
}
```
{% endtab %}
{% endtabs %}

{% hint style="warning" %}
 Notice that when filter used with `BsonDocument`, _**\_id**_  field name used instead of _**Id**_ which is the property name on the `User` class. When you use `Builders<User>.Filter` this is done automatically for you by the driver, based on the _Id_ serialization settings
{% endhint %}

## Find multiple documents

To search for multiple documents follow the same process but this time use the `ToList` method. The following example finds all documents with female gender.

{% tabs %}
{% tab title="C\#" %}
{% code title="ReadDocuments.cs" %}
```csharp
// Get the collection
var personsCollection = usersDatabase.GetCollection<User>("users");

// Create an equality filter on a user's gender
var femaleGenderFilter = Builders<User>.Filter
    .Eq(person => person.Gender, Gender.Female);
    
// Find all documents having female gender
var females = await personsCollection
    .Find(femaleGenderFilter).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
// Get the collection
var bsonPersonCollection = usersDatabase.GetCollection<BsonDocument>("users");

// Create an equality filter on gender
var bsonFemaleGenderFilter = Builders<BsonDocument>
            .Filter.Eq("gender", Gender.Female);

// Get all female users
var bsonFemales = await bsonPersonCollection
            .Find(bsonFemaleGenderFilter).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.find(
{
   "gender": 1
})

----------------

// sample document
{
	"_id" : ObjectId("5e8a35e2cc20587f34f0cc2a"),
	"gender" : 1,
	"firstName" : "Ruth",
	"lastName" : "Bruen",
	"userName" : "Ruth51",
	"avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/polarity/128.jpg",
	"email" : "Ruth_Bruen44@hotmail.com",
	"dateOfBirth" : ISODate("1955-06-24T20:34:26.809+02:00"),
	"address" : {
		"street" : "9217 O'Kon Alley",
		"suite" : "Apt. 111",
		"city" : "Lake Nicklaus",
		"state" : "Indiana",
		"zipCode" : "12931",
		"geo" : {
			"lat" : 60.0623,
			"lng" : 175.3924
		}
	},
	"phone" : "(358) 955-3171 x931",
	"website" : "linda.net",
	"company" : {
		"name" : "Cummerata, Kilback and Bashirian",
		"catchPhrase" : "Seamless user-facing success",
		"bs" : "orchestrate transparent infrastructures"
	},
	"salary" : 4000,
	"monthlyExpenses" : 3633,
	"favoriteSports" : [
		"Moto GP",
		"American Football",
		"Snooker",
		"Motor Sport",
		"Baseball"
	],
	"profession" : "Marketing Manager"
},
{
	"_id" : ObjectId("5e8a35e2cc20587f34f0cc2c"),
	"gender" : 1,
	"firstName" : "Sue",
	"lastName" : "Muller",
	"userName" : "Sue0",
	"avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/vm_f/128.jpg",
	"email" : "Sue58@yahoo.com",
	"dateOfBirth" : ISODate("1965-11-08T19:19:54.681+02:00"),
	"address" : {
		"street" : "090 Diego Vista",
		"suite" : "Suite 562",
		"city" : "South Rexton",
		"state" : "Kentucky",
		"zipCode" : "62445",
		"geo" : {
			"lat" : -19.2521,
			"lng" : -129.7291
		}
	},
	"phone" : "1-887-789-2466",
	"website" : "lorenzo.name",
	"company" : {
		"name" : "Parker - Wilderman",
		"catchPhrase" : "Function-based executive pricing structure",
		"bs" : "iterate cutting-edge technologies"
	},
	"salary" : 4000,
	"monthlyExpenses" : 1601,
	"favoriteSports" : [
		"Basketball",
		"Table Tennis",
		"Cricket",
		"MMA",
		"Moto GP",
		"Golf",
		"Ice Hockey",
		"Beach Volleyball"
	],
	"profession" : "Pilot"
}
```
{% endtab %}
{% endtabs %}



