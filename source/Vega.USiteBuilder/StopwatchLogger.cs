namespace Vega.USiteBuilder
{
#if DEBUG
    using System;
    using System.Collections.Generic;

    public static class StopwatchLogger
    {
        private static List<string> _logMessages = new List<string>();

        public static void AddToLog(string format, params object[] args)
        {
            string message = String.Format(format, args);

            _logMessages.Add(message);
        }

        public static List<string> LogMessages
        {
            get
            {
                return _logMessages;
            }
        }
    }
#endif
}
