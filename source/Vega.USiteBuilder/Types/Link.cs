using System.Xml.Serialization;
using Newtonsoft.Json;
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
        [JsonProperty(PropertyName = "link")]
        public string Url { get; set; }

        /// <summary>
        /// True if link should be opened in new window (target=_blank)
        /// </summary>
        public bool NewWindow { get; set; }

        /// <summary>
        /// Link caption
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Is link editable
        /// </summary>
        public bool Edit { get; set; }

        /// <summary>
        /// Is link internal
        /// </summary>
        public bool IsInternal { get; set; }

        /// <summary>
        /// Internal node id
        /// </summary>
        public string Internal { get; set; }

        /// <summary>
        /// Internal node name
        /// </summary>
        public string internalName { get; set; }
    }
}
