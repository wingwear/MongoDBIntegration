using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoIntegrationFull.Activities.Properties;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Activities;
using System.Threading;

namespace MongoIntegrationFull.Activities
{
    [LocalizedDisplayName("Update Document")]
    [LocalizedDescription("Update one document in Mongo DB")]
    public class UpdateDoc : AsyncTaskCodeActivity<int>
    {
        [RequiredArgument]
        [LocalizedDisplayName("Database")]
        [LocalizedDescription("Name of database to update")]
        [LocalizedCategory(nameof(Resources.Input))]
        public InArgument<string> Database { get; set; }

        [RequiredArgument]
        [LocalizedDisplayName("Collection")]
        [LocalizedDescription("Name of collection to update")]
        [LocalizedCategory(nameof(Resources.Input))]
        public InArgument<string> Collection { get; set; }

        [RequiredArgument]
        [LocalizedDisplayName("Filter Criteria")]
        [LocalizedDescription("Criteria to find first matching document")]
        [LocalizedCategory(nameof(Resources.Input))]
        public InArgument<FilterDefinition<BsonDocument>> Filter { get; set; }

        [RequiredArgument]
        [LocalizedDisplayName("Update Definition")]
        [LocalizedDescription("What to update for the target document")]
        [LocalizedCategory(nameof(Resources.Input))]
        [OverloadGroup("Update Definitions")]
        public InArgument<UpdateDefinition<BsonDocument>> Update { get; set; }

        [RequiredArgument]
        [LocalizedDisplayName("Replace Definition")]
        [LocalizedDescription("What to replace for the target document")]
        [LocalizedCategory(nameof(Resources.Input))]
        [OverloadGroup("Replace Definitions")]
        public InArgument<BsonDocument> Replace { get; set; }

        /// <inheritdoc />
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);

            if (Database == null) metadata.AddValidationError(string.Format(Resources.MetadataValidationError, nameof(Database)));
            if (Collection == null) metadata.AddValidationError(string.Format(Resources.MetadataValidationError, nameof(Collection)));
            if (Filter == null) metadata.AddValidationError(string.Format(Resources.MetadataValidationError, nameof(Filter)));
        }

        protected override Task<int> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken, MongoProperty client)
        {

            return Task.FromResult(0);
        }

        protected override void OutputResult(AsyncCodeActivityContext context)
        {
            var database = Database.Get(context);
            var collection = Collection.Get(context);
            var update = Update.Get(context);
            var replace = Replace.Get(context);
            var filter = Filter.Get(context);
            var mongoProperty = context.DataContext.GetProperties()[ParentScope.ParentContainerPropertyTag].GetValue(context.DataContext) as MongoProperty;
            var connectionString = mongoProperty.URL;
            var mongoClient = new MongoClient(connectionString);
            IMongoCollection<BsonDocument> mongoCollection = mongoClient.GetDatabase(database).GetCollection<BsonDocument>(collection);
            // If this is not an update operation
            if (update == null)
            {
                mongoCollection.ReplaceOne(filter, replace);
            }
            else
            {
                mongoCollection.UpdateOne(filter, update);
            }
        }
    }
}
