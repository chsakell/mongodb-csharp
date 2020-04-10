# Ordered insert

## Ordered behavior

When adding multiple documents using the `InsertMany` method and if an error occurs with one of the documents to be added, by default MongoDB will return an error without processing the remain documents in the array. For example, if you try to insert 3 documents and the 2nd violates a unique index key, then the 1st one will be added in the collection but 2nd and 3rd won't. Let's see it in action.

{% tabs %}
{% tab title="C\#" %}
{% code title="OrderedInsert.cs" %}
```csharp
var sportsCollection = bettingDatabase.GetCollection<Sport>("sports");

// Sport title is the identifier _id field
var sports = new List<Sport>
{
    new Sport { Title = "Soccer", TotalEvents = 100 },
    new Sport { Title = "Basketball", TotalEvents = 50 },
    new Sport { Title = "Tennis", TotalEvents = 60 },
};

// Insert 3 documents
await sportsCollection.InsertManyAsync(sports);

// Now try to add 3 more
// The 2nd though 
var sportsToAdd = new List<Sport>
{
    new Sport { Title = "Volleyball", TotalEvents = 12 },
    
    // This should cause an error and stop further processing
    new Sport { Title = "Basketball", TotalEvents = 44 }, 
    
    // This will never be inserted
    new Sport { Title = "Formula 1", TotalEvents = 67 },
};

try
{
    await sportsCollection.InsertManyAsync(sportsToAdd);
}
catch (MongoBulkWriteException e)
{
    Utils.Log(e.Message);
}
```
{% endcode %}
{% endtab %}

{% tab title="Sport" %}
```csharp
public class Sport
{
    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    public string Title { get; set; }
    
    public int TotalEvents { get; set; }
}
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.sports.insertMany([
 { _id: "Soccer", totalEvents: 100  },
 { _id: "Basketball", totalEvents: 50  },
 { _id: "Tennis", totalEvents: 60  }
])


db.sports.insertMany([
 { _id: "Volleyball", totalEvents: 12  },
 { _id: "Basketball", totalEvents: 44  },
 { _id: "Formula 1", totalEvents: 67  }
])
```
{% endtab %}

{% tab title="Result" %}
```javascript
A bulk write operation resulted in one or more errors.
  E11000 duplicate key error collection: 
    Betting.sports index: _id_ dup key: { _id: "Basketball" }

// First InsertMany
{
	"acknowledged" : true,
	"insertedIds" : [
		"Soccer",
		"Basketball",
		"Tennis"
	]
}

// Second InsertMany
// Notice the nInserted: 1, meaning the 1st item was added
{
	"message" : "write error at item 1 in bulk operation",
	"stack" : "script:1:11",
	"name" : "BulkWriteError",
	"nInserted" : 1,
	"nUpserted" : 0,
	"nMatched" : 0,
	"nModified" : 0,
	"nRemoved" : 0
}

```
{% endtab %}
{% endtabs %}

{% hint style="info" %}
 Ordered insert **doesn't rollback** on failure but doesn't continue either
{% endhint %}

## Unordered behavior

You can configure your multiple documents insertions, to continue even if when some of the documents cause errors, by informing MongoDB not to use the default _ordered insert_ behavior. You can do this by passing an `InsertManyOptions` argument in the `InsertMany` method and setting **`IsOrdered = false`**.

{% tabs %}
{% tab title="C\#" %}
{% code title="OrderedInsert.cs" %}
```csharp
var sportsCollection = bettingDatabase.GetCollection<Sport>("sports");

// Sport title is the identifier _id field
var sports = new List<Sport>
{
    new Sport { Title = "Soccer", TotalEvents = 100 },
    new Sport { Title = "Basketball", TotalEvents = 50 },
    new Sport { Title = "Tennis", TotalEvents = 60 },
};

// Insert 3 documents
await sportsCollection.InsertManyAsync(sports);

var sportsToAddWithRollback = new List<Sport>
{
    new Sport { Title = "Volleyball", TotalEvents = 12 },
    
    // This should cause an error
    new Sport { Title = "Basketball", TotalEvents = 11 }, 
    
    // But this will be inserted as well
    new Sport { Title = "Baseball", TotalEvents = 44 },
    
    // This should cause an error
    new Sport { Title = "Tennis", TotalEvents = 67 }, 
    
    // But this will be inserted as well
    new Sport { Title = "Moto GP", TotalEvents = 12 } 
};

// Switch off ordered insert
await sportsCollection.InsertManyAsync(sportsToAddWithRollback, 
    new InsertManyOptions
    { 
        IsOrdered = false
    });
```
{% endcode %}
{% endtab %}

{% tab title="Shell" %}
```javascript
db.sports.insertMany([
 { _id: "Soccer", totalEvents: 100  },
 { _id: "Basketball", totalEvents: 50  },
 { _id: "Tennis", totalEvents: 60  }
])


db.sports.insertMany([
 { _id: "Volleyball", totalEvents: 12  },
 { _id: "Basketball", totalEvents: 11  }, // dup key
 { _id: "Baseball", totalEvents: 44  },
 { _id: "Tennis", totalEvents: 67  }, // dup key
 { _id: "Moto GP", totalEvents: 67  }
], { ordered : false } )
```
{% endtab %}

{% tab title="Results" %}
```javascript
A bulk write operation resulted in one or more errors.
  E11000 duplicate key error collection: 
    Betting.sports index: _id_ dup key: { _id: "Basketball" }
  E11000 duplicate key error collection: 
    Betting.sports index: _id_ dup key: { _id: "Tennis" }
  
// First insert
  {
	"acknowledged" : true,
	"insertedIds" : [
		"Soccer",
		"Basketball",
		"Tennis"
	]
}

// Second insert
{
	"message" : "2 write errors in bulk operation",
	"stack" : "script:1:11",
	"name" : "BulkWriteError",
	"nInserted" : 3,
	"nUpserted" : 0,
	"nMatched" : 0,
	"nModified" : 0,
	"nRemoved" : 0
}
```
{% endtab %}
{% endtabs %}

{% hint style="danger" %}
`InsertMany` will cause an `MongoBulkWriteException` exception with both ordered and unordered behavior
{% endhint %}

