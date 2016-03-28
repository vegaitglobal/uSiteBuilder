using System.Web;
using umbraco.presentation.nodeFactory;
using System;

namespace Vega.USiteBuilder
{

    /// <summary>
    /// Base class for untyped templates.
    /// </summary>
    public abstract class TemplateBase : System.Web.UI.MasterPage
    {
    }

    /// <summary>
    /// Base class for all strongtyped templates.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TemplateBase<T> : TemplateBase
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
