using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Vega.USiteBuilder.DocumentTypeBuilder
{
    internal class DocumentTypeComparer
    {
        static readonly IContentTypeService ContentTypeService = ApplicationContext.Current.Services.ContentTypeService;
        static bool _hasDefaultValues = false;

        /// <summary>
        /// Checks if there were any changes in document types defined in Umbraco/uSiteBuilder/Both.
        /// Does not check relations between document types (allowed childs and allowable parents)
        /// </summary>
        /// <param name="hasDefaultValues"></param>
        /// <returns></returns>
        public static List<ContentComparison> PreviewDocumentTypeChanges(out bool hasDefaultValues)
        {
#if DEBUG
            Stopwatch timer = new Stopwatch();
            timer.Start();
#endif
            // compare the library based definitions to the Umbraco DB
            var definedDocTypes = PreviewDocTypes(typeof(DocumentTypeBase), "");

#if DEBUG
            timer.Stop();
            StopwatchLogger.AddToLog(string.Format("Total elapsed time for method 'DocumentTypeComparer.PreviewDocumentTypeChanges' - only PreviewDocTypes: {0}ms.", timer.ElapsedMilliseconds));
            timer.Restart();
#endif




#if DEBUG
            timer.Start();
#endif
            hasDefaultValues = _hasDefaultValues;
            // add any umbraco defined doc types that don't exist in the class definitions
            definedDocTypes.AddRange(ContentTypeService.GetAllContentTypes()
                                         .Where(doctype => definedDocTypes.All(dd => dd.Alias != doctype.Alias))
                                         .Select(docType => new ContentComparison { Alias = docType.Alias, DocumentTypeStatus = Status.Deleted, DocumentTypeId = docType.Id }));

#if DEBUG
            timer.Stop();
            StopwatchLogger.AddToLog(string.Format("Total elapsed time for method 'DocumentTypeComparer.PreviewDocumentTypeChanges' - add any umbraco defined doc types that don't exist in the class definitions: {0}ms.", timer.ElapsedMilliseconds));
            timer.Restart();
#endif

            return definedDocTypes;
        }

        private static bool IsUnchanged(Type typeDocType, Type parentDocType, IContentType contentType)
        {
            DocumentTypeAttribute docTypeAttr = DocumentTypeManager.GetDocumentTypeAttribute(typeDocType);

            if (docTypeAttr.Mixins != null)
            {
                // update this document type since it depends on others
                return false;
            }

            string docTypeName = String.IsNullOrEmpty(docTypeAttr.Name) ? typeDocType.Name : docTypeAttr.Name;
            string docTypeAlias = DocumentTypeManager.GetDocumentTypeAlias(typeDocType);


            if (contentType.Name != docTypeName)
            {
                return false;
            }

            if (contentType.Alias != docTypeAlias)
            {
                return false;
            }

            if (contentType.Icon != docTypeAttr.IconUrl)
            {
                return false;
            }

            if (contentType.Thumbnail != docTypeAttr.Thumbnail)
            {
                return false;
            }

            if (contentType.Description != docTypeAttr.Description)
            {
                return false;
            }

            if (contentType.ParentId != -1)
            {
                if (parentDocType == typeof(DocumentTypeBase))
                {
                    return false;
                }

                var existingParentDocumentType = ContentTypeService.GetContentType(contentType.ParentId);

                if (existingParentDocumentType.Alias != DocumentTypeManager.GetDocumentTypeAlias(parentDocType))
                {
                    return false;
                }
            }
            else
            {
                if (parentDocType != typeof(DocumentTypeBase))
                {
                    return false;
                }
            }

#if DEBUG
            Stopwatch timer = new Stopwatch();
            timer.Start();
#endif
            if (!CompareContentTypeProperties(typeDocType, contentType))
            {
                return false;
            }

#if DEBUG
            timer.Stop();
            //StopwatchLogger.AddToLog(string.Format("Total elapsed time for method 'DocumentTypeComparer.IsUnchanged' - CompareContentTypeProperties: {0}ms.", timer.ElapsedMilliseconds));
            timer.Restart();
#endif

            if (!CompareAllowedTemplates(contentType, docTypeAttr, typeDocType))
            {
                return false;
            }

            if (contentType.AllowedAsRoot != docTypeAttr.AllowAtRoot)
            {
                return false;
            }

            return true;
        }

        private static bool CompareAllowedTemplates(IContentType contentType, DocumentTypeAttribute docTypeAttr, Type typeDocType)
        {
            List<ITemplate> allowedTemplates = DocumentTypeManager.GetAllowedTemplates(docTypeAttr, typeDocType);

            IEnumerable<ITemplate> existingTemplates = contentType.AllowedTemplates;

            if (allowedTemplates.Count != existingTemplates.Count())
            {
                return false;
            }

            foreach (var template in allowedTemplates)
            {
                if (!existingTemplates.Any(t => t.Alias == template.Alias))
                {
                    return false;
                }
            }

            ITemplate defaultTemplate = DocumentTypeManager.GetDefaultTemplate(docTypeAttr, typeDocType, allowedTemplates);

            if (defaultTemplate != null)
            {
                return (contentType.DefaultTemplate.Id == defaultTemplate.Id);
            }

            if (allowedTemplates.Count == 1)
            {
                return (contentType.DefaultTemplate.Id == allowedTemplates.First().Id);
            }
             
            return true;
        }

        private static bool CompareContentTypeProperties(Type uSiteBuilderDocType, IContentType contentType)
        {
            // Retrieve tab names and corresponding properties
            Dictionary<string, string> tabsWithProperties = new Dictionary<string, string>();
            string tmpTabName;
            foreach (var tabItem in contentType.PropertyGroups)
            {
                tmpTabName = tabItem.Name;
                foreach (var propertyItem in tabItem.PropertyTypes)
                {
                    tabsWithProperties.Add(propertyItem.Alias, tmpTabName);
                }
            }            
            
            foreach (PropertyInfo propInfo in uSiteBuilderDocType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                DocumentTypePropertyAttribute propAttr = Util.GetAttribute<DocumentTypePropertyAttribute>(propInfo);
                if (propAttr == null)
                {
                    continue; // skip this property - not part of a document type
                }

                if (propAttr.DefaultValue != null)
                {
                    _hasDefaultValues = true;
                }

                // getting name and alias
                string propertyName;
                string propertyAlias;
                DocumentTypeManager.ReadPropertyNameAndAlias(propInfo, propAttr, out propertyName, out propertyAlias);

                if (Util.GetAttribute<ObsoleteAttribute>(propInfo) != null)
                {
                    // If it's marked as obsolete, but still exists, there's a change
                    if (contentType.PropertyTypes.Any(p => p.Alias == propertyAlias))
                    {
                        return false;
                    }

                    // marked as obsolete and already deleted, skip rest of checks
                    continue;
                }                

                var dataTypeDefinition = ManagerBase.GetDataTypeDefinition(uSiteBuilderDocType, propAttr, propInfo);
                
                Umbraco.Core.Models.PropertyType property = contentType.PropertyTypes.FirstOrDefault(p => p.Alias == propertyAlias);
                if (property == null) 
                {
                    // new property 
                    return false;
                }

                if (property.DataTypeDefinitionId != dataTypeDefinition.Id) 
                {
                    return false;
                }

                string tabName = string.Empty;
                tabsWithProperties.TryGetValue(property.Alias, out tabName);
                if (tabName == null)
                {
                    // For generics properties tab
                    tabName = string.Empty;
                }
                if (propAttr.TabAsString != tabName)
                {
                    return false;
                }

                if (property.Name != propertyName)
                {
                    return false;
                }

                if (property.Mandatory != propAttr.Mandatory)
                {
                    return false;
                }

                if (property.ValidationRegExp != propAttr.ValidationRegExp)
                {
                    return false;
                }

                if (property.Description != propAttr.Description)
                {
                    return false;
                }
            }

            return true;
        }

        private static List<ContentComparison> PreviewDocTypes(Type parentDocType, string parentAlias)
        {
            var comparison = new List<ContentComparison>();

            foreach (Type typeDocType in Util.GetFirstLevelSubTypes(parentDocType))
            {
                string alias = DocumentTypeManager.GetDocumentTypeAlias(typeDocType);

                IContentType contentType = ContentTypeService.GetContentType(alias);
                
                if (contentType == null)
                {
                    comparison.Add(new ContentComparison { Alias = alias, DocumentTypeStatus = Status.New, ParentAlias = parentAlias });
                }
                else
                {
#if DEBUG
                    Stopwatch timer = new Stopwatch();
                    timer.Start();
#endif

                    bool unchanged = IsUnchanged(typeDocType, parentDocType, contentType);

#if DEBUG
                    timer.Stop();
                    //StopwatchLogger.AddToLog(string.Format("Total elapsed time for method 'DocumentTypeComparer.PreviewDocTypes' - IsUnchanged: {0}ms.", timer.ElapsedMilliseconds));
                    timer.Restart();
#endif

                    if (unchanged)
                    {
                        comparison.Add(new ContentComparison { Alias = alias, DocumentTypeStatus = Status.Same, ParentAlias = parentAlias, DocumentTypeId = contentType.Id });
                    }
                    else
                    {
                        comparison.Add(new ContentComparison { Alias = alias, DocumentTypeStatus = Status.Changed, ParentAlias = parentAlias, DocumentTypeId = contentType.Id });
                    }
                }

                comparison.AddRange(PreviewDocTypes(typeDocType, alias));
            }

            return comparison;
        }
    }
}
