using System;
using System.Collections.Generic;

using umbraco.BusinessLogic;
using umbraco.NodeFactory;

namespace Vega.USiteBuilder
{
    /// <summary>
    /// Base class for all document types.
    /// </summary>
    public abstract class DocumentTypeBase : ContentTypeBase
    {
        #region [Node properties]
        /// <summary>
        /// Create date of this content item.
        /// </summary>
        public DateTime CreateDate { get; internal set; }
        /// <summary>
        /// Last update date of this content item.
        /// </summary>
        public DateTime UpdateDate { get; internal set; }
        /// <summary>
        /// Id of the user who created this content item.
        /// </summary>
        public int CreatorId { get; internal set; }
        /// <summary>
        /// Name of the user who created this content item.
        /// </summary>
        public string CreatorName { get; internal set; }
        /// <summary>
        /// Id of this content item.
        /// </summary>
        public int Id { get; internal set; }
        /// <summary>
        /// Name of this content item.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Nice url of this content item.
        /// </summary>
        public string NiceUrl { get; internal set; }
        /// <summary>
        /// Document Type alias of this content item.
        /// </summary>
        public string NodeTypeAlias { get; internal set; }
        /// <summary>
        /// Id of a parent node of this content item.
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// Path of this content item.
        /// </summary>
        public string Path { get; internal set; }
        /// <summary>
        /// Sort order of this content item.
        /// </summary>
        public int SortOrder { get; internal set; }
        /// <summary>
        /// Template associated with this content item.
        /// </summary>
        public int Template { get; internal set; }
        /// <summary>
        /// Url of this content item
        /// </summary>
        public string Url { get; internal set; }
        /// <summary>
        /// Version of this content item
        /// </summary>
        public Guid Version { get; internal set; }
        /// <summary>
        /// Writer id of this content item
        /// </summary>
        public int WriterID { get; internal set; }
        /// <summary>
        /// Name of the Writer of this content item
        /// </summary>
        public string WriterName { get; internal set; }
        #endregion

        #region [Public methods]
        /// <summary>
        /// Gets all children nodes of a given type from a given node id.
        /// </summary>
        /// <typeparam name="T">Strongly typed content item</typeparam>
        /// <param name="deepGet">If true it does deep search for children in the whole content tree starting from node whose id is parentId)</param>
        public IEnumerable<T> GetChildren<T>(bool deepGet)
            where T : DocumentTypeBase, new()
        {
            return ContentHelper.GetChildren<T>(this.Id, deepGet);
        }

        /// <summary>
        /// Gets all children nodes of a given type from a given node id.
        /// Note: This method returns only first level children - it doesn't return children's children.
        /// </summary>
        /// <typeparam name="T">Strongly typed content item</typeparam>
        public IEnumerable<T> GetChildren<T>()
            where T : DocumentTypeBase, new()
        {
            return ContentHelper.GetChildren<T>(this.Id);
        }

        /// <summary>
        /// Gets all children nodes from a given node id.
        /// Note: This method returns only first level children - it doesn't return children's children.
        /// </summary>
        public IEnumerable<DocumentTypeBase> GetChildren()
        {
            return ContentHelper.GetChildren(this.Id);
        }

        /// <summary>
        /// Gets all children nodes from a given node id.
        /// </summary>
        /// <param name="deepGet">if set to <c>true</c> method will return children's children (complete tree).</param>
        /// <returns></returns>
        public IEnumerable<DocumentTypeBase> GetChildren(bool deepGet)
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
