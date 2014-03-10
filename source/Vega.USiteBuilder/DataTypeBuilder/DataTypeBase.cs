using umbraco.cms.businesslogic.datatype;

namespace Vega.USiteBuilder
{
	/// <summary>
	/// Base class for DataTypes.
	/// </summary>
	public abstract class DataTypeBase
	{
		public abstract DataTypePrevalue[] Prevalues { get; }
	}
}