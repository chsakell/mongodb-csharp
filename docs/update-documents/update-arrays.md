# Arrays

## Push new item - _$push_

The _$push_ operator is used to append a new item to an array. To push a new item create an `UpdateDefinition<T>` by defining the array field and the new item to push.

{% tabs %}
{% tab title="Syntax" %}
```csharp
Builders<T>.Update
    .Push(doc => doc.<array-field>, <array-item>);
```
{% endtab %}
{% endtabs %}

The sample pushes a new `VisitedCountry` in the _VisitedCountries_ array of a `Traveler` document.

{% tabs %}
{% tab title="C\#" %}
{% code title="UpdatingArrays.cs" %}
```csharp
var collection = database.GetCollection<Traveler>(collectionName);

var firstTraveler = Builders<Traveler>.Filter.Empty;

// create a visited country item
var visitedCountry = RandomData
    .GenerateVisitedCountries(1).First();
visitedCountry.Name = "South Korea";
visitedCountry.TimesVisited = 5;
visitedCountry.LastDateVisited = DateTime.UtcNow.AddYears(5);

// create the update definition
var pushCountryDefinition = Builders<Traveler>
    .Update.Push(t => t.VisitedCountries, visitedCountry);
    
var addNewVisitedCountryResult = await collection
    .UpdateOneAsync(firstTraveler, pushCountryDefinition);
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

var bsonFirstUser = Builders<BsonDocument>.Filter.Empty;
var bsonVisitedCountry = RandomData.GenerateVisitedCountries(1).First();
visitedCountry.Name = "North Korea";
visitedCountry.TimesVisited = 5;
visitedCountry.LastDateVisited = DateTime.UtcNow.AddYears(5);

var bsonPushCountryDefinition = Builders<BsonDocument>.Update
    .Push("visitedCountries", visitedCountry.ToBsonDocument());

var bsonAddNewVisitedCountryResult = await bsonCollection
    .UpdateOneAsync(bsonFirstUser, bsonPushCountryDefinition);
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.travelers.updateOne( {}, { 
    $push: { visitedCountries: { 
        name: "My Own Country", 
        visitedTimes: 2, 
        lastDateVisited: ISODate("2018-07-11T10:00:23.454Z") } }
})

----------------------------

// sample result

{
	"acknowledged" : true,
	"matchedCount" : 1,
	"modifiedCount" : 1
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

