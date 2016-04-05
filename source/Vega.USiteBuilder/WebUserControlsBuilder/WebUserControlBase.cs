using System.Web;
using System.Web.UI;
using Vega.USiteBuilder.DocumentTypeBuilder;

namespace Vega.USiteBuilder.WebUserControlsBuilder
{
    //using umbraco.NodeFactory;

    /// <summary>
    /// Base class for untyped web user controls.
    /// </summary>
    public abstract class WebUserControlBase : UserControl
    {
    }

    /// <summary>
    /// Base class for typed web user controls.
    /// </summary>
    public abstract class WebUserControlBase<T> : WebUserControlBase
        where T : DocumentTypeBase, new()
    {
        private int? _contentNodeId;
        private T _currentContent;

        /// <summary>
        /// Gets or sets the content node id of a content associated with this control.
        /// Note: Default value of this property is the current node id.
        /// </summary>
        public int ContentNodeId
        {
            get
            {
                if (!_contentNodeId.HasValue)
                {
                    _contentNodeId = umbraco.NodeFactory.Node.GetCurrent().Id;
                }

                return (int)_contentNodeId;
            }
            set
            {
                _contentNodeId = value;
            }
        }

        /// <summary>
        /// Gets the current content item.
        /// </summary>
        public T Content
        {
            get
            {
                if (_currentContent == null)
                {
                    if (!HttpContext.Current.Items.Contains(ContentNodeId))
                    {
                        HttpContext.Current.Items.Add(ContentNodeId, ContentHelper.GetByNodeId<T>(ContentNodeId));
                    }

                    _currentContent = (T)HttpContext.Current.Items[ContentNodeId];
                }

                return _currentContent;
            }
        }
    }
}
