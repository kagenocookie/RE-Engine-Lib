using System.Globalization;
using System.Runtime.CompilerServices;

namespace ReeLib;

public static class SpanAdditions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSpanHash(this ReadOnlySpan<char> span) => CultureInfo.InvariantCulture.CompareInfo.GetHashCode(span, CompareOptions.Ordinal);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ParseUint(this ReadOnlySpan<char> ch)
    {
        return uint.Parse(ch, CultureInfo.InvariantCulture);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ParseFloat(this ReadOnlySpan<char> ch)
    {
        return float.Parse(ch, CultureInfo.InvariantCulture);
    }
}
