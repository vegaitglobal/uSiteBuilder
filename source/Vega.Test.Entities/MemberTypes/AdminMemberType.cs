namespace Vega.Test.Entities.MemberTypes
{
    using Vega.Test.Entities.DocumentTypes;
    using Vega.USiteBuilder;

    [MemberType(Name = "Administrator", Description = "Member type that describes administrators", IconUrl = "memberType.gif", Thumbnail = "member.gif")]
    public class AdminMemberType : MemberTypeBase
    {
        public AdminMemberType(string loginName, string email, string password)
            : base(loginName, email, password)
        {

        }

        [MemberTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1b", Alias = "property1", Tab = TabNames.Settings, Mandatory = true, DefaultValue = "default value")]
        public string Prop1 { get; set; }

        [MemberTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Alias = "property2", Tab = TabNames.Settings, Mandatory = true, DefaultValue = "default value")]
        public string Prop2 { get; set; }

        [MemberTypeProperty(UmbracoPropertyType.Textstring, Tab = TabNames.Settings, Mandatory = true, DefaultValue = "default value")]
        public string Prop3 { get; set; }

        [MemberTypeProperty(UmbracoPropertyType.Textstring, Tab = TabNames.Settings, Mandatory = true)]
        public virtual string Prop4 { get; set; }
    }
}
