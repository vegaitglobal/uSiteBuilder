
using Vega.USiteBuilder.DataTypeBuilder;
using Vega.USiteBuilder.DocumentTypeBuilder;
using Vega.USiteBuilder.TemplateBuilder;

namespace Vega.USiteBuilder
{
    using System;
    using System.Collections.Generic;
    using Types;
    using System.Diagnostics;

    /// <summary>
    /// Main class for updating entities defined via uSiteBuilder
    /// </summary>
    public class UmbracoManager
    {
        private static string _syncObj = "sync";
        private static bool _synchronized = false;

        /// <summary>
        /// Entry point for updating entities defined via uSiteBuilder
        /// </summary>
        public static void SynchronizeIfNotSynchronized()
        {
            // avoid immediate locking because it will impact performance
            if (!_synchronized)
            {   
                lock (_syncObj)
                {
                    if (!_synchronized)
                    {
                        if (AreSystemRequirementsOK())
                        {
                            Synchronize();
                        }
                        _synchronized = true;
                    }
                }
            }
        }

        private static bool AreSystemRequirementsOK()
        {
#if DEBUG
            Stopwatch timer = new Stopwatch();
            timer.Start();
#endif
            bool result = true;

            // Check if Umbraco setup is finished
            if (Util.GetAdminUser() == null)
            {
                result = false;
            }

            // Check if Umbraco version is greater than or equal to 6.1.6
            if (Util.IsUmbraco616OrHigher() == false)
            {
                throw new Exception("Current umbraco version is not supported. Only v6.1.6 and above are supported.");
            }


#if DEBUG
            timer.Stop();
            StopwatchLogger.AddToLog(string.Format("Total elapsed time for method 'AreSystemRequirementsOK': {0}ms.", timer.ElapsedMilliseconds));
            timer.Restart();
#endif
            return result;
        }

        private static void Synchronize()
        {
            if (!Configuration.USiteBuilderConfiguration.SuppressSynchronization)
            {
                SynchronizeDataTypes();
                SynchronizeTemplates();
                SynchronizeDocumentTypes();
                SynchronizeMemberTypes();
                SynchronizeUserControls();
            }
            
            //// NOTE: BJ: This was commented out in 1.2.5 version
            ////SynchronizeRazorMacros();

            RegisterConvertors();
        }

        public static List<ContentComparison> PreviewDocumentTypeChanges()
        {
            bool hasDefaultValues;
            return DocumentTypeComparer.PreviewDocumentTypeChanges(out hasDefaultValues);
        }

        public static List<ContentComparison> PreviewTemplateChanges()
        {
            return new TemplateComparer().PreviewTemplateChanges();
        }

        public static List<ContentComparison> PreviewDataTypeChanges()
        {
            return new DataTypeComparer().PreviewDataTypeChanges();
        }

        public static void SynchronizeTemplates()
        {
#if DEBUG
            Stopwatch timer = new Stopwatch();
            timer.Start();
#endif

            TemplateManager templateManager = new TemplateManager();
            templateManager.Synchronize();

#if DEBUG
            timer.Stop();
            StopwatchLogger.AddToLog(string.Format("Total elapsed time for method 'SynchronizeTemplates': {0}ms.", timer.ElapsedMilliseconds));
            timer.Restart();
#endif
        }

        public static void SynchronizeDataTypes()
        {
#if DEBUG
            Stopwatch timer = new Stopwatch();
            timer.Start();
#endif

            var dataTypeManager = new DataTypeManager();
            dataTypeManager.Synchronize();

#if DEBUG
            timer.Stop();
            StopwatchLogger.AddToLog(string.Format("Total elapsed time for method 'SynchronizeDataTypes': {0}ms.", timer.ElapsedMilliseconds));
            timer.Restart();
#endif        
        }

        public static void SynchronizeDocumentTypes()
        {
#if DEBUG
            Stopwatch timer = new Stopwatch();
            timer.Start();
#endif
            DocumentTypeManager docTypeManager = new DocumentTypeManager();
            docTypeManager.Synchronize();

#if DEBUG
            timer.Stop();
            StopwatchLogger.AddToLog(string.Format("Total elapsed time for method 'SynchronizeDocumentTypes': {0}ms.", timer.ElapsedMilliseconds));
            timer.Restart();
#endif
        }

        public static void SynchronizeMemberTypes()
        {
#if DEBUG
            Stopwatch timer = new Stopwatch();
            timer.Start();
#endif
            MemberTypeManager memberTypeManager = new MemberTypeManager();
            memberTypeManager.Synchronize();

#if DEBUG
            timer.Stop();
            StopwatchLogger.AddToLog(string.Format("Total elapsed time for method 'SynchronizeMemberTypes': {0}ms.", timer.ElapsedMilliseconds));
            timer.Restart();
#endif        
        }

        public static void SynchronizeUserControls()
        {
#if DEBUG
            Stopwatch timer = new Stopwatch();
            timer.Start();
#endif
            WebUserControlsManager userControlsManager = new WebUserControlsManager();
            userControlsManager.Synchronize();

#if DEBUG
            timer.Stop();
            StopwatchLogger.AddToLog(string.Format("Total elapsed time for method 'SynchronizeUserControls': {0}ms.", timer.ElapsedMilliseconds));
            timer.Restart();
#endif
        }

        //private static void SynchronizeRazorMacros()
        //{
        //    RazorManager razorManager = new RazorManager();
        //    razorManager.Synchronize();
        //}


        private static void RegisterConvertors()
        {
            List<Type> convertorTypes = Util.GetFirstLevelSubTypes(typeof(ICustomTypeConvertor));

            foreach (Type convertorType in convertorTypes)
            {
                try
                {
                    ICustomTypeConvertor convertor = (ICustomTypeConvertor)Types.Activation.Activator.Current.GetInstance(convertorType);
                    if (convertor != null)
                    {
                        ContentHelper.RegisterDocumentTypePropertyConvertor(convertor.ConvertType, convertor);
                    }
                }
                catch
                {
                    // If an error occurrs while instantiating the convertor, we just skip it
                }
            }
        }


        /// <summary>
        /// Deletes single document type from Umbraco
        /// </summary>
        /// <param name="alias"></param>
        public static void DeleteDocumentType(string alias)
        {
            DocumentTypeManager.DeleteDocumentType(alias);
        }

        /// <summary>
        ///  Removes all Umbraco document types that are not defined using uSiteBuilder
        /// </summary>
        public static void CleanUpDocumentTypes()
        {
            DocumentTypeManager.CleanUpDocumentTypes();
        }


    }
}
