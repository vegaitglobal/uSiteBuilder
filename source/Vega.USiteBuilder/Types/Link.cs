namespace Vega.USiteBuilder.Types
{
    /// <summary>
    /// Base class for all link types in Umbraco
    /// </summary>
    public class Link
    {
        /// <summary>
        /// Link title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Link url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// True if link should be opened in new window (target=_blank)
        /// </summary>
        public bool NewWindow { get; set; }
    }
}
