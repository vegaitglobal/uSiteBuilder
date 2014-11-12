using System;
using System.Reflection;

namespace Vega.USiteBuilder
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MixinPropertyAttribute : Attribute
    {
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