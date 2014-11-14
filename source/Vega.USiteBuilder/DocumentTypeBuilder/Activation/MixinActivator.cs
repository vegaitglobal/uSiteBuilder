using System;
using umbraco.NodeFactory;

namespace Vega.USiteBuilder
{
    /// <summary>
    /// Mixin activator creates mixin instances.
    /// Current implementation doesn't support virtual properties in mixins.  
    /// </summary>
    public class MixinActivator
    {
        static MixinActivator()
        {
            Current = new MixinActivator();
        }

        /// <summary>
        /// Poor-man-IoC: Currently used activator
        /// </summary>
        public static MixinActivator Current { get; set; }

        /// <summary>
        /// Creates an instance of mixin
        /// </summary>
        /// <param name="mixinType">Mixin type, should be derived from MixinBase</param>
        /// <param name="node">Source node</param>
        /// <returns>Returns a newly created mixin object</returns>
        public virtual MixinBase CreateInstance(Type mixinType, Node node)
        {
            var result = (MixinBase) DocumentTypeResolver.Instance.Activator.CreateInstance(mixinType);
            result.Source = node;
            return result;
        }
    }
}