# 🎯 Match

## Filter documents

The match stage is used to filter documents that will be used as input to the next stage in the pipeline. The filtered documents match one or more specified conditions.

One way to use the Match stage in a pipeline is to use the `Aggregate` method in a `IMongoCollection<T>` and chain the `Match` method.

{% tabs %}
{% tab title="Syntax" %}
```csharp
IMongoCollection<T>.Aggregate()
    .Match(FilterDefinition<T> filter)
```
{% endtab %}
{% endtabs %}

The following sample builds an aggregate with only one stage included. An aggregate with only one stage which happens to be a _Match_ stage is actually equal to a simple _find_ result with the same filter definitions. The sample filters `User` documents having salary between 3500 and 5000. 

{% tabs %}
{% tab title="C\#" %}
{% code title="Match.cs" %}
```csharp
var collection = database
    .GetCollection<User>(Constants.UsersCollection);

// creates an aggregate pipeline
var aggregate = collection.Aggregate()
    .Match(Builders<User>.Filter.Gte(u => u.Salary, 3500) &
           Builders<User>.Filter.Lte(u => u.Salary, 5000));

var highSalaryUsers = await aggregate.ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database
    .GetCollection<BsonDocument>(collectionName);

var bsonAggregate = bsonCollection.Aggregate()
    .Match(Builders<BsonDocument>.Filter.Gte("salary", 3500) &
           Builders<BsonDocument>.Filter.Lte("salary", 5000));

var bsonHighSalaryUsers = await bsonAggregate.ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.aggregate([
 { 
    $match : { $and: 
    	[ { salary: { $gte: 3500 }}, { salary: { $lte: 5000 }}  ] 
    } 
 }])
 
 ----------------------

// sample result
{
	"_id" : ObjectId("5e9acfb79d4cc53f6882b690"),
	"gender" : 0,
	"firstName" : "Dave",
	"lastName" : "Hansen",
	"userName" : "Dave_Hansen24",
	"avatar" : "https://s3.amazonaws.com/uifaces/faces/twitter/hsinyo23/128.jpg",
	"email" : "Dave77@yahoo.com",
	"dateOfBirth" : ISODate("1985-09-05T11:13:59.678+03:00"),
	"address" : {
		"street" : "36719 Amalia Unions",
		"suite" : "Suite 687",
		"city" : "Leannabury",
		"state" : "Alabama",
		"zipCode" : "04571",
		"geo" : {
			"lat" : 23.9471,
			"lng" : 42.4494
		}
	},
	"phone" : "(717) 904-7910 x8987",
	"website" : "caden.info",
	"company" : {
		"name" : "Prosacco and Sons",
		"catchPhrase" : "Automated responsive archive",
		"bs" : "morph granular content"
	},
	"salary" : 4510, // matched here
	"monthlyExpenses" : 5994,
	"favoriteSports" : [
		"MMA",
		"Water Polo",
		"Basketball",
		"Golf",
		"Cricket",
		"Tennis",
		"Volleyball",
		"Handball",
		"Motor Sport",
		"Table Tennis",
		"American Football",
		"Darts",
		"Snooker"
	],
	"profession" : "Firefighter"
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

### Match with Project

The following sample adds a _Match_ stage to filter `Traveler` documents that have visited "_Greece"_ exactly 3 times and projects the _Id, Name, Age_ and _VisitedCountries_ of each document. Notice that the match stage has been applied to array field elements using the [$elemMatch](../read-documents/array-operators.md#elemmatch-operator-usdelemmatch) operator.

{% tabs %}
{% tab title="C\#" %}
{% code title="Match.cs" %}
```csharp
var travelersCollection = travelersDatabase.
    GetCollection<Traveler>(Constants.TravelersCollection);

// match stage
var visitedGreeceExactly3Times = Builders<Traveler>.Filter
    .ElemMatch(t => t.VisitedCountries,
    country => country.Name == "Greece" && 
    country.TimesVisited == 3);

// projection stage
var simpleProjection = Builders<Traveler>.Projection
    .Include(t => t.Name)
    .Include(t => t.Age)
    .Include(t => t.VisitedCountries);

// build the pipeline
var greeceAggregate = travelersCollection
    .Aggregate()
    .Match(visitedGreeceExactly3Times) // match stage
    .Project(simpleProjection); // projection stage

var greeceVisited3Times = await greeceAggregate.ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var travelersBsonCollection = travelersDatabase
    .GetCollection<BsonDocument>(Constants.TravelersCollection);

var bsonSimpleProjection = Builders<BsonDocument>.Projection
    .Include("name")
    .Include("age")
    .Include("visitedCountries");

var bsonVisitedGreeceExactly3Times = Builders<BsonDocument>.Filter
    .ElemMatch<BsonValue>("visitedCountries", 
        new BsonDocument { 
            { "name", "Greece" }, 
            { "timesVisited", 3 } });

var bsonGreeceAggregate = travelersBsonCollection
    .Aggregate()
    .Match(bsonVisitedGreeceExactly3Times)
    .Project(bsonSimpleProjection);

var bsonGreeceVisited3Times = await bsonGreeceAggregate
                                .ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.trips.aggregate([
   {
      "$match":{
         "visitedCountries":{
            "$elemMatch":{
               "name":"Greece",
               "timesVisited":3
            }
         }
      }
   },
   {
      "$project":{
         "name":1,
         "age":1,
         "visitedCountries":1
      }
   }
])
 
 -------------------------------------

// sample result
{
	"_id" : ObjectId("5e9afcd20f86e454c4518e1d"),
	"name" : "Johanna Hahn",
	"age" : 31,
	"visitedCountries" : [
		{
			"name" : "Greece", // matched here
			"timesVisited" : 3, // matched here
			"lastDateVisited" : ISODate("2018-06-03T19:06:51.789+03:00"),
			"coordinates" : {
				"latitude" : 79.9786,
				"longitude" : -79.7394
			}
		},
		{
			"name" : "Iran",
			"timesVisited" : 1,
			"lastDateVisited" : ISODate("2016-01-06T12:32:07.230+02:00"),
			"coordinates" : {
				"latitude" : -62.9213,
				"longitude" : 37.0205
			}
		}
	]
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

