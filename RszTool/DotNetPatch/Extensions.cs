using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace RszTool;

public static class EnumHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum Parse<TEnum>(ReadOnlySpan<char> span, bool ignoreCase = false) where TEnum : struct, Enum
#if !NET5_0_OR_GREATER
        => (TEnum)Enum.Parse(typeof(TEnum), span.ToString(), ignoreCase);
#else
        => Enum.Parse<TEnum>(span, ignoreCase);
#endif
}
public static class SpanAdditions
{
    public static bool StartsWith(this ReadOnlySpan<char> span, string str)
    {
        return str.Length <= span.Length && span.Slice(0, str.Length).SequenceEqual(str.AsSpan());
    }

#if !NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSpanHash(this ReadOnlySpan<char> span) => CultureInfo.InvariantCulture.CompareInfo.GetHashCode(span.ToString(), CompareOptions.Ordinal);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSpanHash(this ReadOnlySpan<char> span) => CultureInfo.InvariantCulture.CompareInfo.GetHashCode(span, CompareOptions.Ordinal);
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ParseUint(this ReadOnlySpan<char> ch)
    {
#if !NET5_0_OR_GREATER
		return uint.Parse(ch.ToString(), CultureInfo.InvariantCulture);
#else
        return uint.Parse(ch, CultureInfo.InvariantCulture);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ParseFloat(this ReadOnlySpan<char> ch)
    {
#if !NET5_0_OR_GREATER
		return float.Parse(ch.ToString(), CultureInfo.InvariantCulture);
#else
        return float.Parse(ch, CultureInfo.InvariantCulture);
#endif
    }

}
