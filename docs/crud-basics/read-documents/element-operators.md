# Element Operators

## Overview

MongoDB providers two element query operators that helps you find documents based on a field's existence or type. In other words, you can match documents based on whether a field **exists** or in case it does exist, based on its **type**. The two element operators presented on this section are:

| Operator | Description |
| :--- | :--- |
| **Exists** | Matches documents when the specified field exists |
| **Type** | Matches documents when a field is of the specified type |

![](../../.gitbook/assets/element.png)



## _Exists_ operator - _$exists_

The _$exists_ operator matches the documents that contain the field, even its value is _null_. 

{% tabs %}
{% tab title="Syntax" %}
```csharp
Builders<T>.Filter
    .Exists(doc => doc.<field>, <true || false>)
```
{% endtab %}
{% endtabs %}

The filter definition being created with the `Exists`method on a specific field, matches the documents which contain the field event if its value is _null_.

The sample uses the _Exists_ operator to find `Order` documents that have assigned a _lot number_. The _LotNumber_ is a nullable int property in the `Order`class.

{% tabs %}
{% tab title="C\#" %}
{% code title="ElementOperators.cs" %}
```csharp
var collection = database
            .GetCollection<Order>(Constants.InvoicesCollection);

// find all orders having a lotnumber
var lotNumberFilter = Builders<Order>.Filter
            .Exists(o => o.LotNumber, exists:true);

var ordersWithLotNumber = await collection
            .Find(lotNumberFilter).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database
    .GetCollection<BsonDocument>(Constants.InvoicesCollection);

var bsonLotNumberFilter = Builders<BsonDocument>.Filter
    .Exists("lotNumber", exists: true);

var bsonOrdersWithLotNumber = await collection
    .Find(lotNumberFilter).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.invoices.find({ lotNumber: { $exists: true } })

--------------------------- 
        
// sample matched document
{
	"_id" : 8,
	"item" : "Ergonomic Frozen Pants",
	"quantity" : 8,
	"lotNumber" : 54, // matched here
	"shipmentDetails" : {
		"shipAddress" : "018 Ortiz Green, Kennytown, Pitcairn Islands",
		"city" : "Abshireville",
		"country" : "Suriname",
		"contactName" : "Jonas Kertzmann",
		"contactPhone" : "(310) 890-1795"
	}
}
```
{% endtab %}

{% tab title="Order" %}
```csharp
public class Order
{
    [BsonId]
    public int OrderId { get; set; }
    public string Item { get; set; }
    public int Quantity { get; set; }

    [BsonIgnoreIfDefault]
    public int? LotNumber { get; set; }

    public ShipmentDetails ShipmentDetails { get; set; }
}
```
{% endtab %}
{% endtabs %}

{% hint style="info" %}
_LotNumber_ property has the `[BsonIgnoreIfDefault]` attribute assigned. This ensures that when a document having null _LotNumber_ is inserted in the collection, this field will be totally ignored, meaning the document will not contain a _lotNumber_ field.

On the contrary, if you omit this attribute and try to insert a document with null LotNumber, the field will be added with a _null_ value as follow.

```javascript
{
	"_id" : 0,
	"item" : "Handcrafted Steel Salad",
	"quantity" : 9,
	"lotNumber" : null, // nullable not ignored
	"shipmentDetails" : {
		"shipAddress" : "323 Edna Mission",
		"city" : "Jaquelineberg",
		"country" : "Moldova",
		"contactName" : "Eve Legros",
		"contactPhone" : "420.498.4974 x12459"
	}
}
```
{% endhint %}

## Type operator - _$type_

The _Type_ _operator_ matches documents where the field's value is an instance of a [BSON](https://docs.mongodb.com/manual/reference/glossary/#term-bson) [type](https://docs.mongodb.com/manual/reference/bson-types/).

{% tabs %}
{% tab title="Syntax" %}
```csharp
Builders<T>.Filter.Type(doc => doc.<field>, BsonType type)
```
{% endtab %}
{% endtabs %}

Use this operator when you need to ensure that a document's field has \(or hasn't\) been assigned with a specific value type.

The sample uses the _Type_ operator to find all orders that have been shipped by checking that the `ShippedDate`property has been assigned with a `DateTime` value.

{% tabs %}
{% tab title="C\#" %}
{% code title="ElementOperators.cs" %}
```csharp
var collection = database
    .GetCollection<User>(Constants.InvoicesCollection);

// find documents with shippedDate assigned a DateTime value
var typeFilter = Builders<Order>.Filter
    .Type(o => o.ShipmentDetails.ShippedDate, BsonType.DateTime);

var shippedOrders = await collection
    .Find(typeFilter).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database
    .GetCollection<BsonDocument>(Constants.InvoicesCollection);

var bsonMaleFilter = Builders<BsonDocument>.Filter
    .Eq("gender", Gender.Male);
    
var bsonDoctorFilter = Builders<BsonDocument>.Filter
    .Eq("profession", "Doctor");
    
var bsonMaleDoctorsFilter = Builders<BsonDocument>.Filter
    .And(bsonMaleFilter, bsonDoctorFilter);
    
var bsonMaleDoctors = await bsonCollection
    .Find(bsonMaleDoctorsFilter).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.invoices
	.find({"shipmentDetails.shippedDate" : { $type: 9 }})

db.invoices.find({"shipmentDetails.shippedDate" : { $type: "date" }})
// type 9 is the integer identifier for Date types
// https://docs.mongodb.com/manual/reference/bson-types/
--------------------------- 
        
// sample matched document
{
	"_id" : 0,
	"item" : "Generic Metal Cheese",
	"quantity" : 9,
	"shipmentDetails" : {
		"shippedDate" : ISODate("2019-07-06T01:53:49.804+03:00"), // matched here
		"shipAddress" : "7870 Shannon Mills, West Theodoreview, Palau",
		"city" : "Swaniawskimouth",
		"country" : "Guadeloupe",
		"contactName" : "Hermina Boyer",
		"contactPhone" : "495-231-3113"
	}
}
```
{% endtab %}

{% tab title="Models" %}
```csharp
public class Order
{
    [BsonId]
    public int OrderId { get; set; }
    public string Item { get; set; }
    public int Quantity { get; set; }

    [BsonIgnoreIfDefault]
    public int? LotNumber { get; set; }

    public ShipmentDetails ShipmentDetails { get; set; }
}

public class ShipmentDetails
{
    [BsonIgnoreIfDefault]
    public DateTime? ShippedDate { get; set; }
    public string ShipAddress { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string ContactName { get; set; }
    public string ContactPhone { get; set; }
}
```
{% endtab %}
{% endtabs %}

### Query for NULL values

Assuming you have a property that might get assigned with `NULL`value, you can find these documents by applying a filter on `BsonType.Null` types on that property.

The sample finds all documents with `NULL` shipment's contact phone number.

{% tabs %}
{% tab title="C\#" %}
{% code title="ElementOperators.cs" %}
```csharp
var collection = database
     .GetCollection<Order>(Constants.InvoicesCollection);

// search for null contact phone numbers
// the field does exists, but has null value
var nullContactPhoneFilter = Builders<Order>.Filter
     .Type(o => o.ShipmentDetails.ContactPhone, BsonType.Null);
            
var nullContactPhoneOrders = await collection
     .Find(nullContactPhoneFilter).ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Bson" %}
```csharp
var bsonCollection = database
      .GetCollection<BsonDocument>(Constants.InvoicesCollection);

var bsonNullContactPhoneFilter = Builders<BsonDocument>.Filter
      .Type("shipmentDetails.contactPhone", BsonType.Null);

var bsonNullContactPhoneOrders = await bsonCollection
      .Find(bsonNullContactPhoneFilter).ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.invoices.find({"shipmentDetails.contactPhone" : { $type: 10 }})

db.invoices.find({"shipmentDetails.contactPhone" : { $type: "null" }})
// type 10 is the integer identifier for null types
// https://docs.mongodb.com/manual/reference/bson-types/
--------------------------- 
        
// sample matched document
{
	"_id" : 7,
	"item" : "Small Steel Bacon",
	"quantity" : 9,
	"lotNumber" : 32,
	"shipmentDetails" : {
		"shippedDate" : ISODate("2019-10-16T17:39:46.584+03:00"),
		"shipAddress" : "15900 Pouros Turnpike",
		"city" : "Beckerfort",
		"country" : "Israel",
		"contactName" : "Shayna Steuber",
		"contactPhone" : null // matched here
	}
}
```
{% endtab %}

{% tab title="Models" %}
```csharp
public class Order
{
    [BsonId]
    public int OrderId { get; set; }
    public string Item { get; set; }
    public int Quantity { get; set; }

    [BsonIgnoreIfDefault]
    public int? LotNumber { get; set; }

    public ShipmentDetails ShipmentDetails { get; set; }
}

public class ShipmentDetails
{
    [BsonIgnoreIfDefault]
    public DateTime? ShippedDate { get; set; }
    public string ShipAddress { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string ContactName { get; set; }
    public string ContactPhone { get; set; }
}
```
{% endtab %}
{% endtabs %}

### 

