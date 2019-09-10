using System.Threading.Tasks;
using UiPathTeam.MongoDB.Activities.Properties;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Activities;
using System.Threading;

namespace UiPathTeam.MongoDB.Activities
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
        [LocalizedDisplayName("Capped")]
        [LocalizedDescription("Whether collection should be capped, default is True")]
        [LocalizedCategory("Options to Create Collection")]
        public InArgument<bool?> Capped { get; set; }

        [LocalizedDisplayName("MaxSize")]
        [LocalizedDescription("Maximum size of collection in bytes, default is 1024")]
        [LocalizedCategory("Options to Create Collection")]
        public InArgument<int?> MaxSize { get; set; }

        [LocalizedDisplayName("MaxCount")]
        [LocalizedDescription("Maximum number of documents collection can store, default is 1000")]
        [LocalizedCategory("Options to Create Collection")]
        public InArgument<int?> MaxCount { get; set; }

        public CreateCollection()
        {
            Constraints.Add(ParentConstraint.CheckThatParentsAreOfType<CreateCollection, ParentScope>("Activity is valid only inside Mongo Database Scope"));
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
            //Parse collection options if defined
            var capped = Capped.Get(context);
            if (capped is null)
            {
                capped = true;
            }
            var maxSize = MaxSize.Get(context);
            if (maxSize is null)
            {
                maxSize = 1024;
            }
            var maxCount = MaxCount.Get(context);
            if (maxCount is null)
            {
                maxCount = 1000;
            }

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
