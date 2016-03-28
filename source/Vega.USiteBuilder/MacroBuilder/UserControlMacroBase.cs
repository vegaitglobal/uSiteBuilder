namespace Vega.USiteBuilder.MacroBuilder
{
    /// <summary>
    /// User control macro base class
    /// </summary>
    public abstract class UserControlMacroBase : MacroBase
    {
        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <value>
        /// The control.
        /// </value>
        public abstract string Control { get; }
    }
}
