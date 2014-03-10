namespace Vega.Test.Entities.DocumentTypes
{
    using Vega.USiteBuilder;

    /// <summary>
    /// Tab names in Umbraco CMS.
    /// </summary>
    public enum TabNames
    {
        /// <summary>
        /// Pages tab name.
        /// </summary>
        Pages = 1,

        /// <summary>
        /// Nodes tab name.
        /// </summary>
        Nodes,

        /// <summary>
        /// EmailTemplates tab name.
        /// </summary>
        [TabName("Email Templates")]
        EmailTemplates,

        /// <summary>
        /// Page tab name.
        /// </summary>
        Page,

        /// <summary>
        /// Content tab name.
        /// </summary>
        Content,

        /// <summary>
        /// Settings tab name.
        /// </summary>
        Settings
    }
}
