# 🚩 Unwind

## Array deconstruction

The **$unwind** operator is used to deconstruct array fields.  How deconstruction works ❓ For each array element a new document is produced having that element in the position of the field array. This means that applying the unwind operator on a single document's array field which contains 10 elements, produces 10 different documents having one of the array elements instead of the array field. It is possible to define if a new document will produced in case a document either doesn't contain the array field or it is empty.

{% tabs %}
{% tab title="Document" %}
```javascript
{
	"_id" : ObjectId("5e9afcd20f86e454c4518d8d"),
	"name" : "Margarett Lind",
	"age" : 59,
	"activities" : [
		"Golf",
		"Photography",
		"Hacking"
	]
}
```
{% endtab %}

{% tab title="$unwind" %}
```javascript
// $unwind on activities
db.trips.aggregate()
    .unwind("$activities")

// for each activity produces one document

/* 1 */
{
	"_id" : ObjectId("5e9afcd20f86e454c4518d8d"),
	"name" : "Margarett Lind",
	"age" : 59,
	"activities" : "Golf"
	
},

/* 2 */
{
	"_id" : ObjectId("5e9afcd20f86e454c4518d8d"),
	"name" : "Margarett Lind",
	"age" : 59,
	"activities" : "Photography"
},

/* 3 */
{
	"_id" : ObjectId("5e9afcd20f86e454c4518d8d"),
	"name" : "Margarett Lind",
	"age" : 59,
	"activities" : "Hacking"
}
```
{% endtab %}
{% endtabs %}

The following sample finds the distinct activities grouped by age for `Traveler` documents.

{% tabs %}
{% tab title="C\#" %}
{% code title="Unwind.cs" %}
```csharp
var travelersQueryableCollection = tripsDatabase
    .GetCollection<Traveler>(travelersCollectionName)
    .AsQueryable();

var linqQuery = travelersQueryableCollection
    .SelectMany(t => t.Activities, (t, a) => new
    {
        age = t.Age,
        activity = a
    })
    .GroupBy(q => q.age)
    .Select(g => new { age = g.Key, activities = 
        g.Select(a => a.activity).Distinct() })
    .OrderBy(r => r.age);

var linqQueryResults = await linqQuery.ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Shell" %}
```javascript
db.travelers.aggregate([
   {
      "$unwind":"$activities"
   },
   {
      "$project":{
         "age":"$age",
         "activity":"$activities",
         "_id":0
      }
   },
   {
      "$group":{
         "_id":"$age",
         "__agg0":{
            "$addToSet":"$activity"
         }
      }
   },
   {
      "$project":{
         "age":"$_id",
         "activities":"$__agg0",
         "_id":0
      }
   },
   {
      "$sort":{
         "age":1
      }
   }
])
```
{% endtab %}

{% tab title="Result" %}
```javascript
// sample results

/* 1 */
{
	"age" : 22,
	"activities" : [
		"Photography",
		"Road Touring"
	]
},

/* 2 */
{
	"age" : 25,
	"activities" : [
		"Photography",
		"Blogging",
		"Baking",
		"Collecting",
		"Hacking"
	]
},

/* 3 */
{
	"age" : 30,
	"activities" : [
		"Road Touring",
		"Climbing"
	]
},

/* 4 */
{
	"age" : 34,
	"activities" : [
		"Photography"
	]
}
```
{% endtab %}
{% endtabs %}

{% hint style="info" %}
### LINQ query explanation

* `SelectMany` on _Activities_ creates the _$unwind_ stage
* `new { }` creates the _$project_ stage
* `GroupBy` groups documents by age field
* `Select`creates another _$project_ stage where `Distinct` ensures activities will not contain duplicate values per group - it does that by using **$addToSet** operator in the _$group_ stage rather than a **$push**
{% endhint %}

