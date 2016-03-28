using System;

namespace Vega.USiteBuilder.TemplateBuilder
{
    /// <summary>
    /// Sets the template properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TemplateAttribute :  Attribute
    {
        /// <summary>
        /// Sets the template properties.
        /// </summary>
        public TemplateAttribute()
        {
            AllowedForDocumentType = true;
        }

        /// <summary>
        /// Default value of this property is true.
        /// Indicates if this template should be set as allowed template
        /// for a DocumentType to which this template is strongly typed.
        /// Note: if this template is not strongly typed than setting
        /// this property makes not sense and is ignored.
        /// </summary>
        public bool AllowedForDocumentType { get; set; }
    }
}
