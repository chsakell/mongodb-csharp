# Operators

## _Set_ operator - _$set_

The _$set_ operator is used to update the value of a specified field. 

> **Syntax**: `Builders<T<.Update.Set(doc => doc.<field>, <value>)`

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

### Multiple fields update

You can update multiple document's fields in one operation by declaring more than one update definitions.

The sample updates the first document's _Phone_, _Website_ and _FavoriteSports \(array field\)._

{% tabs %}
{% tab title="C\#" %}
{% code title="Update/BasicOperators.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

var multiUpdateDefinition = Builders<User>.Update
        .Set(u => u.Phone, "123-456-789")
        .Set(u => u.Website, "https://chsakell.com")
        .Set(u => u.FavoriteSports, 
                new List<string> { "Soccer", "Basketball" });

var multiUpdateResult = await collection
        .UpdateOneAsync(firstUserFilter, 
                multiUpdateDefinition);
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database
            .GetCollection<BsonDocument>(collectionName);

var bsonMultiUpdateDefinition = Builders<BsonDocument>.Update
    .Set("phone", "123-456-789")
    .Set("website", "https://chsakell.com")
    .Set("favoriteSports", 
        new List<string> { "Soccer", "Basketball" });

var bsonMultiUpdateResult = await bsonCollection
    .UpdateOneAsync(bsonFirstUserFilter, 
        bsonMultiUpdateDefinition);

```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.updateOne({}, { $set: { 
    phone: "123-456-789", 
    website: "https://chsakell.com", 
    favoriteSports: ["Soccer", "Basketball"]  
}});

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

## _Inc_ operator - _$inc_

The _$inc_ operator is used to update the value of a specified field. 

> **Syntax**: `Builders<T<.Update.Inc(doc => doc.<field>, <value>)`

The sample increments the first document's salary.

{% tabs %}
{% tab title="C\#" %}
{% code title="Update/BasicOperators.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

// create an empty filter
var firstUserFilter = Builders<User>.Filter.Empty;

// create an $inc update definition
var incrementSalaryDefinition = Builders<User>
            .Update.Inc(u => u.Salary, 450);
            
var incrementSalaryResult = await collection
            .UpdateOneAsync(firstUserFilter, 
                        incrementSalaryDefinition);
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database
            .GetCollection<BsonDocument>(collectionName);

var bsonIncrementSalaryDefinition = Builders<BsonDocument>
            .Update.Inc("salary", 450);
            
var bsonIncrementSalaryResult = await bsonCollection
      .UpdateOneAsync(bsonFirstUserFilter, 
         bsonIncrementSalaryDefinition);
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.updateOne({}, { $inc: 
	{ salary: NumberDecimal("450.00") }});

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

### 

