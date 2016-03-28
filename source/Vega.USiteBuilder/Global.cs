namespace Vega.USiteBuilder
{
    using System;

    /// <summary>
    /// Intercepts application events from IIS.
    /// This class subclasses Umbraco Global class so it is safe to deploy Global.asax associated with this class
    /// to IIS website. When deploying this class to Umbraco IIS website, existing App_global.asax.dll
    /// must be deleted (which is safe since this class subclasses Umbraco's Global class).
    /// </summary>
    public class Global : umbraco.Global
    {
        /// <summary>
        /// Intercepts Init function of a Global class. Used to synchronize application data model with Umbraco database.
        /// </summary>
        public override void Init()
        {
            base.Init();

            

            UmbracoManager.SynchronizeIfNotSynchronized();
        }

        /// <summary>
        /// Intercepts begin request event and checks if synchronization is already done. If yes, it does nothing.
        /// If not, it retries synchronization.
        /// </summary>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            UmbracoManager.SynchronizeIfNotSynchronized();

            //System.Web.HttpContext.Current.Application.
        }

/*
        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
 */
    }
}
