# Insert documents

## Insert one document

You can insert a document using the `InsertOne` method on a `IMongoCollection<T>` reference.&#x20;

{% tabs %}
{% tab title="Syntax" %}
```csharp
IMongoCollection<T>.InsertOne(<document>)
```
{% endtab %}
{% endtabs %}

Depending on the collection type you can pass either your own class type or a `BsonDocument`. You can build the `BsonDocument` either manually or using the `BsonDocument.Parse` method.

The sample insert a `Use`r document in collection.

{% tabs %}
{% tab title="C#" %}
{% code title="InsertDocuments.cs" %}
```csharp
var database = Client
    .GetDatabase(Constants.SamplesDatabase);

var personsCollection = database
    .GetCollection<User>(Constants.UsersCollection);

User appPerson = RandomData.GenerateUsers(1).First();

// Insert one document
await personsCollection.InsertOneAsync(appPerson);
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
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

{% tab title="Parse" %}
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

-----------------------------

// sample result
{
	"acknowledged" : true,
	"insertedId" : ObjectId("5e8a39059c819d22e031c4f4")
}
```
{% endtab %}
{% endtabs %}

{% hint style="danger" %}
&#x20;Notice how overwhelming querying using `BsonDocument` can be. And it's not only that you have to carefully type all these in curly brackets, it is also dangerous that you might end up having wrong **type of data** in the database because MongoDB will use default data types for values that their type haven't explicitly defined.&#x20;

> :no\_entry: **Building the documents manually or using the `Parse` method is not recommended**

Luckily, there is the **`ToBsonDocument`** helper method that builds the `BsonDocument` from your typed class automatically and saves you from all the trouble.

```csharp
var personsBsonCollection = database
    .GetCollection<BsonDocument>(Constants.UsersCollection);

User appPerson = RandomData
    .GenerateUsers(1).First();

await personsBsonCollection
    .InsertOneAsync(appPerson.ToBsonDocument());
```
{% endhint %}

## Insert many documents

To add multiple documents at once, you can use the `InsertMany` collection method, passing the array of items to be inserted in the collection.&#x20;

{% hint style="success" %}
Prefer this method when you need to add multiple documents, instead looping the array and calling the `InsertOne` method. `InsertMany` is more efficient since you avoid making round trips to the database
{% endhint %}

{% tabs %}
{% tab title="Syntax" %}
```csharp
IMongoCollection<T>
    .InsertMany(IEnumerable<T> documents)
```
{% endtab %}
{% endtabs %}

The sample inserts 10 `User` documents in the collection.

{% tabs %}
{% tab title="C#" %}
{% code title="InsertDocuments.cs" %}
```csharp
// generate 10 users
var persons = RandomData.GenerateUsers(10);

// Insert multiple documents
await personsCollection.InsertManyAsync(persons);
```
{% endcode %}
{% endtab %}

{% tab title="Result" %}
```javascript
{
	"acknowledged" : true,
	"insertedIds" : [
		ObjectId("5e8a393f9c819d22e031c4f5"),
		ObjectId("5e8a393f9c819d22e031c4f6")
	]
}

// sample document

{
    "_id" : ObjectId("5ea4038a4e1a72d3eb7b7376"),
    "gender" : 1,
    "firstName" : "Hope",
    "lastName" : "O'Hara",
    "userName" : "Hope.OHara",
    "avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/goddardlewis/128.jpg",
    "email" : "Hope.OHara41@yahoo.com",
    "dateOfBirth" : ISODate("1978-09-17T08:18:40.694Z"),
    "address" : {
        "street" : "8347 Lesly Pines",
        "suite" : "Apt. 924",
        "city" : "Brownberg",
        "state" : "Washington",
        "zipCode" : "78970",
        "geo" : {
            "lat" : -14.8666,
            "lng" : 52.3172
        }
    },
    "phone" : "1-584-453-1767 x134",
    "website" : "dashawn.biz",
    "company" : {
        "name" : "Kemmer - Denesik",
        "catchPhrase" : "Re-engineered even-keeled workforce",
        "bs" : "revolutionize one-to-one supply-chains"
    },
    "salary" : NumberDecimal("3865"),
    "monthlyExpenses" : 5644,
    "favoriteSports" : [
        "Boxing",
        "Cycling",
        "Table Tennis",
        "Volleyball",
        "Handball",
        "MMA",
        "Water Polo",
        "Baseball",
        "Golf",
        "Ice Hockey",
        "Cricket",
        "Soccer",
        "Basketball",
        "Snooker",
        "American Football",
        "Beach Volleyball"
    ],
    "profession" : "Personal Trainer"
}
```
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
{% endtabs %}

{% hint style="success" %}
Notice that insert operations return the **inserted** _Id(s_) as part of the result. This way, the driver automatically updates the _Id_ field on the argument(s) passed on the update operation
{% endhint %}
