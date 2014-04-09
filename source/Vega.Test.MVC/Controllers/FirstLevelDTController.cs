using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Vega.Test.Entities.DocumentTypes;
using Vega.USiteBuilder.TemplateBuilder;
using System.Collections.Generic;

namespace Vega.USiteBuilder.MVC.Controllers
{
    public class FirstLevelDTController : TemplateControllerBase<FirstLevelDT>
    {
        public override ActionResult Index(RenderModel model)
        {
            IEnumerable<SecondLevelDT1> children = ContentHelper.GetChildren<SecondLevelDT1>(this.CurrentContent.Id);
            IEnumerable<SecondLevelDT1> children1 = this.CurrentContent.GetChildren<SecondLevelDT1>();

            //Do some stuff here, then return the base method
            return CurrentTemplate(this.CurrentContent);
        }
    }
}
