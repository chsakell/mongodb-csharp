# 📦 Group

## Group documents

The group stage groups documents by a specified _**\_id expression**_ and outputs a document for each group. One way to use the Match stage in a pipeline is to use the `Aggregate` method in a `IMongoCollection<T>` and chain the `Match` method.

> **Syntax**: `IMongoCollection<T>.Aggregate().Group(<group-key>, <output>)`

The sample groups `User` documents by their _profession_ field and outputs the total documents per group.

{% tabs %}
{% tab title="C\#" %}
{% code title="Match.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

var singleFieldAggregate = collection.Aggregate()
    .Group(u => u.Profession,
        ac => new { 
            profession = ac.Key, 
            total = ac.Sum(u => 1)
        });

var groupedProfessions = 
    await singleFieldAggregate.ToListAsync();
    
foreach (var group in groupedProfessions)
{
    Utils.Log($"{group.profession}: {group.total}");
}
```
{% endcode %}
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.aggregate([
   {
      "$group":{
         "_id":"$profession",
         "total":{
            "$sum":1
         }
      }
   }
])

----------------------------

// sample results

/* 1 */
{
	"_id" : "Lawyer",
	"total" : 44
},

/* 2 */
{
	"_id" : "Social-Media Manager",
	"total" : 35
},

/* 3 */
{
	"_id" : "Financial Adviser",
	"total" : 34
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

### Group by an embedded document's field

You can group by any document's field you want. The sample groups the `User` documents by the _Address.State_ field to find the total documents **per state sorted by total**.

{% tabs %}
{% tab title="C\#" %}
{% code title="Match.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

var embeddedDocFieldAggregate = collection.Aggregate()
    .Group(u => u.Address.State, // embedded document
        ac => new {state = ac.Key, total = ac.Sum(u => 1)})
    .SortBy(group => group.total); // ASC

var groupedPerCountry = await embeddedDocFieldAggregate
    .ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.aggregate([
    { "$group" : 
    	{ 	"_id" : "$address.state", 
    			"total" : { "$sum" : 1 } } 
    	},
    { "$sort" : { "total" : 1 } }
])

----------------------------

// sample results

/* 1 */
{
	"_id" : "Arizona",
	"total" : 11
},

/* 2 */
{
	"_id" : "Missouri",
	"total" : 12
},

/* 3 */
{
	"_id" : "Iowa",
	"total" : 12
},

/* 4 */
{
	"_id" : "Kentucky",
	"total" : 13
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

### 

