# Replace

## _ReplaceOne_

You can replace a single document entirely by using the `ReplaceOne` method on an `IMongoCollection<T>`.

{% tabs %}
{% tab title="Syntax" %}
```csharp
IMongoCollection<T>
    .ReplaceOne(FilterDefinition<T> filter, T document)
```
{% endtab %}
{% endtabs %}

The sample replaces the first document with a new one.

{% tabs %}
{% tab title="C\#" %}
{% code title="ReplaceDocuments.cs" %}
```csharp
var collection = database
    .GetCollection<User>(Constants.UsersCollection);

// this is the new document - demo only
var newUser = RandomData.GenerateUsers(1).First();

// replace the first document in the collection
var replaceOneResult = await collection
    .ReplaceOneAsync(Builders<User>.Filter.Empty, newUser);
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database
            .GetCollection<BsonDocument>(Constants.UsersCollection);

// this is the new document - demo only
var newUser = RandomData.GenerateUsers(1).First();

var bsonReplaceOneResult = await bsonCollection
    .ReplaceOneAsync(new BsonDocument(), 
                newUser.ToBsonDocument());
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.replaceOne({},
{
	"gender" : 1,
	"firstName" : "Chris",
	"lastName" : "Sakellarios",
	"userName" : "Elsie.VonRueden72",
	"avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/miguelmendes/128.jpg",
	"email" : "Elsie.VonRueden@yahoo.com",
	"dateOfBirth" : ISODate("1965-08-26T15:55:31.907+02:00"),
	"address" : {
		"street" : "8902 Baumbach Burg",
		"suite" : "Apt. 717",
		"city" : "West Carlieton",
		"state" : "South Carolina",
		"zipCode" : "85642-3703",
		"geo" : {
			"lat" : -69.6681,
			"lng" : -116.3583
		}
	},
	"phone" : "(222) 443-5341 x35825",
	"website" : "https://github.com/chsakell",
	"company" : {
		"name" : "Abshire Inc",
		"catchPhrase" : "Function-based mission-critical budgetary management",
		"bs" : "monetize holistic eyeballs"
	},
	"salary" : 2482,
	"monthlyExpenses" : 2959,
	"favoriteSports" : [
		"Basketball",
		"Baseball",
		"Table Tennis",
		"Ice Hockey",
		"Handball",
		"Formula 1",
		"American Football"
	],
	"profession" : "Model"
})

--------------------------- 
        
// sample update result
{
	"acknowledged" : true,
	"matchedCount" : 1,
	"modifiedCount" : 1
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

{% hint style="warning" %}
The identifier field **\_**_**id**_  on the new document must fulfill one of the following conditions:

* If specified, **must be equal** to the current document's value
* Not specified at all. MongoDB will create a new one automatically

⚠ In case you do set a value for the identifier \_id field and it's different than the current one, you will get the exception:**`After applying the update, the (immutable) field '_id' was found to have been altered to _id: <new-id>`**
{% endhint %}

One common scenario where you need to replace documents is when you want to **move fields** inside the documents. Consider that you have the following document.

```javascript
{
        "_id" : ObjectId("5ea55f572109b8cadfc146e9"),
        "username" : "chsakell",
        "friends" : [
                "John",
                "Maria",
                "Catherine"
        ],
        "blocked" : [
                "Jake",
                "Helen"
        ]
}
```

The following sample moves _friends_ and _blocked_ top level fields in a new embedded document field named _relationships._

{% tabs %}
{% tab title="C\#" %}
{% code title="ReplaceDocuments.cs" %}
```csharp
var socialAccountAfter = new SocialAccount
{
    Username = username,
    RelationShips = new RelationShips
    {
        Friends = friends,
        Blocked = blocked
    }
};

await socialNetworkCollection
    .ReplaceOneAsync(Builders<SocialAccount>.Filter
        .Eq(ac => ac.Username, username), socialAccountAfter);
        
```
{% endcode %}
{% endtab %}

{% tab title="Result" %}
```javascript
{
        "_id" : ObjectId("5ea55f572109b8cadfc146e9"),
        "username" : "chsakell",
        "relationShips" : {
                "friends" : [
                        "John",
                        "Maria",
                        "Catherine"
                ],
                "blocked" : [
                        "Jake",
                        "Helen"
                ]
        }
```
{% endtab %}

{% tab title="Models" %}
```csharp
public class SocialAccount
{
    [BsonIgnoreIfDefault]
    public ObjectId Id { get; set; }
    public string Username { get; set; }
    public RelationShips RelationShips { get; set; }
}

public class RelationShips
{
    public List<string> Friends { get; set; }
    public List<string> Blocked { get; set; }
}
```
{% endtab %}
{% endtabs %}

{% hint style="info" %}
When replacing a document make sure to be precise 🎯 on your filter, otherwise you might get a **`E11001 duplicate key on update`** error. For example, in the previous sample if the _socialAccountAfter_ object already contained the **\_**_**id**_ value and there were more than one documents with username _"chsakell"_ you would get the error. Why? Because MongoDB would try to set in more than one documents the same _**\_id**_ unique identifier value.
{% endhint %}

## Upsert

If the filter in the `ReplaceOne` operation fails to match a document then nothing happens in the database. You can change this behavior by passing a _replace options_ argument to the replace operation and setting `upsert = true`. 

{% tabs %}
{% tab title="Syntax" %}
```csharp
IMongoCollection<T>
    .ReplaceOne(FilterDefinition<T> filter, 
                T document, 
                ReplaceOptions options)
```
{% endtab %}
{% endtabs %}

The sample tries to replace a user document that has company name "_Microsoft Corp_". If it finds a match  then it will replace it with the _microsoftCeo_ document but if it doesn't, it will insert it.

{% tabs %}
{% tab title="C\#" %}
{% code title="ReplaceDocuments.cs" %}
```csharp
var collection = database
    .GetCollection<User>(Constants.UsersCollection);

// this is the new document - demo only
var microsoftCeo = RandomData.GenerateUsers(1).First();
            microsoftCeo.FirstName = "Satya";
            microsoftCeo.LastName = "Nadella";
            microsoftCeo.Company.Name = "Microsoft";

// replace the first document in the collection
var addOrReplaceSatyaNadellaResult = await collection
    .ReplaceOneAsync<User>(u => u.Company.Name == "Microsoft Corp",
        microsoftCeo, new ReplaceOptions()
        {
            IsUpsert = true
        });
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database
      .GetCollection<BsonDocument>(Constants.UsersCollection);

// this is the new document - demo only
// this is the new document - demo only
var microsoftCeo = RandomData.GenerateUsers(1).First();
            microsoftCeo.FirstName = "Satya";
            microsoftCeo.LastName = "Nadella";
            microsoftCeo.Company.Name = "Microsoft";

var bsonAddOrReplaceSatyaNadellaUser = await bsonCollection
    .FindOneAndReplaceAsync(
        Builders<BsonDocument>.Filter.Eq("company.name", "Microsoft Corp"), 
            microsoftCeo.ToBsonDocument(), 
        new FindOneAndReplaceOptions<BsonDocument>()
    {
        IsUpsert = true,
        ReturnDocument = ReturnDocument.After
    });
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.replaceOne({ "company.name": "Microsoft Corp"},
{
	"gender" : 1,
	"firstName" : "Chris",
	"lastName" : "Sakellarios",
	"userName" : "Elsie.VonRueden72",
	"avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/miguelmendes/128.jpg",
	"email" : "Elsie.VonRueden@yahoo.com",
	"dateOfBirth" : ISODate("1965-08-26T15:55:31.907+02:00"),
	"address" : {
		"street" : "8902 Baumbach Burg",
		"suite" : "Apt. 717",
		"city" : "West Carlieton",
		"state" : "South Carolina",
		"zipCode" : "85642-3703",
		"geo" : {
			"lat" : -69.6681,
			"lng" : -116.3583
		}
	},
	"phone" : "(222) 443-5341 x35825",
	"website" : "https://github.com/chsakell",
	"company" : {
		"name" : "Abshire Inc",
		"catchPhrase" : "Function-based mission-critical budgetary management",
		"bs" : "monetize holistic eyeballs"
	},
	"salary" : 2482,
	"monthlyExpenses" : 2959,
	"favoriteSports" : [
		"Basketball",
		"Baseball",
		"Table Tennis",
		"Ice Hockey",
		"Handball",
		"Formula 1",
		"American Football"
	],
	"profession" : "Model"
}, { upsert: true })

--------------------------- 
        
// sample update result
{
	"acknowledged" : true,
	"matchedCount" : 0,
	"modifiedCount" : 0,
	"upsertedId" : ObjectId("5e99f20346296441706dfb4d")
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

{% hint style="info" %}
When no match found, the update result will be the following:

```javascript
{
	"acknowledged" : true,
	"matchedCount" : 0, // no match found
	"modifiedCount" : 0, // so nothing modified
	"upsertedId" : ObjectId("5e99f20346296441706dfb4d")
}
```

🦉 Since you set **`upsert = true`** a new document inserted with _\_id_ equal to the _upsertedId_ 
{% endhint %}

## _FindOneAndReplaceOne_

`IMongoCollection<T>` contains a `FindOneAndReplaceOne` __method that behaves exactly the same as the `ReplaceOne` except that the returned result is of type `T` instead of a `ReplaceOneResult`, in other words it returns the updated or _upserted_ document itself. This can be quite convenient when you want to keep working with the new document after replacing it. 

{% tabs %}
{% tab title="Syntax" %}
```csharp
IMongoCollection<T>
    .FindOneReplaceOne(FilterDefinition<T> filter, 
                T document, 
                FindOneAndReplaceOptions options)
```
{% endtab %}
{% endtabs %}

The sample replaces the first document with a new one and gets back the entire document.

{% tabs %}
{% tab title="C\#" %}
{% code title="ReplaceDocuments.cs" %}
```csharp
var collection = database
  .GetCollection<User>(Constants.UsersCollection);

var firstDbUser = await collection.
            Find(Builders<User>.Filter.Empty)
            .FirstOrDefaultAsync();
            
// this is the new document - demo only
var newUser = RandomData.GenerateUsers(1).First();
newUser.Id = firstDbUser.Id;
newUser.FirstName = "Chris";
newUser.LastName = "Sakellarios";
newUser.Website = "https://github.com/chsakell";

// replace the first document in the collection
var firstUser = await collection
.FindOneAndReplaceAsync(
    Builders<User>.Filter.Eq(u => u.Id, firstDbUser.Id), 
    newUser,
    new FindOneAndReplaceOptions<User> 
                {ReturnDocument = ReturnDocument.After});
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database
     .GetCollection<BsonDocument>(Constants.UsersCollection);

// this is the new document - demo only

var newUser = RandomData.GenerateUsers(1).First();
newUser.Id = firstDbUser.Id;
newUser.FirstName = "Chris";
newUser.LastName = "Sakellarios";
newUser.Website = "https://github.com/chsakell";

var bsonFirstUser = await bsonCollection.FindOneAndReplaceAsync(
        Builders<BsonDocument>.Filter.Eq("_id", firstDbUser.Id), 
        newUser.ToBsonDocument(),
        new FindOneAndReplaceOptions<BsonDocument> 
            { ReturnDocument = ReturnDocument.After });
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

{% hint style="info" %}
### FindOneAndReplaceOptions

When using the `FindOneAndReplace` method you have two options for the returned result:

1. Return the updated document - you need to set **`ReturnDocument = ReturnDocument.After`** in the `FindOneAndReplaceOptions`
2. Return the document before being updated - you need to set **`ReturnDocument = ReturnDocument.Before`** in the `FindOneAndREplaceOptions` or leave it as is since it's the default value
{% endhint %}

