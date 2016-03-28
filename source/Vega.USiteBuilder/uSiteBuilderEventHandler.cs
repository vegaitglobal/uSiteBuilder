using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Umbraco.Core;

namespace Vega.USiteBuilder
{
    public class uSiteBuilderEventHandler : IApplicationEventHandler 
    {
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            if (!Configuration.USiteBuilderConfiguration.SuppressSynchronization)
            {
                UmbracoManager.SynchronizeIfNotSynchronized();
            }
        }
    }
}
