namespace Vega.USiteBuilder
{
    using System.Web;
    using umbraco.presentation.nodeFactory;

    //using umbraco.NodeFactory;

    /// <summary>
    /// Base class for untyped web user controls.
    /// </summary>
    public abstract class WebUserControlBase : System.Web.UI.UserControl
    {
    }

    /// <summary>
    /// Base class for typed web user controls.
    /// </summary>
    public abstract class WebUserControlBase<T> : WebUserControlBase
        where T : DocumentTypeBase, new()
    {
        private int? _contentNodeId;
        private T _currentContent = null;

        /// <summary>
        /// Gets or sets the content node id of a content associated with this control.
        /// Note: Default value of this property is the current node id.
        /// </summary>
        public int ContentNodeId
        {
            get
            {
                if (!this._contentNodeId.HasValue)
                {
                    this._contentNodeId = Node.GetCurrent().Id;
                }

                return (int)this._contentNodeId;
            }
            set
            {
                this._contentNodeId = value;
            }
        }

        /// <summary>
        /// Gets the current content item.
        /// </summary>
        public T Content
        {
            get
            {
                if (this._currentContent == null)
                {
                    if (!HttpContext.Current.Items.Contains(this.ContentNodeId))
                    {
                        HttpContext.Current.Items.Add(this.ContentNodeId, ContentHelper.GetByNodeId<T>(this.ContentNodeId));
                    }

                    this._currentContent = (T)HttpContext.Current.Items[this.ContentNodeId];
                }

                return this._currentContent;
            }
        }
    }
}
