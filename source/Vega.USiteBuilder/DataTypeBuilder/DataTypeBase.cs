using umbraco.cms.businesslogic.datatype;

namespace Vega.USiteBuilder
{
	/// <summary>
	/// Base class for DataTypes.
	/// </summary>
	public abstract class DataTypeBase
	{
        /// <summary>
        /// Gets the prevalues.
        /// </summary>
        /// <value>
        /// The prevalues.
        /// </value>
		public abstract DataTypePrevalue[] Prevalues { get; }
	}
}