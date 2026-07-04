using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ReeLib.Common
{
    public static class CloneExtensions
    {
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

        /// <summary>
        /// Deep copy an object onto another object.
        /// </summary>
        public static void CloneFieldsTo<T>(this T source, T target, bool includeValueFields = true) where T : class
        {
            DeepCloneUtil<T>.ReplaceFields(source, target, includeValueFields);
        }

        /// <summary>
        /// Deep copy an object onto another object.
        /// </summary>
        public static void CloneListTo<T>(this List<T> source, List<T> target) where T : class
        {
            target.Clear();
            foreach (var v in source) {
                target.Add(v.DeepClone<T>());
            }
        }
    }

    public static class DeepCloneUtil<T> where T : class
    {
        private static readonly MethodInfo MemberwiseCloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance)!;

        private static FieldInfo[]? _classFields;
        private static FieldInfo[]? _valueFields;
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
            ReplaceFields(source, clone);
            return clone;
        }

        public static void ReplaceFields(T source, T target, bool includeValueFields = false)
        {
            if (source is ITargetCloneable<T> cc) {
                cc.CloneTo(target);
                return;
            }
            if (_classFields == null) {
                // note to self: we shouldn't need to also clone properties here since backing fields already get picked up with GetFields
                IEnumerable<FieldInfo> allFields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var baseType = typeof(T).BaseType;
                while (baseType != null && baseType != typeof(object)) {
                    // need to separately handle private base fields
                    allFields = allFields.Concat(baseType!.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
                    baseType = baseType.BaseType;
                }
                var fields = allFields.Where(fi => fi.FieldType.IsClass && fi.FieldType != typeof(string));
                _classFields = fields.Where(f => !f.FieldType.IsAssignableTo(typeof(ICloneable))).ToArray();
                _cloneableFields = fields.Where(f => f.FieldType.IsAssignableTo(typeof(ICloneable))).ToArray();
                _valueFields = allFields.Where(f => f.FieldType.IsValueType).ToArray();
            }

            foreach (var plain in _classFields) {
                plain.SetValue(target, plain.GetValue(source).DeepClone());
            }

            foreach (var plain in _cloneableFields!) {
                plain.SetValue(target, ((ICloneable?)plain.GetValue(source))?.Clone());
            }

            if (includeValueFields) {
                foreach (var ff in _valueFields!) {
                    ff.SetValue(target, ff.GetValue(source));
                }
            }
        }
    }

    public interface ITargetCloneable<T>
    {
        T CloneTo(T target);
    }
}
