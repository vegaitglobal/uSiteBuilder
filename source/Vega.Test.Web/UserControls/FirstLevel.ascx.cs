using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vega.USiteBuilder;
using Vega.Test.Entities.DocumentTypes;

namespace Vega.Test.Web.UserControls
{
    [Macro(Name = "FirstLevelMacro", UseInEditor = true, RenderContentInEditor = true, CachePeriod = 33, CacheByPage = true, CachePersonalized = true)]
    public partial class FirstLevel : WebUserControlBase<FirstLevelDT>
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}