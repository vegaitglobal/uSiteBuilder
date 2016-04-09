namespace Vega.USiteBuilder.Types.Activation
{
    /// <summary>
    /// "Poor man IoC" for ativation. 
    /// </summary>
    public class Activator
    {
        static Activator()
        {
            Current = new ReflectionBasedActivator();
        }

        /// <summary>
        /// Gets currently registered activator
        /// </summary>
        public static IActivator Current { get; private set; }

        /// <summary>
        /// Sets the current activator. 
        /// </summary>
        /// <param name="activator">Activator to use. </param>
        /// <returns>Returns previously used value. </returns>
        public static IActivator SetCurrent(IActivator activator)
        {
            var old = Current;
            Current = activator;
            return old;
        }
    }
}
