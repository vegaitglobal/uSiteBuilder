using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using umbraco.cms.businesslogic.macro;
using Vega.USiteBuilder.Configuration;
using Vega.USiteBuilder.MacroBuilder;

namespace Vega.USiteBuilder.Razor
{
    internal class RazorManager : ManagerBase
    {
        private const string TagMacroStart = "Macro={";

        public void Synchronize()
        {
            foreach (string path in Directory.GetFiles(Constants.MacroScriptsDirectory, "*.cshtml", SearchOption.AllDirectories))
            {                
                string cshtml = File.ReadAllText(path);

                // remove not needed characters to make parsing easier
                cshtml = cshtml.Replace(" ", "");
                cshtml = cshtml.Replace("\n", "");
                cshtml = cshtml.Replace("\r", "");

                AddMacroIfDefined(cshtml, path);
            }            
        }

        private void AddMacroIfDefined(string cshtml, string path)
        {
            int startIndex = cshtml.IndexOf(TagMacroStart);
            if (startIndex >= 0)
            {
                startIndex += TagMacroStart.Length;
                int endIndex = 0;
                
                int openBracket = 1;
                for (int i = startIndex; i < cshtml.Length; i++)
                {
                    if (cshtml[i] == '}')
                    {
                        openBracket--;
                    }
                    else if (cshtml[i] == '{')
                    {
                        openBracket++;
                    }

                    if (openBracket == 0)
                    {
                        endIndex = i;
                        break;
                    }
                } // for

                if (endIndex == 0)
                {
                    throw new Exception(String.Format("Macro not defined correctly (brackets mismatch) in file '{0}'", path));
                }

                string json = cshtml.Substring(startIndex, endIndex - startIndex);

                RazorMacro macro = Util.JsonDeserialize<RazorMacro>(json);
                macro.ScriptFilePath = path;

                AddOrUpdateMacro(macro);
            }
        }

        private void AddOrUpdateMacro(RazorMacro razorMacro)
        {
            try
            {
                AddToSynchronized(razorMacro.Name, razorMacro.Alias, razorMacro.GetType());
            }
            catch (ArgumentException exc)
            {
                throw new Exception(string.Format("Macro with alias '{0}' already exists! Please use unique razor macro names. Macro causing the problem: '{1}' (path: '{2}'). Error message: {3}",
                    razorMacro.Alias, razorMacro.Name, razorMacro.ScriptFilePath, exc.Message));
            }

            if (!USiteBuilderConfiguration.SuppressSynchronization)
            {
                Macro macro = new Macro(razorMacro.Alias);
                if (macro == null || macro.Id == 0)
                {
                    macro = Macro.MakeNew(razorMacro.Name);
                }

                macro.Alias = razorMacro.Alias;
                macro.Name = razorMacro.Name;
                macro.Type = string.Format("/{0}/{1}", Constants.UserControlsDirectory, razorMacro.ScriptFilePath);
                macro.UseInEditor = razorMacro.UseInEditor;
                macro.RenderContent = razorMacro.RenderContentInEditor;
                macro.RefreshRate = razorMacro.CachePeriod;
                macro.CacheByPage = razorMacro.CacheByPage;
                macro.CachePersonalized = razorMacro.CachePersonalized;

                // Add parameters
                if (razorMacro.Parameters != null && razorMacro.Parameters.Length > 0)
                {
                    // first delete all macro parameters
                    macro.Properties.ToList().ForEach(mp => mp.Delete());

                    List<MacroPropertyType> allPropertyTypes = MacroPropertyType.GetAll;

                    foreach (MacroParameter parameter in razorMacro.Parameters)
                    {
                        MacroPropertyType macroPropertyType = allPropertyTypes.FirstOrDefault(pt => pt.Type.Equals(parameter.Type, StringComparison.OrdinalIgnoreCase));
                        if (macroPropertyType == null || macroPropertyType.Id == 0)
                        {
                            string macroTypes = String.Empty;
                            allPropertyTypes.ForEach(pt => macroTypes += pt.Alias + ",");

                            throw new Exception(String.Format("Macro parameter type '{0}' not found. Parameter name: '{1}', Script: '{2}'. The following macro types are available: '{3}'",
                                parameter.Type, parameter.Name, razorMacro.ScriptFilePath, macroTypes));
                        }

                        MacroProperty.MakeNew(macro, parameter.Show, parameter.Alias, parameter.Name, macroPropertyType);
                    }
                }
            }
        }
    }
}
