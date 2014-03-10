using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Vega.USiteBuilder.MVC.Controllers
{
    public class SecondLevelDT1Controller : RenderMvcController
    {
        public override ActionResult Index(RenderModel model)
        {
            //Do some stuff here, then return the base method
            return CurrentTemplate(model);
        }
    }
}
