
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using umbraco.cms.businesslogic.datatype;
using umbraco.DataLayer;
using Umbraco.Core.Services;
using ApplicationContext = Umbraco.Core.ApplicationContext;
using DataTypeDefinition = Umbraco.Core.Models.DataTypeDefinition;
using PreValue = Umbraco.Core.Models.PreValue;


namespace Vega.USiteBuilder.DataTypeBuilder
{
    /// <summary>
	/// Manages DataType synchronization
	/// </summary>
	internal class DataTypeManager : ManagerBase
	{
		public DataTypeManager()
		{
            DataLayerHelper.CreateSqlHelper(ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ConnectionString);
		}

        protected IDataTypeService DataTypeService
        {
            get { return ApplicationContext.Current.Services.DataTypeService; }
        }

		/// <summary>
		/// Synchronizes this instance.
		/// </summary>
		public void Synchronize()
		{
			SynchronizeDataTypes();
		}

        private IDictionary<string, PreValue> GetEditorSettings(Type typeDataType)
        {
            var instance = (DataTypeBase)Types.Activation.Activator.Current.GetInstance(typeDataType);

            DataTypePrevalue[] prevalues = instance.Prevalues;

            if (prevalues == null)
            {
                return new Dictionary<string, PreValue>();
            }

            var result = prevalues.ToDictionary(x => x.Alias, x => new PreValue(x.SortOrder, x.Value));
            return result;
        }

        private void SynchronizeDataTypes()
        {
            foreach (Type typeDataType in Util.GetFirstLevelSubTypes(typeof(DataTypeBase)))
            {
                var dataTypeAttr = GetDataTypeAttribute(typeDataType);

                try
                {
                    this.AddToSynchronized(null, dataTypeAttr.Name, typeDataType);
                }
                catch (ArgumentException exc)
                {
                    throw new Exception(
                        string.Format(
                            "DataType with name '{0}' already exists! Please use unique DataType names. DataType causing the problem: '{1}' (assembly: '{2}'). Error message: {3}",
                            dataTypeAttr.Name,
                            typeDataType.FullName,
                            typeDataType.Assembly.FullName,
                            exc.Message));
                }

                // Old code that caused the error.
                //// var dtd = DataTypeService.GetDataTypeDefinitionByPropertyEditorAlias(dataTypeAttr.PropertyEditorAlias).FirstOrDefault();

                var dtd = this.DataTypeService.GetAllDataTypeDefinitions().FirstOrDefault(d => d.Name == dataTypeAttr.Name || (!string.IsNullOrEmpty(dataTypeAttr.UniqueId) && d.Key == new Guid(dataTypeAttr.UniqueId)));

                // If there are no datatypes with name already we can go ahead and create one
                if (dtd == null)
                {
                    var newDataTypeDefinition = new DataTypeDefinition(dataTypeAttr.ParentId, dataTypeAttr.RenderControlGuid)
                    {
                        Name = dataTypeAttr.Name,
                        DatabaseType = dataTypeAttr.DataTypeDatabaseType
                    };

                    if (!string.IsNullOrEmpty(dataTypeAttr.UniqueId))
                    {
                        newDataTypeDefinition.Key = new Guid(dataTypeAttr.UniqueId);
                    }

                    var codeFirstSettings = GetEditorSettings(typeDataType);

                    if (codeFirstSettings.Any())
                    {
                        DataTypeService.SaveDataTypeAndPreValues(newDataTypeDefinition, codeFirstSettings);
                    }
                    else
                    {
                        DataTypeService.Save(newDataTypeDefinition);
                    }
                }
                // data type definition already present
                else
                {
                    // var settings = GetEditorSettings(typeDataType);

                    var existingSettings = DataTypeService.GetPreValuesCollectionByDataTypeId(dtd.Id);

                    if (existingSettings == null || existingSettings.IsDictionaryBased ? !existingSettings.PreValuesAsDictionary.Any() : !existingSettings.PreValuesAsArray.Any())
                    {
                        var codeFirstSettings = GetEditorSettings(typeDataType);

                        if (codeFirstSettings.Any())
                        {
                            DataTypeService.SavePreValues(dtd.Id, codeFirstSettings);
                        }
                    }
                }
            }
        }

		/// <summary>
		/// Get's the document type attribute or returns attribute with default values if attribute is not found
		/// </summary>
		/// <param name="typeDataType">An document type type</param>
		/// <returns>The DataTypeAttribute for the DataType</returns>
		internal static DataTypeAttribute GetDataTypeAttribute(Type typeDataType)
		{
			var retVal = Util.GetAttribute<DataTypeAttribute>(typeDataType) ?? CreateDefaultDataTypeAttribute(typeDataType);

			return retVal;
		}

		/// <summary>
		/// Creates the default data type attribute.
		/// </summary>
		/// <param name="typeDataType">Type of the type data.</param>
		/// <returns>The DataTypeAttribute for the DataType</returns>
		private static DataTypeAttribute CreateDefaultDataTypeAttribute(Type typeDataType)
		{
			var retVal = new DataTypeAttribute
				{
					Name = typeDataType.Name,
					UniqueId = Guid.NewGuid().ToString(),
					RenderControlName = "Textbox",
					DatabaseDataType = DBTypes.Nvarchar
				};

			return retVal;
		}
	}
}
