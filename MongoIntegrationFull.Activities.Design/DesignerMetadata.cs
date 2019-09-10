using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using UiPathTeam.MongoDB.Activities.Design.Properties;

namespace UiPathTeam.MongoDB.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute =  new CategoryAttribute($"{Resources.Category}");


            builder.AddCustomAttributes(typeof(ParentScope), categoryAttribute);
            builder.AddCustomAttributes(typeof(ParentScope), new DesignerAttribute(typeof(ParentScopeDesigner)));
            builder.AddCustomAttributes(typeof(ParentScope), new HelpKeywordAttribute("https://go.uipath.com"));

            //builder.AddCustomAttributes(typeof(Query), categoryAttribute);
            //builder.AddCustomAttributes(typeof(Query), new DesignerAttribute(typeof(ChildActivityDesigner)));
            //builder.AddCustomAttributes(typeof(Query), new HelpKeywordAttribute("https://go.uipath.com"));

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
