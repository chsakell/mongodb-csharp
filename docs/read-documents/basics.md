---
description: "Meet the builder \U0001F91D"
---

# Basics

## The `FilterDefinitionBuilder` class

To create a filter query for an `IMongoCollection<T>` collection, you need to build a `FilterDefinition<T>` filter. You can do this using the `Builders<T>.Filter` filter definition builder which contains several filters such as the equality or element filters. This section will give you an overview on how to create and use the filter definition builder.

> **Syntax**: `Builders<T>.Filter.<operator>`

##  The `Empty` filter

Use the `Empty` filter when you want to get either the 1st document or all of its documents.

{% tabs %}
{% tab title="C\#" %}
{% code title="Crud.Read.Basics.cs" %}
```csharp
// empty filter
var emptyFilter = Builders<User>.Filter.Empty;

// first user
var firstUser = await collection.Find(emptyFilter).FirstOrDefaultAsync();

// all users
var allUsers = await collection.Find(emptyFilter).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="BsonDocument" %}
```csharp
var bsonEmptyDocument = Builders<BsonDocument>.Filter.Empty;

var bsonFirstUser = await bsonCollection
    .Find(new BsonDocument()).FirstOrDefaultAsync();
    
var bsonAllUsers = await bsonCollection
    .Find(bsonEmptyDocument).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
// first document
db.users.findOne({})

// all documents (returns a cursor)
db.users.find({})
```
{% endtab %}
{% endtabs %}

{% hint style="info" %}
 When querying a `IMongoCollection<BsonDocument>` collection, you can use an empty `BsonDocument` as an empty filter definition
{% endhint %}

## Equality filter on a top level field

The equality filter is one of the most used filters you are gonna use when querying MongoDB. The following examples filter user documents on top level **string** fields, _profession_ and _email_.

> **Syntax**: `Builders<T>.Filter.Eq(doc => doc.<field>, <value>)`

{% tabs %}
{% tab title="C\#" %}
{% code title="Crud.Read.Basics.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

// create the equality filter
var doctorsFilter = Builders<User>.Filter
    .Eq(u => u.Profession, "Doctor");
    
var doctors = await collection.Find(doctorsFilter).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="BsonDocument" %}
```csharp
var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

var sampleBsonUserFilter = Builders<BsonDocument>.Filter
    .Eq("email", sampleUser.Email);
    
var dbBsonSampleUser = await bsonCollection
    .Find(sampleBsonUserFilter).FirstOrDefaultAsync();
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

{% tab title="Shell" %}
```javascript
// find all doctors
db.users.find({ profession: "Doctor"})

// find a user document with specific email address
db.users.findOne({ email: "sample@example.com" })

```
{% endtab %}
{% endtabs %}

{% hint style="warning" %}
Equality filter is **case sensitive**, so always make sure to use it properly!
{% endhint %}

## Equality filter on a nested field

You can use the equality filter to match your documents based on an embedded document field. In the following example the _address_ field is an **embedded** field on the user document and contains a _city_ string field.  The sample show how to filter documents based on the _city_ field.

> **Syntax**: `Builders<T>.Filter.Eq(doc => doc.<field1>..<fieldN>.<embedded-field>, <value>)`

{% hint style="warning" %}
The hierarchy from _`<field1>`_ to the _`<embedded-field>`_ **cannot** contain an array field
{% endhint %}

{% tabs %}
{% tab title="C\#" %}
{% code title="Crud.Read.Basics.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

// create the filter on the address.city field
var athensCityFilter = Builders<User>
    .Filter.Eq(u => u.Address.City, "Athens");
    
var athensUsers = await collection
    .Find(athensCityFilter).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="BsonDocument" %}
```csharp
var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

var bsonAthensCityFilter = Builders<BsonDocument>
    .Filter.Eq("address.city", "Athens");
    
var bsonAthensUsers = await bsonCollection
    .Find(bsonAthensCityFilter).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.find({"address.city": { $eq: "Athens"}})
db.users.find({"address.city": "Athens"})
```
{% endtab %}

{% tab title="Sample result" %}
```javascript
{
	"_id" : ObjectId("5e919356139fe1568028900d"),
	"gender" : 1,
	"firstName" : "Lana",
	"lastName" : "Price",
	"userName" : "Lana_Price",
	"avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/adhiardana/128.jpg",
	"email" : "Lana.Price@hotmail.com",
	"dateOfBirth" : ISODate("1951-11-17T06:47:46.404+02:00"),
	"address" : {
		"street" : "8901 Korbin Fords",
		"suite" : "Apt. 810",
		"city" : "Athens", // matched document
		"state" : "Maine",
		"zipCode" : "28939-3112",
		"geo" : {
			"lat" : 62.7547,
			"lng" : -171.0489
		}
	},
	"phone" : "351.573.6992 x6949",
	"website" : "sabryna.com",
	"company" : {
		"name" : "O'Connell Group",
		"catchPhrase" : "Profound value-added hardware",
		"bs" : "reinvent back-end channels"
	},
	"salary" : 4879,
	"monthlyExpenses" : 4378,
	"favoriteSports" : [
		"Tennis",
		"Water Polo",
		"Baseball",
		"Cricket",
		"MMA",
		"Cycling",
		"Snooker",
		"Beach Volleyball",
		"Darts",
		"Motor Sport",
		"Table Tennis"
	],
	"profession" : "Model"
},
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

{% tab title="AddressCard" %}
```csharp
public class AddressCard
{
    public string Street {get; set; }
    public string Suite {get; set; }
    public string City {get; set; }
    public string State {get; set; }
    public string ZipCode {get; set; }
    public AppCardGeo Geo {get; set; }

    public class AppCardGeo
    {
        public double Lat {get; set; }
        public double Lng {get; set; }
    }
}
```
{% endtab %}
{% endtabs %}

{% hint style="info" %}
Remember, the filter might be on an embedded document field, but the result will be always the entire document\(s\) that matched the criteria
{% endhint %}

## Equality filter on a array field

Assuming your document contains an array field with **string** values, you want to get all documents that their array field **contains** a specific value. The sample that follows works for other types as well \(_e.g. bool, int, decimal, float\)_.

> **Syntax**: `Builders<T>.Filter.Eq(doc => doc.<array-field>, <value>)`

The example retrieves all user documents that their _FavoriteSports_ array field contains a specific sport. It does that by using an **equality filter for an array field** called `AnyEq`.

{% tabs %}
{% tab title="C\#" %}
{% code title="Crud.Read.Basics.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

// create an equality filter on an array field
// find all user documents that contains 'Basketball' on their sports array
var basketballFilter = Builders<User>.Filter
    .AnyEq(u => u.FavoriteSports, "Basketball");

// the matched documents might have other sports
// in the favoriteSports array
var usersHaveBasketball = await collection
    .Find(basketballFilter).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="BsonDocument" %}
```csharp
var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

var bsonBasketballFilter = Builders<BsonDocument>.Filter
    .AnyEq("favoriteSports", "Basketball");
    
var bsonUsersHaveBasketball = await bsonCollection
    .Find(bsonBasketballFilter).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
// MongoDB understands that favoriteSports field is an array
// and makes the right search on it
db.users.find({"favoriteSports": "Basketball"})
```
{% endtab %}

{% tab title="Sample Result" %}
```javascript
{
	"_id" : ObjectId("5e91e3ba3c1ba62570a67b98"),
	"gender" : 0,
	"firstName" : "Spencer",
	"lastName" : "Swift",
	"userName" : "Spencer.Swift",
	"avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/daykiine/128.jpg",
	"email" : "Spencer61@hotmail.com",
	"dateOfBirth" : ISODate("1955-01-08T04:19:35.543+02:00"),
	"address" : {
		"street" : "9336 Walker Crest",
		"suite" : "Suite 144",
		"city" : "South Tate",
		"state" : "South Carolina",
		"zipCode" : "46603",
		"geo" : {
			"lat" : 39.9979,
			"lng" : -117.9544
		}
	},
	"phone" : "1-377-659-2465 x7862",
	"website" : "barbara.com",
	"company" : {
		"name" : "Wisoky, Lynch and Torphy",
		"catchPhrase" : "Diverse dedicated customer loyalty",
		"bs" : "scale sexy technologies"
	},
	"salary" : 4325,
	"monthlyExpenses" : 3023,
	"favoriteSports" : [
		"Basketball", // matched document
		"Table Tennis",
		"Moto GP",
		"Tennis",
		"Boxing",
		"Motor Sport",
		"Ice Hockey",
		"Baseball",
		"American Football",
		"Water Polo",
		"Volleyball",
		"Golf",
		"Cycling",
		"Formula 1",
		"Cricket"
	],
	"profession" : "Pilot"
},
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
When the search term used in `AnyEq` is a simple value, then you are running a search for that term in the array field, meaning that there might be other values contained as well
{% endhint %}

## Equality filter on a array field - Exact match

In case you want to run an exact match on the array field then you must use an array argument in the `AnyEq` filter. This will try match the entire array field rather than just searching inside the array.

> **Syntax**: `Builders<T>.Filter.Eq(doc => doc.<array-field>, <array-values>)`

The following example finds the documents that their _FavoriteSports_ array field **contains only** the Soccer term.

{% tabs %}
{% tab title="C\#" %}
{% code title="Crud.Read.Basics.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

// create an equality filter on an array field
// find all user documents that have only 'Soccer' on their sports array
var onlySoccerFilter = Builders<User>.Filter
    .Eq(u => u.FavoriteSports, new List<string> { "Soccer" });

// the matched documents contain only Soccer in the favoriteSports
var soccerUsers = await collection
    .Find(onlySoccerFilter).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="BsonDocument" %}
```csharp
var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

var bsonOnlySoccerFilter = Builders<BsonDocument>.Filter
            .Eq("favoriteSports", new List<string>() { "Soccer" });

var bsonSoccerUsers = await bsonCollection
            .Find(bsonOnlySoccerFilter).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.find({"favoriteSports": ["Soccer"]})
```
{% endtab %}

{% tab title="Sample result" %}
```javascript
{
	"_id" : ObjectId("5e91e3ba3c1ba62570a67e08"),
	"gender" : 1,
	"firstName" : "June",
	"lastName" : "Dach",
	"userName" : "June.Dach23",
	"avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/d33pthought/128.jpg",
	"email" : "June64@gmail.com",
	"dateOfBirth" : ISODate("1970-02-26T04:13:22.471+02:00"),
	"address" : {
		"street" : "0942 Prosacco Extension",
		"suite" : "Apt. 518",
		"city" : "Aubreytown",
		"state" : "Louisiana",
		"zipCode" : "39912",
		"geo" : {
			"lat" : 43.4411,
			"lng" : -73.1164
		}
	},
	"phone" : "1-832-666-1307 x6427",
	"website" : "wilfrid.biz",
	"company" : {
		"name" : "Feest LLC",
		"catchPhrase" : "Reduced national project",
		"bs" : "incentivize plug-and-play action-items"
	},
	"salary" : 3871,
	"monthlyExpenses" : 2241,
	"favoriteSports" : [
		"Soccer" // matched document
	],
	"profession" : "Engineer"
}
```
{% endtab %}
{% endtabs %}

