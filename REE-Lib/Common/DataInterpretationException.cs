using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using ReeLib.Common;

namespace ReeLib
{
    /// <summary>
    /// Thrown when an expected assumption as to how the data for a file format works is found to be false.
    /// </summary>
    public class DataInterpretationException(string message = "Found unexpected data while reading file") : Exception(message)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIf([DoesNotReturnIf(true)] bool condition, string message = "Found unexpected data while reading file")
        {
            if (condition) throw new DataInterpretationException(message);
        }

        [Conditional("DEBUG"), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DebugThrowIf([DoesNotReturnIf(true)] bool condition, string message = "Found unexpected data while reading file", [CallerArgumentExpression(nameof(condition))] string conditionText = null!)
        {
            if (condition) throw new DataInterpretationException($"[{conditionText}] {message}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNotZero<TValue>(TValue value, string message = "Found unexpected data instead of padding while reading file") where TValue : IBinaryNumber<TValue>
        {
            if (!value.Equals(TValue.Zero)) throw new DataInterpretationException(message);
        }

        [Conditional("DEBUG"), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfDifferent<TValue>(TValue value1, TValue value2, string message = "Values were expected to be identical, but weren't") where TValue : IBinaryNumber<TValue>
        {
            if (value1 != value2) throw new DataInterpretationException(message);
        }

        [Conditional("DEBUG"), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNotEqualValues<TValue>(ReadOnlySpan<TValue> values, string message = "Array values were expected to be identical, but weren't") where TValue : IBinaryNumber<TValue>
        {
            if (values.Length == 0) return;
            var value = values[0];
            foreach (var val in values) {
                if (val != value) throw new DataInterpretationException($"{message} (index {values.IndexOf(val)})");
            }
        }

        [Conditional("DEBUG"), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfArraysNotEqual<TValue>(ReadOnlySpan<TValue> values1, ReadOnlySpan<TValue> values2, string message = "Array values were expected to be identical, but weren't") where TValue : IBinaryNumber<TValue>
        {
            var count = values1.Length;
            for (int i = 0; i < count; ++i)
            {
                ThrowIfDifferent(values1[i], values2[i], message);
            }
        }

        [Conditional("DEBUG")]
        public static void ThrowIfNotZeroEOF(FileHandler handler, string message = "Found unhandled data at end of file")
        {
            var size = (int)(handler.Stream.Length - handler.Position);
            if (size < 0) throw new DataInterpretationException(message);

            Span<byte> bytes = stackalloc byte[size];
            for (int i = 0; i < bytes.Length; ++i) DataInterpretationException.ThrowIfNotZero(bytes[i], message);
        }

        [Conditional("DEBUG"), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DebugWarnIf([DoesNotReturnIf(true)] bool condition, string message = "Found unexpected data while reading file", [CallerArgumentExpression(nameof(condition))] string conditionText = null!)
        {
            if (condition)
            {
                Log.Warn($"[{conditionText}] {message}");
            }
        }

    }
}
