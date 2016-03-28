using System;
using System.Web;
using umbraco.presentation.nodeFactory;

namespace Vega.USiteBuilder
{
    public abstract class UmbracoTemplatePageBase : Umbraco.Web.Mvc.UmbracoTemplatePage
    {
    }

    public abstract class UmbracoTemplatePageBase<T> : UmbracoTemplatePageBase
    where T : DocumentTypeBase, new()
    {
        private T _currentContent;

        /// <summary>
        /// Gets the current content item.
        /// </summary>
        public T CurrentContent
        {
            get
            {
                if (this._currentContent == null)
                {
                    int nodeId = Node.GetCurrent().Id;
                    if (!HttpContext.Current.Items.Contains(nodeId))
                    {
                        T item = ContentHelper.GetByNodeId<T>(nodeId);

                        if (item == null)
                            throw new Exception(string.Format("The document type {0} does not exist in Umbraco, please run sync again. (siteBuilderSupressSynchronization=false)", typeof(T).Name));

                        HttpContext.Current.Items.Add(nodeId, item);
                    }

                    this._currentContent = (T)HttpContext.Current.Items[nodeId];
                }

                return this._currentContent;
            }
        }
    }


}