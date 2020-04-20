---
description: Skip - Limit
---

# ⏩ Pagination

## Skip & Limit - _$skip & $limit_

_Skip_ and _Limit_ operators are operators commonly used for pagination purposes. In most cases they are used together along with a _$sort_ operator.

| Operator | Description |
| :--- | :--- |
| Skip | Skips a specified number of documents and pass the result to the next stage |
| Limit | Limits the number of documents passed to the next state |

The sample creates a pagination result on `User` documents by skipping 3 documents and returning 3 documents at a time. The results are sorted by _DateOfBirth_.

{% tabs %}
{% tab title="C\#" %}
{% code title="Limit\_skip.cs" %}
```csharp
var usersCollection = personsDatabase
    .GetCollection<User>(usersCollectionName);

var topLevelProjection = Builders<User>.Projection
    .Exclude(u => u.Id)
    .Include(u => u.UserName)
    .Include(u => u.DateOfBirth);

var topLevelProjectionResults = await usersCollection
    .Find(Builders<User>.Filter.Empty)
    .Project(topLevelProjection)
    .SortBy(u => u.DateOfBirth)
    .Skip(skipSize)
    .Limit(limitSize)
    .ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var usersBsonCollection = personsDatabase
    .GetCollection<BsonDocument>(usersCollectionName);
    
var bsonTopLevelProjection = Builders<BsonDocument>
    .Projection
    .Exclude("_id")
    .Include("userName")
    .Include("dateOfBirth");

var bsonTopLevelProjectionResults = await usersBsonCollection
    .Find(Builders<BsonDocument>.Filter.Empty)
    .Project(bsonTopLevelProjection)
    .SortBy(doc => doc["dateOfBirth"])
    .Skip(skipSize)
    .Limit(limitSize)
    .ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.aggregate([
    { "$project" : { _id: 0, userName: 1, dateOfBirth: 1 } },
    { "$sort" : { dateOfBirth: 1 } },
    { "$skip":  3 },
    { "$limit": 3 }
])

----------------------

// sample results

/* 1 */
{
	"userName" : "Irene.OKon85",
	"dateOfBirth" : ISODate("1951-05-02T11:47:36.734+02:00")
},

/* 2 */
{
	"userName" : "Kristopher.Brown",
	"dateOfBirth" : ISODate("1951-12-15T13:41:54.000+02:00")
},

/* 3 */
{
	"userName" : "Whitney.Jones",
	"dateOfBirth" : ISODate("1952-08-14T07:04:49.589+03:00")
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

{% hint style="info" %}
To fetch the next page result just change the _skip_ size. The same query can also be built using **LINQ** as follow:

```csharp
var usersQueryableCollection = personsDatabase
    .GetCollection<User>(usersCollectionName)
    .AsQueryable();
    
var linqTopLevelResults = await usersQueryableCollection
    .Select(u => new { u.UserName, u.DateOfBirth })
    .OrderBy(u => u.DateOfBirth)
    .Skip(skipSize)
    .Take(limitSize)
    .ToListAsync();
```
{% endhint %}

## Paginate array field

Paginating an array field requires at least an extra **$unwind** stage to deconstruct the array. The sample creates a pagination on the _FavoriteSports_ of the first `User` document that contains more than 10 items in its favorite sports array field.

{% tabs %}
{% tab title="C\#" %}
{% code title="Limit\_skip.cs" %}
```csharp
var usersQueryableCollection = personsDatabase
    .GetCollection<User>(usersCollectionName)
    .AsQueryable();

var user = await usersCollection
    .Find(u => u.FavoriteSports.Count > 10)
    .FirstOrDefaultAsync();

var sliceQuery = usersQueryableCollection
    .Where(u => u.Id == user.Id)
    .SelectMany(u => u.FavoriteSports, (u, s) => new
    {
        id = u.Id,
        sport = s
    })
    .OrderBy(u => u.sport)
    .Skip(skipSize)
    .Take(limitSize)
    .GroupBy(q => q.id)
    .Select(g => new
    {
        id = g.Key,
        sports = g.Select(a => a.sport)
    });
```
{% endcode %}
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.aggregate([
   {
      "$match":{
         "_id":ObjectId("5e9d578781f29c3b1c22d733")
      }
   },
   {
      "$unwind":"$favoriteSports"
   },
   {
      "$project":{
         "id":"$_id",
         "sport":"$favoriteSports",
         "_id":0
      }
   },
   {
      "$sort":{
         "sport":1
      }
   },
   {
      "$skip":3
   },
   {
      "$limit":3
   },
   {
      "$group":{
         "_id":"$id",
         "__agg0":{
            "$push":"$sport"
         }
      }
   },
   {
      "$project":{
         "id":"$_id",
         "sports":"$__agg0",
         "_id":0
      }
   }
])

// alternative using the $slice operator
db.users.aggregate([
    { "$match" : { _id: ObjectId("5e9d578781f29c3b1c22d733") } },
    { "$unwind" : "$favoriteSports" },
    { "$sort" : { favoriteSports: 1 } },
    { "$group": { _id: '$_id', 'favoriteSports': 
       {$push: '$favoriteSports'} } },
    { "$project" : { _id: 0, sports: { 
       $slice: ["$favoriteSports", 3, 3] } } }
])

```
{% endtab %}

{% tab title="Result" %}
```javascript
// assuming the initial list in the document 
// is the following..

"favoriteSports" : [
		"Baseball",
		"Motor Sport",
		"Water Polo",
		"Cricket",
		"Basketball",
		"Moto GP",
		"Snooker",
		"Tennis",
		"Volleyball",
		"Soccer",
		"Table Tennis",
		"Beach Volleyball",
		"Formula 1",
		"Handball",
		"American Football",
		"Darts"
	]
	
// skip 0: limit 3 result
{
	"id" : ObjectId("5e9d578781f29c3b1c22d733"),
	"sports" : [
		"American Football",
		"Baseball",
		"Basketball"
	]
}

// skip 3: limit 3 result
{
	"id" : ObjectId("5e9d578781f29c3b1c22d733"),
	"sports" : [
		"Beach Volleyball",
		"Cricket",
		"Darts"
	]
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

{% hint style="info" %}
### LINQ explanation

1. `Where` clause creates a **$match** stage to match the user document
2. `SelectMany` creates an **$unwind** stage and deconstructs the _favoriteSports_ array. This will produce a new document for each favorite sport in the array
3. `new {}` creates a **$project** stage that passes only the \__id_ and each _sport_ element from the preview stage
4. `OrderBy` sorts the documents alphabetically based on the sports
5. `Skip/Take` paginate the documents which already sorted by the sports
6. `GroupBy/Select` group the documents together in order to create an array of the selected sports containing in the documents retrieved from the previous stage
{% endhint %}

