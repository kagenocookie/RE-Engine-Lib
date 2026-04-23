using System.Numerics;
using ReeLib.Common;
using ReeLib.InternalAttributes;

namespace ReeLib.Chain
{
    public class Header : ReadWriteModel
    {
        public int version;
        public uint magic = ChainFile.Magic;
        public uint errorFlags;
        public uint masterSize;
        public long collisionAttrOffset;
        internal long modelCollisionOffset;
        internal long extraDataOffset;
        internal long groupsOffset;
        internal long linksOffset;
        internal long uknOffset;
        internal long settingsOffset;
        internal long windSettingsOffset;
        internal byte groupCount;
        internal byte settingCount;
        internal byte modelCollisionCount;
        internal byte windSettingCount;
        internal byte linkCount;
        public RotationOrder rotationOrder;
        public byte defaultSettingIdx;
        public CalculateMode calculateMode;
        public ChainAttrFlags attributeFlags;
        public ChainParamFlags paramFlags;
        public float calculateStepTime = 1;
        public bool modelCollisionSearch;
        public LegacyVersion legacyVersion;
        public byte uknAttr;
        internal byte uknCount;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref version);
            action.Do(ref magic);
            action.Do(ref errorFlags);
            action.Do(ref masterSize);
            action.Do(ref collisionAttrOffset);
            action.Do(ref modelCollisionOffset);
            action.Do(ref extraDataOffset);
            action.Do(ref groupsOffset);
            action.Do(ref linksOffset);
            action.Do(version >= 53, ref uknOffset);
            action.Do(ref settingsOffset);
            action.Do(ref windSettingsOffset);
            action.Do(ref groupCount);
            action.Do(ref settingCount);
            action.Do(ref modelCollisionCount);
            action.Do(ref windSettingCount);
            action.Do(ref linkCount);
            action.Do(ref rotationOrder);
            action.Do(ref defaultSettingIdx);
            action.Do(ref calculateMode);
            action.Do(ref attributeFlags);
            action.Do(ref paramFlags);
            action.Do(ref calculateStepTime);
            action.Do(ref modelCollisionSearch);
            action.Do(ref legacyVersion);
            action.Do(ref uknAttr);
            action.Do(ref uknCount);
            Log.WarnIf(extraDataOffset > 0, "Chain file contains extraDataOffset");
            Log.WarnIf(collisionAttrOffset > 0, "Chain file contains collisionAttrOffset");
            return true;
        }
    }

    public abstract class ChainGroupBase : BaseModel
    {
        public string name = "";
        public abstract IEnumerable<ChainNodeBase> ChainNodes { get; }

        public WindSetting? WindSettings { get; set; }

        public int settingId;
        internal byte nodeCount;
        public RotationOrder rotationOrder;
        public byte autoBlendCheckNodeNo;
        public byte windId;
        public uint terminalNameHash;
        public AttrFlags attrFlags = AttrFlags.RootRotation|AttrFlags.AngleLimit|AttrFlags.CollisionDefault|AttrFlags.WindDefault;
        public CollisionFilterFlags collisionFilterFlags;
        public Vector3 extraNodeLocalPos;

        public uint[] tags = new uint[4];
        public float dampingNoiseMin;
        public float dampingNoiseMax;
        public float endRotConstMax = 12.566f;
        public byte tagCount;
        public byte angleLimitDirectionMode;
        protected short subGroupCount;
        public int[] hierarchyHashes = new int[4];

        public uint colliderQualityLevel;
        public GenericFlagsU32 clspFlags1 = GenericFlagsU32.All;
        public GenericFlagsU32 clspFlags2 = GenericFlagsU32.All;

        protected long nodeOffset;
        protected long subGroupDataOffset;

        public override string ToString() => $"Group {name} [{terminalNameHash}]";
    }

    public class ChainGroup : ChainGroupBase
    {
        public ChainSetting? Settings { get; set; }
        public override List<ChainNode> ChainNodes { get; } = new();
        public List<ChainSubGroupData> ChainSubGroups { get; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            var version = handler.FileVersion;
            name = handler.ReadOffsetWString();

            handler.Read(ref nodeOffset);

            handler.Read(ref settingId);
            handler.Read(ref nodeCount);
            handler.Read(ref rotationOrder);
            handler.Read(ref autoBlendCheckNodeNo);
            handler.Read(ref windId);
            handler.Read(ref terminalNameHash);
            handler.Read(ref attrFlags);
            handler.Read(ref collisionFilterFlags);
            handler.Read(ref extraNodeLocalPos);

            if (version >= 35) {
                handler.ReadArray(tags);
                if (version >= 48) {
                    handler.ReadArray(hierarchyHashes);
                }
                handler.Read(ref dampingNoiseMin);
                handler.Read(ref dampingNoiseMax);
                handler.Read(ref endRotConstMax);
                handler.Read(ref tagCount);
                handler.Read(ref angleLimitDirectionMode);
                handler.Read(ref subGroupCount);
            }
            if (version >= 48) {
                handler.Read(ref colliderQualityLevel);
                handler.ReadNull(4);
                if (version >= 52) {
                    handler.Read(ref clspFlags1);
                    handler.Read(ref clspFlags2);
                }
            }
            if (version >= 44) {
                handler.Read(ref subGroupDataOffset);
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            var version = handler.FileVersion;

            handler.Skip(16); // nameOffset, nodeOffset
            handler.Write(ref settingId);
            handler.Write(ref nodeCount);
            handler.Write(ref rotationOrder);
            handler.Write(ref autoBlendCheckNodeNo);
            handler.Write(ref windId);
            handler.Write(ref terminalNameHash);
            handler.Write(ref attrFlags);
            handler.Write(ref collisionFilterFlags);
            handler.Write(ref extraNodeLocalPos);

            if (version >= 35) {
                handler.WriteArray(tags);
                if (version >= 48) {
                    handler.WriteArray(hierarchyHashes);
                }
                handler.Write(ref dampingNoiseMin);
                handler.Write(ref dampingNoiseMax);
                handler.Write(ref endRotConstMax);
                handler.Write(ref tagCount);
                handler.Write(ref angleLimitDirectionMode);
                handler.Write(ref subGroupCount);
            }
            if (version >= 48) {
                handler.Write(ref colliderQualityLevel);
                handler.WriteNull(4);
                if (version >= 52) {
                    handler.Write(ref clspFlags1);
                    handler.Write(ref clspFlags2);
                }
            }
            if (version >= 44) {
                handler.Write(ref subGroupDataOffset);
            }

            return true;
        }

        internal void ReadData(FileHandler handler)
        {
            using (var _ = handler.SeekJumpBack(nodeOffset)) {
                ChainNodes.Read(handler, nodeCount);
                foreach (var node in ChainNodes) {
                    node.ReadData(handler);
                }
            }

            using (var _ = handler.SeekJumpBack(subGroupDataOffset)) {
                ChainSubGroups.Read(handler, subGroupCount);
                foreach (var sub in ChainSubGroups) {
                    sub.ReadNodes(handler, nodeCount);
                }
            }
        }

        internal void WriteNodes(FileHandler handler)
        {
            handler.Align(16);
            handler.Write(Start, handler.Tell());
            handler.WriteWString(name);

            handler.Align(16);
            handler.Write(Start + 8, nodeOffset = handler.Tell());
            ChainNodes.Write(handler);

            foreach (var node in ChainNodes) {
                node.WriteData(handler);
                node.Rewrite(handler);
            }

            subGroupCount = (byte)ChainSubGroups.Count;
            if (ChainSubGroups.Count == 0) {
                subGroupDataOffset = 0;
                return;
            }

            subGroupDataOffset = handler.Tell();
            ChainSubGroups.Write(handler);
            foreach (var sub in ChainSubGroups) {
                sub.WriteNodes(handler);
            }
        }
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ChainSubGroupData : BaseModel
    {
        private long nodeOffset;
        public int subGroupId;
        public int settingId;

        [field: RszIgnore]
        public List<ChainNode> Nodes { get; } = [];

        public void ReadNodes(FileHandler handler, int nodeCount)
        {
            handler.Seek(nodeOffset);
            Nodes.Read(handler, nodeCount);
        }

        public void WriteNodes(FileHandler handler)
        {
            nodeOffset = handler.Tell();
            Nodes.Write(handler);
        }
    }

    public class ChainNodeBase : ReadWriteModel
    {
        public Quaternion angleLimitDirection = Quaternion.Identity;
        public float angleLimitRadius;
        public float angleLimitDistance;
        public float angleLimitRestitution;
        public float angleLimitRestituteStopSpeed = 0.1f;
        public float collisionRadius;
        public CollisionFilterFlags collisionFilterFlags = CollisionFilterFlags.All;
        public float capsuleStretchRate = 1;
        public float capsuleStretchRate2 = 1;
        public AttrFlags attributeFlag;
        public uint constraintJntNameHash;
        public float windCoef = 1;
        public AngleMode angleMode = AngleMode.LimitCone;
        public ChainNodeCollisionShape collisionShape = ChainNodeCollisionShape.Capsule;
        public byte attachType;
        public byte rotationType;

        protected long jiggleDataOffset;
        public float gravityCoef = 1;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref angleLimitDirection);
            action.Do(ref angleLimitRadius);
            action.Do(ref angleLimitDistance);
            action.Do(ref angleLimitRestitution);
            action.Do(ref angleLimitRestituteStopSpeed);
            action.Do(ref collisionRadius);
            action.Do(ref collisionFilterFlags);
            action.Do(ref capsuleStretchRate);
            action.Do(ref capsuleStretchRate2);
            action.Do(ref attributeFlag);
            action.Do(ref constraintJntNameHash);
            action.Do(ref windCoef);
            action.Do(ref angleMode);
            action.Do(ref collisionShape);
            action.Do(ref attachType);
            action.Do(ref rotationType);

            if (action.Handler.FileVersion >= 35) {
                action.Do(ref jiggleDataOffset);
                action.Do(ref gravityCoef);
                action.Null(4);
            }
            return true;
        }

        public override string ToString() => $"{collisionShape} Radius={collisionRadius}";
    }

    public class ChainNode : ChainNodeBase
    {
        public ChainJiggleData? JiggleData;

        public void ReadData(FileHandler handler)
        {
            if (jiggleDataOffset != 0) {
                handler.Seek(jiggleDataOffset);
                JiggleData = new ChainJiggleData();
                JiggleData.Read(handler);
            }
        }

        public void WriteData(FileHandler handler)
        {
            if (JiggleData == null) {
                jiggleDataOffset = 0;
                return;
            }

            jiggleDataOffset = handler.Tell();
            JiggleData.Write(handler);
        }
    }

    [RszAutoReadWrite, RszGenerate]
    public partial class ChainJiggleData : BaseModel
    {
        [RszPaddingAfter(4)]
        public Vector3 range;
        [RszPaddingAfter(4)]
        public Vector3 rangeOffset;
        public Quaternion rangeAxis = Quaternion.Identity;
        public ChainJiggleRangeShape rangeShape;
        public float springForce;
        public float gravityCoef;
        public float damping;
        public AttrFlags flags;
        public float windCoef;
    }

    public class ChainLink : ReadWriteModel
    {
        public string? nodeName1;
        public string? nodeName2;

        protected long nodeOffset;
        public uint terminalNodeNameHashA;
        public uint terminalNodeNameHashB;
        public float distanceShrinkLimitCoef;
        public float distanceExpandLimitCoef;
        public LinkMode linkMode;
        public ConnectFlags connectFlags;
        public LinkAttrFlags linkAttrFlags;
        public byte linkNodeCount;
        public byte skipGroupA;
        public byte skipGroupB;
        public RotationOrder linkOrder;

        public List<ChainLinkNode> Links { get; } = new();

        public void UpdateJointNames()
        {
            nodeName1 = Utils.HashedBoneNames.GetValueOrDefault(terminalNodeNameHashA);
            nodeName2 = Utils.HashedBoneNames.GetValueOrDefault(terminalNodeNameHashB);
        }

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref nodeOffset);
            action.Do(ref terminalNodeNameHashA);
            action.Do(ref terminalNodeNameHashB);
            action.Do(ref distanceShrinkLimitCoef);
            action.Do(ref distanceExpandLimitCoef);
            action.Do(ref linkMode);
            action.Do(ref connectFlags);
            action.Do(ref linkAttrFlags);
            action.Null(1);
            action.Do(ref linkNodeCount);
            action.Do(ref skipGroupA);
            action.Do(ref skipGroupB);
            action.Do(ref linkOrder);
            return true;
        }

        internal void ReadLinks(FileHandler handler)
        {
            handler.Seek(nodeOffset);
            Links.ReadStructList(handler, linkNodeCount);
        }

        internal void WriteLinks(FileHandler handler)
        {
            nodeOffset = Links.Count == 0 ? 0 : handler.Tell();
            handler.Write(Start, nodeOffset);
            Links.Write(handler);
        }

        public override string ToString() => $"{nodeName1 ?? terminalNodeNameHashA.ToString()} <=> {nodeName2 ?? terminalNodeNameHashB.ToString()}";
    }

    public struct ChainLinkNode
    {
        public float collisionRadius;
        public CollisionFilterFlags flags;
    }

    public class CollisionData : CollisionDataBase
    {
        public List<SubCollisionData> SubCollisions { get; } = new();

        protected override bool ReadWrite<THandler>(THandler action)
        {
            var version = action.Handler.FileVersion;
            action.Do(ref nodeOffset);
            action.Do(ref position);
            action.Do(ref pairPosition);
            if (version >= 35) {
                action.Do(ref rotationOffset);
                if (version >= 39) {
                    action.Do(ref rotationOrder);
                }
            }

            action.Do(ref jointNameHash);
            action.Do(ref pairJointNameHash);
            action.Do(ref radius);
            action.Do(version >= 48, ref endRadius);
            action.Do(ref lerp);

            if (version > 5) {
                action.Do(ref shape);
                action.Do(ref div);
                action.Do(ref subDataCount);
                action.Null(1);
                action.Do(ref collisionFilterFlags);
                if (version is 39 or 46 or 44) {
                    action.Do(ref uknInt);
                    // action.Null(4);
                }
            }

            UpdateJointNames();
            return true;
        }

        internal void ReadData(FileHandler handler)
        {
            handler.Seek(nodeOffset);
            SubCollisions.Read(handler, subDataCount);
        }

        internal void WriteData(FileHandler handler)
        {
            nodeOffset = SubCollisions.Count == 0 ? 0 : handler.Tell();
            handler.Write(Start, nodeOffset);
            SubCollisions.Write(handler);
        }
    }

    public abstract class CollisionDataBase : ReadWriteModel
    {
        public string? jointName;
        public string? pairJointName;

        protected long nodeOffset;
        public uint jointNameHash;
        public uint pairJointNameHash;
        public Vector3 position;
        public Vector3 pairPosition;
        public Quaternion rotationOffset = Quaternion.Identity;
        public RotationOrderNullable rotationOrder;
        public float radius;
        public float lerp;
        public float endRadius;

        public ChainCollisionShape shape;
        public byte div;
        public byte subDataCount;
        public CollisionFilterFlags collisionFilterFlags;
        public int uknInt;

        public void UpdateJointNames()
        {
            jointName = Utils.HashedBoneNames.GetValueOrDefault(jointNameHash);
            pairJointName = Utils.HashedBoneNames.GetValueOrDefault(pairJointNameHash);
        }

        public override string ToString() => $"{jointName ?? jointNameHash.ToString()} <=> {pairJointName ?? pairJointNameHash.ToString()}";
    }

    public class SubCollisionData : ReadWriteModel
    {
        public Vector3 position;
        public Vector3 pairPosition;
        public Quaternion rotationOffset = Quaternion.Identity;
        public float radius;
        public int id;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            if (action.Handler.FileVersion >= 35) {
                action.Do(ref position);
                action.Do(ref pairPosition);
                action.Do(ref rotationOffset);
                action.Do(ref radius);
            } else {
                action.Do(ref position);
                action.Do(ref radius);
                action.Do(ref pairPosition);
            }
            action.Do(ref id);
            return true;
        }

        public override string ToString() => $"[{id}] {position}";
    }

    public class ChainSetting : ChainSettingBase
    {
        public float sprayArc;
        public float sprayFrequency;
        public float sprayCurve1;
        public float sprayCurve2;
        public ChainType chainType;
        public EmissionDirection muzzleDirection;
        public Vector3 muzzleVelocity;

        public float ukn0;
        public float ukn1;
        public float ukn2;
        public float ukn3;

        public void EnsureUniqueId(List<ChainSetting> settings)
        {
            if (settings.Any(s => s != this && s.id == id)) {
                id = settings.Max(s => s.id) + 1;
            }
        }

        protected override bool ReadWrite<THandler>(THandler action)
        {
            var version = action.Handler.FileVersion;

            action.HandleOffsetWString(ref collisionFilterFilePath, true);
            action.Do(ref sprayArc);
            action.Do(ref sprayFrequency);
            action.Do(ref sprayCurve1);
            action.Do(ref sprayCurve2);
            action.Do(ref id);
            action.Do(ref chainType);
            action.Do(ref settingAttrFlags);
            action.Do(ref muzzleDirection);
            action.Do(ref windId);
            action.Do(ref gravity);
            action.Do(ref muzzleVelocity);
            action.Do(ref damping);
            action.Do(ref secondDamping);
            action.Do(ref secondDampingSpeed);

            if (version >= 24) {
                action.Do(ref minDamping);
                action.Do(ref secondMinDamping);
                action.Do(ref dampingPow);
                action.Do(ref secondDampingPow);
                action.Do(ref collideMaxVelocity);
            }

            action.Do(ref springForce);

            if (version >= 24) {
                action.Do(ref springLimitRate);
                action.Do(ref springMaxVelocity);
                action.Do(ref springCalcType);
                action.Do(ref springUkn1);
                action.Null(2);
            }

            action.Do(ref reduceSelfDistanceRate);
            action.Do(ref secondReduceDistanceRate);
            action.Do(ref secondReduceDistanceSpeed);
            action.Do(ref friction);
            action.Do(ref shockAbsorptionRate);
            action.Do(ref coefOfElasticity);
            action.Do(ref coefOfExternalForces);
            action.Do(ref stretchInteractionRatio);
            action.Do(ref angleLimitInteractionRatio);
            action.Do(ref shootingElasticLimitRate);
            action.Do(ref groupDefaultAttr);
            action.Do(ref windEffectCoef);
            action.Do(ref velocityLimit);
            action.Do(ref hardness);

            if (version >= 46) {
                action.Do(ref ukn0);
                action.Do(ref ukn1);
                if (version >= 52) {
                    action.Do(ref ukn2);
                    action.Do(ref ukn3);
                }
            }

            return true;
        }
    }

    public abstract class ChainSettingBase : ReadWriteModel
    {
        public WindSetting? WindSettings { get; set; }

        public string? collisionFilterFilePath;
        public int id;
        public SettingAttrFlags settingAttrFlags = SettingAttrFlags.Default;
        public byte windId;
        public Vector3 gravity = new Vector3(0, -9.8f, 0);
        public float damping = 0.047f;
        public float secondDamping = 0.05f;
        public float secondDampingSpeed;

        public float minDamping = 0.047f;
        public float secondMinDamping = 0.05f;
        public float dampingPow = 1;
        public float secondDampingPow = 1;
        public float collideMaxVelocity;

        public float springForce = 0.03f;

        public float springLimitRate;
        public float springMaxVelocity;
        public ChainSpringCalcType springCalcType;
        public byte springUkn1;

        public float reduceSelfDistanceRate = 0.73f;
        public float secondReduceDistanceRate = 0.5f;
        public float secondReduceDistanceSpeed;
        public float friction;
        public float shockAbsorptionRate;
        public float coefOfElasticity;
        public float coefOfExternalForces;
        public float stretchInteractionRatio = 0.5f;
        public float angleLimitInteractionRatio = 0.5f;
        public float shootingElasticLimitRate;
        public AttrFlags groupDefaultAttr;
        public float windEffectCoef = 1f;
        public float velocityLimit;
        public float hardness;

        public override string ToString() => $"Chain Settings {id}  {collisionFilterFilePath}";
    }

    public class WindSetting : ReadWriteModel
    {
        public int id;
        public byte windDirection;
        public byte windCount;
        public byte windType;

        public float randomDamping;
        public float randomDampingCycle;
        public float randomCycleScaling;
        public uint uknHash;
        public Vector3[] directions = new Vector3[5];
        public float[] min = new float[5];
        public float[] max = new float[5];
        public float[] phaseShift = new float[5];
        public float[] cycle = new float[5];
        public float[] interval = new float[5];

        public void EnsureUniqueId(List<WindSetting> windSettings)
        {
            if (windSettings.Any(s => s != this && s.id == id)) {
                id = windSettings.Max(s => s.id) + 1;
            }
        }

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref id);
            action.Do(ref windDirection);
            action.Do(ref windCount);
            action.Do(ref windType);
            action.Null(1);
            action.Do(ref randomDamping);
            action.Do(ref randomDampingCycle);
            action.Do(ref randomCycleScaling);
            action.Do(ref uknHash);
            action.Do(ref directions);
            action.Do(ref min);
            action.Do(ref max);
            action.Do(ref phaseShift);
            action.Do(ref cycle);
            action.Do(ref interval);
            return true;
        }

        public override string ToString() => $"Wind Settings {id}";
    }

    public class UknExtraData : ReadWriteModel
    {
        private long dataOffset;
        public float float1;
        public float float2;
        public int int1;
        public int dataCount;
        public short id;
        public short ukn1;
        public uint int2;

        public struct ExtraDataItem
        {
            public int a, b, c;
            public uint hash;

            public override string ToString() => $"ExtraDataItem {hash}";
        }

        public List<ExtraDataItem> Items { get; } = new();

        protected override bool ReadWrite<THandler>(THandler action)
        {
            dataCount = Items.Count;
            action.Do(ref dataOffset);
            action.Do(ref float1);
            action.Do(ref float2);
            action.Do(ref int1);
            action.Do(ref dataCount);
            action.Do(ref id);
            action.Do(ref ukn1);
            action.Do(ref int2);
            return true;
        }

        internal void ReadData(FileHandler handler)
        {
            handler.Seek(dataOffset);
            Items.ReadStructList(handler, dataCount);
        }

        internal void WriteData(FileHandler handler)
        {
            handler.Write(Start, dataOffset = handler.Tell());
            Items.Write(handler);
        }
    }

    [Flags]
    public enum HitFlags : byte
    {
        None = 0,
        Self = 1 << 1,
        Model = 1 << 2,
        Collider = 1 << 3,
        Angle = 1 << 4,
        Group = 1 << 5,
        VGround = 1 << 6,
        Collision = Self|Model|Collider|Group|VGround,
    }

    public enum RotationOrder : byte
    {
        XYZ = 0,
        YZX = 1,
        ZXY = 2,
        ZYX = 3,
        YXZ = 4,
        XZY = 5,
    }
    public enum RotationOrderNullable : int
    {
        XYZ = 0,
        YZX = 1,
        ZXY = 2,
        ZYX = 3,
        YXZ = 4,
        XZY = 5,
        Undefined = -1,
    }

    public enum LegacyVersion : byte
    {
        Latest = 0,
        Legacy1 = 1,
    }

    public enum CalculateMode : byte
    {
        Default = 0,
        Performance = 1,
        Balance = 2,
        Quality = 3,
    }

    [Flags]
    public enum AttrFlags : int
    {
        None = 0x0,
        RootRotation = 0x1,
        AngleLimit = 0x2,
        ExtraNode = 0x4,
        CollisionDefault = 0x8,
        CollisionSelf = 0x10,
        CollisionModel = 0x20,
        CollisionVGround = 0x40,
        CollisionCollider = 0x80,
        CollisionGroup = 0x100,
        EnablePartBlend = 0x200,
        WindDefault = 0x400,
        TransAnimation = 0x800,
        AngleLimitRestitution = 0x1000,
        StretchBoth = 0x2000,
        EndRotConstraint = 0x4000,
        Uknown16 = 0x8000,
    };

    [Flags]
    public enum GenericFlagsU32 : uint
    {
        V1 = (1 << 0),
        V2 = (1 << 1),
        V3 = (1 << 2),
        V4 = (1 << 3),
        V5 = (1 << 4),
        V6 = (1 << 5),
        V7 = (1 << 6),
        V8 = (1 << 7),
        V9 = (1 << 8),
        V10 = (1 << 9),
        V11 = (1 << 10),
        V12 = (1 << 11),
        V13 = (1 << 12),
        V14 = (1 << 13),
        V15 = (1 << 14),
        V16 = (1 << 15),
        V17 = (1 << 16),
        V18 = (1 << 17),
        V19 = (1 << 18),
        V20 = (1 << 19),
        V21 = (1 << 20),
        V22 = (1 << 21),
        V23 = (1 << 22),
        V24 = (1 << 23),
        V25 = (1 << 24),
        V26 = (1 << 25),
        V27 = (1 << 26),
        V28 = (1 << 27),
        V29 = (1 << 28),
        V30 = (1 << 29),
        V31 = (1 << 30),
        V32 = (1u << 31),
        All = ~0u
    };

    [Flags]
    public enum ChainAttrFlags
    {
        None = 0,
        ModelCollisionPreset = 1,
    }

    [Flags]
    public enum ConnectFlags : byte
    {
        None = 0,
        Neighbour = 1,
        Upper = 2,
        Bottom = 4,
    }

    public enum LinkMode : byte
    {
        TopToBottom = 0,
        BottomToTop = 1,
        Manual = 2,
    }

    public enum LinkAttrFlags : byte
    {
        None = 0,
        EnableStretch = 1,
    }

    public enum ChainParamFlags
    {
        None = 0,
        ReflectEnviromental = 1,
    }

    public enum CollisionFilterFlags
    {
        Self = 0,
        Model = 1,
        Collider = 2,
        VGround = 3,
        All = -1,
    }

    public enum ChainJiggleRangeShape
    {
        None = 0,
        OBB = 1,
        Sphere = 2,
        Cone = 3,
    }
    public enum AngleMode : byte
    {
        Free = 0,
        LimitCone = 1,
        LimitHinge = 2,
        LimitConeBox = 3,
        LimitOval = 4,
        LimitElliptic = 5,
    }

    public enum ChainCollisionShape : byte
    {
        None = 0,
        Sphere = 1,
        Capsule = 2,
        OBB = 3,
        Plane = 4,
        LineSphere = 5,
        LerpSphere = 6,
    }

    public enum ChainNodeCollisionShape : byte
    {
        None = 0,
        Sphere = 1,
        Capsule = 2,
        StretchCapsule = 3,
    }

    public enum CollisionShapeType : uint
    {
        Sphere = 0,
        Capsule = 1,
        Box = 2,
        TaperedCapsule = 3,
        Plane = 4,
        Max = 5,
        None = 6,
    }

    public enum ChainType : byte
    {
        Chain = 0,
        Shooter = 1,
    }

    public enum SettingAttrFlags : byte
    {
        None = 0,
        Default = 1,
        VirtualGroundRoot = 2,
        VirtualGroundTarget = 4,
        IgnoreSameGroupCollision = 8,
        VirtualGroundMask = VirtualGroundRoot | VirtualGroundTarget,
    }

    public enum EmissionDirection : byte
    {
        Global = 0,
        Local = 1,
        GroupLocal = 2,
    }

    public enum ChainSpringCalcType : byte
    {
        Position = 0,
        Rotation = 1,
    }
}

namespace ReeLib
{
    using ReeLib.Chain;

    public class ChainFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public Header Header { get; } = new();
        public HitFlags[] HitFlags { get; } = new HitFlags[8];
        public List<ChainGroup> Groups { get; } = [];
        public List<CollisionData> Collisions { get; } = [];
        public List<ChainLink> ChainLinks { get; } = [];
        public List<WindSetting> WindSettings { get; } = [];
        public List<ChainSetting> Settings { get; } = [];
        public List<UknExtraData> ExtraData { get; } = [];

        public const int Magic = 0x6e616863;

        protected override bool DoRead()
        {
            Groups.Clear();
            Collisions.Clear();
            ChainLinks.Clear();
            WindSettings.Clear();
            Settings.Clear();
            ExtraData.Clear();

            var handler = FileHandler;
            var header = Header;
            header.Read(handler);
            if (header.magic != Magic) {
                throw new Exception("Invalid chain file");
            }
            if (header.version == 5) {
                throw new NotSupportedException("RE7 chains not yet supported");
            }
            if (header.version > 5) {
                handler.ReadArray(HitFlags);
            }

            if (header.settingCount > 0) {
                handler.Seek(header.settingsOffset);
                Settings.Read(handler, header.settingCount);
            }

            if (header.groupCount > 0) {
                handler.Seek(header.groupsOffset);
                Groups.Read(handler, header.groupCount);
                foreach (var group in Groups) {
                    group.ReadData(handler);
                }
            }

            if (header.modelCollisionOffset > 0) {
                handler.Seek(header.modelCollisionOffset);
                Collisions.Read(handler, header.modelCollisionCount);
                foreach (var coll in Collisions) {
                    coll.ReadData(handler);
                }
            }

            if (header.windSettingCount > 0) {
                handler.Seek(header.windSettingsOffset);
                WindSettings.Read(handler, header.windSettingCount);
            }

            if (header.linkCount > 0) {
                handler.Seek(header.linksOffset);
                ChainLinks.Read(handler, header.linkCount);
                foreach (var link in ChainLinks) {
                    link.ReadLinks(handler);
                }
            }

            if (header.uknCount > 0 && header.uknOffset > 0) {
                handler.Seek(header.uknOffset);
                ExtraData.Read(handler, header.uknCount);
                foreach (var extra in ExtraData) {
                    extra.ReadData(handler);
                }
            }

            foreach (var setting in Settings) {
                setting.WindSettings = WindSettings.FirstOrDefault(w => w.id == setting.windId);
            }

            foreach (var group in Groups) {
                // note: some cases have no matching setting despite > 0 settingId, what's up with that? (re3 pl1000.chain.24)
                group.Settings = Settings.FirstOrDefault(s => s.id == group.settingId);
                group.WindSettings = WindSettings.FirstOrDefault(ws => ws.id == group.windId);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var header = Header;
            header.Write(handler);
            if (header.version > 5) {
                handler.WriteArray(HitFlags);
            }

            foreach (var grp in Groups) {
                if (grp.Settings != null) {
                    if (!Settings.Contains(grp.Settings)) {
                        Settings.Add(grp.Settings);
                    }
                    grp.settingId = grp.Settings.id;
                }

                if (grp.WindSettings != null && !WindSettings.Contains(grp.WindSettings)) {
                    WindSettings.Add(grp.WindSettings);
                    grp.windId = (byte)grp.WindSettings.id;
                }
                grp.nodeCount = (byte)grp.ChainNodes.Count;
            }

            foreach (var setting in Settings) {
                if (setting.WindSettings != null && !WindSettings.Contains(setting.WindSettings)) {
                    WindSettings.Add(setting.WindSettings);
                    setting.windId = (byte)setting.WindSettings.id;
                }
            }

            header.settingCount = (byte)Settings.Count;
            header.windSettingCount = (byte)WindSettings.Count;
            header.groupCount = (byte)Groups.Count;
            header.modelCollisionCount = (byte)Collisions.Count;
            header.linkCount = (byte)ChainLinks.Count;

            header.settingsOffset = handler.Tell();
            Settings.Write(handler);
            handler.StringTableFlush();

            handler.Align(16);
            header.modelCollisionOffset = handler.Tell();
            Collisions.Write(handler);
            foreach (var coll in Collisions) {
                coll.WriteData(handler);
            }

            header.groupsOffset = handler.Tell();
            Groups.Write(handler);
            foreach (var group in Groups) {
                group.WriteNodes(handler);
            }

            handler.Align(16);
            header.windSettingsOffset = handler.Tell();
            WindSettings.Write(handler);

            header.linksOffset = ChainLinks.Count == 0 ? 0 : handler.Tell();
            ChainLinks.Write(handler);

            foreach (var link in ChainLinks) {
                link.WriteLinks(handler);
            }

            header.uknOffset = ExtraData.Count == 0 ? 0 : handler.Tell();
            ExtraData.Write(handler);
            foreach (var extra in ExtraData) {
                extra.WriteData(handler);
            }

            header.Write(handler, 0);
            return true;
        }
    }
}