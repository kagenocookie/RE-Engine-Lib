namespace RszTool.Efx;

using System.Reflection;
using RszTool.Efx.Structs;

public enum EfxAttributeType
{
    Unknown = 0,
    FixRandomGenerator = 1,
    EffectOptimizeShader,
    Transform2D,
    Transform2DExpression,
    Transform3D,
    Transform3DModifierDelayFrame,
    Transform3DModifier,
    Transform3DClip,
    Transform3DExpression,
    ParentOptions,
    Spawn,
    SpawnExpression,
    EmitterColor,
    EmitterColorClip,
    PtSort,
    TypeBillboard2D,
    TypeBillboard2DExpression,
    TypeBillboard3D,
    TypeBillboard3DExpression,
    TypeMesh,
    TypeMeshClip,
    TypeMeshExpression,
    TypeRibbonFollow,
    TypeRibbonLength,
    TypeRibbonChain,
    TypeRibbonFixEnd,
    unknRE4Struct58Expression, // maybe TypeRibbonLengthExpression
    unknRE4Struct60,
    TypePolygon,
    TypePolygonClip,
    TypePolygonExpression,
    TypePolygonMaterial,
    TypePolygonMaterialExpression,
    TypePolygonTrail,
    TypePolygonTrailExpression,
    TypePolygonTrailMaterial,
    TypePolygonTrailMaterialExpression,
    TypeRibbonTrail,
    unknRE4Struct70,
    TypeNoDraw,
    unknDD2Struct77,
    Velocity2D,
    Velocity2DDelayFrame,
    Velocity2DExpression,
    Velocity3D,
    Velocity3DDelayFrame,
    Velocity3DExpression,
    RotateAnimDelayFrame,
    RotateAnim,
    RotateAnimExpression,
    ScaleAnimDelayFrame,
    ScaleAnim,
    VanishArea3D,
    Life,
    LifeExpression,
    UVSequence,
    UVSequenceModifier,
    UVSequenceExpression,
    unknDD2Struct95,
    UVScroll,
    TextureUnit,
    TextureFilter,
    EmitterShape2D,
    EmitterShape2DExpression,
    EmitterShape3D,
    EmitterShape3DExpression,
    AlphaCorrection,
    Blink,
    unknRE4Struct109,
    unknRE4Struct110,
    TexelChannelOperator,
    TexelChannelOperatorClip,
    TexelChannelOperatorExpression,
    unknRE4Struct114,
    ShaderSettings,
    Distortion,
    unknDD2Struct128,
    unknRE4Struct130,
    PtLife,
    PtBehavior,
    unknDD2Struct133,
    unknRE4Struct134,
    FadeByAngle,
    FadeByDepth,
    FadeByDepthExpression,
    FadeByOcclusion,
    FadeByOcclusionExpression,
    FakeDoF,
    unknRE4Struct144,
    unknRE4Struct150,
    LuminanceBleed,
    PlayEmitter,
    unknRE4Struct155,
    PtTransform3D,
    PtTransform3DClip,
    PtVelocity3D,
    PtVelocity3DClip,
    PtColliderAction,
    PtCollision,
    PtColor,
    PtColorClip,
    PtUvSequence,
    PtUvSequenceClip,
    unknRE4Struct170Mesh,
    unknRE4Struct173,
    VectorFieldParameter,
    VectorFieldParameterClip,
    VectorFieldParameterExpression,
    unknRE4Struct177,
    unknRE4Struct184,
    unknRE4Struct185,
    unknRE4Struct187,
    unknRE4Struct191,
    Attractor,
    AttractorClip,
    unknRE4Struct195,
    unknRE4Struct197,
    EmitterPriority,
    DrawOverlay,
    AngularVelocity2DDelayFrame,
    AngularVelocity2D,
    AngularVelocity3DDelayFrame,
    AngularVelocity3D,
    unknRE4Struct210,
    unknRE4Struct213,
    unknSBStruct195,
    ProceduralDistortionDelayFrame,
    ProceduralDistortion,
    ProceduralDistortionClip,
    ProceduralDistortionExpression,
    StretchBlur,
    unknRE4Struct226,
    unknRE4Struct228,
    unknRE4Struct231,
    unknRE4Struct243_UnknGPUBillboard,
    unknRE4Struct244,
    unknRE4Struct245_UnknType,
    unknRE4Struct247_UnknTypeGPU,
    unknRE4Struct248,
    unknRE4Struct249_UnknTypeB,
    TypeGpuMesh,
    AttractorExpression,
    ColorGrading,
    ContrastHighlighter,
    CustomComputeShader,
    DepthOcclusion,
    DepthOperator,
    DirectionalField,
    DirectionalFieldParameter,
    DirectionalFieldParameterClip,
    DirectionalFieldParameterExpression,
    DistortionExpression,
    EmitMask,
    EmitterHSV,
    EmitterHSVExpression,
    FadeByAngleExpression,
    FadeByEmitterAngle,
    FlowMap,
    FluidEmitter2D,
    FluidEmitter2DClip,
    FluidEmitter2DExpression,
    FluidSimulator2D,
    GlobalVectorField,
    GlobalVectorFieldClip,
    GlobalVectorFieldExpression,
    IgnorePlayerColor,
    ItemNum,
    MeshEmitter,
    MeshEmitterClip,
    MeshEmitterExpression,
    Noise,
    NoiseExpression,
    PlaneCollider,
    PlaneColliderExpression,
    PlayEfx,
    PtAngularVelocity2D,
    PtAngularVelocity2DExpression,
    PtAngularVelocity3D,
    PtAngularVelocity3DExpression,
    PtBehaviorClip,
    PtTransform2D,
    PtTransform2DClip,
    PtVelocity2D,
    PtVelocity2DClip,
    RenderTarget,
    RgbCommon,
    RgbWater,
    PtFreezer,
    AssignCSV,
    ScaleAnimExpression,
    ScaleByDepth,
    ScreenSpaceEmitter,
    ShaderSettingsExpression,
    ShapeOperator,
    ShapeOperatorExpression,
    StretchBlurExpression,
    TestBehaviorUpdater,
    TextureUnitExpression,
    Transform2DClip,
    Transform2DModifier,
    Transform2DModifierDelayFrame,
    TypeBillboard3DMaterial,
    TypeBillboard3DMaterialClip,
    TypeBillboard3DMaterialExpression,
    TypeGpuBillboard,
    TypeGpuBillboardExpression,
    TypeGpuLightning3D,
    TypeGpuMeshExpression,
    TypeGpuMeshTrail,
    TypeGpuMeshTrailClip,
    TypeGpuMeshTrailExpression,
    TypeGpuPolygon,
    TypeGpuPolygonExpression,
    TypeGpuRibbonFollow,
    TypeGpuRibbonFollowExpression,
    TypeGpuRibbonLength,
    TypeGpuRibbonLengthExpression,
    TypeLightning3D,
    TypeLightning3DExpression,
    TypeLightning3DMaterial,
    TypeModularBillboard,
    TypeModularRibbonFollow,
    TypeModularRibbonLength,
    TypeModularPolygon,
    TypeModularMesh,
    TypeNoDrawExpression,
    TypeNodeBillboard,
    TypeNodeBillboardExpression,
    TypeRibbonChainExpression,
    TypeRibbonChainMaterial,
    TypeRibbonChainMaterialClip,
    TypeRibbonChainMaterialExpression,
    TypeRibbonFixEndExpression,
    TypeRibbonLightweightExpression,
    TypeRibbonParticleExpression,
    TypeRibbonFixEndMaterial,
    TypeRibbonFixEndMaterialClip,
    TypeRibbonFixEndMaterialExpression,
    TypeRibbonFollowExpression,
    TypeRibbonFollowMaterial,
    TypeRibbonFollowMaterialClip,
    TypeRibbonFollowMaterialExpression,
    TypeRibbonLengthExpression,
    TypeRibbonLengthMaterial,
    TypeRibbonLengthMaterialClip,
    TypeRibbonLengthMaterialExpression,
    TypeRibbonLightweight,
    TypeRibbonLightweightMaterial,
    TypeRibbonLightweightMaterialClip,
    TypeRibbonLightweightMaterialExpression,
    TypeRibbonParticle,
    TypeStrainRibbon,
    TypeStrainRibbonExpression,
    TypeStrainRibbonMaterial,
    TypeStrainRibbonMaterialClip,
    TypeStrainRibbonMaterialExpression,
    UnitCulling,
    VanishArea3DExpression,
    VectorField,
    VolumeField,
    VolumetricLighting,
    WindInfluence3D,
    WindInfluence3DDelayFrame,
    unknSBStruct104_NoiseExpression,
    unknSBStruct156_MeshEmitter,
    unknSBStruct189_PtAngularVelocity3D,
    unknRERTStruct168,
    unknRERTStruct215,
    unknRERTStruct219, // similar to TypeGpuPolygon
    unknRERTStruct220Expression, // expression for unknRERTStruct219
    unknRERTStruct181,
    unknRERTStruct136,

    unknRERTStruct151_PtColor,
    unknRERTStruct152_PtColorClip,
}

public static partial class EfxAttributeTypeRemapper
{
    private static readonly Dictionary<EfxVersion, Dictionary<int, EfxAttributeType>> types = new();
    private static readonly Dictionary<EfxVersion, Dictionary<EfxAttributeType, int>> typesOut = new();
    private static readonly Dictionary<EfxVersion, Dictionary<EfxAttributeType, Type>> typeFactories = new();
    private static Dictionary<EfxAttributeType, List<(EfxVersion version, Type type)>>? typelist;

    public static EfxAttributeType GetAttributeType(this EfxVersion version, int typeId)
    {
        if (GetAllTypes(version).TryGetValue(typeId, out var val)) return val;

        throw new ArgumentException($"Unknown {version} EFX attribute type {typeId}", nameof(typeId));
    }

    public static Dictionary<int, EfxAttributeType> GetAllTypes(EfxVersion version)
    {
        if (!types.TryGetValue(version, out var map))
        {
            return map = GenerateEfxLookup(version);
        }
        return map;
    }

    public static int ToAttributeTypeID(this EfxVersion version, EfxAttributeType type)
    {
        return AttributeTypeIDs.TryGetValue(type, out var lookup) && lookup.TryGetValue(version, out var id)
            ? id
            : throw new ArgumentException($"Unsupported {version} EFX attribute type {type}", nameof(type));
    }

    public static EFXAttribute? Create(EfxAttributeType type, EfxVersion version)
    {
        var instanceType = GetAttributeInstanceType(type, version);
        return instanceType != null ? (EFXAttribute?)Activator.CreateInstance(instanceType) : null;
    }

    public static Type? GetAttributeInstanceType(EfxAttributeType type, EfxVersion version)
    {
        if (!typeFactories.TryGetValue(version, out var dict))
        {
            GenerateEfxLookup(version);
            dict = typeFactories![version];
        }

        return dict.TryGetValue(type, out var t) ? t : null;
    }

    private static readonly EfxVersion[] GameOrder = EfxFile.AllVersions;

    private static Dictionary<int, EfxAttributeType> GenerateEfxLookup(EfxVersion version)
    {
        var readDict = new Dictionary<int, EfxAttributeType>();
        var typeDict = new Dictionary<EfxAttributeType, Type>();
        if (typelist == null)
        {
            typelist = new ();
            var structs = typeof(EfxAttributeTypeRemapper).Assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<EfxStructAttribute>() != null);
            foreach (var type in structs)
            {
                var attr = type.GetCustomAttribute<EfxStructAttribute>()!;
                var efxType = attr.AttributeType;
                if (!typelist.TryGetValue(efxType, out var list)) {
                    typelist[efxType] = list = new List<(EfxVersion, Type)>();
                }

                foreach (var ver in attr.GameVersions)
                {
                    list.Add((ver, type));
                }
            }
        }

        foreach (var (type, lookup) in AttributeTypeIDs) {
            if (lookup.TryGetValue(version, out var id)) {
                if (id < 0) continue;

                readDict.Add(id, type);
                var typesList = typelist.GetValueOrDefault(type);
                if (typesList == null) {
                    // throw new Exception("Unmapped EFX structure " + type);
                    continue;
                }

                var instanceType = typesList?.FirstOrDefault(t => t.version == version).type;
                if (instanceType == null)
                {
                    var gameIndex = Array.IndexOf(GameOrder, version);
                    if (gameIndex == -1)
                    {
                        throw new Exception("Unhandled EFX version " + version);
                    }

                    // first try the latest earlier game
                    var closestMatch = GameOrder.Where((a, i) => i < gameIndex).Select(g => typesList!.FirstOrDefault(t => t.version == g).type).LastOrDefault(t => t != null);
                    if (closestMatch == null) {
                        // failing that, find the earliest match
                        closestMatch = GameOrder.Select(g => typesList!.FirstOrDefault(t => t.version == g).type).FirstOrDefault(t => t != null);
                    }
                    instanceType = closestMatch;
                    if (instanceType == null)
                    {
                        continue;
                        // throw new Exception("Unmapped EFX structure " + type + " for version " + version);
                    }
                    // Console.WriteLine($"Matched {version} EFX {type} to closest struct {instanceType.FullName}");
                }
                typeDict[type] = instanceType;
            }
        }

        types[version] = readDict;
        typeFactories[version] = typeDict;
        return readDict;
    }

    private class EfxLookup : Dictionary<EfxVersion, int>
    {
    }

    private const int UNSUPPORTED = -2;
    private const int UNKNOWN = -1;

    private static readonly Dictionary<EfxAttributeType, EfxLookup> AttributeTypeIDs = new()
    {
        [EfxAttributeType.FixRandomGenerator] = new() {
            [EfxVersion.RE3] = 1,
            [EfxVersion.RE2] = 12,
            [EfxVersion.DMC5] = 12,
            [EfxVersion.RE7] = 8,
            [EfxVersion.RE8] = 1,
            [EfxVersion.SF6] = 1,
            [EfxVersion.RERT] = 1,
            [EfxVersion.RE4] = 1,
        },
        [EfxAttributeType.EffectOptimizeShader] = new() {
            [EfxVersion.RE8] = 2,
            [EfxVersion.RERT] = 2,
            [EfxVersion.RE4] = 2,
        },
        [EfxAttributeType.Transform2D] = new() {
            [EfxVersion.RE3] = 4,
            [EfxVersion.RE2] = 3,
            [EfxVersion.DMC5] = 3,
            [EfxVersion.RE7] = 1,
            [EfxVersion.RE8] = 5,
            [EfxVersion.RERT] = 5,
            [EfxVersion.RE4] = 4,
        },
        [EfxAttributeType.Transform2DModifierDelayFrame] = new() {
            [EfxVersion.RE3] = 5,
            [EfxVersion.RE8] = 6,
            [EfxVersion.RERT] = 6,
            [EfxVersion.RE4] = 5,
        },
        [EfxAttributeType.Transform2DModifier] = new() {
            [EfxVersion.RE3] = 6,
            [EfxVersion.RE2] = 4,
            [EfxVersion.DMC5] = 4,
            [EfxVersion.RE8] = 7,
            [EfxVersion.RERT] = 7,
            [EfxVersion.RE4] = 6, // guess
        },
        [EfxAttributeType.Transform2DClip] = new() {
            [EfxVersion.RE3] = 7,
            [EfxVersion.RE2] = 5,
            [EfxVersion.DMC5] = 5,
            [EfxVersion.RE7] = 2,
            [EfxVersion.RE8] = 8,
            [EfxVersion.RERT] = 8,
            [EfxVersion.RE4] = 7, // guess
        },
        [EfxAttributeType.Transform2DExpression] = new() {
            [EfxVersion.RE3] = 8,
            [EfxVersion.RE2] = 6,
            [EfxVersion.DMC5] = 6,
            [EfxVersion.RE7] = 3,
            [EfxVersion.RE8] = 9,
            [EfxVersion.RERT] = 9,
            [EfxVersion.RE4] = 8,
        },
        [EfxAttributeType.Transform3D] = new() {
            [EfxVersion.RE3] = 9,
            [EfxVersion.RE2] = 7,
            [EfxVersion.DMC5] = 7,
            [EfxVersion.RE7] = 4,
            [EfxVersion.RE8] = 10,
            [EfxVersion.RERT] = 10,
            [EfxVersion.RE4] = 9,
        },
        [EfxAttributeType.Transform3DModifierDelayFrame] = new() {
            [EfxVersion.RE3] = 10,
            [EfxVersion.RE8] = 11,
            [EfxVersion.RERT] = 11,
            [EfxVersion.RE4] = 10,
        },
        [EfxAttributeType.Transform3DModifier] = new() {
            [EfxVersion.RE3] = 11,
            [EfxVersion.RE2] = 8,
            [EfxVersion.DMC5] = 8,
            [EfxVersion.RE8] = 12,
            [EfxVersion.RERT] = 12,
            [EfxVersion.RE4] = 11,
        },
        [EfxAttributeType.Transform3DClip] = new() {
            [EfxVersion.RE3] = 12,
            [EfxVersion.RE2] = 9,
            [EfxVersion.DMC5] = 9,
            [EfxVersion.RE7] = 5,
            [EfxVersion.RE8] = 13,
            [EfxVersion.RERT] = 13,
            [EfxVersion.RE4] = 12,
        },
        [EfxAttributeType.Transform3DExpression] = new() {
            [EfxVersion.RE3] = 13,
            [EfxVersion.RE2] = 10,
            [EfxVersion.DMC5] = 10,
            [EfxVersion.RE7] = 6,
            [EfxVersion.RE8] = 14,
            [EfxVersion.RERT] = 14,
            [EfxVersion.RE4] = 13,
        },
        [EfxAttributeType.ParentOptions] = new() {
            [EfxVersion.RE3] = 14,
            [EfxVersion.RE2] = 11,
            [EfxVersion.DMC5] = 11,
            [EfxVersion.RE7] = 7,
            [EfxVersion.RE8] = 15,
            [EfxVersion.RERT] = 15,
            [EfxVersion.RE4] = 14,
        },
        [EfxAttributeType.Spawn] = new() {
            [EfxVersion.RE3] = 2,
            [EfxVersion.RE2] = 1,
            [EfxVersion.DMC5] = 1,
            [EfxVersion.RE7] = 9,
            [EfxVersion.RE8] = 3,
            [EfxVersion.RERT] = 3,
            [EfxVersion.RE4] = 16,
        },
        [EfxAttributeType.SpawnExpression] = new() {
            [EfxVersion.RE3] = 3,
            [EfxVersion.RE2] = 2,
            [EfxVersion.DMC5] = 2,
            [EfxVersion.RE7] = 10,
            [EfxVersion.RE8] = 4,
            [EfxVersion.RERT] = 4,
            [EfxVersion.RE4] = 17,
        },
        [EfxAttributeType.EmitterColor] = new() {
            [EfxVersion.RE3] = 15,
            [EfxVersion.RE8] = 16,
            [EfxVersion.RERT] = 16,
            [EfxVersion.RE4] = 18,
        },
        [EfxAttributeType.EmitterColorClip] = new() {
            [EfxVersion.RE3] = 16,
            [EfxVersion.RE8] = 17,
            [EfxVersion.RERT] = 17,
            [EfxVersion.RE4] = 19,
        },
        [EfxAttributeType.PtSort] = new() {
            [EfxVersion.RE8] = 18,
            [EfxVersion.RERT] = 18,
            [EfxVersion.RE4] = 20,
        },
        [EfxAttributeType.TypeBillboard2D] = new() {
            [EfxVersion.RE3] = 17,
            [EfxVersion.RE2] = 13,
            [EfxVersion.DMC5] = 13,
            [EfxVersion.RE7] = 11,
            [EfxVersion.RE8] = 19,
            [EfxVersion.RERT] = 19,
            [EfxVersion.RE4] = 21,
        },
        [EfxAttributeType.TypeBillboard2DExpression] = new() {
            [EfxVersion.RE3] = 18,
            [EfxVersion.RE2] = 14,
            [EfxVersion.DMC5] = 14,
            [EfxVersion.RE7] = 12,
            [EfxVersion.RE8] = 20,
            [EfxVersion.RERT] = 20,
            [EfxVersion.RE4] = 22,
        },
        [EfxAttributeType.TypeBillboard3D] = new() {
            [EfxVersion.RE3] = 19,
            [EfxVersion.RE2] = 15,
            [EfxVersion.DMC5] = 15,
            [EfxVersion.RE7] = 13,
            [EfxVersion.RE8] = 21,
            [EfxVersion.RERT] = 21,
            [EfxVersion.RE4] = 23,
        },
        [EfxAttributeType.TypeBillboard3DExpression] = new() {
            [EfxVersion.RE3] = 20,
            [EfxVersion.RE2] = 16,
            [EfxVersion.DMC5] = 16,
            [EfxVersion.RE7] = 14,
            [EfxVersion.RE8] = 22,
            [EfxVersion.RERT] = 22,
            [EfxVersion.RE4] = 24,
        },
        [EfxAttributeType.TypeBillboard3DMaterial] = new() {
            [EfxVersion.RE3] = 21,
            [EfxVersion.RE8] = 23,
            [EfxVersion.RERT] = 23,
            [EfxVersion.RE4] = 25,
        },
        [EfxAttributeType.TypeBillboard3DMaterialClip] = new() {
            [EfxVersion.RE3] = 22,
            [EfxVersion.RE8] = 24,
            [EfxVersion.RERT] = 24,
            [EfxVersion.RE4] = 26,
        },
        [EfxAttributeType.TypeBillboard3DMaterialExpression] = new() {
            [EfxVersion.RE3] = 23,
            [EfxVersion.RE8] = 25,
            [EfxVersion.RERT] = 25,
            [EfxVersion.RE4] = 27,
        },
        [EfxAttributeType.TypeMesh] = new() {
            [EfxVersion.RE3] = 24,
            [EfxVersion.RE2] = 17,
            [EfxVersion.DMC5] = 17,
            [EfxVersion.RE7] = 15,
            [EfxVersion.RE8] = 26,
            [EfxVersion.RERT] = 26,
            [EfxVersion.RE4] = 28,
        },
        [EfxAttributeType.TypeMeshClip] = new() {
            [EfxVersion.RE3] = 25,
            [EfxVersion.RE2] = 18,
            [EfxVersion.DMC5] = 18,
            [EfxVersion.RE7] = 16,
            [EfxVersion.RE8] = 27,
            [EfxVersion.RERT] = 27,
            [EfxVersion.RE4] = 29,
        },
        [EfxAttributeType.TypeMeshExpression] = new() {
            [EfxVersion.RE3] = 26,
            [EfxVersion.RE2] = 19,
            [EfxVersion.DMC5] = 19,
            [EfxVersion.RE7] = 17,
            [EfxVersion.RE8] = 28,
            [EfxVersion.RERT] = 28,
            [EfxVersion.RE4] = 30,
        },
        [EfxAttributeType.TypeRibbonFollow] = new() {
            [EfxVersion.RE3] = 27,
            [EfxVersion.RE2] = 20,
            [EfxVersion.DMC5] = 20,
            [EfxVersion.RE7] = 18,
            [EfxVersion.RE8] = 29,
            [EfxVersion.RERT] = 29,
            [EfxVersion.RE4] = 31,
        },
        [EfxAttributeType.TypeRibbonLength] = new() {
            [EfxVersion.RE3] = 28,
            [EfxVersion.RE2] = 21,
            [EfxVersion.DMC5] = 21,
            [EfxVersion.RE7] = 19,
            [EfxVersion.RE8] = 30,
            [EfxVersion.RERT] = 30,
            [EfxVersion.RE4] = 32,
        },
        [EfxAttributeType.TypeRibbonChain] = new() {
            [EfxVersion.RE3] = 29,
            [EfxVersion.RE2] = 22,
            [EfxVersion.DMC5] = 22,
            [EfxVersion.RE7] = 20,
            [EfxVersion.RE8] = 31,
            [EfxVersion.RERT] = 31,
            [EfxVersion.RE4] = 33,
        },
        [EfxAttributeType.TypeRibbonFixEnd] = new() {
            [EfxVersion.RE3] = 30,
            [EfxVersion.RE2] = 23,
            [EfxVersion.DMC5] = 23,
            [EfxVersion.RE8] = 32,
            [EfxVersion.RERT] = 32,
            [EfxVersion.RE4] = 34,
        },

        [EfxAttributeType.TypeRibbonLightweight] = new() {
            [EfxVersion.RE3] = 31,
            [EfxVersion.RE2] = 24,
            [EfxVersion.DMC5] = 24,
            [EfxVersion.RE8] = 33,
            [EfxVersion.RERT] = 33,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonParticle] = new() {
            [EfxVersion.RE8] = 34,
            [EfxVersion.RERT] = 34,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonFollowMaterial] = new() {
            [EfxVersion.RE8] = 35,
            [EfxVersion.RERT] = 35,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonFollowMaterialClip] = new() {
            [EfxVersion.RE8] = 36,
            [EfxVersion.RERT] = 36,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonFollowMaterialExpression] = new() {
            [EfxVersion.RE8] = 37,
            [EfxVersion.RERT] = 37,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonLengthMaterial] = new() {
            [EfxVersion.RE8] = 38,
            [EfxVersion.RERT] = 38,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonLengthMaterialClip] = new() {
            [EfxVersion.RE8] = 39,
            [EfxVersion.RERT] = 39,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonLengthMaterialExpression] = new() {
            [EfxVersion.RE8] = 40,
            [EfxVersion.RERT] = 40,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonChainMaterial] = new() {
            [EfxVersion.RE8] = 41,
            [EfxVersion.RERT] = 41,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonChainMaterialClip] = new() {
            [EfxVersion.RE8] = 42,
            [EfxVersion.RERT] = 42,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonChainMaterialExpression] = new() {
            [EfxVersion.RE8] = 43,
            [EfxVersion.RERT] = 43,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonFixEndMaterial] = new() {
            [EfxVersion.RE8] = 44,
            [EfxVersion.RERT] = 44,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonFixEndMaterialClip] = new() {
            [EfxVersion.RE8] = 45,
            [EfxVersion.RERT] = 45,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonFixEndMaterialExpression] = new() {
            [EfxVersion.RE8] = 46,
            [EfxVersion.RERT] = 46,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonLightweightMaterial] = new() {
            [EfxVersion.RE8] = 47,
            [EfxVersion.RERT] = 47,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonLightweightMaterialClip] = new() {
            [EfxVersion.RE8] = 48,
            [EfxVersion.RERT] = 48,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonLightweightMaterialExpression] = new() {
            [EfxVersion.RE8] = 49,
            [EfxVersion.RERT] = 49,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeStrainRibbonMaterial] = new() {
            [EfxVersion.RE8] = 50,
            [EfxVersion.RERT] = 50,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeStrainRibbonMaterialClip] = new() {
            [EfxVersion.RE8] = 51,
            [EfxVersion.RERT] = 51,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeStrainRibbonMaterialExpression] = new() {
            [EfxVersion.RE8] = 52,
            [EfxVersion.RERT] = 52,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonFollowExpression] = new() {
            [EfxVersion.RE3] = 32,
            [EfxVersion.RE2] = 25,
            [EfxVersion.DMC5] = 25,
            [EfxVersion.RE7] = 21,
            [EfxVersion.RE8] = 53,
            [EfxVersion.RERT] = 53,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonLengthExpression] = new() {
            [EfxVersion.RE3] = 33,
            [EfxVersion.RE2] = 26,
            [EfxVersion.DMC5] = 26,
            [EfxVersion.RE7] = 22,
            [EfxVersion.RE8] = 54,
            [EfxVersion.RERT] = 54,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonChainExpression] = new() {
            [EfxVersion.RE3] = 34,
            [EfxVersion.RE2] = 27,
            [EfxVersion.DMC5] = 27,
            [EfxVersion.RE7] = 23,
            [EfxVersion.RE8] = 55,
            [EfxVersion.RERT] = 55,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonFixEndExpression] = new() {
            [EfxVersion.RE3] = 35,
            [EfxVersion.RE2] = 28,
            [EfxVersion.DMC5] = 28,
            [EfxVersion.RE8] = 56,
            [EfxVersion.RERT] = 56,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonLightweightExpression] = new() {
            [EfxVersion.RE8] = 57,
            [EfxVersion.RERT] = 57,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonParticleExpression] = new() {
            [EfxVersion.RE8] = 58,
            [EfxVersion.RERT] = 58,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.unknRE4Struct58Expression] = new() {
            [EfxVersion.RE4] = 58,
        },
        [EfxAttributeType.unknRE4Struct60] = new() {
            [EfxVersion.RE4] = 60,
        },

        [EfxAttributeType.TypePolygon] = new() {
            [EfxVersion.RE3] = 36,
            [EfxVersion.RE2] = 29,
            [EfxVersion.DMC5] = 29,
            [EfxVersion.RE7] = 24,
            [EfxVersion.RE8] = 59,
            [EfxVersion.RERT] = 59,
            [EfxVersion.RE4] = 63,
        },
        [EfxAttributeType.TypePolygonClip] = new() {
            [EfxVersion.RE3] = 37,
            [EfxVersion.RE2] = 30,
            [EfxVersion.DMC5] = 30,
            [EfxVersion.RE7] = 25,
            [EfxVersion.RE8] = 60,
            [EfxVersion.RERT] = 60,
            [EfxVersion.RE4] = 64,
        },
        [EfxAttributeType.TypePolygonExpression] = new() {
            [EfxVersion.RE3] = 38,
            [EfxVersion.RE2] = 31,
            [EfxVersion.DMC5] = 31,
            [EfxVersion.RE7] = 26,
            [EfxVersion.RE8] = 61,
            [EfxVersion.RERT] = 61,
            [EfxVersion.RE4] = 65, // probably
        },
        [EfxAttributeType.TypePolygonMaterial] = new() {
            [EfxVersion.RE8] = 62,
            [EfxVersion.RERT] = 62,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypePolygonMaterialExpression] = new() {
            [EfxVersion.RE8] = 63,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeRibbonTrail] = new() {
            [EfxVersion.RE3] = 39,
            [EfxVersion.RE2] = 32,
            [EfxVersion.DMC5] = 32,
            [EfxVersion.RE7] = 27,
            [EfxVersion.RE8] = 64,
            [EfxVersion.RE4] = 69,
        },

        [EfxAttributeType.unknRE4Struct70] = new() {
            [EfxVersion.RERT] = 65,
            [EfxVersion.RE4] = 70, // TypePolygonTrail?
        },
        [EfxAttributeType.TypePolygonTrail] = new() {
            [EfxVersion.RE3] = 40,
            [EfxVersion.RE2] = 33,
            [EfxVersion.DMC5] = 33,
            [EfxVersion.RE7] = 28,
            [EfxVersion.RE8] = 65,
            [EfxVersion.RERT] = 66, // possibly wrong
            [EfxVersion.RE4] = UNKNOWN, // 70?
        },
        [EfxAttributeType.TypePolygonTrailExpression] = new() {
            [EfxVersion.RE8] = 66,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypePolygonTrailMaterial] = new() {
            [EfxVersion.RE8] = 67,
            [EfxVersion.RERT] = 67,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypePolygonTrailMaterialExpression] = new() {
            [EfxVersion.RE8] = 68,
            [EfxVersion.RE4] = UNKNOWN,
        },

        [EfxAttributeType.TypeNoDraw] = new() {
            [EfxVersion.RE3] = 41,
            [EfxVersion.RE2] = 34,
            [EfxVersion.DMC5] = 34,
            [EfxVersion.RE7] = 29,
            [EfxVersion.RE8] = 69,
            [EfxVersion.RERT] = 70,
            [EfxVersion.RE4] = 75,
        },
        [EfxAttributeType.TypeNoDrawExpression] = new() {
            [EfxVersion.RE3] = 42,
            [EfxVersion.RE2] = 35,
            [EfxVersion.DMC5] = 35,
            [EfxVersion.RE8] = 70,
            [EfxVersion.RERT] = 71,
            [EfxVersion.RE4] = 76,
        },
        [EfxAttributeType.Velocity2DDelayFrame] = new() {
            [EfxVersion.RE3] = 43,
            [EfxVersion.RE8] = 71,
            [EfxVersion.RERT] = 72,
            [EfxVersion.RE4] = 77,
        },
        [EfxAttributeType.Velocity2D] = new() {
            [EfxVersion.RE3] = 44,
            [EfxVersion.RE2] = 36,
            [EfxVersion.DMC5] = 36,
            [EfxVersion.RE7] = 30,
            [EfxVersion.RE8] = 72,
            [EfxVersion.RERT] = 73,
            [EfxVersion.RE4] = 78,
        },
        [EfxAttributeType.Velocity2DExpression] = new() {
            [EfxVersion.RE3] = 45,
            [EfxVersion.RE2] = 37,
            [EfxVersion.DMC5] = 37,
            [EfxVersion.RE7] = 31,
            [EfxVersion.RE8] = 73,
            [EfxVersion.RERT] = 74,
            [EfxVersion.RE4] = 79,
        },
        [EfxAttributeType.Velocity3DDelayFrame] = new() {
            [EfxVersion.RE3] = 46,
            [EfxVersion.RE8] = 74,
            [EfxVersion.RERT] = 75,
            [EfxVersion.RE4] = 80,
        },
        [EfxAttributeType.Velocity3D] = new() {
            [EfxVersion.RE3] = 47,
            [EfxVersion.RE2] = 38,
            [EfxVersion.DMC5] = 38,
            [EfxVersion.RE7] = 32,
            [EfxVersion.RE8] = 75,
            [EfxVersion.RERT] = 76,
            [EfxVersion.RE4] = 81,
        },
        [EfxAttributeType.Velocity3DExpression] = new() {
            [EfxVersion.RE3] = 48,
            [EfxVersion.RE2] = 39,
            [EfxVersion.DMC5] = 39,
            [EfxVersion.RE7] = 33,
            [EfxVersion.RE8] = 76,
            [EfxVersion.RERT] = 77,
            [EfxVersion.RE4] = 82,
        },
        [EfxAttributeType.RotateAnimDelayFrame] = new() {
            [EfxVersion.RE3] = 49,
            [EfxVersion.RE8] = 77,
            [EfxVersion.RERT] = 78,
            [EfxVersion.RE4] = 83,
        },
        [EfxAttributeType.RotateAnim] = new() {
            [EfxVersion.RE3] = 50,
            [EfxVersion.RE2] = 40,
            [EfxVersion.DMC5] = 40,
            [EfxVersion.RE7] = 34,
            [EfxVersion.RE8] = 78,
            [EfxVersion.RERT] = 79,
            [EfxVersion.RE4] = 84,
        },
        [EfxAttributeType.RotateAnimExpression] = new() {
            [EfxVersion.RE3] = 51,
            [EfxVersion.RE2] = 41,
            [EfxVersion.DMC5] = 41,
            [EfxVersion.RE7] = 35,
            [EfxVersion.RE8] = 79,
            [EfxVersion.RERT] = 80,
            [EfxVersion.RE4] = 85,
        },
        [EfxAttributeType.ScaleAnimDelayFrame] = new() {
            [EfxVersion.RE3] = 52,
            [EfxVersion.RE8] = 80,
            [EfxVersion.RERT] = 81,
            [EfxVersion.RE4] = 86,
        },
        [EfxAttributeType.ScaleAnim] = new() {
            [EfxVersion.RE3] = 53,
            [EfxVersion.RE2] = 42,
            [EfxVersion.DMC5] = 42,
            [EfxVersion.RE7] = 36,
            [EfxVersion.RE8] = 81,
            [EfxVersion.RERT] = 82,
            [EfxVersion.RE4] = 87,
        },
        [EfxAttributeType.ScaleAnimExpression] = new() {
            [EfxVersion.RE3] = 54,
            [EfxVersion.RE2] = 43,
            [EfxVersion.DMC5] = 43,
            [EfxVersion.RE7] = 37,
            [EfxVersion.RE8] = 82,
            [EfxVersion.RERT] = 83,
            [EfxVersion.RE4] = 88, // unused
        },
        [EfxAttributeType.VanishArea3D] = new() {
            [EfxVersion.RE3] = 55,
            [EfxVersion.RE8] = 83,
            [EfxVersion.RERT] = 84,
            [EfxVersion.RE4] = 89,
        },
        [EfxAttributeType.VanishArea3DExpression] = new() {
            [EfxVersion.RE3] = 56,
            [EfxVersion.RE8] = 84,
            [EfxVersion.RERT] = 85,
            [EfxVersion.RE4] = 90, // unused
        },
        [EfxAttributeType.Life] = new() {
            [EfxVersion.RE3] = 57,
            [EfxVersion.RE2] = 44,
            [EfxVersion.DMC5] = 44,
            [EfxVersion.RE7] = 38,
            [EfxVersion.RE8] = 85,
            [EfxVersion.RERT] = 86,
            [EfxVersion.RE4] = 91,
        },
        [EfxAttributeType.LifeExpression] = new() {
            [EfxVersion.RE3] = 58,
            [EfxVersion.RE2] = 45,
            [EfxVersion.DMC5] = 45,
            [EfxVersion.RE7] = 39,
            [EfxVersion.RE8] = 86,
            [EfxVersion.RERT] = 87,
            [EfxVersion.RE4] = 92,
        },
        [EfxAttributeType.UVSequence] = new() {
            [EfxVersion.RE3] = 59,
            [EfxVersion.RE2] = 46,
            [EfxVersion.DMC5] = 46,
            [EfxVersion.RE7] = 40,
            [EfxVersion.RE8] = 87,
            [EfxVersion.RERT] = 88,
            [EfxVersion.RE4] = 93,
        },
        [EfxAttributeType.UVSequenceModifier] = new() {
            [EfxVersion.RE3] = 60,
            [EfxVersion.RE8] = 88,
            [EfxVersion.RERT] = 89,
            [EfxVersion.RE4] = 94,
        },
        [EfxAttributeType.UVSequenceExpression] = new() {
            [EfxVersion.RE3] = 61,
            [EfxVersion.RE2] = 47,
            [EfxVersion.DMC5] = 47,
            [EfxVersion.RE7] = 41,
            [EfxVersion.RE8] = 89,
            [EfxVersion.RERT] = 90,
            [EfxVersion.RE4] = 95,
        },
        [EfxAttributeType.UVScroll] = new() {
            [EfxVersion.RE3] = 62,
            [EfxVersion.RE2] = 48,
            [EfxVersion.DMC5] = 48,
            [EfxVersion.RE8] = 90,
            [EfxVersion.RERT] = 91,
            [EfxVersion.RE4] = 96,
        },
        [EfxAttributeType.TextureUnit] = new() {
            [EfxVersion.RE3] = 63,
            [EfxVersion.RE2] = 49,
            [EfxVersion.DMC5] = 49,
            [EfxVersion.RE8] = 91,
            [EfxVersion.RERT] = 92,
            [EfxVersion.RE4] = 97,
        },
        [EfxAttributeType.TextureUnitExpression] = new() {
            [EfxVersion.RE3] = 64,
            [EfxVersion.RE8] = 92,
            [EfxVersion.RERT] = 93,
            [EfxVersion.RE4] = 98, // unused
        },
        [EfxAttributeType.TextureFilter] = new() {
            [EfxVersion.RE3] = 65,
            [EfxVersion.RE8] = 93,
            [EfxVersion.RERT] = 94,
            [EfxVersion.RE4] = 99,
        },
        [EfxAttributeType.EmitterShape2D] = new() {
            [EfxVersion.RE3] = 66,
            [EfxVersion.RE2] = 50,
            [EfxVersion.DMC5] = 50,
            [EfxVersion.RE7] = 42,
            [EfxVersion.RE8] = 94,
            [EfxVersion.RERT] = 95,
            [EfxVersion.RE4] = 100,
        },
        [EfxAttributeType.EmitterShape2DExpression] = new() {
            [EfxVersion.RE3] = 67,
            [EfxVersion.RE2] = 51,
            [EfxVersion.DMC5] = 51,
            [EfxVersion.RE7] = 43,
            [EfxVersion.RE8] = 95,
            [EfxVersion.RERT] = 96,
            [EfxVersion.RE4] = 101,
        },
        [EfxAttributeType.EmitterShape3D] = new() {
            [EfxVersion.RE3] = 68,
            [EfxVersion.RE2] = 52,
            [EfxVersion.DMC5] = 52,
            [EfxVersion.RE7] = 44,
            [EfxVersion.RE8] = 96,
            [EfxVersion.RERT] = 97,
            [EfxVersion.RE4] = 102,
        },
        [EfxAttributeType.EmitterShape3DExpression] = new() {
            [EfxVersion.RE3] = 69,
            [EfxVersion.RE2] = 53,
            [EfxVersion.DMC5] = 53,
            [EfxVersion.RE7] = 45,
            [EfxVersion.RE8] = 97,
            [EfxVersion.RERT] = 98,
            [EfxVersion.RE4] = 103,
        },
        [EfxAttributeType.AlphaCorrection] = new() {
            [EfxVersion.RE3] = 70,
            [EfxVersion.RE2] = 54,
            [EfxVersion.DMC5] = 54,
            [EfxVersion.RE7] = 46,
            [EfxVersion.RE8] = 98,
            [EfxVersion.RERT] = 99,
            [EfxVersion.RE4] = 104,
        },
        [EfxAttributeType.ContrastHighlighter] = new() {
            [EfxVersion.RE3] = 71,
            [EfxVersion.RE8] = 99,
            [EfxVersion.RERT] = 100,
            [EfxVersion.RE4] = 105, // probably
        },
        [EfxAttributeType.ColorGrading] = new() {
            [EfxVersion.RE3] = 72,
            [EfxVersion.RE8] = 100,
            [EfxVersion.RERT] = 101,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.Blink] = new() {
            [EfxVersion.RE8] = 101,
            [EfxVersion.RERT] = 102,
            [EfxVersion.RE4] = 108,
        },

        [EfxAttributeType.Noise] = new() {
            [EfxVersion.RE8] = 102,
            [EfxVersion.RERT] = 103,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.NoiseExpression] = new() {
            [EfxVersion.RERT] = 104,
            [EfxVersion.RE4] = UNKNOWN,
        },

        // TODO noise and noise expression probably?
        [EfxAttributeType.unknRE4Struct109] = new() {
            [EfxVersion.RE4] = 109,
        },
        [EfxAttributeType.unknRE4Struct110] = new() {
            [EfxVersion.RE4] = 110,
        },

        [EfxAttributeType.TexelChannelOperator] = new() {
            [EfxVersion.RE3] = 73,
            [EfxVersion.RE8] = 103,
            [EfxVersion.RERT] = 105,
            [EfxVersion.RE4] = 111,
        },
        [EfxAttributeType.TexelChannelOperatorClip] = new() {
            [EfxVersion.RE3] = 74,
            [EfxVersion.RE8] = 104,
            [EfxVersion.RERT] = 106,
            [EfxVersion.RE4] = 112, // unused
        },
        [EfxAttributeType.TexelChannelOperatorExpression] = new() {
            [EfxVersion.RE3] = 75,
            [EfxVersion.RE8] = 105,
            [EfxVersion.RERT] = 107,
            [EfxVersion.RE4] = 113, // unused
        },
        [EfxAttributeType.unknRE4Struct114] = new() {
            [EfxVersion.RE4] = 114, // TypeStrainRibbon?
        },
        [EfxAttributeType.TypeStrainRibbon] = new() {
            [EfxVersion.RE3] = 76,
            [EfxVersion.RE2] = 55,
            [EfxVersion.DMC5] = 55,
            [EfxVersion.RE7] = 47,
            [EfxVersion.RE8] = 106,
            [EfxVersion.RERT] = 108,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeStrainRibbonExpression] = new() {
            [EfxVersion.RE3] = 77,
            [EfxVersion.RE2] = 56,
            [EfxVersion.DMC5] = 56,
            [EfxVersion.RE7] = 48,
            [EfxVersion.RE8] = 107,
            [EfxVersion.RERT] = 109,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeLightning3D] = new() {
            [EfxVersion.RE3] = 78,
            [EfxVersion.RE2] = 57,
            [EfxVersion.DMC5] = 57,
            [EfxVersion.RE8] = 108,
            [EfxVersion.RERT] = 110,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeLightning3DExpression] = new() {
            [EfxVersion.RE8] = 109,
            [EfxVersion.RERT] = 111,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeLightning3DMaterial] = new() {
            [EfxVersion.RE8] = 110,
            [EfxVersion.RERT] = 112,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.ShaderSettings] = new() {
            [EfxVersion.RE3] = 79,
            [EfxVersion.RE2] = 58,
            [EfxVersion.DMC5] = 58,
            [EfxVersion.RE7] = 49,
            [EfxVersion.RE8] = 111,
            [EfxVersion.RERT] = 113,
            [EfxVersion.RE4] = 123,
        },
        [EfxAttributeType.ShaderSettingsExpression] = new() {
            [EfxVersion.RE3] = 80,
            [EfxVersion.RE2] = 59,
            [EfxVersion.DMC5] = 59,
            [EfxVersion.RE7] = 50,
            [EfxVersion.RE8] = 112,
            [EfxVersion.RERT] = 114, // unused
            [EfxVersion.RE4] = 124, // unused
        },
        [EfxAttributeType.Distortion] = new() {
            [EfxVersion.RE3] = 81,
            [EfxVersion.RE2] = 60,
            [EfxVersion.DMC5] = 60,
            [EfxVersion.RE7] = 51,
            [EfxVersion.RE8] = 113,
            [EfxVersion.RERT] = 115,
            [EfxVersion.RE4] = 125,
        },
        [EfxAttributeType.DistortionExpression] = new() {
            [EfxVersion.RE8] = 114,
            [EfxVersion.RERT] = 116,
            [EfxVersion.RE4] = 126, // unused
        },
        [EfxAttributeType.VolumetricLighting] = new() {
            [EfxVersion.RE8] = 115,
            [EfxVersion.RE4] = 127, // probably, unused
        },
        [EfxAttributeType.RenderTarget] = new() {
            [EfxVersion.RE3] = 82,
            [EfxVersion.RE2] = 61,
            [EfxVersion.DMC5] = 61,
            [EfxVersion.RE7] = 52,
            [EfxVersion.RE8] = 116,
            [EfxVersion.RERT] = 118,
            [EfxVersion.RE4] = 128,
        },
        [EfxAttributeType.unknRE4Struct130] = new() {
            [EfxVersion.RE4] = 130,
        },
        [EfxAttributeType.PtLife] = new() {
            [EfxVersion.RE3] = 83,
            [EfxVersion.RE2] = 62,
            [EfxVersion.DMC5] = 62,
            [EfxVersion.RE7] = 53,
            [EfxVersion.RE8] = 117,
            [EfxVersion.RERT] = 119,
            [EfxVersion.RE4] = 131,
        },
        [EfxAttributeType.PtBehavior] = new() {
            [EfxVersion.RE3] = 84,
            [EfxVersion.RE2] = 63,
            [EfxVersion.DMC5] = 63,
            [EfxVersion.RE7] = 54,
            [EfxVersion.RE8] = 118,
            [EfxVersion.RERT] = 120,
            [EfxVersion.RE4] = 132,
        },
        [EfxAttributeType.PtBehaviorClip] = new() {
            [EfxVersion.RE3] = 85,
            [EfxVersion.RE2] = 64,
            [EfxVersion.DMC5] = 64,
            [EfxVersion.RE7] = 55,
            [EfxVersion.RE8] = 119,
            [EfxVersion.RERT] = 121, // probably, unused
            [EfxVersion.RE4] = 133, // probably, unused
        },
        [EfxAttributeType.PlayEfx] = new() {
            [EfxVersion.RE3] = 86,
            [EfxVersion.RE2] = 65,
            [EfxVersion.DMC5] = 65,
            [EfxVersion.RE7] = 56,
            [EfxVersion.RE8] = 120,
            [EfxVersion.RERT] = 122,
            [EfxVersion.RE4] = 134, // guess
        },
        [EfxAttributeType.FadeByAngle] = new() {
            [EfxVersion.RE3] = 87,
            [EfxVersion.RE2] = 66,
            [EfxVersion.DMC5] = 66,
            [EfxVersion.RE7] = 57,
            [EfxVersion.RE8] = 121,
            [EfxVersion.RERT] = 123,
            [EfxVersion.RE4] = 135,
        },
        [EfxAttributeType.FadeByAngleExpression] = new() {
            [EfxVersion.RE3] = 88,
            [EfxVersion.RE2] = 67,
            [EfxVersion.DMC5] = 67,
            [EfxVersion.RE7] = 58,
            [EfxVersion.RE8] = 122,
            [EfxVersion.RERT] = 124,
            [EfxVersion.RE4] = 136,
        },
        [EfxAttributeType.FadeByEmitterAngle] = new() {
            [EfxVersion.RE3] = 89,
            [EfxVersion.RE2] = 68,
            [EfxVersion.DMC5] = 68,
            [EfxVersion.RE8] = 123,
            [EfxVersion.RERT] = 125,
            [EfxVersion.RE4] = 137, // probably
        },
        [EfxAttributeType.FadeByDepth] = new() {
            [EfxVersion.RE3] = 90,
            [EfxVersion.RE2] = 69,
            [EfxVersion.DMC5] = 69,
            [EfxVersion.RE7] = 59,
            [EfxVersion.RE8] = 124,
            [EfxVersion.RERT] = 126,
            [EfxVersion.RE4] = 138,
        },
        [EfxAttributeType.FadeByDepthExpression] = new() {
            [EfxVersion.RE3] = 91,
            [EfxVersion.RE2] = 70,
            [EfxVersion.DMC5] = 70,
            [EfxVersion.RE7] = 60,
            [EfxVersion.RE8] = 125,
            [EfxVersion.RERT] = 127,
            [EfxVersion.RE4] = 139,
        },
        [EfxAttributeType.FadeByOcclusion] = new() {
            [EfxVersion.RE3] = 92,
            [EfxVersion.RE2] = 71,
            [EfxVersion.DMC5] = 71,
            [EfxVersion.RE7] = 61,
            [EfxVersion.RE8] = 126,
            [EfxVersion.RERT] = 128,
            [EfxVersion.RE4] = 140,
        },
        [EfxAttributeType.FadeByOcclusionExpression] = new() {
            [EfxVersion.RE3] = 93,
            [EfxVersion.RE2] = 72,
            [EfxVersion.DMC5] = 72,
            [EfxVersion.RE7] = 62,
            [EfxVersion.RE8] = 127,
            [EfxVersion.RERT] = 129,
            [EfxVersion.RE4] = 141,
        },
        [EfxAttributeType.FakeDoF] = new() {
            [EfxVersion.RE3] = 94,
            [EfxVersion.RE2] = 73,
            [EfxVersion.DMC5] = 73,
            [EfxVersion.RE7] = 63,
            [EfxVersion.RE8] = 128,
            [EfxVersion.RERT] = 130, // guess
            [EfxVersion.RE4] = 142,
        },
        [EfxAttributeType.LuminanceBleed] = new() {
            [EfxVersion.RE3] = 95,
            [EfxVersion.RE2] = 74,
            [EfxVersion.DMC5] = 74,
            [EfxVersion.RE7] = 64,
            [EfxVersion.RE8] = 129,
            [EfxVersion.RERT] = 131,
            [EfxVersion.RE4] = 143,
        },

        [EfxAttributeType.ScaleByDepth] = new() {
            [EfxVersion.RE8] = 130,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeNodeBillboard] = new() {
            [EfxVersion.RE3] = 96,
            [EfxVersion.RE2] = 75,
            [EfxVersion.DMC5] = 75,
            [EfxVersion.RE7] = 65,
            [EfxVersion.RE8] = 131,
            [EfxVersion.RERT] = 133,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeNodeBillboardExpression] = new() {
            [EfxVersion.RE3] = 97,
            [EfxVersion.RE2] = 76,
            [EfxVersion.DMC5] = 76,
            [EfxVersion.RE7] = 66,
            [EfxVersion.RE8] = 132,
            [EfxVersion.RERT] = 134,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.UnitCulling] = new() {
            [EfxVersion.RE3] = 98,
            [EfxVersion.RE2] = 77,
            [EfxVersion.DMC5] = 77,
            [EfxVersion.RE7] = 67,
            [EfxVersion.RE8] = 133,
            [EfxVersion.RERT] = 135, // possibly wrong
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.unknRERTStruct136] = new() {
            [EfxVersion.RERT] = 136,
        },
        [EfxAttributeType.FluidEmitter2D] = new() {
            [EfxVersion.RE3] = 99,
            [EfxVersion.RE2] = 78,
            [EfxVersion.DMC5] = 78,
            [EfxVersion.RE7] = 68,
            [EfxVersion.RE8] = 134,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.FluidEmitter2DClip] = new() {
            [EfxVersion.RE8] = 135,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.FluidEmitter2DExpression] = new() {
            [EfxVersion.RE8] = 136,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.FluidSimulator2D] = new() {
            [EfxVersion.RE3] = 100,
            [EfxVersion.RE2] = 79,
            [EfxVersion.DMC5] = 79,
            [EfxVersion.RE7] = 69,
            [EfxVersion.RE8] = 137,
            [EfxVersion.RERT] = 139,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.unknRE4Struct144] = new() {
            [EfxVersion.RE4] = 144,
        },
        [EfxAttributeType.unknRE4Struct150] = new() {
            [EfxVersion.RE4] = 150,
        },
        [EfxAttributeType.PlayEmitter] = new() {
            [EfxVersion.RE3] = 101,
            [EfxVersion.RE2] = 80,
            [EfxVersion.DMC5] = 80,
            [EfxVersion.RE7] = 70,
            [EfxVersion.RE8] = 138,
            [EfxVersion.RERT] = 140,
            [EfxVersion.RE4] = 151,
        },
        [EfxAttributeType.PtTransform3D] = new() {
            [EfxVersion.RE3] = 102,
            [EfxVersion.RE2] = 81,
            [EfxVersion.DMC5] = 81,
            [EfxVersion.RE7] = 71,
            [EfxVersion.RE8] = 139,
            [EfxVersion.RERT] = 141,
            [EfxVersion.RE4] = 152,
        },
        [EfxAttributeType.PtTransform3DClip] = new() {
            [EfxVersion.RE3] = 103,
            [EfxVersion.RE2] = 82,
            [EfxVersion.DMC5] = 82,
            [EfxVersion.RE7] = 72,
            [EfxVersion.RE8] = 140,
            [EfxVersion.RERT] = 142,
            [EfxVersion.RE4] = 153,
        },

        [EfxAttributeType.PtTransform2D] = new() {
            [EfxVersion.RE3] = 104,
            [EfxVersion.RE2] = 83,
            [EfxVersion.DMC5] = 83,
            [EfxVersion.RE7] = 73,
            [EfxVersion.RE8] = 141,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.PtTransform2DClip] = new() {
            [EfxVersion.RE3] = 105,
            [EfxVersion.RE2] = 84,
            [EfxVersion.DMC5] = 84,
            [EfxVersion.RE7] = 74,
            [EfxVersion.RE8] = 142,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.unknRE4Struct155] = new() {
            [EfxVersion.RE4] = 155,
            // [EfxVersion.RERT] = 145,
        },

        [EfxAttributeType.PtVelocity3D] = new() {
            [EfxVersion.RE3] = 106,
            [EfxVersion.RE2] = 85,
            [EfxVersion.DMC5] = 85,
            [EfxVersion.RE7] = 75,
            [EfxVersion.RE8] = 143,
            [EfxVersion.RERT] = 145, // should be correct
            [EfxVersion.RE4] = 157,
        },
        [EfxAttributeType.PtVelocity3DClip] = new() {
            [EfxVersion.RE3] = 107,
            [EfxVersion.RE2] = 86,
            [EfxVersion.DMC5] = 86,
            [EfxVersion.RE7] = 76,
            [EfxVersion.RE8] = 144,
            [EfxVersion.RERT] = 146,
            [EfxVersion.RE4] = 158, // unused
        },
        [EfxAttributeType.PtVelocity2D] = new() {
            [EfxVersion.RE3] = 108,
            [EfxVersion.RE2] = 87,
            [EfxVersion.DMC5] = 87,
            [EfxVersion.RE7] = 77,
            [EfxVersion.RE8] = 145,
            [EfxVersion.RERT] = 147, // guess
            [EfxVersion.RE4] = 159, // probably
        },
        [EfxAttributeType.PtVelocity2DClip] = new() {
            [EfxVersion.RE3] = 109,
            [EfxVersion.RE2] = 88,
            [EfxVersion.DMC5] = 88,
            [EfxVersion.RE7] = 78,
            [EfxVersion.RE8] = 146,
            [EfxVersion.RERT] = 149, // guess
            [EfxVersion.RE4] = 160, // probably
        },
        [EfxAttributeType.PtColliderAction] = new() {
            [EfxVersion.RE3] = 110,
            [EfxVersion.RE2] = 89,
            [EfxVersion.DMC5] = 89,
            [EfxVersion.RE7] = 79,
            [EfxVersion.RE8] = 147,
            [EfxVersion.RERT] = 150, // guess
            [EfxVersion.RE4] = 161,
        },
        [EfxAttributeType.PtCollision] = new() {
            [EfxVersion.RE3] = 111,
            [EfxVersion.RE2] = 90,
            [EfxVersion.DMC5] = 90,
            [EfxVersion.RE7] = 80,
            [EfxVersion.RE8] = 148,
            [EfxVersion.RE4] = 162,
        },
        [EfxAttributeType.unknRERTStruct151_PtColor] = new() {
            [EfxVersion.RERT] = 151, // same as 153
        },
        [EfxAttributeType.unknRERTStruct152_PtColorClip] = new() {
            [EfxVersion.RERT] = 152, // same as 154
        },
        [EfxAttributeType.PtColor] = new() {
            [EfxVersion.RE3] = 112,
            [EfxVersion.RE2] = 91,
            [EfxVersion.DMC5] = 91,
            [EfxVersion.RE7] = 81,
            [EfxVersion.RE8] = 149,
            [EfxVersion.RERT] = 153, // same as 151
            [EfxVersion.RE4] = 164,
        },
        [EfxAttributeType.PtColorClip] = new() {
            [EfxVersion.RE3] = 113,
            [EfxVersion.RE2] = 92,
            [EfxVersion.DMC5] = 92,
            [EfxVersion.RE7] = 82,
            [EfxVersion.RE8] = 150,
            [EfxVersion.RERT] = 154, // same as 152
            [EfxVersion.RE4] = 165,
        },
        [EfxAttributeType.PtUvSequence] = new() {
            [EfxVersion.RE3] = 114,
            [EfxVersion.RE2] = 93,
            [EfxVersion.DMC5] = 93,
            [EfxVersion.RE7] = 83,
            [EfxVersion.RE8] = 151,
            [EfxVersion.RERT] = UNKNOWN,
            [EfxVersion.RE4] = 168,
        },
        [EfxAttributeType.PtUvSequenceClip] = new() {
            [EfxVersion.RE3] = 115,
            [EfxVersion.RE2] = 94,
            [EfxVersion.DMC5] = 94,
            [EfxVersion.RE7] = 84,
            [EfxVersion.RE8] = 152,
            [EfxVersion.RE4] = 169,
        },
        [EfxAttributeType.MeshEmitter] = new() {
            [EfxVersion.RE3] = 116,
            [EfxVersion.RE2] = 95,
            [EfxVersion.DMC5] = 95,
            [EfxVersion.RE8] = 153,
            [EfxVersion.MHRiseSB] = 156,
            [EfxVersion.RERT] = 155,
            [EfxVersion.RE4] = 170, // probably
        },
        [EfxAttributeType.MeshEmitterClip] = new() {
            [EfxVersion.RE3] = 117,
            [EfxVersion.RE2] = 96,
            [EfxVersion.DMC5] = 96,
            [EfxVersion.RE8] = 154,
            [EfxVersion.RERT] = 156, // probably
            [EfxVersion.RE4] = 171, // unused
        },
        [EfxAttributeType.MeshEmitterExpression] = new() {
            [EfxVersion.RE3] = 118,
            [EfxVersion.RE2] = 97,
            [EfxVersion.DMC5] = 97,
            [EfxVersion.RE8] = 155,
            [EfxVersion.RERT] = 157, // probably
            [EfxVersion.RE4] = 172, // unused
        },
        [EfxAttributeType.ScreenSpaceEmitter] = new() {
            [EfxVersion.RE8] = 156,
            [EfxVersion.RERT] = 159,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.unknRE4Struct173] = new() {
            [EfxVersion.RE4] = 173, // screen space emitter?
        },
        [EfxAttributeType.VectorFieldParameter] = new() {
            [EfxVersion.RE3] = 119,
            [EfxVersion.RE2] = 98,
            [EfxVersion.DMC5] = 98,
            [EfxVersion.RE8] = 157,
            [EfxVersion.RERT] = 160,
            [EfxVersion.RE4] = 175,
        },
        [EfxAttributeType.VectorFieldParameterClip] = new() {
            [EfxVersion.RE3] = 120,
            [EfxVersion.RE2] = 99,
            [EfxVersion.DMC5] = 99,
            [EfxVersion.RE8] = 158,
            [EfxVersion.RERT] = 161,
            [EfxVersion.RE4] = 176, // probably, unused
        },
        [EfxAttributeType.VectorFieldParameterExpression] = new() {
            [EfxVersion.RE3] = 121,
            [EfxVersion.RE2] = 100,
            [EfxVersion.DMC5] = 100,
            [EfxVersion.RE8] = 159,
            [EfxVersion.RERT] = 162,
            [EfxVersion.RE4] = 177, // probably
        },

        [EfxAttributeType.GlobalVectorField] = new() {
            [EfxVersion.RE8] = 160,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.GlobalVectorFieldClip] = new() {
            [EfxVersion.RE8] = 161,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.GlobalVectorFieldExpression] = new() {
            [EfxVersion.RE8] = 162,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.DirectionalFieldParameter] = new() {
            [EfxVersion.RE8] = 163,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.DirectionalFieldParameterClip] = new() {
            [EfxVersion.RE8] = 164,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.DirectionalFieldParameterExpression] = new() {
            [EfxVersion.RE8] = 165,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.DepthOperator] = new() {
            [EfxVersion.RE3] = 122,
            [EfxVersion.RE2] = 101,
            [EfxVersion.DMC5] = 101,
            [EfxVersion.RE8] = 166,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.PlaneCollider] = new() {
            [EfxVersion.RE3] = 123,
            [EfxVersion.RE8] = 167,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.PlaneColliderExpression] = new() {
            [EfxVersion.RE3] = 124,
            [EfxVersion.RE8] = 168,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.DepthOcclusion] = new() {
            [EfxVersion.RE3] = 125,
            [EfxVersion.RE8] = 169,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.ShapeOperator] = new() {
            [EfxVersion.RE3] = 126,
            [EfxVersion.RE2] = 102,
            [EfxVersion.DMC5] = 102,
            [EfxVersion.RE8] = 170,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.ShapeOperatorExpression] = new() {
            [EfxVersion.RE3] = 127,
            [EfxVersion.RE2] = 103,
            [EfxVersion.DMC5] = 103,
            [EfxVersion.RE8] = 171,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.WindInfluence3DDelayFrame] = new() {
            [EfxVersion.RE3] = 128,
            [EfxVersion.RE8] = 172,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.WindInfluence3D] = new() {
            [EfxVersion.RE3] = 129,
            [EfxVersion.RE2] = 104,
            [EfxVersion.DMC5] = 104,
            [EfxVersion.RE8] = 173,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.unknRERTStruct168] = new() {
            [EfxVersion.RERT] = 168,
        },
        [EfxAttributeType.unknRE4Struct184] = new() {
            [EfxVersion.RE4] = 184,
        },
        [EfxAttributeType.unknRE4Struct185] = new() {
            [EfxVersion.RE4] = 185,
        },
        [EfxAttributeType.unknRE4Struct187] = new() {
            [EfxVersion.RE4] = 187,
        },
        [EfxAttributeType.unknRE4Struct191] = new() {
            [EfxVersion.RE4] = 191,
        }, // TODO see if we can}match these unknowns to the previous ones

        [EfxAttributeType.Attractor] = new() {
            [EfxVersion.RE3] = 130,
            [EfxVersion.RE8] = 174,
            [EfxVersion.RERT] = 176,
            [EfxVersion.RE4] = 192,
        },
        [EfxAttributeType.AttractorClip] = new() {
            [EfxVersion.RE3] = 131,
            [EfxVersion.RE8] = 175,
            [EfxVersion.RERT] = 177, // probably
            [EfxVersion.RE4] = 193, // unused
        },
        [EfxAttributeType.AttractorExpression] = new() {
            [EfxVersion.RE3] = 132,
            [EfxVersion.RE8] = 176,
            [EfxVersion.RERT] = 178, // probably
            [EfxVersion.RE4] = 194, // unused
        },
        [EfxAttributeType.unknRE4Struct195] = new() {
            [EfxVersion.RE4] = 195,
        },

        // these 2 look pretty similar
        [EfxAttributeType.unknRE4Struct197] = new() {
            [EfxVersion.RE4] = 197,
        },
        [EfxAttributeType.unknRERTStruct219] = new() {
            [EfxVersion.RERT] = 219,
        },
        [EfxAttributeType.unknRERTStruct220Expression] = new() {
            [EfxVersion.RERT] = 220,
        },

        [EfxAttributeType.CustomComputeShader] = new() {
            [EfxVersion.RE3] = 133,
            [EfxVersion.RE8] = 177,
        },
        [EfxAttributeType.TypeGpuBillboard] = new() {
            [EfxVersion.RE3] = 134,
            [EfxVersion.RE2] = 105,
            [EfxVersion.DMC5] = 105,
            [EfxVersion.RE7] = 85,
            [EfxVersion.RE8] = 178,
            [EfxVersion.RERT] = 221,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeGpuBillboardExpression] = new() {
            [EfxVersion.RE3] = 135,
            [EfxVersion.RE2] = 106,
            [EfxVersion.DMC5] = 106,
            [EfxVersion.RE8] = 179,
            [EfxVersion.RERT] = 222,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeGpuPolygon] = new() {
            [EfxVersion.RE3] = 136,
            [EfxVersion.RE8] = 180,
            [EfxVersion.RERT] = 223,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeGpuPolygonExpression] = new() {
            [EfxVersion.RE8] = 181,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeGpuRibbonFollow] = new() {
            [EfxVersion.RE3] = 137,
            [EfxVersion.RE2] = 107,
            [EfxVersion.DMC5] = 107,
            [EfxVersion.RE8] = 182,
            [EfxVersion.RERT] = 225,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeGpuRibbonFollowExpression] = new() {
            [EfxVersion.RE8] = 183,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeGpuRibbonLength] = new() {
            [EfxVersion.RE3] = 138,
            [EfxVersion.RE8] = 184,
            [EfxVersion.RERT] = 227,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeGpuRibbonLengthExpression] = new() {
            [EfxVersion.RE8] = 185,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeGpuMesh] = new() {
            [EfxVersion.RE3] = 139,
            [EfxVersion.RE8] = 186,
            [EfxVersion.RERT] = 229,
            [EfxVersion.RE4] = 251,
        },
        [EfxAttributeType.TypeGpuMeshExpression] = new() {
            [EfxVersion.RE3] = 140,
            [EfxVersion.RE8] = 187,
            [EfxVersion.RERT] = 230,
            [EfxVersion.RE4] = 252, // unused
        },
        [EfxAttributeType.TypeGpuMeshTrail] = new() {
            [EfxVersion.RE8] = 188,
            [EfxVersion.RERT] = 231,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeGpuMeshTrailClip] = new() {
            [EfxVersion.RE8] = 189,
            [EfxVersion.RERT] = 232,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeGpuMeshTrailExpression] = new() {
            [EfxVersion.RE8] = 190,
            [EfxVersion.RERT] = 233,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeGpuLightning3D] = new() {
            [EfxVersion.RE3] = 141,
            [EfxVersion.RE8] = 191,
            [EfxVersion.RERT] = 234,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.EmitterPriority] = new() {
            [EfxVersion.RE3] = 142,
            [EfxVersion.RE2] = 108,
            [EfxVersion.DMC5] = 108,
            [EfxVersion.RE7] = 86,
            [EfxVersion.RE8] = 192,
            [EfxVersion.RERT] = 181,
            [EfxVersion.RE4] = 200,
        },
        [EfxAttributeType.DrawOverlay] = new() {
            [EfxVersion.RE3] = 143,
            [EfxVersion.RE2] = 109,
            [EfxVersion.DMC5] = 109,
            [EfxVersion.RE8] = 193,
            [EfxVersion.RERT] = 182,
            [EfxVersion.RE4] = 201,
        },
        [EfxAttributeType.VectorField] = new() {
            [EfxVersion.RE3] = 144,
            [EfxVersion.RE2] = 110,
            [EfxVersion.DMC5] = 110,
            [EfxVersion.RE8] = 194,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.VolumeField] = new() {
            [EfxVersion.RE3] = 145,
            [EfxVersion.RE2] = 111,
            [EfxVersion.DMC5] = 111,
            [EfxVersion.RE8] = 195,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.DirectionalField] = new() {
            [EfxVersion.RE8] = 196,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.AngularVelocity3DDelayFrame] = new() {
            [EfxVersion.RE3] = 146,
            [EfxVersion.RE8] = 197,
            [EfxVersion.RERT] = 186, // guess
            [EfxVersion.RE4] = 205,
        },
        [EfxAttributeType.AngularVelocity3D] = new() {
            [EfxVersion.RE3] = 147,
            [EfxVersion.RE2] = 112,
            [EfxVersion.DMC5] = 112,
            [EfxVersion.RE8] = 198,
            [EfxVersion.RERT] = 187,
            [EfxVersion.RE4] = 206,
        },

        [EfxAttributeType.PtAngularVelocity3D] = new() {
            [EfxVersion.RE3] = 148,
            [EfxVersion.DMC5] = 113,
            [EfxVersion.RE8] = 199,
            [EfxVersion.RERT] = 189,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.PtAngularVelocity3DExpression] = new() {
            [EfxVersion.RE3] = 149,
            [EfxVersion.DMC5] = 114,
            [EfxVersion.RE8] = 200,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.unknSBStruct189_PtAngularVelocity3D] = new() {
            [EfxVersion.MHRiseSB] = 189,
        },
        [EfxAttributeType.AngularVelocity2DDelayFrame] = new() {
            [EfxVersion.RE3] = 150,
            [EfxVersion.RE8] = 201,
            [EfxVersion.RERT] = 191,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.AngularVelocity2D] = new() {
            [EfxVersion.RE3] = 151,
            [EfxVersion.RE2] = 113,
            [EfxVersion.DMC5] = 115,
            [EfxVersion.RE8] = 202,
            [EfxVersion.RERT] = 192,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.PtAngularVelocity2D] = new() {
            [EfxVersion.RE3] = 152,
            [EfxVersion.DMC5] = 116,
            [EfxVersion.RE8] = 203,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.PtAngularVelocity2DExpression] = new() {
            [EfxVersion.RE3] = 153,
            [EfxVersion.DMC5] = 117,
            [EfxVersion.RE8] = 204,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.IgnorePlayerColor] = new() {
            [EfxVersion.RE3] = 154,
            [EfxVersion.RE2] = 114,
            [EfxVersion.DMC5] = 118,
            [EfxVersion.RE8] = 205,
            [EfxVersion.RERT] = 194,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.unknRE4Struct210] = new() {
            [EfxVersion.RE4] = 210,
        },
        [EfxAttributeType.unknRE4Struct213] = new() {
            [EfxVersion.RE4] = 213,
        },
        [EfxAttributeType.unknSBStruct195] = new() {
            [EfxVersion.MHRiseSB] = 195, // may be the same thing as RE4 213
            [EfxVersion.RERT] = 195,
        },

        [EfxAttributeType.ProceduralDistortionDelayFrame] = new() {
            [EfxVersion.RE3] = 155,
            [EfxVersion.RE8] = 206,
            [EfxVersion.RERT] = 198,
            [EfxVersion.RE4] = 216,
        },
        [EfxAttributeType.ProceduralDistortion] = new() {
            [EfxVersion.RE3] = 156,
            [EfxVersion.RE2] = 115,
            [EfxVersion.DMC5] = 119,
            [EfxVersion.RE8] = 207,
            [EfxVersion.RERT] = 199,
            [EfxVersion.RE4] = 217,
        },
        [EfxAttributeType.ProceduralDistortionClip] = new() {
            [EfxVersion.RE3] = 157,
            [EfxVersion.RE2] = 116,
            [EfxVersion.DMC5] = 120,
            [EfxVersion.RE8] = 208,
            [EfxVersion.RERT] = 200,
            [EfxVersion.RE4] = 218,
        },
        [EfxAttributeType.ProceduralDistortionExpression] = new() {
            [EfxVersion.RE3] = 158,
            [EfxVersion.RE8] = 209,
            [EfxVersion.RERT] = 201,
            [EfxVersion.RE4] = 219,
        },
        [EfxAttributeType.TestBehaviorUpdater] = new() {
            [EfxVersion.RE3] = 159,
            [EfxVersion.RE8] = 210,
            [EfxVersion.RE4] = 220, // unused
        },
        [EfxAttributeType.StretchBlur] = new() {
            [EfxVersion.RE8] = 211,
            [EfxVersion.RE4] = 221,
        },
        [EfxAttributeType.StretchBlurExpression] = new() {
            [EfxVersion.RE8] = 212,
            [EfxVersion.RE4] = 222, // unused
        },

        [EfxAttributeType.EmitterHSV] = new() {
            [EfxVersion.RE8] = 213,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.EmitterHSVExpression] = new() {
            [EfxVersion.RE8] = 214,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.FlowMap] = new() {
            [EfxVersion.RE8] = 215,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.RgbCommon] = new() {
            [EfxVersion.RE8] = 216,
            [EfxVersion.RERT] = 208,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.RgbWater] = new() {
            [EfxVersion.RE8] = 217,
            [EfxVersion.RERT] = 209,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.PtFreezer] = new() {
            [EfxVersion.RE8] = 218,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.AssignCSV] = new() {
            [EfxVersion.RE8] = 219,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.EmitMask] = new() {
            [EfxVersion.RE8] = 220,
            [EfxVersion.RERT] = 213,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeModularBillboard] = new() {
            [EfxVersion.RE8] = 221,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeModularRibbonFollow] = new() {
            [EfxVersion.RE8] = 222,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeModularRibbonLength] = new() {
            [EfxVersion.RE8] = 223,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeModularPolygon] = new() {
            [EfxVersion.RE8] = 224,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeModularMesh] = new() {
            [EfxVersion.RE8] = 225,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.ItemNum] = new() {
            [EfxVersion.RE3] = 160,
            [EfxVersion.RE2] = 117,
            [EfxVersion.DMC5] = 121,
            [EfxVersion.RE7] = 87,
            [EfxVersion.RE8] = 226,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.unknRE4Struct226] = new() {
            [EfxVersion.RE4] = 226,
        },
        [EfxAttributeType.unknRE4Struct228] = new() {
            [EfxVersion.RE4] = 228,
        },
        [EfxAttributeType.unknRE4Struct231] = new() {
            [EfxVersion.RE4] = 231,
        },
        [EfxAttributeType.unknRE4Struct243_UnknGPUBillboard] = new() {
            [EfxVersion.RE4] = 243,
        },
        [EfxAttributeType.unknRE4Struct244] = new() {
            [EfxVersion.RE4] = 244,
        },
        [EfxAttributeType.unknRE4Struct245_UnknType] = new() {
            [EfxVersion.RE4] = 245,
        },
        [EfxAttributeType.unknRE4Struct247_UnknTypeGPU] = new() {
            [EfxVersion.RE4] = 247,
        },
        [EfxAttributeType.unknRE4Struct248] = new() {
            [EfxVersion.RE4] = 248,
        },
        [EfxAttributeType.unknRE4Struct249_UnknTypeB] = new() {
            [EfxVersion.RE4] = 249,
        },
        // TODO try match previous entries
    };

}
