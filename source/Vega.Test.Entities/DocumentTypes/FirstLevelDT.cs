namespace Vega.Test.Entities.DocumentTypes
{
    using System;
    using System.Collections.Generic;
    using Vega.USiteBuilder;
    using Vega.USiteBuilder.Types;

    [DocumentType(IconUrl = "doc4.gif", 
                  Thumbnail = "doc.png", 
                  Description = "Decription of FirsteLevelDT", 
                  AllowedTemplates = new string[] { "FirstLevel", "SecondLevel1" },
                  DefaultTemplate = "FirstLevel",
                  AllowedChildNodeTypes = new Type[] {typeof(FirstLevelDT)})]
    public class FirstLevelDT : DocumentTypeBase
    {
        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "First Level Property 1", Alias = "firstlevelproperty1", Tab = TabNames.Settings, Mandatory = true)]
        public string FirstLevelProp1 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Other,
                      OtherTypeName = "Related Links",
                      Tab = TabNames.Settings,
                      Name = "Related Links",
                      Description = "Related Links",
                      Mandatory = true)]
        public virtual List<RelatedLink> RelatedLinks { get; set; }
    }
}
