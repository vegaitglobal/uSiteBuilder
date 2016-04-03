using System;
using System.Collections.Generic;
using System.Linq;
using Vega.USiteBuilder.Configuration;
using Vega.USiteBuilder.DataTypeBuilder;
using Vega.USiteBuilder.DocumentTypeBuilder;
using Vega.USiteBuilder.MemberBuilder;
using Vega.USiteBuilder.TemplateBuilder;
using Vega.USiteBuilder.Types;
using Vega.USiteBuilder.WebUserControlsBuilder;

namespace Vega.USiteBuilder
{
    /// <summary>
    /// Umbraco manager
    /// </summary>
    public class UmbracoManager
    {
        private const string SyncObj = "sync";
        private static bool _synchronized;

        /// <summary>
        /// Synchronizes if not synchronized.
        /// </summary>
        public static void SynchronizeIfNotSynchronized()
        {
            // we are not locking immediatly because it will impact performance
            if (!_synchronized)
            {
                lock (SyncObj)
                {
                    if (!_synchronized)
                    {
                        Synchronize();

                        _synchronized = true;
                    }
                }
            }
        }

        private static void Synchronize()
        {
            // stop processing here if the synchroniser is suppressed
            if (USiteBuilderConfiguration.SuppressSynchronization)
            {
                SynchronizeMemberTypes();
                return;
            }

            SynchronizeDatatTypes();
            SynchronizeTemplates();
            SynchronizeDocumentTypes();
            SynchronizeMemberTypes();
            SynchronizeUserControls();
            //SynchronizeRazorMacros();

            // Register convertors only if any document or member types are found
            if (DocumentTypeManager.HasSynchronizedDocumentTypes() || MemberTypeManager.HasSynchronizedMemberTypes())
            {
                RegisterConvertors();
            }
        }

        /// <summary>
        /// Synchronizes the type of the document.
        /// </summary>
        /// <param name="siteBuilderType">Type of the site builder.</param>
        public void SynchronizeDocumentType(Type siteBuilderType)
        {
            new DocumentTypeManager().SynchronizeDocumentType(siteBuilderType);
        }

        /// <summary>
        /// Deletes the type of the document.
        /// </summary>
        /// <param name="alias">The alias.</param>
        public void DeleteDocumentType(string alias)
        {
            new DocumentTypeManager().DeleteDocumentType(alias);
        }

        /// <summary>
        /// Previews the document type changes.
        /// </summary>
        /// <returns></returns>
        public static List<ContentComparison> PreviewDocumentTypeChanges()
        {
            return DocumentTypeComparer.PreviewDocumentTypeChanges();
        }

        /// <summary>
        /// Previews the template changes.
        /// </summary>
        /// <returns></returns>
        public static List<ContentComparison> PreviewTemplateChanges()
        {
            return new TemplateComparer().PreviewTemplateChanges();
        }

        /// <summary>
        /// Previews the data type changes.
        /// </summary>
        /// <returns></returns>
        public static List<ContentComparison> PreviewDataTypeChanges()
        {
            return new DataTypeComparer().PreviewDataTypeChanges();
        }

        /// <summary>
        /// Synchronizes all templates.
        /// </summary>
        public static void SynchronizeAllTemplates()
        {
            lock (SyncObj)
            {
                SynchronizeTemplates();
            }
        }

        /// <summary>
        /// Synchronizes all data types.
        /// </summary>
        public static void SynchronizeAllDataTypes()
        {
            lock (SyncObj)
            {
                SynchronizeDatatTypes();
            }
        }

        /// <summary>
        /// Cleans up document types.
        /// </summary>
        public static void CleanUpDocumentTypes()
        {
            DocumentTypeManager docTypeManager = new DocumentTypeManager();
            docTypeManager.CleanUpDocumentTypes();
        }

        /// <summary>
        /// Synchronizes all document types.
        /// </summary>
        public static void SynchronizeAllDocumentTypes()
        {
            lock (SyncObj)
            {
                SynchronizeDocumentTypes();

                // Register convertors only if any document or member types are found
                if (DocumentTypeManager.HasSynchronizedDocumentTypes())
                {
                    RegisterConvertors();
                }
            }
        }

        private static void SynchronizeTemplates()
        {
                TemplateManager templateManager = new TemplateManager();
                templateManager.Synchronize();
        }

        private static void SynchronizeDatatTypes()
		{
			var dataTypeManager = new DataTypeManager();
			dataTypeManager.Synchronize();
		}

        private static void SynchronizeDocumentTypes()
        {
            DocumentTypeManager docTypeManager = new DocumentTypeManager();
            docTypeManager.Synchronize();
        }

        private static void SynchronizeMemberTypes()
        {
            MemberTypeManager memberTypeManager = new MemberTypeManager();
            memberTypeManager.Synchronize();
        }

        private static void SynchronizeUserControls()
        {
            WebUserControlsManager userControlsManager = new WebUserControlsManager();
            userControlsManager.Synchronize();
        }

        private static void RegisterConvertors()
        {
            List<Type> convertorTypes = Util.GetFirstLevelSubTypes(typeof(ICustomTypeConvertor));

            foreach (Type convertorType in convertorTypes.Where(c => !c.IsGenericType))
            {
                ICustomTypeConvertor convertor = (ICustomTypeConvertor)Activator.CreateInstance(convertorType);

                ContentHelper.RegisterDocumentTypePropertyConvertor(convertor.ConvertType, convertor);
            }
        }
    }
}
