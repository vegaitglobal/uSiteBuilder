namespace Vega.USiteBuilder
{
    using System;

    /// <summary>
    /// Marks a property as a macro parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MacroParameterAttribute : Attribute
    {
        internal MacroParameterAttribute()
        {
            this.Show = false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type of this macro parameter</param>
        public MacroParameterAttribute(MacroParameterType type)
            : this()
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets or sets if this parameter should be shown.
        /// Default value: false
        /// </summary>
        public bool Show { get; set; }

        /// <summary>
        /// Gets or sets macro parameter name.
        /// Default value: Name of the property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the macro parameter type.
        /// </summary>
        public MacroParameterType Type { get; internal set; }
    }
}
