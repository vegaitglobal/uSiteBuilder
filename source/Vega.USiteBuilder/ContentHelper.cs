using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using umbraco;
using umbraco.cms.businesslogic.web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Vega.USiteBuilder.DocumentTypeBuilder;
using Vega.USiteBuilder.Types;

namespace Vega.USiteBuilder
{
    /// <summary>
    /// This class contains methods for getting the strongly typed content from Umbraco
    /// </summary>
    public static class ContentHelper
    {
        /// <summary>
        /// Contains list of all custom type convertors.
        /// </summary>
        internal static Dictionary<Type, ICustomTypeConvertor> PropertyConvertors = new Dictionary<Type, ICustomTypeConvertor>();
        static readonly IContentService ContentService = ApplicationContext.Current.Services.ContentService;

        /// <summary>
        /// Registers document type property convertor.
        /// </summary>
        /// <param name="propertyType">Document type property type</param>
        /// <param name="convertor">Convertor implementation</param>
        internal static void RegisterDocumentTypePropertyConvertor(Type propertyType, ICustomTypeConvertor convertor)
        {
            if (!PropertyConvertors.ContainsKey(propertyType))
            {
                PropertyConvertors.Add(propertyType, convertor);
            }
        }

        /// <summary>
        /// Gets the current content being rendered.
        /// </summary>
        /// <returns></returns>
        public static DocumentTypeBase GetCurrentContent()
        {
            return GetByNode(umbraco.NodeFactory.Node.GetCurrent());
        }

        /// <summary>
        /// Gets all children nodes of a given type from a given node id.
        /// </summary>
        /// <typeparam name="T">Strongly typed content item</typeparam>
        /// <param name="parentId">Parent node id of all children to get</param>
        /// <param name="deepGet">If true it does deep search for children in the whole content tree starting from node whose id is parentId)</param>
        public static List<T> GetChildren<T>(int parentId, bool deepGet)
            where T : DocumentTypeBase, new()
        {
            List<T> retVal = new List<T>();

            var parentNode = new umbraco.NodeFactory.Node(parentId);

            string docTypeAlias = DocumentTypeManager.GetDocumentTypeAlias(typeof(T));

            foreach (umbraco.NodeFactory.Node childNode in parentNode.Children)
            {
                // Check if this childNode is of a given document type and if not deleted
                if (docTypeAlias == childNode.NodeTypeAlias && !IsInRecycleBin(childNode.Path))
                {
                    var d = GetByNode<T>(childNode);
                    if (d != null)
                    {
                        retVal.Add(d);
                    }
                }

                if (deepGet)
                {
                    retVal.AddRange(GetChildren<T>(childNode.Id, true));
                }
            }

            return retVal;
        }

        /// <summary>
        /// Gets all children nodes of a given type from a given node id.
        /// Note: This method returns only first level children - it doesn't return children's children.
        /// </summary>
        /// <typeparam name="T">Strongly typed content item</typeparam>
        /// <param name="parentId">Parent node id of all children to get</param>
        public static List<T> GetChildren<T>(int parentId)
            where T : DocumentTypeBase, new()
        {
            return GetChildren<T>(parentId, false);
        }

        /// <summary>
        /// Gets all children nodes from a given node id.
        /// Note: This method returns only first level children - it doesn't return children's children.
        /// </summary>
        /// <param name="parentId">Parent node id of all children to get</param>
        public static List<DocumentTypeBase> GetChildren(int parentId)
        {
            return GetChildren(parentId, false);
        }

        /// <summary>
        /// Gets all children nodes from a given node id.
        /// </summary>
        /// <param name="parentId">Parent node id of all children to get</param>
        /// <param name="deepGet">if set to <c>true</c> method will return children's children (complete tree).</param>
        /// <returns></returns>
        public static List<DocumentTypeBase> GetChildren(int parentId, bool deepGet)
        {
            List<DocumentTypeBase> retVal = new List<DocumentTypeBase>();

            var parentNode = new umbraco.NodeFactory.Node(parentId);

            if (parentNode.Id == parentId && parentNode.Children != null) // check if it is loaded correctly
            {
                foreach (umbraco.NodeFactory.Node childNode in parentNode.Children)
                {
                    // Check if this childNode is not deleted
                    if (!IsInRecycleBin(childNode.Path))
                    {
                        var d = GetByNode(childNode);
                        if (d != null)
                        {
                            retVal.Add(d);
                        }
                    }

                    if (deepGet)
                    {
                        retVal.AddRange(GetChildren(childNode.Id, true));
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Get's the content item by node id.
        /// </summary>
        /// <typeparam name="T">Strongly typed content item</typeparam>
        /// <param name="nodeId">Node Id associated with the content item</param>
        /// <returns>Content item</returns>
        public static T GetByNodeId<T>(int nodeId)
            where T : DocumentTypeBase, new()
        {
            var node = new umbraco.NodeFactory.Node(nodeId);

            return GetByNode<T>(node);
        }


        /// <summary>
        /// Get's the content item by node.
        /// </summary>
        /// <typeparam name="T">Strongly typed content item</typeparam>
        /// <param name="node">node associated with the content item</param>
        /// <returns>Content item</returns>
        public static T GetByNode<T>(umbraco.NodeFactory.Node node)
             where T : DocumentTypeBase, new()
        {
            DocumentTypeBase retVal = GetByNode(node);
            if (retVal != null)
            {
                if (retVal is T)
                {
                    return retVal as T;
                }
                else
                {
                    throw new Exception(string.Format("Cannot convert document type '{0}' to document type '{1}' or document type not found. Node id: '{2}', node name: '{3}'",
                        node.NodeTypeAlias, DocumentTypeManager.GetDocumentTypeAlias(typeof(T)), node.Id, node.Name));
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get's the content item by node id.
        /// </summary>
        /// <param name="nodeId">Node Id associated with the content item</param>
        /// <returns>Content item</returns>
        public static DocumentTypeBase GetByNodeId(int nodeId)
        {
            var node = new umbraco.NodeFactory.Node(nodeId);

            return GetByNode(node);
        }

  

        /// <summary>
        /// Get's the content item by node.
        /// </summary>
        /// <param name="node">node associated with the content item</param>
        /// <returns>Content item</returns>
        public static DocumentTypeBase GetByNode(umbraco.NodeFactory.Node node)
        {
            if (node == null || node.NodeTypeAlias == null || node.Id == 0)
            {
                return null;
            }

            DocumentTypeBase retVal = null;

            Type typeDocType = DocumentTypeManager.GetDocumentTypeType(node.NodeTypeAlias);
            if (typeDocType != null)
            {
                ConstructorInfo constructorInfo = typeDocType.GetConstructor(new[] { typeof(int) });
                if (constructorInfo == null)
                {
                    retVal = (DocumentTypeBase)Activator.CreateInstance(typeDocType);
                }

                else
                {
                    retVal = (DocumentTypeBase)constructorInfo.Invoke(new object[] { node.Id });
                }

                foreach (PropertyInfo propInfo in typeDocType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    DocumentTypePropertyAttribute propAttr = Util.GetAttribute<DocumentTypePropertyAttribute>(propInfo);
                    if (propAttr == null)
                    {
                        continue; // skip this property - not part of a Document Type
                    }

                    string propertyName;
                    string propertyAlias;
                    DocumentTypeManager.ReadPropertyNameAndAlias(propInfo, propAttr, out propertyName, out propertyAlias);

                    var property = node.GetProperty(propertyAlias);

                    object value = null;
                    try
                    {
                        if (property == null)
                        {
                            var objVal = propInfo.GetValue(retVal, new object[]{});
                            if (objVal != null) value = objVal;
                        }
                        else if (propInfo.PropertyType == typeof(Boolean))
                        {
                            if (String.IsNullOrEmpty(property.Value) || property.Value == "0")
                            {
                                value = false;
                            }
                            else
                            {
                                value = true;
                            }
                        }
                        else if (propAttr.CustomTypeConverter != null)
                        {
                            value = ((ICustomTypeConvertor)Activator.CreateInstance(propAttr.CustomTypeConverter)).ConvertValueWhenRead(property.Value);
                        }
                        else if (PropertyConvertors.ContainsKey(propInfo.PropertyType))
                        {
                            // will be transformed later. TODO: move transformation here
                            value = GetInnerXml(node.Id.ToString(), propertyAlias);
                        }
                        else if (String.IsNullOrEmpty(property.Value))
                        {
                            // if property type is string or if it's some custom type, try to get the inner xml of this property within a node.
                            if (propInfo.PropertyType == typeof(string) ||
                                PropertyConvertors.ContainsKey(propInfo.PropertyType))
                            {
                                value = GetInnerXml(node.Id.ToString(), propertyAlias);
                                if (value == null && propInfo.PropertyType == typeof(string))
                                {
                                    value = string.Empty;
                                }
                            }
                            else
                            {
                                value = null;
                            }
                        }
                        else if (propInfo.PropertyType.IsGenericType &&
                                 propInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            value = Convert.ChangeType(property.Value, Nullable.GetUnderlyingType(propInfo.PropertyType));

                            // TODO: If data type is DateTime and is nullable and is less than 1.1.1000 than set it to NULL
                        }
                        else if (propInfo.PropertyType == typeof(HtmlString))
                        {
                            value = new HtmlString(property.Value);
                        }

                        else
                        {
                            value = Convert.ChangeType(property.Value, propInfo.PropertyType);
                        }

                        propInfo.SetValue(retVal, value, null);
                    }
                    catch (Exception exc)
                    {
                        throw new Exception(string.Format("Cannot set the value of a document type property {0}.{1} (document type: {2}) to value: '{3}' (value type: {4}). Error: {5}",
                            typeDocType.Name, propInfo.Name, propInfo.PropertyType.FullName,
                            value, value != null ? value.GetType().FullName : "", exc.Message));
                    }
                }
            }

            return retVal;
        }

        private static string GetInnerXml(string nodeId, string propertyAlias)
        {
            string retVal = null;

            XmlNode node = content.Instance.XmlContent.GetElementById(nodeId);
            if (node != null)
            {
                XmlNode propertyNode = node.SelectSingleNode(propertyAlias);
                if (propertyNode != null && propertyNode.FirstChild != null)
                {
                    if (propertyNode.FirstChild.GetType() != typeof(XmlCDataSection))
                    {
                        retVal = propertyNode.InnerXml;
                    }
                    else
                    {
                        retVal = propertyNode.InnerText;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Gets the content by XPath query
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns>List of content that matches the XPath query</returns>
        public static List<DocumentTypeBase> SelectContentNodes(string xpath)
        {
            List<DocumentTypeBase> retVal = new List<DocumentTypeBase>();

            XPathNodeIterator rootDocIterator = library.GetXmlAll();

            XmlDocument rootDoc = new XmlDocument();
            rootDoc.LoadXml(rootDocIterator.Current.OuterXml);

            XmlNodeList nodes = rootDoc.SelectNodes(xpath);

            foreach (XmlNode node in nodes)
            {
                var n = new umbraco.NodeFactory.Node(node);

                var d = GetByNode(n);
                if (d != null)
                {
                    retVal.Add(d);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Updates or adds the content item using current user. If content item already exists, it updates it.
        /// If content item doesn't exists, it creates new content item.
        /// NOTE: Set the ParentId property of this item.
        /// </summary>
        /// <param name="contentItem">Content item to update/add</param>
        /// <param name="publish">If set to <c>true</c> it contentItem will be published as well.</param>
        public static void Save(DocumentTypeBase contentItem, bool publish)
        {
            Save(contentItem, Util.GetAdminUser().Id, publish);
        }

        /// <summary>
        /// Updates or adds the content item using current user. If content item already exists, it updates it. 
        /// If content item doesn't exists, it creates new content item.
        /// NOTE: Set the ParentId property of this item.
        /// </summary>
        /// <param name="contentItem">Content item to update/add</param>
        public static void Save(DocumentTypeBase contentItem)
        {
            Save(contentItem, Util.GetAdminUser().Id, true);
        }


        /// <summary>
        /// Updates or adds the content item. If content item already exists, it updates it. 
        /// If content item doesn't exists, it creates new content item (in that case contentItem.Id will be set to newly created id).
        /// NOTE: Set the ParentId property of this item.
        /// </summary>
        /// <param name="contentItem">Content item to update/add</param>
        /// <param name="userId">User used for add or updating the content</param>
        /// <param name="publish">If set to <c>true</c> it contentItem will be published as well.</param>
        public static void Save(DocumentTypeBase contentItem, int userId, bool publish)
        {
            if (contentItem.Parent.Id < 1)
            {
                throw new ArgumentException("Parent property cannot be null");
            }

            if (String.IsNullOrEmpty(contentItem.Name))
            {
                throw new Exception("Name property of this content item is not set");
            }

            IContentType contentType = DocumentTypeManager.GetDocumentType(contentItem.GetType());

            IContent content;
            if (contentItem.Id == 0) // content item is new so create Document
            {
                content = ContentService.CreateContent(contentItem.Name, contentItem.Parent.Id, contentType.Alias);
            }
            else // content item already exists, so load it
            {
                content = ContentService.GetById(contentItem.Id);
            }

            var documentTypeProperties = contentItem.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            SaveProperties(contentItem, contentType, content, documentTypeProperties);

            if (publish)
            {
                ContentService.SaveAndPublishWithStatus(content, userId);
            }
        }

        /// <summary>
        /// The method saves values from the given properties of the valueSource specified to the content of the contentType type
        /// </summary>
        /// <param name="valueSource">An object to get property values from. Can be either DocumentTypeBase or mixin object reference</param>
        /// <param name="contentType">A content type information</param>
        /// <param name="content">A content to save values to</param>
        /// <param name="properties">A valueSource properties collection</param>
        private static void SaveProperties(object valueSource, IContentType contentType, IContent content, IEnumerable<PropertyInfo> properties)
        {
            foreach (var propInfo in properties)
            {
                try
                {
                    var mixinAttribute = Util.GetAttribute<MixinPropertyAttribute>(propInfo);
                    if (mixinAttribute != null)
                    {
                        var mixin = propInfo.GetValue(valueSource, null);
                        if (mixin != null)
                        {
                            var mixinType = mixinAttribute.GetMixinType(propInfo);
                            var mixinProperties = mixinType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                            SaveProperties(mixin, contentType, content, mixinProperties);
                        }
                        continue;
                    }

                    DocumentTypePropertyAttribute propAttr = Util.GetAttribute<DocumentTypePropertyAttribute>(propInfo);
                    if (propAttr == null)
                    {
                        continue; // skip this property - not part of a Document Type
                    }

                    string propertyName;
                    string propertyAlias;
                    DocumentTypeManager.ReadPropertyNameAndAlias(propInfo, propAttr, out propertyName, out propertyAlias);

                    PropertyType property = content.PropertyTypes.FirstOrDefault(p => p.Alias == propertyAlias);
                    if (property == null)
                    {
                        throw new Exception(string.Format("Property '{0}' not found in this node: {1}. Content type: {2}.",
                            propertyAlias, content.Id, contentType.Alias));
                    }

                    if (PropertyConvertors.ContainsKey(propInfo.PropertyType))
                    {
                        content.SetValue(propertyAlias, PropertyConvertors[propInfo.PropertyType].ConvertValueWhenWrite(propInfo.GetValue(valueSource, null)));
                    }
                    else
                    {
                        content.SetValue(propertyAlias, propInfo.GetValue(valueSource, null));
                    }
                }
                catch (Exception exc)
                {
                    throw new Exception(String.Format("Error while saving property: {0}.{1}. Error: {2}, Stack trace: {3}", contentType.Alias, propInfo.Name, exc.Message, exc.StackTrace), exc);
                }
            }
        }

        /// <summary>
        /// Deletes the content specified by id.
        /// </summary>
        /// <param name="id">The node id to delete.</param>
        /// <param name="deletePermanently">if set to <c>true</c>, node will be deleted without moving to Trash (otherwise items is moved to Trash).</param>
        public static void DeleteContent(int id, bool deletePermanently)
        {
            var document = ContentService.GetById(id);
            if (deletePermanently)
            {
                ContentService.Delete(document);
            }
            else
            {
                ContentService.MoveToRecycleBin(document);
            }

            library.RefreshContent();
        }

        /// <summary>
        /// Deletes the content specified by id (moves items to Trash)
        /// </summary>
        /// <param name="id">Content item id</param>
        public static void DeleteContent(int id)
        {
            DeleteContent(id, false);
        }

        /// <summary>
        /// Returns true if content item is deleted and currently contained in the recycle bin.
        /// </summary>
        /// <param name="contentItem">Content item</param>
        public static bool IsInRecycleBin(DocumentTypeBase contentItem)
        {
            return contentItem.Path.Contains(string.Format(",{0},", Constants.UmbracoRecycleBinId));
        }

        /// <summary>
        /// Checks if node with a given path is in the Recycle bin
        /// </summary>
        /// <param name="path">Node path</param>
        /// <returns>true if in recycle bin</returns>
        public static bool IsInRecycleBin(string path)
        {
            return path.Contains(string.Format(",{0},", Constants.UmbracoRecycleBinId));
        }
    }
}
