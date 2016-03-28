namespace Vega.USiteBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml.XPath;
    using System.Xml;

    using umbraco.BusinessLogic;
    using umbraco.cms.businesslogic.web;
    using umbraco.presentation.nodeFactory;
    using umbraco;

    using Vega.USiteBuilder.Types;
    using umbraco.interfaces;
    using umbraco.cms.businesslogic;
    using umbraco.cms.businesslogic.property;

    /// <summary>
    /// NOT USED YET
    /// </summary>
    internal class ContentUtil
    {
        /// <summary>
        /// Contains list of all custom type convertors.
        /// </summary>
        internal static Dictionary<Type, ICustomTypeConvertor> PropertyConvertors = new Dictionary<Type, ICustomTypeConvertor>();

        /// <summary>
        /// Registers document type property convertor.
        /// </summary>
        /// <param name="propertyType">Document type property type</param>
        /// <param name="convertor">Convertor implementation</param>
        internal static void RegisterDocumentTypePropertyConvertor(Type propertyType, ICustomTypeConvertor convertor)
        {
            if (PropertyConvertors.ContainsKey(propertyType))
            {
                throw new Exception(string.Format("Failed registering convertor '{0}'. Convertor '{1}' already registered with type '{2}'",
                    convertor.GetType().FullName, PropertyConvertors[propertyType].GetType().FullName,
                    propertyType));
            }
            else
            {
                PropertyConvertors.Add(propertyType, convertor);
            }
        }

        public static void ReadProperties(Type contentType, Content content, object output)
        {
            foreach (PropertyInfo propInfo in contentType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    DocumentTypePropertyAttribute propAttr = Util.GetAttribute<DocumentTypePropertyAttribute>(propInfo);
                    if (propAttr == null)
                    {
                        continue; // skip this property - not part of a Document Type
                    }

                    string propertyName;
                    string propertyAlias;
                    DocumentTypeManager.ReadPropertyNameAndAlias(propInfo, propAttr, out propertyName, out propertyAlias);

                    umbraco.cms.businesslogic.property.Property property = content.getProperty(propertyAlias);
                    
                    object value = null;
                    try
                    {
                        if (property == null)
                        {
                            value = null;
                        }
                        else if (propInfo.PropertyType.Equals(typeof(System.Boolean)))
                        {
                            if (String.IsNullOrEmpty(Convert.ToString(property.Value)) || Convert.ToString(property.Value) == "0")
                            {
                                value = false;
                            }
                            else
                            {
                                value = true;
                            }
                        }
                        else if (PropertyConvertors.ContainsKey(propInfo.PropertyType))
                        {
                            value = ContentUtil.GetInnerXml(content.Id.ToString(), propertyAlias);
                        }
                        else if (String.IsNullOrEmpty(Convert.ToString(property.Value)))
                        {
                            // if property type is string or if it's some custom type, try to get the inner xml of this property within a node.
                            if (propInfo.PropertyType == typeof(string) ||
                                PropertyConvertors.ContainsKey(propInfo.PropertyType))
                            {
                                value = ContentUtil.GetInnerXml(content.Id.ToString(), propertyAlias);
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
                        else
                        {
                            value = Convert.ChangeType(property.Value, propInfo.PropertyType);
                        }

                        if (PropertyConvertors.ContainsKey(propInfo.PropertyType))
                        {
                            value = PropertyConvertors[propInfo.PropertyType].ConvertValueWhenRead(value);
                        }

                        propInfo.SetValue(output, value, null);
                    }
                    catch (Exception exc)
                    {
                        throw new Exception(string.Format("Cannot set the value of a document type property {0}.{1} (document type: {2}) to value: '{3}' (value type: {4}). Error: {5}",
                            contentType.Name, propInfo.Name, propInfo.PropertyType.FullName,
                            value, value != null ? value.GetType().FullName : "", exc.Message));
                    }
                }
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
    }
}
