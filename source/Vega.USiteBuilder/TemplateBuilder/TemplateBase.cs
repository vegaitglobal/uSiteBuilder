using System;
using System.Web;
using System.Web.UI;
using Vega.USiteBuilder.DocumentTypeBuilder;

namespace Vega.USiteBuilder.TemplateBuilder
{

    /// <summary>
    /// Base class for untyped templates.
    /// </summary>
    public abstract class TemplateBase : MasterPage
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
                if (_currentContent == null)
                {
                    int nodeId = umbraco.NodeFactory.Node.GetCurrent().Id;
                    if (!HttpContext.Current.Items.Contains(nodeId))
                    {
                        T item = ContentHelper.GetByNodeId<T>(nodeId);

                        if (item == null)
                            throw new Exception(string.Format("The document type {0} does not exist in Umbraco, please run sync again. (siteBuilderSupressSynchronization=false)", typeof(T).Name));

                        HttpContext.Current.Items.Add(nodeId, item);
                    }

                    _currentContent = (T)HttpContext.Current.Items[nodeId];
                }

                return _currentContent;
            }
        }
    }
}
