# Comparison Operators

## Overview

Comparison operators are probably the most used operators when querying any type of database provider. MongoDB provides a set of operators 🛠 that can cover all type of comparisons you might need for your queries. This section presents samples for the following **query operators**:

* **Equal** operator
* **Not Equal** operator
* **Greater Than** operator
* **Greater Than or Equal** operator
* **Less Than** operator
* **Less Than or Equal** operator
* **In** operator
* **Not In** operator

![Comparison Operators](../.gitbook/assets/compare.png)

## 

## Equal operator

The equal operator is used to match documents having a field value equal to a specific value. You can use it for both top level and embedded documents.

The sample uses an equal operator to find all documents that have the _profession_ field _\(top level field\)_ equal to "_Pilot_".

{% tabs %}
{% tab title="C\#" %}
{% code title="ComparisonOperators.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

// Case sensitive!
var equalPilotsFilter = Builders<User>.Filter
    .Eq(u => u.Profession, "Pilot");

var pilots = await collection
    .Find(equalPilotsFilter).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="BsonDocument" %}
```csharp
var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

// Case sensitive matters!  
var bsonEqualPilotsFilter = Builders<BsonDocument>.Filter
    .Eq("profession", "Pilot");
    
var bsonPilots = await bsonCollection
    .Find(bsonEqualPilotsFilter).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.find({profession: { $eq: "Pilot"}})
// or..
db.users.find({profession: "Pilot"})
```
{% endtab %}

{% tab title="Result" %}
```javascript
// sample matched document
{
	"_id" : ObjectId("5e91e3ba3c1ba62570a67b97"),
	"gender" : 0,
	"firstName" : "Gilbert",
	"lastName" : "Beer",
	"userName" : "Gilbert43",
	"avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/lingeswaran/128.jpg",
	"email" : "Gilbert_Beer@yahoo.com",
	"dateOfBirth" : ISODate("1950-06-07T20:53:27.758+02:00"),
	"address" : {
		"street" : "3952 Felicita Garden",
		"suite" : "Suite 048",
		"city" : "North Pasqualefort",
		"state" : "Kansas",
		"zipCode" : "56191",
		"geo" : {
			"lat" : -0.8177,
			"lng" : -154.6886
		}
	},
	"phone" : "(973) 473-1826 x2746",
	"website" : "virgie.net",
	"company" : {
		"name" : "Quigley, Mitchell and McGlynn",
		"catchPhrase" : "Multi-layered holistic moratorium",
		"bs" : "enable front-end markets"
	},
	"salary" : 3028,
	"monthlyExpenses" : 3080,
	"favoriteSports" : [
		"Cycling",
		"MMA",
		"Boxing",
		"Handball",
		"Snooker",
		"American Football",
		"Volleyball",
		"Water Polo",
		"Beach Volleyball",
		"Ice Hockey",
		"Motor Sport",
		"Tennis",
		"Formula 1"
	],
	"profession" : "Pilot" // matched here
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

## Not Equal operator

When you want to match all document that certain field\(s\) values **are not equal** to a specific value then you use the `Not Equal` operator. 

The sample uses a not equal operator to match all documents that their _profession_ field is other than "_Doctor_".

{% tabs %}
{% tab title="C\#" %}
{% code title="ComparisonOperators.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

var notEqualDoctorsFilter = Builders<User>.Filter
    .Ne(u => u.Profession, "Doctor");
    
var notDoctors = await collection
    .Find(notEqualDoctorsFilter).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="BsonDocument" %}
```csharp
var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

var bsonNotEqualDoctorsFilter = Builders<BsonDocument>.Filter
            .Ne("profession", "Doctor");
            
var bsonNotDoctors = await bsonCollection
            .Find(bsonNotEqualDoctorsFilter).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.find({profession: { $ne: "Doctor"}})
```
{% endtab %}

{% tab title="Result" %}
```javascript
// sample matched document
{
	"_id" : ObjectId("5e91e3ba3c1ba62570a67b95"),
	"gender" : 0,
	"firstName" : "Ronnie",
	"lastName" : "Weissnat",
	"userName" : "Ronnie.Weissnat",
	"avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/dactrtr/128.jpg",
	"email" : "Ronnie.Weissnat61@gmail.com",
	"dateOfBirth" : ISODate("1981-03-23T18:39:37.569+02:00"),
	"address" : {
		"street" : "48091 Hazle Trafficway",
		"suite" : "Suite 355",
		"city" : "South Fayeburgh",
		"state" : "New Hampshire",
		"zipCode" : "77588",
		"geo" : {
			"lat" : 19.3213,
			"lng" : 19.1313
		}
	},
	"phone" : "719-576-6815 x76397",
	"website" : "grayce.org",
	"company" : {
		"name" : "Davis Inc",
		"catchPhrase" : "Mandatory maximized attitude",
		"bs" : "grow clicks-and-mortar eyeballs"
	},
	"salary" : 2578,
	"monthlyExpenses" : 2986,
	"favoriteSports" : [
		"Darts",
		"Tennis",
		"MMA",
		"Snooker",
		"Handball",
		"Ice Hockey",
		"Boxing",
		"American Football",
		"Beach Volleyball",
		"Volleyball",
		"Cycling",
		"Baseball"
	],
	"profession" : "Lawyer" // matched here
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

## Greater Than operator



## Greater Than or Equal operator



## Less Than operator



## Less Than or Equal operator



## In operator



## Not In operator

