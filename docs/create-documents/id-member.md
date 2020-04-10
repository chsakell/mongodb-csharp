# Id Member

## Unique Identifier

Each top level document in MongoDB contains an _id_ field that uniquely identifies documents in the collection.  This field can be mapped from/to a public property in _C\#_ models. By convention public members named _Id_, _id_ and _id_ will be used as the identifier. MongoDB will ensure to generate a value for your identifier field when inserting documents and deserialize it back your model member during reads.

{% hint style="warning" %}
The identifier member can be of any type but this doesn't mean that MongoDB will be able to automatically generate the value during insertions
{% endhint %}

## Supported MongoDB Id generators

You can use the following types for your Id identifier type:

### String

Let's assume that you have a Message class with a string Id public member:

{% tabs %}
{% tab title="Typed" %}
{% code title="IdMember.cs" %}
```csharp
var message = new Message {Text = "hello world"};

await messagesCollection.InsertOneAsync(message);

Utils.Log(message.ToBsonDocument());
```
{% endcode %}
{% endtab %}

{% tab title="Message - Faulty" %}
```csharp
public class Message
{
    public string Id { get; set; }
    public string Text { get; set; }
}
```
{% endtab %}

{% tab title="Result" %}
```javascript
// Id hasn't be recognized by MongoDB as an auto generated property
{
  "_id": null,
  "text": "hello world"
}
```
{% endtab %}
{% endtabs %}

{% hint style="danger" %}
When you want to use a string member as an auto generated MongoDB `Id` member, then just declaring it as a `string` isn't enough. You need to inform the driver which [IdGenerator](https://mongodb.github.io/mongo-csharp-driver/2.10/reference/bson/mapping/#id-generators) should use so that can understand if the Id member has been assigned a value or not. 

In case you don't do this, the next time you try to insert a document you will get a  **`E11000 duplicate key error collection`** error
{% endhint %}

The correct way to use a string Id member as identifier field is the following:

{% tabs %}
{% tab title="Message" %}
{% code title="IdMember.cs" %}
```csharp
public class Message
{
    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    public string Id { get; set; }
    public string Text { get; set; }
}
```
{% endcode %}
{% endtab %}

{% tab title="Message with custom identifier name" %}
```csharp
public class Message
{
    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    public string MyCustomId { get; set; }
    public string Text { get; set; }
}
```
{% endtab %}

{% tab title="Result" %}
```javascript
// _id field gets a generated string
{
  "_id": "5e8e0b39f618356c98297e0c",
  "text": "hello world"
}
```
{% endtab %}
{% endtabs %}

{% hint style="success" %}
In case you want to name your identifier member other than _Id, id or \_id_, make sure you use the `[BsonId]`attribute on it and MongoDB driver will do the rest
{% endhint %}

### Guid

Guid is one of the most used types for identifying documents and MongoDB fully support them. In fact, in case you use it along with a convention compatible Id member, you don't even need to specify the `IdGenerator`.

{% tabs %}
{% tab title="Message" %}
{% code title="IdMember.cs" %}
```csharp
public class Message
{
    public Guid Id { get; set; }
    public string Text { get; set; }
}
```
{% endcode %}
{% endtab %}

{% tab title="Message with custom identifier name" %}
```csharp
public class Message
{
    [BsonId(IdGenerator = typeof(GuidGenerator))]
    public Guid MyCustomId { get; set; }
    public string Text { get; set; }
}
```
{% endtab %}

{% tab title="Result" %}
```javascript
{
  "_id": "9bdc79ca-aafb-4a58-a97e-02f199f774b8",
  "text": "hello world"
}
```
{% endtab %}
{% endtabs %}

### COMB Guid

MongoDB supports generating Guid values using the COMB algorithm and you can use it as follow:

{% code title="IdMember.cs" %}
```csharp
public class Message
{
    [BsonId(IdGenerator = typeof(CombGuidGenerator))]
    public Guid MyCustomId { get; set; }
    public string Text { get; set; }
}
```
{% endcode %}

### ObjectId

MongoDB loves `ObjectId` ♥ which is a 12-byte special type for MongoDB which is fast to generate and contains:

* A **timestamp** value representing the _ObjectId'_s creations _\(4-byte\)_
* An auto-incrementing **counter** _\(3-byte\)_
* A **random value** _\(5-byte\)_

{% hint style="info" %}
In fact, if you try to insert a document in MongoDB in the shell and don't provide any value for the `_id` field, it will be saved as an `ObjectId` type
{% endhint %}

Convention based names with `ObjectId` type don't require the `[BsonId]` attribute.

{% tabs %}
{% tab title="Message" %}
{% code title="IdMember.cs" %}
```csharp
public class Message
{
    public ObjectId Id { get; set; }
    public string Text { get; set; }
}
```
{% endcode %}
{% endtab %}

{% tab title="Result" %}
```javascript
{
  "_id": "5e903c0012193c8ba86bc780",
  "text": "hello world"
}
```
{% endtab %}
{% endtabs %}

If you want to use a custom identifier name, just add the `[BsonId`\] attribute. Defining the `ObjectIdGenerator` is optional.

{% tabs %}
{% tab title="\[BsonId\]" %}
```csharp
public class Message
{
    [BsonId]
    public ObjectId CustomId { get; set; }
    public string Text { get; set; }
}
```
{% endtab %}

{% tab title="ObjectIdGenerator" %}
```csharp
public class Message
{
    [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
    public ObjectId CustomId { get; set; }
    public string Text { get; set; }
}
```
{% endtab %}
{% endtabs %}

### BsonObjectId

Everything applies for the `ObjectId` type applies for `BsonObjectId` as well.

{% tabs %}
{% tab title="Message" %}
{% code title="IdMember.cs" %}
```csharp
public class Message
{
    public BsonObjectId Id { get; set; }
    public string Text { get; set; }
}
```
{% endcode %}
{% endtab %}

{% tab title="Resut" %}
```csharp
// _id is actually ObjectId("5e8e19fd9ebaad96c89eb225")
// .ToString() will display only the string part
{
  "_id": "5e8e19fd9ebaad96c89eb225",
  "text": "hello world"
}
```
{% endtab %}
{% endtabs %}

If you want to use a custom identifier name, just add the `[BsonId`\] attribute. Defining the `BsonObjectIdGenerator` is optional.

{% tabs %}
{% tab title="\[BsonId\]" %}
```csharp
public class Message
{
    [BsonId]
    public BsonObjectId MyCustomId { get; set; }
    public string Text { get; set; }
}
```
{% endtab %}

{% tab title="BsonObjectIdGenerator" %}
```csharp
public class Message
{
    [BsonId(IdGenerator = typeof(BsonObjectIdGenerator))]
    public BsonObjectId MyCustomId { get; set; }
    public string Text { get; set; }
}
```
{% endtab %}
{% endtabs %}

### _NullIdChecker_ Generator

In case you want to ensure that the identifier field has been assigned a value before sending the query to MongoDB, you can use the NullIdChecker generator.

{% tabs %}
{% tab title="Message" %}
```csharp
public class Message
{
    [BsonId(IdGenerator = typeof(NullIdChecker))]
    public object Id { get; set; }
    public string Text { get; set; }
}
```
{% endtab %}

{% tab title="Exception" %}
```csharp
// The following will throw System.InvalidOperationException: Id cannot be null.
var message = new Message {Text = "hello world"};
await messagesCollection.InsertOneAsync(message);
```
{% endtab %}
{% endtabs %}

{% hint style="warning" %}
Trying to insert a document without assigning a value first for an identifier field with NullIdChecker generator will throw  **`System.InvalidOperationException: <field> cannot be null.`**
{% endhint %}

