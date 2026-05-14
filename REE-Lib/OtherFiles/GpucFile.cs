using System.Numerics;
using System.Runtime.InteropServices;
using ReeLib.Chain;
using ReeLib.Common;
using ReeLib.InternalAttributes;
using ReeLib.via;

namespace ReeLib.Gpuc
{
    public enum GpucVersion
    {
        RE8 = 62,
        RE_RT = 65,
        RE4 = 221108868,
        DD2 = 231011972,
        SF6 = 230110953,
        MHWILDS = 241111744,
        MHST3 = 250604242,
        PragmataDemo = 250925361,
        RE9 = 250925365,
        Pragmata = 251121978,
    }

    public class Header : ReadWriteModel
    {
        public uint magic;
        public uint meshResourcePathHash;
        public uint meshResourcePathHash2;
        public int cpuMemorySize;
        internal int numControlPoints;
        internal int numTriangles;
        internal int numDistanceLinks;
        internal int numCollisionEdges;
        internal int numTriangleGroups;
        internal int numDistanceLinkGroups;
        internal int numCollisionEdgeGroups;
        internal int numPartInfos;
        internal int numBatchInfos;
        internal int numBatchSubInfos;
        internal int numConfigInfos;
        internal int numDeformInfos;
        internal int numDeformWeights;
        internal int numTotalRenderVertices;
        public int lodCount;
        public int numUkn;
        public int numUkn1;
        internal int numBatchGroup;
        public int numDeformBones;
        internal ushort numCollisionPlanes;
        internal ushort numCollisionSpheres;
        internal ushort numCollisionCapsules;
        internal ushort numCollisionOBBs;
        public ClothCalculateMode calculateMode;
        public float friction;
        public float bounce;
        public float stickiness;
        public float maxVelocity;
        public ushort numNativeJoints;
        public ushort maxJointCount;
        public int maxBatchControlPoints;
        public int maxDeformWeightCount;
        public int numPointTriangleContactDescs;
        public int numEdgeEdgeContactDescs;
        public int numVertexDeformInfos;
        public uint status;

        public AABB domain;
        internal long controlPointTbl;
        internal long triangleTbl;
        internal long distanceLinkTbl;
        internal long collisionEdgeTbl;
        internal long numTrianglesInGroupTbl;
        internal long numDistanceLinksInGroupTbl;
        internal long numCollisionEdgesInGroupTbl;
        internal long partInfoTbl;
        internal long batchInfoTbl;
        internal long configInfoTbl;
        internal long batchSubInfoTbl;
        internal long deformBonesTbl;
        internal long keyFrameTbl;
        internal long deformInfoTbl;
        internal long blendInfoTbl;
        internal long blendMultiMatricesTbl;
        internal long blendControlPointMatricesTbl;
        internal long vertexIdToDeformInfoIndexTbl;
        internal long batchGroupTbl;
        internal long collisionPlaneTbl;
        internal long collisionSphereTbl;
        internal long collisionCapsuleTbl;
        internal long collisionOBBTbl;
        internal long debugControlPointTbl;
        internal long offsUnused1;
        internal long offsUnused2;
        internal long offsUnused3;
        internal long debugDistanceLinkTbl;
        internal long pointTriangleContactDescTbl;
        internal long edgeEdgeContactDescTbl;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            var version = (GpucVersion)action.Version;
            action.Do(ref magic);
            if (action is FileHandlerRead && action.Handler.IsEnd) {
                // there are some empty gpuc files that only contain the signature bytes and nothing else
                return true;
            }
            if (version >= GpucVersion.RE9) {
                action.Do(ref cpuMemorySize);
                action.Do(ref meshResourcePathHash);
                action.Do(ref meshResourcePathHash2);
                action.Null(12);
            } else if (version >= GpucVersion.DD2) {
                action.Do(ref meshResourcePathHash);
                action.Do(ref cpuMemorySize);
                if (version != GpucVersion.MHST3)
                    action.Null(8);
            }
            action.Do(ref numControlPoints);
            action.Do(ref numTriangles);
            action.Do(ref numDistanceLinks);
            action.Do(ref numCollisionEdges);
            action.Do(version < GpucVersion.MHST3, ref numTriangleGroups);
            action.Do(ref numDistanceLinkGroups);
            action.Do(ref numCollisionEdgeGroups);
            action.Do(ref numPartInfos);
            action.Do(ref numBatchInfos);
            action.Do(version >= GpucVersion.MHST3, ref numBatchSubInfos);
            action.Do(ref numConfigInfos);
            action.Do(ref numDeformInfos);
            if (version >= GpucVersion.MHST3) {
                action.Do(ref numDeformWeights);
                action.Do(ref numUkn);
                action.Do(ref numTotalRenderVertices);
                action.Do(ref numUkn1);
                action.Do(ref numBatchGroup);
                action.Null(8);
                action.Do(ref lodCount);
                if (version == GpucVersion.MHST3) {
                    action.Do(ref numDeformBones);
                    action.Null(4);
                } else {
                    action.Null(4);
                    action.Do(ref numDeformBones);
                }
            } else if (version >= GpucVersion.RE4) {
                action.Do(ref numDeformWeights);
                action.Do(ref numTotalRenderVertices);
                action.Do(ref lodCount);
            }
            action.Do(ref numCollisionPlanes);
            action.Do(ref numCollisionSpheres);
            action.Do(ref numCollisionCapsules);
            action.Do(ref numCollisionOBBs);
            action.Do(ref calculateMode);
            action.Do(ref friction);
            action.Do(ref bounce);
            action.Do(ref stickiness);
            action.Do(ref maxVelocity);
            action.Do(ref numNativeJoints);
            action.Do(ref maxJointCount);
            action.Do(ref maxBatchControlPoints);
            action.Do(version < GpucVersion.MHST3, ref maxDeformWeightCount);
            action.Do(ref numPointTriangleContactDescs);
            action.Do(ref numEdgeEdgeContactDescs);
            action.Do(ref status);
            action.Handler.Align(16);

            action.Do(ref domain);
            action.Do(ref controlPointTbl);
            action.Do(ref triangleTbl);
            action.Do(ref distanceLinkTbl);
            action.Do(ref collisionEdgeTbl);
            action.Do(version < GpucVersion.MHST3, ref numTrianglesInGroupTbl);
            action.Do(ref numDistanceLinksInGroupTbl);
            action.Do(ref numCollisionEdgesInGroupTbl);
            action.Do(ref partInfoTbl);
            action.Do(ref batchInfoTbl);
            action.Do(ref configInfoTbl);

            if (version >= GpucVersion.DD2) {
                if (version >= GpucVersion.MHST3) {
                    action.Null(8);
                    action.Do(ref batchSubInfoTbl);
                    action.Do(ref deformBonesTbl);
                }
                action.Do(ref keyFrameTbl);
            } else {
                action.Do(ref deformInfoTbl);
            }
            if (version >= GpucVersion.MHST3) {
                action.Do(ref batchGroupTbl);
            }
            action.Do(ref collisionPlaneTbl);
            action.Do(ref collisionSphereTbl);
            action.Do(ref collisionCapsuleTbl);
            if (version >= GpucVersion.DD2) {
                action.Do(ref collisionOBBTbl);
            }
            if (version < GpucVersion.DD2) {
                action.Do(ref debugControlPointTbl);
                action.Do(ref debugDistanceLinkTbl);
                action.Do(ref pointTriangleContactDescTbl);
                action.Do(ref edgeEdgeContactDescTbl);
                if (version == GpucVersion.RE4) {
                    action.Do(ref numVertexDeformInfos);
                }
            } else {
                if (version >= GpucVersion.MHST3) {
                    action.Do(ref offsUnused1);
                    action.Do(ref offsUnused2);
                }
                action.Do(ref pointTriangleContactDescTbl);
                action.Do(ref edgeEdgeContactDescTbl);
                if (version >= GpucVersion.RE9) {
                    action.Do(ref offsUnused3);
                }
                action.Do(ref deformInfoTbl);
                action.Do(ref blendInfoTbl);
                if (version >= GpucVersion.MHST3) {
                    action.Do(ref blendMultiMatricesTbl);
                    action.Do(ref blendControlPointMatricesTbl);
                }
                action.Do(ref vertexIdToDeformInfoIndexTbl);
                action.Do(ref debugControlPointTbl);
                action.Do(ref debugDistanceLinkTbl);
            }
            return true;
        }
    }

    public class ControlPoint : ReadWriteModel
    {
        public float weight;
        public float radius;
        public float inverseMass;
        public float maxDistance;
        public float backstopRadius;
        public float backstopOffset;
        public Vector2 uknFloats;
        public int deltaPositionOffset;
        public int numDeltaPositions;
        public uint collisionPlaneBits;
        public uint collisionSphereBits;
        public uint collisionCapsuleBits;
        public int partId;
        public int configId;
        public short[] controlPointIndices = new short[4];
        public int[] initialDistances = new int[4];
        public Vector3 position;
        public uint normal;
        public int directionControlPointIndex;
        public byte[] jointIndices = new byte[8];
        public byte[] jointWeights = new byte[8];

        public ConfigInfo? Config { get; set; }

        public DebugControlPoint DebugData { get; } = new DebugControlPoint();

        protected override bool ReadWrite<THandler>(THandler action)
        {
            var version = (GpucVersion)action.Version;
            action.Do(ref weight);
            action.Do(ref radius);
            action.Do(ref inverseMass);
            action.Do(ref maxDistance);
            action.Do(ref backstopRadius);
            if (version >= GpucVersion.DD2) {
                action.Do(ref backstopOffset);
                if (version >= GpucVersion.MHST3) {
                    action.Do(ref uknFloats);
                }
                action.Do(ref deltaPositionOffset);
                action.Do(ref numDeltaPositions);
            } else {
                action.Do(ref collisionPlaneBits);
                action.Do(ref collisionSphereBits);
                action.Do(ref collisionCapsuleBits);
            }
            action.Do(ref partId);
            action.Do(ref configId);
            action.Do(ref controlPointIndices);
            action.Do(ref initialDistances);
            action.Do(ref position);
            action.Do(ref normal);
            if (version >= GpucVersion.RE_RT) {
                action.Do(ref directionControlPointIndex);
            }

            action.Do(ref jointIndices);
            action.Do(ref jointWeights);
            return true;
        }
    }

    public class BatchInfo : ReadWriteModel
    {
        public int numControlPoints;
        public int numTriangles;
        public int numDistanceLinks;
        public int numCollisionEdges;
        public int numTriangleGroups;
        public int numDistanceLinkGroups;
        public int numCollisionEdgeGroups;
        public int numDeformInfos;
        public int numPointTriangleContactDescs;
        public int numEdgeEdgeContactDescs;

        public int controlPointOffset;
        public int triangleOffset;
        public int distanceLinkOffset;
        public int collisionEdgeOffset;
        public int triangleGroupOffset;
        public int distanceLinkGroupOffset;
        public int collisionEdgeGroupOffset;
        public int deformInfoOffset;
        public int pointTriangleContactDescOffset;
        public int edgeEdgeContactDescOffset;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            var version = (GpucVersion)action.Version;
            action.Do(ref numControlPoints);
            action.Do(ref numTriangles);
            action.Do(ref numDistanceLinks);
            action.Do(ref numCollisionEdges);
            action.Do(version < GpucVersion.MHST3, ref numTriangleGroups);
            action.Do(ref numDistanceLinkGroups);
            action.Do(ref numCollisionEdgeGroups);
            action.Do(ref numDeformInfos);
            action.Do(ref numPointTriangleContactDescs);
            action.Do(ref numEdgeEdgeContactDescs);

            action.Do(ref controlPointOffset);
            action.Do(ref triangleOffset);
            action.Do(ref distanceLinkOffset);
            action.Do(ref collisionEdgeOffset);
            action.Do(version < GpucVersion.MHST3, ref triangleGroupOffset);
            action.Do(ref distanceLinkGroupOffset);
            action.Do(ref collisionEdgeGroupOffset);
            action.Do(ref deformInfoOffset);
            action.Do(ref pointTriangleContactDescOffset);
            action.Do(ref edgeEdgeContactDescOffset);
            if (version != GpucVersion.RE4) action.Handler.Align(16);
            return true;
        }
    }

    public class PartInfo : ReadWriteModel
    {
        public string name = "";
        public uint nameHash;
        public int configId;

        public ulong tagBit;
        public PartFlags flags;

        public string? baseJointName;
        public uint baseJointNameHash;
        public float friction;
        public float bounce;
        public float stickiness;
        public float longRangeAttachmentStretchiness;
		public GenericFlagsU32 resourceCollisionPlaneBits;
		public GenericFlagsU32 resourceCollisionSphereBits;
		public GenericFlagsU32 resourceCollisionCapsuleBits;
        public GenericFlagsU32 resourceCollisionOBBBits;

        public ConfigInfo? Config { get; internal set; }

        public void UpdateJointNames()
        {
            baseJointName = Utils.HashedBoneNames.GetValueOrDefault(baseJointNameHash);
        }

        protected override bool ReadWrite<THandler>(THandler action)
        {
            var version = (GpucVersion)action.Version;
            if (version != GpucVersion.DD2) {
                action.HandleOffsetWString(ref name);
            }
            action.Do(ref nameHash);
            action.Do(ref configId);
            action.Do(version >= GpucVersion.RE4, ref tagBit);
            if (version >= GpucVersion.DD2) {
                action.Do(ref flags);
            } else {
                action.HandleOffsetWString(ref baseJointName, true);
            }
            action.Do(ref baseJointNameHash);
            action.Do(ref friction);
            action.Do(ref bounce);
            action.Do(ref stickiness);
            action.Do(ref longRangeAttachmentStretchiness);
            if (version >= GpucVersion.RE4) {
                action.Do(ref resourceCollisionPlaneBits);
                action.Do(ref resourceCollisionSphereBits);
                action.Do(ref resourceCollisionCapsuleBits);
                if (version >= GpucVersion.DD2) {
                    action.Do(ref resourceCollisionOBBBits);
                    if (version == GpucVersion.DD2) {
                        action.Null(8);
                    }
                }
            } else {
                action.Null(4);
            }
            return true;
        }

        public override string ToString() => !string.IsNullOrEmpty(name) ? name : $"Part {nameHash}";
    }

    public class ConfigInfo : ReadWriteModel
    {
        public string name = "";
        public uint nameHash;
        public Guid guid;
        public ConfigFlags flags;
        public ConfigFlags2 flags2;

        public float velocityDamping;
        public float secondDamping;
        public float secondDampingSpeed;
        public int uknInt1;
        public int uknInt2;
        public int uknInt3;
        public float blendAmountTranslation;
        public float blendAmountRotation;
        public float simulationStrength;
        public float simulationBlend;
        public float gravityFactor;
        public uint status;

		public float maxTranslationSpeed;
		public float maxRotationAngularSpeed;
        public AnimationCurveData blendAmountTranslationCurve = new();
        public AnimationCurveData blendAmountRotationCurve = new();

        public bool useSpeedLimit;
        public float speedLimit;
        public bool useAngularSpeedLimit;
        public float angularSpeedLimit;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            var version = (GpucVersion)action.Version;
            if (version < GpucVersion.DD2) {
                action.HandleOffsetWString(ref name);
            }
            action.Do(ref nameHash);
            action.Handler.Align(16);
            if (version == GpucVersion.RE4) action.Null(4);
            action.Do(ref guid);
            action.Do(ref flags);
            action.Do(ref velocityDamping);
            if (version >= GpucVersion.DD2) {
                action.Do(ref flags2);
                action.Do(ref secondDamping);
                action.Do(ref secondDampingSpeed);
                action.Do(ref uknInt1);
                action.Do(ref uknInt2);
                action.Do(ref uknInt3);
            }
            action.Do(ref blendAmountTranslation);
            action.Do(ref blendAmountRotation);
            if (version >= GpucVersion.DD2) {
                action.Do(ref maxTranslationSpeed);
                action.Do(ref maxRotationAngularSpeed);
                action.Handle(blendAmountTranslationCurve);
                action.Handle(blendAmountRotationCurve);
                if (version >= GpucVersion.MHST3) {
                    action.Do(ref useSpeedLimit);
                    action.Null(3);
                    action.Do(ref speedLimit);
                    action.Do(ref useAngularSpeedLimit);
                    action.Null(3);
                    action.Do(ref angularSpeedLimit);
                }
            }
            action.Do(ref simulationStrength);
            action.Do(ref simulationBlend);
            action.Do(ref gravityFactor);
            action.Do(ref status);
            return true;
        }

        public override string ToString() => !string.IsNullOrEmpty(name) ? name : $"[{guid}] Config {nameHash}";
    }

    public class ClothTriangle : ReadWriteModel
    {
        public int groupIndex;

        public ushort indexA;
        public ushort indexB;
        public ushort indexC;
        public ushort flags;

        public ushort[] addIndices = new ushort[6];

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref indexA);
            action.Do(ref indexB);
            action.Do(ref indexC);
            action.Do(ref flags);
            if ((GpucVersion)action.Version >= GpucVersion.MHST3) {
                action.Do(ref addIndices);
            }
            return true;
        }

        public override string ToString() => $"[{groupIndex}] {indexA}, {indexB}, {indexC}";
    }

    public class DistanceLink : BaseModel
    {
        public int groupIndex;

        public int DebugID { get; set; }

        public ushort indexA;
        public ushort indexB;
        public float restLength;

        public float stiffness;
        public float stiffnessMultiplier;
        public float stretchLimit;
        public float compressionLimit;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref indexA);
            handler.Read(ref indexB);
            handler.Read(ref restLength);
            stiffness = handler.Read<ushort>() * (1f / ushort.MaxValue);
            stiffnessMultiplier = handler.Read<ushort>() * (1f / ushort.MaxValue);
            stretchLimit = handler.Read<ushort>() * (1f / ushort.MaxValue);
            compressionLimit = handler.Read<ushort>() * (1f / ushort.MaxValue);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref indexA);
            handler.Write(ref indexB);
            handler.Write(ref restLength);
            handler.Write((ushort)Math.Round(stiffness * ushort.MaxValue));
            handler.Write((ushort)Math.Round(stiffnessMultiplier * ushort.MaxValue));
            handler.Write((ushort)Math.Round(stretchLimit * ushort.MaxValue));
            handler.Write((ushort)Math.Round(compressionLimit * ushort.MaxValue));
            return true;
        }

        public override string ToString() => $"[{groupIndex}] {indexA} <=> {indexB}";
    }

    public class CollisionEdge : ReadWriteModel
    {
        public int groupIndex;

        public ushort indexA;
        public ushort indexB;
        public ushort deltaPositionIndexA;
        public ushort deltaPositionIndexB;

        public override string ToString() => $"[{groupIndex}] {indexA} <=> {indexB} <{deltaPositionIndexA} <=> {deltaPositionIndexB}>";

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref indexA);
            action.Do(ref indexB);
            if (action.Version >= (int)GpucVersion.DD2) {
                action.Do(ref deltaPositionIndexA);
                action.Do(ref deltaPositionIndexB);
            }
            return true;
        }
    }

    public class CollisionShape : BaseModel
    {
        public string? primaryJointName;
        public string? secondaryJointName;
        public uint primaryJointNameHash;
        public uint secondaryJointNameHash;

        protected override bool DoRead(FileHandler handler)
        {
            var version = (GpucVersion)handler.FileVersion;
            if (version < GpucVersion.DD2) {
                primaryJointName = handler.ReadOffsetWString();
                secondaryJointName = handler.ReadOffsetWString();
            }
            handler.Read(ref primaryJointNameHash);
            handler.Read(ref secondaryJointNameHash);
            handler.ReadNull(8);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            var version = (GpucVersion)handler.FileVersion;
            if (version < GpucVersion.DD2) {
                handler.WriteOffsetWString(primaryJointName ?? "");
                handler.WriteOffsetWString(secondaryJointName ?? "");
            }
            handler.Write(ref primaryJointNameHash);
            handler.Write(ref secondaryJointNameHash);
            handler.WriteNull(8);
            return true;
        }

        public void UpdateJointNames()
        {
            primaryJointName = Utils.HashedBoneNames.GetValueOrDefault(primaryJointNameHash);
            secondaryJointName = Utils.HashedBoneNames.GetValueOrDefault(secondaryJointNameHash);
        }

        public override string ToString() => $"{primaryJointName ?? primaryJointNameHash.ToString()} <=> {secondaryJointName ?? secondaryJointNameHash.ToString()}";
    }

    public class CollisionPlane : CollisionShape
    {
        public via.Plane plane;

        protected override bool DoRead(FileHandler handler)
        {
            base.DoRead(handler);
            handler.Read(ref plane);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            base.DoWrite(handler);
            handler.Write(ref plane);
            return true;
        }
    }

    public class CollisionSphere : CollisionShape
    {
        public via.Sphere sphere;

        protected override bool DoRead(FileHandler handler)
        {
            base.DoRead(handler);
            handler.Read(ref sphere);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            base.DoWrite(handler);
            handler.Write(ref sphere);
            return true;
        }
    }

    public class CollisionCapsule : CollisionShape
    {
        public via.TaperedCapsule capsule;

        protected override bool DoRead(FileHandler handler)
        {
            base.DoRead(handler);
            handler.Read(ref capsule);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            base.DoWrite(handler);
            handler.Write(ref capsule);
            return true;
        }
    }

    public class CollisionOBB : CollisionShape
    {
        public via.OBB box;

        protected override bool DoRead(FileHandler handler)
        {
            base.DoRead(handler);
            handler.Read(ref box);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            base.DoWrite(handler);
            handler.Write(ref box);
            return true;
        }
    }

    public class DebugControlPoint : ReadWriteModel
    {
        public float massScale;
        public float shearStiffness;
        public float bendStiffness;
        public Vector3 userColor;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref massScale);
            action.Do(ref shearStiffness);
            action.Do(ref bendStiffness);
            action.Do(ref userColor);
            return true;
        }
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class BatchSubInfo : BaseModel
    {
        public int controlPointStartIndex;
        public int numControlPoints;
        public int numTriangles;
        public int numDistanceLinks;
        [RszPaddingAfter(4)]
        public int numCollisionEdges;
        [RszPaddingAfter(4)]
        public int numDeformInfos;
        public long controlPointOffset;
        public long triangleOffset;
        public long distanceLinkOffset;
        [RszPaddingAfter(8)]
        public long collisionEdgeOffset;
        public long deformInfosOffset;
        public long blendInfosOffset;
        public long blendMatricesOffset;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class BatchGroup : BaseModel
    {
        public RangeI controlPointRange;
        public RangeI triangleRange;
        public RangeI distanceLinkRange;
        public RangeI collisionEdgeRange;
        [RszPaddingAfter(16)]
        public RangeI pointTriangleContactDescsRange;
        [RszPaddingAfter(8)]
        public RangeI deformInfoRange;
        public RangeI batchRange;
    }

    public class Batch(GpucFile file)
    {
        public BatchInfo Info { get; } = new();
        public List<ControlPoint> ControlPoints { get; } = [];
        public List<DistanceLink> DistanceLinks { get; } = [];
        public List<CollisionEdge> CollisionEdges { get; } = [];
        public List<PointTriangleContactDesc> PointTriangleContactDescs { get; } = [];
        public List<EdgeEdgeContactDesc> EdgeEdgeContactDescs { get; } = [];

        public Span<ClothTriangle> Triangles => CollectionsMarshal.AsSpan(file.Triangles).Slice(Info.triangleOffset, Info.numTriangles);
        public Span<DeformInfo> DeformInfos => CollectionsMarshal.AsSpan(file.DeformInfos).Slice(Info.deformInfoOffset, Info.numDeformInfos);
    }

    public class AnimationCurveData : BaseModel
    {
        public ushort keysLength;
        public ushort loopCount;
        public float loopStartTime;
        public float loopEndTime;
        public float minValue;
        public float maxValue;
        public AnimationCurveWrap wrap;
        public AnimationCurveFlags flags;

        public List<KeyFrame> Keys { get; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref keysLength);
            handler.Read(ref loopCount);
            handler.Read(ref loopStartTime);
            handler.Read(ref loopEndTime);
            handler.Read(ref minValue);
            handler.Read(ref maxValue);
            handler.Read(ref wrap);
            handler.Read(ref flags);
            handler.ReadNull(4);
            var keysOffset = handler.Read<long>();
            using var _ = handler.SeekJumpBack(keysOffset);
            Keys.ReadStructList(handler, keysLength);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref keysLength);
            handler.Write(ref loopCount);
            handler.Write(ref loopStartTime);
            handler.Write(ref loopEndTime);
            handler.Write(ref minValue);
            handler.Write(ref maxValue);
            handler.Write(ref wrap);
            handler.Write(ref flags);
            handler.WriteNull(4);
            handler.Skip(8); // keysOffset
            return true;
        }

        public void WriteKeys(FileHandler handler)
        {
            handler.Write(Start + 32, handler.Tell());
            Keys.Write(handler);
        }
    }

    public abstract class DeformInfo
    {
    }

    public class TriangleDeformInfo : DeformInfo
    {
        public int configId;
        public TriangleBlendInfoCompressed[] blendInfos = [];
    }

    public class ControlPointDeformInfo : DeformInfo
    {
        public Float3x4 matrix;
    }

    public struct MeshMultiJointBlendInfo
    {
        public int controlPointIndex;
        public float blendWeight;
        public Float3x4 transposeInverseBindMatrix;

        public readonly override string ToString() => $"{controlPointIndex}: {blendWeight}";
    }

    public class MeshMultiJointDeformInfo : DeformInfo
    {
        public int jointIndex;
        public MeshMultiJointBlendInfo[] BlendInfo = [];

        public override string ToString() => jointIndex.ToString();
    }

    public struct TriangleBlendInfo
    {
        public uint triangleIndex;
        public float weight1;
        public float weight2;
        public float weight3;
        public float weight4;

        public readonly override string ToString() => $"{triangleIndex}: {weight1}";
    }

    public struct TriangleBlendInfoCompressed
    {
        public ushort uknIndex;
        public ushort triangleIndex;
        public byte weight;
        public byte ukn1;
        public byte ukn2;
        public byte ukn3;

        public float Weight {
            readonly get => weight * (1f / 255f);
            set => weight = (byte)Math.Round(value * 255f);
        }

        public readonly override string ToString() => $"{triangleIndex}: {Weight}";
    }

    public class VertexDeformInfo : DeformInfo
    {
        public uint vertexIndex;
        public uint configId;
        public TriangleBlendInfo[] Weights = new TriangleBlendInfo[16];

        public void Read(FileHandler handler)
        {
            handler.Read(ref vertexIndex);
            handler.Read(ref configId);
            handler.ReadArray(Weights);
        }

        public void Write(FileHandler handler)
        {
            handler.Write(ref vertexIndex);
            handler.Write(ref configId);
            handler.WriteArray(Weights);
        }
    }

    public struct EdgeEdgeContactDesc
    {
        public ushort controlPointIndexAA;
        public ushort controlPointIndexAB;
        public ushort controlPointIndexBA;
        public ushort controlPointIndexBB;
    }

    public struct PointTriangleContactDesc
    {
        public ushort pointIndex;
        public ushort triangleIndex;

        public int TriangleIndex {
            readonly get => triangleIndex & 0x7fff;
            set => triangleIndex = (ushort)(value & 0x7fff);
        }

        public bool IsBackFace {
            readonly get => (triangleIndex & 0x8000) != 0;
            set => triangleIndex = (ushort)(value ? (triangleIndex | 0x8000) : (triangleIndex & ~0x8000));
        }

        public readonly override string ToString() => $"{pointIndex}: {TriangleIndex} (Back Face = {IsBackFace})";
    }


    /// <summary>
    /// Convenience made up enum to simplify determining gpuc deformation type.
    /// </summary>
    public enum ClothDeformType
    {
        Unknown = 0,
        PerControlPoint = 1,
        PerTriangle = 2,
        PerVertex = 3,
        MultiJoint = 4,
        MultiJoint_Linear = 5,
    }

    public enum ClothCalculateMode
    {
        Default = 0,
        Performance = 1,
        Balance = 2,
        Quality = 3,
    }

    [Flags]
    public enum PartFlags : uint
    {
        UseBaseJoint = 1,
    }

    public enum AnimationCurveWrap
    {
        Once = 0,
        Loop = 1,
        Loop_Always = 2,
    }

    [Flags]
    public enum ConfigFlags
    {
        SelfCollision = 1,
    }

    [Flags]
    public enum ConfigFlags2
    {
        EnableSecondDamping = 1,
    }

    [Flags]
    public enum AnimationCurveFlags
    {
        IsEnableClamp = 1,
    }
}


namespace ReeLib
{
    using ReeLib.Gpuc;

    public class GpucFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public ClothDeformType DeformType { get; private set; }

        public Header Header { get; } = new();
        public List<Batch> Batches { get; } = [];
        public List<BatchSubInfo> BatchSubInfos { get; } = [];
        public List<BatchGroup> BatchGroups { get; } = [];
        public List<PartInfo> Parts { get; } = [];
        public List<ConfigInfo> Configs { get; } = [];
        public List<ClothTriangle> Triangles { get; } = [];
        public List<DeformInfo> DeformInfos { get; } = [];
        public List<Float3x4> ControlPointMatrices { get; } = [];
        public List<CollisionPlane> CollisionPlanes { get; } = [];
        public List<CollisionSphere> CollisionSpheres { get; } = [];
        public List<CollisionCapsule> CollisionCapsules { get; } = [];
        public List<CollisionOBB> CollisionOBBs { get; } = [];
        public List<PointTriangleContactDesc> PointTriangleContactDescs { get; } = [];
        public List<EdgeEdgeContactDesc> EdgeEdgeContactDescs { get; } = [];
        public List<uint> VertexIdToDeformIndex { get; } = [];
        public List<int> DeformBoneIndices { get; } = [];

        public const int Magic = 0x4F4C4347;

        public void ClearData()
        {
            Batches.Clear();
            Parts.Clear();
            Configs.Clear();
            Triangles.Clear();
            BatchSubInfos.Clear();
            BatchGroups.Clear();
            DeformBoneIndices.Clear();
            CollisionPlanes.Clear();
            CollisionSpheres.Clear();
            CollisionCapsules.Clear();
            CollisionOBBs.Clear();
            DeformInfos.Clear();
            ControlPointMatrices.Clear();
            VertexIdToDeformIndex.Clear();
            PointTriangleContactDescs.Clear();
            EdgeEdgeContactDescs.Clear();
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            if (handler.FileVersion < (int)GpucVersion.RE8) {
                throw new NotSupportedException("GPUC files pre RE8 (< 62) are not supported");
            }
            ClearData();

            var header = Header;
            header.Read(handler);
            if (header.magic != Magic) {
                throw new Exception("Invalid Gpuc file");
            }

            handler.Seek(header.controlPointTbl);
            var controlPoints = new List<ControlPoint>();
            var distanceLinks = new List<DistanceLink>();
            var collisionEdges = new List<CollisionEdge>();

            controlPoints.Read(handler, header.numControlPoints);

            handler.Seek(header.triangleTbl);
            Triangles.Read(handler, header.numTriangles);

            handler.Seek(header.distanceLinkTbl);
            distanceLinks.Read(handler, header.numDistanceLinks);

            handler.Seek(header.collisionEdgeTbl);
            collisionEdges.Read(handler, header.numCollisionEdges);

            handler.Seek(header.numTrianglesInGroupTbl);
            int k = 0;
            for (int i = 0; i < header.numTriangleGroups; ++i) {
                var count = handler.Read<int>();
                for (int j = 0; j < count; ++j) Triangles[k++].groupIndex = i;
            }

            handler.Seek(header.numDistanceLinksInGroupTbl);
            k = 0;
            for (int i = 0; i < header.numDistanceLinkGroups; ++i) {
                var count = handler.Read<int>();
                for (int j = 0; j < count; ++j) distanceLinks[k++].groupIndex = i;
            }

            handler.Seek(header.numCollisionEdgesInGroupTbl);
            k = 0;
            for (int i = 0; i < header.numCollisionEdgeGroups; ++i) {
                var count = handler.Read<int>();
                for (int j = 0; j < count; ++j) collisionEdges[k++].groupIndex = i;
            }

            handler.Seek(header.partInfoTbl);
            Parts.Read(handler, header.numPartInfos);

            handler.Seek(header.configInfoTbl);
            Configs.Read(handler, header.numConfigInfos);

            handler.Seek(header.pointTriangleContactDescTbl);
            PointTriangleContactDescs.ReadStructList(handler, header.numPointTriangleContactDescs);

            handler.Seek(header.edgeEdgeContactDescTbl);
            EdgeEdgeContactDescs.ReadStructList(handler, header.numEdgeEdgeContactDescs);

            handler.Seek(header.batchInfoTbl);
            for (int i = 0; i < header.numBatchInfos; i++) {
                var batch = new Batch(this);
                batch.Info.Read(handler);
                Batches.Add(batch);
            }

            if (header.batchSubInfoTbl > 0 && header.numBatchSubInfos > 0) {
                handler.Seek(header.batchSubInfoTbl);
                BatchSubInfos.Read(handler, header.numBatchSubInfos);
                DataInterpretationException.DebugWarnIf(header.numBatchSubInfos != header.numBatchInfos);
            }

            handler.Seek(header.deformBonesTbl);
            DeformBoneIndices.ReadStructList(handler, header.numDeformBones);

            if (header.batchGroupTbl > 0) {
                handler.Seek(header.batchGroupTbl);
                BatchGroups.Read(handler, header.numBatchGroup);
            }

            handler.Seek(header.collisionPlaneTbl);
            CollisionPlanes.Read(handler, header.numCollisionPlanes);

            handler.Seek(header.collisionSphereTbl);
            CollisionSpheres.Read(handler, header.numCollisionSpheres);

            handler.Seek(header.collisionCapsuleTbl);
            CollisionCapsules.Read(handler, header.numCollisionCapsules);

            handler.Seek(header.collisionOBBTbl);
            CollisionOBBs.Read(handler, header.numCollisionOBBs);

            handler.Seek(header.debugControlPointTbl);
            for (int i = 0; i < header.numControlPoints; i++) {
                controlPoints[i].DebugData.Read(handler);
            }

            handler.Seek(header.debugDistanceLinkTbl);
            for (int i = 0; i < header.numDistanceLinks; i++) {
                distanceLinks[i].DebugID = handler.Read<int>();
            }

            foreach (var cp in Parts) {
                cp.Config = Configs[cp.configId];
            }

            foreach (var cp in controlPoints) {
                cp.Config = Configs[cp.configId];
            }

            // attempt to guess which struct to use
            DeformType = ClothDeformType.Unknown;
            var version = (GpucVersion)handler.FileVersion;
            if (version >= GpucVersion.DD2) {
                if (header.vertexIdToDeformInfoIndexTbl < Header.debugControlPointTbl) {
                    DeformType = ClothDeformType.PerTriangle;
                } else {
                    DeformType = ClothDeformType.MultiJoint;
                }
            } else if (version == GpucVersion.RE4) {
                if (header.numVertexDeformInfos > 0) {
                    DeformType = ClothDeformType.PerVertex;
                } else {
                    DeformType = ClothDeformType.MultiJoint_Linear;
                }
            } else if (header.numControlPoints == header.numDeformInfos) {
                DeformType = ClothDeformType.PerControlPoint;
            } else {
                DeformType = ClothDeformType.PerVertex;
            }
            handler.Seek(header.deformInfoTbl);
            switch (DeformType) {
                case ClothDeformType.PerVertex:
                    // re8, re4
                    for (int i = 0; i < header.numDeformInfos; i++) {
                        var v = new VertexDeformInfo();
                        v.Read(handler);
                        DeformInfos.Add(v);
                    }
                    break;
                case ClothDeformType.PerControlPoint:
                    // re2rt
                    for (int i = 0; i < header.numDeformInfos; i++) {
                        var v = new ControlPointDeformInfo();
                        handler.Read(ref v.matrix);
                        DeformInfos.Add(v);
                    }
                    break;
                case ClothDeformType.MultiJoint:
                    // re9
                    for (int i = 0; i < header.numDeformInfos; i++) {
                        var v = new MeshMultiJointDeformInfo();
                        v.BlendInfo = new MeshMultiJointBlendInfo[header.numDeformWeights];
                        handler.Read(ref v.jointIndex);
                        DeformInfos.Add(v);
                    }
                    handler.Seek(header.blendInfoTbl);
                    for (int i = 0; i < header.numDeformInfos; i++) {
                        for (k = 0; k < header.numDeformWeights; k++) {
                            handler.Read(ref ((MeshMultiJointDeformInfo)DeformInfos[i]).BlendInfo[k].controlPointIndex);
                            handler.Read(ref ((MeshMultiJointDeformInfo)DeformInfos[i]).BlendInfo[k].blendWeight);
                        }
                    }
                    handler.Seek(header.blendMultiMatricesTbl);
                    for (int i = 0; i < header.numDeformInfos; i++) {
                        for (k = 0; k < header.numDeformWeights; k++) {
                            handler.Read(ref ((MeshMultiJointDeformInfo)DeformInfos[i]).BlendInfo[k].transposeInverseBindMatrix);
                        }
                    }
                    handler.Seek(header.blendControlPointMatricesTbl);
                    ControlPointMatrices.ReadStructList(handler, header.numControlPoints);
                    break;
                case ClothDeformType.MultiJoint_Linear:
                    // re4
                    for (int i = 0; i < header.numDeformInfos; i++) {
                        var v = new MeshMultiJointDeformInfo();
                        handler.Read(ref v.jointIndex);
                        v.BlendInfo = new MeshMultiJointBlendInfo[8];
                        handler.ReadArray(v.BlendInfo);
                        DeformInfos.Add(v);
                    }
                    break;
                case ClothDeformType.PerTriangle:
                    // dd2, pragmata
                    for (int i = 0; i < header.numDeformInfos; i++) {
                        var v = new TriangleDeformInfo();
                        handler.Read(ref v.configId);
                        v.blendInfos = new TriangleBlendInfoCompressed[header.numDeformWeights];
                        DeformInfos.Add(v);
                    }
                    handler.Seek(header.blendInfoTbl);
                    for (int i = 0; i < header.numDeformInfos; i++) {
                        handler.ReadArray(((TriangleDeformInfo)DeformInfos[i]).blendInfos);
                    }
                    handler.Seek(header.vertexIdToDeformInfoIndexTbl);
                    VertexIdToDeformIndex.ReadStructList(handler, header.numTotalRenderVertices);
                    break;
                default:
                    Log.Error("Unknown or unsupported gpuc deform type");
                    break;
            }

            foreach (var batch in Batches) {
                batch.ControlPoints.AddRange(controlPoints.Slice(batch.Info.controlPointOffset, batch.Info.numControlPoints));
                batch.DistanceLinks.AddRange(distanceLinks.Slice(batch.Info.distanceLinkOffset, batch.Info.numDistanceLinks));
                batch.CollisionEdges.AddRange(collisionEdges.Slice(batch.Info.collisionEdgeOffset, batch.Info.numCollisionEdges));
                batch.PointTriangleContactDescs.AddRange(PointTriangleContactDescs.Slice(batch.Info.pointTriangleContactDescOffset, batch.Info.numPointTriangleContactDescs));
                batch.EdgeEdgeContactDescs.AddRange(EdgeEdgeContactDescs.Slice(batch.Info.edgeEdgeContactDescOffset, batch.Info.numEdgeEdgeContactDescs));

                // validate triangle / link / edge groups
                int groupedCount = Triangles.Count(tri => tri.groupIndex >= batch.Info.triangleGroupOffset
                    && tri.groupIndex < batch.Info.triangleGroupOffset + batch.Info.numTriangleGroups);
                DataInterpretationException.DebugWarnIf(batch.Info.numTriangleGroups > 0 && groupedCount != batch.Info.numTriangles, "GPUC triangle groups count mismatch");

                groupedCount = distanceLinks.Count(link => link.groupIndex >= batch.Info.distanceLinkGroupOffset
                    && link.groupIndex < batch.Info.distanceLinkGroupOffset + batch.Info.numDistanceLinkGroups);
                DataInterpretationException.DebugWarnIf(groupedCount != batch.Info.numDistanceLinks, "GPUC distance link groups count mismatch");

                groupedCount = collisionEdges.Count(edge => edge.groupIndex >= batch.Info.collisionEdgeGroupOffset
                    && edge.groupIndex < batch.Info.collisionEdgeGroupOffset + batch.Info.numCollisionEdgeGroups);
                DataInterpretationException.DebugWarnIf(groupedCount != batch.Info.numCollisionEdges, "GPUC collision edge groups count mismatch");
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var header = Header;
            var version = (GpucVersion)handler.FileVersion;

            if (Batches.Count == 0 && Configs.Count == 0) {
                handler.Write(Magic);
                // straight up write as disabled blank GPUC
                handler.Stream.SetLength(4);
                return true;
            }

            header.Write(handler);

            var controlPoints = Batches.SelectMany(b => b.ControlPoints).ToList();
            var distanceLinks = Batches.SelectMany(b => b.DistanceLinks).ToList();
            var collisionEdges = Batches.SelectMany(b => b.CollisionEdges).ToList();

            static void WriteList<T>(List<T> list, FileHandler handler, ref int count, ref long offset, int align = 16) where T : BaseModel
            {
                offset = handler.AlignTell(align);
                count = list.Count;
                list.Write(handler);
            }

            if (version >= GpucVersion.DD2) {
                WriteList(Parts, handler, ref header.numPartInfos, ref header.partInfoTbl);
            }

            header.batchInfoTbl = handler.Tell();
            header.numBatchInfos = Batches.Count;
            foreach (var batch in Batches) {
                batch.Info.Write(handler);
            }

            if (version < GpucVersion.DD2) {
                WriteList(Parts, handler, ref header.numPartInfos, ref header.partInfoTbl, 1);
            }

            WriteList(Configs, handler, ref header.numConfigInfos, ref header.configInfoTbl, 1);

            if (version >= GpucVersion.MHST3) {
                WriteList(BatchSubInfos, handler, ref header.numBatchSubInfos, ref header.batchSubInfoTbl);

                header.deformBonesTbl = handler.AlignTell();
                header.numDeformBones = DeformBoneIndices.Count;
                DeformBoneIndices.Write(handler);
            }

            if (version >= GpucVersion.DD2) {
                header.keyFrameTbl = handler.AlignTell();
                foreach (var config in Configs) {
                    config.blendAmountTranslationCurve.WriteKeys(handler);
                    config.blendAmountRotationCurve.WriteKeys(handler);
                }
                WriteList(BatchGroups, handler, ref header.numBatchGroup, ref header.batchGroupTbl);
            }

            WriteList(controlPoints, handler, ref header.numControlPoints, ref header.controlPointTbl, 1);
            if (version < GpucVersion.DD2) {
                handler.Align(16);
                WriteDeformData();
            }
            WriteList(distanceLinks, handler, ref header.numDistanceLinks, ref header.distanceLinkTbl);
            WriteList(Triangles, handler, ref header.numTriangles, ref header.triangleTbl);
            WriteList(collisionEdges, handler, ref header.numCollisionEdges, ref header.collisionEdgeTbl, 8);

            if (version < GpucVersion.MHST3) {
                header.numTrianglesInGroupTbl = handler.Tell();
                foreach (var item in Triangles.GroupBy(g => g.groupIndex).OrderBy(k => k.Key)) {
                    handler.Write(item.Count());
                }
            }

            header.numDistanceLinksInGroupTbl = handler.Tell();
            foreach (var item in distanceLinks.GroupBy(g => g.groupIndex).OrderBy(k => k.Key)) {
                handler.Write(item.Count());
            }

            header.numCollisionEdgesInGroupTbl = handler.Tell();
            foreach (var item in collisionEdges.GroupBy(g => g.groupIndex).OrderBy(k => k.Key)) {
                handler.Write(item.Count());
            }

            header.pointTriangleContactDescTbl = handler.Tell();
            header.offsUnused2 = handler.Tell();
            header.numPointTriangleContactDescs = PointTriangleContactDescs.Count;
            PointTriangleContactDescs.Write(handler);

            if (EdgeEdgeContactDescs.Count > 0) {
                header.edgeEdgeContactDescTbl = handler.AlignTell(8);
                header.numEdgeEdgeContactDescs = EdgeEdgeContactDescs.Count;
                EdgeEdgeContactDescs.Write(handler);
            }

            header.collisionPlaneTbl = CollisionPlanes.Count == 0 ? 0 : handler.AlignTell();
            header.numCollisionPlanes = (ushort)CollisionPlanes.Count;
            CollisionPlanes.Write(handler);

            header.collisionSphereTbl = CollisionSpheres.Count == 0 ? 0 : handler.AlignTell();
            header.numCollisionSpheres = (ushort)CollisionSpheres.Count;
            CollisionSpheres.Write(handler);

            header.collisionCapsuleTbl = CollisionCapsules.Count == 0 ? 0 : handler.AlignTell();
            header.numCollisionCapsules = (ushort)CollisionCapsules.Count;
            CollisionCapsules.Write(handler);

            header.collisionOBBTbl = CollisionOBBs.Count == 0 ? 0 : handler.AlignTell();
            header.numCollisionOBBs = (ushort)CollisionOBBs.Count;
            CollisionOBBs.Write(handler);

            if (version >= GpucVersion.DD2) {
                handler.Align(16);
                header.offsUnused1 = header.offsUnused3 = handler.Tell();
                handler.StringTableFlush();
                handler.Align(16);

                WriteDeformData();
            }

            header.debugControlPointTbl = handler.AlignTell();
            foreach (var cp in controlPoints) {
                cp.DebugData.Write(handler);
            }

            header.debugDistanceLinkTbl = handler.AlignTell();
            for (int i = 0; i < header.numDistanceLinks; i++) {
                handler.Write(distanceLinks[i].DebugID);
            }

            handler.Align(16);
            handler.StringTableFlush();

            header.Rewrite(handler);
            return true;
        }

        private void WriteDeformData()
        {
            var handler = FileHandler;
            var header = Header;
            header.deformInfoTbl = handler.Tell();
            header.numDeformInfos = DeformInfos.Count;
            header.numTotalRenderVertices = VertexIdToDeformIndex.Count;
            header.numVertexDeformInfos = 0;

            header.blendInfoTbl = header.deformInfoTbl;
            header.blendMultiMatricesTbl = header.deformInfoTbl;
            header.blendControlPointMatricesTbl = header.deformInfoTbl;
            header.vertexIdToDeformInfoIndexTbl = header.deformInfoTbl;

            switch (DeformType) {
                case ClothDeformType.PerVertex:
                    // re8, re4
                    if ((GpucVersion)handler.FileVersion == GpucVersion.RE4) {
                        header.numVertexDeformInfos = header.numDeformInfos;
                    }
                    foreach (var item in DeformInfos.Cast<VertexDeformInfo>()) {
                        item.Write(handler);
                    }
                    break;
                case ClothDeformType.PerControlPoint:
                    // re2rt
                    foreach (var item in DeformInfos.Cast<ControlPointDeformInfo>()) {
                        handler.Write(ref item.matrix);
                    }
                    break;
                case ClothDeformType.MultiJoint:
                    // re9
                    if (ControlPointMatrices.Count != header.numControlPoints) {
                        throw new InvalidDataException("Count mismatch between control points and blend matrices!");
                    }

                    foreach (var item in DeformInfos.Cast<MeshMultiJointDeformInfo>()) {
                        handler.Write(ref item.jointIndex);
                    }
                    header.blendInfoTbl = handler.AlignTell();
                    foreach (var item in DeformInfos.Cast<MeshMultiJointDeformInfo>()) {
                        foreach (var info in item.BlendInfo) {
                            handler.Write(info.controlPointIndex);
                            handler.Write(info.blendWeight);
                        }
                    }
                    header.blendMultiMatricesTbl = handler.AlignTell();
                    foreach (var item in DeformInfos.Cast<MeshMultiJointDeformInfo>()) {
                        foreach (var info in item.BlendInfo) {
                            handler.Write(info.transposeInverseBindMatrix);
                        }
                    }
                    header.blendControlPointMatricesTbl = handler.AlignTell();
                    header.cpuMemorySize = (int)handler.Tell();
                    ControlPointMatrices.Write(handler);
                    header.vertexIdToDeformInfoIndexTbl = handler.AlignTell();
                    break;
                case ClothDeformType.MultiJoint_Linear:
                    // re4
                    foreach (var item in DeformInfos.Cast<MeshMultiJointDeformInfo>()) {
                        handler.Write(ref item.jointIndex);
                        handler.WriteArray(item.BlendInfo);
                    }
                    break;
                case ClothDeformType.PerTriangle:
                    // dd2, pragmata
                    header.cpuMemorySize = (int)handler.AlignTell();
                    foreach (var item in DeformInfos.Cast<TriangleDeformInfo>()) {
                        handler.Write(ref item.configId);
                    }
                    header.blendInfoTbl = handler.AlignTell();
                    foreach (var item in DeformInfos.Cast<TriangleDeformInfo>()) {
                        handler.WriteArray(item.blendInfos);
                    }
                    header.blendMultiMatricesTbl = header.blendControlPointMatricesTbl = header.vertexIdToDeformInfoIndexTbl = handler.AlignTell();
                    VertexIdToDeformIndex.Write(handler);
                    break;
                default:
                    Log.Error("Unknown or unsupported gpuc deform type");
                    break;
            }
        }
    }
}