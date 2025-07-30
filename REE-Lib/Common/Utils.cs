using System.Runtime.CompilerServices;
using System.Text;

namespace ReeLib.Common
{
    public struct TimerRecord
    {
        public string? Name { get; set; }
        public long StartUs { get; set; }

        public void Start(string name)
        {
            if (StartUs != 0)
            {
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

        public static bool DetectFloat(Span<byte> data, out float floatValue)
        {
            if (data.Length == 4)
            {
                floatValue = MemoryUtils.AsRef<float>(data);
                float absValue = Math.Abs(floatValue);
                if (data[3] < 255 && absValue > 0.0000001 && absValue < 10000000)
                {
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

        public static void Debug(object value, [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            Console.WriteLine($"{name} = {value}");
        }
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
            if (index == -1)
            {
                index = list.Count;
                list.Add(obj);
            }
            return index;
        }

        public static void AppendIndent(this StringBuilder sb, int indent)
        {
            for (int i = 0; i < indent; i++)
            {
                sb.Append("    ");
            }
        }

        public static string GetUniqueName(this string basename, Func<string, bool> existsCheck)
        {
            var name = basename;
            int attempts = 2;
            while (existsCheck.Invoke(name)) {
                name = $"{basename}_{attempts++}";
            }

            return name;
        }
    }
}
