using System.Linq;
using Umbraco.Core.Models;
using umbraco.MacroEngines;
using umbraco.NodeFactory;

namespace Vega.USiteBuilder
{
    using System;
    using System.Collections.Generic;
    using umbraco.BusinessLogic;

    /// <summary>
    /// Base class for all document types.
    /// </summary>
    public abstract class DocumentTypeBase : DynamicNode
    {
        public DocumentTypeBase()
        {
        }

        protected DocumentTypeBase(int nodeId)
            : base(nodeId)
        {
        }
        
        #region [Public methods]


        public T TypedAncestorOrSelf<T>() where T : DocumentTypeBase, new()
        {
            DynamicNode ancestorOrSelf = this.AncestorOrSelf(DocumentTypeManager.GetDocumentTypeAlias(typeof (T)));

            return ContentHelper.GetByNodeId<T>(ancestorOrSelf.Id);
        }

        /// <summary>
        /// Gets all descendant node of a given type from this node.
        /// </summary>
        /// <typeparam name="T">Strongly typed content item</typeparam>
        public IEnumerable<T> TypedDescendants<T>() where T : DocumentTypeBase, new()
        {
            DynamicNodeList descendants = Descendants(DocumentTypeManager.GetDocumentTypeAlias(typeof(T)));

            return descendants.Items.Select(d => ContentHelper.GetByNodeId<T>(d.Id));
        }

        /// <summary>
        /// Gets all children of a given type from this node.
        /// </summary>
        /// <typeparam name="T">Strongly typed content item</typeparam>
        public IEnumerable<T> TypedChildren<T>() where T : DocumentTypeBase, new()
        {
            var documentTypeAlias = DocumentTypeManager.GetDocumentTypeAlias(typeof (T));
            IEnumerable<DynamicNode> descendants = this.ChildrenAsList.Where(c => c.NodeTypeAlias == documentTypeAlias);

            return descendants.Select(d => ContentHelper.GetByNodeId<T>(d.Id));
        }


        /// <summary>
        /// Gets all children nodes of a given type from a given node id.
        /// </summary>
        /// <typeparam name="T">Strongly typed content item</typeparam>
        /// <param name="deepGet">If true it does deep search for children in the whole content tree starting from this item)</param>
        public List<T> GetChildren<T>(bool deepGet)
            where T : DocumentTypeBase, new()
        {
            return ContentHelper.GetChildren<T>(this.Id, deepGet);
        }

        /// <summary>
        /// Gets all children nodes of a given type from a given node id.
        /// Note: This method returns only first level children - it doesn't return children's children.
        /// </summary>
        /// <typeparam name="T">Strongly typed content item</typeparam>
        public List<T> GetChildren<T>()
            where T : DocumentTypeBase, new()
        {
            return ContentHelper.GetChildren<T>(this.Id);
        }

        /// <summary>
        /// Gets all children nodes from a given node id.
        /// Note: This method returns only first level children - it doesn't return children's children.
        /// </summary>
        public List<DocumentTypeBase> GetChildren()
        {
            return ContentHelper.GetChildren(this.Id);
        }

        /// <summary>
        /// Gets all children nodes from a given node id.
        /// </summary>
        /// <param name="deepGet">if set to <c>true</c> method will return children's children (complete tree).</param>
        /// <returns></returns>
        public List<DocumentTypeBase> GetChildren(bool deepGet)
        {
            return ContentHelper.GetChildren(this.Id);
        }

                /// <summary>
        /// Updates or adds the content item using current user. If content item already exists, it updates it.
        /// If content item doesn't exists, it creates new content item.
        /// NOTE: Set the ParentId property of this item.
        /// </summary>
        /// <param name="publish">If set to <c>true</c> it contentItem will be published as well.</param>
        public void Save(bool publish)
        {
            ContentHelper.Save(this, publish);
        }

        /// <summary>
        /// Updates or adds the content item using current user. If content item already exists, it updates it. 
        /// NOTE: Set the ParentId property of this item.
        /// If content item doesn't exists, it creates new content item.
        /// </summary>
        public void Save()
        {
            ContentHelper.Save(this);
        }

        /// <summary>
        /// Updates or adds the content item. If content item already exists, it updates it. 
        /// NOTE: Set the ParentId property of this item.
        /// If content item doesn't exists, it creates new content item (in that case contentItem.Id will be set to newly created id).
        /// </summary>
        /// <param name="user">User used for add or updating the content</param>
        /// <param name="publish">If set to <c>true</c> it contentItem will be published as well.</param>
        public void Save(User user, bool publish)
        {
            ContentHelper.Save(this, user, publish);
        }
        #endregion
    }
}
