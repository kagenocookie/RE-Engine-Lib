using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ReeLib.Common
{
    public class MurMur3HashUtils
    {
        public static uint MurMur3Hash(ReadOnlySpan<byte> bytes)
        {
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;
            uint hash = 0xffffffff;

            ref byte curByte = ref MemoryMarshal.GetReference(bytes);
            if (bytes.Length >= 4)
            {
                ref uint curUint = ref Unsafe.As<byte, uint>(ref curByte);
                ref uint end = ref Unsafe.Add(ref curUint, bytes.Length >> 2);
                do
                {
                    hash ^= BitOperations.RotateLeft(curUint * c1, 15) * c2;
                    hash = BitOperations.RotateLeft(hash, 13) * 5 + 0xe6546b64;
                    curUint = ref Unsafe.Add(ref curUint, 1);
                } while (Unsafe.IsAddressLessThan(ref curUint, ref end));
                curByte = ref Unsafe.As<uint, byte>(ref end);
            }

            var remainder = bytes.Length & 3;
            if (remainder > 0)
            {
                uint num = remainder switch
                {
                    3 => (uint)(curByte | Unsafe.Add(ref curByte, 1) << 8 | Unsafe.Add(ref curByte, 2) << 16),
                    2 => (uint)(curByte | Unsafe.Add(ref curByte, 1) << 8),
                    _ => curByte,
                };

                hash ^= BitOperations.RotateLeft(num * c1, 15) * c2;
            }

            hash ^= (uint)bytes.Length;
            hash = (hash ^ (hash >> 16)) * 0x85ebca6b;
            hash = (hash ^ (hash >> 13)) * 0xc2b2ae35;
            return hash ^ hash >> 16;
        }

        /// <summary>
        /// Calculate the PAK hash (lowercase + uppercase hash) of a UTF16 file path string.
        /// </summary>
        public static ulong GetPakFilepathHash(ReadOnlySpan<char> path)
        {
            Span<char> strLow = stackalloc char[path.Length];
            Span<char> strHigh = stackalloc char[path.Length];
            path.ToLowerInvariant(strLow);
            path.ToUpperInvariant(strHigh);
            var hLow = MurMur3Hash(MemoryMarshal.AsBytes(strLow));
            var hHigh = MurMur3Hash(MemoryMarshal.AsBytes(strHigh));
            return (ulong)hHigh << 32 | hLow;
        }

        public static uint GetHash(string text)
        {
            return MurMur3Hash(MemoryMarshal.AsBytes(text.AsSpan()));
        }

        public static uint GetHash(ReadOnlySpan<char> text)
        {
            return MurMur3Hash(MemoryMarshal.AsBytes(text));
        }

        /// <summary>
        /// Get the hash of the invariant lowercase of a given string.
        /// </summary>
        public static uint GetHashLower(ReadOnlySpan<char> text)
        {
            Span<char> strLow = stackalloc char[text.Length];
            text.ToLowerInvariant(strLow);
            return GetHash(strLow);
        }

        public static uint GetAsciiHash(string text)
        {
            return MurMur3Hash(Encoding.ASCII.GetBytes(text));
        }

        public static uint GetUTF8Hash(string text)
        {
            return MurMur3Hash(Encoding.UTF8.GetBytes(text));
        }
    }
}
