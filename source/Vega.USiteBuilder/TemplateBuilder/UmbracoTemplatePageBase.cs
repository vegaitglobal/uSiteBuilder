using System;
using System.Web;

using umbraco.NodeFactory;

namespace Vega.USiteBuilder
{
    public abstract class UmbracoTemplatePageBase : Umbraco.Web.Mvc.UmbracoTemplatePage
    {
    }

    public abstract class UmbracoTemplatePageBase<T> : UmbracoTemplatePageBase
    where T : DocumentTypeBase, new()
    {
        private T _typedContent;

        /// <summary>
        /// Gets the current content item.
        /// </summary>
        public T TypedModel
        {
            get
            {
                if (this._typedContent == null)
                {
                    int nodeId = Node.getCurrentNodeId();
                    if (!HttpContext.Current.Items.Contains(nodeId))
                    {
                        T item = DocumentTypeResolver.Instance.GetTyped<T>(nodeId);

                        if (item == null)
                            throw new Exception(string.Format("The document type {0} does not exist in Umbraco, please run sync again. (siteBuilderSupressSynchronization=false)", typeof(T).Name));

                        HttpContext.Current.Items.Add(nodeId, item);
                    }

                    this._typedContent = (T)HttpContext.Current.Items[nodeId];
                }

                return this._typedContent;
            }
        }
    }


}