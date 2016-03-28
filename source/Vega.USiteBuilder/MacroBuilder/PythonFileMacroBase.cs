namespace Vega.USiteBuilder
{
    /// <summary>
    /// Python macro file base class
    /// </summary>
    public abstract class PythonFileMacroBase : MacroBase
    {
        /// <summary>
        /// Gets the python file.
        /// </summary>
        /// <value>
        /// The python file.
        /// </value>
        public abstract string PythonFile { get; }
    }
}
