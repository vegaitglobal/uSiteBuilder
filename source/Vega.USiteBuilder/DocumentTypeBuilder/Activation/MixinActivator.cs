using System;

namespace Vega.USiteBuilder
{
    public class MixinActivator
    {
        static MixinActivator()
        {
            Current = new MixinActivator();
        }

        public static MixinActivator Current { get; set; }

        public virtual object CreateInstance(Type mixinType)
        {
            var result = System.Activator.CreateInstance(mixinType);
            return result;
        }
    }
}