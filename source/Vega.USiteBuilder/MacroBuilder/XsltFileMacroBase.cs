namespace Vega.USiteBuilder.MacroBuilder
{
    /// <summary>
    /// XSLT file macro base
    /// </summary>
    public abstract class XsltFileMacroBase : MacroBase
    {
        /// <summary>
        /// Gets the name of the XSLT file.
        /// </summary>
        /// <value>
        /// The name of the XSLT file.
        /// </value>
        public abstract string XsltFileName { get; }
    }
}
