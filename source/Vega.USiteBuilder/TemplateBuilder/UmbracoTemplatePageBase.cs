using System;
using System.Web;

using umbraco.NodeFactory;

namespace Vega.USiteBuilder
{
    public abstract class UmbracoTemplatePageBase : Umbraco.Web.Mvc.UmbracoViewPage<DocumentTypeBase>
    {
    }

    public abstract class UmbracoTemplatePageBase<T> : Umbraco.Web.Mvc.UmbracoViewPage<T>
        where T : DocumentTypeBase, new()
    {
    }
}