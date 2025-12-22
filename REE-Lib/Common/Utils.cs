using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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
        /// Makes a deep clone of the target object.
        /// </summary>
        [return: NotNullIfNotNull(nameof(target))]
        public static T? DeepClone<T>(this object? target)
        {
            return (T?)DeepClone(target);
        }

        /// <summary>
        /// Makes a deep clone of the target object.
        /// </summary>
        [return: NotNullIfNotNull(nameof(target))]
        public static object? DeepClone(this object? target)
        {
            if (target == null) return null;
            var type = target.GetType();
            if (type.IsValueType || type == typeof(string)) return target;
            return typeof(DeepCloneUtil<>).MakeGenericType(type).GetMethod("Clone")!.Invoke(null, [target])!;
        }

        /// <summary>
        /// Makes a deep clone of the target object.
        /// </summary>
        public static T DeepCloneGeneric<T>(this T target) where T : class
        {
            return DeepCloneUtil<T>.Clone(target);
        }
    }

    public static class DeepCloneUtil<T> where T : class
    {
        private static readonly MethodInfo MemberwiseCloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance)!;

        private static FieldInfo[]? _classFields;
        private static FieldInfo[]? _cloneableFields;

        /// <summary>
        /// Makes a deep clone of the source object.
        /// </summary>
        public static T Clone(T source)
        {
            if (source is IList list) {
                var count = list.Count;
                if (typeof(T).IsArray) {
                    var newArray = Array.CreateInstance(typeof(T).GetElementType()!, count)! as IList;
                    for (int i = 0; i < count; ++i) newArray[i] = list[i].DeepClone();
                    return (T)newArray;
                } else {
                    var newList = (IList)Activator.CreateInstance<T>();
                    for (int i = 0; i < count; ++i) newList.Add(list[i].DeepClone());
                    return (T)newList;
                }
            }

            var clone = (T)MemberwiseCloneMethod.Invoke(source, Array.Empty<object?>())!;

            if (_classFields == null) {
                // note to self: we shouldn't need to also clone properties here since backing fields already get picked up with GetFields
                IEnumerable<FieldInfo> allFields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var baseType = typeof(T).BaseType;
                while (baseType != null && baseType != typeof(object))
                {
                    // need to separately handle private base fields
                    allFields = allFields.Concat(baseType!.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
                    baseType = baseType.BaseType;
                }
                var fields = allFields.Where(fi => fi.FieldType.IsClass && fi.FieldType != typeof(string));
                _classFields = fields.Where(f => !f.FieldType.IsAssignableTo(typeof(ICloneable))).ToArray();
                _cloneableFields = fields.Where(f => f.FieldType.IsAssignableTo(typeof(ICloneable))).ToArray();
            }

            foreach (var plain in _classFields) {
                plain.SetValue(clone, plain.GetValue(source).DeepClone());
            }

            foreach (var plain in _cloneableFields!) {
                plain.SetValue(clone, ((ICloneable?)plain.GetValue(source))?.Clone());
            }
            return clone;
        }
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
