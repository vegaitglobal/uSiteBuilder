namespace Vega.USiteBuilder
{
    using System;
    using System.Configuration;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Configuration;

    /// <summary>
    /// Class used to manipulate current website's web.config
    /// </summary>
    internal class WebConfigManager
    {
        public static void AddHttpModule(string name, string type)
        {
            try
            {
                // Set the configPath value to the path for your target Web site.
                string configPath = "/";

                // Get the configuration object.
                System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration(configPath);

                // Call ConfigureHttpModules subroutine.
                AddWebServerModules(config, name, type);
                AddWebModules(config, name, type);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Cannot change web.config automatically! Error: {0}. Please add the following httpmodule: <add name=\"USiteBuilderHttpModule\" type=\"Vega.USiteBuilder.USiteBuilderHttpModule, Vega.USiteBuilder\" />",
                    e.Message));
            }
        }

        private static void AddWebServerModules(System.Configuration.Configuration config, string name, string type)
        {
            IgnoreSection ignoreSection = (IgnoreSection)config.GetSection("system.webServer");
            string xml = ignoreSection.SectionInformation.GetRawXml();
            bool isChanged = false;
            if (xml == null)
            {
                string temp = String.Format("<system.webServer><modules><add name=\"{0}\" type=\"{1}\" /></modules></system.webServer>", name, type);
                xml = temp;
                isChanged = true;
            }
            else
            {
                xml = Regex.Replace(xml, "<modules\\s*/>", "<modules/>");
                xml = Regex.Replace(xml, "</modules\\s*>", "</modules>");
                xml = Regex.Replace(xml, "<modules\\s*>", "<modules>");
                xml = Regex.Replace(xml, "<system.webServer\\s*>", "<system.webServer>");
                xml = Regex.Replace(xml, "<system.webServer\\s*/>", "<system.webServer/>");
                xml = Regex.Replace(xml, "</system.webServer\\s*>", "</system.webServer>");

                if (xml.Contains("<modules/>"))
                {
                    string temp = String.Format("<modules><add name=\"{0}\" type=\"{1}\" /></modules>", name, type);
                    xml = xml.Replace("<modules/>", temp.ToString());
                    isChanged = true;
                }
                else if (xml.Contains("<modules"))
                {
                    string temp = String.Format("<add name=\"{0}\" type=\"{1}\"", name, type);
                    if (!xml.Contains(temp))
                    {
                        temp += "/>";
                        int position = xml.IndexOf("</modules>");
                        StringBuilder tempXml = new StringBuilder(xml);
                        tempXml.Insert(position, temp, 1);
                        xml = tempXml.ToString();
                        isChanged = true;
                    }
                }
                else if (xml.Contains("<system.webServer/>"))
                {
                    string temp = String.Format("<system.webServer><modules><add name=\"{0}\" type=\"{1}\" /></modules></system.webServer>", name, type);
                    xml = xml.Replace("<system.webServer/>", temp);
                    isChanged = true;
                }
                else if (xml.Contains("<system.webServer"))
                {
                    string temp = String.Format("<modules><add name=\"{0}\" type=\"{1}\" /></modules>", name, type);
                    int position = xml.IndexOf("</system.webServer>");
                    StringBuilder tempXml = new StringBuilder(xml);
                    tempXml.Insert(position, temp, 1);
                    xml = tempXml.ToString();
                    isChanged = true;
                }
            }

            if (isChanged)
            {
                ignoreSection.SectionInformation.SetRawXml(xml);
                config.Save();
            }
        }

        private static void AddWebModules(System.Configuration.Configuration config, string name, string type)
        {
            // Get the <httpModules> section.
            HttpModulesSection sectionSystemWeb = (HttpModulesSection)config.GetSection("system.web/httpModules");

            // Create a new module action object.
            HttpModuleAction myHttpModuleAction = new HttpModuleAction(name, type);

            int indexOfHttpModule = sectionSystemWeb.Modules.IndexOf(myHttpModuleAction);
            if (indexOfHttpModule == -1)
            {
                sectionSystemWeb.Modules.Add(myHttpModuleAction);
                if (!sectionSystemWeb.SectionInformation.IsLocked)
                {
                    config.Save();
                }
            }
        }
    }
}
