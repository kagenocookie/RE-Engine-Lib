namespace ReeLib.Common
{
    public static class Log
    {
        private static string DateTimeString => DateTime.Now.ToString("MM-dd HH:mm:ss");  // .fff

        public static Action<LogLevel, string> LogCallback { get; set; } = DefaultLog;

        public enum LogLevel
        {
            Debug,
            Info,
            Warn,
            Error,
        }

        private static void DefaultLog(LogLevel level, string message)
        {
            switch (level) {
                case ReeLib.Common.Log.LogLevel.Debug:
                    Console.WriteLine($"{DateTimeString} [D] {message}");
                    break;
                default:
                case ReeLib.Common.Log.LogLevel.Info:
                    Console.WriteLine($"{DateTimeString} [I] {message}");
                    break;
                case ReeLib.Common.Log.LogLevel.Warn:
                    Console.WriteLine($"{DateTimeString} [W] {message}");
                    break;
                case ReeLib.Common.Log.LogLevel.Error:
                    Console.Error.WriteLine($"{DateTimeString} [E] {message}");
                    break;
            }
        }

        public static void Info(string message)
        {
            LogCallback.Invoke(LogLevel.Info, message);
        }

        public static void Debug(string message)
        {
            LogCallback.Invoke(LogLevel.Debug, message);
        }

        public static void Warn(string message)
        {
            LogCallback.Invoke(LogLevel.Warn, message);
        }

        public static void WarnIf(bool condition, string message)
        {
            if (condition) {
                Warn(message);
            }
        }

        public static void Error(string message)
        {
            LogCallback.Invoke(LogLevel.Error, message);
        }

        public static void Error(Exception e)
        {
            Error(e.Message);
        }
    }
}