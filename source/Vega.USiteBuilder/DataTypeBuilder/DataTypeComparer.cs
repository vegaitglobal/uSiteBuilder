using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using umbraco.cms.businesslogic.datatype;

namespace Vega.USiteBuilder.DataTypeBuilder
{
    internal class DataTypeComparer
    {
        public List<ContentComparison> PreviewDataTypeChanges()
        {
            List<ContentComparison> comparison = new List<ContentComparison>();

            var existingDataTypes = DataTypeDefinition.GetAll();

            List<Type> firstLevelSubTypes = Util.GetFirstLevelSubTypes(typeof (DataTypeBase));
            foreach (Type typeDataType in firstLevelSubTypes)
            {
                var dataTypeAttr = DataTypeManager.GetDataTypeAttribute(typeDataType);
                var dtd = existingDataTypes.FirstOrDefault(d => d.Text == dataTypeAttr.Name || (!string.IsNullOrEmpty(dataTypeAttr.UniqueId) && d.UniqueId == new Guid(dataTypeAttr.UniqueId)));
                if (dtd == null)
                {
                    comparison.Add(new ContentComparison { Alias = dataTypeAttr.Name, ParentAlias = "", DocumentTypeStatus = Status.New });
                }
                else
                {
                    if (!CompareProperties(typeDataType, dtd))
                    {
                        comparison.Add(new ContentComparison { Alias = dataTypeAttr.Name, ParentAlias = "", DocumentTypeStatus = Status.Changed });
                    }
                    else
                    {
                        comparison.Add(new ContentComparison { Alias = dataTypeAttr.Name, ParentAlias = "", DocumentTypeStatus = Status.Same });
                    }
                }
            }

            // TODO: add deleted / not present?

            return comparison;
        }

        private bool CompareProperties(Type dataType, DataTypeDefinition existingDataType)
        {
            var dataTypeAttr = DataTypeManager.GetDataTypeAttribute(dataType);
            if (existingDataType.Text != dataTypeAttr.Name)
            {
                return false;
            }
            if (existingDataType.DataType == null || existingDataType.DataType.Id != new Guid(dataTypeAttr.RenderControlGuid))
            {
                return false;
            }

            var instance = Activator.CreateInstance(dataType, null) as DataTypeBase;
            DataTypePrevalue[] prevalues = instance.Prevalues;

            var settingsStorage = new DataEditorSettingsStorage();
            var existingSettings = settingsStorage.GetSettings(existingDataType.Id);

            if (existingSettings.Count != prevalues.Count())
            {
                return false;
            }

            int counter = 0;
            foreach (var setting in existingSettings)
            {
                if (setting.Key != prevalues[counter].Alias || setting.Value != prevalues[counter].Value)
                {
                    return false;
                }

                counter++;
            }

            return true;
        }
    }
}
