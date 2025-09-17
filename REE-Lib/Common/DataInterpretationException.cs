using System.Diagnostics;
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
        public static void ThrowIf(bool condition, string message = "Found unexpected data while reading file")
        {
            if (condition) throw new DataInterpretationException(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNotZero<TValue>(TValue value, string message = "Found unexpected data instead of padding while reading file") where TValue : IBinaryInteger<TValue>
        {
            if (!value.Equals(TValue.Zero)) throw new DataInterpretationException(message);
        }
    }
}
