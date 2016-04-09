using System;
using System.Linq;
using umbraco.cms.businesslogic.macro;

namespace Vega.USiteBuilder.MacroBuilder
{
    internal class MacroManager : ManagerBase
    {
        public void Synchronize()
        {
            foreach (Type type in Util.GetAllSubTypes(typeof(MacroBase)))
            {
                if (!type.IsAbstract)
                {
                    SynchronizeMacro(type);
                }
            }
        }

        private void SynchronizeMacro(Type typeMacro)
        {
            MacroBase macroDefinition = Activator.CreateInstance(typeMacro, null) as MacroBase;

            if (macroDefinition == null) return;

            // add try/catch
            AddToSynchronized(macroDefinition.MacroName, typeMacro.Name, typeMacro);

            Macro macro = Macro.GetByAlias(typeMacro.Name);
            if (macro == null || macro.Id == 0)
            {
                macro = Macro.MakeNew(macroDefinition.MacroName);
            }

            macro.Alias = typeMacro.Name;
            macro.Name = macroDefinition.MacroName;
            macro.CacheByPage = macroDefinition.CacheByPage;
            macro.CachePersonalized = macroDefinition.CachePersonalized;
            macro.RefreshRate = macroDefinition.CachePeriod;
            macro.RenderContent = macroDefinition.RenderContentInEditor;
            macro.UseInEditor = macroDefinition.UseInEditor;            

            if (typeMacro.IsSubclassOf(typeof(XsltFileMacroBase)))
            {
                macro.Xslt = ((XsltFileMacroBase)macroDefinition).XsltFileName;
            }
            else if (typeMacro.IsSubclassOf(typeof(UserControlMacroBase)))
            {
                macro.Type = ((UserControlMacroBase)macroDefinition).Control;
            }
            else if (typeMacro.IsSubclassOf(typeof(CustomControlMacroBase)))
            {
                macro.Assembly = ((CustomControlMacroBase)macroDefinition).Assembly;
                macro.Type = ((CustomControlMacroBase)macroDefinition).Type;
            }
            else if (typeMacro.IsSubclassOf(typeof(PythonFileMacroBase)))
            {
                macro.Type = ((PythonFileMacroBase)macroDefinition).PythonFile;
            }

            macro.Save();

            SynchronizeMacroProperties(macroDefinition, macro);
        }

        private void SynchronizeMacroProperties(MacroBase macroDefinition, Macro macro)
        {
            if (macroDefinition.Parameters != null && macroDefinition.Parameters.Length > 0)
            {
                int sortOrder = 0;
                foreach (MacroParameter parameter in macroDefinition.Parameters)
                {
                    MacroPropertyType type = MacroPropertyType.GetAll.First(mpt => mpt.Alias.ToLower() == parameter.Type.ToString().ToLower());

                    MacroProperty macroProperty = MacroProperty.GetProperties(macro.Id).FirstOrDefault(mp => mp.Alias == parameter.Alias) ??
                                                  MacroProperty.MakeNew(macro, parameter.Show, parameter.Alias, parameter.Name, type);

                    macroProperty.Name = parameter.Name;
                    macroProperty.Type = type;
                    macroProperty.SortOrder = sortOrder;
                    
                    sortOrder++;

                    macroProperty.Save();
                }

                // remove all unused properties (select all properties which do not exists in macroDefinition.Parameters)
                foreach (MacroProperty macroProperty in macro.Properties.Where(mp => macroDefinition.Parameters.FirstOrDefault(mdp => mdp.Alias == mp.Alias) == null))
                {
                    macroProperty.Delete();
                }

                macro.RefreshProperties();
            }
            else
            {
                // remove all properties
                if (macro.Properties != null && macro.Properties.Length > 0)
                {
                    foreach (MacroProperty mp in macro.Properties)
                    {
                        mp.Delete();
                    }

                    macro.RefreshProperties();
                }
            }
        }
    }
}
