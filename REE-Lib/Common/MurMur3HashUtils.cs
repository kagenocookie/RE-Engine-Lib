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
            const uint seed = 0xffffffff;

            uint h1 = seed;
            uint k1;

            for (int i = 0; i < bytes.Length; i += 4)
            {
                int chunkLength = Math.Min(4, bytes.Length - i);
                k1 = chunkLength switch
                {
                    4 => (uint)(bytes[i] | bytes[i + 1] << 8 | bytes[i + 2] << 16 | bytes[i + 3] << 24),
                    3 => (uint)(bytes[i] | bytes[i + 1] << 8 | bytes[i + 2] << 16),
                    2 => (uint)(bytes[i] | bytes[i + 1] << 8),
                    1 => bytes[i],
                    _ => 0
                };
                k1 = BitOperations.RotateLeft(k1 * c1, 15);
                h1 ^= k1 * c2;
                if (chunkLength == 4)
                {
                    h1 = BitOperations.RotateLeft(h1, 13);
                    h1 = h1 * 5 + 0xe6546b64;
                }
            }

            h1 = Fmix(h1 ^ (uint)bytes.Length);

            return h1;
        }

        /// <summary>
        /// Extra optimized method for calculating the PAK hash of a UTF16 file path string. Assumes no non-ascii base characters.
        /// </summary>
        public static ulong PakFilepathHash(ReadOnlySpan<char> path)
        {
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;
            const uint seed = 0xffffffff;

            uint hLow = seed;
            uint hHigh = seed;
            uint kLow;
            uint kHigh;

            var bytes = MemoryMarshal.AsBytes(path);
            var byteCount = bytes.Length;
            Span<byte> bufLow = stackalloc byte[byteCount];
            Span<byte> bufHigh = stackalloc byte[byteCount];
            Ascii.ToLower(bytes, bufLow, out _);
            Ascii.ToUpper(bytes, bufHigh, out _);

            int i = 0;
            while (byteCount - i >= 4) {
                kLow = (uint)(bufLow[i] | (bufLow[i + 2] << 16)) * c1;
                kHigh = (uint)(bufHigh[i] | (bufHigh[i + 2] << 16)) * c1;
                hLow ^= BitOperations.RotateLeft(kLow, 15) * c2;
                hHigh ^= BitOperations.RotateLeft(kHigh, 15) * c2;

                hLow = BitOperations.RotateLeft(hLow, 13) * 5 + 0xe6546b64;
                hHigh = BitOperations.RotateLeft(hHigh, 13) * 5 + 0xe6546b64;
                i += 4;
            }

            if (byteCount - i == 2) {
                kLow = (uint)(bufLow[i]) * c1;
                kHigh = (uint)(bufHigh[i]) * c1;
                hLow ^= BitOperations.RotateLeft(kLow, 15) * c2;
                hHigh ^= BitOperations.RotateLeft(kHigh, 15) * c2;
            }

            hLow = Fmix(hLow ^ (uint)byteCount);
            hHigh = Fmix(hHigh ^ (uint)byteCount);
            return (ulong)hHigh << 32 | hLow;
        }

        /// <summary>
        /// Calculate the PAK hash (lowercase + uppercase hash) of a UTF16 file path string. Works with non-ascii characters, but is about 50% slower than <see cref="PakFilepathHash"/>.
        /// </summary>
        public static ulong PakFilepathHash_SafeUTF16(ReadOnlySpan<char> path)
        {
            Span<char> strLow = stackalloc char[path.Length];
            Span<char> strHigh = stackalloc char[path.Length];
            path.ToLowerInvariant(strLow);
            path.ToUpperInvariant(strHigh);
            var hLow = GetHash(strLow);
            var hHigh = GetHash(strHigh);
            return (ulong)hHigh << 32 | hLow;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Fmix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }

        public static uint GetHash(string text)
        {
            return MurMur3Hash(MemoryMarshal.AsBytes(text.AsSpan()));
        }

        public static uint GetHash(ReadOnlySpan<char> text)
        {
            return MurMur3Hash(MemoryMarshal.AsBytes(text));
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
