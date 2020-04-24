[![MongoDB C# docs](https://github.com/chsakell/mongodb-csharp/blob/master/src/MongoDb.Csharp.Samples/logo.png)](https://chsakell.gitbook.io/mongodb-csharp-docs)

# MongoDB for C# developers

>Repository for the [MongoDB C# docs](https://chsakell.gitbook.io/mongodb-csharp-docs) gitbook

> Build MongoDB queries using the [MongoDB C# driver](https://mongodb.github.io/mongo-csharp-driver/)

> Operators explanations with examples written in C# and node.js _(shell)_

> Advanced querying using LINQ

## Gitbook

> The docs contain code samples for building MongoDB queries using the MongoDB C# driver an a typed way rather than using `BsonDocument`.

## Example - $unwind operator

```csharp
var travelersQueryableCollection = tripsDatabase
    .GetCollection<Traveler>(Constants.TravelersCollection)
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

---

## License

[![License](http://img.shields.io/:license-mit-blue.svg?style=flat-square)](http://badges.mit-license.org)

- **[MIT license](https://github.com/chsakell/mongodb-csharp/blob/master/LICENSE)**
- Copyright 2020 © <a href="https://chsakell.com/" target="_blank">chsakell's blog</a>.
