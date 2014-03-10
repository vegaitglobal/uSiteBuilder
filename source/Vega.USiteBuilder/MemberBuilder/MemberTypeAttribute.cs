namespace Vega.USiteBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Declares a class as MemberType. You can use this attribute to set various Umbraco properties of this Member type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MemberTypeAttribute : Attribute
    {
        /// <summary>
        /// Provides access to various Umbraco properties of this Member type.
        /// </summary>
        public MemberTypeAttribute()
        {
            // setting up default values
            this.IconUrl = DocumentTypeDefaultValues.IconUrl;
            this.Thumbnail = DocumentTypeDefaultValues.Thumbnail;
            this.Description = "";
        }

        /// <summary>
        /// Name of this Member Type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Icon property of a Member Type.
        /// Default value: folder.gif.
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// Thumbnail property of a Member Type.
        /// Default value: folder.png
        /// </summary>
        public string Thumbnail { get; set; }

        /// <summary>
        /// Description of a Member Type
        /// Default value: empty
        /// </summary>
        public string Description { get; set; }
    }
}
