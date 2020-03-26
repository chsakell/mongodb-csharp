using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDb.Csharp.Samples.Expressions
{
    public class Filter : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Expressions_Filter;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Trips);
        }

        public async Task Run()
        {
            await FilterOperations();
        }

        private async Task FilterOperations()
        {
            var travelersCollectionName = "travelers";
            var tripsDatabase = Client.GetDatabase(Databases.Trips);
            var travelersCollection = tripsDatabase.GetCollection<Traveler>(travelersCollectionName);
            var travelersQueryableCollection = tripsDatabase.GetCollection<Traveler>(travelersCollectionName).AsQueryable();
            var travelersBsonCollection = tripsDatabase.GetCollection<BsonDocument>(travelersCollectionName);
            #region Prepare data

            await travelersCollection.InsertManyAsync(RandomData.GenerateTravelers(10, 5));

            #endregion

            #region Linq

            // $filter visited once

            var filterQuery = from t in travelersQueryableCollection
                              select new
                              {
                                  t.Name,
                                  visitedCountries = t.VisitedCountries.Where(c => c.TimesVisited == 1)
                              };

            var filterQueryResults = await filterQuery.ToListAsync();

            foreach (var result in filterQueryResults)
            {
                Utils.Log($"Age: {result.Name} - country visited once: {string.Join(',', result.visitedCountries.Select(c => c.Name))}");
            }

            var filterProjection = Builders<Traveler>.Projection.Expression(u =>
                new
                {
                    name = u.Name,
                    visitedCountries = u.VisitedCountries.Where(c => c.TimesVisited == 1)
                });

            var filterProjectionResults = await travelersCollection.Find(Builders<Traveler>.Filter.Empty)
                .Project(filterProjection)
                .ToListAsync();


            #endregion

            #region BsonDocument commands

            var bsonFilterPipeline = new[]
            {
                new BsonDocument()
                {
                    {
                        "$project", new BsonDocument(){
                            { "name", "$name" },
                            { "visitedCountries", new BsonDocument(){
                                    { "$filter", new BsonDocument(){
                                            { "input","$visitedCountries"},
                                                {"as","c"},
                                                {"cond", new BsonDocument(){
                                                    { "$eq", new BsonArray() {"$$c.timesVisited",1}}
                                                }}
                                            }
                                    }}
                            },
                            { "_id",0 }
                        }
                    }
                }
            };

            var bsonFilterPipelineResults = await travelersBsonCollection.Aggregate<BsonDocument>(bsonFilterPipeline).ToListAsync();

            foreach (var result in bsonFilterPipelineResults)
            {

            }

            #endregion


            #region Shell commands

#if false
                 db.travelers.aggregate([
                   {
                      "$project":{
                         "name":"$name",
                         "visitedCountries":{
                            "$filter":{
                               "input":"$visitedCountries",
                               "as":"c",
                               "cond":{
                                  "$eq":[
                                     "$$c.timesVisited",
                                     1
                                  ]
                               }
                            }
                         },
                         "_id":0
                      }
                   }
                ])
#endif

            #endregion

            #region Exercise

            // for each traveler project name and max visited times sorted by visited times among all travelers
            var exercise_1_linqQuery = travelersQueryableCollection
                .SelectMany(t => t.VisitedCountries, (t, v) => new
                {
                    name = t.Name,
                    timesVisited = v.TimesVisited
                })
                .GroupBy(q => q.name)
                .Select(g => new { name = g.Key, maxTimesVisited = g.Max(t => t.timesVisited) })
                .OrderBy(r => r.maxTimesVisited)
                .ThenBy(r => r.name);

            var exercise_1_linqQueryResults = await exercise_1_linqQuery.ToListAsync();

#if false

            db.travelers.aggregate([
                { $unwind: "$visitedCountries" },
                { $project: { _id: 1, name: 1, age: 1, timesVisited: "$visitedCountries.timesVisited" } },
                { $sort: { timesVisited: -1 } },
                { $group: { _id: "$_id", name: { $first: "$name" }, maxVisitedTimes: { $max: "$timesVisited" } } },
                { $sort: { maxVisitedTimes: 1, name: 1 } }
              ]).pretty();

#endif

            #endregion
        }
    }
}
