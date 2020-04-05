# Update documents

## Update one document

To update a document in MongoDB you need to configure 2 basic things:

1. A filter definition that defines which document should be updated
2. An update definition what defines what should be updated in the matched document

After configuring the above two, you use the `UpdateOne` method on an `IMongoCollection<T>` reference.

The following example updates the phone number of a user's document filter by its id.

{% tabs %}
{% tab title="Typed" %}
{% code title="UpdateDocuments.cs" %}
```csharp
var personsCollection = usersDatabase.GetCollection<User>("users");

// Create an equality filter
var filter = Builders<User>.Filter
    .Eq(person => person.Id, appPerson.Id);

// Create an update definition using the Set operator    
var update = Builders<User>.Update
    .Set(person => person.Phone, "123-456-789");

// Update the document
var personUpdateResult = await personsCollection.UpdateOneAsync(filter, update);
if (personUpdateResult.MatchedCount == 1 && personUpdateResult.ModifiedCount == 1)
{
    Utils.Log( $"Document {appPerson.Id} Updated");
}
```
{% endcode %}
{% endtab %}

{% tab title="BsonDocument" %}
```csharp
// Get a collection reference
var bsonPersonCollection = usersDatabase.GetCollection<BsonDocument>("users");

// Create an equality filter
var bsonSingleFilter = Builders<BsonDocument>.Filter.Eq("_id", appPerson.Id);

// Create a Set update definition
var bsonUpdate = Builders<BsonDocument>.Update.Set("phone", "123-456-678");

// Update the document
var bsonPersonUpdateResult = 
    await bsonPersonCollection.UpdateOneAsync(bsonSingleFilter, bsonUpdate);
if (bsonPersonUpdateResult.MatchedCount == 1 && 
    bsonPersonUpdateResult.ModifiedCount == 1)
{
    Utils.Log("Person updated");
}
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.updateOne({ _id: Object("") }, { $set: {  phone: "123-456-789" } })
```
{% endtab %}

{% tab title="Result" %}
```javascript

```
{% endtab %}
{% endtabs %}

{% hint style="info" %}
 `UpdateOne` method returns an `UpdateResult` result that indicates the result of the update operation. 

1. **MatchedCount**: The number of documents matched your filter definition
2. **ModifiedCount**: The number of documents updated
3. **IsAcknowledged**: Indicates whether the result is acknowledged
4. **UpsertedId**: The _**upserted**_ id, if one exists

Remember that if the document already has the same value that you want to update to, `modifiedCount` will be 0
{% endhint %}

## Update multiple documents

To update many documents at once, follow the same process but this time use the `UpdateMany` method.

The following example filters user documents having _salary_ greater than 1200 and less than 3500 and set the _salary_ value to 4000.

{% tabs %}
{% tab title="Typed" %}
{% code title="UpdateDocuments.cs" %}
```csharp
// Create a filter by combining a Greater & Less than filters
var salaryFilter = Builders<User>.Filter
    .And(
        Builders<User>.Filter.Gt(person => person.Salary, 1200),
        Builders<User>.Filter.Lt(person => person.Salary, 3500)
        );

// This is just for demonstration - validate the update result
var totalPersons = await personsCollection
    .Find(salaryFilter).CountDocumentsAsync();

// Create an update definition using the Set operator 
var updateDefinition = Builders<User>.Update
    .Set(person => person.Salary, 4000);

var updateResult = await personsCollection
    .UpdateManyAsync(salaryFilter, updateDefinition);

if (updateResult.MatchedCount.Equals(totalPersons))
{
    Utils.Log($"Salary has been updated for {totalPersons}");
}
```
{% endcode %}
{% endtab %}

{% tab title="BsonDocument" %}
```csharp
// Create a filter by combining a Greater & Less than filters
var bsonSalaryFilter = Builders<BsonDocument>.Filter
    .And(
        Builders<BsonDocument>.Filter.Gt("salary", 1200),
        Builders<BsonDocument>.Filter.Lt("salary", 3500)
    );

// Create an update definition using the Set operator 
var bsonUpdateDefinition =
    Builders<BsonDocument>.Update.Set("salary", 4000);

var bsonUpdateResult = await bsonPersonCollection
    .UpdateManyAsync(bsonSalaryFilter, bsonUpdateDefinition);
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.updateMany(
    { $and: [{ salary: { $gt: 1200} }, {salary: { $lt: 3500} }] },
    { $set: { salary: 4000  } }
)
```
{% endtab %}

{% tab title="Sample result" %}
```javascript
{
	"acknowledged" : true,
	"matchedCount" : 20,
	"modifiedCount" : 20
}
```
{% endtab %}
{% endtabs %}



