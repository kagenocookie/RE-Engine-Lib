using RszTool.Common;

namespace RszTool
{
    public class RcolFile : BaseRszFile
    {
        public class HeaderStruct : VersionedBaseModel {
            public uint magic;
            public int groupCount;
            public int userdataCount;
            public int shapeCount;
            public int uknCount;
            public int requestSetCount;
            public uint maxRequestSetId;
            public int ignoreTagCount;
            public int autoGenerateJointsCount;
            public uint userDataSize;
            public uint status;
            public long ukn;
            public long groupsOffset;
            public long userdataOffset;
            public long requestSetOffset;
            public long ignoreTagOffset;
            public long autoGenerateJointDescOffset;
            public long unknPtr0;
            public long unknPtr1;

            public HeaderStruct(GameVersion version) {
                Version = version;
            }

            protected override bool DoRead(FileHandler handler)
            {
                handler.Read(ref magic);
                var fileVersion = handler.FileVersion;
                if (fileVersion == 2) {
                    groupCount = handler.Read<short>();
                    shapeCount = handler.Read<short>();
                    userdataCount = handler.Read<short>();
                    requestSetCount = handler.Read<short>();
                } else {
                    handler.Read(ref groupCount);
                    if (fileVersion >= 25) {
                        handler.Read(ref userdataCount);
                        handler.Read(ref uknCount);
                    } else {
                        handler.Read(ref shapeCount);
                        handler.Read(ref userdataCount);
                    }
                    handler.Read(ref requestSetCount);
                    handler.Read(ref maxRequestSetId);
                    if (fileVersion > 11) {
                        handler.Read(ref ignoreTagCount);
                        handler.Read(ref autoGenerateJointsCount);
                    }
                    handler.Read(ref userDataSize);
                    handler.Read(ref status);
                    if (fileVersion == 11) {
                        handler.Read(ref unknPtr0);
                        handler.Read(ref unknPtr1);
                    }
                    if (fileVersion == 20) {
                        var uk1 = handler.Read<uint>();
                        var uk2 = handler.Read<uint>();
                        if (uk1 != 0 || uk2 != 0) {
                            Console.WriteLine("Found rcol uk1/2 != 0");
                        }
                    }
                }
                if (Version is >= GameVersion.re4 and not GameVersion.sf6) {
                    handler.Read(ref ukn);
                }
                handler.Read(ref groupsOffset);
                handler.Read(ref userdataOffset);
                handler.Read(ref requestSetOffset);
                if (fileVersion > 11) {
                    handler.Read(ref ignoreTagOffset);
                    handler.Read(ref autoGenerateJointDescOffset);
                }
                if (fileVersion == 11) {
                    handler.Read(ref unknPtr0);
                }
                if (Version is >= GameVersion.re4 and not GameVersion.sf6) {
                    handler.Read(ref unknPtr0);
                    handler.Read(ref unknPtr1);
                }
                return true;
            }

            protected override bool DoWrite(FileHandler handler)
            {
                handler.Write(ref magic);
                var fileVersion = handler.FileVersion;
                if (fileVersion == 2) {
                    groupCount = handler.Read<short>();
                    shapeCount = handler.Read<short>();
                    userdataCount = handler.Read<short>();
                    requestSetCount = handler.Read<short>();
                } else {
                    handler.Write(ref groupCount);
                    if (fileVersion >= 25) {
                        handler.Write(ref userdataCount);
                        handler.Write(ref uknCount);
                    } else {
                        handler.Write(ref shapeCount);
                        handler.Write(ref userdataCount);
                    }
                    handler.Write(ref requestSetCount);
                    handler.Write(ref maxRequestSetId);
                    if (fileVersion > 11) {
                        handler.Write(ref ignoreTagCount);
                        handler.Write(ref autoGenerateJointsCount);
                    }
                    handler.Write(ref userDataSize);
                    handler.Write(ref status);
                    if (fileVersion == 11) {
                        handler.Write(ref unknPtr0);
                        handler.Write(ref unknPtr1);
                    }
                    if (fileVersion == 20) {
                        handler.Write(0u);
                        handler.Write(0u);
                    }
                }
                if (Version is >= GameVersion.re4 and not GameVersion.sf6) {
                    handler.Write(ref ukn);
                }
                handler.Write(ref groupsOffset);
                handler.Write(ref userdataOffset);
                handler.Write(ref requestSetOffset);
                if (fileVersion > 11) {
                    handler.Write(ref ignoreTagOffset);
                    handler.Write(ref autoGenerateJointDescOffset);
                }
                if (fileVersion == 11) {
                    handler.Write(ref unknPtr0);
                }
                if (Version is >= GameVersion.re4 and not GameVersion.sf6) {
                    handler.Write(ref unknPtr0);
                    handler.Write(ref unknPtr1);
                }
                return true;
            }
        }

        public class RcolGroupInfo : BaseModel {
            public Guid guid;
            public string? name;
            public uint nameHash;
            public int numShapes;
            public int userDataIndex;
            public int numMaskGuids;
            public long shapesOffset;
            public int layerIndex;
            public uint maskBits;
            public ulong maskGuidsOffset;
            public Guid layerGuid;

            public Guid[] MaskGuids { get; set; } = Array.Empty<Guid>();

            public string? Name { get => name; set => name = value; }
            public Guid Guid { get => guid; set => guid = value; }
            public int LayerIndex { get => layerIndex; set => layerIndex = value; }
            public uint MaskBits { get => maskBits; set => maskBits = value; }
            public Guid LayerGuid { get => layerGuid; set => layerGuid = value; }

            protected override bool DoRead(FileHandler handler)
            {
                handler.Read(ref guid);
                handler.ReadOffsetWString(out name);
                handler.Read(ref nameHash);
                if (handler.FileVersion >= 25) {
                    handler.Read(ref numShapes);
                    handler.Read(ref userDataIndex);
                } else {
                    handler.Read(ref userDataIndex);
                    handler.Read(ref numShapes);
                }
                handler.Read(ref numMaskGuids);
                handler.Read(ref shapesOffset);
                handler.Read(ref layerIndex);
                handler.Read(ref maskBits);
                handler.Read(ref maskGuidsOffset);
                handler.Read(ref layerGuid);

                MaskGuids = new Guid[numMaskGuids];
                if (numMaskGuids > 0) {
                    using var _ = handler.SeekJumpBack((long)maskGuidsOffset);
                    for (int i = 0; i < numMaskGuids; ++i) {
                        MaskGuids[i] = handler.Read<Guid>();
                    }
                }
                return true;
            }

            protected override bool DoWrite(FileHandler handler)
            {
                handler.Write(ref guid);
                handler.WriteOffsetWString(name ?? string.Empty);
                nameHash = MurMur3HashUtils.GetHash(name ?? string.Empty);
                handler.Write(ref nameHash);
                if (handler.FileVersion >= 25) {
                    handler.Write(ref numShapes);
                    handler.Write(ref userDataIndex);
                } else {
                    handler.Write(ref userDataIndex);
                    handler.Write(ref numShapes);
                }
                handler.Write(MaskGuids.Length);
                handler.Write(ref shapesOffset);
                handler.Write(ref layerIndex);
                handler.Write(ref maskBits);
                handler.WriteOffsetGuidArray(MaskGuids.ToArray());
                handler.Write(ref layerGuid);
                return true;
            }

            public override string ToString()
            {
                return name ?? "";
            }
        }

        public class RcolGroup : BaseModel
        {
            public RcolGroupInfo Info { get; } = new();
            public List<RcolShape> Shapes { get; } = new();

            protected override bool DoRead(FileHandler handler)
            {
                Info.Read(handler);
                var pos = handler.Tell();
                handler.Seek(Info.shapesOffset);
                Shapes.Clear();
                Shapes.Read(handler, Info.numShapes);
                handler.Seek(pos);
                return true;
            }

            protected override bool DoWrite(FileHandler handler)
            {
                Info.numShapes = Shapes.Count;
                Info.userDataIndex = 0; // TODO
                Info.Write(handler);
                return true;
            }
        }

        public class RcolShape : BaseModel
        {
            public Guid Guid;
            public string? name;
            public uint nameHash;
            public int userDataIndex;
            public int layerIndex;
            public int attribute;
            public uint skipIdBits;
            public ShapeType shapeType;
            public uint ignoreTagBits;
            public uint unkn;
            public string? primaryJointNameStr;
            public string? secondaryJointNameStr;
            public uint primaryJointHash;
            public uint secondaryJointHash;
            public object? shape;

            public string? Name { get => name; set => name = value; }
            public int LayerIndex { get => layerIndex; set => layerIndex = value; }
            public uint SkipIdBits { get => skipIdBits; set => skipIdBits = value; }
            public uint IgnoreTagBits { get => ignoreTagBits; set => ignoreTagBits = value; }
            public int Attribute { get => attribute; set => attribute = value; }
            public ShapeType ShapeType { get => shapeType; set => shapeType = value; }
            public string? PrimaryJointNameStr { get => primaryJointNameStr; set => primaryJointNameStr = value; }
            public string? SecondaryJointNameStr { get => secondaryJointNameStr; set => secondaryJointNameStr = value; }

            public RszInstance? UserData { get; set; }

            protected override bool DoRead(FileHandler handler)
            {
                // unknown: file ver < 20, ver 25 (re4/sf6), ver 28 (mhws)
                handler.Read(ref Guid);
                handler.ReadOffsetWString(out name);
                handler.Read(ref nameHash);
                handler.Read(ref userDataIndex);
                handler.Read(ref layerIndex);
                handler.Read(ref attribute);
                handler.Read(ref skipIdBits);

                if (handler.FileVersion >= 25) {
                    handler.Read(ref shapeType);
                    handler.Read(ref ignoreTagBits);
                    handler.Read(ref unkn);
                    handler.ReadOffsetWString(out primaryJointNameStr);
                    handler.ReadOffsetWString(out secondaryJointNameStr);
                    handler.Read(ref primaryJointHash);
                    handler.Read(ref secondaryJointHash);
                } else {
                    handler.Read(ref ignoreTagBits);
                    handler.ReadOffsetWString(out primaryJointNameStr);
                    handler.ReadOffsetWString(out secondaryJointNameStr);
                    handler.Read(ref primaryJointHash);
                    handler.Read(ref secondaryJointHash);
                    handler.Read(ref shapeType);
                }

                var tell = handler.Tell();
                shape = shapeType switch
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
                    _ => throw new Exception("Unsupported RCOL shape type " + shapeType),
                };
                handler.Seek(tell + sizeof(float) * 4 * 5);
                return true;
            }

            protected override bool DoWrite(FileHandler handler)
            {
                primaryJointHash = MurMur3HashUtils.GetHash(primaryJointNameStr ?? string.Empty);
                secondaryJointHash = MurMur3HashUtils.GetHash(secondaryJointNameStr ?? string.Empty);
                nameHash = MurMur3HashUtils.GetHash(name ?? string.Empty);
                handler.Write(ref Guid);
                handler.WriteOffsetWString(name ?? string.Empty);
                handler.Write(ref nameHash);
                handler.Write(ref userDataIndex);
                handler.Write(ref layerIndex);
                handler.Write(ref attribute);
                handler.Write(ref skipIdBits);
                if (handler.FileVersion >= 25) {
                    handler.Write(ref shapeType);
                    handler.Write(ref ignoreTagBits);
                    handler.Write(ref unkn);
                    handler.WriteOffsetWString(primaryJointNameStr ?? string.Empty);
                    handler.WriteOffsetWString(secondaryJointNameStr ?? string.Empty);

                    handler.Write(ref primaryJointHash);
                    handler.Write(ref secondaryJointHash);
                } else {
                    handler.Write(ref ignoreTagBits);
                    handler.WriteOffsetWString(primaryJointNameStr ?? string.Empty);
                    handler.WriteOffsetWString(secondaryJointNameStr ?? string.Empty);
                    handler.Write(ref primaryJointHash);
                    handler.Write(ref secondaryJointHash);
                    handler.Write(ref shapeType);
                }

                var tell = handler.Tell();
                shape = shapeType switch
                {
                    ShapeType.Aabb => handler.Write(tell, (via.AABB)shape!),
                    ShapeType.Sphere => handler.Write(tell, (via.Sphere)shape!),
                    ShapeType.Capsule => handler.Write(tell, (via.Capsule)shape!),
                    ShapeType.Box => handler.Write(tell, (via.OBB)shape!),
                    ShapeType.Area => handler.Write(tell, (via.Area)shape!),
                    ShapeType.Triangle => handler.Write(tell, (via.Triangle)shape!),
                    ShapeType.Cylinder => handler.Write(tell, (via.Cylinder)shape!),
                    ShapeType.ContinuousSphere => handler.Write(tell, (via.Sphere)shape!),
                    ShapeType.ContinuousCapsule => handler.Write(tell, (via.Capsule)shape!),
                    _ => throw new Exception("Unsupported RCOL shape type " + shapeType),
                };
                handler.Skip(sizeof(float) * 4 * 5);
                return true;
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

        public class RequestSet : BaseModel
        {
            public uint id;
            public int groupIndex;
            public int requestSetUserdataIndex = -1; // >= dd2 (rcol.27)
            public uint groupUserdataIndexStart; // >= dd2 (rcol.27)
            public uint status = 1;
            public uint shapeOffset; // <= re2r (rcol.20)
            public uint requestSetIndex; // >= dd2 (rcol.27)
            public string? name;
            public string? keyName;
            public uint keyHash;
            public uint keyNameHash;

            public RcolGroup? Group { get; set; }
            public RszInstance? Userdata { get; set; }

            protected override bool DoRead(FileHandler handler)
            {
                handler.Read(ref id);
                handler.Read(ref groupIndex);

                if (handler.FileVersion >= 25) {
                    handler.Read(ref requestSetUserdataIndex);
                    handler.Read(ref groupUserdataIndexStart);
                    handler.Read(ref status);
                    handler.Read(ref requestSetIndex);
                } else {
                    handler.Read(ref shapeOffset);
                    handler.Read(ref status);
                }

                name = handler.ReadOffsetWString();
                keyName = handler.ReadOffsetWString();
                handler.Read(ref keyHash);
                handler.Read(ref keyNameHash);
                return true;
            }

            protected override bool DoWrite(FileHandler handler)
            {
                handler.Write(ref id);
                handler.Write(ref groupIndex);

                if (handler.FileVersion >= 25) {
                    handler.Write(ref requestSetUserdataIndex);
                    handler.Write(ref groupUserdataIndexStart);
                    handler.Write(ref status);
                    handler.Write(ref requestSetIndex);
                } else {
                    handler.Write(ref shapeOffset);
                    handler.Write(ref status);
                }

                handler.WriteOffsetWString(name ?? string.Empty);
                handler.WriteOffsetWString(keyName ?? string.Empty);
                keyHash = MurMur3HashUtils.GetHash(name ?? string.Empty);
                handler.Write(ref keyHash);
                keyNameHash = MurMur3HashUtils.GetHash(keyName ?? string.Empty);
                handler.Write(ref keyNameHash);
                return true;
            }
        }

        public HeaderStruct Header { get; }
        public List<RcolGroup> GroupInfoList { get; } = new();
        public List<RequestSet> RequestSetInfoList { get; } = new();
        // public List<object> AutoGenerateJointList { get; } = new(0);
        // public List<object> IgnoreTagList { get; } = new(0);
        public RSZFile RSZ { get; }

        public RcolFile(RszFileOption option, FileHandler fileHandler) : base(option, fileHandler)
        {
            Header = new(option.Version);
            RSZ = new RSZFile(option, fileHandler);
            if (fileHandler.FilePath != null)
            {
                RszUtils.CheckFileExtension(fileHandler.FilePath, Extension, GetVersionExt());
            }
        }

        public const uint Magic = 0x4c4f4352;
        public const string Extension = ".rcol";

        public string? GetVersionExt()
        {
            return Option.GameName switch
            {
                GameName.re2 => ".10",
                GameName.re2rt => ".20",
                GameName.re3 => ".11",
                GameName.re3rt => ".20",
                GameName.re4 => ".25",
                GameName.re8 => ".18",
                GameName.re7 => ".2",
                GameName.re7rt => ".20",
                GameName.dmc5 =>".10",
                GameName.mhrise => ".20",
                GameName.sf6 => ".25",
                GameName.dd2 => ".27",
                GameName.mhwilds => ".28",
                _ => ".28"
            };
        }

        public override RSZFile? GetRSZ() => RSZ;

        protected override bool DoRead()
        {
            GroupInfoList.Clear();
            RequestSetInfoList.Clear();

            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a RCOL file");
            }

            handler.Seek(header.groupsOffset);
            GroupInfoList.Read(handler, header.groupCount);

            ReadRsz(RSZ, header.userdataOffset);

            handler.Seek(header.requestSetOffset);
            RequestSetInfoList.Read(handler, header.requestSetCount);

            SetupReferences();

            return true;
        }

        public void SetupReferences()
        {
            foreach (var set in RequestSetInfoList) {
                set.Group = GroupInfoList[set.groupIndex];
                if (set.requestSetUserdataIndex != -1) {
                    set.Userdata = RSZ.ObjectList[set.requestSetUserdataIndex];
                }
            }

            foreach (var group in GroupInfoList) {
                foreach (var shape in group.Shapes) {
                    if (shape.userDataIndex != -1) {
                        shape.UserData = RSZ.ObjectList[shape.userDataIndex];
                    }
                }
            }
        }

        protected override bool DoWrite()
        {
            if (StructChanged)
            {
                // TODO RebuildInfoTable();
            }

            FileHandler handler = FileHandler;
            handler.Clear();
            var header = Header;
            header.Write(handler);

            handler.Align(16);
            header.groupsOffset = handler.Tell();
            GroupInfoList.Write(handler);

            handler.Align(16);
            foreach (var grp in GroupInfoList) {
                grp.Info.shapesOffset = handler.Tell();
                grp.Shapes.Write(handler);
            }

            // note: not ideal, we're writing the group infos twice because we need the shape offsets
            var pos = handler.Tell();
            handler.Seek(header.groupsOffset);
            GroupInfoList.Write(handler);
            handler.Seek(pos);

            header.userdataOffset = handler.Tell();
            WriteRsz(RSZ, header.userdataOffset);
            header.userDataSize = (uint)(handler.Tell() - header.userdataOffset);

            handler.Align(16);
            header.requestSetOffset = handler.Tell();
            RequestSetInfoList.Write(handler);

            handler.Align(16);
            header.ignoreTagOffset = handler.Tell();
            header.autoGenerateJointDescOffset = handler.Tell();
            header.unknPtr0 = handler.Tell();
            header.unknPtr1 = handler.Tell();

            handler.OffsetContentTableFlush();
            handler.StringTableFlush();

            header.magic = Magic;
            header.requestSetCount = RequestSetInfoList.Count;
            header.groupCount = GroupInfoList.Count;
            header.uknCount = 1;
            header.userdataCount = GroupInfoList.Sum(grp => grp.Shapes.Count);
            header.shapeCount = GroupInfoList.Sum(grp => grp.Shapes.Count);
            header.maxRequestSetId = RequestSetInfoList.Max(s => s.id);
            header.Write(handler, 0);

            return true;
        }
    }
}
