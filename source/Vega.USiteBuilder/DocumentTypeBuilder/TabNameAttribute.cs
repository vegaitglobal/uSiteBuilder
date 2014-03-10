namespace Vega.USiteBuilder
{
    using System;

    /// <summary>
    /// Allows setting document type tab name with spaces
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class TabNameAttribute : Attribute
    {
        /// <summary>
        /// Use this to set tab name if it contain spaces.
        /// </summary>
        /// <param name="name">The name.</param>
        public TabNameAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the tab name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
    }
}
