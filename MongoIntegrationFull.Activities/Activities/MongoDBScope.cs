using System;
using System.Activities;
using System.ComponentModel;
using System.Activities.Statements;
using MongoIntegrationFull.Activities.Properties;

namespace MongoIntegrationFull.Activities
{

    [LocalizedDescription("Establish a connection to Mongo DB")]
    [LocalizedDisplayName("Mongo Database Scope")]
    [Designer(typeof(UiPathTeam.Box.Designer.BoxScopeDesigner))]
    public class ParentScope : NativeActivity
    {
        #region Properties

        [Browsable(false)]
        public ActivityAction<MongoIntegrationFull.MongoProperty> Body { get; set; }

        [LocalizedCategory(nameof(Resources.Authentication))]
        [LocalizedDisplayName(nameof(Resources.ParentScopeUsernameDisplayName))]
        [LocalizedDescription(nameof(Resources.ParentScopeUsernameDescription))]
        public InArgument<string> Username { get; set; }

        [LocalizedCategory(nameof(Resources.Authentication))]
        [LocalizedDisplayName(nameof(Resources.ParentScopePasswordDisplayName))]
        [LocalizedDescription(nameof(Resources.ParentScopePasswordDescription))]
        public InArgument<string> Password { get; set; }

        [RequiredArgument]
        [LocalizedCategory(nameof(Resources.Authentication))]
        [LocalizedDisplayName(nameof(Resources.ParentScopeURLDisplayName))]
        [LocalizedDescription(nameof(Resources.ParentScopeURLDescription) + ", e.g: mongodb://mongodb0.example.com:27017/admin")]
        public InArgument<string> URL { get; set; }

        internal static string ParentContainerPropertyTag => "MongoScope";

        #endregion


        #region Constructors

        public ParentScope()
        {

            Body = new ActivityAction<MongoProperty>
            {
                Argument = new DelegateInArgument<MongoProperty>(ParentContainerPropertyTag),
                Handler = new Sequence { DisplayName = "Do" }
            };
        }

        #endregion


        #region Private Methods

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        protected override void Execute(NativeActivityContext context)
        {
            var username = Username.Get(context);
            var password = Password.Get(context);
            //replace special char (@ / : %) with corresponding percent encoding
            if(password != null)
            {
                password = password.Replace("%", "%25").Replace("@", "%40").Replace("/", "%2F").Replace(":", "%3A");
            }
            var url = URL.Get(context);
            //form the new URL if username and password is provided
            if(username != null & password != null)
            {
                url = url.Replace("mongodb://", "mongodb://" + username + ":" + password + "@");
            }
            var application = new MongoIntegrationFull.MongoProperty(username, password, url);
            
            if (Body != null)
            {
                context.ScheduleAction<MongoIntegrationFull.MongoProperty>(Body, application, this.OnCompleted, this.OnFaulted);
            }
        }

        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            //TODO
        }

        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            //TODO
        }

        #endregion


        #region Helpers

        #endregion
    }
}
