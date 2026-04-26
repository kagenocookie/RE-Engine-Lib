using System.Diagnostics;
using System.Runtime.CompilerServices;

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

        [Conditional("DEBUG")]
        internal static void UniqueValue<T>(T value) => ValueLog<T>.RecordUnique(value);

        [Conditional("DEBUG")]
        internal static void UniqueValueKeyed<T>(T value, [CallerArgumentExpression(nameof(value))] string? valueKey = null) => ValueLog<T>.RecordUniqueKeyed(value, true, valueKey);

        internal static class ValueLog<T>
        {
            private static readonly HashSet<T> _values = new();
            private static readonly Dictionary<string, HashSet<T>> _keyedValues = new();

            public static HashSet<T> UniqueValues => _values;
            public static HashSet<T> GetUnique(string key) => _keyedValues.GetValueOrDefault(key) ?? new();

            [Conditional("DEBUG")]
            public static void RecordUnique(T value, bool logNew = true)
            {
                if (_values.Add(value)) {
                    if (logNew) {
                        Log.Debug($"{typeof(T).Name}: {value}");
                    }
                }
            }

            [Conditional("DEBUG")]
            public static void RecordUniqueKeyed(T value, bool logNew = true, [CallerArgumentExpression(nameof(value))] string? valueKey = null)
            {
                if (!_keyedValues.TryGetValue(valueKey ?? "", out var set)) {
                    _keyedValues[valueKey ?? ""] = set = new HashSet<T>();
                }
                if (set.Add(value)) {
                    if (logNew) {
                        Log.Debug($"{valueKey}: {value}");
                    }
                }
            }
        }
    }
}