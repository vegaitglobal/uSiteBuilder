using System.Web;
using umbraco.cms.businesslogic.macro;
using umbraco.interfaces;
using umbraco.MacroEngines;
using Vega.USiteBuilder.DocumentTypeBuilder;

namespace Vega.USiteBuilder.Razor
{
    /// <summary>
    /// Base class for Razor views. Inherits directly from Umbraco's DynamicNodeContext.
    /// </summary>
    public abstract class USiteBuilderNodeContext<T> : DynamicNodeContext
        where T : DocumentTypeBase, new()
    {
        private T _currentContent;

        /// <summary>
        /// Gets the current node being rendered throug strongly typed property
        /// </summary>
        public T CurrentContent
        {
            get
            {
                return _currentContent;
            }
        }

        /// <summary>
        /// Sets the members.
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <param name="node">The node.</param>
        public override void SetMembers(MacroModel macro, INode node)
        {
            T content = ContentHelper.GetByNodeId<T>(node.Id);

            if (!HttpContext.Current.Items.Contains(node.Id))
            {
                HttpContext.Current.Items.Add(node.Id, content);
            }
            _currentContent = content;
            base.SetMembers(macro, node);
        }
    }
}
