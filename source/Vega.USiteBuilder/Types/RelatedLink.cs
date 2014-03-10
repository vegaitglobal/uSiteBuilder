namespace Vega.USiteBuilder.Types
{
    /// <summary>
    /// Related link type.
    /// </summary>
    public class RelatedLink : Link
    {
        /// <summary>
        /// Id of a node if Related link is Internal link or Media link.
        /// </summary>
        public int? RelatedNodeId { get; set; }
        
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
