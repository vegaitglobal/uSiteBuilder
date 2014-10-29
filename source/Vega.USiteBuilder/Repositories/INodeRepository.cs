using System.Collections.Generic;
using umbraco.NodeFactory;

namespace Vega.USiteBuilder.Repositories
{
    /// <summary>
    /// Node repository
    /// </summary>
    public interface INodeRepository
    {
        /// <summary>
        /// Gets the very root node. 
        /// </summary>
        Node RootNode { get; }

        /// <summary>
        /// Gets node by id
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        Node GetNodeById(int nodeId);

        /// <summary>
        /// Indicates whether the current user has read permission
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool CurrentUserHasAccess(Node node);

        /// <summary>
        /// Filters nodes accessible for current user:
        ///  - not in recycle bin
        ///  - which user can read
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        IEnumerable<Node> FilterAvailableForCurrentUser(IEnumerable<Node> nodes);

        /// <summary>
        /// Indicates whether the given path in recycle bin.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool IsInRecycleBin(string path);

        /// <summary>
        /// Gets immediate child nodes. 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IEnumerable<Node> GetChildren(Node node);

        /// <summary>
        /// Gets immediate child nodes. 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeTypeAlias"></param>
        /// <returns></returns>
        IEnumerable<Node> GetChildrenOfType(Node node, string nodeTypeAlias);
        
        /// <summary>
        /// Gets ancestor nodes
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IEnumerable<Node> GetAncestors(Node node);

        /// <summary>
        /// Gets ancestors plus self nodes
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IEnumerable<Node> GetAncestorsOrSelf(Node node);

        /// <summary>
        /// Gets descendant nodes
        /// WARN: can be slow!
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IEnumerable<Node> GetDescendants(Node node);

        /// <summary>
        /// Filters the given collection by exact node type
        /// </summary>
        /// <param name="nodes">collection to filter</param>
        /// <param name="nodeTypeAlias">node type alias</param>
        /// <returns></returns>
        IEnumerable<Node> FilterByType(IEnumerable<Node> nodes, string nodeTypeAlias);

        /// <summary>
        /// Copied from UmbracoHelper. Indicates whether the given document access is protected. 
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        bool IsProtected(int documentId, string path);

        /// <summary>
        /// Copied from UmbracoHelper. Indicates whether current user (Membership.GetUser()) has access to the given node. 
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        bool MemberHasAccess(int nodeId, string path);

        /// <summary>
        /// Indicates whether the given node is available for the current user. Which means:
        /// - node is not null
        /// - node is not deleted
        /// - current user has permissions to read it
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool IsAvailableForCurrentUser(Node node);
    }
}