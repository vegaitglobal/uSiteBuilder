namespace Vega.USiteBuilder.MacroBuilder
{
    /// <summary>
    /// Custom control macro base class
    /// </summary>
    public abstract class CustomControlMacroBase : MacroBase
    {
        /// <summary>
        /// Gets the assembly.
        /// </summary>
        /// <value>
        /// The assembly.
        /// </value>
        public abstract string Assembly { get; }
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public abstract string Type { get; }
    }
}
