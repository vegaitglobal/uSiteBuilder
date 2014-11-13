using System;
using System.Reflection;

namespace Vega.USiteBuilder
{
    /// <summary>
    /// Marks a property as a mixin
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MixinPropertyAttribute : Attribute
    {
        /// <summary>
        /// Mixin Type. Leave it empty to use Property Type. 
        /// </summary>
        public Type MixinType { get; set; }

        /// <summary>
        /// Gets MixinType using property type as a fallback
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public Type GetMixinType(PropertyInfo property)
        {
            return MixinType ?? property.PropertyType;
        }
    }
}