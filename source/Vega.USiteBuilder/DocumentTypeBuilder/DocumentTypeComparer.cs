using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.propertytype;
using umbraco.cms.businesslogic.template;
using umbraco.cms.businesslogic.web;

namespace Vega.USiteBuilder.DocumentTypeBuilder
{
    internal class DocumentTypeComparer
    {
        public static List<ContentComparison> PreviewDocumentTypeChanges()
        {
            // compare the library based definitions to the Umbraco DB
            var definedDocTypes = PreviewDocTypes(typeof(DocumentTypeBase), "");

            // add any umbraco defined doc types that don't exist in the class definitions
            definedDocTypes.AddRange(DocumentType.GetAllAsList()
                                         .Where(doctype => definedDocTypes.All(dd => dd.Alias != doctype.Alias))
                                         .Select(docType => new ContentComparison { Alias = docType.Alias, DocumentTypeStatus = Status.Deleted }));

            return definedDocTypes;
        }

        private static bool IsUnchanged(Type typeDocType, Type parentDocType, DocumentType docType)
        {
            DocumentTypeAttribute docTypeAttr = DocumentTypeManager.GetDocumentTypeAttribute(typeDocType);

            string docTypeName = String.IsNullOrEmpty(docTypeAttr.Name) ? typeDocType.Name : docTypeAttr.Name;
            string docTypeAlias = DocumentTypeManager.GetDocumentTypeAlias(typeDocType);

            if (docType.Text != docTypeName)
            {
                return false;
            }

            if (docType.Alias != docTypeAlias)
            {
                return false;
            }

            if (docType.IconUrl != docTypeAttr.IconUrl)
            {
                return false;
            }

            if (docType.Thumbnail != docTypeAttr.Thumbnail)
            {
                return false;
            }

            if (docType.Description != docTypeAttr.Description)
            {
                return false;
            }

            if (docType.MasterContentType != 0)
            {
                if (parentDocType == typeof(DocumentTypeBase))
                {
                    return false;
                }

                var existingParentDocumentType = ContentType.GetContentType(docType.MasterContentType);

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

            if (!CompareContentTypeProperties(typeDocType, docType))
            {
                return false;
            }

            if (!CompareAllowedTemplates(docType, docTypeAttr, typeDocType))
            {
                return false;
            }

            return true;
        }

        private static bool CompareAllowedTemplates(DocumentType docType, DocumentTypeAttribute docTypeAttr, Type typeDocType)
        {
            List<Template> allowedTemplates = DocumentTypeManager.GetAllowedTemplates(docTypeAttr, typeDocType);

            Template[] existingTemplates = docType.allowedTemplates;

            if (allowedTemplates.Count != existingTemplates.Count())
            {
                return false;
            }

            foreach (Template template in allowedTemplates)
            {
                if (!existingTemplates.Any(t => t.Alias == template.Alias))
                {
                    return false;
                }
            }

            //TODO: not sure about the logic here
            int defaultTemplateId = DocumentTypeManager.GetDefaultTemplate(docTypeAttr, typeDocType, allowedTemplates);

            if (defaultTemplateId != 0)
            {
                return (docType.DefaultTemplate == defaultTemplateId);
            }

            if (allowedTemplates.Count == 1) 
            {
                return (docType.DefaultTemplate == allowedTemplates.First().Id);
            }
            
            return true;
        }

        private static bool CompareContentTypeProperties(Type uSiteBuilderDocType, ContentType contentType)
        {
            foreach (PropertyInfo propInfo in uSiteBuilderDocType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                DocumentTypePropertyAttribute propAttr = Util.GetAttribute<DocumentTypePropertyAttribute>(propInfo);
                if (propAttr == null)
                {
                    continue; // skip this property - not part of a document type
                }

                // getting name and alias
                string propertyName;
                string propertyAlias;
                DocumentTypeManager.ReadPropertyNameAndAlias(propInfo, propAttr, out propertyName, out propertyAlias);
                
                if (Util.GetAttribute<ObsoleteAttribute>(propInfo) != null)
                {
                    // If it's marked as obsolete, but still exists, there's a change
                    if(contentType.getPropertyType(propertyAlias) != null)
                    {
                        return false;
                    }
                    
                    // marked as obsolete and already deleted, skip rest of checks
                    continue;
                }

                var dataTypeDefinition = ManagerBase.GetDataTypeDefinition(uSiteBuilderDocType, propAttr, propInfo);

                // getting property if already exists, or creating new if it not exists
                PropertyType property = contentType.getPropertyType(propertyAlias);
                if (property == null) // if not exists, new
                {
                    return false;
                }

                if (property.DataTypeDefinition.Id != dataTypeDefinition.Id) // if data type definition changed
                {
                    return false;
                }

                var tabAsString = propAttr.TabAsString;

                if (string.IsNullOrEmpty(tabAsString))
                {
                    tabAsString = Util.GetAttribute<DocumentTypeAttribute>(uSiteBuilderDocType).DefaultTab;
                }

                if (!String.IsNullOrEmpty(tabAsString))
                {
                    if (tabAsString == DocumentTypeDefaultValues.TabGenericProperties)
                    {
                        if (property.TabId != 0)
                        {
                            return false;
                        }
                    }
                    else
                    {

                        var tab =
                            contentType.PropertyTypeGroups.FirstOrDefault(
                                t => t.Name == tabAsString && t.ContentTypeId == contentType.Id);
                        if (tab == null)
                        {
                            return false;
                        }

                        if (property.PropertyTypeGroup != tab.Id)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (property.TabId != 0)
                    {
                        return false;
                    }
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
            List<Type> firstLevelSubTypes = Util.GetFirstLevelSubTypes(parentDocType);

            foreach (Type typeDocType in firstLevelSubTypes)
            {
                string alias = DocumentTypeManager.GetDocumentTypeAlias(typeDocType);

                DocumentType docType = DocumentType.GetByAlias(alias);
                if (docType == null)
                {
                    comparison.Add(new ContentComparison { Alias = alias, DocumentTypeStatus = Status.New, ParentAlias = parentAlias });
                }
                else
                {
                    bool unchanged = IsUnchanged(typeDocType, parentDocType, docType);

                    if (unchanged)
                    {
                        comparison.Add(new ContentComparison { Alias = alias, DocumentTypeStatus = Status.Same, ParentAlias = parentAlias });
                    }
                    else
                    {
                        comparison.Add(new ContentComparison { Alias = alias, DocumentTypeStatus = Status.Changed, ParentAlias = parentAlias });
                    }
                }

                comparison.AddRange(PreviewDocTypes(typeDocType, alias));
            }

            return comparison;
        }
    }
}
