using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using UiPathTeam.MongoDB.Activities.Properties;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace UiPathTeam.MongoDB.Activities
{
    [LocalizedDisplayName("Query Document")]
    [LocalizedDescription("Query Document from Mongo DB")]
    public class QueryDoc : AsyncTaskCodeActivity<int>
    {
        [RequiredArgument]
        [LocalizedDisplayName("Database")]
        [LocalizedDescription("Name of database to query from")]
        [LocalizedCategory(nameof(Resources.Input))]
        public InArgument<string> Database { get; set; }

        [RequiredArgument]
        [LocalizedDisplayName("Collection")]
        [LocalizedDescription("Name of collection to query from")]
        [LocalizedCategory(nameof(Resources.Input))]
        public InArgument<string> Collection { get; set; }

        [LocalizedDisplayName("Filter Criteria")]
        [LocalizedDescription("Criteria to filter output")]
        [LocalizedCategory(nameof(Resources.Input))]
        public InArgument<FilterDefinition<BsonDocument>> Filter { get; set; }

        [LocalizedDisplayName("Results")]
        [LocalizedDescription("Query results")]
        [LocalizedCategory(nameof(Resources.Output))]
        public OutArgument<string[]> Results { get; set; }

        public QueryDoc()
        {
            Constraints.Add(ParentConstraint.CheckThatParentsAreOfType<QueryDoc, ParentScope>("Activity is valid only inside Mongo Database Scope"));
        }

        /// <inheritdoc />
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);

            if (Database == null) metadata.AddValidationError(string.Format(Resources.MetadataValidationError, nameof(Database)));
            if (Collection == null) metadata.AddValidationError(string.Format(Resources.MetadataValidationError, nameof(Collection)));
        }

        protected override Task<int> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken, MongoProperty client)
        {
            
            return Task.FromResult(0);
        }

        protected override void OutputResult(AsyncCodeActivityContext context)
        {
            var this_filter = Filter.Get(context);
            if (this_filter is null)
            {
                this_filter = FilterDefinition<BsonDocument>.Empty;
            }
            var database = Database.Get(context);
            var collection = Collection.Get(context);
            var mongoProperty = context.DataContext.GetProperties()[ParentScope.ParentContainerPropertyTag].GetValue(context.DataContext) as MongoProperty;
            var connectionString = mongoProperty.URL;
            //System.Console.WriteLine("connection ->" + connectionString + ";");
            var mongoClient = new MongoClient(connectionString);
            IMongoCollection<BsonDocument> mongoCollection = mongoClient.GetDatabase(database).GetCollection<BsonDocument>(collection);

            List<string> collectionResults = new List<string>();
            mongoCollection.Find(this_filter).ForEachAsync(doc => collectionResults.Add(doc.ToString())).Wait();

            Results.Set(context, collectionResults.ToArray());
        }
    }
}
