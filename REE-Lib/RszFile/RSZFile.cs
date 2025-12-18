using System.Text;
using ReeLib.InternalAttributes;

namespace ReeLib
{
    public partial class RSZFile : BaseRszFile
    {
        [RszGenerate, RszAutoReadWrite]
        public partial class RszHeader : BaseModel
        {
            public uint magic;
            public uint version;
            public int objectCount;
            public int instanceCount;
            [RszConditional(nameof(version), '>', 3)]
            public long userdataCount;
            public long instanceOffset;
            public long dataOffset;
            [RszConditional(nameof(version), '>', 3)]
            public long userdataOffset;
        }

        public record struct ObjectTableEntry(int InstanceId);

        public RszHeader Header { get; } = new();
        public List<ObjectTableEntry> ObjectTableList { get; } = new();
        public List<InstanceInfo> InstanceInfoList { get; } = new();
        public List<IRSZUserDataInfo> RSZUserDataInfoList { get; } = new();

        /// <summary>
        /// List of object instances.
        /// The first element should usually be NULL.
        /// </summary>
        public List<RszInstance> InstanceList { get; } = new();
        /// <summary>
        /// List of instances that are accessed from outside of the RSZ file.
        /// All elements are also within InstanceList
        /// </summary>
        public List<RszInstance> ObjectList { get; private set; } = new();
        public List<RSZFile>? EmbeddedRSZFileList { get; set; }

        public RSZFile(RszFileOption option, FileHandler fileHandler) : base(option, fileHandler)
        {
            Header.version = option.Version == GameVersion.re7 ? 3u
                : option.Version <= GameVersion.dmc5 ? 8u
                : 16u;
        }

        public const uint Magic = 0x5a5352;

        public override RSZFile GetRSZ() => this;

        protected override bool DoRead()
        {
            ObjectTableList.Clear();
            InstanceInfoList.Clear();
            RSZUserDataInfoList.Clear();
            InstanceList.Clear();
            ObjectList.Clear();
            EmbeddedRSZFileList?.Clear();

            var handler = FileHandler;
            handler.Seek(0);
            if (!Header.Read(handler)) return false;
            if (Header.magic != Magic)
            {
                throw new InvalidDataException($"Not a RSZ data");
            }

            var rszParser = RszParser;

            ObjectTableList.EnsureCapacity(Header.objectCount);
            for (int i = 0; i < Header.objectCount; ++i) ObjectTableList.Add(handler.Read<ObjectTableEntry>());

            handler.Seek(Header.instanceOffset);
            for (int i = 0; i < Header.instanceCount; i++)
            {
                InstanceInfo instanceInfo = new(Option.Version);
                instanceInfo.Read(handler);
                instanceInfo.ReadClassName(rszParser);
                InstanceInfoList.Add(instanceInfo);
            }

            handler.Seek(Header.userdataOffset);
            Dictionary<int, int> instanceIdToUserData = new();
            if (Header.version < 16)
            {
                if (Header.userdataCount > 0)
                {
                    EmbeddedRSZFileList = new();
                    for (int i = 0; i < (int)Header.userdataCount; i++)
                    {
                        RSZUserDataInfo_TDB_LE_67 rszUserDataInfo = new();
                        rszUserDataInfo.Read(handler);
                        rszUserDataInfo.ReadClassName(rszParser);
                        RSZUserDataInfoList.Add(rszUserDataInfo);
                        instanceIdToUserData[rszUserDataInfo.instanceId] = i;

                        long pos = handler.Tell();
                        RSZFile embeddedRSZFile = new(Option, handler.WithOffset(rszUserDataInfo.RSZOffset));
                        embeddedRSZFile.Read();
                        EmbeddedRSZFileList.Add(embeddedRSZFile);
                        rszUserDataInfo.EmbeddedRSZ = embeddedRSZFile;
                        handler.Seek(pos);
                    }
                }
            }
            else
            {
                for (int i = 0; i < (int)Header.userdataCount; i++)
                {
                    RSZUserDataInfo rszUserDataInfo = new();
                    rszUserDataInfo.Read(handler);
                    RSZUserDataInfoList.Add(rszUserDataInfo);
                    instanceIdToUserData[rszUserDataInfo.instanceId] = i;
                }
            }

            // read instance data
            handler.Seek(Header.dataOffset);
            for (int i = 0; i < InstanceInfoList.Count; i++)
            {
                var typeId = InstanceInfoList[i].typeId;
                if (typeId == 0)
                {
                    InstanceList.Add(RszInstance.NULL);
                    continue;
                }
                RszClass? rszClass = RszParser.GetRSZClass(typeId);
                if (rszClass == null)
                {
                    throw new Exception($"RszClass {typeId} not found!");
                }
                if (!instanceIdToUserData.TryGetValue(i, out int userDataIdx))
                {
                    userDataIdx = -1;
                }
                RszInstance instance = new(rszClass, i, userDataIdx != -1 ?
                    RSZUserDataInfoList[userDataIdx] : null);
                if (instance.RSZUserData == null)
                {
                    instance.Read(handler);
                }
                InstanceList.Add(instance);
            }

            // 正常的数据，被依赖的实例排在前面，但为了兼容手动改过的数据，还是先读取数据，再Unflatten
            InstanceListUnflatten();

            for (int i = 0; i < ObjectTableList.Count; i++)
            {
                var item = ObjectTableList[i];
                var instance = InstanceList[item.InstanceId];
                instance.ObjectTableIndex = i;
                ObjectList.Add(instance);
            }
            return true;
        }

        protected override bool DoWrite()
        {
            if (StructChanged)
            {
                RebuildInstanceInfo();
            }
            FileHandler handler = FileHandler;
            var header = Header;
            header.Write(handler);
            foreach (var item in ObjectTableList) handler.Write(item);

            handler.Align(16);
            header.instanceOffset = handler.Tell();
            InstanceInfoList.Write(handler);

            handler.Align(16);
            header.userdataOffset = handler.Tell();
            RSZUserDataInfoList.Write(handler);

            handler.Align(16);
            handler.StringTableFlush();

            if (EmbeddedRSZFileList != null)
            {
                for (int i = 0; i < RSZUserDataInfoList.Count; i++)
                {
                    handler.Align(16);
                    var item = (RSZUserDataInfo_TDB_LE_67)RSZUserDataInfoList[i];
                    item.RSZOffset = handler.Tell();
                    var embeddedRSZ = item.EmbeddedRSZ ?? EmbeddedRSZFileList[i];
                    if (!EmbeddedRSZFileList.Contains(embeddedRSZ)) {
                        EmbeddedRSZFileList.Add(embeddedRSZ);
                    }
                    embeddedRSZ.WriteTo(handler.WithOffset(item.RSZOffset), false);
                    // rewrite
                    item.dataSize = (uint)embeddedRSZ.Size;
                    item.Rewrite(handler);
                }
            }

            handler.Align(16);
            header.dataOffset = handler.Tell();
            InstanceList.Write(handler);

            header.magic = Magic;
            header.objectCount = ObjectTableList.Count;
            header.instanceCount = InstanceList.Count;
            header.userdataCount = RSZUserDataInfoList.Count;
            Header.Write(handler, 0);
            return true;
        }

        // 下面的函数用于重新构建数据，基本读写功能都在上面

        public void ClearInstances()
        {
            InstanceList.Clear();
            InstanceList.Add(RszInstance.NULL);
        }

        public void ClearObjects()
        {
            foreach (var obj in ObjectList) {
                obj.ObjectTableIndex = -1;
            }
            ObjectTableList.Clear();
            ObjectList.Clear();
        }

        public string InstanceStringify(RszInstance instance)
        {
            StringBuilder sb = new();
            sb.AppendLine(instance.Stringify(InstanceList));
            return sb.ToString();
        }

        public string ObjectsStringify()
        {
            StringBuilder sb = new();
            foreach (var instance in ObjectList)
            {
                sb.AppendLine(instance.Stringify(InstanceList));
            }
            return sb.ToString();
        }

        private void FixObjectTableInstanceId(RszInstance instance)
        {
            if (instance.ObjectTableIndex >= 0 && instance.ObjectTableIndex < ObjectTableList.Count)
            {
                ObjectTableList[instance.ObjectTableIndex] = new(instance.Index);
            }
        }

        /// <summary>
        /// Insert a new instance into the RSZFile.
        /// </summary>
        public void InsertInstance(RszInstance instance)
        {
            var list = InstanceList;
            if (instance.Index != -1) return;

            instance.Index = list.Count;
            list.Add(instance);
        }

        /// <summary>
        /// Insert a list of new instances into the RSZFile.
        /// </summary>
        public void InsertInstances(IEnumerable<RszInstance> instances)
        {
            foreach (var instance in instances) InsertInstance(instance);
        }

        /// <summary>
        /// Reconstructs the full instance list based on the given list including nested instances.
        /// Unreferenced / dangling instances will be discarded.
        /// </summary>
        public void RebuildInstanceList(IEnumerable<RszInstance> srcList)
        {
            ClearInstances();
            var list = InstanceList;
            foreach (var instance in srcList)
            {
                foreach (var item in instance.GetChildren(x => x.Index != -1))
                {
                    if (item.RszClass == RszClass.Empty) continue;
                    item.Index = -1;
                }
            }
            foreach (var instance in srcList)
            {
                if (instance.Index != -1) continue;
                foreach (var item in instance.GetChildren())
                {
                    if (item.Index < 0 || item.Index >= list.Count || list[item.Index] != item)
                    {
                        item.Index = list.Count;
                        list.Add(item);
                        FixObjectTableInstanceId(item);
                    }
                }
            }

            if (EmbeddedRSZFileList != null)
            {
                foreach (var innerRsz in EmbeddedRSZFileList)
                {
                    innerRsz.RebuildInstanceList();
                }
            }
        }

        /// <summary>
        /// Reconstructs the full instance list based on the current object list.
        /// Unreferenced / dangling instances will be discarded.
        /// </summary>
        public void RebuildInstanceList()
        {
            RebuildInstanceList(ObjectList);
        }

        /// <summary>
        /// Rebuild the object's hierarchy. Any instance indexes will be resolved to their RszInstance objects.
        /// </summary>
        /// <param name="instance"></param>
        public void InstanceUnflatten(RszInstance instance)
        {
            if (instance.RSZUserData != null) return;
            var fields = instance.RszClass.fields;
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.IsReference)
                {
                    if (field.array)
                    {
                        var items = (List<object>)instance.Values[i];
                        for (int j = 0; j < items.Count; j++)
                        {
                            if (items[j] is int instanceId)
                            {
                                if (instanceId >= instance.Index && field.IsTypeInferred)
                                {
                                    // TODO: may detect error, should roll back
                                    field.type = RszFieldType.S32;
                                    throw new RszRetryOpenException($"Detected {instance.RszClass.name}.{field.name} as Object before, but seems wrong");
                                }
                                items[j] = InstanceList[instanceId];
                                InstanceUnflatten(InstanceList[instanceId]);
                            }
                        }
                    }
                    else if (instance.Values[i] is int instanceId)
                    {
                        if ((instanceId < 0 || instanceId >= instance.Index) && field.IsTypeInferred)
                        {
                            // TODO: may detect error, should roll back
                            field.type = RszFieldType.S32;
                            throw new RszRetryOpenException($"Detected {instance.RszClass.name}.{field.name} as Object before, but seems wrong");
                        }
                        instance.Values[i] = InstanceList[instanceId];
                        InstanceUnflatten(InstanceList[instanceId]);
                    }
                }
            }
        }

        /// <summary>
        /// 所有实例的字段值，如果是对象序号，替换成对应的实例对象
        /// </summary>
        public void InstanceListUnflatten()
        {
            foreach (var instance in InstanceList)
            {
                InstanceUnflatten(instance);
            }
        }

        /// <summary>
        /// 所有实例的字段值，如果是对象序号，替换成对应的实例对象
        /// </summary>
        public void InstanceListUnflatten(IEnumerable<RszInstance> list)
        {
            foreach (var instance in list)
            {
                InstanceUnflatten(instance);
            }
        }

        /// <summary>
        /// 根据实例列表，重建InstanceInfo
        /// </summary>
        /// <param name="flatten">是否先进行flatten</param>
        public void RebuildInstanceInfo(bool rebuildInstances = true, bool rebuildObjectTable = true)
        {
            if (rebuildInstances)
            {
                RebuildInstanceList();
            }
            InstanceInfoList.Clear();
            RSZUserDataInfoList.Clear();
            foreach (var instance in InstanceList)
            {
                InstanceInfoList.Add(new InstanceInfo(Option.Version)
                {
                    typeId = instance.RszClass.typeId,
                    CRC = instance.RszClass.crc
                });
                if (instance.RSZUserData != null)
                {
                    RSZUserDataInfoList.Add(instance.RSZUserData);
                }
            }
            if (rebuildObjectTable)
            {
                // Rebuild ObjectTableList
                ObjectTableList.Clear();
                List<RszInstance> newObjectList = new();
                foreach (var instance in ObjectList)
                {
                    if (InstanceList.Contains(instance))
                    {
                        instance.ObjectTableIndex = ObjectTableList.Count;
                        ObjectTableList.Add(new ObjectTableEntry(instance.Index));
                        newObjectList.Add(instance);
                    }
                }
                ObjectList = newObjectList;
            }

            if (EmbeddedRSZFileList != null)
            {
                foreach (var innerRsz in EmbeddedRSZFileList)
                {
                    innerRsz.RebuildInstanceInfo(rebuildInstances, rebuildObjectTable);
                }
            }

            StructChanged = false;
        }

        /// <summary>
        /// 添加到ObjectTable，会自动修正instance.ObjectTableIndex
        /// </summary>
        /// <param name="instance"></param>
        public void AddToObjectTable(RszInstance instance)
        {
            if (instance.ObjectTableIndex >= 0 && instance.ObjectTableIndex < ObjectTableList.Count &&
                ObjectTableList[instance.ObjectTableIndex].InstanceId == instance.Index)
            {
                return;
            }
            if (instance.Index == -1) InsertInstance(instance);
            instance.ObjectTableIndex = ObjectTableList.Count;
            ObjectTableList.Add(new ObjectTableEntry(instance.Index));
            ObjectList.Add(instance);
        }

        /// <summary>
        /// Creates a new RszInstance and adds it to the file.
        /// </summary>
        /// <param name="className"></param>
        public RszInstance CreateInstance(string className)
        {
            var rszClass = RszParser.GetRSZClass(className) ??
                throw new Exception($"RszClass {className} not found!");
            RszInstance instance = RszInstance.CreateInstance(RszParser, rszClass);
            InsertInstances(instance.GetChildren());
            StructChanged = true;
            return instance;
        }
    }


    public class InstanceInfo : BaseModel
    {
        public uint typeId;
        public uint CRC;
        public uint userPathHashRe7;
        public string? ClassName { get; set; }

        public GameVersion Version;

        public InstanceInfo()
        {
            Version = GameVersion.unknown;
        }

        public InstanceInfo(GameVersion version)
        {
            Version = version;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref typeId);
            handler.Read(ref CRC);

            if (Version == GameVersion.re7) {
                handler.Read(ref userPathHashRe7);
                handler.Skip(4);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(typeId);
            handler.Write(CRC);

            if (Version == GameVersion.re7) {
                handler.Write(ref userPathHashRe7);
                handler.Skip(4);
            }
            return true;
        }

        public void ReadClassName(RszParser parser)
        {
            ClassName = parser.GetRSZClassName(typeId);
        }

        public override string ToString() => ClassName ?? CRC.ToString();
    }


    public interface IRSZUserDataInfo : IModel, ICloneable
    {
        int InstanceId { get; set; }
        uint TypeId { get; }
        string? ClassName { get; }
    }


    public class RSZUserDataInfo : BaseModel, IRSZUserDataInfo
    {
        public int instanceId;
        public uint typeId;
        public ulong pathOffset;
        public string? Path { get; set; }

        public int InstanceId { get => instanceId; set => instanceId = value; }
        public uint TypeId => typeId;
        public string? ClassName { get; set; }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref instanceId);
            handler.Read(ref typeId);
            handler.Read(ref pathOffset);
            Path = handler.ReadWString((long)pathOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(instanceId);
            handler.Write(typeId);
            handler.StringTableAdd(Path);
            handler.Write(pathOffset);
            return true;
        }

        public string ReadClassName(RszParser parser)
        {
            return ClassName = parser.GetRSZClassName(typeId);
        }

        public UserdataInfo ToUserdataInfo(RszParser parser)
        {
            UserdataInfo info = new()
            {
                typeId = typeId,
                Path = Path,
                // CRC = parser.GetRSZClassCRC(typeId),
            };
            return info;
        }
    }


    public class RSZUserDataInfo_TDB_LE_67 : BaseModel, IRSZUserDataInfo
    {
        public int instanceId;
        public uint typeId;
        public uint jsonPathHash;
        public uint dataSize;
        public long RSZOffset;

        public int InstanceId { get => instanceId; set => instanceId = value; }
        public uint TypeId => typeId;
        public string? ClassName { get; set; }
        public RSZFile? EmbeddedRSZ { get; set; }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref instanceId);
            handler.Read(ref typeId);
            handler.Read(ref jsonPathHash);
            handler.Read(ref dataSize);
            handler.Read(ref RSZOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(instanceId);
            handler.Write(typeId);
            handler.Write(jsonPathHash);
            handler.Write(dataSize);
            handler.Write(RSZOffset);
            return true;
        }

        public string ReadClassName(RszParser parser)
        {
            return ClassName = parser.GetRSZClassName(typeId);
        }
    }
}
