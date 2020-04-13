# Element Operators

## Overview

MongoDB providers two element query operators that helps you find documents based on a field's existence or type. In other words, you can match documents based on whether a field **exists** or in case it does exist, based on its **type**. The two element operators presented on this section are:

* **Exists** operator
* **Type** operator

![](../.gitbook/assets/element.png)



## _Exists_ operator - $exists

The _$exists_ operator matches the documents that contain the field, event its value is _null_. 

> **Syntax**: `Builders<T>.Filter.Exists(doc => doc.<field>, <true || false>)`

The filter definition being created with the `Exists`method on a specific field, matches the documents which contain the field event if its value is _null_.

The sample uses an _And_ operator to find all documents that have male _gender_ **AND** have the _profession_ field equal to "_Doctor_".

{% tabs %}
{% tab title="C\#" %}
{% code title="LogicalOperators.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

// create an equality filter on the gender for 'male' (0)
var maleFilter = Builders<User>.Filter
    .Eq(u => u.Gender, Gender.Male);
    
// create an equality filter on profession for 'Doctor'
var doctorFilter = Builders<User>.Filter
    .Eq(u => u.Profession, "Doctor");
    
// compine the filters with AND operator
var maleDoctorsFilter = Builders<User>.Filter
    .And(maleFilter, doctorFilter);

var maleDoctors = await collection.Find(maleDoctorsFilter).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

var bsonMaleFilter = Builders<BsonDocument>.Filter
    .Eq("gender", Gender.Male);
    
var bsonDoctorFilter = Builders<BsonDocument>.Filter
    .Eq("profession", "Doctor");
    
var bsonMaleDoctorsFilter = Builders<BsonDocument>.Filter
    .And(bsonMaleFilter, bsonDoctorFilter);
    
var bsonMaleDoctors = await bsonCollection
    .Find(bsonMaleDoctorsFilter).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.find({ $and: [
    { profession: { $eq: "Doctor"} }, 
    {gender: { $eq: 0} }
]})

--------------------------- 
        
// sample matched document
{
	"_id" : ObjectId("5e91e3ba3c1ba62570a67bdd"),
	"gender" : 0, // matched here
	"firstName" : "Ralph",
	"lastName" : "Emard",
	"userName" : "Ralph78",
	"avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/xripunov/128.jpg",
	"email" : "Ralph62@yahoo.com",
	"dateOfBirth" : ISODate("1967-02-18T00:07:22.546+02:00"),
	"address" : {
		"street" : "3046 Orn Locks",
		"suite" : "Apt. 046",
		"city" : "North Nestorview",
		"state" : "Wyoming",
		"zipCode" : "03620-0321",
		"geo" : {
			"lat" : 54.3724,
			"lng" : 127.4216
		}
	},
	"phone" : "422-441-7773",
	"website" : "neva.name",
	"company" : {
		"name" : "Kilback LLC",
		"catchPhrase" : "Configurable 6th generation encoding",
		"bs" : "transition seamless initiatives"
	},
	"salary" : 4787,
	"monthlyExpenses" : 4546,
	"favoriteSports" : [
		"Darts",
		"American Football",
		"Basketball",
		"Cricket",
		"Boxing",
		"Golf",
		"Beach Volleyball",
		"Ice Hockey",
		"Water Polo",
		"Table Tennis",
		"Tennis",
		"Snooker",
		"Cycling",
		"MMA",
		"Handball"
	],
	"profession" : "Doctor" // matched here
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
The _Gender_ property on the `User` class is an `Enum` type and the driver is smart enough 🧙♂ 🦉 to translate it properly when sending the query to MongoDB
{% endhint %}



## Type operator

The AND _operator_ performs a logical AND on a set of expressions and match documents that satisfy all of them. 

> **Syntax**: `Builders<T>.Filter.And(FilterDefinition<T>[] filters)`

The idea is that you create as many **filter definitions** you want and pass them as an argument to the `And` `FilterDefinitionBuilder` method. 

The sample uses an _And_ operator to find all documents that have male _gender_ **AND** have the _profession_ field equal to "_Doctor_".

{% tabs %}
{% tab title="C\#" %}
{% code title="LogicalOperators.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

// create an equality filter on the gender for 'male' (0)
var maleFilter = Builders<User>.Filter
    .Eq(u => u.Gender, Gender.Male);
    
// create an equality filter on profession for 'Doctor'
var doctorFilter = Builders<User>.Filter
    .Eq(u => u.Profession, "Doctor");
    
// compine the filters with AND operator
var maleDoctorsFilter = Builders<User>.Filter
    .And(maleFilter, doctorFilter);

var maleDoctors = await collection.Find(maleDoctorsFilter).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

var bsonMaleFilter = Builders<BsonDocument>.Filter
    .Eq("gender", Gender.Male);
    
var bsonDoctorFilter = Builders<BsonDocument>.Filter
    .Eq("profession", "Doctor");
    
var bsonMaleDoctorsFilter = Builders<BsonDocument>.Filter
    .And(bsonMaleFilter, bsonDoctorFilter);
    
var bsonMaleDoctors = await bsonCollection
    .Find(bsonMaleDoctorsFilter).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.find({ $and: [
    { profession: { $eq: "Doctor"} }, 
    {gender: { $eq: 0} }
]})

--------------------------- 
        
// sample matched document
{
	"_id" : ObjectId("5e91e3ba3c1ba62570a67bdd"),
	"gender" : 0, // matched here
	"firstName" : "Ralph",
	"lastName" : "Emard",
	"userName" : "Ralph78",
	"avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/xripunov/128.jpg",
	"email" : "Ralph62@yahoo.com",
	"dateOfBirth" : ISODate("1967-02-18T00:07:22.546+02:00"),
	"address" : {
		"street" : "3046 Orn Locks",
		"suite" : "Apt. 046",
		"city" : "North Nestorview",
		"state" : "Wyoming",
		"zipCode" : "03620-0321",
		"geo" : {
			"lat" : 54.3724,
			"lng" : 127.4216
		}
	},
	"phone" : "422-441-7773",
	"website" : "neva.name",
	"company" : {
		"name" : "Kilback LLC",
		"catchPhrase" : "Configurable 6th generation encoding",
		"bs" : "transition seamless initiatives"
	},
	"salary" : 4787,
	"monthlyExpenses" : 4546,
	"favoriteSports" : [
		"Darts",
		"American Football",
		"Basketball",
		"Cricket",
		"Boxing",
		"Golf",
		"Beach Volleyball",
		"Ice Hockey",
		"Water Polo",
		"Table Tennis",
		"Tennis",
		"Snooker",
		"Cycling",
		"MMA",
		"Handball"
	],
	"profession" : "Doctor" // matched here
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
The _Gender_ property on the `User` class is an `Enum` type and the driver is smart enough 🧙♂ 🦉 to translate it properly when sending the query to MongoDB
{% endhint %}

