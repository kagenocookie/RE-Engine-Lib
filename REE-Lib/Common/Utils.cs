using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ReeLib.Common
{
    public struct TimerRecord
    {
        public string? Name { get; set; }
        public long StartUs { get; set; }

        public void Start(string name)
        {
            if (StartUs != 0) {
                End();
            }
            Name = name;
            StartUs = DateTime.Now.Ticks / 10;
        }

        public void End()
        {
            long endUs = DateTime.Now.Ticks / 10;
            Console.WriteLine($"time of {Name}: {endUs - StartUs} us");
        }
    }


    public static class Utils
    {
        /// <summary>
        /// 对齐字节
        /// </summary>
        /// <param name="n"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int AlignSize(int n, int size) => (n + (size - 1)) & ~(size - 1);

        /// <summary>
        /// 对齐字节
        /// </summary>
        /// <param name="n"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static long AlignSize(long n, int size)
        {
            var tail = n & (size - 1);
            if (tail != 0)
                n += size - tail;
            return n;
        }

        /// <summary>
        /// 对齐4字节
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int Align4(int n) => AlignSize(n, 4);

        /// <summary>
        /// 对齐8字节
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int Align8(int n) => AlignSize(n, 8);
        public static int Align16(int n) => AlignSize(n, 16);

        public static bool DetectFloat(Span<byte> data, out float floatValue)
        {
            if (data.Length == 4) {
                floatValue = MemoryUtils.AsRef<float>(data);
                float absValue = Math.Abs(floatValue);
                if (data[3] < 255 && absValue > 0.0000001 && absValue < 10000000) {
                    return true;
                }
            }
            floatValue = 0;
            return false;
        }

        public static uint BitsGet(uint value, int bitOffset, int bitLength)
        {
            uint mask = (uint)((1 << bitLength) - 1);
            return (value >> bitOffset) & mask;
        }

        public static uint BitsSet(uint value, int bitOffset, int bitLength, uint data)
        {
            uint mask = (uint)((1 << bitLength) - 1);
            uint newValue = (value & ~(mask << bitOffset)) | ((data & mask) << bitOffset);
            return newValue;
        }

        public static ulong BitsGet(ulong value, int bitOffset, int bitLength)
        {
            ulong mask = (ulong)((1 << bitLength) - 1);
            return (value >> bitOffset) & mask;
        }

        public static ulong BitsSet(ulong value, int bitOffset, int bitLength, ulong data)
        {
            ulong mask = (ulong)((1 << bitLength) - 1);
            ulong newValue = (value & ~(mask << bitOffset)) | ((data & mask) << bitOffset);
            return newValue;
        }

        private static HashSet<KnownFileFormats> Offset4Formats = [
            KnownFileFormats.UserVariables, KnownFileFormats.ChainWind,
            KnownFileFormats.Clip, KnownFileFormats.GUI,
            KnownFileFormats.Motion, KnownFileFormats.MotionTree,
            KnownFileFormats.MotionPack, KnownFileFormats.MotionCamera,
            KnownFileFormats.MotionList, KnownFileFormats.MotionCameraList,
            KnownFileFormats.MotionFsm2, KnownFileFormats.MotionBank, KnownFileFormats.MotionCameraBank,
            KnownFileFormats.DialogueConfig,
            KnownFileFormats.FbxSkeleton, KnownFileFormats.Skeleton, KnownFileFormats.RefSkeleton,
        ];

        public static List<(uint magicBytes, int magicOffset, KnownFileFormats format)> GetSupportedFileContentFormats()
        {
            var list = new List<(uint magicBytes, int magicOffset, KnownFileFormats format)>();
            var types = typeof(ReeLib.BaseFile).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(BaseFile)));
            foreach (var type in types) {
                var magicField = type.GetField("Magic", System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Public);
                if (magicField == null || magicField.FieldType != typeof(uint)) {
                    continue;
                }
                var magic = (uint)magicField.GetValue(null)!;
                var format = CachedMemoryPakReader.GuessFileFormatFromMagic(magic);
                if (format != KnownFileFormats.Unknown) {
                    var offset = Offset4Formats.Contains(format) ? 4 : 0;
                    list.Add((magic, offset, format));
                }
            }

            return list;
        }

        /// <summary>
        /// Common dictionary storage for all known bone name hashes, intended to help editing files that only contain the hashes.
        /// </summary>
        public static readonly Dictionary<uint, string> HashedBoneNames = new() { {2180083513, ""} };
    }

    public static class Extensions
    {
        public static T? GetTarget<T>(this WeakReference<T> reference) where T : class
        {
            reference.TryGetTarget(out T? target);
            return target;
        }

        public static int GetIndexOrAdd<T>(this List<T> list, T obj)
        {
            int index = list.IndexOf(obj);
            if (index == -1) {
                index = list.Count;
                list.Add(obj);
            }
            return index;
        }

        public static void AppendIndent(this StringBuilder sb, int indent)
        {
            for (int i = 0; i < indent; i++) {
                sb.Append("    ");
            }
        }

        public static string GetUniqueName(this string basename, Func<string, bool> existsCheck, string? suffix = null)
        {
            var name = basename;
            int attempts = 1;
            while (existsCheck.Invoke(name)) {
                name = $"{basename}_{(suffix == null ? ++attempts : (suffix + attempts++))}";
            }

            return name;
        }
    }

    public sealed class PakHashedPathComparer : IEqualityComparer<string>, IComparer<string>
    {
        public static readonly PakHashedPathComparer Instance = new();

        public int Compare(string? x, string? y) => MurMur3HashUtils.GetPakFilepathHash(x)
            .CompareTo(MurMur3HashUtils.GetPakFilepathHash(y));

        public bool Equals(string? x, string? y) => x?.Equals(y, StringComparison.OrdinalIgnoreCase) == true;

        public int GetHashCode([DisallowNull] string str) => MurMur3HashUtils.GetPakFilepathHash(str).GetHashCode();
    }

    public class FuncComparer<T>(Func<T, T, int> func) : IComparer<T>
    {
        public int Compare(T? x, T? y)
        {
            if (x == null)
            {
                return y == null ? 0 : -1;
            }
            return y == null ? 1 : func(x, y);
        }
    }
}
