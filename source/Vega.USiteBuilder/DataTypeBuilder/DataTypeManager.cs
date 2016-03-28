using System;
using System.Linq;
using System.Web;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.datatype.controls;
using umbraco.DataLayer;

namespace Vega.USiteBuilder.DataTypeBuilder
{
    /// <summary>
	/// Manages DataType synchronization
	/// </summary>
	internal class DataTypeManager : ManagerBase
	{
		public DataTypeManager()
		{
			DataLayerHelper.CreateSqlHelper(GlobalSettings.DbDSN);
		}

		/// <summary>
		/// Synchronizes this instance.
		/// </summary>
		public void Synchronize()
		{
			SynchronizeDataTypes();
		}

		private void SynchronizeDataTypes()
		{
			var factory = new Factory();

			foreach (Type typeDataType in Util.GetFirstLevelSubTypes(typeof(DataTypeBase)))
			{
				var dataTypeAttr = GetDataTypeAttribute(typeDataType);

				try
				{
					AddToSynchronized(null, dataTypeAttr.Name, typeDataType);
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

                var dtd = DataTypeDefinition.GetAll().FirstOrDefault(d => d.Text == dataTypeAttr.Name || (!string.IsNullOrEmpty(dataTypeAttr.UniqueId) && d.UniqueId == new Guid(dataTypeAttr.UniqueId)));

				// If there are no datatypes with name already we can go ahead and create one
                if (dtd == null)
                {
                    if (!string.IsNullOrEmpty(dataTypeAttr.UniqueId))
                    {
                        dtd = DataTypeDefinition.MakeNew(User.GetUser(0), dataTypeAttr.Name,
                                                         new Guid(dataTypeAttr.UniqueId));
                    }
                    else
                    {
                        dtd = DataTypeDefinition.MakeNew(User.GetUser(0), dataTypeAttr.Name);
                    }
                }

			    dtd.DataType = factory.DataType(new Guid(dataTypeAttr.RenderControlGuid));
                dtd.Text = dataTypeAttr.Name;

			    HttpRuntime.Cache.Remove(string.Format("UmbracoDataTypeDefinition{0}", dtd.UniqueId));

				dtd.Save();
                
				var instance = Activator.CreateInstance(typeDataType, null) as DataTypeBase;
				DataTypePrevalue[] prevalues = instance.Prevalues;
				if (prevalues.Any())
				{
					var settingsStorage = new DataEditorSettingsStorage();
                    
                    // updating all settings to those defined in datatype class
                    // If you've exported, all settings will be defined here anyway?
                    settingsStorage.ClearSettings(dtd.Id);
                    settingsStorage.InsertSettings(dtd.Id, prevalues.Select(pre => new Setting<string, string> { Key = pre.Alias, Value = pre.Value }).ToList());
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
