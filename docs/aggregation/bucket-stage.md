---
description: Documents distribution
---

# 📈 Bucket

## Bucket - $bucket

The **$bucket** operator is used to categorize documents into groups _\(buckets\)_ based on a specified expression and boundaries. It's used when you need to view the distribution of documents based on a specific criteria.

The sample finds the distribution among `Traveler` documents based on their age and for each bucket also finds the average age and the total documents. The boundaries \(ages\) is a fixed array **\[20, 32, 45, 60, 80\]** so the result presents the distribution of documents to these boundaries.

{% tabs %}
{% tab title="C\#" %}
{% code title="Bucket.cs" %}
```csharp
var travelersCollection = tripsDatabase
    .GetCollection<Traveler>(travelersCollectionName);

var aggregate = travelersCollection.Aggregate();

var bucket = aggregate.Bucket(
    t => t.Age,
    new[] { 20, 32, 45, 60, 80 },
    g => new
    {
        _id = default(int),
        averageAge = g.Average(e => e.Age),
        totalTravelers = g.Count()

    });

var bucketResults = await bucket.ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Shell" %}
```javascript
db.travelers.aggregate([
   {
      "$bucket":{
         "groupBy":"$age",
         "boundaries":[20,32, 45,60,80],
         "output":{
            "averageAge":{
               "$avg":"$age"
            },
            "totalTravelers":{
               "$sum":1
            }
         }
      }
   }
])

// Result

/* 1 */
{
	"_id" : 20,
	"averageAge" : 26.444444444444443,
	"totalTravelers" : 9
},

/* 2 */
{
	"_id" : 32,
	"averageAge" : 36,
	"totalTravelers" : 14
},

/* 3 */
{
	"_id" : 45,
	"averageAge" : 54,
	"totalTravelers" : 9
},

/* 4 */
{
	"_id" : 60,
	"averageAge" : 64,
	"totalTravelers" : 8
}
```
{% endtab %}

{% tab title="Traveler" %}
```csharp
public class Traveler
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public List<string> Activities { get; set; }
    public List<VisitedCountry> VisitedCountries { get; set; }
}
```
{% endtab %}
{% endtabs %}

## BucketAuto - _$bucketAuto_

The **$bucketAuto** operator works similar to the **$bucket** except that it automatically determines the boundaries for you. The boundaries are created so that the results are evenly distributed to the specified number of buckets.

The sample distributes the documents based on their age to 4 buckets.

{% tabs %}
{% tab title="C\#" %}
{% code title="Bucket.cs" %}
```csharp
var travelersCollection = tripsDatabase
    .GetCollection<Traveler>(travelersCollectionName);

var aggregate = travelersCollection.Aggregate();

// try to create 4 buckets
var autoBucket = aggregate.BucketAuto(t => t.Age, 4);

var autoBucketResults = await autoBucket.ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Shell" %}
```javascript
db.travelers.aggregate([
    { "$bucketAuto" : 
        { "groupBy" : "$age", "buckets" : 4 } 
    }
])

// sample results
/* 1 */
{
	"_id" : {
		"min" : 23,
		"max" : 32
	},
	"count" : 11
},

/* 2 */
{
	"_id" : {
		"min" : 32,
		"max" : 48
	},
	"count" : 11
},

/* 3 */
{
	"_id" : {
		"min" : 48,
		"max" : 62
	},
	"count" : 10
},

/* 4 */
{
	"_id" : {
		"min" : 62,
		"max" : 68
	},
	"count" : 8
}
```
{% endtab %}

{% tab title="Traveler" %}
```csharp
public class Traveler
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public List<string> Activities { get; set; }
    public List<VisitedCountry> VisitedCountries { get; set; }
}
```
{% endtab %}
{% endtabs %}

