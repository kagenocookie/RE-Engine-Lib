using System.Numerics;
using System.Runtime.CompilerServices;
using ReeLib.Common;
using ReeLib.Rcol;
using ReeLib.via;


namespace ReeLib.Rcol
{
    public class Header : ReadWriteModel
    {
        public uint magic = RcolFile.Magic;
        public int numGroups;
        public int numShapes;
        public int uknCount;
        public int numUserData;
        public int numRequestSets;
        public uint maxRequestSetId;

        public int numIgnoreTags;
        public int numAutoGenerateJoints;

        public int userDataSize;
        public int status;

        public ulong uknRe3_A;
        public ulong uknRe3_B;

        public uint ukn1;
        public uint ukn2;

        public long groupsPtrOffset;
        public long dataOffset;
        public long requestSetOffset;
        public long ignoreTagOffset;
        public long requestSetIDLookupsOffset; //rcol.2
        public long autoGenerateJointDescOffset;
        public long unknPtr0;
        public long unknPtr1;
        public ulong uknRe3;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            var version = action.Handler.FileVersion;
            action.Do(ref magic);
            if (version == 2)
            {
                action.Do(ref Unsafe.As<int, byte>(ref numGroups));
                action.Do(ref Unsafe.As<int, byte>(ref numShapes));
                action.Do(ref Unsafe.As<int, short>(ref uknCount));
                action.Do(ref Unsafe.As<int, short>(ref numRequestSets));
                action.Do(ref Unsafe.As<uint, ushort>(ref maxRequestSetId));
            }
            else
            {
                action.Do(ref numGroups);
                if (version >= 25) {
                    action.Do(ref numUserData);
                    action.Do(ref uknCount);
                    numShapes = 0;
                } else {
                    action.Do(ref numShapes);
                    action.Do(ref numUserData);
                }
                action.Do(ref numRequestSets);
                action.Do(ref maxRequestSetId);
            }
            if (version > 11)
            {
                action.Do(ref numIgnoreTags);
                action.Do(ref numAutoGenerateJoints);
            }
            action.Do(ref userDataSize);
            action.Do(ref status);
            // if (version == 2) action.Skip(4);
            if (version == 2) action.Null(4);
            if (version == 11)
            {
                action.Do(ref uknRe3_A);
                action.Do(ref uknRe3_B);
            }
            if (version >= 20)
            {
                action.Do(ref ukn1);
                action.Do(ref ukn2);
            }
            action.Do(ref groupsPtrOffset);
            action.Do(ref dataOffset);
            action.Do(ref requestSetOffset);
            if (version > 11)
            {
                action.Do(ref ignoreTagOffset);
                action.Do(ref autoGenerateJointDescOffset);
            }
            else if (version == 2)
            {
                action.Do(ref requestSetIDLookupsOffset);
            }
            else if (version == 11)
            {
                action.Do(ref uknRe3);
            }
            if (version >= 20) {
                action.Do(ref unknPtr0);
                action.Do(ref unknPtr1);
            }
            return true;
        }
    }

    public class GroupInfo : BaseModel
    {
        public Guid guid;
        public string Name = string.Empty;
        public uint NameHash;

        public int UserDataIndex;
        public int NumExtraShapes;
        public int NumShapes;

        public int NumMaskGuids;
        public long ShapesOffset;
        public int LayerIndex;
        public uint MaskBits;
        public long MaskGuidsOffset;
        public Guid LayerGuid;

        public List<Guid>? MaskGuids { get; set; }

        public RszInstance? UserData { get; set; }

        public long ShapesOffsetStart;

        private void ReadWriteShared<THandler>(THandler action) where THandler : IFileHandlerAction
        {
            var version = action.Handler.FileVersion;

            action.Do(ref guid);
            action.HandleOffsetWString(ref Name);
            action.Do(ref NameHash);
            if (version >= 25)
            {
                action.Do(ref NumShapes);
                // verified: for 27, this is extra (mirror) shape count and not userdata. need to recheck v25 and pre-25
                action.Do(ref NumExtraShapes);
                action.Do(ref NumMaskGuids);
            }
            else if (version > 2)
            {
                action.Do(ref UserDataIndex);
                action.Do(ref NumShapes);
                action.Do(ref NumMaskGuids);
            }
            else // RE7
            {
                action.Do(ref Unsafe.As<int, short>(ref UserDataIndex));
                action.Do(ref Unsafe.As<int, short>(ref NumShapes));
            }
        }

        protected override bool DoRead(FileHandler handler)
        {
            ReadWriteShared(new FileHandlerRead(handler));

            handler.Read(ref ShapesOffset);
            handler.Read(ref LayerIndex);
            handler.Read(ref MaskBits);

            MaskGuids?.Clear();
            if (handler.FileVersion > 2) {
                handler.Read(ref MaskGuidsOffset);
                handler.Read(ref LayerGuid);
                using var defer = handler.SeekJumpBack(MaskGuidsOffset);
                {
                    MaskGuids ??= new();
                    handler.ReadList(MaskGuids, NumMaskGuids);
                }
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            NameHash = MurMur3HashUtils.GetHash(Name ??= string.Empty);
            UserDataIndex = UserData?.Index ?? UserDataIndex;
            NumMaskGuids = MaskGuids?.Count ?? 0;
            ReadWriteShared(new FileHandlerWrite(handler));

            ShapesOffsetStart = handler.Tell();
            handler.Write(ref ShapesOffset);
            handler.Write(ref LayerIndex);
            handler.Write(ref MaskBits);

            if (handler.FileVersion > 2) {
                if (MaskGuids != null)
                {
                    handler.OffsetContentTableAdd(handler => handler.WriteList(MaskGuids));
                }
                handler.Write(ref MaskGuidsOffset);
                handler.Write(ref LayerGuid);
            }
            return true;
        }

        public override string ToString() => Name;
    }


    public class RcolGroup : BaseModel
    {
        public GroupInfo Info { get; } = new();
        public List<RcolShape> Shapes { get; } = new();
        public List<RcolShape> ExtraShapes { get; } = new(); // may be >= rcol.25 exclusive

        public bool ReadInfo(FileHandler handler)
        {
            return Info.Read(handler);
        }

        public bool WriteInfo(FileHandler handler)
        {
            Info.NumShapes = Shapes.Count;
            Info.NumExtraShapes = ExtraShapes.Count;
            return Info.Write(handler);
        }

        protected override bool DoRead(FileHandler handler)
        {
            Shapes.Clear();
            // var pos = handler.Tell(); TODO
            if (Info.NumShapes > 0)
            {
                handler.Seek(Info.ShapesOffset);
                Shapes.Read(handler, Info.NumShapes);
            }
            ExtraShapes.Clear();
            if (Info.NumExtraShapes > 0) {
                ExtraShapes.Read(handler, Info.NumExtraShapes);
            }
            // handler.Seek(pos); TODO
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (Info.NumShapes > 0)
            {
                Info.ShapesOffset = handler.Tell();
                if (Info.ShapesOffsetStart > 0)
                {
                    handler.WriteInt64(Info.ShapesOffsetStart, Info.ShapesOffset);
                }
                else
                {
                    throw new InvalidOperationException($"Should WriteInfo first");
                }
                Shapes.Write(handler);
            }
            if (Info.NumExtraShapes > 0)
            {
                ExtraShapes.Write(handler);
            }
            return true;
        }

        public override string ToString()
        {
            return Info.Name;
        }
    }

    public enum ShapeType
    {
        Aabb = 0x0,
        Sphere = 0x1,
        ContinuousSphere = 0x2,
        Capsule = 0x3,
        ContinuousCapsule = 0x4,
        Box = 0x5,
        Mesh = 0x6,
        HeightField = 0x7,
        StaticCompound = 0x8,
        Area = 0x9,
        Triangle = 0xA,
        SkinningMesh = 0xB,
        Cylinder = 0xC,
        DeformableMesh = 0xD,
        Invalid = 0xE,
        Max = 0xF,
    }


    public class RcolShapeInfo : ReadWriteModel
    {
        public Guid Guid;
        public string Name = string.Empty;
        public uint NameHash;
        // NOTE: the "UserDataIndex" for rcol >= 25 seems to be just an IsExtra bool
        public int UserDataIndex;
        public int LayerIndex;
        public int Attribute;
        public uint SkipIdBits;
        public uint IgnoreTagBits;
        public string primaryJointNameStr = string.Empty;
        public string secondaryJointNameStr = string.Empty;
        public uint PrimaryJointNameHash;
        public uint SecondaryJointNameHash;
        public ShapeType shapeType;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            if (action is FileHandlerWrite) {
                PrimaryJointNameHash = MurMur3HashUtils.GetHash(primaryJointNameStr ?? string.Empty);
                SecondaryJointNameHash = MurMur3HashUtils.GetHash(secondaryJointNameStr ?? string.Empty);
                NameHash = MurMur3HashUtils.GetHash(Name ?? string.Empty);
            }

            action.Do(ref Guid);
            action.HandleOffsetWString(ref Name);
            action.Do(ref NameHash);
            action.Do(ref UserDataIndex);
            action.Do(ref LayerIndex);
            action.Do(ref Attribute);

            if (action.Handler.FileVersion >= 27)
            {
                action.Do(ref SkipIdBits);
                action.Do(ref shapeType);
                action.Do(ref IgnoreTagBits);
                action.Skip(4);
                action.HandleOffsetWString(ref primaryJointNameStr);
                action.HandleOffsetWString(ref secondaryJointNameStr);
                action.Do(ref PrimaryJointNameHash);
                action.Do(ref SecondaryJointNameHash);
            }
            else if (action.Handler.FileVersion > 2)
            {
                action.Do(ref SkipIdBits);
                action.Do(ref IgnoreTagBits);
                action.HandleOffsetWString(ref primaryJointNameStr);
                action.HandleOffsetWString(ref secondaryJointNameStr);
                action.Do(ref PrimaryJointNameHash);
                action.Do(ref SecondaryJointNameHash);
                action.Do(ref shapeType);
                action.Skip(4);
            }
            else
            {
                action.HandleOffsetWString(ref primaryJointNameStr);
                action.HandleOffsetWString(ref secondaryJointNameStr);
                action.Do(ref PrimaryJointNameHash);
                action.Do(ref SecondaryJointNameHash);
                action.Do(ref shapeType);
                action.Skip(12);
            }
            return true;
        }
    }


    public class RcolShape : BaseModel
    {
        public RcolShapeInfo Info { get; } = new();
        public object? shape = new AABB(Vector3.Zero, Vector3.One);

        public RszInstance? Instance { get; set; }

        public void UpdateShapeType()
        {
            switch (Info.shapeType) {
                case ShapeType.Aabb: shape = new AABB(new System.Numerics.Vector3(-0.5f), new System.Numerics.Vector3(0.5f)); break;
                case ShapeType.Sphere or ShapeType.ContinuousSphere: shape = new Sphere(); break;
                case ShapeType.Capsule or ShapeType.ContinuousCapsule: shape = new via.Capsule(); break;
                case ShapeType.Box: shape = new via.OBB(); break;
                case ShapeType.Area: shape = new via.Area(); break;
                case ShapeType.Triangle: shape = new via.Triangle(); break;
                case ShapeType.Cylinder: shape = new via.Cylinder(); break;
                default:
                    Log.Error("Unsupported shape for rcol: " + Info.shapeType);
                    break;
            }
        }

        protected override bool DoRead(FileHandler handler)
        {
            Info.Read(handler);

            var tell = handler.Tell();
            shape = Info.shapeType switch
            {
                ShapeType.Aabb => handler.Read<via.AABB>(),
                ShapeType.Sphere => handler.Read<via.Sphere>(),
                ShapeType.Capsule => handler.Read<via.Capsule>(),
                ShapeType.Box => handler.Read<via.OBB>(),
                ShapeType.Area => handler.Read<via.Area>(),
                ShapeType.Triangle => handler.Read<via.Triangle>(),
                ShapeType.Cylinder => handler.Read<via.Cylinder>(),
                ShapeType.ContinuousSphere => handler.Read<via.Sphere>(),
                ShapeType.ContinuousCapsule => handler.Read<via.Capsule>(),
                _ => throw new Exception("Unsupported RCOL shape type " + Info.shapeType),
            };
            handler.Seek(tell + sizeof(float) * 4 * 5);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            Info.Write(handler);
            var tell = handler.Tell();
            switch (Info.shapeType)
            {
                case ShapeType.Aabb: handler.Write(tell, (via.AABB)shape!); break;
                case ShapeType.Sphere: handler.Write(tell, (via.Sphere)shape!); break;
                case ShapeType.Capsule: handler.Write(tell, (via.Capsule)shape!); break;
                case ShapeType.Box: handler.Write(tell, (via.OBB)shape!); break;
                case ShapeType.Area: handler.Write(tell, (via.Area)shape!); break;
                case ShapeType.Triangle: handler.Write(tell, (via.Triangle)shape!); break;
                case ShapeType.Cylinder: handler.Write(tell, (via.Cylinder)shape!); break;
                case ShapeType.ContinuousSphere: handler.Write(tell, (via.Sphere)shape!); break;
                case ShapeType.ContinuousCapsule: handler.Write(tell, (via.Capsule)shape!); break;
                default: throw new Exception("Unsupported RCOL shape type " + Info.shapeType);
            }
            handler.Seek(tell + sizeof(float) * 4 * 5);
            return true;
        }

        public override string ToString() => shape == null ? $"{nameof(RcolShape)} [{Instance}]" : $"{shape} [{Instance}]";
    }


    public class RequestSetInfo : ReadWriteModel
    {
        public uint ID;
        public int GroupIndex;
        public int ShapeOffset;
        public int status;

        public int requestSetUserdataIndex; // >= rcol.25
        public int groupUserdataIndexStart; // >= rcol.25
        public int requestSetIndex; // >= rcol.25
        public string Name = string.Empty;
        public uint NameHash;

        public string KeyName = string.Empty;
        public uint KeyHash;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            var version = action.Handler.FileVersion;
            if (version >= 25)
            {
                action.Do(ref ID);
                action.Do(ref GroupIndex);
                action.Do(ref requestSetUserdataIndex);
                action.Do(ref groupUserdataIndexStart);
                action.Do(ref status);
                action.Do(ref requestSetIndex);
                action.HandleOffsetWString(ref Name);
                action.HandleOffsetWString(ref KeyName);
                action.Do(ref NameHash);
                action.Do(ref KeyHash);
            }
            else if (version > 2)
            {
                action.Do(ref ID);
                action.Do(ref GroupIndex);
                action.Do(ref ShapeOffset);
                action.Do(ref status);
                action.HandleOffsetWString(ref Name);
                action.Do(ref NameHash);
                action.Skip(4);
                action.HandleOffsetWString(ref KeyName);
                action.Do(ref KeyHash);
                action.Skip(4);
            }
            else
            {
                action.Do(ref GroupIndex);
                action.Do(ref ShapeOffset);
                action.Do(ref status);
                action.Skip(4);
            }
            return true;
        }

        public override string ToString() => Name;
    }


    public class RequestSet(RequestSetInfo? info = null) : ICloneable
    {
        public RequestSetInfo Info { get; set; } = info ?? new();
        public RcolGroup? Group { get; set; }
        public RszInstance? Instance { get; set; }
        public List<RszInstance> ShapeUserdata { get; set; } = new();

        public override string ToString() => $"[{Info.ID:00000000}] {Info.Name}";

        object ICloneable.Clone() => Clone();

        public RequestSet Clone()
        {
            var clone = new RequestSet((RequestSetInfo)Info.Clone());
            clone.Instance = Instance?.Clone();
            clone.Group = Group;
            clone.ShapeUserdata = new List<RszInstance>(ShapeUserdata);
            return clone;
        }
    }


    public class IgnoreTag : BaseModel
    {
        public string tag = string.Empty;
        public uint hash;
        public uint ukn;

        protected override bool DoRead(FileHandler handler)
        {
            handler.ReadOffsetWString(out tag);
            handler.Read(ref hash);
            handler.Read(ref ukn);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.WriteOffsetWString(tag);
            handler.Write(ref hash);
            handler.Write(ref ukn);
            return true;
        }

        public override string ToString() => tag;
    }
}


namespace ReeLib
{
    public class RcolFile : BaseRszFile
    {
        public Header Header { get; } = new();
        public RSZFile RSZ { get; private set; }
        public List<RcolGroup> Groups { get; } = new();
        public List<RequestSet> RequestSets { get; } = new();
        public List<IgnoreTag> IgnoreTags { get; private set; } = new();
        public List<string>? AutoGenerateJointDescs { get; private set; } = new();

        public const uint Magic = 0x4c4f4352;
        public const string Extension2 = ".rcol";

        public RcolFile(RszFileOption option, FileHandler fileHandler) : base(option, fileHandler)
        {
            RSZ = new RSZFile(option, fileHandler);
        }

        public RequestSet CreateNewRequestSet(string name)
        {
            var set = new RequestSet();
            set.Info.ID = RequestSets.Count == 0 ? 0 : RequestSets.Max(s => s.Info.ID);
            set.Info.Name = string.IsNullOrEmpty(name) ? "NewSet" : name;
            RequestSets.Add(set);
            set.Instance = RszInstance.CreateInstance(Option.RszParser, Option.RszParser.GetRSZClass("via.physics.RequestSetColliderUserData")!);
            RSZ.AddToObjectTable(set.Instance);
            set.Group = new RcolGroup();
            set.Group.Info.guid = Guid.NewGuid();
            set.Group.Info.Name = string.IsNullOrEmpty(name) ? "NewGroup" : name;
            return set;
        }

        public override RSZFile? GetRSZ() => RSZ;

        protected override bool DoRead()
        {
            Groups.Clear();
            RequestSets.Clear();
            IgnoreTags?.Clear();
            AutoGenerateJointDescs?.Clear();
            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a RCOL file");
            }

            if (header.numGroups > 0)
            {
                handler.Seek(header.groupsPtrOffset);
                for (int i = 0; i < header.numGroups; i++)
                {
                    RcolGroup group = new();
                    if (!group.ReadInfo(handler)) return false;
                    Groups.Add(group);
                }
                foreach (var group in Groups)
                {
                    if (!group.Read(handler)) return false;
                }
            }

            ReadRsz(RSZ, header.dataOffset);

            if (header.numRequestSets > 0)
            {
                handler.Seek(header.requestSetOffset);
                for (int i = 0; i < header.numRequestSets; i++)
                {
                    RequestSetInfo requestSetInfo = new();
                    if (!requestSetInfo.Read(handler)) return false;

                    RequestSet requestSet = new(requestSetInfo);
                    RequestSets.Add(requestSet);
                }
            }

            if (header.numIgnoreTags > 0)
            {
                handler.Seek(header.ignoreTagOffset);
                IgnoreTags ??= new();
                IgnoreTags.Read(handler, header.numIgnoreTags);
                DataInterpretationException.DebugThrowIf(IgnoreTags.Any(tag => tag.hash != MurMur3HashUtils.GetHash(tag.tag)));
            }

            if (header.numAutoGenerateJoints > 0)
            {
                handler.Seek(header.autoGenerateJointDescOffset);
                AutoGenerateJointDescs ??= new();
                for (int i = 0; i < header.numAutoGenerateJoints; i++)
                {
                    AutoGenerateJointDescs.Add(handler.ReadOffsetWString());
                }
            }

            if (header.requestSetIDLookupsOffset > 0)
            {
                handler.Seek(header.requestSetIDLookupsOffset);
                for (int i = 0; i < header.maxRequestSetId; ++i)
                {
                    var index = handler.Read<int>();
                    if (index != -1)
                    {
                        RequestSets[index].Info.ID = (uint)i;
                    }
                }
            }

            SetupReferences(handler.FileVersion);

            return true;
        }

        public void SetupReferences(int fileVersion)
        {
            foreach (var group in Groups) {
                group.Info.UserData = group.Info.UserDataIndex == 0 ? null : RSZ.InstanceList[group.Info.UserDataIndex];
                if (fileVersion < 25) {
                    foreach (var shape in group.Shapes) {
                        shape.Instance = shape.Info.UserDataIndex == -1 ? null : RSZ.ObjectList[shape.Info.UserDataIndex];
                    }
                }
            }

            for (var i = 0; i < RequestSets.Count; i++) {
                RequestSet requestSet = RequestSets[i];

                requestSet.Group = Groups[requestSet.Info.GroupIndex];
                if (fileVersion >= 25)
                {
                    requestSet.Instance = RSZ.ObjectList[requestSet.Info.requestSetUserdataIndex];
                    for (int k = 0; k < requestSet.Group.Shapes.Count; ++k)
                    {
                        requestSet.ShapeUserdata.Add(RSZ.ObjectList[requestSet.Info.groupUserdataIndexStart + k]);
                    }
                }
                else if (fileVersion > 2)
                {
                    requestSet.Instance = RSZ.ObjectList[i];
                    for (int k = 0; k < requestSet.Group.Shapes.Count; ++k) {
                        var shape = requestSet.Group.Shapes[k];
                        var instanceId = RSZ.ObjectTableList[shape.Info.UserDataIndex + requestSet.Info.ShapeOffset].InstanceId;
                        requestSet.ShapeUserdata.Add(RSZ.InstanceList[instanceId]);
                    }
                }
                else
                {
                    requestSet.Info.Name = Groups[requestSet.Info.GroupIndex].Info.Name;
                    requestSet.Instance = RSZ.ObjectList[i];
                    for (int k = 0; k < requestSet.Group.Shapes.Count; ++k) {
                        var shape = requestSet.Group.Shapes[k];
                        var instanceId = RSZ.ObjectTableList[shape.Info.UserDataIndex + requestSet.Info.ShapeOffset].InstanceId;
                        requestSet.ShapeUserdata.Add(RSZ.InstanceList[instanceId]);
                    }
                }
            }
        }

        protected override bool DoWrite()
        {
            FileHandler handler = FileHandler;
            handler.Clear();
            var header = Header;

            header.numRequestSets = RequestSets.Count;
            header.numGroups = Groups.Count;
            header.numIgnoreTags = IgnoreTags?.Count ?? 0;
            // uknCount value seems meaningless (rcol.25+), putting something in just in case
            if (header.uknCount == 0) header.uknCount = Groups.Count;
            header.numUserData = 0;
            header.numShapes = 0;
            header.maxRequestSetId = RequestSets.Count == 0 ? uint.MaxValue : (uint)RequestSets.Max(s => s.Info.ID);

            header.Write(handler);
            handler.Align(16);

            header.groupsPtrOffset = handler.Tell();
            foreach (var group in Groups)
            {
                group.WriteInfo(handler);
            }

            foreach (var group in Groups)
            {
                group.Write(handler);
                header.numShapes += group.Shapes.Count;
            }

            handler.Align(16);
            header.dataOffset = handler.Tell();
            WriteRsz(RSZ, header.dataOffset);
            header.userDataSize = (int)RSZ.Size;

            handler.Align(16);
            header.maxRequestSetId = 0;
            header.numRequestSets = RequestSets.Count;
            header.requestSetOffset = handler.Tell();
            foreach (var item in RequestSets)
            {
                item.Info.Write(handler);
                if (item.Info.ID > header.maxRequestSetId) header.maxRequestSetId = item.Info.ID;
            }

            // the field is used as an array size for rcol.2 so it needs to be +1 from the highest one
            if (handler.FileVersion == 2) header.maxRequestSetId++;

            handler.Align(16);
            header.numIgnoreTags = IgnoreTags?.Count ?? 0;
            header.ignoreTagOffset = handler.Tell();
            IgnoreTags?.Write(handler);

            header.numAutoGenerateJoints = AutoGenerateJointDescs?.Count ?? 0;
            header.autoGenerateJointDescOffset = handler.Tell();
            header.unknPtr0 = header.autoGenerateJointDescOffset;
            header.unknPtr1 = header.autoGenerateJointDescOffset;
            if (AutoGenerateJointDescs != null)
            {
                foreach (var item in AutoGenerateJointDescs)
                {
                    handler.WriteOffsetWString(item);
                }
            }

            handler.Align(16);
            if (handler.FileVersion == 2)
            {
                header.requestSetIDLookupsOffset = handler.Tell();
                int index = 0;
                int setIndex = 0;
                foreach (var set in RequestSets.OrderBy(s => s.Info.ID))
                {
                    while (index < set.Info.ID) {
                        index++;
                        handler.Write<int>(-1);
                    }
                    handler.Write(setIndex++);
                    index++;
                }
            }

            handler.StringTableFlush();
            handler.OffsetContentTableFlush();

            if (handler.FileVersion >= 25)
            {
                header.numUserData = RequestSets.Sum(s => s.ShapeUserdata.Count);
            }
            else if (handler.FileVersion > 2)
            {
                int minIndex = int.MaxValue;
                int maxIndex = int.MinValue;
                foreach (var g in Groups) {
                    foreach (var s in g.Shapes) {
                        minIndex = Math.Min(s.Info.UserDataIndex, minIndex);
                        maxIndex = Math.Max(s.Info.UserDataIndex, maxIndex);
                    }
                }
                header.numUserData = maxIndex - minIndex;
            }

            header.magic = Magic;
            header.Write(handler, 0);
            return true;
        }
    }
}
