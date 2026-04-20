using System.Numerics;
using ReeLib.Chain;

namespace ReeLib.Chain2
{
    using ReeLib.Common;
    using ReeLib.InternalAttributes;

    public class Header : Chain.Header
    {
        internal long freeLinksOffset;

        public byte taperedCollideMethod;
        public byte freeLinkCount;
        public byte freeLinkJoint;
        public HitFlags[] HitFlags = new HitFlags[8];

        public short wildsUkn1;
        public short highFpsCalculateMode;
        public byte wildsUkn3;
        public byte wildsUkn4;

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
            action.Do(ref freeLinksOffset);
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
            action.Do(ref taperedCollideMethod);
            action.Do(ref freeLinkCount);
            action.Do(ref freeLinkJoint);
            action.Do(ref HitFlags);
            if (version >= 13) {
                action.Do(ref wildsUkn1);
                action.Do(ref highFpsCalculateMode);
                action.Do(ref wildsUkn3);
                action.Do(ref wildsUkn4);
                action.Null(10);
            }
            Log.WarnIf(extraDataOffset > 0, "Chain file contains extraDataOffset");
            Log.WarnIf(collisionAttrOffset > 0, "Chain file contains collisionAttrOffset");
            return true;
        }
    }

    public class Chain2Setting : Chain.ChainSettingBase
    {
        public float motionForce;
        public byte windDelayType;
        public float windDelaySpeed;

        public float envWindEffectCoef;

        private int subDataCount;
        public Quaternion subDataRotation;
        private long subDataOffset;
        private uint ukn4;
        private uint ukn5;

        public List<Chain2SettingSubData> SubData { get; } = new();

        public void EnsureUniqueId(List<Chain2Setting> settings)
        {
            if (settings.Any(s => s != this && s.id == id)) {
                id = settings.Max(s => s.id) + 1;
            }
        }

        protected override bool ReadWrite<THandler>(THandler action)
        {
            var version = action.Handler.FileVersion;

            action.HandleOffsetWString(ref collisionFilterFilePath, true);
            action.Do(ref id);
            action.Do(ref settingAttrFlags);
            action.Do(ref springCalcType);
            action.Do(ref windId);
            action.Do(ref windDelayType);
            action.Do(ref gravity);
            action.Do(ref springMaxVelocity);
            action.Do(ref damping);
            action.Do(ref secondDamping);
            action.Do(ref secondDampingSpeed);
            action.Do(ref minDamping);
            action.Do(ref secondMinDamping);
            action.Do(ref dampingPow);
            action.Do(ref secondDampingPow);
            action.Do(ref collideMaxVelocity);
            action.Do(ref springForce);
            action.Do(ref springLimitRate);
            action.Do(ref reduceSelfDistanceRate);
            action.Do(ref secondReduceDistanceRate);
            action.Do(ref secondReduceDistanceSpeed);
            action.Do(ref friction);
            action.Do(ref shockAbsorptionRate);
            action.Do(ref coefOfElasticity);
            action.Do(ref coefOfExternalForces);
            action.Do(ref stretchInteractionRatio);
            action.Do(ref angleLimitInteractionRatio);

            action.Do(ref motionForce);
            action.Do(ref groupDefaultAttr);
            action.Do(ref windEffectCoef);
            action.Do(ref velocityLimit);
            action.Do(ref hardness);
            action.Do(ref windDelaySpeed);
            action.Do(ref envWindEffectCoef);

            if (version >= 13) {
                action.Do(ref subDataCount);
                action.Do(ref subDataRotation); // "probably" a rotation
                DataInterpretationException.DebugWarnIf(subDataRotation != Quaternion.Identity);
                action.Null(12); // might be a vector3
                action.Do(ref subDataOffset);

                // might be >=15 only?
                action.Do(ref ukn4);
                action.Do(ref ukn5);
            }

            return true;
        }

        internal void ReadData(FileHandler handler)
        {
            handler.Seek(subDataOffset);
            SubData.ReadStructList(handler, subDataCount);
        }

        internal void WriteData(FileHandler handler)
        {
            handler.Align(8);
            var strTell = 0L;
            if (!string.IsNullOrEmpty(collisionFilterFilePath)) {
                strTell = handler.Tell();
                handler.Write(Start, strTell);
                handler.WriteWString(collisionFilterFilePath);
            }

            if (SubData.Count == 0) {
                subDataOffset = 0;
            } else {
                handler.Align(8);
                subDataOffset = handler.Tell();
                SubData.Write(handler);
            }

            this.Rewrite(handler);
            handler.Write(Start, strTell);
        }

        public override string ToString() => $"Chain Settings {id}  {collisionFilterFilePath}";
    }

    public struct Chain2SettingSubData
    {
        public int ukn0;
        public byte ukn1_a;
        public byte ukn1_b;
        public byte ukn1_c;
        public byte ukn1_d;
        public int ukn2;
        public int ukn3;
    }

    public class Chain2Group : Chain.ChainGroupBase
    {
        public Chain2Setting? Settings { get; set; }
        public override List<Chain2Node> ChainNodes { get; } = new();
        public List<ChainSubGroupData> ChainSubGroups { get; } = new();

        public int interpCount;
        public int nodeInterpolationMode;

        protected override bool DoRead(FileHandler handler)
        {
            var version = handler.FileVersion;

            long nodeOffset = handler.Read<long>();

            handler.Read(ref settingId);
            handler.Read(ref nodeCount);
            handler.Read(ref rotationOrder);
            handler.Read(ref autoBlendCheckNodeNo);
            handler.Read(ref windId);
            handler.ReadArray(tags);
            handler.ReadArray(hierarchyHashes);
            handler.Read(ref dampingNoiseMin);
            handler.Read(ref dampingNoiseMax);
            handler.Read(ref endRotConstMax);
            handler.Read(ref tagCount);
            handler.Read(ref angleLimitDirectionMode);
            handler.Read(ref subGroupCount);
            handler.Read(ref colliderQualityLevel);

            handler.Read(ref attrFlags);
            handler.Read(ref clspFlags1);
            handler.Read(ref clspFlags2);
            handler.Read(ref subGroupDataOffset);

            handler.Read(ref terminalNameHash);
            handler.Read(ref interpCount);
            handler.Read(ref nodeInterpolationMode);

            handler.ReadNull(4);

            ReadData(handler, nodeOffset);
            name = Utils.HashedBoneNames.GetValueOrDefault(terminalNameHash) ?? "";
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            var version = handler.FileVersion;

            handler.Skip(8); // nodeOffset
            handler.Write(ref settingId);
            handler.Write(ref nodeCount);
            handler.Write(ref rotationOrder);
            handler.Write(ref autoBlendCheckNodeNo);
            handler.Write(ref windId);
            handler.WriteArray(tags);
            handler.WriteArray(hierarchyHashes);
            handler.Write(ref dampingNoiseMin);
            handler.Write(ref dampingNoiseMax);
            handler.Write(ref endRotConstMax);
            handler.Write(ref tagCount);
            handler.Write(ref angleLimitDirectionMode);
            handler.Write(ref subGroupCount);
            handler.Write(ref colliderQualityLevel);

            handler.Write(ref attrFlags);
            handler.Write(ref clspFlags1);
            handler.Write(ref clspFlags2);
            handler.Write(ref subGroupDataOffset);

            handler.Write(ref terminalNameHash);
            handler.Write(ref interpCount);
            handler.Write(ref nodeInterpolationMode);

            handler.WriteNull(4);


            return true;
        }

        private void ReadData(FileHandler handler, long nodeOffset)
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

        internal void WriteData(FileHandler handler)
        {
            handler.Align(16);
            handler.Write(Start, handler.Tell()); // nodeOffset
            ChainNodes.Write(handler);

            foreach (var node in ChainNodes) {
                node.WriteData(handler);
                node.Rewrite(handler);
            }

            subGroupCount = (byte)ChainSubGroups.Count;
            handler.Align(16);
            subGroupDataOffset = handler.Tell();
            if (ChainSubGroups.Count == 0) {
                return;
            }

            if (ChainSubGroups.Count > 0) {
                ChainSubGroups.Write(handler);
                foreach (var sub in ChainSubGroups) {
                    sub.WriteNodes(handler);
                }
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(name)) {
                return $"Group [{terminalNameHash}]";
            }
            return base.ToString();
        }
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ChainSubGroupData : BaseModel
    {
        private long nodeOffset;
        public int subGroupId;
        public int settingId;

        [field: RszIgnore]
        public List<Chain2Node> Nodes { get; } = [];

        public void ReadNodes(FileHandler handler, int nodeCount)
        {
            handler.Seek(nodeOffset);
            Nodes.Read(handler, nodeCount);
            foreach (var node in Nodes) {
                node.ReadData(handler);
            }
        }

        public void WriteNodes(FileHandler handler)
        {
            handler.Align(16);
            handler.Write(Start, nodeOffset = handler.Tell());
            Nodes.Write(handler);
            foreach (var node in Nodes) {
                node.WriteData(handler);
                node.Rewrite(handler);
            }
        }
    }

    public class Chain2Node : Chain.ChainNodeBase
    {
        public uint jointHash;
        public Vector3 basePos;

        public Chain2JiggleData? JiggleData;

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
            action.Do(ref jiggleDataOffset);
            action.Do(ref gravityCoef);
            action.Do(ref jointHash);
            action.Do(ref basePos);
            action.Null(4);
            return true;
        }

        public void ReadData(FileHandler handler)
        {
            if (jiggleDataOffset != 0) {
                handler.Seek(jiggleDataOffset);
                JiggleData = new Chain2JiggleData();
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
    public partial class Chain2JiggleData : BaseModel
    {
        [RszPaddingAfter(4)]
        public Vector3 range;
        [RszPaddingAfter(4)]
        public Vector3 rangeOffset;
        public Quaternion rangeAxis;
        public ChainJiggleRangeShape rangeShape;
        public AttrFlags flags;
        public float springForce;
        public float gravityCoef;
        public float damping;
        public float windCoef;
    }

    public class Chain2Link : ChainLink
    {
        public uint clspFlags1;
        public uint clspFlags2;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref nodeOffset);
            action.Do(ref terminalNodeNameHashA);
            action.Do(ref terminalNodeNameHashB);
            action.Do(ref distanceShrinkLimitCoef);
            action.Do(ref distanceExpandLimitCoef);
            if (action.Handler.FileVersion >= 13) {
                action.Do(ref clspFlags1);
                action.Do(ref clspFlags2);
            }
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

        public override string ToString() => $"{nodeName1 ?? terminalNodeNameHashA.ToString()} <=> {nodeName2 ?? terminalNodeNameHashB.ToString()}";
    }

    public class CollisionData : Chain.CollisionDataBase
    {
        public List<SubCollisionData> SubCollisions { get; } = new();

        protected override bool ReadWrite<THandler>(THandler action)
        {
            var version = action.Handler.FileVersion;
            action.Do(ref nodeOffset);
            action.Do(ref position);
            action.Do(ref pairPosition);
            action.Do(ref rotationOffset);
            action.Do(ref rotationOrder);

            action.Do(ref jointNameHash);
            action.Do(ref pairJointNameHash);
            action.Do(ref radius);
            action.Do(ref endRadius);
            action.Do(ref lerp);

            action.Do(ref shape);
            action.Do(ref div);
            action.Do(ref subDataCount);
            action.Null(1);
            action.Do(ref collisionFilterFlags);
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

    public class SubCollisionData : ReadWriteModel
    {
        public Vector3 position;
        public Vector3 pairPosition;
        public Quaternion rotationOffset = Quaternion.Identity;
        public float radius;
        public int id;
        public uint subdata0;
        public uint subdata1;
        public uint subdata2;
        public uint subdata3;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref position);
            action.Do(ref pairPosition);
            action.Do(ref rotationOffset);
            action.Do(ref radius);
            action.Do(ref id);
            action.Do(ref subdata0);
            action.Do(ref subdata1);
            action.Do(ref subdata2);
            action.Do(ref subdata3);
            return true;
        }

        public override string ToString() => $"[{id}] {position}";
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
}


namespace ReeLib
{
    using ReeLib.Chain2;
    using ReeLib.Common;

    public class Chain2File(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public Header Header { get; } = new();
        public List<Chain2Group> Groups { get; } = [];
        public List<CollisionData> Collisions { get; } = [];
        public List<Chain2Link> ChainLinks { get; } = [];
        public List<WindSetting> WindSettings { get; } = [];
        public List<Chain2Setting> Settings { get; } = [];
        public List<UknExtraData> ExtraData { get; } = [];

        public const int Magic = 0x326e6863;

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
                throw new Exception("Invalid chain2 file");
            }

            if (header.settingCount > 0) {
                handler.Seek(header.settingsOffset);
                Settings.Read(handler, header.settingCount);
                foreach (var setting in Settings) {
                    setting.ReadData(handler);
                }
            }

            if (header.groupCount > 0) {
                handler.Seek(header.groupsOffset);
                Groups.Read(handler, header.groupCount);
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

            if (header.freeLinkCount > 0) {
                Log.Warn("Chain2 file contains free link data that is currently unhandled!");
            }
            if (header.extraDataOffset > 0) {
                Log.Warn("Chain2 file contains extra data that is currently unhandled!");
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
            handler.StringTable?.Clear();
            foreach (var setting in Settings) {
                setting.WriteData(handler);
            }

            handler.Align(16);
            header.modelCollisionOffset = handler.Tell();
            Collisions.Write(handler);
            foreach (var coll in Collisions) {
                coll.WriteData(handler);
            }

            header.groupsOffset = handler.Tell();
            Groups.Write(handler);
            foreach (var group in Groups) {
                group.WriteData(handler);
                group.Rewrite(handler);
            }

            handler.Align(16);
            header.windSettingsOffset = handler.Tell();
            WindSettings.Write(handler);

            header.linksOffset = ChainLinks.Count == 0 ? 0 : handler.Tell();
            ChainLinks.Write(handler);

            foreach (var link in ChainLinks) {
                link.WriteLinks(handler);
            }

            header.Write(handler, 0);
            return true;
        }
    }
}