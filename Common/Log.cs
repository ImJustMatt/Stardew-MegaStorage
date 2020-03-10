using StardewModdingAPI;

namespace MegaStorage
{
    internal static class Log
    {
        public static IMonitor Monitor;
        public static void Verbose(string message)
        {
            Monitor.VerboseLog(message);
        }

        public static void Trace(string message)
        {
            Monitor.Log(message, LogLevel.Trace);
        }

        public static void Debug(string message)
        {
            Monitor.Log(message, LogLevel.Debug);
        }

        public static void Info(string message)
        {
            Monitor.Log(message, LogLevel.Info);
        }

        public static void Warn(string message)
        {
            Monitor.Log(message, LogLevel.Warn);
        }

        public static void Error(string message)
        {
            Monitor.Log(message, LogLevel.Error);
        }
    }
}
