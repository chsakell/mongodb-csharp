using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Read.Query
{
    public class EvaluationOperators : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Crud_Read_Query_EvaluationOperators;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Constants.SamplesDatabase);
        }

        public async Task Run()
        {
            await ElementOperatorsSamples();
        }

        private async Task ElementOperatorsSamples()
        {
            var database = Client.GetDatabase(Constants.SamplesDatabase);
            var usersCollection = database.GetCollection<User>(Constants.UsersCollection);
            var usersBsonCollection = database.GetCollection<BsonDocument>(Constants.UsersCollection);

            var productsCollection = database.GetCollection<Product>(Constants.ProductsCollection);
            var productsBsonCollection = database.GetCollection<BsonDocument>(Constants.ProductsCollection);

            #region Prepare data

            var users = RandomData.GenerateUsers(500);
            var products = RandomData.GenerateProducts(500);
            await usersCollection.InsertManyAsync(users);
            await productsCollection.InsertManyAsync(products);
            #endregion

            #region Typed classes commands

            #region regex

            var gmailFilter = Builders<User>.Filter.Regex(u => u.Email, new BsonRegularExpression("/gmail/"));
            var gmailUsers = await usersCollection.Find(gmailFilter).ToListAsync();
            Utils.Log($"{gmailUsers.Count} users found to have gmail acounts");

            #endregion

            #region expr

            #endregion

            #region text

            productsCollection.Indexes.CreateOne(new CreateIndexModel<Product>
                (Builders<Product>.IndexKeys.Text(p => p.Name)));

            var searchFilter = Builders<Product>.Filter.Text("shirt");
            var searchFilterQuery = searchFilter.Render(BsonSerializer.SerializerRegistry.GetSerializer<Product>(),
                BsonSerializer.SerializerRegistry);
            var shirtsProducts = await productsCollection.Find(searchFilter).ToListAsync();

            Utils.Log($"There are {shirtsProducts.Count} total shirt products");

            #endregion

            #endregion

            #region BsonDocument commands

            #region regex

            var bsonGmailFilter = Builders<BsonDocument>.Filter
                .Regex("email", new BsonRegularExpression("/gmail/"));

            var bsonGmailUsers = await usersBsonCollection.Find(bsonGmailFilter).ToListAsync();

            #endregion

            #region text

            var bsonSearchFilter = Builders<BsonDocument>.Filter.Text("shirt");
            var bsonShirtsProducts = await productsBsonCollection.Find(bsonSearchFilter).ToListAsync();

            #endregion

            #endregion

            #region Shell commands

#if false
        db.users.find({"email": { $regex : /gmail/ }})
#endif


            #endregion
        }
    }
}
