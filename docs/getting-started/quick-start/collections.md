# Collections

## Get a collection reference

In order to read/write data from/to a database collection, first you need to get a reference to that collection. The collection doesn't have to exist already and if it doesn't, the very first time you run a read or write operation, it will be created automatically for you.

{% tabs %}
{% tab title="C\#" %}
{% code title="AccessCollections.cs" %}
```csharp
// Get a reference to the database
var usersDatabase = Client.GetDatabase(Databases.Persons);

// Get a reference to a database's collection named 'users'
var personsTypedCollection = usersDatabase.GetCollection<User>("users");
```
{% endcode %}
{% endtab %}

{% tab title="User" %}
```csharp
public class User
    {
        [BsonId]
        [BsonIgnoreIfDefault] // required for replace documents 
        public ObjectId Id { get; set; }
        public Gender Gender { get; set; }
        public string FirstName {get; set; }
        public string LastName {get; set; }
        public string UserName {get; set; }
        public string Avatar {get; set; }
        public string Email {get; set; }
        public DateTime DateOfBirth {get; set; }
        public AddressCard Address {get; set; }
        public string Phone {get; set; }
        
        [BsonIgnoreIfDefault]
        public string Website {get; set; }
        public CompanyCard Company {get; set; }
        public decimal Salary { get; set; }
        public int MonthlyExpenses { get; set; }
        public List<string> FavoriteSports { get; set; }
        public string Profession { get; set; }
    }
```
{% endtab %}

{% tab title="Sample database document" %}
```javascript
{
	"_id" : ObjectId("5e8897d94d749f71d8240a3c"),
	"gender" : 1,
	"firstName" : "Jacqueline",
	"lastName" : "Dickens",
	"userName" : "Jacqueline.Dickens",
	"avatar" : "https://api.adorable.io/avatars/285/abott@adorable.png",
	"email" : "Jacqueline99@gmail.com",
	"dateOfBirth" : ISODate("2000-01-22T09:52:09.032+02:00"),
	"address" : {
		"street" : "65603 Wyatt Roads",
		"suite" : "Suite 220",
		"city" : "South Bernardo",
		"state" : "Kansas",
		"zipCode" : "54687-9947",
		"geo" : {
			"lat" : -2.4298,
			"lng" : 179.5312
		}
	},
	"phone" : "874.533.9433 x9618",
	"website" : "madison.info",
	"company" : {
		"name" : "Lebsack - Robel",
		"catchPhrase" : "Innovative mission-critical local area network",
		"bs" : "engage integrated markets"
	},
	"salary" : 4218,
	"monthlyExpenses" : 2628,
	"favoriteSports" : [
		"American Football",
		"Basketball",
		"Soccer",
		"Baseball",
		"Tennis",
		"Moto GP",
		"Table Tennis",
		"Motor Sport",
		"Beach Volleyball",
		"Snooker",
		"Ice Hockey",
		"Cricket",
		"Volleyball",
		"Cycling",
		"MMA",
		"Water Polo"
	],
	"profession" : "Photographer"
}
```
{% endtab %}
{% endtabs %}

Notice that in the above sample the collection reference is a **typed** collection of type `IMongoCollection<User>` and most of its methods require parameters of type `User`. It basically implies that a document stored in the collection will match and can be deserialized by the driver back to a `User` entity.

This is different than getting a reference collection of type `IMongoCollection<BsonDocument>`:

```csharp
var bsonPersonsCollection = usersDatabase.GetCollection<BsonDocument>("users");
```

{% hint style="success" %}
Using a `IMongoCollection<BsonDocument>`collection reference,  you can build pretty much all MongoDB queries you write in the shell, even the complex ones. The downside though is that you end up writing ugly code that is hard to maintain when models change.  Luckily, these docs' goal is to learn you how to build **typed** MongoDB queries
{% endhint %}

## Create collections

You can create a collection using the `CreateCollection` method on a `IMongoDatabase` reference.

{% tabs %}
{% tab title="C\#" %}
{% code title="AccessCollections.cs" %}
```csharp
var loginsCollectionName = "logins";
await usersDatabase.CreateCollectionAsync(loginsCollectionName);
```
{% endcode %}
{% endtab %}

{% tab title="Shell" %}
```
db.createCollection("myCollection")
```
{% endtab %}
{% endtabs %}

### Create a Capped collection

A Capped collection in MongoDB is a special **fixed-size** collection where when its allocated sized is filled, the oldest documents inserted are automatically removed to make room for the new ones. You can define a collection as capped only during its creation by using the `CreateCollectionOptions` and setting the values for either \(_or both_\) the **max documents** and the **max size**. If any max documents or max size exceeds, the latest document will be removed to make room for the next one inserted.

{% hint style="info" %}
Remember that in case you set the `MaxDocuments` you are required to set the `MaxSize` as well
{% endhint %}

{% tabs %}
{% tab title="C\#" %}
{% code title="AccessCollections.cs" %}
```csharp
var travelersCollectionName = "travelers";
await tripsDatabase
    .CreateCollectionAsync(travelersCollectionName,
        new CreateCollectionOptions()
        {
            Capped = true, MaxDocuments = 3, MaxSize = 10000
        });
        var travelers = RandomData.GenerateTravelers(3);
```
{% endcode %}
{% endtab %}

{% tab title="Example" %}
```csharp
travelers.First().Name = "Christos";

var travelersCollection = tripsDatabase
    .GetCollection<Traveler>(travelersCollectionName);

await travelersCollection.InsertManyAsync(travelers);

// Max documents reached - Now let's insert another one
await travelersCollection.InsertManyAsync(RandomData.GenerateTravelers(1));

// First document have been removed to fill room for the new one
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.createCollection("myCollection", { 
    capped : true, 
    size : 50 * 1024 * 1024, 
    max : 100 * 1000 
})
```
{% endtab %}
{% endtabs %}

{% hint style="info" %}
#### Why a capped collection anyway?

* Order retrieved is always same as the order inserted. This is not guaranteed with a normal collection
* Automatic clear up ensures fixed collection size and hence efficient to work with
* Ideal for scenarios where you want high insertion throughput while you don't care or want to get rid of old data

[Reference](https://docs.mongodb.com/manual/core/capped-collections/)
{% endhint %}

## List collections

You can get all available collections in a database by using the `ListCollections` method on an `IMongoDatabase` reference.

{% tabs %}
{% tab title="C\#" %}
{% code title="AccessCollections.cs" %}
```csharp
var usersDatabase = Client.GetDatabase(Databases.Persons);

// Get all collections
var collections = (await usersDatabase.ListCollectionsAsync()).ToList();
```
{% endcode %}
{% endtab %}

{% tab title="Collection properties" %}
```javascript
{
  "name": "users",
  "type": "collection",
  "options": {},
  "info": {
    "readOnly": false,
    "uuid": "fb0d121f-6113-4e43-95c6-065948b6c9af"
  },
  "idIndex": {
    "v": 2,
    "key": {
      "_id": 1
    },
    "name": "_id_",
    "ns": "Persons.users"
  }
}
```
{% endtab %}
{% endtabs %}

## Drop collections

Delete a collection using the `DropCollection` method. The collection along with its data will be entirely removed.

```csharp
var loginsCollectionName = "logins";
await usersDatabase.DropCollectionAsync(loginsCollectionName);
```



