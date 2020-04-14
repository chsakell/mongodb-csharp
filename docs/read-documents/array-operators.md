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

