using System.Web;
using System.Xml;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Vega.USiteBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml.XPath;
    using umbraco.BusinessLogic;
    using umbraco.cms.businesslogic.datatype;
    using umbraco.cms.businesslogic.member;
    using umbraco.cms.businesslogic.propertytype;
    using Vega.USiteBuilder.Configuration;

    /// <summary>
    /// Contains various utility methods
    /// </summary>
    internal static class Util
    {
        private static Version UmbracoVersion { get; set; }
        private static List<Type> _types = null;
        static readonly IDataTypeService DataTypeService = ApplicationContext.Current.Services.DataTypeService;

        private static List<Type> Types
        {
            get
            {
                if (_types == null)
                {
                    _types = new List<Type>();
                    LoadTypes();
                }

                return _types;
            }
        }

        static readonly IContentTypeService ContentTypeService = ApplicationContext.Current.Services.ContentTypeService;

        private static void LoadTypes()
        {
            foreach (Assembly assembly in GetSiteBuilderAssemblies())
            {
                Module[] modules = assembly.GetLoadedModules();

                foreach (Module module in modules)
                {
                    Type[] types = null;
                    try
                    {
                        types = module.GetTypes();
                        _types.AddRange(types);
                    }
                    catch
                    {
                        //TODO: add logging/exception handling
                    } // required because Exception is thrown for some dlls when .GetTypes method is called
                }
            }
        }

        /// <summary>
        /// Gets all first level subtypes (directly inherited types) of type given as argument. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Type> GetFirstLevelSubTypes(Type type)
        {
#if DEBUG
            Stopwatch timer = new Stopwatch();
            timer.Start();
#endif

            List<Type> retVal = new List<Type>();

            // NOTE: The similar functionality is located in the method bellow
            try
            {
                foreach (Type t in Types)
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

#if DEBUG
            timer.Stop();
            //StopwatchLogger.AddToLog(string.Format("Total elapsed time for method 'GetFirstLevelSubTypes': {0}ms.", timer.ElapsedMilliseconds));
            timer.Restart();
#endif
            return retVal;
        }

        /// <summary>
        /// Get's all subtypes of a given type.
        /// </summary>
        public static List<Type> GetAllSubTypes(Type type)
        {
#if DEBUG
            Stopwatch timer = new Stopwatch();
            timer.Start();
#endif
            List<Type> retVal = new List<Type>();

            // NOTE: The similar functionality is located in the method above
            try
            {
                foreach (Type t in Types)
                {
                    if (t.BaseType != null && t.IsSubclassOf(type))
                    {
                        retVal.Add(t);
                    }
                }
            }
            catch { } // required because Exception is thrown for some dlls when .GetTypes method is called

#if DEBUG
            timer.Stop();
            //StopwatchLogger.AddToLog(string.Format("Total elapsed time for method 'GetAllSubTypes': {0}ms.", timer.ElapsedMilliseconds));
            timer.Restart();
#endif
            return retVal;
        }

        private static IEnumerable<Assembly> GetSiteBuilderAssemblies()
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

                return result;
            }
            else
            {
                // Enforcing loading of assemblies from bin folder because they are not all loaded when uSiteBuilder needs them.
                LoadAssembliesFromBinFolder();

                return AppDomain.CurrentDomain.GetAssemblies();
            }
        }

        private static void LoadAssembliesFromBinFolder()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            List<string> loadedPaths = new List<string>();
            foreach (Assembly assembly in loadedAssemblies)
            {
                try
                {
                    if (!string.IsNullOrEmpty(assembly.Location))
                    {
                        loadedPaths.Add(assembly.Location);
                    }
                }
                catch
                {
                    // Do nothing, just catch the not supported exception and continue.
                }
            }

            var referencedPaths = Directory.GetFiles(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin")), "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
            toLoad.ForEach(path => AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path)));
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
            try
            {
                User adminUser = User.GetUser(0);
                if (adminUser == null)
                {
                    adminUser = User.getAll().FirstOrDefault(u=>u.IsAdmin());
                }

                return adminUser;
            }
            catch (Exception)
            {
                return null;
            }
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
        /// <param name="tabName">Name of the tab.</param>
        public static void AddPropertyType(IContentTypeBase contentType, DataTypeDefinition dt, string alias, string name, string tabName)
        {
            IDataTypeDefinition idt = DataTypeService.GetDataTypeDefinitionById(dt.Id);
            Umbraco.Core.Models.PropertyType pt = new Umbraco.Core.Models.PropertyType(idt)
            {
                Alias = alias,
                Name = name
            };

            if (String.IsNullOrEmpty(tabName))
            {
                contentType.AddPropertyType(pt);
            }
            else
            {
                contentType.AddPropertyType(pt, tabName);
            }
        }

        public static void DeletePropertyType(IContentType contentType, string alias)
        {
            contentType.RemovePropertyType(alias);
        }

        public static void DeletePropertyType(MemberType memberType, string alias)
        {
            PropertyType propertyType = memberType.PropertyTypes.FirstOrDefault(x => x.Alias.Equals(alias, StringComparison.OrdinalIgnoreCase));
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
            if (UmbracoVersion == null)
            {                
                UmbracoVersion = typeof(umbraco.content).Assembly.GetName().Version;
            }

            return UmbracoVersion;
        }


        /// <summary>
        /// Determines whether current instance of Umbraco is of version 6.0.6 or higher
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if current Umbraco instance is v6.0.6 or higher; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUmbraco606OrHigher()
        {
            return GetCurrentUmbracoVersion() >= new Version(1, 0, 4898, 0);
        }

        public static bool IsUmbraco700OrHigher()
        {
            return GetCurrentUmbracoVersion() >= new Version(1, 0, 5073, 23298);
        }

        /// <summary>
        /// Determines whether current instance of Umbraco is of version 6.1.6 or higher
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if current Umbraco instance is v6.1.6 or higher; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUmbraco616OrHigher()
        {
            return GetCurrentUmbracoVersion() >= new Version(1, 0, 5021, 24868);
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

            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
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
                            if (value == "WebForms")
                            {
                                engine = RenderingEngine.WebForms;
                            }
                            else if (value == "Mvc")
                            {
                                engine = RenderingEngine.Mvc;
                            }
                            else
                            {
                                engine = RenderingEngine.Unknown;
                            }

                            //Enum.TryParse(value, true, out engine);
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
