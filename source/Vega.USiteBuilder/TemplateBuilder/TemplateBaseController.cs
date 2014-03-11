using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Mvc;
using umbraco.NodeFactory;
using System.Web.Mvc;
using Umbraco.Web.Models;

namespace Vega.USiteBuilder.TemplateBuilder
{
    public class TemplateBaseController : RenderMvcController
    {
        private DocumentTypeBase _currentContent;

        /// <summary>
        /// Gets the current content item.
        /// </summary>
        public DocumentTypeBase CurrentContent
        {
            get
            {
                if (this._currentContent == null)
                {
                    int nodeId = Node.getCurrentNodeId();
                    if (!HttpContext.Items.Contains(nodeId))
                    {
                        DocumentTypeBase item = DocumentTypeResolver.Instance.GetTyped<DocumentTypeBase>(nodeId);

                        if (item == null)
                            throw new Exception(string.Format("The document type {0} does not exist in Umbraco, please run sync again. (siteBuilderSupressSynchronization=false)", typeof(DocumentTypeBase).Name));

                        HttpContext.Items.Add(nodeId, item);
                    }

                    this._currentContent = (DocumentTypeBase)HttpContext.Items[nodeId];
                }

                return this._currentContent;
            }
        }

        public override ActionResult Index(RenderModel model)
        {
            return CurrentTemplate(this.CurrentContent);
        }
    }

    public class TemplateControllerBase<T> : RenderMvcController
        where T : DocumentTypeBase, new()
    {
        private T _currentContent;

        /// <summary>
        /// Gets the current content item.
        /// </summary>
        public T CurrentContent
        {
            get
            {
                if (this._currentContent == null)
                {
                    int nodeId = Node.getCurrentNodeId();
                    if (!HttpContext.Items.Contains(nodeId))
                    {
                        T item = DocumentTypeResolver.Instance.GetTyped<T>(nodeId);

                        if (item == null)
                            throw new Exception(string.Format("The document type {0} does not exist in Umbraco, please run sync again. (siteBuilderSupressSynchronization=false)", typeof(T).Name));

                        HttpContext.Items.Add(nodeId, item);
                    }

                    this._currentContent = (T)HttpContext.Items[nodeId];
                }

                return this._currentContent;
            }
        }

        public override ActionResult Index(Umbraco.Web.Models.RenderModel model)
        {
            return CurrentTemplate(CurrentContent);
        }
    }
}
