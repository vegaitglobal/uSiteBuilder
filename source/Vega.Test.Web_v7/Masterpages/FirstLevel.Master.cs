using System;
using System.Linq;
using Vega.USiteBuilder;
using Vega.Test.Entities.DocumentTypes;
using System.Collections.Generic;
using Vega.Test.Entities.MemberTypes;

namespace Vega.Test.Web_v7.Masterpages
{
    public partial class FirstLevel : TemplateBase<FirstLevelDT>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.CurrentContent.RelatedLinks != null)
            {
                Response.Write(this.CurrentContent.RelatedLinks[0].Url);
                Response.Write("<br />");
            }

            Response.Write(this.CurrentContent.FirstLevelProp1);
            Response.Write("<br />");
            IEnumerable<SecondLevelDT1> children = this.CurrentContent.GetChildren().Cast<SecondLevelDT1>();
            if (children != null && children.Count() > 0)
            {
                Response.Write(children.First().Property3);
                Response.Write("<br />");
            }

            AdminMemberType member = MemberHelper.GetMemberFromEmail("d.eremic@vegaitsourcng.rs") as AdminMemberType;
            if (member != null)
            {
                MemberHelper.LoginWithFormsAuthentication("d.eremic@vegaitsourcng.rs", "1234", true, 1500, false);
                AdminMemberType currentMember = MemberHelper.GetCurrentMember() as AdminMemberType;
                if (currentMember != null)
                {
                    Response.Write(currentMember.LoginName);
                    Response.Write("<br />");
                }
            }
        }
    }
}