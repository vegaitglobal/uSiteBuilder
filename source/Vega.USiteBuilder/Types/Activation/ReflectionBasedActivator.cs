using System;

namespace Vega.USiteBuilder.Types.Activation
{
    /// <summary>
    /// Implements activator based on System.Activator
    /// </summary>
    public class ReflectionBasedActivator : IActivator
    {
        /// <summary>
        /// Creates instance of a given type
        /// </summary>
        /// <param name="type">type</param>
        /// <returns></returns>
        public virtual object GetInstance(Type type)
        {
            var result = System.Activator.CreateInstance(type);
            return result;
        }
    }
}