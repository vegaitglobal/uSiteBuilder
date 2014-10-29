using Umbraco.Core.Models;

namespace Vega.USiteBuilder
{
    /// <summary>
    /// This class contains methods for work with meta information about document types
    /// </summary>
    public static class DocumentTypeHelper
    {
        /// <summary>
        /// Gets document type alias for the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetDocumentTypeAlias<T>()
            where T : DocumentTypeBase
        {
            return DocumentTypeManager.GetDocumentTypeAlias(typeof(T));
        }

        /// <summary>
        /// Gets document type name for the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetDocumentTypeName<T>()
        {
            return DocumentTypeManager.GetDocumentType(typeof (T)).Name;
        }

        /// <summary>
        /// Gets document type name for the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IContentType GetDocumentType<T>()
        {
            return DocumentTypeManager.GetDocumentType(typeof(T));
        }
    }
}
