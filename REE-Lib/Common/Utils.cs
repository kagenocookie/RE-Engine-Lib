using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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

        /// <summary>
        /// Makes a deep clone of the target object. NOTE: probably doesn't handle arrays/lists correctly yet.
        /// </summary>
        [return: NotNullIfNotNull(nameof(target))]
        public static T? DeepClone<T>(this object? target)
        {
            return (T?)DeepClone(target);
        }

        /// <summary>
        /// Makes a deep clone of the target object. NOTE: probably doesn't handle arrays/lists correctly yet.
        /// </summary>
        [return: NotNullIfNotNull(nameof(target))]
        public static object? DeepClone(this object? target)
        {
            if (target == null) return null;
            return typeof(DeepCloneUtil<>).MakeGenericType(target.GetType()).GetMethod("Clone")!.Invoke(null, [target])!;
        }

        /// <summary>
        /// Makes a deep clone of the target object. NOTE: probably doesn't handle arrays/lists correctly yet.
        /// </summary>
        public static T DeepCloneGeneric<T>(this T target) where T : class
        {
            return DeepCloneUtil<T>.Clone(target);
        }
    }

    public static class DeepCloneUtil<T> where T : class
    {
        private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance)!;

        private static FieldInfo[]? _classFields;
        private static FieldInfo[]? _cloneableFields;

        /// <summary>
        /// Makes a deep clone of the target object. NOTE: probably doesn't handle arrays/lists correctly yet.
        /// </summary>
        public static T Clone(T target)
        {
            if (_classFields == null) {
                var allFields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var fields = allFields.Where(fi => fi.FieldType.IsClass && fi.FieldType != typeof(string));
                _classFields = fields.Where(f => !f.FieldType.IsAssignableTo(typeof(ICloneable))).ToArray();
                _cloneableFields = fields.Where(f => f.FieldType.IsAssignableTo(typeof(ICloneable))).ToArray();
            }

            var clone = (T)CloneMethod.Invoke(target, Array.Empty<object?>())!;
            foreach (var plain in _classFields) {
                plain.SetValue(clone, plain.GetValue(target).DeepClone());
            }
            foreach (var plain in _cloneableFields!) {
                plain.SetValue(clone, ((ICloneable)plain.GetValue(target)!).Clone());
            }

            return clone;
        }
    }
}
