using RszTool.Rcol;


namespace RszTool.Rcol
{
    public class Header : BaseModel
    {
        public uint magic;
        public int numGroups;
        public int numShapes;
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

        public ulong uknRe4;
        public long groupsPtrOffset;
        public long dataOffset;
        public long requestSetOffset;
        public long ignoreTagOffset;
        public long autoGenerateJointDescOffset;
        public long uOffset;
        public long vOffset;
        public ulong uknRe3;

        public GameVersion Version { get; set; }
        public int ExtensionVersion { get; set; }

        public Header(GameVersion version, int extensionVersion)
        {
            Version = version;
            ExtensionVersion = extensionVersion;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref magic);
            if (Version == GameVersion.re7)
            {
                numGroups = handler.ReadShort();
                numShapes = handler.ReadShort();
                numUserData = handler.ReadShort();
                numRequestSets = handler.ReadShort();
            }
            else
            {
                handler.Read(ref numGroups);
                handler.Read(ref numShapes);
                handler.Read(ref numUserData);
                handler.Read(ref numRequestSets);
                handler.Read(ref maxRequestSetId);
            }
            if (Version >= GameVersion.re8)
            {
                handler.Read(ref numIgnoreTags);
                handler.Read(ref numAutoGenerateJoints);
            }
            handler.Read(ref userDataSize);
            handler.Read(ref status);
            if (Version == GameVersion.re3)
            {
                handler.Read(ref uknRe3_A);
                handler.Read(ref uknRe3_B);
            }
            if (ExtensionVersion == 20)
            {
                handler.Read(ref ukn1);
                handler.Read(ref ukn2);
            }
            if (Version == GameVersion.re4) handler.Read(ref uknRe4);
            handler.Read(ref groupsPtrOffset);
            handler.Read(ref dataOffset);
            handler.Read(ref requestSetOffset);
            if (Version >= GameVersion.re8)
            {
                handler.Read(ref ignoreTagOffset);
                handler.Read(ref autoGenerateJointDescOffset);
                handler.Read(ref uOffset);  // same as autoGenerateJointDescOffset?
                handler.Read(ref vOffset);  // same as autoGenerateJointDescOffset?
                handler.ReadUInt64();  // always zero?
            }
            else if (Version == GameVersion.re3)
            {
                handler.Read(ref uknRe3);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref magic);
            if (Version == GameVersion.re7)
            {
                handler.WriteShort((short)numGroups);
                handler.WriteShort((short)numShapes);
                handler.WriteShort((short)numUserData);
                handler.WriteShort((short)numRequestSets);
            }
            else
            {
                handler.Write(ref numGroups);
                handler.Write(ref numShapes);
                handler.Write(ref numUserData);
                handler.Write(ref numRequestSets);
                handler.Write(ref maxRequestSetId);
            }
            if (Version >= GameVersion.re8)
            {
                handler.Write(ref numIgnoreTags);
                handler.Write(ref numAutoGenerateJoints);
            }
            handler.Write(ref userDataSize);
            handler.Write(ref status);
            if (Version == GameVersion.re3)
            {
                handler.Write(ref uknRe3_A);
                handler.Write(ref uknRe3_B);
            }
            if (ExtensionVersion == 20)
            {
                handler.Write(ref ukn1);
                handler.Write(ref ukn2);
            }
            if (Version == GameVersion.re4) handler.Write(ref uknRe4);
            handler.Write(ref groupsPtrOffset);
            handler.Write(ref dataOffset);
            handler.Write(ref requestSetOffset);
            if (Version >= GameVersion.re8)
            {
                handler.Write(ref ignoreTagOffset);
                handler.Write(ref autoGenerateJointDescOffset);
                handler.Write(ref uOffset);  // same as autoGenerateJointDescOffset?
                handler.Write(ref vOffset);  // same as autoGenerateJointDescOffset?
                handler.WriteUInt64(0);
            }
            else if (Version == GameVersion.re3)
            {
                handler.Write(ref uknRe3);
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
        public int NumShapes;

        public int NumMaskGuids;
        public long ShapesOffset;
        public int LayerIndex;
        public uint MaskBits;
        public long MaskGuidsOffset;
        public Guid LayerGuid;
        public List<Guid>? MaskGuids { get; set; }

        public GameVersion Version { get; set; }
        public long ShapesOffsetStart;

        public GroupInfo(GameVersion version)
        {
            Version = version;
        }

        protected override bool DoRead(FileHandler handler)
        {
            MaskGuids?.Clear();
            handler.Read(ref guid);
            handler.ReadOffsetWString(out Name);
            handler.Read(ref NameHash);
            if (Version == GameVersion.re4)
            {
                handler.Read(ref NumShapes);
                handler.Read(ref UserDataIndex);
            }
            else
            {
                handler.Read(ref UserDataIndex);
                handler.Read(ref NumShapes);
            }
            handler.Read(ref NumMaskGuids);
            handler.Read(ref ShapesOffset);
            handler.Read(ref LayerIndex);
            handler.Read(ref MaskBits);
            handler.Read(ref MaskGuidsOffset);
            handler.Read(ref LayerGuid);

            using var defer = handler.SeekJumpBack(MaskGuidsOffset);
            {
                MaskGuids ??= new();
                handler.ReadList(MaskGuids, NumMaskGuids);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref guid);
            handler.WriteOffsetWString(Name);
            handler.Write(ref NameHash);
            if (Version == GameVersion.re4)
            {
                handler.Write(ref NumShapes);
                handler.Write(ref UserDataIndex);
            }
            else
            {
                handler.Write(ref UserDataIndex);
                handler.Write(ref NumShapes);
            }
            NumMaskGuids = MaskGuids?.Count ?? 0;
            handler.Write(ref NumMaskGuids);
            ShapesOffsetStart = handler.Tell();
            handler.Write(ref ShapesOffset);
            handler.Write(ref LayerIndex);
            handler.Write(ref MaskBits);
            if (MaskGuids != null)
            {
                handler.OffsetContentTableAdd(handler => handler.WriteList(MaskGuids));
            }
            handler.Write(ref MaskGuidsOffset);
            handler.Write(ref LayerGuid);
            return true;
        }
    }


    public class RcolGroup : BaseModel
    {
        public GroupInfo Info { get; }
        public List<RcolShape> Shapes { get; } = new();

        public GameVersion Version => Info.Version;

        public RcolGroup(GameVersion version)
        {
            Info = new(version);
        }

        public bool ReadInfo(FileHandler handler)
        {
            return Info.Read(handler);
        }

        public bool WriteInfo(FileHandler handler)
        {
            Info.NumShapes = Shapes.Count;
            return Info.Write(handler);
        }

        protected override bool DoRead(FileHandler handler)
        {
            Shapes.Clear();
            if (Info.NumShapes > 0)
            {
                handler.Seek(Info.ShapesOffset);
                Shapes.Read(handler, Info.NumShapes);
            }
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
        public int UserDataIndex;
        public int LayerIndex;
        public int Attribute;
        public uint SkipIdBits;
        public uint IgnoreTagBits;
        public string primaryJointNameStr = string.Empty;
        public string secondaryJointNameStr = string.Empty;
        public uint PrimaryJointNameHash;
        public uint SecondaryJointNameHash;


        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref Guid);
            handler.ReadOffsetWString(out Name);
            handler.ReadRange(ref NameHash, ref IgnoreTagBits);
            handler.ReadOffsetWString(out primaryJointNameStr);
            handler.ReadOffsetWString(out secondaryJointNameStr);
            handler.Read(ref PrimaryJointNameHash);
            handler.Read(ref SecondaryJointNameHash);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref Guid);
            handler.WriteOffsetWString(Name);
            handler.WriteRange(ref NameHash, ref IgnoreTagBits);
            handler.WriteOffsetWString(primaryJointNameStr);
            handler.WriteOffsetWString(secondaryJointNameStr);
            handler.Write(ref PrimaryJointNameHash);
            handler.Write(ref SecondaryJointNameHash);
            return true;
        }
    }


    public class RcolShape : BaseModel
    {
        public RcolShapeInfo Info { get; } = new();
        public ShapeType ShapeType;
        public via.OBB Parameters;

        public RszInstance? Instance { get; set; }

        protected override bool DoRead(FileHandler handler)
        {
            Info.Read(handler);
            handler.Read(ref ShapeType);
            handler.Skip(4);
            handler.Read(ref Parameters);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            Info.Write(handler);
            handler.Write(ref ShapeType);
            handler.Skip(4);
            handler.Write(ref Parameters);
            return true;
        }
    }


    public class RequestSetInfo(GameVersion version) : BaseModel
    {
        public uint ID;
        public int GroupIndex;
        public int ShapeOffset;
        public int status;

        private uint uknA, uknB;
        public string Name = string.Empty;
        public uint NameHash;
        public string KeyName = string.Empty;
        public uint KeyHash;
        public uint KeyHash2;

        public GameVersion Version { get; set; } = version;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref ID);
            handler.Read(ref GroupIndex);
            handler.Read(ref ShapeOffset);
            handler.Read(ref status);

            if (Version == GameVersion.re4)
            {
                handler.Read(ref uknA);
                handler.Read(ref uknB);
                handler.ReadOffsetWString(out Name);
                handler.ReadOffsetWString(out KeyName);
                handler.Read(ref KeyHash);
                handler.Read(ref KeyHash2);
            }
            else
            {
                handler.ReadOffsetWString(out Name);
                handler.Read(ref NameHash);
                handler.Skip(4);
                handler.ReadOffsetWString(out KeyName);
                handler.Read(ref KeyHash);
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref ID);
            handler.Write(ref GroupIndex);
            handler.Write(ref ShapeOffset);
            handler.Write(ref status);

            if (Version == GameVersion.re4)
            {
                handler.Write(ref uknA);
                handler.Write(ref uknB);
                handler.WriteOffsetWString(Name);
                handler.WriteOffsetWString(KeyName);
                handler.Write(ref KeyHash);
                handler.Write(ref KeyHash2);
            }
            else
            {
                handler.WriteOffsetWString(Name);
                handler.Write(ref NameHash);
                handler.Skip(4);
                handler.WriteOffsetWString(KeyName);
                handler.Write(ref KeyHash);
            }

            return true;
        }
    }


    public class RequestSet(int index, RequestSetInfo info)
    {
        public int Index { get; set; } = index;
        public RequestSetInfo Info { get; set; } = info;
        public RcolGroup? Group { get; set; }
        public RszInstance? Instance { get; set; }
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


namespace RszTool
{
    public class RcolFile : BaseRszFile
    {
        public Header Header { get; }
        public RSZFile? RSZ { get; private set; }
        public List<RcolGroup> Groups { get; } = new();
        public List<RequestSet> RequestSets { get; } = new();
        public List<IgnoreTag>? IgnoreTags { get; private set; } = new();
        public List<string>? AutoGenerateJointDescs { get; private set; } = new();

        public const uint Magic = 0x4c4f4352;
        public const string Extension2 = ".rcol";

        public RcolFile(RszFileOption option, FileHandler fileHandler) : base(option, fileHandler)
        {
            int extensionVersion = RszUtils.GetFileExtensionVersion(fileHandler.FilePath!);
            Header = new Header(option.Version, extensionVersion);
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
                    RcolGroup group = new(Option.Version);
                    if (!group.ReadInfo(handler)) return false;
                    Groups.Add(group);
                }
                foreach (var group in Groups)
                {
                    if (!group.Read(handler)) return false;
                }
            }

            RSZ = ReadRsz(header.dataOffset);

            if (header.numRequestSets > 0)
            {
                handler.Seek(header.requestSetOffset);
                for (int i = 0; i < header.numRequestSets; i++)
                {
                    RequestSetInfo requestSetInfo = new(Option.Version);
                    if (!requestSetInfo.Read(handler)) return false;

                    RequestSet requestSet = new(i, requestSetInfo)
                    {
                        Group = Groups[requestSetInfo.GroupIndex],
                        Instance = RSZ.ObjectList[i]
                    };
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

            foreach (var group in Groups)
            {
                foreach (var shape in group.Shapes)
                {
                    if (shape.Info.UserDataIndex > -1)
                    {
                        shape.Instance = RSZ.ObjectList[shape.Info.UserDataIndex];
                    }
                }
            }
            return true;
        }

        protected override bool DoWrite()
        {
            FileHandler handler = FileHandler;
            handler.Clear();
            var header = Header;
            header.numShapes = 0;
            header.numUserData = 0;
            handler.Seek(Header.Size);

            handler.Align(16);
            header.numGroups = Groups.Count;
            header.groupsPtrOffset = handler.Tell();
            foreach (var group in Groups)
            {
                group.WriteInfo(handler);
            }
            foreach (var group in Groups)
            {
                group.Write(handler);
                header.numShapes += group.Shapes.Count;
                foreach (var shape in group.Shapes)
                {
                    if (shape.Instance != null) header.numUserData++;
                }
            }

            handler.Align(16);
            header.dataOffset = handler.Tell();
            WriteRsz(RSZ!, header.dataOffset);
            header.userDataSize = (int)RSZ!.Size;

            handler.Align(16);
            header.maxRequestSetId = 0;
            header.numRequestSets = RequestSets.Count;
            header.requestSetOffset = handler.Tell();
            foreach (var item in RequestSets)
            {
                item.Info.Write(handler);
                if (item.Info.ID > header.maxRequestSetId) header.maxRequestSetId = item.Info.ID;
            }

            handler.Align(16);
            header.numIgnoreTags = IgnoreTags?.Count ?? 0;
            header.ignoreTagOffset = handler.Tell();
            IgnoreTags?.Write(handler);

            header.numAutoGenerateJoints = AutoGenerateJointDescs?.Count ?? 0;
            header.autoGenerateJointDescOffset = handler.Tell();
            header.uOffset = header.autoGenerateJointDescOffset;
            header.vOffset = header.autoGenerateJointDescOffset;
            if (AutoGenerateJointDescs != null)
            {
                foreach (var item in AutoGenerateJointDescs)
                {
                    handler.WriteOffsetWString(item);
                }
            }

            handler.Align(16);
            handler.StringTableFlush();
            handler.OffsetContentTableFlush();

            header.magic = Magic;
            Header.Write(handler, 0);
            return true;
        }
    }
}
