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
    [LocalizedDisplayName("Insert Document")]
    [LocalizedDescription("Insert one document in Mongo DB")]
    public class InsertDoc : AsyncTaskCodeActivity<int>
    {
        [RequiredArgument]
        [LocalizedDisplayName("Database")]
        [LocalizedDescription("Name of database to insert")]
        [LocalizedCategory(nameof(Resources.Input))]
        public InArgument<string> Database { get; set; }

        [RequiredArgument]
        [LocalizedDisplayName("Collection")]
        [LocalizedDescription("Name of collection to insert")]
        [LocalizedCategory(nameof(Resources.Input))]
        public InArgument<string> Collection { get; set; }

        [RequiredArgument]
        [LocalizedDisplayName("Document")]
        [LocalizedDescription("Name of Document to insert")]
        [LocalizedCategory(nameof(Resources.Input))]
        public InArgument<BsonDocument> Document { get; set; }

        /// <inheritdoc />
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);

            if (Database == null) metadata.AddValidationError(string.Format(Resources.MetadataValidationError, nameof(Database)));
            if (Collection == null) metadata.AddValidationError(string.Format(Resources.MetadataValidationError, nameof(Collection)));
            if (Document == null) metadata.AddValidationError(string.Format(Resources.MetadataValidationError, nameof(Document)));

        }

        protected override Task<int> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken, MongoProperty client)
        {

            return Task.FromResult(0);
        }

        protected override void OutputResult(AsyncCodeActivityContext context)
        {
            var database = Database.Get(context);
            var collection = Collection.Get(context);
            var document = Document.Get(context);
            var mongoProperty = context.DataContext.GetProperties()[ParentScope.ParentContainerPropertyTag].GetValue(context.DataContext) as MongoProperty;
            var connectionString = mongoProperty.URL;
            var mongoClient = new MongoClient(connectionString);
            IMongoCollection<BsonDocument> mongoCollection = mongoClient.GetDatabase(database).GetCollection<BsonDocument>(collection);
            mongoCollection.InsertOneAsync(document).Wait();
        }
    }
}
