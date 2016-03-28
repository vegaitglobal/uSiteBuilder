namespace Vega.USiteBuilder.DataTypeBuilder
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