using System;
using Vega.USiteBuilder.DocumentTypeBuilder;

namespace Vega.USiteBuilder.MemberBuilder
{
    /// <summary>
    /// Declares a member type property. Use this property in MemberType definition class
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
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
