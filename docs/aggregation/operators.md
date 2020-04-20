# ✨ Operators

## _Slice_ operator - _$slice_

The _$slice_ operator is used to return a subset of an **array**. The `Enumerable.Take` method can be used to create a _$slice_ operation on a array field.

| Method call | Description |
| :--- | :--- |
| **`Take(1)`** | Returns the **first** element |
| **`Take(N)`** | Returns the **first N** elements |
| **`Take(-1)`** | Returns the **last** element |
| **`Take(-N)`** | Returns the **last N** elements |

### Get first N elements

The sample returns `Traveler` documents but only include the **first** element of their _VisitedCountries_ array field. 

{% tabs %}
{% tab title="C\#" %}
{% code title="Slice.cs" %}
```csharp
var travelersQueryableCollection = tripsDatabase
    .GetCollection<Traveler>(travelersCollectionName)
    .AsQueryable();

var sliceQuery = from t in travelersQueryableCollection
    select new {
        t.Name, visitedCountries = 
        t.VisitedCountries.Take(1) // slice here
    };

var sliceQueryResults = await sliceQuery.ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Shell" %}
```javascript
db.travelers.aggregate()
.project(
    {   
        name: 1, 
        visitedCountries : { 
            $slice: ["$visitedCountries", 1] 
        }
    })

-------------------

// sample result
{
	"_id" : ObjectId("5e9d705b45359358b426065f"),
	"name" : "Leopoldo Lueilwitz",
	"visitedCountries" : [ // only one item
		{
			"name" : "Malta",
			"timesVisited" : 9,
			"lastDateVisited" : ISODate("2017-12-19T21:22:35.607+02:00"),
			"coordinates" : {
				"latitude" : 79.2858,
				"longitude" : 13.7049
			}
		}
	]
}
```
{% endtab %}

{% tab title="Traveler" %}
```csharp
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

The same result can be achieved using a `ProjectionDefinition`.

```csharp
var sliceProjection = Builders<Traveler>.Projection
    .Expression(u =>
    new
    {
        name = u.Name,
        visitedCountries = u.VisitedCountries.Take(1)
    });

var sliceProjectionResults = await travelersCollection
    .Find(Builders<Traveler>.Filter.Empty)
    .Project(sliceProjection)
    .ToListAsync();
```

### Get last _N_ elements

The following sample returns the Traveler documents but only **the last 2** visited countries included.

{% tabs %}
{% tab title="C\#" %}
{% code title="Slice.cs" %}
```csharp
var sliceQueryTwoLastCountries = 
   from t in travelersQueryableCollection
   select new 
   { 
      t.Name, 
      visitedCountries = t.VisitedCountries.Take(-2) 
   };

var sliceQueryTwoLastCountriesResults = 
   await sliceQueryTwoLastCountries
   .ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Shell" %}
```javascript
db.travelers
  .aggregate()
  .project(
  { 
    name: 1, 
    visitedCountries : 
        { $slice: ["$visitedCountries", -2] } 
  })
  
-----------------------

// sample result

{
	"_id" : ObjectId("5e9d96f37db33d7ec8aed9ee"),
	"name" : "Emmitt Wuckert",
	"visitedCountries" : [ // last two
		{
			"name" : "Greece",
			"timesVisited" : 1,
			"lastDateVisited" : ISODate("2019-05-30T15:26:04.146+03:00"),
			"coordinates" : {
				"latitude" : 23.1572,
				"longitude" : 36.6096
			}
		},
		{
			"name" : "Malta",
			"timesVisited" : 5,
			"lastDateVisited" : ISODate("2017-12-08T05:52:58.182+02:00"),
			"coordinates" : {
				"latitude" : -75.3477,
				"longitude" : 97.188
			}
		}
	]
}
```
{% endtab %}

{% tab title="Traveler" %}
```csharp
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

## _Filter_ operator - _$filter_

The _equal operator_ is used to match documents having a field value equal to a specific value. You can use it for both top level and embedded documents.

{% tabs %}
{% tab title="Syntax" %}
```csharp
Builders<T>.Filter.Eq(doc => doc.<field>, <value>)
```
{% endtab %}
{% endtabs %}

The sample uses an _equal_ operator to find all documents that have the _profession_ field _\(top level field\)_ equal to "_Pilot_".

{% tabs %}
{% tab title="C\#" %}
{% code title="ComparisonOperators.cs" %}
```csharp
var collection = database.GetCollection<User>(collectionName);

// Case sensitive!
var equalPilotsFilter = Builders<User>.Filter
    .Eq(u => u.Profession, "Pilot");

var pilots = await collection
    .Find(equalPilotsFilter).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database.GetCollection<BsonDocument>(collectionName);

// Case sensitive matters!  
var bsonEqualPilotsFilter = Builders<BsonDocument>.Filter
    .Eq("profession", "Pilot");
    
var bsonPilots = await bsonCollection
    .Find(bsonEqualPilotsFilter).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.find({profession: { $eq: "Pilot"}})
// or..
db.users.find({profession: "Pilot"})

--------------------

// sample matched document
{
	"_id" : ObjectId("5e91e3ba3c1ba62570a67b97"),
	"gender" : 0,
	"firstName" : "Gilbert",
	"lastName" : "Beer",
	"userName" : "Gilbert43",
	"avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/lingeswaran/128.jpg",
	"email" : "Gilbert_Beer@yahoo.com",
	"dateOfBirth" : ISODate("1950-06-07T20:53:27.758+02:00"),
	"address" : {
		"street" : "3952 Felicita Garden",
		"suite" : "Suite 048",
		"city" : "North Pasqualefort",
		"state" : "Kansas",
		"zipCode" : "56191",
		"geo" : {
			"lat" : -0.8177,
			"lng" : -154.6886
		}
	},
	"phone" : "(973) 473-1826 x2746",
	"website" : "virgie.net",
	"company" : {
		"name" : "Quigley, Mitchell and McGlynn",
		"catchPhrase" : "Multi-layered holistic moratorium",
		"bs" : "enable front-end markets"
	},
	"salary" : 3028,
	"monthlyExpenses" : 3080,
	"favoriteSports" : [
		"Cycling",
		"MMA",
		"Boxing",
		"Handball",
		"Snooker",
		"American Football",
		"Volleyball",
		"Water Polo",
		"Beach Volleyball",
		"Ice Hockey",
		"Motor Sport",
		"Tennis",
		"Formula 1"
	],
	"profession" : "Pilot" // matched here
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

