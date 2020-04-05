# Read documents

## Find a document

To search for a document in MongoDB you use the `Find` method on a `IMongoCollection<T>` reference. `Find` method access a `FilterDefinition<T>` parameter where T is the collection's type. 

Filters can be created using the `Builders<T>.Filter` definition builder which contain multiple filters. The following example finds a user document based on its Id. It does this using the **equality filter** `Eq<T>` on the id field.

{% tabs %}
{% tab title="Typed" %}
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

{% tab title="BsonDocument" %}
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
{% endtabs %}

{% hint style="warning" %}
 Notice that when filter used with `BsonDocument`, _**\_id**_  field name used instead of _**Id**_ which is the property name on the `User` class. When you use `Builders<User>.Filter` this is done automatically for you by the driver, based on the _Id_ serialization settings
{% endhint %}

## Find multiple documents

To search for multiple documents follow the same process but this time use the `ToList` method. The following example finds all documents with female gender.

{% tabs %}
{% tab title="Typed" %}
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

{% tab title="BsonDocument" %}
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
```
{% endtab %}
{% endtabs %}



