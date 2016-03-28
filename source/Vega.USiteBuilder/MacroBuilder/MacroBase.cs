namespace Vega.USiteBuilder
{
    /// <summary>
    /// Macro base abstract class
    /// </summary>
    public abstract class MacroBase
    {
        /// <summary>
        /// Gets the name of the macro.
        /// </summary>
        /// <value>
        /// The name of the macro.
        /// </value>
        public abstract string MacroName { get; }
        /// <summary>
        /// Gets a value indicating whether [use in editor].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use in editor]; otherwise, <c>false</c>.
        /// </value>
        public abstract bool UseInEditor { get; }
        /// <summary>
        /// Gets a value indicating whether [render content in editor].
        /// </summary>
        /// <value>
        /// <c>true</c> if [render content in editor]; otherwise, <c>false</c>.
        /// </value>
        public abstract bool RenderContentInEditor { get; }
        /// <summary>
        /// Gets the cache period.
        /// </summary>
        /// <value>
        /// The cache period.
        /// </value>
        public abstract int CachePeriod { get; }
        /// <summary>
        /// Gets a value indicating whether [cache by page].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [cache by page]; otherwise, <c>false</c>.
        /// </value>
        public abstract bool CacheByPage { get; }
        /// <summary>
        /// Gets a value indicating whether [cache personalized].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [cache personalized]; otherwise, <c>false</c>.
        /// </value>
        public abstract bool CachePersonalized { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public abstract MacroParameter[] Parameters { get; }
    }
}
