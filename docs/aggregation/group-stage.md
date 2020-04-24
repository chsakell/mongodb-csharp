# 📦 Group

## Group documents

The group stage groups documents by a specified _**\_id expression**_ and outputs a document for each group. One way to use the Match stage in a pipeline is to use the `Aggregate` method in a `IMongoCollection<T>` and chain the **`Group`** method.

```csharp
IMongoCollection<T>.Aggregate()
    .Group(doc => doc.<field>, <output>)
```

The sample groups `User` documents by their _profession_ field and outputs the total documents per group.

{% tabs %}
{% tab title="C\#" %}
{% code title="Group.cs" %}
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
{% code title="Group.cs" %}
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

### Match - Group - Project

The sample combines 4 stages to calculate the average monthly expenses per gender. 

* The **$match** stage filters documents based on the salary
* The **$group** stage groups the filtered documents per gender and calculate the average monthly expenses
* The **$project** stage formats the gender as a string
* The last stage **$sort,** simply sorts the results per average monthly expenses

{% tabs %}
{% tab title="C\#" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

var excercice1Aggregate = collection.Aggregate()
    .Match(Builders<User>.Filter.Gte(u => u.Salary, 1500) &
           Builders<User>.Filter.Lte(u => u.Salary, 3000))
    .Group(u => u.Gender,
        ac => new
        {
            gender = ac.Key,
            averageMonthlyExpenses = ac.Average(u => u.MonthlyExpenses),
            total = ac.Sum(u => 1)
        })
    .Project(group => new 
    {
        Gender = group.gender == 0 ? "Male" : "Female",
        AverageMonthlyExpenses = group.averageMonthlyExpenses,
        Total = group.total
    })
    .SortByDescending(group => group.AverageMonthlyExpenses);
    
var excercice1Result = await excercice1Aggregate.ToListAsync();
```
{% endtab %}

{% tab title="LINQ" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

var linqQuery = collection.AsQueryable()
    .Where(u => u.Salary > 1500 && u.Salary < 3000)
    .GroupBy(u => u.Gender)
    .Select(ac => new
    {
        gender = ac.Key,
        averageMonthlyExpenses = 
            Math.Ceiling(ac.Average(u => u.MonthlyExpenses)),
        total = ac.Sum(u => 1)
    })
    .OrderBy(group => group.total);

var excercice1LinqResult = linqQuery.ToList();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.aggregate([
   {
      "$match":{
         "salary":{
            "$gt":NumberDecimal("1500"),
            "$lt":NumberDecimal("3000")
         }
      }
   },
   {
      "$group":{
         "_id":"$gender",
         "__agg0":{
            "$avg":"$monthlyExpenses"
         },
         "__agg1":{
            "$sum":1
         }
      }
   },
   {
      "$project":{
         "gender":"$_id",
         "averageMonthlyExpenses":{
            "$ceil":"$__agg0"
         },
         "total":"$__agg1",
         "_id":0
      }
   },
   {
      "$sort":{
         "total":1
      }
   }
])

-----------------------------------------

// results with ceil
/* 1 */
{
	"gender" : 0,
	"averageMonthlyExpenses" : 4127,
	"total" : 181
},

/* 2 */
{
	"gender" : 1,
	"averageMonthlyExpenses" : 4037,
	"total" : 188
}


// results without ceil

/* 1 */
{
	"gender" : 0,
	"averageMonthlyExpenses" : 4126.922651933702,
	"total" : 181
},

/* 2 */
{
	"gender" : 1,
	"averageMonthlyExpenses" : 4036.478723404255,
	"total" : 188
}
```
{% endtab %}
{% endtabs %}

{% hint style="info" %}
The LINQ query supports the `Math.Ceiling` method and can produce an average of `Int32` type.
{% endhint %}

