using System.Collections.Generic;
using umbraco.NodeFactory;

namespace Vega.USiteBuilder.Repositories
{
    /// <summary>
    /// Document repository - provides access to typed documents. 
    /// </summary>
    public interface IDocumentRepository
    {
        /// <summary>
        /// Gets a typed document by id. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="nodeId"></param>
        /// <returns>Returns null if no document found or document type cannot be converted to TContent. </returns>
        TContent GetById<TContent>(int nodeId)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Gets a typed document from node. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="node"></param>
        /// <returns>Returns null if document type cannot be converted to TContent. </returns>
        TContent GetFromNode<TContent>(Node node)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Gets a collection of typed documents built from nodes collection and filtered by the given content type. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="nodes"></param>
        /// <returns></returns>
        IEnumerable<TContent> GetFromNodes<TContent>(IEnumerable<Node> nodes)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Gets a collection of typed documents built from nodes collection and filtered for the current user exactly of the given content type. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="nodes"></param>
        /// <returns></returns>
        IEnumerable<TContent> GetFromNodesOfExactType<TContent>(IEnumerable<Node> nodes)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Gets a collection of typed documents built from nodes collection filtered for the current user by the given content type. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="nodes"></param>
        /// <returns></returns>
        IEnumerable<TContent> GetFromNodesAvailableForCurrentUser<TContent>(IEnumerable<Node> nodes)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Gets immediate child documents of the given node filtered by the content type. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="parentNodeId"></param>
        /// <returns></returns>
        IEnumerable<TContent> GetChildren<TContent>(int parentNodeId)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Gets immediate child documents of the given node filtered by exact type. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="parentNodeId"></param>
        /// <returns></returns>
        IEnumerable<TContent> GetChildrenOfExactType<TContent>(int parentNodeId)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Gets descendant documents of the given node filtered by the content type. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="parentNodeId"></param>
        /// <returns></returns>
        IEnumerable<TContent> GetDescendants<TContent>(int parentNodeId)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Gets ancestor documents of the given node filtered by the content type. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="parentNodeId"></param>
        /// <returns></returns>
        IEnumerable<TContent> GetAncestors<TContent>(int parentNodeId)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Gets ancestor documents or self of the given node filtered by the content type. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="parentNodeId"></param>
        /// <returns></returns>
        IEnumerable<TContent> GetAncestorsOrSelf<TContent>(int parentNodeId)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Selects nodes using xpath query and filters results by the content type.  
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="xpath"></param>
        /// <returns></returns>
        IEnumerable<TContent> SelectNodes<TContent>(string xpath)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Saves the given document. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="contentItem"></param>
        void Save<TContent>(TContent contentItem)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Saves the given document as user with the specified id. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="contentItem"></param>
        /// <param name="userId"></param>
        void SaveAsUser<TContent>(TContent contentItem, int userId)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Saves and publishes the given document. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="contentItem"></param>
        void SaveAndPublish<TContent>(TContent contentItem)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Saves and publishes the given document as user with the specified id. . 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="contentItem"></param>
        /// <param name="userId"></param>
        void SaveAndPublishAsUser<TContent>(TContent contentItem, int userId)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Deletes the given node. 
        /// </summary>
        /// <param name="nodeId"></param>
        void DeleteContent(int nodeId);

        /// <summary>
        /// Deletes the given document. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="contentItem"></param>
        void DeleteContent<TContent>(TContent contentItem)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Indicates whether the given content in recycle bin. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="contentItem"></param>
        /// <returns></returns>
        bool IsInRecycleBin<TContent>(TContent contentItem)
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Gets the immediate child documents of the root node. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <returns></returns>
        IEnumerable<TContent> GetRootLevelContents<TContent>()
            where TContent : DocumentTypeBase;

        /// <summary>
        /// Gets the immediate child documents of the root node filtered by exact type. 
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <returns></returns>
        IEnumerable<TContent> GetRootLevelContentsOfExactType<TContent>()
            where TContent : DocumentTypeBase;
    }
}