namespace Vega.USiteBuilder.DocumentTypeBuilder
{
    /// <summary>
    /// Comtent comparison entity class
    /// </summary>
    public class ContentComparison
    {
        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        public string Alias { get; set; }
        /// <summary>
        /// Gets or sets the document type status.
        /// </summary>
        /// <value>
        /// The document type status.
        /// </value>
        public Status DocumentTypeStatus { get; set; }
        /// <summary>
        /// Gets or sets the parent alias.
        /// </summary>
        /// <value>
        /// The parent alias.
        /// </value>
        public string ParentAlias { get; set; }
    }
}