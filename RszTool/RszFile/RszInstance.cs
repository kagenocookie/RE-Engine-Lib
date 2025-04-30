using System.Numerics;
using System.Text;
using RszTool.Common;

namespace RszTool
{
    /// <summary>
    /// 存放RszClass的数据
    /// </summary>
    public class RszInstance : BaseModel, ICloneable
    {
        public RszClass RszClass { get; set; }
        public object[] Values { get; set; }
        private int index;
        public int Index
        {
            get => index;
            set
            {
                index = value;
                name = null;
                if (RSZUserData != null)
                {
                    RSZUserData.InstanceId = value;
                }
            }
        }
        // 在ObjectTable中的序号，-1表示不在
        public int ObjectTableIndex { get; set; } = -1;
        private string? name;
        public string Name => name ??= $"{RszClass.name}[{index}]";
        public IRSZUserDataInfo? RSZUserData { get; set; }
        public RszField[] Fields => RszClass.fields;
        public bool HasValues => Values.Length > 0;
        public event Action<RszInstance>? ValuesChanged;

        public RszInstance(RszClass rszClass, int index = -1, IRSZUserDataInfo? userData = null, object[]? values = null)
        {
            RszClass = rszClass;
            if (userData == null && values != null)
            {
                if (values.Length != rszClass.fields.Length)
                {
                    throw new ArgumentException($"values length {values.Length} != fields length {rszClass.fields.Length}");
                }
            }
            Values = userData == null ? (values ?? new object[rszClass.fields.Length]) : [];
            Index = index;
            RSZUserData = userData;
        }

        private RszInstance()
        {
            RszClass = RszClass.Empty;
            Values = [];
        }

        static RszInstance()
        {
            foreach (var item in RszFieldTypeToCSharpTypeDict)
            {
                if (!CSharpTypeToRszFieldTypeDict.ContainsKey(item.Value))
                {
                    CSharpTypeToRszFieldTypeDict[item.Value] = item.Key;
                }
            }
        }

        /// <summary>
        /// 一般InstanceList第一个对象是NULL实例
        /// </summary>
        public static readonly RszInstance NULL = new();

        public override string ToString()
        {
            return Name;
        }

        private void AlignFirstField(FileHandler handler)
        {
            handler.Align(RszClass.fields[0].array ? 4 : RszClass.fields[0].align);
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        protected override bool DoRead(FileHandler handler)
        {
            // if has RSZUserData, it is external
            if (RSZUserData != null || RszClass.fields.Length == 0) return true;

            AlignFirstField(handler);
            // Console.WriteLine($"read {Name} at: {handler.Position:X}");
            for (int i = 0; i < RszClass.fields.Length; i++)
            {
                Values[i] = ReadRszField(handler, i);
            }
            return true;
        }

        /// <summary>
        /// 读取字段
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public object ReadRszField(FileHandler handler, int index)
        {
            RszField field = RszClass.fields[index];
            handler.Align(field.array ? 4 : field.align);
            // Console.WriteLine($"    read at: {handler.Position:X} {field.original_type} {field.name}");
            if (field.array)
            {
                int count = handler.ReadInt();
                if (count < 0)
                {
                    throw new InvalidDataException($"{field.name} count {count} < 0");
                }
                if (count > 5000)
                {
                    throw new InvalidDataException($"{field.name} count {count} too large");
                }
                List<object> arrayItems = new(count);
                if (count > 0) handler.Align(field.align);
                for (int i = 0; i < count; i++)
                {
                    if (field.IsString)
                    {
                        handler.Align(4);
                    }
                    object value = ReadNormalField(handler, field);
                    if (DetectDataType(field, ref value) && i > 0)
                    {
                        // 如果检测到其他类型，那么索引0的时候就应该检测出来了，说明之前检测的不对？
                        throw new InvalidDataException($"Detect {RszClass.name}.{field.name} as {field.type}, but index > 0");
                    }
                    arrayItems.Add(value);
                }
                return arrayItems;
            }
            else
            {
                object value = ReadNormalField(handler, field);
                DetectDataType(field, ref value);
                return value;
            }
        }

        /// <summary>
        /// 推断type是Data的字段的实际类型
        /// </summary>
        private bool DetectDataType(RszField field, ref object data)
        {
            if (field.type == RszFieldType.Data && field.size == 4 && field.native)
            {
                int intValue = BitConverter.ToInt32((byte[])data, 0);
                // 检测Object
                if (intValue < Index && intValue > 2 && intValue > Index - 101)
                {
                    field.type = RszFieldType.Object;
                    field.IsTypeInferred = true;
                    data = intValue;
                    Console.WriteLine($"Detect {Name}.{field.name} as Object");
                    return true;
                }
                // 检测float和int
                else if (Utils.DetectFloat((byte[])data, out float floatValue))
                {
                    field.type = RszFieldType.F32;
                    field.IsTypeInferred = true;
                    data = floatValue;
                }
                else
                {
                    field.type = RszFieldType.S32;
                    field.IsTypeInferred = true;
                    data = intValue;
                }
            }
            return false;
        }

        public static object ReadNormalField(FileHandler handler, RszField field)
        {
            if (field.IsString)
            {
                int charCount = handler.ReadInt();
                long stringStart = handler.Tell();
                string value = charCount <= 1 ? "" : handler.ReadWString(charCount: charCount);
                handler.Seek(stringStart + charCount * 2);
                // TODO checkOpenResource
                return value;
            }
            else if (field.type == RszFieldType.RuntimeType)
            {
                var count = (int)handler.ReadUInt();
                var str = handler.ReadAsciiString(-1, count, false);
                return str;
            }
            else
            {
                long startPos = handler.Tell();
                object value;
                if (field.type == RszFieldType.Data)
                {
                    value = handler.ReadBytes(field.size);
                }
                else
                {
                    Type dataType = RszFieldTypeToCSharpType(field.type);
                    value = handler.ReadObject(dataType);
                }
                /* if (field.size != handler.Tell() - startPos)
                {
                    throw new InvalidDataException($"{field.name} size not match");
                } */
                handler.Seek(startPos + field.size);
                return value;
            }
        }

        public static bool WriteNormalField(FileHandler handler, RszField field, object value)
        {
            if (field.IsString)
            {
                string valueStr = (string)value;
                return handler.Write(valueStr.Length + 1) && handler.WriteWString(valueStr);
            }
            else if (field.type == RszFieldType.Object || field.type == RszFieldType.UserData)
            {
                return handler.Write(value is RszInstance instance ? instance.Index : (int)value);
            }
            else if (field.type == RszFieldType.RuntimeType)
            {
                string valueStr = (string)value;
                return handler.Write(valueStr.Length + 1) && handler.WriteAsciiString(valueStr);
            }
            else
            {
                long startPos = handler.Tell();
                if (field.type == RszFieldType.Data)
                {
                    handler.WriteBytes((byte[])value);
                }
                else
                {
                    _ = RszFieldTypeToCSharpType(field.type);
                    handler.WriteObject(value);
                }
                handler.Seek(startPos + field.size);
                return true;
            }
        }

        private static readonly Dictionary<RszFieldType, Type> RszFieldTypeToCSharpTypeDict = new()
        {
            [RszFieldType.S32] = typeof(int),
            [RszFieldType.Object] = typeof(int),
            [RszFieldType.UserData] = typeof(int),
            [RszFieldType.U32] = typeof(uint),
            [RszFieldType.S64] = typeof(long),
            [RszFieldType.U64] = typeof(ulong),
            [RszFieldType.F32] = typeof(float),
            [RszFieldType.F64] = typeof(double),
            [RszFieldType.Bool] = typeof(bool),
            [RszFieldType.S8] = typeof(sbyte),
            [RszFieldType.U8] = typeof(byte),
            [RszFieldType.S16] = typeof(short),
            [RszFieldType.U16] = typeof(ushort),
            [RszFieldType.Data] = typeof(byte[]),
            [RszFieldType.Mat4] = typeof(via.mat4),
            [RszFieldType.Vec2] = typeof(Vector2),
            [RszFieldType.Float2] = typeof(Vector2),
            [RszFieldType.Point] = typeof(Vector2),
            [RszFieldType.Vec3] = typeof(Vector3),
            [RszFieldType.Float3] = typeof(Vector3),
            [RszFieldType.Vec4] = typeof(Vector4),
            [RszFieldType.Float4] = typeof(Vector4),
            [RszFieldType.Int2] = typeof(via.Int2),
            [RszFieldType.Int3] = typeof(via.Int3),
            [RszFieldType.Int4] = typeof(via.Int4),
            [RszFieldType.Uint2] = typeof(via.Uint2),
            [RszFieldType.Uint3] = typeof(via.Uint3),
            [RszFieldType.Uint4] = typeof(via.Uint4),
            [RszFieldType.Position] = typeof(via.Position),
            [RszFieldType.OBB] = typeof(via.OBB),
            [RszFieldType.AABB] = typeof(via.AABB),
            [RszFieldType.Guid] = typeof(Guid),
            [RszFieldType.GameObjectRef] = typeof(Guid),
            [RszFieldType.Uri] = typeof(Guid),
            [RszFieldType.Color] = typeof(via.Color),
            [RszFieldType.Range] = typeof(via.Range),
            [RszFieldType.RangeI] = typeof(via.RangeI),
            [RszFieldType.Quaternion] = typeof(Quaternion),
            [RszFieldType.Sphere] = typeof(via.Sphere),
            [RszFieldType.Capsule] = typeof(via.Capsule),
            [RszFieldType.Area] = typeof(via.Area),
            [RszFieldType.TaperedCapsule] = typeof(via.TaperedCapsule),
            [RszFieldType.Cone] = typeof(via.Cone),
            [RszFieldType.Line] = typeof(via.Line),
            [RszFieldType.LineSegment] = typeof(via.LineSegment),
            [RszFieldType.Plane] = typeof(via.Plane),
            [RszFieldType.PlaneXZ] = typeof(via.PlaneXZ),
            [RszFieldType.Size] = typeof(via.Size),
            [RszFieldType.Ray] = typeof(via.Ray),
            [RszFieldType.RayY] = typeof(via.RayY),
            [RszFieldType.Segment] = typeof(via.Segment),
            [RszFieldType.Triangle] = typeof(via.Triangle),
            [RszFieldType.Cylinder] = typeof(via.Cylinder),
            [RszFieldType.Ellipsoid] = typeof(via.Ellipsoid),
            [RszFieldType.Torus] = typeof(via.Torus),
            [RszFieldType.Rect] = typeof(via.Rect),
            [RszFieldType.Rect3D] = typeof(via.Rect3D),
            [RszFieldType.Frustum] = typeof(via.Frustum),
            [RszFieldType.KeyFrame] = typeof(via.KeyFrame),
            [RszFieldType.Sfix] = typeof(via.sfix),
            [RszFieldType.Sfix2] = typeof(via.Sfix2),
            [RszFieldType.Sfix3] = typeof(via.Sfix3),
            [RszFieldType.Sfix4] = typeof(via.Sfix4),
            [RszFieldType.Enum] = typeof(uint),
        };

        public static Type RszFieldTypeToCSharpType(RszFieldType fieldType)
        {
            if (!RszFieldTypeToCSharpTypeDict.TryGetValue(fieldType, out Type? type))
            {
                throw new NotSupportedException($"Not support type {fieldType}");
            }
            return type;
        }

        private static readonly Dictionary<Type, RszFieldType> CSharpTypeToRszFieldTypeDict = new()
        {
        };

        public static RszFieldType CSharpTypeToRszFieldType(Type type)
        {
            RszFieldType fieldType = RszFieldType.ukn_type;
            CSharpTypeToRszFieldTypeDict.TryGetValue(type, out fieldType);
            return fieldType;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        protected override bool DoWrite(FileHandler handler)
        {
            // if has RSZUserData, it is external
            if (RSZUserData != null || RszClass.fields.Length == 0) return true;
            AlignFirstField(handler);
            // Console.WriteLine($"write {Name} at: {(handler.Offset + handler.Tell()):X}");
            for (int i = 0; i < RszClass.fields.Length; i++)
            {
                WriteRszField(handler, i);
            }
            return true;
        }

        /// <summary>
        /// 写入字段
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool WriteRszField(FileHandler handler, int index)
        {
            RszField field = RszClass.fields[index];
            handler.Align(field.array ? 4 : field.align);
            // Console.WriteLine($"    write at: {handler.Position:X} {field.original_type} {field.name}");
            if (field.array)
            {
                IList<object> list = (IList<object>)Values[index];
                handler.Write(list.Count);
                if (list.Count > 0)
                {
                    handler.Align(field.align);
                    foreach (var value in list)
                    {
                        if (field.IsString)
                        {
                            handler.Align(4);
                        }
                        WriteNormalField(handler, field, value);
                    }
                }
                return true;
            }
            else
            {
                return WriteNormalField(handler, field, Values[index]);
            }
        }

        /// <summary>
        /// 获取字段值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object? GetFieldValue(string name)
        {
            int index = RszClass.IndexOfField(name);
            if (index == -1) return null;
            return Values[index];
        }

        public void SetFieldValue(int index, object value)
        {
            Values[index] = value;
            // TODO ValueChangedEvent
        }

        /// <summary>
        /// 设置字段值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public bool SetFieldValue(string name, object value)
        {
            int index = RszClass.IndexOfField(name);
            if (index == -1) return false;
            Values[index] = value;
            return true;
        }

        /// <summary>
        /// 避免被引用多次的instance拷贝多次
        /// 每次拷贝会话，比如ImportGameObject后应该清空
        /// </summary>
        public static Dictionary<RszInstance, RszInstance> CloneCache { get; } = new();

        public static void CleanCloneCache()
        {
            CloneCache.Clear();
        }

        /// <summary>
        /// 拷贝自身，如果字段值是RszInstance，则递归拷贝
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return CloneImpl(false);
        }

        /// <summary>
        /// 拷贝自身，如果字段值是RszInstance，则递归拷贝
        /// 同一个Instance引用只会拷贝一次
        /// </summary>
        /// <returns></returns>
        public RszInstance CloneCached()
        {
            return CloneImpl(true);
        }

        private RszInstance CloneImpl(bool cached)
        {
            if (this == NULL)
            {
                // do not clone NULL
                return NULL;
            }
            if (cached && CloneCache.TryGetValue(this, out RszInstance? copy)) return copy;
            IRSZUserDataInfo? userData = RSZUserData != null ? (IRSZUserDataInfo)RSZUserData.Clone() : null;
            copy = new(RszClass, -1, userData);
            if (cached) CloneCache[this] = copy;
            copy.CopyValuesFrom(this, cached);
            return copy;
        }

        /// <summary>
        /// 拷贝字段值
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CopyValuesFrom(RszInstance other, bool cached = false)
        {
            if (other.RszClass != RszClass) return false;
            if (RSZUserData == null)
            {
                var fields = RszClass.fields;
                for (int i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];
                    if (field.array)
                    {
                        var items = new List<object>();
                        var otherItems = (List<object>)other.Values[i];
                        Values[i] = items;
                        if (field.IsReference)
                        {
                            for (int j = 0; j < otherItems.Count; j++)
                            {
                                items.Add(((RszInstance)otherItems[j]).CloneImpl(cached));
                            }
                        }
                        else
                        {
                            for (int j = 0; j < otherItems.Count; j++)
                            {
                                items.Add(CloneValueType(otherItems[j]));
                            }
                        }
                    }
                    else if (field.IsReference)
                    {
                        Values[i] = ((RszInstance)other.Values[i]).CloneImpl(cached);
                    }
                    else
                    {
                        Values[i] = CloneValueType(other.Values[i]);
                    }
                }
                ValuesChanged?.Invoke(this);
            }
            return true;
        }

        private static System.Reflection.MethodInfo? memberwiseClone;

        private static object CloneValueType(object value)
        {
            Type type = value.GetType();
            if (type.IsValueType && !type.IsPrimitive && !type.IsEnum)
            {
                memberwiseClone ??= typeof(object).GetMethod("MemberwiseClone",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (memberwiseClone != null)
                {
                    var newObject = memberwiseClone.Invoke(value, null);
                    if (newObject != null) return newObject;
                }
            }
            return value;
        }

        /// <summary>
        /// 遍历所有子实例
        /// </summary>
        /// <param name="paths">记录当前instance父实例路径</param>
        /// <returns></returns>
        public IEnumerable<RszInstance> Flatten(RszInstanceFlattenArgs? args = null)
        {
            if (RSZUserData == null)
            {
                var fields = RszClass.fields;
                for (int i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];
                    if (field.IsReference)
                    {
                        if (args?.Paths != null)
                        {
                            args.Paths.Add(new(this, field));
                        }
                        if (field.array)
                        {
                            var items = (List<object>)Values[i];
                            for (int j = 0; j < items.Count; j++)
                            {
                                if (items[j] is RszInstance instanceValue)
                                {
                                    if (args?.Predicate != null && !args.Predicate(instanceValue))
                                    {
                                        continue;
                                    }
                                    foreach (var item in instanceValue.Flatten(args))
                                    {
                                        yield return item;
                                    }
                                }
                                else
                                {
                                    throw new InvalidOperationException("Instance should unflatten first");
                                }
                            }
                        }
                        else if (Values[i] is RszInstance instanceValue)
                        {
                            if (args?.Predicate != null && !args.Predicate(instanceValue))
                            {
                                continue;
                            }
                            foreach (var item in instanceValue.Flatten(args))
                            {
                                yield return item;
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("Instance should unflatten first");
                        }
                        if (args?.Paths != null)
                        {
                            args.Paths.RemoveAt(args.Paths.Count - 1);
                        }
                    }
                }
            }
            yield return this;
        }

        public void Stringify(StringBuilder sb, IList<RszInstance>? instances = null, int indent = 0)
        {
            if (RszClass.crc == 0)
            {
                sb.Append("NULL");
                return;
            }
            sb.Append(Name);
            sb.AppendLine(" {");

            void ValueStringify(RszField field, object value)
            {
                if (field.IsReference)
                {
                    if (field.array)
                    {
                        sb.AppendLine();
                        sb.AppendIndent(indent + 2);
                    }
                    if (value is int index && instances != null)
                    {
                        instances[(int)value].Stringify(sb, instances, indent + 2);
                    }
                    else if (value is RszInstance instance)
                    {
                        instance.Stringify(sb, instances, indent + 2);
                    }
                    else
                    {
                        sb.Append(value);
                    }
                }
                else
                {
                    sb.Append(value);
                }
            }

            if (RSZUserData != null)
            {
                if (RSZUserData is RSZUserDataInfo info)
                {
                    sb.AppendIndent(indent + 1);
                    sb.AppendLine($"RSZUserDataPath = {info.Path}");
                }
            }
            else
            {
                for (int i = 0; i < RszClass.fields.Length; i++)
                {
                    RszField field = RszClass.fields[i];
                    string type = field.DisplayType;
                    sb.AppendIndent(indent + 1);
                    sb.Append($"{type} {field.name} = ");
                    if (field.array)
                    {
                        sb.Append('[');
                        var items = (List<object>)Values[i];
                        if (items.Count > 0)
                        {
                            foreach (var item in items)
                            {
                                ValueStringify(field, item);
                                sb.Append(", ");
                            }
                            sb.Length -= 2;
                        }
                        sb.AppendLine("];");
                    }
                    else
                    {
                        ValueStringify(field, Values[i]);
                        sb.AppendLine(";");
                    }
                }
            }
            sb.AppendIndent(indent);
            sb.Append('}');
        }

        public string Stringify(IList<RszInstance>? instances = null, int indent = 0)
        {
            StringBuilder sb = new();
            Stringify(sb, instances, indent);
            return sb.ToString();
        }

        /// <summary>
        /// 创建RszInstance
        /// </summary>
        /// <param name="rszParser">RszParser</param>
        /// <param name="rszClass">Rsz类型</param>
        /// <param name="index">instance序号</param>
        /// <param name="createChildren">创建子对象</param>
        /// <returns></returns>
        public static RszInstance CreateInstance(RszParser rszParser, RszClass rszClass, int index = -1, bool createChildren = true)
        {
            RszInstance instance = new(rszClass, index);
            var fields = instance.Fields;
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.array)
                {
                    instance.Values[i] = new List<object>();
                }
                else if (field.IsReference)
                {
                    if (createChildren)
                    {
                        RszClass? fieldClass = rszParser.GetRSZClass(field.original_type) ??
                            throw new Exception($"RszClass {field.original_type} not found!");
                        instance.Values[i] = CreateInstance(rszParser, fieldClass, -1, createChildren) ??
                            throw new NullReferenceException($"Can not create RszInstance of type {fieldClass.name}");
                    }
                }
                else
                {
                    instance.Values[i] = CreateNormalObject(field);
                }
            }
            return instance;
        }

        /// <summary>
        /// 创建数组元素
        /// </summary>
        /// <param name="rszParser"></param>
        /// <param name="field"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static object? CreateArrayItem(RszParser rszParser, RszField field, string? className = null)
        {
            if (field.type == RszFieldType.Object)
            {
                className ??= GetElementType(field.original_type);
                var rszClass = rszParser.GetRSZClass(className) ??
                    throw new Exception($"RszClass {className} not found!");
                return CreateInstance(rszParser, rszClass);
            }
            else
            {
                return CreateNormalObject(field);
            }
        }

        /// <summary>
        /// 创建普通对象
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static object CreateNormalObject(RszField field)
        {
            if (field.type == RszFieldType.Data)
            {
                return new byte[field.size];
            }
            else if (field.IsString)
            {
                return "";
            }
            var type = RszFieldTypeToCSharpType(field.type);
            return Activator.CreateInstance(type) ??
                throw new NullReferenceException($"Can not create instance of type {type.Name}");
        }

        /// <summary>
        /// 根据数组类型获取元素类型
        /// </summary>
        /// <param name="arrayType"></param>
        /// <returns></returns>
        public static string GetElementType(string arrayType)
        {
            if (arrayType.EndsWith("[]"))
            {
                return arrayType[..^2];
            }
            const string listPrefix = "System.Collections.Generic.List`1<";
            if (arrayType.StartsWith(listPrefix))
            {
                return arrayType[listPrefix.Length..^1];
            }
            return arrayType;
        }
    }


    public record RszInstanceFieldRecord(RszInstance Instance, RszField Field)
    {
        public override string ToString()
        {
            return $"{Instance.Name}.{Field.name}";
        }
    }


    public class RszInstanceFlattenArgs
    {
        public List<RszInstanceFieldRecord>? Paths { get; set; }
        public Predicate<RszInstance>? Predicate { get; set; }
    }
}
