using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Security;
using umbraco;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.web;
using umbraco.NodeFactory;

namespace Vega.USiteBuilder.Repositories
{
    /// <summary>
    /// Default node repository implementation. 
    /// </summary>
    public class NodeRepository : INodeRepository
    {
        static NodeRepository()
        {
            Current = new NodeRepository();
        }

        /// <summary>
        /// The current node repository
        /// </summary>
        public static INodeRepository Current { get; private set; }

        private static readonly object SyncRoot = new object();

        public virtual Node RootNode
        {
            get { return new Node(-1); }
        }

        /// <summary>
        /// Sets the current node repository implementation
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static INodeRepository SetCurrent(INodeRepository repository)
        {
            lock (SyncRoot)
            {
                var result = Current;
                Current = repository;
                return result;
            }
        }

        public virtual Node GetNodeById(int nodeId)
        {
            return uQuery.GetNode(nodeId);
        }

        public virtual bool CurrentUserHasAccess(Node node)
        {
            var result = MemberHasAccess(node.Id, node.Path);
            return result;
        }

        public virtual bool IsProtected(int documentId, string path)
        {
            return Access.IsProtected(documentId, path);
        }

        public virtual bool MemberHasAccess(int nodeId, string path)
        {
            if (!this.IsProtected(nodeId, path))
                return true;
            if (Member.IsLoggedOn())
                return Access.HasAccess(nodeId, path, Membership.GetUser());
            else
                return false;
        }

        public virtual bool IsAvailableForCurrentUser(Node node)
        {
            return node != null && !IsInRecycleBin(node.Path) && CurrentUserHasAccess(node);
        }

        public virtual IEnumerable<Node> FilterAvailableForCurrentUser(IEnumerable<Node> nodes)
        {
            return nodes.Where(IsAvailableForCurrentUser);
        }

        public virtual bool IsInRecycleBin(string path)
        {
            return ContentHelper.IsInRecycleBin(path);
        }

        public virtual IEnumerable<Node> GetChildren(Node node)
        {
            return node.GetChildNodes();
        }

        public virtual IEnumerable<Node> GetChildrenOfType(Node node, string nodeTypeAlias)
        {
            return FilterByType(GetChildren(node), nodeTypeAlias);
        }
        
        public virtual IEnumerable<Node> GetAncestors(Node node)
        {
            return node.GetAncestorNodes();
        }

        public virtual IEnumerable<Node> GetAncestorsOrSelf(Node node)
        {
            return node.GetAncestorOrSelfNodes();
        }

        public virtual IEnumerable<Node> GetDescendants(Node node)
        {
            return node.GetDescendantNodes();
        }

        public virtual IEnumerable<Node> FilterByType(IEnumerable<Node> nodes, string nodeTypeAlias)
        {
            return nodes.Where(n => n.NodeTypeAlias == nodeTypeAlias);
        }
    }
}
