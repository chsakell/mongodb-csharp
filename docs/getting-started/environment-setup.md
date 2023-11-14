---
description: Installation requirements & source code download
---

# Environment setup

## MongoDB server installation

In case you want to run and debug the samples existing in these docs locally, your development machine should have at least the followings configured:

* **A MongoDB instance**: If you haven't any instance installed on your machine, you can download the [Community Edition](https://docs.mongodb.com/manual/administration/install-community/) which is free. Depending on your computer's OS, click the respective link and follow the instructions.
* **.NET Core SDK:** Download the latest .NET Core SDK from Microsoft's official [page](https://dotnet.microsoft.com/download).

{% hint style="success" %}
Both MongoDB and .NET Core are open source projects which means you can run the samples either in a MAC, Linux or Windows machine :clap:&#x20;
{% endhint %}

## :arrow\_down: Clone the repository

You can download the source code for the docs by either selecting _**Clone or download**_ in the repository's [page](https://github.com/chsakell/mongodb-csharp) or by running the following git command

```
git clone https://github.com/chsakell/mongodb-csharp.git
```

After doing so, don't forget to set the connection string to point your MongoDB instance in the _appsettings.json_ file.

```javascript
{
  "ConnectionStrings": {
    "MongoDBConnection": "mongodb://localhost:27017"
}
```
