using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

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
        public static void DebugThrowIf([DoesNotReturnIf(true)] bool condition, string message = "Found unexpected data while reading file")
        {
            if (condition) throw new DataInterpretationException(message);
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
    }
}
