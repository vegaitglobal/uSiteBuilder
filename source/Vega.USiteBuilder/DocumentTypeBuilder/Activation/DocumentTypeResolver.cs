using System;

using umbraco.NodeFactory;

namespace Vega.USiteBuilder
{
    public class DocumentTypeResolver
    {
        private static DocumentTypeResolver _instance;

        public static DocumentTypeResolver Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DocumentTypeResolver();

                return _instance;
            }
        }

        public DocumentTypeActivator Activator { get; set; }

        private DocumentTypeResolver()
        {
            Activator = new DocumentTypeActivator();
        }

        public virtual T GetTyped<T>(Node node) where T : DocumentTypeBase
        {
            return Activator.CreateAndPopulateTypedInstance<T>(node);
        }

        public virtual T GetTyped<T>(int nodeId) where T : DocumentTypeBase
        {
            return Activator.CreateAndPopulateTypedInstance<T>(nodeId);
        }
    }
}
