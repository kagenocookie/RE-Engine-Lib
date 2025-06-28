using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ReeLib.Efx;

public class BitSet : BaseModel
{
    public int BitCount { get; }
    public int[] Bits { get; set; }

    public int Count => Bits.Sum(b => BitOperations.PopCount((uint)b));
    public int HighestBit => GetHighestBit();
    public string?[]? BitNames { get; init; }

    /// <summary>
    /// Dictionary for 1-based bit index to name mapping.
    /// </summary>
    public Dictionary<int, string> BitNameDict
    {
        // TODO probably move these to some static storage since they're the same across all instances
        // would also make version-specificity easier
        init {
            if (value.Keys.Count == 0) return;
            var max = value.Keys.Max();
            BitNames = new string[max];
            for (int i = 0; i < max; ++i) {
                BitNames[i] = value.GetValueOrDefault(i + 1);
            }
        }
    }

    public BitSet(int bitcount)
    {
        BitCount = bitcount;
        Bits = new int[(bitcount + 31) >> 5];
    }
    public BitSet(int bitcount, int[] array)
    {
        BitCount = bitcount;
        Bits = array;
    }

    protected override bool DoRead(FileHandler handler)
    {
        handler.ReadArray(Bits);
        if (HighestBit > BitCount) {
            Console.Error.WriteLine($"Read bitset exceeds expected max bit count: {HighestBit} > {BitCount}");
        }
        return true;
    }

    protected override bool DoWrite(FileHandler handler)
    {
        handler.WriteArray(Bits);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasBit(int bitIndex)
    {
        return (Bits[bitIndex >> 5] & (1 << bitIndex)) != 0;
    }

    public string? GetBitName(int bitIndex)
    {
        if (BitNames == null || bitIndex >= BitNames.Length) return null;
        return BitNames[bitIndex];
    }

    public bool HasBit(string bitName)
    {
        for (int i = 0; i < BitNames?.Length; ++i) {
            if (BitNames[i] == bitName) return HasBit(i + 1);
        }
        return false;
    }

    public int GetBitInsertIndex(int bitIndex)
    {
        int index = 0;
        for (int i = 0; i < bitIndex; ++i) {
            if (HasBit(i)) index++;
        }
        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBit(int bitIndex, bool set)
    {
        if (set) {
            Bits[bitIndex >> 5] |= (1 << bitIndex);
        } else {
            Bits[bitIndex >> 5] &= ~(1 << bitIndex);
        }
    }

    public IEnumerable<int> GetExpressionInts()
    {
        var count = BitCount;
        for (int i = 0; i < count; ++i) {
            if (HasBit(i)) yield return i;
        }
    }

    public int GetHighestBit()
    {
        for (int i = Bits.Length - 1; i >= 0; --i) {
            if (Bits[i] == 1) return i * 32 + 1;
            var bit = BitOperations.Log2((uint)Bits[i]);
            if (bit != 0) {
                return i * 32 + bit + 1;
            }
        }
        return 0;
    }

    public override string ToString() => $"BitSet: {Count} / {BitCount} {StringifyBits()} <Max: {HighestBit}>";

    private string StringifyBits()
    {
        var nameCount = BitNames?.Length ?? 0;
        var sb = new StringBuilder();
        var first = true;
        for (int i = 0; i < BitCount;++i) {
            if (HasBit(i)) {
                if (!first) sb.Append(", ");
                first = false;
                sb.Append(i < nameCount ? BitNames![i] : (i + 1).ToString());
            }
        }
        if (!first) {
            sb.Insert(0, '(');
            sb.Append(')');
        }
        return sb.ToString();
    }
}