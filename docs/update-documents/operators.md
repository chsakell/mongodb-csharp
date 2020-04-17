# Operators



## _Set_ operator - _$set_

The _$set_ operator is used to update the value of a specified field. 

The sample updates the _FirstName_ field of the first document in the collection.

{% tabs %}
{% tab title="C\#" %}
{% code title="Update/BasicOperators.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

// create an empty filter
var firstUserFilter = Builders<User>.Filter.Empty;

// create a Set operator update definition
var updateNameDefinition = Builders<User>.Update
            .Set(u => u.FirstName, "Chris");

// update the document
var updateNameResult = await collection
            .UpdateOneAsync(firstUserFilter, 
            updateNameDefinition);
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database
            .GetCollection<BsonDocument>(collectionName);

var bsonUpdateNameDefinition = Builders<BsonDocument>
            .Update.Set("firstName", "John");

var bsonUpdateNameResult = await bsonCollection
            .UpdateOneAsync(bsonFirstUserFilter, 
            bsonUpdateNameDefinition);
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.updateOne({}, 
	{ $set: { firstName: "Chris"  }});

--------------------------- 
        
// sample update result
{
	"acknowledged" : true,
	"matchedCount" : 1,
	"modifiedCount" : 1
}
```
{% endtab %}

{% tab title="Models" %}
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

