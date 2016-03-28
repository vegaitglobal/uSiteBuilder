using System;

namespace Vega.USiteBuilder.Types
{
    /// <summary>
    /// Interface for converting a value from umbraco xml.
    /// </summary>
    public interface ICustomTypeConvertor
    {
        /// <summary>
        /// Gets the Type that this convertor converts to and from
        /// </summary>
        Type ConvertType { get; }

        /// <summary>
        /// Converts inputValue to other type and returns converted value. This method is used when reading item from Umbraco.
        /// </summary>
        /// <param name="inputValue">Input value (for example string xml)</param>
        /// <returns>Output value (instance of class created from input xml, could be anything)</returns>
        object ConvertValueWhenRead(object inputValue);

        /// <summary>
        /// Converts inputValue to other type and returns converted value. This method is used when writting item to Umbraco (e.g. with ContentHelper.Save).
        /// </summary>
        /// <param name="inputValue">Input value (for example List of RelatedLinks)</param>
        /// <returns>Output value (string or xml)</returns>
        object ConvertValueWhenWrite(object inputValue);
    }
}
