using umbraco.DataLayer;
using Umbraco.Core.Events;

namespace Vega.USiteBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;

    using Vega.USiteBuilder.DocumentTypeBuilder;

    /// <summary>
    /// Manages document types synchronization
    /// </summary>
    internal class DocumentTypeManager : ManagerBase
    {
        // Holds all document types found in 
        // Type = Document type type (subclass of DocumentTypeBase), string = document type alias
        private static readonly Dictionary<string, Type> DocumentTypes = new Dictionary<string, Type>();        
        private static readonly Dictionary<string, int> DocumentTypesId = new Dictionary<string, int>();
        private static List<ContentComparison> DocumentTypesComparisonSummary = new List<ContentComparison>();
        static readonly IContentTypeService ContentTypeService = ApplicationContext.Current.Services.ContentTypeService;
        static readonly IFileService FileService = ApplicationContext.Current.Services.FileService;

        // indicates if any of synced document types had a default value
        private bool _hadDefaultValues = false;

        /// <summary>
        /// Returns true if there's any document type synchronized (defined)
        /// </summary>
        /// <returns></returns>
        public static bool HasSynchronizedDocumentTypes()
        {
            return DocumentTypes.Count > 0;
        }

        /// <summary>
        /// Main mathod for synchronizing document types
        /// </summary>
        public void Synchronize()
        {
            DocumentTypes.Clear();
            DocumentTypesId.Clear();
            DocumentTypesComparisonSummary.Clear();


#if DEBUG
            Stopwatch timer = new Stopwatch();
            timer.Start();
#endif
            // Check which document types have been changed
            // Does not check relations between document types (allowed childs and allowable parents) since it engages similar resources as 
            // method for updating relations
            DocumentTypesComparisonSummary = DocumentTypeComparer.PreviewDocumentTypeChanges(out _hadDefaultValues);

            // store node id for existing document types in dictionary (to avoid unnecessary API calls)
            foreach (var dtItem in DocumentTypesComparisonSummary.Where(dt => dt.DocumentTypeStatus != Status.New))
            {
                if (DocumentTypesId.ContainsKey(dtItem.Alias) == false)
                {
                    DocumentTypesId.Add(dtItem.Alias, dtItem.DocumentTypeId);
                }
            }

#if DEBUG
            timer.Stop();
            StopwatchLogger.AddToLog(string.Format("Total elapsed time for method 'DocumentTypeManager.Synchronize' - only preview of type changes: {0}ms.", timer.ElapsedMilliseconds));
            timer.Restart();
#endif

            this.SynchronizeDocumentTypes(typeof(DocumentTypeBase));

#if DEBUG
            timer.Stop();
            StopwatchLogger.AddToLog(string.Format("Total elapsed time for method 'DocumentTypeManager.SynchronizeDocumentTypes': {0}ms.", timer.ElapsedMilliseconds));
            timer.Restart();
#endif

            this.SynchronizeChildNodes(typeof(DocumentTypeBase));

#if DEBUG
            timer.Stop();
            StopwatchLogger.AddToLog(string.Format("Total elapsed time for method 'DocumentTypeManager.SynchronizeChildNodes': {0}ms.", timer.ElapsedMilliseconds));
            timer.Restart();
#endif

            if (this._hadDefaultValues) // if there were default values set subscribe to Creating event in which we'll set default values.
            {
                // Subscribe to Creating event
                ContentService.Creating += ContentServiceOnCreating;
            }
        }

        private void ContentServiceOnCreating(IContentService sender, NewEventArgs<IContent> newEventArgs)
        {
            Type typeDocType = DocumentTypeManager.GetDocumentTypeType(newEventArgs.Alias);
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
                    ReadPropertyNameAndAlias(propInfo, propAttr, out propertyName, out propertyAlias);

                    if (propAttr.DefaultValue != null)
                    {
                        try
                        {
                            newEventArgs.Entity.SetValue(propertyAlias, propAttr.DefaultValue);
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

        public static string GetDocumentTypeAlias(Type typeDocType)
        {
            string alias;
            bool aliasUsed = false;

            DocumentTypeAttribute docTypeAttr = GetDocumentTypeAttribute(typeDocType);

            if (!String.IsNullOrEmpty(docTypeAttr.Alias))
            {
                alias = docTypeAttr.Alias;
                aliasUsed = true;
            }
            else
            {
                alias = typeDocType.Name;
            }

            if (alias.ToLower() != alias.ToSafeAlias().ToLower())
                throw new ArgumentException(string.Format("The {0} '{1}', for the document type '{2}', is invalid.", (aliasUsed ? "alias" : "name"), alias, typeDocType.Name), "Alias");

            return alias;
        }

        public static int GetDocumentTypeId(Type typeDocType)
        {
            int id;

            string alias = GetDocumentTypeAlias(typeDocType);

            if (!DocumentTypesId.TryGetValue(alias, out id))
            {
                id = -1;
            }

            return id;
        }

        public static int GetDocumentTypeId(string alias)
        {
            int id;

            if (!DocumentTypesId.TryGetValue(alias, out id))
            {
                id = -1;
            }
            return id;
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
            }
            else
            {

                // create the alias from the name 
                if (propInfo.Name.Length > 1)
                {
                    alias += propInfo.Name.Substring(1, propInfo.Name.Length - 1);
                }

                // This is required because it seems that Umbraco has a bug when property type alias is called pageName.
                if (alias == "pageName")
                {
                    alias += "_";
                }
            }
        }

        public static IContentType GetDocumentType(Type typeDocType)
        {
            return ContentTypeService.GetContentType(GetDocumentTypeAlias(typeDocType));
        }

        public static Type GetDocumentTypeType(string documentTypeAlias)
        {
            if (!HasSynchronizedDocumentTypes())
            {
                FillDocumentTypes(typeof(DocumentTypeBase));
            }

            Type retVal = null;

            if (DocumentTypes.ContainsKey(documentTypeAlias))
            {
                retVal = DocumentTypes[documentTypeAlias];
            }

            return retVal;
        }

        private static void FillDocumentTypes(Type baseTypeDocType)
        {
            foreach (Type typeDocType in Util.GetFirstLevelSubTypes(baseTypeDocType))
            {
                string docTypeAlias = GetDocumentTypeAlias(typeDocType);

                // Prevent "An item with the same key has already been added."
                if (DocumentTypes.ContainsKey(docTypeAlias))
                {
                    // Reassign the value.
                    DocumentTypes[docTypeAlias] = typeDocType;
                }
                else
                {
                    DocumentTypes.Add(docTypeAlias, typeDocType);
                }

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
            // Get DocumentTypeAttribute attribute for typeDocType
            DocumentTypeAttribute docTypeAttr = GetDocumentTypeAttribute(typeDocType);
            string docTypeName = string.IsNullOrEmpty(docTypeAttr.Name) ? typeDocType.Name : docTypeAttr.Name;

            string docTypeAlias = string.Empty;
            if (!String.IsNullOrEmpty(docTypeAttr.Alias))
            {
                docTypeAlias = docTypeAttr.Alias;
            }
            else
            {
                docTypeAlias = typeDocType.Name;
            }

            DocumentTypes.Add(docTypeAlias, typeDocType);

            // If document type is not changed, skip update
            if (DocumentTypesComparisonSummary.Exists(dt => (dt.DocumentTypeStatus == Status.Same) && (dt.Alias == docTypeAlias)))
            {
                return;
            }

            try
            {
                AddToSynchronized(typeDocType.Name, docTypeAlias, typeDocType);
            }
            catch (ArgumentException exc)
            {
                throw new Exception(string.Format("Document type with alias '{0}' already exists! Please use unique class names as class name is used as alias. Document type causing the problem: '{1}' (assembly: '{2}'). Error message: {3}",
                    docTypeAlias, typeDocType.FullName, typeDocType.Assembly.FullName, exc.Message));
            }

            // If parent is some other DT, retrieve parentID; otherwise it's -1              
            int parentId = -1;
            if (baseTypeDocType != typeof(DocumentTypeBase))
            {
                parentId = GetDocumentTypeId(baseTypeDocType);
            }

            // Get DT with same alias from Umbraco if exists
            IContentType contentType = ContentTypeService.GetContentType(docTypeAlias);

            if (contentType == null)
            {
                // New DT
                if (parentId != -1)
                {
                    IContentType parentType = ContentTypeService.GetContentType(parentId);
                    contentType = new ContentType(parentType);
                }
                else
                {
                    contentType = new ContentType(parentId);
                }
            }
            else
            {
                // Existing DT
                if (contentType.ParentId != parentId)
                {
                    throw new Exception(string.Format("Document type inheritance for document type with '{0}' alias, cannot be updated.", docTypeAlias));
                }
            }

            contentType.Name = docTypeName;
            contentType.Alias = docTypeAlias;
            contentType.Icon = docTypeAttr.IconUrl;
            contentType.Thumbnail = docTypeAttr.Thumbnail;
            contentType.Description = docTypeAttr.Description;
            contentType.AllowedAsRoot = docTypeAttr.AllowAtRoot;

            SetAllowedTemplates(contentType, docTypeAttr, typeDocType);

            SynchronizeDocumentTypeProperties(typeDocType, contentType, docTypeAttr);

            ContentTypeService.Save(contentType);

            // store node id for new document types in dictionary (to avoid unnecessary API calls)
            if (DocumentTypesId.ContainsKey(contentType.Alias) == false)
            {
                DocumentTypesId.Add(contentType.Alias, contentType.Id);
            }
        }

        private void SetAllowedTemplates(IContentType contentType, DocumentTypeAttribute docTypeAttr, Type typeDocType)
        {
            List<ITemplate> allowedTemplates = GetAllowedTemplates(docTypeAttr, typeDocType);
            try
            {
                if ((allowedTemplates.Count == 0) && (contentType.AllowedTemplates.Count() != 0))
                {    
                    // Clear all allowed templates and default template
                    foreach (var templateItem in contentType.AllowedTemplates)
                    {
                        contentType.RemoveTemplate(templateItem);
                    }                    
                }
                else 
                {
                    contentType.AllowedTemplates = allowedTemplates.ToArray();
                }
            }
            catch (SqlHelperException e)
            {
                throw new Exception(string.Format("Sql error setting templates for doc type '{0}' with templates '{1}'",
                    GetDocumentTypeAlias(typeDocType), string.Join(", ", allowedTemplates)));
            }

            ITemplate defaultTemplate = GetDefaultTemplate(docTypeAttr, typeDocType, allowedTemplates);

            if (defaultTemplate != null)
            {
                contentType.SetDefaultTemplate(defaultTemplate);
            }
            else if (contentType.AllowedTemplates.Count() == 1) // if only one template is defined for this doc type -> make it default template for this doc type
            {
                contentType.SetDefaultTemplate(contentType.AllowedTemplates.First());
            }
        }

        internal static ITemplate GetDefaultTemplate(DocumentTypeAttribute docTypeAttr, Type typeDocType, List<ITemplate> allowedTemplates)
        {
            if (!String.IsNullOrEmpty(docTypeAttr.DefaultTemplateAsString))
            {
                ITemplate defaultTemplate = allowedTemplates.FirstOrDefault(t => t.Alias == docTypeAttr.DefaultTemplateAsString);
                if (defaultTemplate == null)
                {
                    throw new Exception(string.Format("Document type '{0}' has a default template '{1}' but that template does not use this document type",
                        GetDocumentTypeAlias(typeDocType), docTypeAttr.DefaultTemplateAsString));
                }

                return defaultTemplate;
            }

            return null;
        }

        internal static List<ITemplate> GetAllowedTemplates(DocumentTypeAttribute docTypeAttr, Type typeDocType)
        {
            var allowedTemplates = new List<ITemplate>();

            // Use AllowedTemplates if given
            if (docTypeAttr.AllowedTemplates != null)
            {
                foreach (string templateName in docTypeAttr.AllowedTemplates)
                {
                    ITemplate template = FileService.GetTemplate(templateName);
                    if (template != null)
                    {
                        allowedTemplates.Add(template);
                    }
                    else
                    {
                        throw new Exception(string.Format("Template '{0}' does not exists. That template is set as allowed template for document type '{1}'",
                            templateName, GetDocumentTypeAlias(typeDocType)));
                    }
                }
            }
            else
            {
                if (Util.DefaultRenderingEngine == Umbraco.Core.RenderingEngine.WebForms)
                {
                    // if AllowedTemplates is null, use all generic templates
                    foreach (Type typeTemplate in TemplateManager.GetAllTemplates(typeDocType))
                    {
                        ITemplate template = FileService.GetTemplate(typeTemplate.Name);

                        if (template != null)
                        {
                            allowedTemplates.Add(template);
                        }
                    }
                }
                else if (Util.DefaultRenderingEngine == Umbraco.Core.RenderingEngine.Mvc)
                {
                    // if AllowedTemplates is null, use all generic templates
                    foreach (ITemplate template in FileService.GetTemplates())
                    {
                        if (IsViewForDocumentType(typeDocType.Name, template.Content))
                        {
                            allowedTemplates.Add(template);
                        }
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
            var retVal = Util.GetAttribute<DocumentTypeAttribute>(typeDocType);

            if (retVal == null)
            {
                retVal = CreateDefaultDocumentTypeAttribute(typeDocType);
            }

            return retVal;
        }

        private static DocumentTypeAttribute CreateDefaultDocumentTypeAttribute(Type typeDocType)
        {
            var retVal = new DocumentTypeAttribute();

            retVal.Name = typeDocType.Name;
            retVal.IconUrl = DocumentTypeDefaultValues.IconUrl;
            retVal.Thumbnail = DocumentTypeDefaultValues.Thumbnail;

            return retVal;
        }


        private void SynchronizeDocumentTypeProperties(Type typeDocType, IContentType docType, DocumentTypeAttribute documentTypeAttribute)
        {
            SynchronizeContentTypeProperties(typeDocType, docType, documentTypeAttribute, out _hadDefaultValues);
        }

        private void SynchronizeChildNodes(Type baseTypeDocType)
        {

            // Allowed child nodetypes can be defined in two ways:
            //      1. Using "AllowedChildNodeTypes" property which defines allowable children
            //      2. Using "AllowedChildNodeTypeOf" property which defines allowable parents
            //
            //      Final list of allowable children is union between these two properties            
            
            Dictionary<int, List<ContentTypeSort>> parentsToUpdate = new Dictionary<int, List<ContentTypeSort>>();

            // Retrieve list of allowable parents for every node
            foreach (Type type in Util.GetAllSubTypes(baseTypeDocType))
            {
                GetAllowableParents(type, parentsToUpdate);                
            }

            // Create list of allowable children using "AllowedChildNodeTypes" property and add additional children retrieved in previous method
            SynchronizeAllowedChildContentTypes(baseTypeDocType, parentsToUpdate);
        }

        private void SynchronizeAllowedChildContentTypes(Type baseTypeDocType, Dictionary<int, List<ContentTypeSort>> parents)
        {
            foreach (Type type in Util.GetFirstLevelSubTypes(baseTypeDocType))
            {
                this.SynchronizeAllowedChildContentType(type, parents);

                // process all children document types
                this.SynchronizeAllowedChildContentTypes(type, parents);
            }
        }

        /// <summary>
        /// Create list of parents that has allowable children defined via "AllowedChildNodeTypesOf" property
        /// </summary>
        /// <param name="typeDocType"></param>
        /// <param name="parents"></param>
        private void GetAllowableParents(Type typeDocType, Dictionary<int, List<ContentTypeSort>> parents)
        {
            DocumentTypeAttribute docTypeAttr = Util.GetAttribute<DocumentTypeAttribute>(typeDocType);
            if (docTypeAttr != null)
            {
                if (docTypeAttr.AllowedChildNodeTypeOf != null && docTypeAttr.AllowedChildNodeTypeOf.Length > 0)
                {
                    List<ContentTypeSort> tmpChilds = null;

                    string childDocumentTypeAlias = GetDocumentTypeAlias(typeDocType);
                    int childDocumentTypeId = GetDocumentTypeId(childDocumentTypeAlias);

                    if (childDocumentTypeId != -1)
                    {
                        // enumerates through each one of the parent node type
                        foreach (Type parent in docTypeAttr.AllowedChildNodeTypeOf)
                        {
                            string parentDocumentTypeAlias = GetDocumentTypeAlias(parent);
                            int parentDocumentTypeId = GetDocumentTypeId(parentDocumentTypeAlias);

                            if (parents.TryGetValue(parentDocumentTypeId, out tmpChilds))
                            {
                                tmpChilds.Add(new ContentTypeSort { Id = new Lazy<int>(() => childDocumentTypeId), Alias = childDocumentTypeAlias });
                                parents[parentDocumentTypeId] = tmpChilds;
                            }
                            else
                            {
                                tmpChilds = new List<ContentTypeSort>();
                                tmpChilds.Add(new ContentTypeSort { Id = new Lazy<int>(() => childDocumentTypeId), Alias = childDocumentTypeAlias });
                                parents.Add(parentDocumentTypeId, tmpChilds);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates list of allowable children
        /// </summary>
        /// <param name="typeDocType"></param>
        /// <param name="parents"></param>
        private void SynchronizeAllowedChildContentType(Type typeDocType, Dictionary<int, List<ContentTypeSort>> parents)
        {
            int tmpDocTypeId = GetDocumentTypeId(typeDocType);
            List<ContentTypeSort> additionalAllowableTypes;
            parents.TryGetValue(tmpDocTypeId, out additionalAllowableTypes);

            DocumentTypeAttribute docTypeAttr = Util.GetAttribute<DocumentTypeAttribute>(typeDocType);
            if (docTypeAttr != null)
            {
                IContentType contentType = ContentTypeService.GetContentType(GetDocumentTypeAlias(typeDocType));
                List<ContentTypeSort> allowedTypeIds = new List<ContentTypeSort>();

                // If "AllowedChildNodeTypes" property has allowable children
                if (docTypeAttr.AllowedChildNodeTypes != null && docTypeAttr.AllowedChildNodeTypes.Length > 0)
                {
                    foreach (Type allowedType in docTypeAttr.AllowedChildNodeTypes)
                    {
                        string allowedContentTypeAlias = GetDocumentTypeAlias(allowedType);

                        int allowedContentTypeId = GetDocumentTypeId(allowedContentTypeAlias);

                        // Adds children defined in "AllowedChildNodeTypes" property
                        if (!allowedTypeIds.Any(s => s.Id.Value == allowedContentTypeId))
                        {
                            allowedTypeIds.Add(new ContentTypeSort { Alias = allowedContentTypeAlias, Id = new Lazy<int>(() => allowedContentTypeId) });
                        }

                        // Add children defined via "AllowedChildNodeTypesOf" property (Retrieved from method "GetAllowableParents")
                        if (additionalAllowableTypes != null)
                        {
                            foreach (ContentTypeSort item in additionalAllowableTypes)
                            {
                                int childId = item.Id.Value;
                                if (!allowedTypeIds.Any(s => s.Id.Value == childId))
                                {
                                    allowedTypeIds.Add(new ContentTypeSort { Alias = item.Alias, Id = new Lazy<int>(() => childId) });
                                }
                            } 
                        }

                    }

                    contentType.AllowedContentTypes = allowedTypeIds.ToArray();
                    ContentTypeService.Save(contentType);
                }
                
                // IF children are defined only via "AllowedChildNodeTypesOf" property (Retrieved from method "GetAllowableParents")
                if (additionalAllowableTypes != null && additionalAllowableTypes.Count > 0)
                {
                    foreach (ContentTypeSort item in additionalAllowableTypes)
                    {
                        int childId = item.Id.Value;
                        if (!allowedTypeIds.Any(s => s.Id.Value == childId))
                        {
                            allowedTypeIds.Add(new ContentTypeSort { Alias = item.Alias, Id = new Lazy<int>(() => childId) });
                        }
                    }
                }

                // Update only in case there are allowed type ID's or if the allowed type ID's are cleared for the document type.
                if (allowedTypeIds.Count > 0 || (contentType.AllowedContentTypes.Count() > 0 && allowedTypeIds.Count == 0))
                {
                    contentType.AllowedContentTypes = allowedTypeIds.ToArray();
                    ContentTypeService.Save(contentType);
                }
            }
        }

        private static bool IsViewForDocumentType(string typeName, string templateCode)
        {
            bool retVal = false;
            var match = Regex.Match(templateCode, @"UmbracoTemplatePageBase\s*\<\s*(.*)\s*\>");
            if (match.Success)
            {
                string templateTypeName = match.Groups[1].Value;
                string[] nameSplitArray = templateTypeName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (nameSplitArray.Length > 0)
                {
                    templateTypeName = nameSplitArray.Last();
                    if (!string.IsNullOrEmpty(templateTypeName) && templateTypeName == typeName)
                    {
                        retVal = true;
                    }
                }
            }

            return retVal;
        }

        public static void DeleteDocumentType(string alias)
        {
            var contentType = ContentTypeService.GetContentType(alias);
            ContentTypeService.Delete(contentType);
        }

        public static void CleanUpDocumentTypes()
        {
            List<string> existingDocumentTypesAliases = new List<string>();
            List<Type> existingDocumentTypes = Util.GetAllSubTypes(typeof(DocumentTypeBase));

            foreach (var item in existingDocumentTypes)
            {
                existingDocumentTypesAliases.Add(GetDocumentTypeAlias(item));                
            }

            // get all umbraco defined doc types that don't exist in the class definitions
            var docTypesToDelete = ContentTypeService.GetAllContentTypes().Where(contentType => existingDocumentTypesAliases.All(alias => alias != contentType.Alias));

            foreach (var docTypeToDelete in docTypesToDelete.OrderByDescending(dt => dt.Level))
            {
                DeleteDocumentType(docTypeToDelete.Alias);
            }
        }
    }
}
