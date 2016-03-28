using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.propertytype;
using Vega.USiteBuilder.DataTypeBuilder;
using Vega.USiteBuilder.DocumentTypeBuilder;

namespace Vega.USiteBuilder
{
    /// <summary>
    /// Base class for all managers
    /// </summary>
    internal class ManagerBase
    {
        protected User siteBuilderUser = Util.GetSiteBuilderUmbracoUser();

        // string = name, Type = synchronized type
        private readonly Dictionary<string, Type> _synchronizedNames = new Dictionary<string, Type>();
        // string = alias, Type = synchronized type
        private readonly Dictionary<string, Type> _synchronizedAliases = new Dictionary<string, Type>();

        /// <summary>
        /// This method checks if an item with the given name and alias is already synchronized.
        /// This method is used to used as a constraint that all items must have unique names and aliases.
        /// </summary>
        /// <param name="name">Name of an item for synchronization. Can be null in which case name constraint is not checked</param>
        /// <param name="alias">Alias of an item for synchronization.</param>
        /// <param name="typeSynced">Type being synchronized</param>
        protected void AddToSynchronized(string name, string alias, Type typeSynced)
        {
            try
            {
                // check name
                if (!string.IsNullOrEmpty(name))
                {
                    if (_synchronizedNames.ContainsKey(name))
                    {
                        throw new ArgumentException(GetExceptionText(name, alias, typeSynced));
                    }
                    else
                    {
                        _synchronizedNames.Add(name, typeSynced);
                    }
                }

                // check alias
                if (_synchronizedAliases.ContainsKey(alias))
                {
                    throw new ArgumentException(GetExceptionText(name, alias, typeSynced));
                }
                else
                {
                    _synchronizedAliases.Add(alias, typeSynced);
                }
            }
            catch
            {
                throw new ArgumentException(GetExceptionText(name, alias, typeSynced));
            }
        }

        private string GetExceptionText(string alias, string name, Type type)
        {
            string retVal = "";
            retVal += string.Format("Alias/Name duplicated ({0}/{1}). Type causing problem: '{2}'. Already synchronized types are:\n", alias, name, type.FullName);

            foreach (KeyValuePair<string, Type> syncedAlias in _synchronizedAliases)
            {
                // get the name by using the type from synced aliases
                string typeName = "";

                if (_synchronizedNames.ContainsValue(syncedAlias.Value))
                {
                    typeName = _synchronizedNames.First(sn => sn.Value == syncedAlias.Value).Key;
                }

                retVal += string.Format("Alias: {0}, Name: {1}, Type: {2}, Assembly: {3}\n",
                    syncedAlias.Key, typeName, syncedAlias.Value.FullName, syncedAlias.Value.Assembly.FullName);
            }

            return retVal;
        }

        #region [Document type properties synchronization]
        /// <summary>
        /// Synchronizes content type properties
        /// </summary>
        /// <param name="typeContentType">ContentType type</param>
        /// <param name="contentType">Umbraco content type</param>
        /// <param name="hadDefaultValues">set to true if some of properties has default values</param>
        protected void SynchronizeContentTypeProperties(Type typeContentType, ContentType contentType, out bool hadDefaultValues)
        {
            var documentTypeAttribute = DocumentTypeManager.GetDocumentTypeAttribute(typeContentType);

            // sync the mixins first so that any properties are overwritten by the specific properties on the class
            if (documentTypeAttribute.Mixins != null)
            {
                foreach (Type mixinType in documentTypeAttribute.Mixins)
                {
                    SynchronizeContentTypeProperties(mixinType, contentType, out hadDefaultValues);
                    contentType.Save();
                }
            }

            hadDefaultValues = false;

            int propertySortOrder = 0;
            foreach (PropertyInfo propInfo in typeContentType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
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

                if (RemoveIfObsolete(contentType, propInfo, propertyAlias))
                {
                    continue; // skip this property as it's obsolete
                }

                if (propAttr.DefaultValue != null)
                {
                    hadDefaultValues = true; // at least one property has a default value
                }

                var dataTypeDefinition = GetDataTypeDefinition(typeContentType, propAttr, propInfo);

                // getting property if already exists, or creating new if it not exists
                PropertyType property = contentType.getPropertyType(propertyAlias);
                if (property == null) // if not exists, create it
                {
                    //contentType.AddPropertyType(dataTypeDefinition, propertyAlias, propertyName);
                    Util.AddPropertyType(contentType, dataTypeDefinition, propertyAlias, propertyName);
                    property = contentType.getPropertyType(propertyAlias);
                }
                else
                {
                    if (property.DataTypeDefinition.Id != dataTypeDefinition.Id) // if data type definition changed
                    {
                        property.DataTypeDefinition = dataTypeDefinition;
                    }
                }

                string tabName = propAttr.TabAsString;

                if (string.IsNullOrEmpty(tabName))
                {
                    tabName = Util.GetAttribute<DocumentTypeAttribute>(typeContentType).DefaultTab;
                }

                // Setting up the tab of this property. If tab doesn't exists, create it.
                if (!string.IsNullOrEmpty(tabName) && tabName.ToLower() != DocumentTypeDefaultValues.TabGenericProperties.ToLower())
                {
                    int tabId;

                    var tab = contentType.getVirtualTabs.FirstOrDefault(t => t.Caption == tabName);

                    if (tab == null)
                    {
                        tabId = contentType.AddVirtualTab(tabName);
                    }
                    else
                    {
                        tabId = tab.Id;
                        // inherited tab
                        if (tab.ContentType != contentType.Id)
                        {
                            var propertyTypeGroup = contentType.PropertyTypeGroups.FirstOrDefault(t => t.Name == tabName && t.ContentTypeId == contentType.Id);

                            if (propertyTypeGroup == null)
                            {
                                // create a new inherited tab                   
                                propertyTypeGroup = new PropertyTypeGroup(tab.Id, contentType.Id,
                                                                              tabName);

                                propertyTypeGroup.Save();
                            }
                            tabId = propertyTypeGroup.Id;
                        }
                    }


                    if (propAttr.TabOrder.HasValue)
                    {
                        contentType.SetTabSortOrder(tabId, propAttr.TabOrder.Value);
                    }

                    property.PropertyTypeGroup = tabId;
                }
                else
                {
                    // move to generic properties
                    property.PropertyTypeGroup = 0;
                }

                // updating
                property.Name = propertyName;
                property.Mandatory = propAttr.Mandatory;
                property.ValidationRegExp = propAttr.ValidationRegExp;
                property.Description = propAttr.Description;
                property.SortOrder = propertySortOrder;

                property.Save();

                propertySortOrder++;

                // refresh document type to load tabs again
                contentType = ContentType.GetByAlias(contentType.Alias);
            } // foreach
        }

        public static DataTypeDefinition GetDataTypeDefinition(Type typeContentType, DocumentTypePropertyAttribute propAttr,
                                                                PropertyInfo propInfo)
        {
            // getting data type definition
            DataTypeDefinition dataTypeDefinition = null;

            // Convention mapping for property types <==> Umbraco Property types
            if (propAttr.Type == null)
            {
                if (propInfo.PropertyType == typeof(string))
                {
                    return DataTypeDefinition.GetDataTypeDefinition((int)UmbracoPropertyType.Textstring);
                }
                if (propInfo.PropertyType == typeof(DateTime))
                {
                    return DataTypeDefinition.GetDataTypeDefinition((int)UmbracoPropertyType.DatePicker);
                }
                if (propInfo.PropertyType == typeof(bool))
                {
                    return DataTypeDefinition.GetDataTypeDefinition((int)UmbracoPropertyType.TrueFalse);
                }
                if (propInfo.PropertyType == typeof(int) || propInfo.PropertyType == typeof(decimal) || propInfo.PropertyType == typeof(double))
                {
                    return DataTypeDefinition.GetDataTypeDefinition((int)UmbracoPropertyType.Numeric);
                }
                if (propInfo.PropertyType == typeof(HtmlString))
                {
                    return DataTypeDefinition.GetDataTypeDefinition((int)UmbracoPropertyType.RichtextEditor);
                }
            }

            if (propAttr.Type == UmbracoPropertyType.Other)
            {
                // If the OtherType is specified we need to do some reflection to get the name 
                // from the attribute on the class definition
                if (propAttr.OtherType != null)
                {
                    var dataTypeAttribute =
                        (DataTypeAttribute)
                        Attribute.GetCustomAttributes(propAttr.OtherType, typeof(DataTypeAttribute)).First();
                    var dataTypeName = dataTypeAttribute.Name;

                    dataTypeDefinition = DataTypeDefinition.GetAll().FirstOrDefault(itm => itm.Text == dataTypeName);
                    if (dataTypeDefinition == null)
                    {
                        throw new Exception(
                            string.Format(
                                "Property '{1}.{0}' is set as 'Other' umbraco data type ('{2}') but data type '{2}' cannot be found in Umbraco.",
                                propInfo.Name, typeContentType.Name, dataTypeName));
                    }
                }
                else if (!string.IsNullOrEmpty(propAttr.OtherTypeName))
                {
                    dataTypeDefinition = DataTypeDefinition.GetAll().FirstOrDefault(itm => itm.Text == propAttr.OtherTypeName);
                    if (dataTypeDefinition == null)
                    {
                        throw new Exception(
                            string.Format(
                                "Property '{1}.{0}' is set as 'Other' umbraco data type ('{2}') but data type '{2}' cannot be found in Umbraco.",
                                propInfo.Name, typeContentType.Name, propAttr.OtherTypeName));
                    }
                }
                else
                {
                    throw new Exception(
                        string.Format(
                            "Property '{1}.{0}' is set as 'Other' umbraco data type but 'OtherTypeName' on that property is empty. Please set 'OtherTypeName' on property '{1}.{0}'.",
                            propInfo.Name, typeContentType.Name));
                }
            }
            else
            {
                if (propAttr.Type.HasValue)
                {
                    dataTypeDefinition = DataTypeDefinition.GetDataTypeDefinition((int) propAttr.Type);
                }
            }
            return dataTypeDefinition;
        }

        protected static bool HasObsoleteAttribute(PropertyInfo propInfo)
        {
            return Util.GetAttribute<ObsoleteAttribute>(propInfo) != null;

        }

        /// <summary>
        /// Removes property if it's obsolete.
        /// </summary>
        /// <param name="contentType">Content type</param>
        /// <param name="propInfo">Property info</param>
        /// <param name="propertyAlias">Property alias</param>
        /// <returns>True if property is obsolete and removed. False if this property is not obsolete</returns>
        private bool RemoveIfObsolete(ContentType contentType, PropertyInfo propInfo, string propertyAlias)
        {
            bool isObsolete = false;

            if (HasObsoleteAttribute(propInfo))
            {
                Util.DeletePropertyType(contentType, propertyAlias);

                isObsolete = true;
            }

            return isObsolete;
        }
        #endregion
    }
}
