using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using umbraco.cms.businesslogic.template;
using Umbraco.Core;
using Umbraco.Core.IO;
using Vega.USiteBuilder.DocumentTypeBuilder;

namespace Vega.USiteBuilder.TemplateBuilder
{
    /// <summary>
    /// Template comparer utility class
    /// </summary>
    public class TemplateComparer
    {
        /// <summary>
        /// Previews the template changes.
        /// </summary>
        /// <returns></returns>
        public List<ContentComparison> PreviewTemplateChanges()
        {
            if (Util.DefaultRenderingEngine == RenderingEngine.WebForms)
            {
                return PreviewTemplates(typeof(TemplateBase));
            }
        
            return PreviewViews();
        }

        private static List<ContentComparison> PreviewViews()
        {
            List<ContentComparison> templateComparison = new List<ContentComparison>();

            string viewsPath = HostingEnvironment.MapPath(SystemDirectories.MvcViews);
            if (viewsPath != null)
            {
                DirectoryInfo viewsFolder = new DirectoryInfo(viewsPath);

                foreach (var viewFile in viewsFolder.GetFiles("*.cshtml", SearchOption.TopDirectoryOnly))
                {

                    string alias = Path.GetFileNameWithoutExtension(viewFile.Name).Replace(" ", "");
                    string path = IOHelper.MapPath(viewsFolder + "/" + alias.Replace(" ", "") + ".cshtml");
                    Template template = Template.GetByAlias(alias);
                    if (template == null)
                    {
                       
                        string parentAlias = TemplateManager.GetViewParent(File.ReadAllText(path));


                        templateComparison.Add(new ContentComparison
                                                   {
                                                       Alias = alias,
                                                       DocumentTypeStatus = Status.New,
                                                       ParentAlias = parentAlias ?? "default"
                                                   });
                    }
                    else
                    {
                        int parentTemplateId = 0;
                        string parentMasterPageName = TemplateManager.GetViewParent(File.ReadAllText(path));

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
                            parentTemplateId = parentTemplate.Id;
                        }
                        if (template.MasterTemplate != parentTemplateId)
                        {
                            templateComparison.Add(new ContentComparison
                                                       {
                                                           Alias = alias,
                                                           DocumentTypeStatus = Status.Changed,
                                                           ParentAlias = parentMasterPageName ?? "default"
                                                       });
                        }
                        else
                        {
                            templateComparison.Add(new ContentComparison
                                                       {
                                                           Alias = alias,
                                                           DocumentTypeStatus = Status.Same,
                                                           ParentAlias = parentMasterPageName ?? "default"
                                                       });
                        }
                    }
                }
            }

            return templateComparison;
        }

        /// <summary>
        /// Previews the templates.
        /// </summary>
        /// <param name="typeBaseTemplate">The type base template.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<ContentComparison> PreviewTemplates(Type typeBaseTemplate)
        {
            List<ContentComparison> templateComparison = new List<ContentComparison>();
            TemplateManager templateManager = new TemplateManager();

            foreach (Type typeTemplate in Util.GetFirstLevelSubTypes(typeBaseTemplate))
            {
                if (typeTemplate.IsGenericType)
                {
                    templateComparison.AddRange(PreviewTemplates(typeTemplate));
                    continue;
                }
                
                string alias = TemplateManager.GetTemplateAlias(typeTemplate);
                Template template = Template.GetByAlias(alias);
                if (template == null)
                {
                    string path =
                        IOHelper.MapPath(SystemDirectories.Masterpages + "/" + alias.Replace(" ", "") + ".master");

                    if (File.Exists(path))
                    {
                        string parentAlias = templateManager.GetParentMasterPageName(File.ReadAllText(path));

                        templateComparison.Add(new ContentComparison
                                                   {
                                                       Alias = alias,
                                                       DocumentTypeStatus = Status.New,
                                                       ParentAlias = parentAlias ?? ""
                                                   });
                    }
                }
                else
                {
                    int parentTemplateId = 0;
                    string parentMasterPageName = templateManager.GetParentMasterPageName(template);
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
                        parentTemplateId = parentTemplate.Id;
                    }
                    if (template.MasterTemplate != parentTemplateId)
                    {
                        templateComparison.Add(new ContentComparison { Alias = alias, DocumentTypeStatus = Status.Changed, ParentAlias = parentMasterPageName ?? "default" });

                    }
                    else
                    {
                        templateComparison.Add(new ContentComparison { Alias = alias, DocumentTypeStatus = Status.Same, ParentAlias = parentMasterPageName ?? "default" });
                    }

                }

                templateComparison.AddRange(PreviewTemplates(typeTemplate));
            }

            return templateComparison;
        }
    }
}
