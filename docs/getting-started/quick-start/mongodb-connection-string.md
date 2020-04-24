# MongoDB connection

## Connection strings

To access a MongoDB instance, you need a **connection string**. Depending on your MongoDB deployment which can be either a _Standalone_, a _Replica Set_ or a _Shared Cluster,_ the connection string must be set accordingly.

The generic format for a MongoDB connection string is:

```csharp
mongodb://[username:password@]host1[:port1][,...hostN[:portN]][/[defaultauthdb][?options]]
```

### Standalone deployment

For a local standalone instance that doesn't require users to identify themselves, use the following connection string:

```csharp
mongodb://localhost:27017
```

If [access control](https://docs.mongodb.com/manual/tutorial/enable-authentication/) is enabled then the connection string should contain the user's **username** and **password** and optionally the database associated with the user's credentials. The latter can be set using the [authSource](https://docs.mongodb.com/manual/reference/connection-string/#urioption.authSource) options parameter.

```csharp
mongodb://chsakell:myPassword@mongodb0.example.com:27017/?authSource=persons
```

The above connection string will try to authenticate user _chsakell_ with password _myPassword_ against the _persons_ database.

## MongoClient

The easiest way to connect to a MongoDB standalone instance via C\# is to create an instance of `MongoClient`and pass the connection string to it's constructor:

{% tabs %}
{% tab title="MongoClient" %}
```csharp
// Create a mongodb client
var client = new MongoClient(DefaultConnectionString);

public static string DefaultConnectionString = 
  Program.Configuration.GetConnectionString("MongoDBConnection");
```
{% endtab %}

{% tab title="appsettings.json" %}
```javascript
{
    "ConnectionStrings": {
        "MongoDBConnection": "mongodb://localhost:27017"
     }
}
```
{% endtab %}
{% endtabs %}

