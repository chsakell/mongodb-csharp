using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDb.Csharp.Samples.Core;
using MongoDB.Driver;

namespace MongoDb.Csharp.Samples.Crud.Insert
{
    public class OrderInsert : RunnableSample, IRunnableSample
    {
        public bool Enabled => true;
        protected override Core.Samples Sample => Core.Samples.Crud_Insert_Ordered;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Core.Databases.Persons);
        }

        public async Task Run()
        {
            
        }
    }
}
