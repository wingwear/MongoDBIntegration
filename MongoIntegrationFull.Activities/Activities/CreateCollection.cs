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
    [LocalizedDisplayName("Create Collection")]
    [LocalizedDescription("Create new Collection/Database in Mongo DB")]
    public class CreateCollection : AsyncTaskCodeActivity<int>
    {
        [RequiredArgument]
        [LocalizedDisplayName("Database")]
        [LocalizedDescription("Name of database to create")]
        [LocalizedCategory(nameof(Resources.Input))]
        public InArgument<string> Database { get; set; }

        [RequiredArgument]
        [LocalizedDisplayName("Collection")]
        [LocalizedDescription("Name of collection to create")]
        [LocalizedCategory(nameof(Resources.Input))]
        public InArgument<string> Collection { get; set; }

        // Options to create collections
        [RequiredArgument]
        [LocalizedDisplayName("Capped")]
        [LocalizedDescription("Whether collection should be capped")]
        [LocalizedCategory("Options to Create Collection")]
        public InArgument<bool> Capped { get; set; }

        [RequiredArgument]
        [LocalizedDisplayName("MaxSize")]
        [LocalizedDescription("Maximum size of collection in bytes")]
        [LocalizedCategory("Options to Create Collection")]
        public InArgument<int> MaxSize { get; set; }

        [RequiredArgument]
        [LocalizedDisplayName("MaxCount")]
        [LocalizedDescription("Maximum number of documents collection can store")]
        [LocalizedCategory("Options to Create Collection")]
        public InArgument<int> MaxCount { get; set; }

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
            var capped = Capped.Get(context);
            var maxSize = MaxSize.Get(context);
            var maxCount = MaxCount.Get(context);
            var database = Database.Get(context);
            var collection = Collection.Get(context);
            var mongoProperty = context.DataContext.GetProperties()[ParentScope.ParentContainerPropertyTag].GetValue(context.DataContext) as MongoProperty;
            var connectionString = mongoProperty.URL;
            var mongoClient = new MongoClient(connectionString);
            mongoClient.GetDatabase(database).CreateCollectionAsync(collection, new CreateCollectionOptions
            {
                Capped = capped,
                MaxSize = maxSize,
                MaxDocuments = maxCount,
            }).Wait();
        }
    }
}
