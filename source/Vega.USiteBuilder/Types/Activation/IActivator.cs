using System;

namespace Vega.USiteBuilder.Types.Activation
{
    /// <summary>
    /// Encapsulates type instance creation
    /// </summary>
    public interface IActivator
    {
        /// <summary>
        /// Creates instance of a given type
        /// </summary>
        /// <param name="type">type</param>
        /// <returns></returns>
        object GetInstance(Type type);
    }
}