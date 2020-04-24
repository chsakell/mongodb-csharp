# ❌ Delete

## Delete one document

To delete a single document, create a filter definition that matches the document you want to remove and call the `DeleteOne` method on a `IMongoCollection<T>` reference.

{% tabs %}
{% tab title="Syntax" %}
```csharp
IMongoCollection<T>
    .DeleteOne(<filter>)
```
{% endtab %}
{% endtabs %}

The samples filter a `User` document by its Id and removes it from the collection.

{% tabs %}
{% tab title="C\#" %}
{% code title="DeleteDocuments.cs" %}
```csharp
// get a collection reference
var personsCollection = usersDatabase
    .GetCollection<User>(Constants.UsersCollection);

// find a person using an equality filter on its id
var filter = Builders<User>.Filter.Eq(person => person.Id, appPerson.Id);

// delete the person
var personDeleteResult = await personsCollection.DeleteOneAsync(filter);
if (personDeleteResult.DeletedCount == 1)
{
    Utils.Log($"Document {appPerson.Id} deleted");
}
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
// get a collection reference
var bsonPersonCollection = usersDatabase
    .GetCollection<BsonDocument>(Constants.UsersCollection);
    
// find a person using a greater than filter on its salary field
var bsonSingleFilter = Builders<BsonDocument>.Filter.Gt("salary", 2000);

// delete the first person that fulfills the filter criteria
var bsonPersonDeleteResult = await bsonPersonCollection
    .DeleteOneAsync(bsonSingleFilter);
```
{% endtab %}

{% tab title="Shell" %}
```javascript
// delete a single document
db.users.deleteOne({ _id : ObjectId("5e5ff25170dc588dd0870073")})

----------------------
// sample result
{
	"acknowledged" : true,
	"deletedCount" : 1
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
*  When your filter criteria matches more the one document, the first document that matches the filter will be removed
*  Use a field that is [unique](https://docs.mongodb.com/manual/reference/glossary/#term-unique-index) across a single collection to be more precise
{% endhint %}

## Delete the first document in the collection

To delete the first document in the collection, simply use an empty filter definition.

{% tabs %}
{% tab title="C\#" %}
{% code title="DeleteDocuments.cs" %}
```csharp
// get a collection reference
var personsCollection = usersDatabase
    .GetCollection<User>(Constants.UsersCollection);

// create an empty filter definition
var emptyFilter = Builders<User>.Filter.Empty;

// delete the first document in the collection
var firstPersonDeleteResult = await personsCollection
    .DeleteOneAsync(emptyFilter);
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
// get a collection reference
var bsonPersonCollection = usersDatabase
    .GetCollection<BsonDocument>(Constants.UsersCollection);

// create an empty filter
var bsonEmptyFilter = Builders<BsonDocument>.Filter.Empty;

// delete the first document in the collection
var bsonFirstPersonDeleteResult = await bsonPersonCollection
    .DeleteOneAsync(bsonEmptyFilter);
```
{% endtab %}

{% tab title="Shell" %}
```javascript
// delete the first document in the collection
db.users.deleteOne({})
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

## Delete multiple documents

To remove more that one documents at the same time, create a filter definition to match the documents you wish to delete and use the `DeleteMany` method on an `IMongoCollection<T>`. 

> **Syntax**: `IMongoCollection<T>.DeleteMany(<filter>)`

{% tabs %}
{% tab title="Syntax" %}
```csharp
IMongoCollection<T>
    .DeleteMany(<filter>)
```
{% endtab %}
{% endtabs %}

The following example shows how to delete user documents based on the _salary_ field .

{% tabs %}
{% tab title="C\#" %}
{% code title="DeleteDocuments.cs" %}
```csharp
var personsCollection = usersDatabase
    .GetCollection<User>(Constants.UsersCollection);
    
// create a filter
var salaryFilter = Builders<User>.Filter
    .And(
        Builders<User>.Filter.Gt(person => person.Salary, 1200),
        Builders<User>.Filter.Lt(person => person.Salary, 3500)
        );

// for demonstration only - just to validate the total documents removed
var totalPersons = await personsCollection
    .Find(salaryFilter).CountDocumentsAsync();

// delete the documents
var personsDeleteResult = await personsCollection.DeleteManyAsync(salaryFilter);

if (personsDeleteResult.DeletedCount.Equals(totalPersons))
{
    Utils.Log($"{totalPersons} users deleted");
}
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonPersonCollection = usersDatabase
    .GetCollection<BsonDocument>(Constants.UsersCollection);
    
// find all users with salary > 2000
var bsonSingleFilter = Builders<BsonDocument>.Filter.Gt("salary", 2000);

// delete the matched documents
var bsonPersonsDeleteResult = await bsonPersonCollection
            .DeleteManyAsync(bsonSingleFilter);
```
{% endtab %}

{% tab title="Shell" %}
```javascript
// delete multiple documents
db.users.deleteMany(
    { $and: [{ salary: { $gt: 1200} }, {salary: { $lt: 3500} }] }
)

-----------------------

// sample result
{
	"acknowledged" : true,
	"deletedCount" : 15
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

