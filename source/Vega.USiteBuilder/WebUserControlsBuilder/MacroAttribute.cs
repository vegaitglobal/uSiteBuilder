namespace Vega.USiteBuilder
{
    using System;

    /// <summary>
    /// Sets the properties of a macro corresponding to this control.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MacroAttribute : Attribute
    {
        /// <summary>
        /// Sets the properties of a macro corresponding to this control.
        /// </summary>
        public MacroAttribute()
        {
            this.UseInEditor = false;
            this.RenderContentInEditor = true;
            this.CachePeriod = 0;
            this.CacheByPage = true;
            this.CachePersonalized = false;
        }

        /// <summary>
        /// Gets or sets the macro name.
        /// Default value: name of this class.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets if macro can be used in editor
        /// Default value: false
        /// </summary>
        public bool UseInEditor { get; set; }

        /// <summary>
        /// Gets or sets if macro should be rendered in editor
        /// Default value: true
        /// </summary>
        public bool RenderContentInEditor { get; set; }

        /// <summary>
        /// Gets or sets cache period in seconds
        /// Default value: 0
        /// </summary>
        public int CachePeriod { get; set; }

        /// <summary>
        /// Gets or sets if macro should be cached per page.
        /// Default value: true
        /// </summary>
        public bool CacheByPage { get; set; }

        /// <summary>
        /// Gets or sets if cache should be personalized.
        /// Default value: false
        /// </summary>
        public bool CachePersonalized { get; set; }
    }
}
