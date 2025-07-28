using ReeLib.Common;
using ReeLib.Rcol;


namespace ReeLib.Rcol
{
    public class Header : BaseModel
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

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref magic);
            if (handler.FileVersion == 2)
            {
                numGroups = handler.ReadByte();
                numShapes = handler.ReadByte();
                uknCount = handler.ReadShort();
                numRequestSets = handler.ReadShort();
                maxRequestSetId = handler.ReadUShort();
            }
            else
            {
                handler.Read(ref numGroups);
                if (handler.FileVersion >= 25) {
                    handler.Read(ref numUserData);
                } else {
                    handler.Read(ref numShapes);
                }
                handler.Read(ref uknCount);
                handler.Read(ref numRequestSets);
                handler.Read(ref maxRequestSetId);
            }
            if (handler.FileVersion > 11)
            {
                handler.Read(ref numIgnoreTags);
                handler.Read(ref numAutoGenerateJoints);
            }
            handler.Read(ref userDataSize);
            handler.Read(ref status);
            if (handler.FileVersion == 2) handler.Read<int>();
            if (handler.FileVersion == 11)
            {
                handler.Read(ref uknRe3_A);
                handler.Read(ref uknRe3_B);
            }
            if (handler.FileVersion >= 20)
            {
                handler.Read(ref ukn1);
                handler.Read(ref ukn2);
            }
            handler.Read(ref groupsPtrOffset);
            handler.Read(ref dataOffset);
            handler.Read(ref requestSetOffset);
            if (handler.FileVersion > 11)
            {
                handler.Read(ref ignoreTagOffset);
                handler.Read(ref autoGenerateJointDescOffset);
            }
            else if (handler.FileVersion == 2)
            {
                handler.Read(ref requestSetIDLookupsOffset);
            }
            else if (handler.FileVersion == 11)
            {
                handler.Read(ref uknRe3);
            }
            if (handler.FileVersion >= 20) {
                handler.Read(ref unknPtr0);
                handler.Read(ref unknPtr1);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref magic);
            if (handler.FileVersion == 2)
            {
                handler.WriteByte((byte)numGroups);
                handler.WriteByte((byte)numShapes);
                handler.WriteShort((short)uknCount);
                handler.WriteShort((short)numRequestSets);
                handler.WriteShort((short)maxRequestSetId);
            }
            else
            {
                handler.Write(ref numGroups);
                if (handler.FileVersion >= 25) {
                    handler.Write(ref numUserData);
                } else {
                    handler.Write(ref numShapes);
                }
                handler.Write(ref uknCount);
                handler.Write(ref numRequestSets);
                handler.Write(ref maxRequestSetId);
            }
            if (handler.FileVersion > 11)
            {
                handler.Write(ref numIgnoreTags);
                handler.Write(ref numAutoGenerateJoints);
            }
            handler.Write(ref userDataSize);
            handler.Write(ref status);
            if (handler.FileVersion == 2) handler.Write<int>(0);
            if (handler.FileVersion == 11)
            {
                handler.Write(ref uknRe3_A);
                handler.Write(ref uknRe3_B);
            }
            if (handler.FileVersion >= 20)
            {
                handler.Write(ref ukn1);
                handler.Write(ref ukn2);
            }
            handler.Write(ref groupsPtrOffset);
            handler.Write(ref dataOffset);
            handler.Write(ref requestSetOffset);
            if (handler.FileVersion > 11)
            {
                handler.Write(ref ignoreTagOffset);
                handler.Write(ref autoGenerateJointDescOffset);
            }
            else if (handler.FileVersion == 2)
            {
                handler.Write(ref requestSetIDLookupsOffset);
            }
            else if (handler.FileVersion == 11)
            {
                handler.Write(ref uknRe3);
            }
            if (handler.FileVersion >= 20) {
                handler.Write(ref unknPtr0);
                handler.Write(ref unknPtr1);
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

        protected override bool DoRead(FileHandler handler)
        {
            MaskGuids?.Clear();
            handler.Read(ref guid);
            handler.ReadOffsetWString(out Name);
            handler.Read(ref NameHash);
            if (handler.FileVersion >= 25)
            {
                handler.Read(ref NumShapes);
                // verified: for 27, this is extra (mirror) shape count and not userdata. need to recheck v25 and pre-25
                handler.Read(ref NumExtraShapes);
                handler.Read(ref NumMaskGuids);
            }
            else if (handler.FileVersion > 2)
            {
                handler.Read(ref UserDataIndex);
                handler.Read(ref NumShapes);
                handler.Read(ref NumMaskGuids);
            }
            else // RE7
            {
                UserDataIndex = handler.Read<short>();
                NumShapes = handler.Read<short>();
            }
            handler.Read(ref ShapesOffset);
            handler.Read(ref LayerIndex);
            handler.Read(ref MaskBits);

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
            handler.Write(ref guid);
            handler.WriteOffsetWString(Name);
            NameHash = MurMur3HashUtils.GetHash(Name ?? string.Empty);
            handler.Write(ref NameHash);
            UserDataIndex = UserData?.Index ?? UserDataIndex;
            NumMaskGuids = MaskGuids?.Count ?? 0;
            if (handler.FileVersion >= 25)
            {
                handler.Write(ref NumShapes);
                handler.Write(ref NumExtraShapes);
                handler.Write(ref NumMaskGuids);
            }
            else if (handler.FileVersion > 2)
            {
                handler.Write(ref UserDataIndex);
                handler.Write(ref NumShapes);
                handler.Write(ref NumMaskGuids);
            }
            else
            {
                handler.Write((short)UserDataIndex);
                handler.Write((short)NumShapes);
            }
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


    public class RcolShapeInfo : BaseModel
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

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref Guid);
            handler.ReadOffsetWString(out Name);
            handler.Read(ref NameHash);
            handler.Read(ref UserDataIndex);
            handler.Read(ref LayerIndex);
            handler.Read(ref Attribute);

            if (handler.FileVersion >= 27)
            {
                handler.Read(ref SkipIdBits);
                handler.Read(ref shapeType);
                handler.Read(ref IgnoreTagBits);
                handler.Skip(4);
                handler.ReadOffsetWString(out primaryJointNameStr);
                handler.ReadOffsetWString(out secondaryJointNameStr);
                handler.Read(ref PrimaryJointNameHash);
                handler.Read(ref SecondaryJointNameHash);
            }
            else if (handler.FileVersion > 2)
            {
                handler.Read(ref SkipIdBits);
                handler.Read(ref IgnoreTagBits);
                handler.ReadOffsetWString(out primaryJointNameStr);
                handler.ReadOffsetWString(out secondaryJointNameStr);
                handler.Read(ref PrimaryJointNameHash);
                handler.Read(ref SecondaryJointNameHash);
                handler.Read(ref shapeType);
                handler.Skip(4);
            }
            else
            {
                handler.ReadOffsetWString(out primaryJointNameStr);
                handler.ReadOffsetWString(out secondaryJointNameStr);
                handler.Read(ref PrimaryJointNameHash);
                handler.Read(ref SecondaryJointNameHash);
                handler.Read(ref shapeType);
                handler.Skip(12);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            PrimaryJointNameHash = MurMur3HashUtils.GetHash(primaryJointNameStr ?? string.Empty);
            SecondaryJointNameHash = MurMur3HashUtils.GetHash(secondaryJointNameStr ?? string.Empty);
            NameHash = MurMur3HashUtils.GetHash(Name ?? string.Empty);
            handler.Write(ref Guid);
            handler.WriteOffsetWString(Name ?? string.Empty);
            handler.Write(ref NameHash);
            handler.Write(ref UserDataIndex);
            handler.Write(ref LayerIndex);
            handler.Write(ref Attribute);

            if (handler.FileVersion >= 27)
            {
                handler.Write(ref SkipIdBits);
                handler.Write(ref shapeType);
                handler.Write(ref IgnoreTagBits);
                handler.Skip(4);
                handler.WriteOffsetWString(primaryJointNameStr ?? string.Empty);
                handler.WriteOffsetWString(secondaryJointNameStr ?? string.Empty);
                handler.Write(ref PrimaryJointNameHash);
                handler.Write(ref SecondaryJointNameHash);
            }
            else if (handler.FileVersion > 2)
            {
                handler.Write(ref SkipIdBits);
                handler.Write(ref IgnoreTagBits);
                handler.WriteOffsetWString(primaryJointNameStr ?? string.Empty);
                handler.WriteOffsetWString(secondaryJointNameStr ?? string.Empty);
                handler.Write(ref PrimaryJointNameHash);
                handler.Write(ref SecondaryJointNameHash);
                handler.Write(ref shapeType);
                handler.Skip(4);
            }
            else
            {
                handler.WriteOffsetWString(primaryJointNameStr ?? string.Empty);
                handler.WriteOffsetWString(secondaryJointNameStr ?? string.Empty);
                handler.Write(ref PrimaryJointNameHash);
                handler.Write(ref SecondaryJointNameHash);
                handler.Write(ref shapeType);
                handler.Skip(12);
            }
            return true;
        }
    }


    public class RcolShape : BaseModel
    {
        public RcolShapeInfo Info { get; } = new();
        public object? shape;

        public RszInstance? Instance { get; set; }

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
    }


    public class RequestSetInfo : BaseModel
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

        protected override bool DoRead(FileHandler handler)
        {
            if (handler.FileVersion >= 25)
            {
                handler.Read(ref ID);
                handler.Read(ref GroupIndex);
                handler.Read(ref requestSetUserdataIndex);
                handler.Read(ref groupUserdataIndexStart);
                handler.Read(ref status);
                handler.Read(ref requestSetIndex);
                handler.ReadOffsetWString(out Name);
                handler.ReadOffsetWString(out KeyName);
                handler.Read(ref NameHash);
                handler.Read(ref KeyHash);
            }
            else if (handler.FileVersion > 2)
            {
                handler.Read(ref ID);
                handler.Read(ref GroupIndex);
                handler.Read(ref ShapeOffset);
                handler.Read(ref status);
                handler.ReadOffsetWString(out Name);
                handler.Read(ref NameHash);
                handler.Skip(4);
                handler.ReadOffsetWString(out KeyName);
                handler.Read(ref KeyHash);
                handler.Skip(4);
            }
            else
            {
                handler.Read(ref GroupIndex);
                handler.Read(ref ShapeOffset);
                handler.Read(ref status);
                handler.Skip(4);
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (handler.FileVersion >= 25)
            {
                handler.Write(ref ID);
                handler.Write(ref GroupIndex);
                NameHash = MurMur3HashUtils.GetHash(Name);
                KeyHash = MurMur3HashUtils.GetHash(KeyName);
                // TODO make sure we set requestSetUserdataIndex
                handler.Write(ref requestSetUserdataIndex);
                handler.Write(ref groupUserdataIndexStart);
                handler.Write(ref status);
                handler.Write(ref requestSetIndex);
                handler.WriteOffsetWString(Name);
                handler.WriteOffsetWString(KeyName);
                handler.Write(ref NameHash);
                handler.Write(ref KeyHash);
            }
            else if (handler.FileVersion > 2)
            {
                handler.Write(ref ID);
                handler.Write(ref GroupIndex);
                NameHash = MurMur3HashUtils.GetHash(Name);
                KeyHash = MurMur3HashUtils.GetHash(KeyName);
                handler.Write(ref ShapeOffset);
                handler.Write(ref status);
                handler.WriteOffsetWString(Name);
                handler.Write(ref NameHash);
                handler.Skip(4);
                handler.WriteOffsetWString(KeyName);
                handler.Write(ref KeyHash);
                handler.Skip(4);
            }
            else
            {
                handler.Write(ref GroupIndex);
                handler.Write(ref ShapeOffset);
                handler.Write(ref status);
                handler.Skip(4);
            }

            return true;
        }

        public override string ToString() => Name;
    }


    public class RequestSet(int index = 0, RequestSetInfo? info = null)
    {
        public int Index { get; set; } = index;
        public RequestSetInfo Info { get; set; } = info ?? new();
        public RcolGroup? Group { get; set; }
        public RszInstance? Instance { get; set; }
        public List<RszInstance> ShapeUserdata { get; set; } = new();
    }


    public class IgnoreTag : BaseModel
    {
        public string ignoreTagString = string.Empty;
        public uint hash;
        public uint ukn;

        protected override bool DoRead(FileHandler handler)
        {
            handler.ReadOffsetWString(out ignoreTagString);
            handler.Read(ref hash);
            handler.Read(ref ukn);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.WriteOffsetWString(ignoreTagString);
            handler.Write(ref hash);
            handler.Write(ref ukn);
            return true;
        }
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

                    RequestSet requestSet = new(i, requestSetInfo);
                    RequestSets.Add(requestSet);
                }
            }

            if (header.numIgnoreTags > 0)
            {
                handler.Seek(header.ignoreTagOffset);
                IgnoreTags ??= new();
                IgnoreTags.Read(handler, header.numIgnoreTags);
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
            header.uknCount = Groups.Count; // value seems meaningless, putting something in just in case
            header.numUserData = 0;
            header.numShapes = 0;
            header.maxRequestSetId = RequestSets.Count == 0 ? uint.MaxValue : (uint)RequestSets.Max(s => s.Index);

            if (handler.FileVersion >= 25)
            {
                header.numUserData = RequestSets.Sum(s => s.ShapeUserdata.Count);
            }
            else
            {
                header.numUserData = RequestSets.Count + Groups.Sum(g => g.Shapes.Count);
            }

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
                foreach (var set in RequestSets.OrderBy(s => s.Info.ID))
                {
                    while (index < set.Info.ID) {
                        index++;
                        handler.Write<int>(-1);
                    }
                    handler.Write(set.Info.ID);
                    index++;
                }
            }

            handler.StringTableFlush();
            handler.OffsetContentTableFlush();

            header.magic = Magic;
            header.Write(handler, 0);
            return true;
        }
    }
}
