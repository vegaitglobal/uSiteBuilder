using umbraco.DataLayer;

namespace Vega.USiteBuilder
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;
    using umbraco.cms.businesslogic;
    using umbraco.cms.businesslogic.template;
    using umbraco.cms.businesslogic.web;

    /// <summary>
    /// Manages document types synchronization
    /// </summary>
    internal class DocumentTypeManager : ManagerBase
    {
        // Holds all document types found in 
        // Type = Document type type (subclass of DocumentTypeBase), string = document type alias
        private static Dictionary<string, Type> _documentTypes = new Dictionary<string, Type>();

        // indicates if any of synced document types had a default value
        private bool _hadDefaultValues = false;

        /// <summary>
        /// Returns true if there's any document type synchronized (defined)
        /// </summary>
        /// <returns></returns>
        public static bool HasSynchronizedDocumentTypes()
        {
            return _documentTypes.Count > 0;
        }

        public void SynchronizeDocumentType(Type siteBuilderType)
        {
            _documentTypes.Clear();

            SynchronizeDocumentType(siteBuilderType, siteBuilderType.BaseType);

            // create all children document types
            SynchronizeDocumentTypes(siteBuilderType);

            SynchronizeAllowedChildContentType(siteBuilderType);

            // process all allowed children document types
            SynchronizeAllowedChildContentTypes(siteBuilderType);

            if (_hadDefaultValues) // if there were default values set subscribe to News event in which we'll set default values.
            {
                // subscribe to New event
                Document.New += Document_New;
            }
        }

        public void DeleteDocumentType(string alias)
        {
            DocumentType.GetByAlias(alias).delete();
        }

        public void Synchronize()
        {
            _documentTypes.Clear();

            this.SynchronizeDocumentTypes(typeof(DocumentTypeBase));
            this.SynchronizeAllowedChildContentTypes(typeof(DocumentTypeBase));
            this.SynchronizeReverseAllowedChildContentTypes(typeof(DocumentTypeBase));

            if (this._hadDefaultValues) // if there were default values set subscribe to News event in which we'll set default values.
            {
                // subscribe to New event
                Document.New += Document_New;
            }
        }


        #region [Document_New]
        void Document_New(Document document, NewEventArgs e)
        {
            Type typeDocType = DocumentTypeManager.GetDocumentTypeType(document.ContentType.Alias);
            if (typeDocType != null)
            {
                foreach (PropertyInfo propInfo in typeDocType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    DocumentTypePropertyAttribute propAttr = Util.GetAttribute<DocumentTypePropertyAttribute>(propInfo);
                    if (propAttr == null)
                    {
                        continue; // skip this property - not part of a Document Type
                    }

                    string propertyName;
                    string propertyAlias;
                    DocumentTypeManager.ReadPropertyNameAndAlias(propInfo, propAttr, out propertyName, out propertyAlias);

                    if (propAttr.DefaultValue != null)
                    {
                        try
                        {
                            document.getProperty(propertyAlias).Value = propAttr.DefaultValue;
                        }
                        catch (Exception exc)
                        {
                            throw new Exception(string.Format("Cannot set default value ('{0}') for property {1}.{2}. Error: {3}",
                                propAttr.DefaultValue, typeDocType.Name, propInfo.Name, exc.Message), exc);
                        }
                    }
                }
            }
        }
        #endregion

        #region [Static methods]
        public static string GetDocumentTypeAlias(Type typeDocType)
        {
            string alias;

            DocumentTypeAttribute docTypeAttr = GetDocumentTypeAttribute(typeDocType);

            if (!String.IsNullOrEmpty(docTypeAttr.Alias))
            {
                alias = docTypeAttr.Alias;
            }
            else
            {
                alias = typeDocType.Name;
            }
            /*
            if (alias == "File" || alias == "Folder" || alias == "Image") // These are reserved Document type names
            {
                alias += "_";
            }
            */
            return alias;
        }

        public static void ReadPropertyNameAndAlias(PropertyInfo propInfo, DocumentTypePropertyAttribute propAttr,
            out string name, out string alias)
        {
            
               
            // set name
            name = string.IsNullOrEmpty(propAttr.Name) ? propInfo.Name : propAttr.Name;

            // set a default alias
            alias = propInfo.Name.Substring(0, 1).ToLower();

            // if an alias has been set, use that one explicitly
            if (!String.IsNullOrEmpty(propAttr.Alias)) 
            {
	            alias = propAttr.Alias;

            // otherwise
            } else {

	            // create the alias from the name 
	            if (propInfo.Name.Length > 1) {
	                alias += propInfo.Name.Substring(1, propInfo.Name.Length - 1);
	            }

	            // This is required because it seems that Umbraco has a bug when property type alias is called pageName.
	            if (alias == "pageName") {
	                alias += "_";
	            }

            }

        }

        public static DocumentType GetDocumentType(Type typeDocType)
        {
            return DocumentType.GetByAlias(DocumentTypeManager.GetDocumentTypeAlias(typeDocType));
        }


        public static Type GetDocumentTypeType(string documentTypeAlias)
        {
            if (!HasSynchronizedDocumentTypes())
            {
                FillDocumentTypes(typeof(DocumentTypeBase));
            }

            Type retVal = null;

            if (_documentTypes.ContainsKey(documentTypeAlias))
            {
                retVal = _documentTypes[documentTypeAlias];
            }

            return retVal;
        }
        #endregion

        #region [Document types creation]

        private static void FillDocumentTypes(Type baseTypeDocType)
        {
            foreach (Type typeDocType in Util.GetFirstLevelSubTypes(baseTypeDocType))
            {
                string docTypeAlias = GetDocumentTypeAlias(typeDocType);
                _documentTypes.Add(docTypeAlias, typeDocType);

                // create all children document types
                FillDocumentTypes(typeDocType);
            }
        }


        private void SynchronizeDocumentTypes(Type baseTypeDocType)
        {
            foreach (Type typeDocType in Util.GetFirstLevelSubTypes(baseTypeDocType))
            {
                this.SynchronizeDocumentType(typeDocType, baseTypeDocType);

                // create all children document types
                this.SynchronizeDocumentTypes(typeDocType);
            }
        }

        private void SynchronizeDocumentType(Type typeDocType, Type baseTypeDocType)
        {
            DocumentTypeAttribute docTypeAttr = DocumentTypeManager.GetDocumentTypeAttribute(typeDocType);

            string docTypeName = string.IsNullOrEmpty(docTypeAttr.Name) ? typeDocType.Name : docTypeAttr.Name;
            string docTypeAlias = DocumentTypeManager.GetDocumentTypeAlias(typeDocType);

            try
            {
                AddToSynchronized(typeDocType.Name, docTypeAlias, typeDocType);
            }
            catch (ArgumentException exc)
            {
                throw new Exception(string.Format("Document type with alias '{0}' already exists! Please use unique class names as class name is used as alias. Document type causing the problem: '{1}' (assembly: '{2}'). Error message: {3}",
                    docTypeAlias, typeDocType.FullName, typeDocType.Assembly.FullName, exc.Message));
            }

            _documentTypes.Add(docTypeAlias, typeDocType);

            DocumentType docType = DocumentType.GetByAlias(docTypeAlias);
            if (docType == null)
            {
                // if name is not set, use name of the type
                docType = DocumentType.MakeNew(this.siteBuilderUser, docTypeName);
            }

            docType.Text = docTypeName;
            docType.Alias = docTypeAlias;
            docType.IconUrl = docTypeAttr.IconUrl;
            docType.Thumbnail = docTypeAttr.Thumbnail;
            docType.Description = docTypeAttr.Description;

            if (baseTypeDocType == typeof(DocumentTypeBase))
            {
                docType.MasterContentType = 0;
            }
            else
            {
                docType.MasterContentType = DocumentType.GetByAlias(DocumentTypeManager.GetDocumentTypeAlias(baseTypeDocType)).Id;
            }

            this.SetAllowedTemplates(docType, docTypeAttr, typeDocType);

            this.SynchronizeDocumentTypeProperties(typeDocType, docType);

            docType.Save();
        }

        private void SetAllowedTemplates(DocumentType docType, DocumentTypeAttribute docTypeAttr, Type typeDocType)
        {
            List<Template> allowedTemplates = GetAllowedTemplates(docTypeAttr, typeDocType);

            try
            {
                docType.allowedTemplates = allowedTemplates.ToArray();
            }
            catch (SqlHelperException e)
            {
                throw new Exception(string.Format("Sql error setting templates for doc type '{0}' with templates '{1}'",
                    GetDocumentTypeAlias(typeDocType), string.Join(", ", allowedTemplates)));
            }

            int defaultTemplateId = GetDefaultTemplate(docTypeAttr, typeDocType, allowedTemplates);

            if (defaultTemplateId != 0)
            {
                docType.DefaultTemplate = defaultTemplateId;
            }
            else if (docType.allowedTemplates.Length == 1) // if only one template is defined for this doc type -> make it default template for this doc type
            {
                docType.DefaultTemplate = docType.allowedTemplates[0].Id;
            }
        }

        internal static int GetDefaultTemplate( DocumentTypeAttribute docTypeAttr, Type typeDocType, List<Template> allowedTemplates)
        {
            int defaultTemplateId = 0;

            if (!String.IsNullOrEmpty(docTypeAttr.DefaultTemplateAsString))
            {
                Template defaultTemplate = allowedTemplates.FirstOrDefault(t => t.Alias == docTypeAttr.DefaultTemplateAsString);
                if (defaultTemplate == null)
                {
                    throw new Exception(string.Format("Document type '{0}' has a default template '{1}' but that template does not use this document type",
                        GetDocumentTypeAlias(typeDocType), docTypeAttr.DefaultTemplateAsString));
                }

                defaultTemplateId =  defaultTemplate.Id;
            }

            return defaultTemplateId;
        }

        internal static List<Template> GetAllowedTemplates(DocumentTypeAttribute docTypeAttr, Type typeDocType)
        {
            List<Template> allowedTemplates = new List<Template>();

            // Use AllowedTemplates if given
            if (docTypeAttr.AllowedTemplates != null)
            {
                foreach (string templateName in docTypeAttr.AllowedTemplates)
                {
                    Template template = Template.GetByAlias(templateName);
                    if (template != null)
                    {
                        allowedTemplates.Add(template);
                    }
                    else
                    {
                        throw new Exception(string.Format("Template '{0}' does not exists. That template is set as allowed template for document type '{1}'",
                            templateName, DocumentTypeManager.GetDocumentTypeAlias(typeDocType)));
                    }
                }
            }
            else
            {
                // if AllowedTemplates if null, use all generic templates
                foreach (Type typeTemplate in TemplateManager.GetAllTemplates(typeDocType))
                {
                    Template template = Template.GetByAlias(typeTemplate.Name);

                    if (template != null)
                    {
                        allowedTemplates.Add(template);
                    }
                }
            }

            return allowedTemplates;
        }

        /// <summary>
        /// Get's the document type attribute or returns attribute with default values if attribute is not found
        /// </summary>
        /// <param name="typeDocType">An document type type</param>
        /// <returns></returns>
        internal static DocumentTypeAttribute GetDocumentTypeAttribute(Type typeDocType)
        {
            DocumentTypeAttribute retVal = Util.GetAttribute<DocumentTypeAttribute>(typeDocType);

            if (retVal == null)
            {
                retVal = DocumentTypeManager.CreateDefaultDocumentTypeAttribute(typeDocType);
            }

            return retVal;
        }

        private static DocumentTypeAttribute CreateDefaultDocumentTypeAttribute(Type typeDocType)
        {
            DocumentTypeAttribute retVal = new DocumentTypeAttribute();

            retVal.Name = typeDocType.Name;
            retVal.IconUrl = DocumentTypeDefaultValues.IconUrl;
            retVal.Thumbnail = DocumentTypeDefaultValues.Thumbnail;

            return retVal;
        }
        #endregion

        #region [Document type properties synchronization]
        private void SynchronizeDocumentTypeProperties(Type typeDocType, DocumentType docType)
        {
            this.SynchronizeContentTypeProperties(typeDocType, docType, out this._hadDefaultValues);
        }
        #endregion

        #region [Allowed child node types synchronization]
        private void SynchronizeAllowedChildContentTypes(Type baseTypeDocType)
        {
            foreach (Type type in Util.GetFirstLevelSubTypes(baseTypeDocType))
            {
                this.SynchronizeAllowedChildContentType(type);

                // process all children document types
                this.SynchronizeAllowedChildContentTypes(type);
            }
        }

        private void SynchronizeAllowedChildContentType(Type typeDocType)
        {
            DocumentTypeAttribute docTypeAttr = Util.GetAttribute<DocumentTypeAttribute>(typeDocType);
            if (docTypeAttr != null)
            {
                DocumentType docType = DocumentType.GetByAlias(DocumentTypeManager.GetDocumentTypeAlias(typeDocType));

                List<int> allowedTypeIds = new List<int>();

                if (docTypeAttr.AllowedChildNodeTypes != null)
                {
                    foreach (Type allowedType in docTypeAttr.AllowedChildNodeTypes)
                    {
                        int id = DocumentType.GetByAlias(DocumentTypeManager.GetDocumentTypeAlias(allowedType)).Id;
                        if (!allowedTypeIds.Contains(id))
                        {
                            allowedTypeIds.Add(id);
                        }
                    }
                }

                docType.AllowedChildContentTypeIDs = allowedTypeIds.ToArray();

                docType.Save();
            }
        }
        #endregion

        #region ["Allowed child node type of" synchronization]
        private void SynchronizeReverseAllowedChildContentTypes(Type baseTypeDocType)
        {
            // process a reverse-lookup if there's any document types to be sync'ed
            if (DocumentTypeManager.HasSynchronizedDocumentTypes())
            {
                foreach (Type docType in DocumentTypeManager._documentTypes.Values)
                {
                    // retrieves the document type attribute, containing all the info
                    // required for synchronisation
                    var docTypeAttr = 
                        DocumentTypeManager.GetDocumentTypeAttribute(docType);

                    var docTypeNode = DocumentTypeManager.GetDocumentType(docType);

                    if (docTypeNode != null
                        && docTypeAttr.AllowedChildNodeTypeOf != null
                        && docTypeAttr.AllowedChildNodeTypeOf.Length > 0)
                    {
                        // enumerates through each one of the parent node type
                        foreach (Type parent in docTypeAttr.AllowedChildNodeTypeOf)
                        {
                            var parentDocType = DocumentTypeManager.GetDocumentType(parent);

                            if (parentDocType != null)
                            {
                                List<int> allowedChildNodeTypes = 
                                    new List<int>(parentDocType.AllowedChildContentTypeIDs);

                                // add to the list of allowed child node types of
                                // the parent node if it isn't already there.
                                if (!allowedChildNodeTypes.Contains(docTypeNode.Id))
                                {
                                    allowedChildNodeTypes.Add(docTypeNode.Id);
                                    parentDocType.AllowedChildContentTypeIDs = 
                                        allowedChildNodeTypes.ToArray();

                                    parentDocType.Save();
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        public List<string> CleanDocumentTypes(Type uSiteBuilderType)
        {
            List<string> aliases = new List<string>();
            List<Type> firstLevelSubTypes = Util.GetFirstLevelSubTypes(uSiteBuilderType);

            foreach (Type typeDocType in firstLevelSubTypes)
            {
                string alias = GetDocumentTypeAlias(typeDocType);

                IEnumerable<PropertyInfo> alluSiteBuilderProperties = typeDocType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic |
                                   BindingFlags.Instance).Where(prop => Util.GetAttribute<DocumentTypePropertyAttribute>(prop) != null);

                List<string> propertyAliases = alluSiteBuilderProperties.Select(prop =>
                    {
                        string propertyName;
                        string propertyAlias;
                        ReadPropertyNameAndAlias(prop, Util.GetAttribute<DocumentTypePropertyAttribute>(prop), out propertyName, out propertyAlias);
                        return propertyAlias;
                    }).ToList();

                // add mixin properties
                var documentTypeAttribute = GetDocumentTypeAttribute(typeDocType);

                if (documentTypeAttribute.Mixins != null)
                {
                    foreach (Type mixinType in documentTypeAttribute.Mixins)
                    {
                        foreach (var mixinProperty in mixinType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic |
                                   BindingFlags.Instance).Where(prop => Util.GetAttribute<DocumentTypePropertyAttribute>(prop) != null))
                        {
                            string propertyName;
                            string propertyAlias;
                            ReadPropertyNameAndAlias(mixinProperty, Util.GetAttribute<DocumentTypePropertyAttribute>(mixinProperty), out propertyName, out propertyAlias);
                            if (!propertyAliases.Contains(propertyAlias))
                            {
                                propertyAliases.Add(propertyAlias);
                            }
                        }
                    }
                }
                DocumentType docType = DocumentType.GetByAlias(alias);
                if (docType != null)
                {
                    foreach(var property in docType.PropertyTypes.Where(prop => prop.ContentTypeId == docType.Id))
                        
                    if (!propertyAliases.Any(prop => prop ==  property.Alias))
                    {
                        property.delete();
                    }
                }

                aliases.Add(alias);
                aliases.AddRange(CleanDocumentTypes(typeDocType));
            }

            return aliases;
        }

        public void CleanUpDocumentTypes()
        {
            var aliases = CleanDocumentTypes(typeof(DocumentTypeBase));

            // delete any umbraco defined doc types that don't exist in the class definitions
            var docTypesToDelete = DocumentType.GetAllAsList().Where(doctype => aliases.All(alias => alias != doctype.Alias));

            foreach (var docTypeToDelete in docTypesToDelete)
            {
                DeleteDocumentType(docTypeToDelete.Alias);
            }
        }
    }
}
