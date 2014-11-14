using System.Diagnostics;
using Umbraco.Core.Models;

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
	    private int _parentId = -1;
		/// <summary>
		/// The Guid of the control to render
		/// </summary>
		private string renderControlGuid = string.Empty;

	    private DataTypeDatabaseType? _dbType;

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
        [Obsolete("Use DataTypeDatabaseType instead. ")]
		public DBTypes DatabaseDataType { get; set; }

	    private DataTypeDatabaseType ConvertDBTypesToDataTypeDatabaseType(DBTypes legacyValue)
	    {
	        switch (legacyValue)
	        {
                case DBTypes.Date:
	                return DataTypeDatabaseType.Date;
                case DBTypes.Integer:
                    return DataTypeDatabaseType.Integer;
                case DBTypes.Ntext:
                    return DataTypeDatabaseType.Ntext;
                default:
                    // case DBTypes.Nvarchar:
                    return DataTypeDatabaseType.Nvarchar;
	        }
	    }

	    public DataTypeDatabaseType DataTypeDatabaseType
	    {
            get { return _dbType.GetValueOrDefault(ConvertDBTypesToDataTypeDatabaseType(DatabaseDataType)); }
	        set { _dbType = value; }
	    }

		/// <summary>
		/// Gets or sets the Guid of the control to render in the Umbraco UI.
		/// </summary>
		/// <value>
		/// The Guid of the control to render in the Umbraco UI.
		/// </value>
        [Obsolete("Use PropertyEditorAlias instead. ")]
		public string RenderControlGuid
		{
			get
			{
				string guid = string.Empty;

				if (!string.IsNullOrEmpty(renderControlGuid))
				{
					guid = renderControlGuid;
				}
				else if (!string.IsNullOrEmpty(this.RenderControlName))
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
        [Obsolete("Use PropertyEditorAlias instead. ")]
		public string RenderControlName 
        {
            get { return PropertyEditorAlias; }
            set { PropertyEditorAlias = value; }
        }

        /// <summary>
        /// Gets or sets parent Id. 
        /// -1 by default. 
        /// </summary>
        public int ParentId
        {
            get { return _parentId; }
            set { _parentId = value; }
        }

        /// <summary>
        /// Gets or sets property editor alias. 
        /// </summary>
        public string PropertyEditorAlias { get; set; }
	}
}
