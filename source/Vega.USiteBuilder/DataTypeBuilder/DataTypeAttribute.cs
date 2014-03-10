namespace Vega.USiteBuilder
{
	using System;
	using System.Linq;

	using umbraco.cms.businesslogic.datatype;

	/// <summary>
	/// Provides access to various Umbraco properties of this DataType.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class DataTypeAttribute : Attribute
	{
		/// <summary>
		/// The Guid of the control to render
		/// </summary>
		private string renderControlGuid = string.Empty;

		/// <summary>
		/// Gets or sets the DataType name.
		/// </summary>
		/// <value>
		/// The DataType name.
		/// </value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the data editor GUID.
		/// </summary>
		/// <value>
		/// The data editor GUID.
		/// </value>
		public string UniqueId { get; set; }

		/// <summary>
		/// Gets or sets the database datatype.
		/// </summary>
		/// <value>
		/// The database datatype.
		/// </value>
		public DBTypes DatabaseDataType { get; set; }

		/// <summary>
		/// Gets or sets the Guid of the control to render in the Umbraco UI.
		/// </summary>
		/// <value>
		/// The Guid of the control to render in the Umbraco UI.
		/// </value>
		public string RenderControlGuid
		{
			get
			{
				string guid = string.Empty;

				if (!string.IsNullOrEmpty(renderControlGuid))
				{
					guid = renderControlGuid;
				}

				if (!string.IsNullOrEmpty(this.RenderControlName))
				{
					var factory = new umbraco.cms.businesslogic.datatype.controls.Factory();
					var dataType =
						factory.GetAll().Where(d => d.DataTypeName == this.RenderControlName).FirstOrDefault();

					guid = dataType.Id.ToString();
				}

				return guid;
			}

			set
			{
				renderControlGuid = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the render control.
		/// </summary>
		/// <value>
		/// The name of the render control.
		/// </value>
		public string RenderControlName { get; set; }
	}
}
