# Access databases

## List databases

To get information about all databases exist in a MongoDB server, use the `ListDatabases` method on a `MongoDB` client.

{% tabs %}
{% tab title="C\#" %}
{% code title="AccessDatabases.cs" %}
```csharp
var client = new MongoClient(Utils.DefaultConnectionString);
var databases = await Client.ListDatabasesAsync();

// iterate databases result

while (databases.MoveNext())
{
    var currentBatch = databases.Current;
    Utils.Log(currentBatch.AsEnumerable(), "List databases");
}
```
{% endcode %}
{% endtab %}

{% tab title="Shell" %}
```text
show dbs
show databases
```
{% endtab %}

{% tab title="Result" %}
```javascript
{
  "name": "admin",
  "sizeOnDisk": 40960.0,
  "empty": false
}
{
  "name": "config",
  "sizeOnDisk": 61440.0,
  "empty": false
}
{
  "name": "local",
  "sizeOnDisk": 73728.0,
  "empty": false
}
```
{% endtab %}
{% endtabs %}

In case you want to get the results instantly rather than iterating a `Task<IAsyncCursor>` result you can use the `ToListAsync`on a `IAsyncCursor` method. 

```csharp
var databases = await Client.ListDatabases().ToListAsync();

foreach (var database in databases)
{
    Utils.Log(database);
}
```

## Filter databases

When listing databases you can use options to **filter** the returned results by passing an instance of `ListDatabasesOptions` on the `ListDatabases` method.

{% tabs %}
{% tab title="C\#" %}
{% code title="AccessDatabases.cs" %}
```csharp
// Search only for the 'admin' database
// Return only the name
var adminDatabase = (await Client.ListDatabasesAsync(
    new ListDatabasesOptions
{
    Filter = Builders<BsonDocument>.Filter.Eq("name", "admin"),
    NameOnly = true
})).FirstOrDefault();
```
{% endcode %}
{% endtab %}

{% tab title="Check if database exists" %}
```csharp
// Create a filter on the database name
var dbNameFilter = Builders<BsonDocument>
                          .Filter.Eq("name", "fictionDb");
                          
var fictionDbExists = (await Client
     .ListDatabasesAsync(new ListDatabasesOptions() { 
                               Filter = dbNameFilter 
     })).FirstOrDefault() != null;
```
{% endtab %}
{% endtabs %}

An example of common filter would be to search all databases that exceeded a certain size on disk. The following query does exactly this.

{% code title="AccessDatabases.cs" %}
```csharp
// Search for all databases tha exceeded 60000 bytes
var highSizeDatabases = await Client.ListDatabases(new ListDatabasesOptions
{
    Filter = Builders<BsonDocument>.Filter.Gte("sizeOnDisk", 60000),
}).ToListAsync();
```
{% endcode %}

{% hint style="warning" %}
The above query won't work if you set **`NameOnly = true`** in the **`ListDatabasesOptions`**
{% endhint %}

## Get a database reference

The very first thing you need to do before accessing data in a database is to get a reference to that database using the `GetDatabase` method on a `MongoDB` client.

```csharp
IMongoDatabase adminDb = Client.GetDatabase("myDatabase");
```

`IMongoDatabase`  interface represents a database in MongoDB and expose its settings along with several methods that can give you access to its collections or another `IMongoDatabase` reference with different _read/write_ settings

