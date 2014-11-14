using System;

using Castle.DynamicProxy;

using umbraco.NodeFactory;
using umbraco;

namespace Vega.USiteBuilder
{
    public class DocumentTypeActivator
    {
        private ProxyGenerator _generator;
        private ProxyGenerationOptions _options;
        private IInterceptor[] _interceptors;

        public DocumentTypeActivator()
        {
            _generator = new ProxyGenerator();
            _options = new ProxyGenerationOptions(new DocumentTypePropertiesProxyGenerationHook());
            _interceptors = new IInterceptor[] 
                               {
                                   new DocumentTypePropertyInterceptor()
                               };
        }

        public virtual T CreateAndPopulateTypedInstance<T>(Node node) where T : DocumentTypeBase
        {
            T retVal = null;
            if (node != null)
            {
                Type typeDocType = DocumentTypeManager.GetDocumentTypeType(node.NodeTypeAlias);
                if (typeDocType == null)
                {
                    var message =
                        string.Format(
                            "Error processing document with id {0} of type '{1}' - no code for the document type found. ",
                            node.Id, node.NodeTypeAlias);

                    throw new Exception(message);
                }

                T typedPage = (T)CreateInstance(typeDocType);
                if (ContentHelper.PopuplateInstance<T>(node, typeDocType, typedPage))
                {
                    retVal = typedPage;
                }
            }

            return retVal;
        }

        public virtual T CreateAndPopulateTypedInstance<T>(int nodeId) where T : DocumentTypeBase
        {
            Node node = uQuery.GetNode(nodeId);
            return CreateAndPopulateTypedInstance<T>(node);
        }

        public virtual object CreateInstance(Type typeDocType)
        {
            return CreateInstance(typeDocType, new object[] {});
        }

        protected virtual object CreateInstance(Type typeDocType, object[] ctorArguments)
        {
            return _generator.CreateClassProxy(typeDocType, new Type[] { }, _options, ctorArguments, _interceptors);
        }
    }
}
