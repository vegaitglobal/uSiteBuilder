using Umbraco.Core;
using Vega.USiteBuilder.Configuration;

namespace Vega.USiteBuilder
{
    /// <summary>
    /// uSiteBuilder startup
    /// </summary>
    public class uSiteBuilderEventHandler : IApplicationEventHandler 
    {
        /// <summary>
        /// Called when [application initialized].
        /// </summary>
        /// <param name="umbracoApplication">The umbraco application.</param>
        /// <param name="applicationContext">The application context.</param>
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        /// <summary>
        /// Called when [application starting].
        /// </summary>
        /// <param name="umbracoApplication">The umbraco application.</param>
        /// <param name="applicationContext">The application context.</param>
        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        /// <summary>
        /// Called when [application started].
        /// </summary>
        /// <param name="umbracoApplication">The umbraco application.</param>
        /// <param name="applicationContext">The application context.</param>
        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            if (!USiteBuilderConfiguration.SuppressSynchronization)
            {
                UmbracoManager.SynchronizeIfNotSynchronized();
            }
        }
    }
}
