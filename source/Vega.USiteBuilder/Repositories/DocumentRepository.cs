using System.Collections.Generic;
using System.Linq;
using umbraco.NodeFactory;

namespace Vega.USiteBuilder.Repositories
{
    /// <summary>
    /// Default IDocumentRepository implementation. 
    /// </summary>
    public class DocumentRepository : IDocumentRepository
    {
        static DocumentRepository()
        {
            Current = new DocumentRepository(Repositories.NodeRepository.Current);
        }

        public static IDocumentRepository Current { get; private set; }

        protected INodeRepository NodeRepository { get; set; }

        public DocumentRepository(INodeRepository nodeRepository)
        {
            NodeRepository = nodeRepository;
        }

        public virtual TContent GetById<TContent>(int nodeId)
            where TContent : DocumentTypeBase
        {
            return DocumentTypeResolver.Instance.GetTyped<DocumentTypeBase>(nodeId) as TContent;
        }

        public virtual TContent GetFromNode<TContent>(Node node)
            where TContent : DocumentTypeBase
        {
            return DocumentTypeResolver.Instance.GetTyped<DocumentTypeBase>(node) as TContent;
        }

        public virtual IEnumerable<TContent> GetFromNodes<TContent>(IEnumerable<Node> nodes)
            where TContent : DocumentTypeBase
        {
            return nodes.Select(GetFromNode<DocumentTypeBase>).OfType<TContent>();
        }

        public virtual IEnumerable<TContent> GetFromNodesOfExactType<TContent>(IEnumerable<Node> nodes)
            where TContent : DocumentTypeBase
        {
            return GetFromNodes<TContent>(NodeRepository.FilterByType(nodes, DocumentTypeHelper.GetDocumentTypeAlias<TContent>()));
        }

        public virtual IEnumerable<TContent> GetFromNodesAvailableForCurrentUser<TContent>(IEnumerable<Node> nodes)
            where TContent : DocumentTypeBase
        {
            return GetFromNodes<TContent>(NodeRepository.FilterAvailableForCurrentUser(nodes));
        }

        public virtual IEnumerable<TContent> GetChildren<TContent>(int parentNodeId)
            where TContent : DocumentTypeBase
        {
            var parent = NodeRepository.GetNodeById(parentNodeId);

            return GetFromNodes<TContent>(NodeRepository.GetChildren(parent));
        }

        public virtual IEnumerable<TContent> GetChildrenOfExactType<TContent>(int parentNodeId)
            where TContent : DocumentTypeBase
        {
            var parent = NodeRepository.GetNodeById(parentNodeId);

            return GetFromNodes<TContent>(NodeRepository.GetChildrenOfType(parent, DocumentTypeHelper.GetDocumentTypeAlias<TContent>()));
        }

        public virtual IEnumerable<TContent> GetDescendants<TContent>(int parentNodeId)
            where TContent : DocumentTypeBase
        {
            var parent = NodeRepository.GetNodeById(parentNodeId);

            return GetFromNodes<TContent>(NodeRepository.GetDescendants(parent));
        }

        public virtual IEnumerable<TContent> GetAncestors<TContent>(int parentNodeId)
            where TContent : DocumentTypeBase
        {
            var parent = NodeRepository.GetNodeById(parentNodeId);

            return GetFromNodes<TContent>(NodeRepository.GetAncestors(parent));
        }

        public IEnumerable<TContent> GetAncestorsOrSelf<TContent>(int parentNodeId) 
            where TContent : DocumentTypeBase
        {
            var parent = NodeRepository.GetNodeById(parentNodeId);

            return GetFromNodes<TContent>(NodeRepository.GetAncestorsOrSelf(parent));
        }

        public virtual IEnumerable<TContent> SelectNodes<TContent>(string xpath)
            where TContent : DocumentTypeBase
        {
            return ContentHelper.SelectContentNodes(xpath).OfType<TContent>();
        }

        public virtual void Save<TContent>(TContent contentItem)
            where TContent : DocumentTypeBase
        { 
            ContentHelper.Save(contentItem, false);
        }

        public virtual void SaveAsUser<TContent>(TContent contentItem, int userId)
            where TContent : DocumentTypeBase
        {
            ContentHelper.Save(contentItem, userId, false);
        }

        public virtual void SaveAndPublish<TContent>(TContent contentItem)
            where TContent : DocumentTypeBase
        {
            ContentHelper.Save(contentItem, true);
        }

        public virtual void SaveAndPublishAsUser<TContent>(TContent contentItem, int userId)
            where TContent : DocumentTypeBase
        {
            ContentHelper.Save(contentItem, userId, true);
        }

        public virtual void DeleteContent(int nodeId)
        {
            ContentHelper.DeleteContent(nodeId);
        }

        public virtual void DeleteContent<TContent>(TContent contentItem)
            where TContent : DocumentTypeBase
        {
            DeleteContent(contentItem.Id);
        }

        public virtual bool IsInRecycleBin<TContent>(TContent contentItem)
            where TContent : DocumentTypeBase
        {
            return ContentHelper.IsInRecycleBin(contentItem);
        }

        public virtual IEnumerable<TContent> GetRootLevelContents<TContent>()
            where TContent : DocumentTypeBase
        {
            return GetFromNodes<TContent>(NodeRepository.GetChildren(NodeRepository.RootNode));
        }

        public virtual IEnumerable<TContent> GetRootLevelContentsOfExactType<TContent>()
            where TContent : DocumentTypeBase
        {
            return GetFromNodes<TContent>(
                NodeRepository.GetChildrenOfType(NodeRepository.RootNode, DocumentTypeHelper.GetDocumentTypeAlias<TContent>()));
        }
    }
}
