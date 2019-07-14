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
    [LocalizedDisplayName("Delete Collection")]
    [LocalizedDescription("Delete Collection in Mongo DB")]
    public class DeleteCollection : AsyncTaskCodeActivity<int>
    {
        [RequiredArgument]
        [LocalizedDisplayName("Database")]
        [LocalizedDescription("Name of database where collection resides")]
        [LocalizedCategory(nameof(Resources.Input))]
        public InArgument<string> Database { get; set; }

        [RequiredArgument]
        [LocalizedDisplayName("Collection")]
        [LocalizedDescription("Name of collection to delete")]
        [LocalizedCategory(nameof(Resources.Input))]
        public InArgument<string> Collection { get; set; }

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
            var database = Database.Get(context);
            var collection = Collection.Get(context);
            var mongoProperty = context.DataContext.GetProperties()[ParentScope.ParentContainerPropertyTag].GetValue(context.DataContext) as MongoProperty;
            var connectionString = mongoProperty.URL;
            var mongoClient = new MongoClient(connectionString);
            mongoClient.GetDatabase(database).DropCollection(collection);
        }
    }
}
