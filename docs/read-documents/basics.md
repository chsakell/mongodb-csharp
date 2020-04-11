---
description: "Meet the builder \U0001F91D"
---

# Basics

## The `FilterDefinitionBuilder` class

To create a filter query for an `IMongoCollection<T>` collection, you need to build a `FilterDefinition<T>` filter. You can do this using the `Builders<T>.Filter` filter definition builder which contains several filters such as the equality or element filters. This section will give you an overview on how to create and use the filter definition builder.

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

```
{% endtab %}
{% endtabs %}



