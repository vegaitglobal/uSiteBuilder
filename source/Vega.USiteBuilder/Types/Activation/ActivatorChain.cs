using System;
using System.Collections.Generic;
using System.Linq;

namespace Vega.USiteBuilder.Types.Activation
{
    /// <summary>
    /// Implements activator chain which sequentally calls configured activators until a new instance created
    /// </summary>
    public class ActivatorChain : IActivator
    {
        /// <summary>
        /// Gets the activators.
        /// </summary>
        /// <value>
        /// The activators.
        /// </value>
        protected List<IActivator> Activators { get; private set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="activators">a sequence of activators to enumerate</param>
        public ActivatorChain(IEnumerable<IActivator> activators)
        {
            if (activators == null) throw new ArgumentNullException("activators");
            Activators = activators.ToList();
        }

        /// <summary>
        /// Creates instance of a given type
        /// </summary>
        /// <param name="type">type</param>
        /// <returns></returns>
        public virtual object GetInstance(Type type)
        {
            foreach (var activator in Activators)
            {
                var result = activator.GetInstance(type);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}