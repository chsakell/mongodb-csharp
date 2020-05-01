# Evaluation Operators

## Overview

Evaluation operators are used to match documents based on some type of evaluation applied on the field values, for example,  find all documents that one of their field values matches a specified **Regex** pattern. The evaluation applied on this example is to check if a field's value matches the regex pattern given.

| Operator | Description |
| :--- | :--- |
| **Regex** | Find documents where their field values match a specified regular expression |
| **Text** | Searches the specified term in the fields indexed with a [text index](https://docs.mongodb.com/manual/core/index-text/) |

![Evaluation operators](../../.gitbook/assets/evaluation.png)

## _Regex_ operator _- $regex_

The _Regex_ operator is used when you want to match documents based on a regular expression. The regular expression is evaluated to one or more specified document fields. The **$regex** operator can be created via the driver by creating a `FilterDefinition` with the **`Regex`** function. Then use this filter definition in the `IMongoCollection<T>.Find` method.

{% tabs %}
{% tab title="Syntax" %}
```csharp
Builders<User>.Filter
    .Regex(doc => doc.<field>, 
        new BsonRegularExpression(<regex-expression>))
```
{% endtab %}
{% endtabs %}

The sample finds all `User` documents that their email field contains the _"gmail_" word. It does this by evaluating if there's a match between the email field and the regular expression, which by the way can be as complex as you wish.

{% tabs %}
{% tab title="C\#" %}
{% code title="EvaluationOperators.cs" %}
```csharp
var collection = database
    .GetCollection<User>(Constants.UsersCollection);

// create a regular expression
var gmailFilter = Builders<User>.Filter
    .Regex(u => u.Email, 
    new BsonRegularExpression("/gmail/"));
    
var gmailUsers = await collection
    .Find(gmailFilter).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database
    .GetCollection<BsonDocument>(Constants.UsersCollection);

// create a regular expression
var bsonGmailFilter = Builders<BsonDocument>
    .Filter.Regex("email", 
        new BsonRegularExpression("/gmail/"));

var bsonGmailUsers = await bsonCollection
    .Find(bsonGmailFilter).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.users.find({"email": { $regex : /gmail/ }})

--------------------

// sample result

{
   "_id": ObjectId("5eabe30dad09da9765850250"),
   "gender":1,
   "firstName":"Lee",
   "lastName":"Carroll",
   "userName":"Lee.Carroll33",
   "avatar":"https://s3.amazonaws.com/uifaces/faces/twitter/hasslunsford/128.jpg",
   "email":"Lee.Carroll@gmail.com", // matched here
   "dateOfBirth":"ISODate("1957-02-25T12:56:15.974Z")",
   "address":{
      "street":"2303 Braxton Gardens",
      "suite":"Apt. 313",
      "city":"Greenfeldermouth",
      "state":"Delaware",
      "zipCode":"21308-0693",
      "geo":{
         "lat":-79.4473,
         "lng":-150.5346
      }
   },
   "phone":"544-623-0642",
   "company":{
      "name":"Little and Sons",
      "catchPhrase":"Adaptive neutral capability",
      "bs":"drive integrated technologies"
   },
   "salary":"NumberDecimal("1275")",
   "monthlyExpenses":2201,
   "favoriteSports":[
      "Golf",
      "Tennis",
      "Water Polo",
      "American Football",
      "Formula 1",
      "Cycling",
      "Table Tennis",
      "Boxing",
      "Volleyball",
      "Basketball"
   ],
   "profession":"Pilot"
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

{% hint style="info" %}
MongoDB uses Perl compatible regular expressions version 8.42 with **UTF-8** support
{% endhint %}

## _Text_ operator - _$text_

Text operator **$text** is used along with a [text index](https://docs.mongodb.com/manual/core/index-text/) for searching a term among **string content** inside documents. The text index indexes string content as if it was an array of string values while stops and stems all words. This means that it words in sentences such as _"and"_, _"or"_, _"it"_ or _"is"_ are ignored.

You can create a text index using the C\# driver using an instance of `CreateIndexModel<T>`. The following snippet creates a text index on the _Name_ string field of the `Product` class.

{% tabs %}
{% tab title="C\#" %}
{% code title="EvaluationOperator" %}
```csharp
var productsCollection = database
                .GetCollection<Product>(Constants.ProductsCollection);
                
// create a text index on the name field                
productsCollection.Indexes
  .CreateOne(new CreateIndexModel<Product>
                (Builders<Product>.IndexKeys.Text(p => p.Name)));
```
{% endcode %}
{% endtab %}

{% tab title="Shell" %}
```javascript
db.products.createIndex( { "name": "text" } )

// Result
{
  "numIndexesBefore" : 1,
  "numIndexesAfter" : 2,
  "note" : "all indexes already exist",
  "ok" : 1
}

// sample document

{
  "_id" : ObjectId("5eac24240a037a3b4b412366"),
  "name" : "Awesome Concrete Shoes"
}
```
{% endtab %}

{% tab title="Product" %}
```csharp
public class Product
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
}
```
{% endtab %}
{% endtabs %}

{% hint style="info" %}
In case the index already exists, nothing will change
{% endhint %}

To create a search text query use the `FilterDefinitionBuilder.Text` method to build a filter definition.

{% tabs %}
{% tab title="C\#" %}
```csharp
Builders<Product>.Filter.Text(string term);
```
{% endtab %}
{% endtabs %}

The sample finds all `Product` documents that contain the term _"shirt"_.

{% tabs %}
{% tab title="C\#" %}
{% code title="EvaluationOperator" %}
```csharp
var productsCollection = database
                .GetCollection<Product>(Constants.ProductsCollection);

// create a text search filter          
var searchFilter = Builders<Product>
                .Filter.Text("shirt");

var shirtsProducts = await productsCollection
                .Find(searchFilter).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp

var productsBsonCollection = database
  .GetCollection<BsonDocument>(Constants.ProductsCollection);
  
var bsonSearchFilter = Builders<BsonDocument>
  .Filter.Text("shirt");
  
var bsonShirtsProducts = await productsBsonCollection
  .Find(bsonSearchFilter).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.products.find({ $text: { $search: 'shirt' } });

-------------------------
// sample results

{ 
  "_id" : ObjectId("5eac2a1a2d5d5e188996b00e"), 
  "name" : "Handcrafted Steel Shirt"
},
{ 
  "_id" : ObjectId("5eac2a1a2d5d5e188996affa"), 
  "name" : "Licensed Wooden Shirt" 
},
{ 
  "_id" : ObjectId("5eac2a1a2d5d5e188996afef"), 
  "name" : "Sleek Soft Shirt" 
}
```
{% endtab %}

{% tab title="Product" %}
```csharp
public class Product
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
}
```
{% endtab %}
{% endtabs %}

{% hint style="info" %}
**`FilterDefinitionBuilder.Text`** method optionally ****accepts a `TextSearchOptions` instance where you can control the case sensitivity on your search query.

```csharp
Builders<T>.Filter.Text(string term, 
    new TextSearchOptions() 
        { CaseSensitive = true });
```
{% endhint %}

