namespace Vega.USiteBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Declares a member type property. Use this property in MemberType definition class
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MemberTypePropertyAttribute : DocumentTypePropertyAttribute
    {
        /// <summary>
        /// Declares a member type property. Use this property in MemberType definition class
        /// </summary>
        /// <param name="type">Property data type</param>
        public MemberTypePropertyAttribute(UmbracoPropertyType type)
            : base (type)
        {
        }
    }
}
