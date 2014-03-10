namespace Vega.Test.Entities.DocumentTypes
{
    using System;
    using Vega.USiteBuilder;

    [DocumentType(IconUrl = "doc4.gif", 
                  Thumbnail = "folder.png", 
                  Description = "Decription of SecondLevelDT1",
                  AllowedChildNodeTypeOf = new Type[] { typeof(FirstLevelDT) })]
    public class SecondLevelDT10 : FirstLevelDT
    {
        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop1 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop2 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property3 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property4 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop11 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop12 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property13 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property14 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop21 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop22 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property23 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property24 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop51 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop52 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property53 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property54 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop61 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop62 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property63 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property64 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop31 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop32 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property33 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property34 { get; set; }
        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop41 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop42 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property43 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property44 { get; set; }
        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop71 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop72 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property73 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property74 { get; set; }
        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop81 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop82 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property83 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property84 { get; set; }
        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop91 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop92 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property93 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property94 { get; set; }
        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop01 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop02 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property03 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property04 { get; set; }
        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop111 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop112 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property113 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property114 { get; set; }
        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop121 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop122 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property123 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property124 { get; set; }
        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop131 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop132 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property133 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property134 { get; set; }
        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop141 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop142 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property143 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property144 { get; set; }
        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop151 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop152 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property153 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property154 { get; set; }
    }
}
