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

## Push multiple items

To push multiple items in a array field use the `UpdateDefinitionBuilder<T>.PushEach` method. The method creates a _**`$push => $each`**_ operation and pushes all array items to the array field.

{% tabs %}
{% tab title="Syntax" %}
```csharp
Builders<Traveler>.Update
     .PushEach(t => t.<array-field>, 
     <array-items>)
```
{% endtab %}
{% endtabs %}

The sample adds 10 new `VisitedCountry` items in the _VisitedCountries_ array field.

{% tabs %}
{% tab title="C\#" %}
{% code title="UpdatingArrays.cs" %}
```csharp
var collection = database.GetCollection<Traveler>(collectionName);

var firstTraveler = Builders<Traveler>.Filter.Empty;

var newVisitedCountries = RandomData.GenerateVisitedCountries(10);

var pushCountriesDefinition = Builders<Traveler>.Update
    .PushEach(t => t.VisitedCountries, newVisitedCountries);

var addNewVisitedCountriesResult = await collection
    .UpdateOneAsync(firstTraveler, pushCountriesDefinition);
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

var bsonFirstUser = Builders<BsonDocument>.Filter.Empty;

var bsonNewVisitedCountries = RandomData.GenerateVisitedCountries(10);
var bsonPushCountriesDefinition = Builders<BsonDocument>.Update
    .PushEach("visitedCountries", bsonNewVisitedCountries);

var bsonAddNewVisitedCountries = await bsonCollection
    .UpdateOneAsync(bsonFirstUser, bsonPushCountriesDefinition);
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.travelers.updateOne({}, {
    $push: {
        visitedCountries: {
            $each: [
                {
                    "name": "South Korea",
                    "timesVisited": 5,
                    "lastDateVisited": ISODate("2025-04-21T19:27:38.700+03:00"),
                    "coordinates": {
                        "latitude": 4.2429,
                        "longitude": -148.3179
                    }
                },
                {
                    "name": "Mozambique",
                    "timesVisited": 6,
                    "lastDateVisited": ISODate("2017-01-20T14:42:27.786+02:00"),
                    "coordinates": {
                        "latitude": -45.9077,
                        "longitude": -121.6868
                    }
                }
            ]
        }
    }
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

## Pull items - _$pull_

To remove items that match a specified condition use the `Builders.Update.PullFilter` method.

{% tabs %}
{% tab title="Syntax" %}
```csharp
Builders<StoreItem>.Update
  .PullFilter(doc => doc.<array-field>,
                item => condition(item));
```
{% endtab %}
{% endtabs %}

The sample removes two **string** values _\("FIFA 20", "NBA 2K17"\)_ from the _PcGames_ string array field. The empty filter ensures to remove the items for all documents in the collection.

{% tabs %}
{% tab title="C\#" %}
{% code title="UpdatingArrays.cs" %}
```csharp
var storesCollection = genericDatabase
    .GetCollection<StoreItem>(storesCollectionName);

var storeEmptyFilter = Builders<StoreItem>.Filter.Empty;

// items to be removed
var removePcGames = new List<string> { "FIFA 20", "NBA 2K17" };

// create the definition
var pullPcGamesDefinition = Builders<StoreItem>
    .Update.PullFilter(s => s.PcGames,
                game => removePcGames.Contains(game));
                
var simplePullResult = await storesCollection
    .UpdateManyAsync(storeEmptyFilter, 
                pullPcGamesDefinition);
```
{% endcode %}
{% endtab %}

{% tab title="Data" %}
```csharp
// initial data in the db

/* 1 */
{
	"_id" : ObjectId("5e9f5fdb6b204f01d96a678b"),
	"pcGames" : [
		"Football Manager",
		"DOOM Eternal",
		"FIFA 20",
		"Grand Theft Auto",
		"NBA 2K17"
	],
	"xboxGames" : [
		"Forza Horizon",
		"Call of Duty",
		"Mortal Kombat",
		"Gears 5"
	]
},

/* 2 */
{
	"_id" : ObjectId("5e9f5fdb6b204f01d96a678c"),
	"pcGames" : [
		"Assassin's Creed",
		"Final Fantasy",
		"The Sims",
		"Football Manager",
		"FIFA 20"
	],
	"xboxGames" : [
		"Resident Evil",
		"Forza Motorsport",
		"Battlefield",
		"Halo 5 Guardians",
		"Mortal Kombat"
	]
}
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.stores.updateMany({}, {
    "$pull": {
        "pcGames": {
            "$in": [
                "FIFA 20",
                "NBA 2K17"
            ]
        }
    }
})

----------------------------

// final result

/* 1 */
{
	"_id" : ObjectId("5e9f5fdb6b204f01d96a678b"),
	"pcGames" : [
		"Football Manager",
		"DOOM Eternal",
		"Grand Theft Auto"
	],
	"xboxGames" : [
		"Forza Horizon",
		"Call of Duty",
		"Mortal Kombat",
		"Gears 5"
	]
},

/* 2 */
{
	"_id" : ObjectId("5e9f5fdb6b204f01d96a678c"),
	"pcGames" : [
		"Assassin's Creed",
		"Final Fantasy",
		"The Sims",
		"Football Manager"
	],
	"xboxGames" : [
		"Resident Evil",
		"Forza Motorsport",
		"Battlefield",
		"Halo 5 Guardians",
		"Mortal Kombat"
	]
}
```
{% endtab %}

{% tab title="Model" %}
```csharp
public class StoreItem
{
    public BsonObjectId Id { get; set; }

    [BsonIgnoreIfDefault]
    public List<string> PcGames { get; set; }
    [BsonIgnoreIfDefault]
    public List<string> XboxGames { get; set; }
}
```
{% endtab %}
{% endtabs %}

{% hint style="info" %}
The _PcGames_ array field doesn't have to contain both values. If any of the string arguments passed is contained, it will be pulled out from the array.
{% endhint %}

## Pull items from multiple arrays

To remove items from multiple array fields at the same time use the  Builders.Update .Combine method to combine 2 or more update definitions.

{% tabs %}
{% tab title="Syntax" %}
```csharp
Builders<StoreItem>.Update
    .Combine(UpdateDefinition[] definitions)
```
{% endtab %}
{% endtabs %}

The sample removes two string values from the _PcGames_ array field and one from the _XboxGames_.

{% tabs %}
{% tab title="C\#" %}
{% code title="UpdatingArrays.cs" %}
```csharp
var storesCollection = genericDatabase
    .GetCollection<StoreItem>(storesCollectionName);

var storeEmptyFilter = Builders<StoreItem>.Filter.Empty;

var removePcGames = new List<string> { "FIFA 20", "NBA 2K17" };
var removeXboxGames = new List<string> { "Mortal Kombat" };

// create the 1st pull definition
var pullPcGamesDefinition = Builders<StoreItem>
    .Update.PullFilter(s => s.PcGames,
        game => removePcGames.Contains(game));

// create the 2nd pull definition      
var pullXboxGamesDefinition = Builders<StoreItem>
    .Update.PullFilter(s => s.XboxGames,
        game => removeXboxGames.Contains(game));

// combine the definition
var pullCombined = Builders<StoreItem>.Update
    .Combine(pullPcGamesDefinition, pullXboxGamesDefinition);
    
var removeUpdateResult = await storesCollection
    .UpdateManyAsync(storeEmptyFilter, pullCombined);
```
{% endcode %}
{% endtab %}

{% tab title="Data" %}
```csharp
// initial data in the db

/* 1 */
{
	"_id" : ObjectId("5e9f5fdb6b204f01d96a678b"),
	"pcGames" : [
		"Football Manager",
		"DOOM Eternal",
		"FIFA 20",
		"Grand Theft Auto",
		"NBA 2K17"
	],
	"xboxGames" : [
		"Forza Horizon",
		"Call of Duty",
		"Mortal Kombat",
		"Gears 5"
	]
},

/* 2 */
{
	"_id" : ObjectId("5e9f5fdb6b204f01d96a678c"),
	"pcGames" : [
		"Assassin's Creed",
		"Final Fantasy",
		"The Sims",
		"Football Manager",
		"FIFA 20"
	],
	"xboxGames" : [
		"Resident Evil",
		"Forza Motorsport",
		"Battlefield",
		"Halo 5 Guardians",
		"Mortal Kombat"
	]
}
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.stores.updateMany({}, {
    "$pull": {
        "pcGames": {
            "$in": [
                "FIFA 20",
                "NBA 2K17"
            ]
        },
        "xboxGames" : {
					"$in" : [
						"Mortal Kombat"
					]
				}
    }
})

----------------------------

// final result

/* 1 */
{
	"_id" : ObjectId("5e9f62f94c1ea5e62f506597"),
	"pcGames" : [
		"Football Manager",
		"DOOM Eternal",
		"Grand Theft Auto"
	],
	"xboxGames" : [
		"Forza Horizon",
		"Call of Duty",
		"Gears 5"
	]
},

/* 2 */
{
	"_id" : ObjectId("5e9f62f94c1ea5e62f506598"),
	"pcGames" : [
		"Assassin's Creed",
		"Final Fantasy",
		"The Sims",
		"Football Manager"
	],
	"xboxGames" : [
		"Resident Evil",
		"Forza Motorsport",
		"Battlefield",
		"Halo 5 Guardians"
	]
}
```
{% endtab %}

{% tab title="Model" %}
```csharp
public class StoreItem
{
    public BsonObjectId Id { get; set; }

    [BsonIgnoreIfDefault]
    public List<string> PcGames { get; set; }
    [BsonIgnoreIfDefault]
    public List<string> XboxGames { get; set; }
}
```
{% endtab %}
{% endtabs %}

## Pull embedded items from array

To remove nested documents from  an array field create an `UpdateDefinition` with the specified condition to be applied on the documents to be removed. Assuming `T` is the type of the root document which contains an array field having `E` type of documents, the syntax is the following.

{% tabs %}
{% tab title="Syntax" %}
```csharp
var pullDefinition = 
        Builders<T>.Update
                .PullFilter(root => root.<array-field>,
                arrayDoc => condition(arrayDoc<field>));
```
{% endtab %}
{% endtabs %}

The sample removes `VisitedCountry` documents from the _VisitedCountries_ array field of `Traveler` documents based on the `VisitedCountry.TimesVisited` value.

{% tabs %}
{% tab title="C\#" %}
{% code title="UpdatingArrays.cs" %}
```csharp
var travelersCollection = tripsDatabase
    .GetCollection<Traveler>(travelerCollectionName);

// create a filter
var visited8Times = Builders<Traveler>
    .Filter.ElemMatch(t => t.VisitedCountries, 
    country =>  country.TimesVisited == 8);

var pullVisited8TimesDefinition = Builders<Traveler>.Update
    .PullFilter(t => t.VisitedCountries,
        country => country.TimesVisited == 8); // condition

var visited8TimesResult = await travelersCollection
    .UpdateManyAsync(visited8Times, pullVisited8TimesDefinition);
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp

var bsonVisited9Times = Builders<BsonDocument>.Filter
    .ElemMatch<BsonValue>("visitedCountries", 
        new BsonDocument { { "timesVisited", 9 } });

var bsonTotalDocVisited9Times = await bsonTravelersCollection
    .Find(bsonVisited9Times).CountDocumentsAsync();

var bsonPullVisited9TimesDefinition = Builders<BsonDocument>.Update
    .PullFilter<BsonValue>("visitedCountries",
        new BsonDocument { { "timesVisited", 9 } });

var bsonVisited9TimesResult = await bsonTravelersCollection
    .UpdateManyAsync(bsonVisited9Times, 
        bsonPullVisited9TimesDefinition);
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.travelers.updateMany(
    {
        "visitedCountries": {
            "$elemMatch": {
                "timesVisited": 8
            }
        },
    },
    {
        "$pull": {
            "visitedCountries": {
                "timesVisited": 8
            }
        }
    }
)

------------------------------

// sample result
{
	"acknowledged" : true,
	"matchedCount" : 10,
	"modifiedCount" : 10
}

// sample document
{
	"_id" : ObjectId("5ea073c55c436f374c4c48ae"),
	"name" : "Orville White",
	"age" : 55,
	"activities" : [
		"Golf",
		"Wine tourism",
		"Running",
		"Geocaching",
		"Snow-kiting",
		"Collecting",
		"Blogging"
	],
	"visitedCountries" : [ // docs with timesVisited = 8 removed
		{
			"name" : "Montenegro",
			"timesVisited" : 2,
			"lastDateVisited" : ISODate("2018-10-10T09:11:38.314+03:00"),
			"coordinates" : {
				"latitude" : 82.9026,
				"longitude" : 95.4488
			}
		},
		{
			"name" : "Ecuador",
			"timesVisited" : 9,
			"lastDateVisited" : ISODate("2018-01-24T11:30:48.835+02:00"),
			"coordinates" : {
				"latitude" : 80.4824,
				"longitude" : 179.1376
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
```
{% endtab %}
{% endtabs %}

