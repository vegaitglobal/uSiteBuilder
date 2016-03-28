namespace Vega.USiteBuilder
{
    public abstract class MacroBase
    {
        public abstract string MacroName { get; }
        public abstract bool UseInEditor { get; }
        public abstract bool RenderContentInEditor { get; }
        public abstract int CachePeriod { get; }
        public abstract bool CacheByPage { get; }
        public abstract bool CachePersonalized { get; }

        public abstract MacroParameter[] Parameters { get; }
    }
}
