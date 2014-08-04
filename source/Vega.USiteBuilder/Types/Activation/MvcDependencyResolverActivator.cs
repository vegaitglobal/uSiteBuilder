using System;
using System.Web.Mvc;

namespace Vega.USiteBuilder.Types.Activation
{
    /// <summary>
    /// Implements activator based on MVC DependencyResolver
    /// </summary>
    public class MvcDependencyResolverActivator : IActivator
    {
        private readonly IDependencyResolver _dependencyResolver;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dependencyResolver">Dependency resolver to use. </param>
        public MvcDependencyResolverActivator(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public virtual object GetInstance(Type type)
        {
            var result = _dependencyResolver.GetService(type);
            return result;
        }
    }
}