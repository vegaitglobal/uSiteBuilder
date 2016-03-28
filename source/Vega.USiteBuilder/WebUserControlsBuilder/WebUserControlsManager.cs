namespace Vega.USiteBuilder
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;

    using umbraco.cms.businesslogic.macro;
    using System.IO;

    internal class WebUserControlsManager : ManagerBase
    {
        public void Synchronize()
        {
            SynchronizeUserControls(typeof(WebUserControlBase));
        }

        private void SynchronizeUserControls(Type typeBaseUserControl)
        {
            foreach (Type typeUserControl in Util.GetFirstLevelSubTypes(typeBaseUserControl))
            {
                if (!this.IsBaseUserControl(typeUserControl))
                {
                    this.SynchronizeUserControl(typeUserControl);
                }

                // sync all children user control
                this.SynchronizeUserControls(typeUserControl);
            }
        }

        private bool IsBaseUserControl(Type typeUserControl)
        {
            bool retVal = false;

            if (typeUserControl == typeof(WebUserControlBase) || typeUserControl.IsGenericType)
            {
                retVal = true;
            }

            return retVal;
        }

        /// <summary>
        /// Synchronization of user control means only creating an macro if
        /// control has appropriate attribute (MacroAttribute)
        /// </summary>
        /// <param name="typeUserControl"></param>
        private void SynchronizeUserControl(Type typeUserControl)
        {
            MacroAttribute macroAttr = Util.GetAttribute<MacroAttribute>(typeUserControl);
            if (macroAttr == null)
            {
                // if macro attribute is not specified, this control do not require macro.
                return;
            }

            string macroName = string.IsNullOrEmpty(macroAttr.Name) ? typeUserControl.Name : macroAttr.Name;
            string macroAlias = typeUserControl.Name;

            try
            {
                this.AddToSynchronized(macroName, macroAlias, typeUserControl);
            }
            catch (ArgumentException exc)
            {
                throw new Exception(string.Format("Macro with alias '{0}' already exists! Please use unique user control names as user control name is used as macro alias. User control causing the problem: '{1}' (assembly: '{2}'). Error message: {3}",
                    macroAlias, typeUserControl.FullName, typeUserControl.Assembly.FullName, exc.Message));
            }

            if (!Configuration.USiteBuilderConfiguration.SuppressSynchronization)
            {
                Macro macro = new Macro(macroAlias);
                if (macro == null || macro.Id == 0) // Check id=0 because there is a bug in umbraco that returns Macro object even if there's no macro with given alias in the database.
                {
                    macro = Macro.MakeNew(macroName);
                }

                macro.Alias = macroAlias;
                macro.Name = macroName;
                macro.Type = string.Format("/{0}/{1}.ascx", Constants.UserControlsDirectory, typeUserControl.Name);
                macro.UseInEditor = macroAttr.UseInEditor;
                macro.RenderContent = macroAttr.RenderContentInEditor;
                macro.RefreshRate = macroAttr.CachePeriod;
                macro.CacheByPage = macroAttr.CacheByPage;
                macro.CachePersonalized = macroAttr.CachePersonalized;

                this.SynchronizeMacroProperties(typeUserControl, macro);
            }
        }

        private void SynchronizeMacroProperties(Type typeUserControl, Macro macro)
        {
            // first delete all macro properties
            macro.Properties.ToList().ForEach(mp => mp.Delete());

            List<MacroPropertyType> allPropertyTypes = MacroPropertyType.GetAll;

            // foreach property marked with MacroParameter create an macro parameter
            foreach (PropertyInfo propInfo in typeUserControl.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                MacroParameterAttribute macroParamAttr = Util.GetAttribute<MacroParameterAttribute>(propInfo);

                if (macroParamAttr == null)
                {
                    // if property is not given attribute MacroParameterAttribute try to create default one based on property type
                    macroParamAttr = this.GetDefaultMacroParameterAttribute(propInfo);
                    if (macroParamAttr == null)
                    {
                        // if default attribute cannot be created (e.g. if property type is DateTime)
                        // skip creating this macro parameter
                        continue;
                    }
                }

                string macroParamName = string.IsNullOrEmpty(macroParamAttr.Name) ? propInfo.Name : macroParamAttr.Name;
                string macroParamAlias = propInfo.Name;

                MacroProperty.MakeNew(macro, macroParamAttr.Show, macroParamAlias, macroParamName,
                    allPropertyTypes.First(mpt => mpt.Alias.ToLower() == macroParamAttr.Type.ToString().ToLower()));
            }
        }

        private MacroParameterAttribute GetDefaultMacroParameterAttribute(PropertyInfo controlPropertyInfo)
        {
            MacroParameterType? paramType;
            TypeCode typeCode = Type.GetTypeCode(controlPropertyInfo.PropertyType);

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    paramType = MacroParameterType.Bool;
                    break;
                case TypeCode.String:
                    paramType = MacroParameterType.Text;
                    break;
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Single:
                    paramType = MacroParameterType.Number;
                    break;
                default:
                    paramType = null; // unknown
                    break;
            }

            if (paramType.HasValue)
            {
                return new MacroParameterAttribute()
                {
                    Name = controlPropertyInfo.Name,
                    Show = false,
                    Type = (MacroParameterType)paramType
                };
            }
            else
            {
                return null;
            }
        }
    }
}
