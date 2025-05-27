namespace RszTool.Efx;

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

public class BitSet : BaseModel
{
    public int BitCount { get; }
    public int[] Bits { get; }

    public int Count => Bits.Sum(b => BitOperations.PopCount((uint)b));
    public int HighestBit => GetHighestBit();
    public string[]? BitNames { get; init; }

    /// <summary>
    /// Dictionary for 1-based bit index to name mapping.
    /// </summary>
    public Dictionary<int, string> BitNameDict
    {
        init {
            var max = value.Keys.Max();
            BitNames = new string[max];
            for (int i = 0; i < max; ++i) {
                BitNames[i] = value.GetValueOrDefault(i + 1) ?? (i + 1).ToString();
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
    public bool HasBit(int bit)
    {
        return (Bits[bit >> 5] & (1 << bit)) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBit(int bit, bool set)
    {
        if (set) {
            Bits[bit >> 5] |= (1 << bit);
        } else {
            Bits[bit >> 5] &= ~(1 << bit);
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
            var po2 = BitOperations.Log2(BitOperations.RoundUpToPowerOf2((uint)Bits[i]));
            if (po2 != 0) {
                return i * 32 + po2;
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