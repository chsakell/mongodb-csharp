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

## _Min_ operator - _$min_

The _$min_ operator is used to update the value of a specified field **only if the new value is less than** the current value.

> **Syntax**: `Builders<T<.Update.Min(doc => doc.<field>, <value>)`

The sample decreases the first document's _salary_ value from 3000 to 2000 💰 .

{% tabs %}
{% tab title="C\#" %}
{% code title="Update/BasicOperators.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

// create an empty filter
var firstUserFilter = Builders<User>.Filter.Empty;

// preparation - set current salary to 3000
// for demo only
await collection.UpdateOneAsync(firstUserFilter, Builders<User>
    .Update.Set(u => u.Salary, 3000));
    
// update only if the new value is less than the current
var minUpdateDefinition = Builders<User>
    .Update.Min(u => u.Salary, 2000);
    
// 2000 is less than 3000 so update succeeds
var minUpdateResult = await collection
    .UpdateOneAsync(firstUserFilter, minUpdateDefinition);
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database
            .GetCollection<BsonDocument>(collectionName);

await bsonCollection.UpdateOneAsync(bsonFirstUserFilter,
    Builders<BsonDocument>.Update.Set("salary", 3000));

// would not update if the new salary was > 3000
var bsonMinUpdateDefinition = Builders<BsonDocument>
    .Update.Min("salary", 2000);
    
var bsonMinUpdateResult = await bsonCollection
    .UpdateOneAsync(bsonFirstUserFilter, 
        bsonMinUpdateDefinition);
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.updateOne({}, { $min: { 
	salary: NumberDecimal("2000") } })


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

{% hint style="info" %}
Of course if the new value is equal to the current one, the update result will return that no documents updated.

```javascript
{
	"acknowledged" : true,
	"matchedCount" : 1,
	"modifiedCount" : 0
}
```
{% endhint %}

## _Max_ operator - _$max_

The _$max_ operator is used to update the value of a specified field **only if the new value is greater than** the current value.

> **Syntax**: `Builders<T<.Update.Max(doc => doc.<field>, <value>)`

The sample increases the first document's _salary_ value from 3000 to 3500 💰 .

{% tabs %}
{% tab title="C\#" %}
{% code title="Update/BasicOperators.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

// create an empty filter
var firstUserFilter = Builders<User>.Filter.Empty;

// preparation - set current salary to 3000
// for demo only
await collection.UpdateOneAsync(firstUserFilter, Builders<User>
    .Update.Set(u => u.Salary, 3000));
    
// update only if the new value is greater than the current
var maxUpdateDefinition = Builders<User>
    .Update.Max(u => u.Salary, 3500);
    
// 3500 is greater than 3000 so update succeeds
var maxUpdateResult = await collection
    .UpdateOneAsync(firstUserFilter, maxUpdateDefinition);
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database
            .GetCollection<BsonDocument>(collectionName);

await bsonCollection.UpdateOneAsync(bsonFirstUserFilter,
    Builders<BsonDocument>.Update.Set("salary", 3000));

// would not update if the new salary was < 3500
var bsonMaxUpdateDefinition = Builders<BsonDocument>
    .Update.Max("salary", 3500);
    
var bsonMaxUpdateResult = await bsonCollection
        .UpdateOneAsync(bsonFirstUserFilter, 
            bsonMaxUpdateDefinition);
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.updateOne({}, { $max: { 
		salary: NumberDecimal("3500") } 
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

## _Mul_ operator - _$mul_

The _$mul_ operator is used to multiply the current value of a specified field by a specified value.

> **Syntax**: `Builders<T<.Update.Mul(doc => doc.<field>, <value>)`

The sample double the first document's _salary_ value from 1000 to 2000 using the $mul operator 💰 .

{% tabs %}
{% tab title="C\#" %}
{% code title="Update/BasicOperators.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

// create an empty filter
var firstUserFilter = Builders<User>.Filter.Empty;

// preparation - set current salary to 1000
// for demo only
await collection.UpdateOneAsync(firstUserFilter, 
    Builders<User>.Update.Set(u => u.Salary, 1000));
    
// multiply the current salary value by 2
var mulUpdateDefinition = Builders<User>
    .Update.Mul(u => u.Salary, 2);
    
var mulUpdateResult = await collection
    .UpdateOneAsync(firstUserFilter, mulUpdateDefinition);
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database
            .GetCollection<BsonDocument>(collectionName);

await bsonCollection.UpdateOneAsync(bsonFirstUserFilter,
    Builders<BsonDocument>.Update
        .Set("salary", 1000));

var bsonMulUpdateDefinition = Builders<BsonDocument>
    .Update.Mul("salary", 2);
    
var bsonMulUpdateResult = await bsonCollection
    .UpdateOneAsync(bsonFirstUserFilter, 
        bsonMulUpdateDefinition);
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.updateOne({}, { $mul: { salary: 2 } })

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

## _Unset_ operator - _$unset_

The _$unset_ operator is used to remove a field from a document.

> **Syntax**: `Builders<T<.Update.Unset(doc => doc.<field>)`

The sample removes ❌ the _Website_ field from a user document.

{% tabs %}
{% tab title="C\#" %}
{% code title="Update/BasicOperators.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

// create an empty filter
var firstUserFilter = Builders<User>.Filter.Empty;

// remove the website field
var removeWebsiteDefinition = Builders<User>
    .Update.Unset(u => u.Website);

var removeWebsiteFieldUpdateResult = await collection
    .UpdateOneAsync(firstUserFilter, 
        removeWebsiteDefinition);
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database
            .GetCollection<BsonDocument>(collectionName);

var bsonRemoveWebsiteDefinition = Builders<BsonDocument>
            .Update.Unset("website");
            
var bsonRemoveWebsiteFieldUpdateResult =
    await bsonCollection.UpdateOneAsync(bsonFirstUserFilter, 
                bsonRemoveWebsiteDefinition);
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.updateOne({}, { $unset: { website: ""  }})

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

{% hint style="warning" %}
####  ​​ ⚡ Danger ​ ⚡​​ Danger ​ ⚡​​ Danger ​ ⚡

Use **caution** when using the _Unset_ operator!! In case you unset a _non nullable_ property then you might end up having unexpected results. For example, if you unset the _salary_ field on a user document, which is a decimal field, then the next time you read that document, the salary will be 0, not null!

```csharp
// remove salary field - decimal property
var removeSalaryDefinition = Builders<User>
    .Update.Unset(u => u.Salary);
    
var removeSalaryFieldUpdateResult =
    await collection.UpdateOneAsync(firstUserFilter, 
    removeSalaryDefinition);

// firstUser.Salary is equal to 0!
var firstUser = await collection.Find(firstUserFilter)
    .FirstOrDefaultAsync();
```
{% endhint %}

