using System.Web;
using System.Web.Caching;
using System.Xml;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;

namespace Vega.USiteBuilder
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml.XPath;

    using umbraco.BusinessLogic;
    using umbraco.cms.businesslogic;
    using umbraco.cms.businesslogic.datatype;
    using umbraco.cms.businesslogic.propertytype;

    using Vega.USiteBuilder.Configuration;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Contains various utility methods
    /// </summary>
    internal static class Util
    {
        private static Version UmbracoVersion { get; set; }

        /// <summary>
        /// Gets all first level subtypes (directly inherited types) of type given as argument. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Type> GetFirstLevelSubTypes(Type type)
        {
            List<Type> retVal = new List<Type>();

            Assembly[] assemblies = GetSiteBuilderAssemblies();

            // NOTE: The similar functionality is located in the method bellow
            foreach (Assembly assembly in assemblies)
            {
                Module[] modules = assembly.GetLoadedModules();

                foreach (Module module in modules)
                {
                    Type[] types = null;
                    try
                    {
                        types = module.GetTypes();
                        foreach (Type t in types)
                        {
                            if (t.BaseType != null && (t.BaseType.Equals(type) ||
                                (type.IsGenericType && type.Name == t.BaseType.Name)) ||
                                t.GetInterfaces().FirstOrDefault(i => i == type) != null)
                            {
                                retVal.Add(t);
                            }
                        }
                    }
                    catch { } // required because Exception is thrown for some dlls when .GetTypes method is called
                }
            }

            return retVal;
        }

        /// <summary>
        /// Get's all subtypes of a given type.
        /// </summary>
        public static List<Type> GetAllSubTypes(Type type)
        {
            List<Type> retVal = new List<Type>();

            Assembly[] assemblies = GetSiteBuilderAssemblies();

            // NOTE: The similar functionality is located in the method above
            foreach (Assembly assembly in assemblies)
            {
                Module[] modules = assembly.GetLoadedModules();

                foreach (Module module in modules)
                {
                    try
                    {
                        Type[] types = null;
                        types = module.GetTypes();
                        foreach (Type t in types)
                        {
                            if (t.BaseType != null && t.IsSubclassOf(type))
                            {
                                retVal.Add(t);
                            }
                        }
                    }
                    catch { } // required because Exception is thrown for some dlls when .GetTypes method is called
                }
            }

            return retVal;
        }

        private static Assembly[] GetSiteBuilderAssemblies()
        {
            if (USiteBuilderConfiguration.Assemblies != "")
            {
                string[] assemblynames = USiteBuilderConfiguration.Assemblies.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                List<Assembly> result = new List<Assembly>();

                foreach (string name in assemblynames)
                {
                    // This will throw an exception if we spelled it wrong.
                    result.Add(Assembly.Load(name));
                }

                result.Add(Assembly.GetExecutingAssembly());

                return result.ToArray();
            }
            else
                return AppDomain.CurrentDomain.GetAssemblies();
        }


        /// <summary>
        /// Returns login name of Umbraco user used by USiteBuilder (for creating document types, templates etc...)
        /// </summary>
        public static User GetSiteBuilderUmbracoUser()
        {
            // user admin user
            User[] users = User.getAllByLoginName(USiteBuilderConfiguration.ApiUser);

            if (users.Length != 1)
            {
                throw new Exception(string.Format("Umbraco user used by USiteBuilder ('{0}') is not found in Umbraco. Please check your web.config (Add <add key=\"siteBuilderUserLoginName\" value=\"someumbracoadminusername\" /> to <appSettings> in web.config).",
                    USiteBuilderConfiguration.ApiUser));
            }

            return users[0];
        }

        /// <summary>
        /// Get's the attribute of a given type from the given type.
        /// Note that if there are multiple attributes of the same type found, this method returns on the first one
        /// so use this method only for searching attributes whose AllowMultiple is set to false.
        /// </summary>
        /// <typeparam name="T">Name of the attribute class</typeparam>
        /// <param name="type">Type whose attributes will be searched</param>
        /// <returns>Attribute found or null of attribute is not found</returns>
        public static T GetAttribute<T>(Type type)
        {
            T retVal;

            object[] attributes = type.GetCustomAttributes(typeof(T), true);
            if (attributes.Length > 0)
            {
                retVal = (T)attributes[0];
            }
            else
            {
                retVal = default(T);
            }

            return retVal;
        }

        /// <summary>
        /// Get's the attribute of a given type from the given type.
        /// Note that if there are multiple attributes of the same type found, this method returns on the first one
        /// so use this method only for searching attributes whose AllowMultiple is set to false.
        /// </summary>
        /// <typeparam name="T">Name of the attribute class</typeparam>
        /// <param name="propertyInfo">PropertyInfo whose attributes will be searched</param>
        /// <returns>Attribute found or null of attribute is not found</returns>
        public static T GetAttribute<T>(PropertyInfo propertyInfo)
        {
            T retVal;

            object[] attributes = propertyInfo.GetCustomAttributes(typeof(T), true);
            if (attributes.Length > 0)
            {
                retVal = (T)attributes[0];
            }
            else
            {
                retVal = default(T);
            }

            return retVal;
        }

        /// <summary>
        /// Returns true if typeArgument is used as an argument in generic type typeGeneric.
        /// </summary>
        /// <param name="typeGeneric">Generic type</param>
        /// <param name="typeArgument">Generic type argument type</param>
        /// <returns>true if typeArgument is used as an argument in generic type typeGeneric</returns>
        public static bool IsGenericArgumentTypeOf(Type typeGeneric, Type typeArgument)
        {
            foreach (Type t in typeGeneric.GetGenericArguments())
            {
                if (t.Equals(typeArgument)) return true;
            }

            if (typeGeneric.BaseType != null)
            {
                return IsGenericArgumentTypeOf(typeGeneric.BaseType, typeArgument);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets admin user
        /// </summary>
        /// <returns></returns>
        public static User GetAdminUser()
        {
            return User.GetUser(0);
        }

        /// <summary>
        /// Get's media url by id
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public static string GetMediaUrlById(int mediaId)
        {
            string retVal = String.Empty;
            try
            {
                XPathNodeIterator xpathNodeIterator = umbraco.library.GetMedia(mediaId, false);
                if (xpathNodeIterator != null && mediaId != 0)
                {
                    xpathNodeIterator.MoveNext();
                    retVal = xpathNodeIterator.Current.SelectSingleNode("umbracoFile").Value;
                }
            }
            catch
            {
            }

            return retVal;
        }

        /// <summary>
        /// Fix for incompatible Umbraco versions
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="dt">The dt.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="name">The name.</param>
        public static void AddPropertyType(ContentType contentType, DataTypeDefinition dt, string alias, string name)
        {
            typeof(ContentType).GetMethod("AddPropertyType").Invoke(contentType, new object[] { dt, alias, name });
        }

        public static void DeletePropertyType(ContentType contentType, string alias)
        {
            PropertyType propertyType = contentType.PropertyTypes.SingleOrDefault(pt => pt.Alias == alias);

            if (propertyType != null)
            {
                propertyType.delete();
            }
        }

        /// <summary>
        /// Gets the current Umbraco version
        /// </summary>
        /// <returns></returns>
        public static Version GetCurrentUmbracoVersion()
        {
            if (Util.UmbracoVersion == null)
            {
                //Util.UmbracoVersion = Assembly.LoadFrom("umbraco.dll").GetName().Version;
                Util.UmbracoVersion = typeof(umbraco.content).Assembly.GetName().Version;
            }

            return Util.UmbracoVersion;
        }


        /// <summary>
        /// Determines whether current instance of Umbraco is of version 4.7 or higher
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if current Umbraco instance is v4.7 or higher; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUmbraco47orHigher()
        {
            bool retVal = false;

            if (Util.GetCurrentUmbracoVersion() >= new Version(1, 0, 4077, 0))
            {
                retVal = true;
            }

            return retVal;
        }

        /// <summary>
        /// Serialize the object to JSON string format.
        /// </summary>
        /// <typeparam name="T">Object to serialized</typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string JsonSerialize<T>(T obj)
        {
            string retVal = null;

            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                retVal = Encoding.Default.GetString(ms.ToArray());
            }

            return retVal;
        }

        /// <summary>
        /// Deserializes the object from JSON string format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string json)
        {
            T obj = default(T);

            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
                obj = (T)serializer.ReadObject(ms);
            }

            return obj;
        }

        private static RenderingEngine? _defaultRenderingEngine;

        /// <summary>
        /// Enables MVC, and at the same time disable webform masterpage templates.
        /// This ensure views are automaticly created instead of masterpages.
        /// Views are display in the tree instead of masterpages and a MVC template editor
        /// is used instead of the masterpages editor
        /// </summary>
        /// <value><c>true</c> if umbraco defaults to using MVC views for templating, otherwise <c>false</c>.</value>
        public static RenderingEngine DefaultRenderingEngine
        {
            get
            {
                if (_defaultRenderingEngine == null)
                {
                    var engine = RenderingEngine.WebForms;
                    var node = SettingsDocument().DocumentElement.SelectSingleNode("/settings/templates/defaultRenderingEngine");

                    if (node != null && node.FirstChild != null & node.FirstChild.Value != null)
                    {
                        var value = node.FirstChild.Value;
                        if (value != null)
                        {
                            Enum.TryParse(value, true, out engine);
                        }
                    }

                    _defaultRenderingEngine = engine;
                }

                return _defaultRenderingEngine.Value;
            }
        }

        private static XmlDocument SettingsDocument()
        {
            var reader = new XmlTextReader(string.Format("{0}{1}config{1}umbracoSettings.config", HttpRuntime.AppDomainAppPath, Path.DirectorySeparatorChar));
            var document = new XmlDocument();
            document.Load(reader);
            return document;
        }

    }
}
