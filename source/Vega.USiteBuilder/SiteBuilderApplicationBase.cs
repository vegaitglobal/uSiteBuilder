using Umbraco.Core;

namespace Vega.USiteBuilder
{
    /// <summary>
    /// Internal class
    /// </summary>
    public class SiteBuilderApplicationBase : IApplicationEventHandler
    {
        /// <summary>
        /// Executes after the ApplicationContext and plugin resolvers are created
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {            
        }

        /// <summary>
        /// Executes before resolution is frozen so that you are able to modify any plugin resolvers
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {            
        }

        /// <summary>
        /// Executes after resolution is frozen so you can get objects from the plugin resolvers. This is the most common method to put logic in.
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            WebConfigManager.AddHttpModule("USiteBuilderHttpModule", "Vega.USiteBuilder.USiteBuilderHttpModule, Vega.USiteBuilder");
        }
    }
}
