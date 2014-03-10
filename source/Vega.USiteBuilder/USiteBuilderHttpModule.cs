namespace Vega.USiteBuilder
{
    using System;
    using System.Web;

    internal class USiteBuilderHttpModule : IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequest;
        }

        void BeginRequest(object sender, EventArgs e)
        {
            UmbracoManager.SynchronizeIfNotSynchronized();
        }

        #endregion
    }
}
