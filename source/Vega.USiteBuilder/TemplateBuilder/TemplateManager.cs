using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using umbraco.cms.businesslogic.template;
using Umbraco.Core;
using Umbraco.Core.IO;

namespace Vega.USiteBuilder.TemplateBuilder
{
    internal class TemplateManager : ManagerBase
    {
        public void Synchronize()
        {
            if (Util.DefaultRenderingEngine == RenderingEngine.WebForms)
            {
                SynchronizeTemplates(typeof(TemplateBase));
                UpdateTemplatesTreeStructure();
            }
            else
            {
                SynchronizeViews();
                UpdateViewsTreeStructure();
            }
        }

        public static string GetTemplateAlias(Type typeTemplate)
        { 
            return typeTemplate.Name;
        }

        public static string GetViewParent(string templateCode)
        {
            string parentMasterPageName = null;
            
            var match = Regex.Match(templateCode, @"Layout\s*\=\s*\""(.*)\""");
            if (match.Success)
            {
                parentMasterPageName = match.Groups[1].Value.Replace(".cshtml", "").Replace(".vbhtml", "");
            }

            return parentMasterPageName;
        }

        private void UpdateViewsTreeStructure()
        {
            List<Template> templates = Template.GetAllAsList();

            foreach (Template template in templates)
            {
                string parentMasterPageName = GetViewParent(template.Design);

                if (!string.IsNullOrEmpty(parentMasterPageName) && parentMasterPageName != "default")
                {
                    Template parentTemplate = Template.GetByAlias(parentMasterPageName);
                    if (parentTemplate == null)
                    {
                        throw new Exception(
                            string.Format(
                                "Template '{0}' is using '{1}' as a parent template (defined in MasterPageFile in {0}.master) but '{1}' template cannot be found",
                                template.Alias, parentMasterPageName));
                    }

                    template.MasterTemplate = parentTemplate.Id;
                }
            }
        }

        private void SynchronizeViews()
        {
            string viewsPath = HostingEnvironment.MapPath(SystemDirectories.MvcViews);
            if (viewsPath != null)
            {
                DirectoryInfo viewsFolder = new DirectoryInfo(viewsPath);

                foreach (var viewFile in viewsFolder.GetFiles("*.cshtml", SearchOption.TopDirectoryOnly))
                {
                    string alias = Path.GetFileNameWithoutExtension(viewFile.Name).Replace(" ", "");
                    Template template = Template.GetByAlias(alias);
                    if (template == null)
                    {
                        Template.MakeNew(alias, siteBuilderUser);
                    }
                }
            }
        }

        private void SynchronizeTemplates(Type typeBaseTemplate)
        {
            foreach (Type typeTemplate in Util.GetFirstLevelSubTypes(typeBaseTemplate))
            {
                if (!IsBaseTemplate(typeTemplate))
                {
                    SynchronizeTemplate(typeTemplate);
                }

                // sync all children templates
                SynchronizeTemplates(typeTemplate);
            }
        }

        private void SynchronizeTemplate(Type typeTemplate)
        {
            string alias = GetTemplateAlias(typeTemplate);
            try
            {
                AddToSynchronized(null, alias, typeTemplate);
            }
            catch (ArgumentException exc)
            {
                throw new Exception(string.Format("Template (masterpage) with alias '{0}' already exists! Please use unique masterpage names as masterpage name is used as alias. Masterpage causing the problem: '{1}' (assembly: '{2}'). Error message: {3}",
                    alias, typeTemplate.FullName, typeTemplate.Assembly.FullName, exc.Message));
            }

            Template template = Template.GetByAlias(alias);
            if (template == null)
            {
                Template.MakeNew(alias, siteBuilderUser);
            }
        }

        public bool IsBaseTemplate(Type typeTemplate)
        {
            bool retVal = typeTemplate == typeof(TemplateBase) || typeTemplate.IsGenericType || typeTemplate.Namespace == "ASP";

            return retVal;
        }

        private void UpdateTemplatesTreeStructure()
        {
            List<Template> templates = Template.GetAllAsList();

            foreach (Template template in templates)
            {
                string parentMasterPageName = GetParentMasterPageName(template);

                if (!string.IsNullOrEmpty(parentMasterPageName) && parentMasterPageName != "default")
                {
                    Template parentTemplate = Template.GetByAlias(parentMasterPageName);
                    if (parentTemplate == null)
                    {
                        throw new Exception(string.Format("Template '{0}' is using '{1}' as a parent template (defined in MasterPageFile in {0}.master) but '{1}' template cannot be found",
                            template.Alias, parentMasterPageName));
                    }
                    template.MasterTemplate = parentTemplate.Id;
                }
            }
        }

        public string GetParentMasterPageName(Template template)
        {
            string masterPageContent = File.ReadAllText(template.TemplateFilePath);
            return GetParentMasterPageName(masterPageContent);
        }

        public string GetParentMasterPageName(string masterPageContent)
        {
            string retVal = null;

            string masterHeader =
                masterPageContent.Substring(0, masterPageContent.IndexOf("%>") + 2).Trim(Environment.NewLine.ToCharArray());
            // find the masterpagefile attribute
            MatchCollection m = Regex.Matches(masterHeader, "(?<attributeName>\\S*)=\"(?<attributeValue>[^\"]*)\"",
                                              RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            foreach (Match attributeSet in m)
            {
                if (attributeSet.Groups["attributeName"].Value.ToLower() == "masterpagefile")
                {
                    // validate the masterpagefile
                    string masterPageFile = attributeSet.Groups["attributeValue"].Value;

                    int startIdx = masterPageFile.LastIndexOf("/", StringComparison.CurrentCultureIgnoreCase);
                    if (startIdx < 0)
                    {
                        startIdx = 0;
                    }
                    else
                    {
                        startIdx += 1; // so it won't include '/'
                    }

                    int endIdx = masterPageFile.LastIndexOf(".master", StringComparison.CurrentCultureIgnoreCase);

                    retVal = masterPageFile.Substring(startIdx, endIdx - startIdx);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Returns all templates which are using given document type
        /// </summary>
        public static List<Type> GetAllTemplates(Type typeDocType)
        {
            List<Type> retVal = new List<Type>();

            List<Type> allTemplates = Util.GetAllSubTypes(typeof(TemplateBase));
            foreach (Type typeTemplate in allTemplates)
            {
                if (Util.IsGenericArgumentTypeOf(typeTemplate, typeDocType))
                {
                    // try to get the attribute
                    TemplateAttribute templateAttr = Util.GetAttribute<TemplateAttribute>(typeTemplate);
                    if (templateAttr == null || templateAttr.AllowedForDocumentType)
                    {
                        retVal.Add(typeTemplate);
                    }
                }
            }

            return retVal;
        }
    }
}
