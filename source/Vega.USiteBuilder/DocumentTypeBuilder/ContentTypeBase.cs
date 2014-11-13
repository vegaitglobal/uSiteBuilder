using System.Collections.Generic;
using umbraco.NodeFactory;

namespace Vega.USiteBuilder
{
    /// <summary>
    /// Base class for content types: DocumentTypeBase and MixinBase for now. 
    /// NOTE: Content Type will support lazy-load for virtual properties, 
    /// that's why source node and property values cache are introduced here. 
    /// </summary>
    public abstract class ContentTypeBase
    {
        /// <summary>
        /// Source node object.
        /// </summary>
        internal Node Source { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> value at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal object this[string index]
        {
            get
            {
                if (_propertyValues.ContainsKey(index))
                {
                    return _propertyValues[index];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (_propertyValues.ContainsKey(index))
                {
                    _propertyValues[index] = value;
                }
                else
                {
                    _propertyValues.Add(index, value);
                }
            }
        }

        private Dictionary<string, object> _propertyValues = new Dictionary<string, object>();        
    }
}