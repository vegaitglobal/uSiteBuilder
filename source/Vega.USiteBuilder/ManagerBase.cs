using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Vega.USiteBuilder
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using umbraco.BusinessLogic;
    using System.Reflection;
    using umbraco.cms.businesslogic.datatype;

    /// <summary>
    /// Base class for all managers
    /// </summary>
    internal class ManagerBase
    {
        protected User siteBuilderUser = Util.GetSiteBuilderUmbracoUser();
        static readonly IContentTypeService ContentTypeService = ApplicationContext.Current.Services.ContentTypeService;

        // string = name, Type = synchronized type
        private Dictionary<string, Type> _synchronizedNames = new Dictionary<string, Type>();
        // string = alias, Type = synchronized type
        private Dictionary<string, Type> _synchronizedAliases = new Dictionary<string, Type>();

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
                    if (this._synchronizedNames.ContainsKey(name))
                    {
                        throw new ArgumentException(this.GetExceptionText(name, alias, typeSynced));
                    }
                    else
                    {
                        this._synchronizedNames.Add(name, typeSynced);
                    }
                }

                // check alias
                if (this._synchronizedAliases.ContainsKey(alias))
                {
                    throw new ArgumentException(this.GetExceptionText(name, alias, typeSynced));
                }
                else
                {
                    this._synchronizedAliases.Add(alias, typeSynced);
                }
            }
            catch
            {
                throw new ArgumentException(this.GetExceptionText(name, alias, typeSynced));
            }
        }

        private string GetExceptionText(string alias, string name, Type type)
        {
            string retVal = "";
            retVal += string.Format("Alias/Name duplicated ({0}/{1}). Type causing problem: '{2}'. Already synchronized types are:\n", alias, name, type.FullName);

            foreach (KeyValuePair<string, Type> syncedAlias in this._synchronizedAliases)
            {
                // get the name by using the type from synced aliases
                string typeName = "";

                if (this._synchronizedNames.ContainsValue(syncedAlias.Value))
                {
                    typeName = this._synchronizedNames.First(sn => sn.Value == syncedAlias.Value).Key;
                }

                retVal += string.Format("Alias: {0}, Name: {1}, Type: {2}, Assembly: {3}\n",
                    syncedAlias.Key, typeName, syncedAlias.Value.FullName, syncedAlias.Value.Assembly.FullName);
            }

            return retVal;
        }


        protected void SynchronizeContentTypeProperties(Type typeContentType, IContentType contentType, DocumentTypeAttribute documentTypeAttribute, out bool hadDefaultValues)
        {
            SynchronizeContentTypeProperties(typeContentType, contentType, documentTypeAttribute, out hadDefaultValues, false);
        }

        //#region [Document type properties synchronization]

        /// <summary>
        /// Synchronizes content type properties
        /// </summary>
        /// <param name="typeContentType">ContentType type</param>
        /// <param name="contentType">Umbraco content type</param>
        /// <param name="hadDefaultValues">set to true if some of properties has default values</param>
        protected void SynchronizeContentTypeProperties(Type typeContentType, IContentType contentType, DocumentTypeAttribute documentTypeAttribute, out bool hadDefaultValues, bool updateMixins)
        {
            // sync the mixins first so that any properties are overwritten by the specific properties on the class
            if ((documentTypeAttribute.Mixins != null) && (updateMixins == false))
            {
                foreach (Type mixinType in documentTypeAttribute.Mixins)
                {
                    SynchronizeContentTypeProperties(mixinType, contentType, documentTypeAttribute, out hadDefaultValues, true);
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

                // Getting name and alias
                string propertyName;
                string propertyAlias;
                DocumentTypeManager.ReadPropertyNameAndAlias(propInfo, propAttr, out propertyName, out propertyAlias);

                // Remove property if it has Obsolete attribute
                if (this.RemoveIfObsolete(contentType, propInfo, propertyAlias))
                {
                    continue; 
                }

                if (propAttr.DefaultValue != null)
                {
                    hadDefaultValues = true; // at least one property has a default value
                }

                DataTypeDefinition dataTypeDefinition = GetDataTypeDefinition(typeContentType, propAttr, propInfo);

                // getting property if already exists, or creating new if it not exists
                Umbraco.Core.Models.PropertyType propertyType = contentType.PropertyTypes.FirstOrDefault(p => p.Alias == propertyAlias);
                if (propertyType == null) // if not exists, create it
                {
                    Util.AddPropertyType(contentType, dataTypeDefinition, propertyAlias, propertyName, propAttr.TabAsString);
                    propertyType = contentType.PropertyTypes.FirstOrDefault(p => p.Alias == propertyAlias);
                }
                else
                {
                    if (propertyType.DataTypeDefinitionId != dataTypeDefinition.Id)
                    {
                        propertyType.DataTypeDefinitionId = dataTypeDefinition.Id;
                    }
                }

                // Setting up the tab of this property. If tab doesn't exists, create it.
                if (!string.IsNullOrEmpty(propAttr.TabAsString) && propAttr.TabAsString.ToLower() != DocumentTypeDefaultValues.TabGenericProperties.ToLower())
                {
                    // try to find this tab
                    PropertyGroup pg = contentType.PropertyGroups.FirstOrDefault(x => x.Name == propAttr.TabAsString);
                    if (pg == null) // if found
                    {
                        contentType.AddPropertyGroup(propAttr.TabAsString);
                        pg = contentType.PropertyGroups.FirstOrDefault(x => x.Name == propAttr.TabAsString);
                    }

                    if (propAttr.TabOrder.HasValue)
                    {
                        pg.SortOrder = propAttr.TabOrder.Value;
                    }

                    if (!pg.PropertyTypes.Any(x => x.Alias == propertyType.Alias))
                    {
                        contentType.MovePropertyType(propertyType.Alias, propAttr.TabAsString);
                    }
                }
                else if ((propAttr.TabAsString == string.Empty) || (propAttr.TabAsString.ToLower() == "generic properties"))
                {
                    // In case when some property exists and needs to be moved to "Generic Properties" tab
                    contentType.MovePropertyType(propertyType.Alias, null);                    
                }

                propertyType.Name = propertyName;
                propertyType.Mandatory = propAttr.Mandatory;
                propertyType.ValidationRegExp = propAttr.ValidationRegExp;
                propertyType.Description = propAttr.Description;
                propertyType.SortOrder = propertySortOrder;

                propertySortOrder++;
            }
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
                dataTypeDefinition = DataTypeDefinition.GetDataTypeDefinition((int)propAttr.Type);
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
        private bool RemoveIfObsolete(IContentType contentType, PropertyInfo propInfo, string propertyAlias)
        {
            bool isObsolete = false;

            if (HasObsoleteAttribute(propInfo))
            {
                Util.DeletePropertyType(contentType, propertyAlias);

                isObsolete = true;
            }

            return isObsolete;
        }
        //#endregion
    }
}
