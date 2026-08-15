using System.Numerics;
using ReeLib.Bvh;
using ReeLib.Common;
using ReeLib.InternalAttributes;
using ReeLib.via;

namespace ReeLib.Jcns
{
    public enum JcnsVersion
    {
        RE2 = 11,
        RE3 = 12,
        RE8 = 16,
        RE_RT = 19,
        MHR = 21,
        RE4_SF6 = 22,
        DD2 = 24,
        MHWilds = 29,
        RE9 = 35,
        OniWS = 36,
        MHWildsTU4 = 102,
    }

    public class Header : ReadWriteModel
    {
        internal JcnsVersion version;
        internal uint magic = JcnsFile.Magic;
        internal long infoTableOffset;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref version);
            action.Do(ref magic);
            action.Null(8);
            action.Expect(48L);
            action.Null(8);
            action.Do(ref infoTableOffset);
            action.Expect(1L);
            action.Expect(15L);
            action.Expect(0L);
            return true;
        }
    }

    public class JcnsDataInfoTable : ReadWriteModel
    {
        internal JcnsVersion version;

        public long coneDriverTableOffset;
        public long constraintInfoOffset;
        public long objectSettingOffset;
        public long rotExpressionInfoOffset;
        public long rotExpressionMapOffset;
        public long rotExpressionSourceHashIndicesOffset;
        public long rotExpressionHashIndicesOffset;
        public long skinConstraintOffset;
        public long skinConstraintSourceTableOffset;
        public long aimConstraintOffset;
        public long materialConstraintInfoOffset;
        public long jointExportGraphInfoOffset;
        public long sectionTableOffset;
        public long dependencyTableOffset;
        public long hashTableOffset;
        public long skinConstraintHashTableOffset;

        public int hashTableItemCount;
        public short coneDriverCount;
        public short constraintCount;
        public short dependencyCount;
        public short objectSettingCount;
        public short rotExpressionCount;
        public short rotExpressionMapCount;
        public short skinConstraintCount;
        public short skinConstraintHashTableItemCount;
        public short skinConstraintSourceCount;
        public short aimConstraintCount;
        public short materialConstraintCount;
        public short sectionCount;
        public short uknNum;
        public byte[] ExtraBytes = [];

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Expect(80L);
            action.Expect(0L);

            action.Do(ref coneDriverTableOffset);
            action.Do(ref constraintInfoOffset);
            action.Do(ref objectSettingOffset);
            action.Do(ref rotExpressionInfoOffset);
            action.Do(ref rotExpressionMapOffset);
            if (version >= JcnsVersion.RE9) {
                action.Do(ref rotExpressionSourceHashIndicesOffset);
                action.Do(ref rotExpressionHashIndicesOffset);
            }
            action.Do(ref skinConstraintOffset);
            if (version is >= JcnsVersion.MHWilds or JcnsVersion.RE3) { // TODO verify RE3
                action.Do(ref skinConstraintSourceTableOffset);
            }
            if (version >= JcnsVersion.RE8) {
                action.Do(ref aimConstraintOffset);
            }
            if (version >= JcnsVersion.RE4_SF6) {
                action.Do(ref materialConstraintInfoOffset);
            }
            if (version >= JcnsVersion.MHWilds) {
                action.Do(ref jointExportGraphInfoOffset);
            }
            if (version >= JcnsVersion.RE8) {
                action.Do(ref sectionTableOffset);
            }
            if (version >= JcnsVersion.MHR) {
                action.Do(ref dependencyTableOffset);
            }
            if (version >= JcnsVersion.RE9) {
                action.Do(ref hashTableOffset);
                action.Do(version >= JcnsVersion.OniWS, ref skinConstraintHashTableOffset);
                action.Do(ref hashTableItemCount);
            }

            action.Do(ref coneDriverCount);
            action.Do(ref constraintCount);
            if (version >= JcnsVersion.MHR) {
                action.Do(ref dependencyCount);
            }
            action.Do(ref objectSettingCount);
            action.Do(ref rotExpressionCount);
            action.Do(ref rotExpressionMapCount);
            action.Do(ref skinConstraintCount);
            if (version is >= JcnsVersion.MHWilds or JcnsVersion.RE3) {
                action.Do(ref skinConstraintHashTableItemCount);
                action.Do(ref skinConstraintSourceCount);
            }
            if (version >= JcnsVersion.RE8) {
                action.Do(ref aimConstraintCount);
            }
            if (version >= JcnsVersion.RE4_SF6) {
                action.Do(ref materialConstraintCount);
            }
            if (version is >= JcnsVersion.DD2) {
                action.Do(ref sectionCount);
            }
            if (version is >= JcnsVersion.RE9) {
                action.Do(ref uknNum);
            }
            var aligned = Utils.Align16((int)action.Handler.Position);
            if (aligned > action.Handler.Position) {
                Array.Resize(ref ExtraBytes, aligned - (int)action.Handler.Position);
                action.Handle(ref ExtraBytes);
            } else {
                ExtraBytes = [];
            }

            return true;
        }
    }

    public class IndexedJointHash
    {
        public string? name;
        public uint hash;
        public int hashIndex = -1;

        public override string ToString() => hash == uint.MaxValue ? "No Joint" : $"[{hash}] {name}";
    }

    public class ConeDriver : BaseModel
    {
        public string name = "";
        public IndexedJointHash joint = new();
        public IndexedJointHash parentJoint = new();
        public IndexedJointHash symmetryJoint = new();
        public Quaternion direction;
        public mat4 offset;
        public Vector3 translation;

        public uint nameHash;

        public float angleRad;
        public uint uknInt;
        public byte[] uknRest = new byte[8];

        protected override bool DoRead(FileHandler handler)
        {
            var version = handler.FileVersion;
            name = handler.ReadOffsetWString();
            if (version >= 35) {
                handler.Read(ref joint.hashIndex);
                handler.Read(ref parentJoint.hashIndex);
                handler.Read(ref symmetryJoint.hashIndex);
            } else {
                joint.name = handler.ReadOffsetWString();
                parentJoint.name = handler.ReadOffsetWString();
                if (version >= 24) {
                    symmetryJoint.name = handler.ReadOffsetWString();
                }
            }

            handler.Read(ref direction);
            if (version >= 24) {
                handler.Read(ref offset);
            } else if (version >= 12) {
                var translation = handler.Read<Vector4>();
                offset = Matrix4x4.CreateTranslation(new Vector3(translation.X, translation.Y, translation.Z));
            }

            if (version < 35) {
                handler.Read(ref joint.hash);
                handler.Read(ref parentJoint.hash);
                if (version >= 24) {
                    handler.Read(ref symmetryJoint.hash);
                }
            }
            handler.Read(ref angleRad);
            if (version >= 24) {
                handler.Read(ref uknInt);
            }
            handler.ReadArray(uknRest);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            var version = handler.FileVersion;
            handler.WriteOffsetWString(name);
            if (version >= 35) {
                handler.Write(ref joint.hashIndex);
                handler.Write(ref parentJoint.hashIndex);
                handler.Write(ref symmetryJoint.hashIndex);
            } else {
                handler.WriteOffsetWStringNullable(joint.name);
                handler.WriteOffsetWStringNullable(parentJoint.name);
                if (version >= 24) {
                    handler.WriteOffsetWStringNullable(symmetryJoint.name);
                }
            }
            handler.Write(ref direction);
            if (version >= 24) {
                handler.Write(ref offset);
            } else if (version >= 12) {
                var translation = offset.ToSystem().Translation;
                handler.Write(new Vector4(translation, 0));
            }

            if (version < 35) {
                handler.Write(ref joint.hash);
                handler.Write(ref parentJoint.hash);
                if (version >= 24) {
                    handler.Write(ref symmetryJoint.hash);
                }
            }
            handler.Write(ref angleRad);
            if (version >= 24) {
                handler.Write(ref uknInt);
            }
            handler.WriteArray(uknRest);
            return true;
        }

        internal void ReadHashes(List<IndexedJointHash> hashes)
        {
            joint = hashes[joint.hashIndex];
            parentJoint = hashes[parentJoint.hashIndex];
            symmetryJoint = symmetryJoint.hashIndex == -1 ? new IndexedJointHash() : hashes[symmetryJoint.hashIndex];
        }
    }

    public struct Range3
    {
        public float min;
        public float mid;
        public float max;
    }

    public class ConstraintSource : BaseModel
    {
        internal long complexMappingInfoOffset;
        public string jointName = "";
        public IndexedJointHash joint = new();
        public List<object> ComplexMappingInfos { get; } = new();
        public short uknShort1;
        public byte uknByte1;
        public byte uknByte2;
        public TransformAxis axis;
        public byte uknByte3;
        public byte uknByte4;
        public byte uknByte5;
        public byte uknByte6;
        public byte uknByte7;
        public Range3 mapFrom;
        public Range3 mapTo;
        public Quaternion rotation;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref complexMappingInfoOffset);
            jointName = handler.ReadOffsetWString();
            handler.Read(ref joint.hashIndex); // TODO <35 version
            var complexMapCount = handler.Read<short>();
            handler.Read(ref uknShort1);
            handler.Read(ref uknByte1);
            handler.Read(ref uknByte2);
            handler.Read(ref axis);
            handler.Read(ref uknByte3);
            handler.Read(ref uknByte4);
            handler.Read(ref uknByte5);
            handler.Read(ref uknByte6);
            handler.Read(ref uknByte7);
            handler.Read(ref mapFrom);
            handler.Read(ref mapTo);
            handler.Read(ref rotation); // TODO check versions
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => jointName;
    }

    public class ConeDriverInfo : ReadWriteModel
    {
        public Vector4 offset;
        public float angleDeg;
        public byte uknByte;
        public short coneDriverIndex;
        public byte uknByte2;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            if (action.Version >= 24) {
                action.Do(ref offset);
            } else {
                action.Do(ref offset.X);
            }
            action.Do(ref angleDeg);
            action.Do(ref uknByte);
            action.Do(ref coneDriverIndex);
            action.Do(ref uknByte2);
            return true;
        }
    }

    public class Constraint : BaseModel
    {
        public string name = "";
        public string? property;
        public IndexedJointHash hashObject = new();
        public IndexedJointHash propertyHash = new();

        public ConstraintFlags flags;
        public ConstraintTransformType transformType;

        public List<ConeDriverInfo> ConeDrivers { get; } = new();
        public List<ConstraintSource> Sources { get; } = new();

        internal long coneDriversOffset;
        internal long sourcesOffset;
        public Vector4 tranformation;
        public Vector2 transformExtraVec2;
        public uint uknUint;
        public TransformAxis transformAxis;
        public byte uknByte1;
        public byte uknByte2;
        public byte uknByte3;
        public byte uknByte4;
        public byte uknByte5;
        public byte uknByte6;
        public byte uknByte7;
        public byte uknByte8;
        public byte uknByte9;
        public byte uknByte10;
        public byte uknByte11;
        public byte uknByte12;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref coneDriversOffset);
            handler.Read(ref sourcesOffset);
            name = handler.ReadOffsetWString();
            if (handler.FileVersion > 12) {
                property = handler.ReadOffsetWStringNullable();
            }
            if (handler.FileVersion >= 35) {
                handler.Read(ref hashObject.hashIndex);
            }
            handler.Read(ref hashObject.hash);
            if (handler.FileVersion > 12) {
                handler.Read(ref propertyHash.hashIndex);
            } else {
                handler.Read(ref uknUint);
            }

            var driverCount = handler.Read<byte>();
            var srcCount = handler.Read<byte>();

            if (handler.FileVersion >= 35) {
                handler.Read(ref flags);
                handler.Read(ref transformType);
            } else {
                handler.Read(ref uknByte1);
                handler.Read(ref transformType);
                handler.Read(ref uknByte2);
                handler.Read(ref transformAxis);
                handler.Read(ref uknByte3);
                handler.Read(ref uknByte4);
            }
            handler.Read(ref tranformation);
            if (handler.FileVersion >= 21) {
                handler.Read(ref transformExtraVec2);
                handler.Read(ref uknByte5);
                if (handler.FileVersion >= 35) {
                    handler.Read(ref transformAxis);
                } else {
                    handler.Read(ref uknByte6);
                }
                handler.Read(ref uknByte7);
                handler.Read(ref uknByte8);
                handler.Read(ref uknByte9);
                handler.Read(ref uknByte10);
                handler.Read(ref uknByte11);
                handler.Read(ref uknByte12);
            }
            ReadData(handler, driverCount, srcCount);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }

        private void ReadData(FileHandler handler, byte driverCount, byte sourceCount)
        {
            handler.Seek(coneDriversOffset);
            ConeDrivers.Read(handler, (int)driverCount);
            handler.Seek(sourcesOffset);
            Sources.Read(handler, (int)sourceCount);
        }

        internal void WriteData(FileHandler handler)
        {
            if (ConeDrivers.Count == 0) {
                coneDriversOffset = 0;
            } else {
                coneDriversOffset = handler.Tell();
                ConeDrivers.Write(handler);
            }

            if (Sources.Count == 0) {
                sourcesOffset = 0;
            } else {
                sourcesOffset = handler.Tell();
                Sources.Write(handler);
            }
        }

        public override string ToString() => name;
    }

    public class Dependency : BaseModel
    {
        internal long offset;
        internal int sourceCount;
        public uint hash;
        public List<uint> SourceHashes { get; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref offset);
            handler.Read(ref sourceCount);
            handler.ReadNull(4);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref offset);
            handler.Write(ref sourceCount);
            handler.WriteNull(4);
            return true;
        }

        internal void ReadData(FileHandler handler)
        {
            handler.Seek(offset);
            handler.Read(ref hash);
            SourceHashes.ReadStructList(handler, sourceCount);
        }

        internal void WriteData(FileHandler handler)
        {
            offset = handler.Tell();
            handler.Write(ref hash);
            SourceHashes.Write(handler);
        }

        public override string ToString() => $"{hash} ({SourceHashes.Count})";
    }

    public class ObjectSetting : BaseModel
    {
        internal long hashOffset;
        public byte ukn1;
        public byte ukn2;
        public byte ukn3;
        public byte ukn4;
        public uint ukn5;

        protected override bool DoRead(FileHandler handler)
        {
            throw new NotImplementedException();
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }

    [Flags]
    public enum ConstraintFlags : byte
    {
        Flag1_IsAddMaybe = 1,
        Flag2 = 2,
        Flag4 = 4,
        Flag8 = 8,
        Flag16_IsJointMaybe = 16,
        Flag32 = 32,
        Flag64 = 64,
        Flag128 = 128,
    }

    public enum ConstraintTransformType : byte
    {
        Translation,
        Rotation,
        Scale,
        BlendShape,
        UnkCtrl_4,
        UnkTopBank_5,
        Material_Color=7,
        Material_4D,
        Material_3D,
        Material_2D,
        Scalar,
        Unknown_12,
        UnkRotation_13,
        UnkRotation_14,
        UnkRotation_15,
        UnkRotation_16
    }

    public enum TransformAxis : byte
    {
        X,
        Y,
        Z,
        W,
        // note: there's probably negative X, Y, Z, W; not sure about the 5th
        UnknownAxis_4,
        UnknownAxis_5,
        UnknownAxis_6,
        UnknownAxis_7,
        UnknownAxis_8
    }
}


namespace ReeLib
{
    using ReeLib.Jcns;

    public class JcnsFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        internal Header Header { get; } = new();
        public List<IndexedJointHash> Joints { get; } = new();

        public JcnsDataInfoTable InfoTable { get; } = new();
        public List<ConeDriver> ConeDrivers { get; } = new();
        public List<Constraint> Constraints { get; } = new();
        public List<Dependency> Dependencies { get; } = new();
        public List<ObjectSetting> ObjectSettings { get; } = new();

        public const uint Magic = 0x736E636A;

        public void UpdateJointNames()
        {
            foreach (var joint in Joints) {
                joint.name = Utils.HashedBoneNames.GetValueOrDefault(joint.hash);
            }
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var header = Header;
            header.Read(handler);
            if (header.magic != Magic)
            {
                throw new Exception(handler.FilePath + " Invalid JCNS file");
            }

            handler.Seek(header.infoTableOffset);
            InfoTable.version = header.version;
            InfoTable.Read(handler);

            // TODO

            handler.Seek(InfoTable.coneDriverTableOffset);
            ConeDrivers.Read(handler, InfoTable.coneDriverCount);

            handler.Seek(InfoTable.dependencyTableOffset);
            Dependencies.Read(handler, InfoTable.dependencyCount);
            foreach (var dep in Dependencies) dep.ReadData(handler);

            handler.Seek(InfoTable.objectSettingOffset);
            // ObjectSettings.Read(handler, InfoTable.objectSettingCount);

            handler.Seek(InfoTable.constraintInfoOffset);
            Constraints.Read(handler, InfoTable.constraintCount);

            handler.Seek(InfoTable.aimConstraintOffset);
            handler.Seek(InfoTable.skinConstraintOffset);
            handler.Seek(InfoTable.rotExpressionInfoOffset);
            handler.Seek(InfoTable.rotExpressionMapOffset);
            handler.Seek(InfoTable.materialConstraintInfoOffset);
            handler.Seek(InfoTable.jointExportGraphInfoOffset);
            handler.Seek(InfoTable.skinConstraintHashTableOffset);
            handler.Seek(InfoTable.rotExpressionHashIndicesOffset);
            handler.Seek(InfoTable.skinConstraintSourceTableOffset);
            handler.Seek(InfoTable.rotExpressionSourceHashIndicesOffset);
            handler.Seek(InfoTable.sectionTableOffset);

            if (InfoTable.hashTableOffset > 0) {
                handler.Seek(InfoTable.hashTableOffset);
                var hashes = handler.ReadArray<uint>(InfoTable.hashTableItemCount);
                for (int i = 0; i < hashes.Length; i++) {
                    Joints.Add(new IndexedJointHash() { hashIndex = i, hash = hashes[i] });
                }

                foreach (var cd in ConeDrivers) {
                    cd.ReadHashes(Joints);
                }
            }
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var header = Header;
            header.Write(handler);
            header.infoTableOffset = handler.Tell();
            InfoTable.Write(handler);
            // TODO

            header.Rewrite(handler);
            InfoTable.Rewrite(handler);
            return true;
        }
    }
}