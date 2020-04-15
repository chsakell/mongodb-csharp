# Array operators

## Overview

There are many times when you want to match documents based on array field values. Luckily, MongoDB and the C\# driver provide the following 3 array query operators that help you build array based queries.

* **Size** _\(or Count\)_ operator
* **ElemMatch** operator
* **All** operator

{% hint style="info" %}
These operators may seem simple at first 😇 , but when combined with other MongoDB features such as **projection** or **unwind**, you will see that you can build quite complex queries! 💪 
{% endhint %}

![Array operators](../.gitbook/assets/arrays.png)



## _Size_ operator - _$size_

The _$size_ operator is applied on array fields and matches documents when an array has a specific number of elements. MongoDB C\# driver, doesn't contain a dedicated method for the _$size_ operator but can resolve it from the `Count`property or `Count()` method of `IEnumerable`types.

The following sample finds:

*  `Traveler` documents having _VisitedCountries_ array with exact 5 elements 
* `Traveler` documents having _VisitedCountries_ array with more than 10 elements 

{% tabs %}
{% tab title="C\#" %}
{% code title="ArrayOperators.cs" %}
```csharp
var collection = database.GetCollection<Traveler>(collectionName);

var fiveVisitedCountriesFilter = await collection
    .Find(t => t.VisitedCountries.Count == 5).ToListAsync();
    
var moreThan10VisitedCountries = await collection
    .Find(t => t.VisitedCountries.Count > 10).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

// exactly 5
var bsonFiveVisitedCountriesFilter = await bsonCollection.Find(
    new BsonDocument
    { 
        {"visitedCountries", new BsonDocument {{ "$size", 5 } }}
    }).ToListAsync();

// more than 10
var bsonMoreThan10VisitedCountries = await bsonCollection
    .Find(new BsonDocument
    {
        {"visitedCountries.10", new BsonDocument {{ "$exists", true } }}
    }).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
// exactly 5
db.travelers.find({ visitedCountries : { $size: 5 } })

// more than 10
db.travelers
	.find({ "visitedCountries.10" : { "$exists" : true } })

--------------------------- 
        
// sample matched document (5 visited countries)
{
	"_id" : ObjectId("5e9621f95cb0474d40845dab"),
	"name" : "Bobbie Murazik",
	"age" : 45,
	"activities" : [
		"Photography",
		"Hacking",
		"Wine tourism",
		"Wildlife watching"
	],
	"visitedCountries" : [
		{
			"name" : "Chad",
			"timesVisited" : 4,
			"lastDateVisited" : ISODate("2016-08-17T16:52:59.566+03:00"),
			"coordinates" : {
				"latitude" : -34.2869,
				"longitude" : 134.3729
			}
		},
		{
			"name" : "Syrian Arab Republic",
			"timesVisited" : 4,
			"lastDateVisited" : ISODate("2018-03-09T19:55:12.084+02:00"),
			"coordinates" : {
				"latitude" : -59.1648,
				"longitude" : -163.6818
			}
		},
		{
			"name" : "Guadeloupe",
			"timesVisited" : 6,
			"lastDateVisited" : ISODate("2018-11-02T02:23:55.274+02:00"),
			"coordinates" : {
				"latitude" : -23.8004,
				"longitude" : -169.4041
			}
		},
		{
			"name" : "Bhutan",
			"timesVisited" : 10,
			"lastDateVisited" : ISODate("2018-02-25T13:23:50.974+02:00"),
			"coordinates" : {
				"latitude" : 67.0804,
				"longitude" : -125.1589
			}
		},
		{
			"name" : "Somalia",
			"timesVisited" : 7,
			"lastDateVisited" : ISODate("2020-01-02T15:48:37.721+02:00"),
			"coordinates" : {
				"latitude" : 61.3348,
				"longitude" : -83.9703
			}
		}
	]
}
```
{% endtab %}

{% tab title="Models" %}
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

public class VisitedCountry
{
    public string Name { get; set; }
    public int TimesVisited { get; set; }
    public DateTime LastDateVisited { get; set; }
    public GeoLocation Coordinates { get; set; }
}

public class GeoLocation
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
```
{% endtab %}
{% endtabs %}

{% hint style="info" %}
Notice that using the `BsonDocument` approach, you need to write the exact query you would write in the shell. 

```javascript
db.travelers
	.find({ "visitedCountries.10" : { "$exists" : true } })
```

🧙♂ This uses the `<array>.<index>` notation to check if the array contains an element at _11th_ position, which would also mean that has more than 10 documents
{% endhint %}



## _ElemMatch_ operator - _$elemMatch_

The _$elemMatch_ operator is used to match elements inside array fields based on one or more criteria.

> **Syntax**: `Builders<T>.Filter.ElemMatch(doc => doc.<array-field>,<expressions>[])`

The sample filters `Traveler` documents that their _VisitedCountries_ array field contain a `VisitedCountry` element with name _Greece_ and _TimesVisited = 3_. 

{% tabs %}
{% tab title="C\#" %}
{% code title="ArrayOperators.cs" %}
```csharp
var collection = database.GetCollection<Traveler>(collectionName);

var visitedGreeceExactly3Times = Builders<Traveler>.Filter
    .ElemMatch(t => t.VisitedCountries,
    country => country.Name == "Greece" 
        && country.TimesVisited == 3);

var visitedGreeceExactly3TimesTravelers = await collection
    .Find(visitedGreeceExactly3Times).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

var bsonVisitedGreeceExactly3Times = Builders<BsonDocument>.Filter
    .ElemMatch<BsonValue>("visitedCountries", 
        new BsonDocument { { "name", "Greece" }, 
                            { "timesVisited", 3 } });

var bsonVisitedGreeceExactly3TimesTravelers = await bsonCollection
    .Find(bsonVisitedGreeceExactly3Times).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.travelers.find({
    visitedCountries: {
        $elemMatch: {
            name : "Greece",
            timesVisited: 3
        }
    }})
    
---------------------------
// sample result

{
	"_id" : ObjectId("5e96c32ad9d34468c0a772b2"),
	"name" : "Ari Hessel",
	"age" : 30,
	"activities" : [
		"Wine tourism",
		"Blogging",
		"Scuba diving",
		"Golf",
		"Photography"
	],
	"visitedCountries" : [
		{
			"name" : "Greece", // match here
			"timesVisited" : 3, // match here
			"lastDateVisited" : ISODate("2020-02-20T08:18:31.292+02:00"),
			"coordinates" : {
				"latitude" : 61.2458,
				"longitude" : -131.4814
			}
		},
		{
			"name" : "Italy",
			"timesVisited" : 1,
			"lastDateVisited" : ISODate("2018-11-26T02:42:05.395+02:00"),
			"coordinates" : {
				"latitude" : -17.3031,
				"longitude" : -171.7654
			}
		}
	]
}
```
{% endtab %}

{% tab title="Models" %}
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

public class VisitedCountry
{
    public string Name { get; set; }
    public int TimesVisited { get; set; }
    public DateTime LastDateVisited { get; set; }
    public GeoLocation Coordinates { get; set; }
}

public class GeoLocation
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
```
{% endtab %}
{% endtabs %}

{% hint style="danger" %}
You might be temped to match array elements using the **$and** operator as follow:

{% code title="ArrayOperators.cs" %}
```csharp
var greeceVisitedFilter = Builders<Traveler>.Filter
    .AnyEq("visitedCountries.name", "Greece");

var visitedTimesFilter = Builders<Traveler>.Filter
    .AnyEq("visitedCountries.timesVisited", 3);

// shell
db.travelers.find({$and: [ 
    {"visitedCountries.name" : "Greece"}, 
    {"visitedCountries.timesVisited":3}])
```
{% endcode %}

This is **wrong** because it doesn't apply the criteria on each array element at a time but in all elements. This means that it might match documents that indeed  contain a visited country with name "_Greece_" which hasn't _TimesVisited = 3_, but a document matched because it also contains another visited country, _e.g. Italy_ with _TimesVisited = 3_.
{% endhint %}



