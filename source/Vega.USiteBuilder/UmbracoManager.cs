using System.Linq;
using Vega.USiteBuilder.DataTypeBuilder;
using Vega.USiteBuilder.DocumentTypeBuilder;
using Vega.USiteBuilder.TemplateBuilder;

namespace Vega.USiteBuilder
{
    using System;
    using Vega.USiteBuilder;
    using System.Collections.Generic;
    using Vega.USiteBuilder.Types;

    public class UmbracoManager
    {
        private static string _syncObj = "sync";
        private static bool _synchronized = false;

        public static void SynchronizeIfNotSynchronized()
        {
            // we are not locking immediatly because it will impact performance
            if (!_synchronized)
            {
                lock (_syncObj)
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
            if (Configuration.USiteBuilderConfiguration.SuppressSynchronization)
                return;

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

        public void SynchronizeDocumentType(Type siteBuilderType)
        {
            new DocumentTypeManager().SynchronizeDocumentType(siteBuilderType);
        }

        public void DeleteDocumentType(string alias)
        {
            new DocumentTypeManager().DeleteDocumentType(alias);
        }

        public static List<ContentComparison> PreviewDocumentTypeChanges()
        {
            return DocumentTypeComparer.PreviewDocumentTypeChanges();
        }

        public static List<ContentComparison> PreviewTemplateChanges()
        {
            return new TemplateComparer().PreviewTemplateChanges();
        }

        public static List<ContentComparison> PreviewDataTypeChanges()
        {
            return new DataTypeComparer().PreviewDataTypeChanges();
        }

        public static void SynchronizeAllTemplates()
        {
            lock (_syncObj)
            {
                SynchronizeTemplates();
            }
        }

        public static void SynchronizeAllDataTypes()
        {
            lock (_syncObj)
            {
                SynchronizeDatatTypes();
            }
        }

        public static void CleanUpDocumentTypes()
        {
            DocumentTypeManager docTypeManager = new DocumentTypeManager();
            docTypeManager.CleanUpDocumentTypes();
        }

        public static void SynchronizeAllDocumentTypes()
        {
            lock (_syncObj)
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

        private static void SynchronizeRazorMacros()
        {
            RazorManager razorManager = new RazorManager();
            razorManager.Synchronize();
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
