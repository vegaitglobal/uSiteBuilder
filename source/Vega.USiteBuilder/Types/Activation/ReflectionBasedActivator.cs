using System;

namespace Vega.USiteBuilder.Types.Activation
{
    /// <summary>
    /// Implements activator based on System.Activator
    /// </summary>
    public class ReflectionBasedActivator : IActivator
    {
        public virtual object GetInstance(Type type)
        {
            var result = System.Activator.CreateInstance(type);
            return result;
        }
    }
}