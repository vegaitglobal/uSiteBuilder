using System.Xml.Serialization;
using System;
namespace Vega.USiteBuilder.Types
{
    /// <summary>
    /// Related link type.
    /// </summary>
    public class RelatedLink : Link
    {
        int? _relatedNodeId;

        /// <summary>
        /// Id of a node if Related link is Internal link or Media link.
        /// </summary>
        [XmlIgnore]
        public int? RelatedNodeId {
            get
            {
                if (!_relatedNodeId.HasValue && !string.IsNullOrEmpty(Internal))
                {
                    int relatedNodeId;
                    if (Int32.TryParse(Internal, out relatedNodeId))
                    {
                        _relatedNodeId = relatedNodeId;
                    }
                }

                return _relatedNodeId;
            }
            set
            {
                _relatedNodeId = value;
            }
        }
        
        /// <summary>
        /// Related link type
        /// </summary>
        public enum RelatedLinkType
        {
            /// <summary>
            /// Internal link
            /// </summary>
            Internal,

            /// <summary>
            /// External link
            /// </summary>
            External,

            /// <summary>
            /// Link to media (exists only in RelatedLinksWithMedia data type)
            /// </summary>
            Media
        }

        /// <summary>
        /// Related link type (external or internal)
        /// </summary>
        public RelatedLinkType Type { get; set; }
    }
}
