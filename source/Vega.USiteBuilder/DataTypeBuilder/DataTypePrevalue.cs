namespace Vega.USiteBuilder
{
  /// <summary>
  /// Prevalue comparison types
  /// </summary>
  public enum DataTypePrevalueCompareType
  {
      /// <summary>
      /// The by alias
      /// </summary>
    ByAlias,
    /// <summary>
    /// The by value
    /// </summary>
    ByValue,
    /// <summary>
    /// The by alias and value
    /// </summary>
    ByAliasAndValue
  }

  /// <summary>
  /// Defines a DataType PreValue
  /// </summary>
  public class DataTypePrevalue
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DataTypePrevalue"/> class.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="value">The value.</param>
    public DataTypePrevalue(string alias, string value)
    {
      this.Alias = alias;
      this.Value = value;
      this.SortOrder = 0;
      this.CompareType = DataTypePrevalueCompareType.ByAliasAndValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTypePrevalue"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="sortOrder">The sort order value.</param>
    public DataTypePrevalue(string value, int sortOrder)
    {
      this.Alias = string.Empty;
      this.Value = value;
      this.SortOrder = sortOrder;
      this.CompareType = DataTypePrevalueCompareType.ByAliasAndValue;
    }

    /// <summary>
    /// Gets or sets the alias.
    /// </summary>
    /// <value>
    /// The alias.
    /// </value>
    public string Alias { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the sort order value.
    /// </summary>
    /// <value>
    /// The sort order value.
    /// </value>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets the type of comparison to perform.
    /// </summary>
    /// <value>
    /// The type of comparison to perform
    /// </value>
    public DataTypePrevalueCompareType CompareType { get; set; }
  }
}