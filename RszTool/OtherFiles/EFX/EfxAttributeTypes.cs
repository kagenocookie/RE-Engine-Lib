namespace RszTool.Efx;

using System.Reflection;
using RszTool.InternalAttributes;

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
    ParentOptionsExpression,
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
    TypeRibbonLightweight,
    TypeRibbonParticle,

    TypeRibbonChainMaterial,
    TypeRibbonChainMaterialClip,
    TypeRibbonChainMaterialExpression,
    TypeRibbonFollowMaterial,
    TypeRibbonFollowMaterialClip,
    TypeRibbonFollowMaterialExpression,
    TypeRibbonLengthMaterial,
    TypeRibbonLengthMaterialClip,
    TypeRibbonLengthMaterialExpression,
    TypeRibbonFixEndMaterial,
    TypeRibbonFixEndMaterialClip,
    TypeRibbonFixEndMaterialExpression,
    TypeRibbonLightweightMaterial,
    TypeRibbonLightweightMaterialClip,
    TypeRibbonLightweightMaterialExpression,

    TypeStrainRibbonMaterial,
    TypeStrainRibbonMaterialClip,
    TypeStrainRibbonMaterialExpression,

    TypeRibbonFollowExpression,
    TypeRibbonLengthExpression,
    TypeRibbonChainExpression,
    TypeRibbonFixEndExpression,
    TypeRibbonLightweightExpression,
    TypeRibbonParticleExpression,

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
    TypePolygonTrailLike_RE470,
    TypeNoDraw,
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
    UVScroll,
    TextureUnit,
    TextureFilter,
    EmitterShape2D,
    EmitterShape2DExpression,
    EmitterShape3D,
    EmitterShape3DExpression,
    AlphaCorrection,
    Blink,
    TexelChannelOperator,
    TexelChannelOperatorClip,
    TexelChannelOperatorExpression,
    UnknownRE4_114TypeStrainRibbon,
    UnknownDD2_117TypeStrainRibbonExpression,
    ShaderSettings,
    Distortion,
    UnitCulling,
    UnitCullingExpression,
    PtLife,
    PtBehavior,
    FadeByAngle,
    FadeByDepth,
    FadeByDepthExpression,
    FadeByOcclusion,
    FadeByOcclusionExpression,
    FakeDoF,
    LuminanceBleed,
    PlayEmitter,
    PtTransform3D,
    PtTransform3DClip,
    PtTransform3DExpression,
    PtVelocity3D,
    PtVelocity3DClip,
    PtColliderAction,
    PtCollision,
    PtColor,
    PtColorClip,
    PtUvSequence,
    PtUvSequenceClip,
    VectorFieldParameter,
    VectorFieldParameterClip,
    VectorFieldParameterExpression,
    Attractor,
    AttractorClip,
    UnknownRE4_195,
    UnknownRE4_197,
    EmitterPriority,
    DrawOverlay,
    AngularVelocity2DDelayFrame,
    AngularVelocity2D,
    AngularVelocity3DDelayFrame,
    AngularVelocity3D,
    ProceduralDistortionDelayFrame,
    ProceduralDistortion,
    ProceduralDistortionClip,
    ProceduralDistortionExpression,
    StretchBlur,
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
    TypeLightning3D,
    TypeLightning3DExpression,
    TypeLightning3DMaterial,
    TypeLightning3DMaterialClip,
    TypeLightning3DMaterialExpression,

    TypeGpuBillboard,
    TypeGpuBillboardClip,
    TypeGpuBillboardExpression,
    TypeGpuLightning3D,
    TypeGpuMesh,
    TypeGpuMeshClip,
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

    TypeModularBillboard,
    TypeModularRibbonFollow,
    TypeModularRibbonLength,
    TypeModularPolygon,
    TypeModularMesh,

    TypeNoDrawExpression,
    TypeNodeBillboard,
    TypeNodeBillboardExpression,

    TypeStrainRibbon,
    TypeStrainRibbonExpression,

    VanishArea3DExpression,
    VectorField,
    VolumeField,
    VolumetricLighting,
    WindInfluence3D,
    WindInfluence3DDelayFrame,

    UnknownRERT_233,

    UnknownRE4_213,
    UnknownSB_195,

    UnknownRE4_226,
    UnknownRE4_228,
    UnknownRE4_231EfCsv,
    UnknownDD2_245Efcsv,

    UnknownDD2_146New,
    UnknownDD2_214,
    UnknownDD2_218,
    UnknownDD2_219,
    UnknownDD2_220,
    UnknownDD2_221Expression,
    UnknownDD2_223,
    UnknownDD2_224,
    UnknownDD2_226,
    UnknownDD2_225Expression,
    UnknownDD2_230,
    UnknownDD2_231Clip,
    UnknownDD2_232Expression,
    UnknownDD2_239RgbColor,
    UnknownDD2_239RgbColorExpression,
    UnknownDD2_243,
    UnknownDD2_247,
    UnknownDD2_249,
    UnknownDD2_250,
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

    public static bool HasAttributeType(this EfxVersion version, EfxAttributeType type)
    {
        return AttributeTypeIDs.TryGetValue(type, out var lookup) && lookup.TryGetValue(version, out var typeId) && typeId > 0;
    }

    public static IEnumerable<(EfxVersion version, int typeId)> GetVersionsOfType(this EfxAttributeType type)
    {
        foreach (var ver in GameOrder) {
            if (HasAttributeType(ver, type)) {
                yield return (ver, ToAttributeTypeID(ver, type));
            }
        }
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

    public static IEnumerable<Type> GetAllEfxAttributeInstanceTypes()
    {
        var list = new HashSet<Type>();

        foreach (var ver in (EfxVersion[])Enum.GetValues(typeof(EfxVersion)))
        {
            var ids = GetAllTypes(ver);
            foreach (var pair in ids) {
                var type = EfxAttributeTypeRemapper.GetAttributeInstanceType(pair.Value, ver);
                if (type != null) list.Add(type);
            }
        }

        return list;
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

        foreach (var pair in AttributeTypeIDs) {
            if (pair.Value.TryGetValue(version, out var id)) {
                if (id < 0) continue;

                readDict.Add(id, pair.Key);
                var typesList = typelist.GetValueOrDefault(pair.Key);
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
                typeDict[pair.Key] = instanceType;
            }
        }

        types[version] = readDict;
        typeFactories[version] = typeDict;
        return readDict;
    }

    private sealed class EfxLookup : Dictionary<EfxVersion, int>
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
            [EfxVersion.MHRise] = 1,
            [EfxVersion.RE8] = 1,
            [EfxVersion.RERT] = 1,
            [EfxVersion.MHRiseSB] = 1,
            [EfxVersion.SF6] = 1,
            [EfxVersion.RE4] = 1,
            [EfxVersion.DD2] = 1,
        },
        [EfxAttributeType.EffectOptimizeShader] = new() {
            [EfxVersion.RE8] = 2,
            [EfxVersion.MHRise] = 2,
            [EfxVersion.RERT] = 2,
            [EfxVersion.MHRiseSB] = 2,
            [EfxVersion.RE4] = 2,
            [EfxVersion.DD2] = 2, // OK
        },
        [EfxAttributeType.Transform2D] = new() {
            [EfxVersion.RE3] = 4,
            [EfxVersion.RE2] = 3,
            [EfxVersion.DMC5] = 3,
            [EfxVersion.RE7] = 1,
            [EfxVersion.MHRise] = 5,
            [EfxVersion.RE8] = 5,
            [EfxVersion.RERT] = 5,
            [EfxVersion.MHRiseSB] = 5,
            [EfxVersion.RE4] = 4,
            [EfxVersion.DD2] = 4, // guess
        },
        [EfxAttributeType.Transform2DModifierDelayFrame] = new() {
            [EfxVersion.RE3] = 5,
            [EfxVersion.MHRise] = 6,
            [EfxVersion.RE8] = 6,
            [EfxVersion.RERT] = 6,
            [EfxVersion.MHRiseSB] = 6,
            [EfxVersion.RE4] = 5,
        },
        [EfxAttributeType.Transform2DModifier] = new() {
            [EfxVersion.RE3] = 6,
            [EfxVersion.RE2] = 4,
            [EfxVersion.DMC5] = 4,
            [EfxVersion.MHRise] = 7,
            [EfxVersion.RE8] = 7,
            [EfxVersion.RERT] = 7,
            [EfxVersion.MHRiseSB] = 7,
            [EfxVersion.RE4] = 6, // guess
            [EfxVersion.DD2] = 6, // guess
        },
        [EfxAttributeType.Transform2DClip] = new() {
            [EfxVersion.RE3] = 7,
            [EfxVersion.RE2] = 5,
            [EfxVersion.DMC5] = 5,
            [EfxVersion.RE7] = 2,
            [EfxVersion.MHRise] = 8,
            [EfxVersion.RE8] = 8,
            [EfxVersion.RERT] = 8,
            [EfxVersion.MHRiseSB] = 8,
            [EfxVersion.RE4] = 7, // guess
            [EfxVersion.DD2] = 7, // guess
        },
        [EfxAttributeType.Transform2DExpression] = new() {
            [EfxVersion.RE3] = 8,
            [EfxVersion.RE2] = 6,
            [EfxVersion.DMC5] = 6,
            [EfxVersion.RE7] = 3,
            [EfxVersion.MHRise] = 9,
            [EfxVersion.RE8] = 9,
            [EfxVersion.RERT] = 9,
            [EfxVersion.MHRiseSB] = 9,
            [EfxVersion.RE4] = 8,
            [EfxVersion.DD2] = 8, // ok
        },
        [EfxAttributeType.Transform3D] = new() {
            [EfxVersion.RE3] = 9,
            [EfxVersion.RE2] = 7,
            [EfxVersion.DMC5] = 7,
            [EfxVersion.RE7] = 4,
            [EfxVersion.MHRise] = 10,
            [EfxVersion.RE8] = 10,
            [EfxVersion.RERT] = 10,
            [EfxVersion.MHRiseSB] = 10,
            [EfxVersion.RE4] = 9,
            [EfxVersion.DD2] = 9, // ok
        },
        [EfxAttributeType.Transform3DModifierDelayFrame] = new() {
            [EfxVersion.RE3] = 10,
            [EfxVersion.MHRise] = 11,
            [EfxVersion.RE8] = 11,
            [EfxVersion.RERT] = 11,
            [EfxVersion.MHRiseSB] = 11,
            [EfxVersion.RE4] = 10,
            [EfxVersion.DD2] = 10, // guess
        },
        [EfxAttributeType.Transform3DModifier] = new() {
            [EfxVersion.RE3] = 11,
            [EfxVersion.RE2] = 8,
            [EfxVersion.DMC5] = 8,
            [EfxVersion.MHRise] = 12,
            [EfxVersion.RE8] = 12,
            [EfxVersion.RERT] = 12,
            [EfxVersion.MHRiseSB] = 12,
            [EfxVersion.RE4] = 11,
            [EfxVersion.DD2] = 11,
        },
        [EfxAttributeType.Transform3DClip] = new() {
            [EfxVersion.RE3] = 12,
            [EfxVersion.RE2] = 9,
            [EfxVersion.DMC5] = 9,
            [EfxVersion.RE7] = 5,
            [EfxVersion.MHRise] = 13,
            [EfxVersion.RE8] = 13,
            [EfxVersion.RERT] = 13,
            [EfxVersion.MHRiseSB] = 13,
            [EfxVersion.RE4] = 12,
            [EfxVersion.DD2] = 12, // guess
        },
        [EfxAttributeType.Transform3DExpression] = new() {
            [EfxVersion.RE3] = 13,
            [EfxVersion.RE2] = 10,
            [EfxVersion.DMC5] = 10,
            [EfxVersion.RE7] = 6,
            [EfxVersion.MHRise] = 14,
            [EfxVersion.RE8] = 14,
            [EfxVersion.RERT] = 14,
            [EfxVersion.MHRiseSB] = 14,
            [EfxVersion.RE4] = 13,
            [EfxVersion.DD2] = 13, // guess
        },
        [EfxAttributeType.ParentOptions] = new() {
            [EfxVersion.RE3] = 14,
            [EfxVersion.RE2] = 11,
            [EfxVersion.DMC5] = 11,
            [EfxVersion.RE7] = 7,
            [EfxVersion.MHRise] = 15,
            [EfxVersion.RE8] = 15,
            [EfxVersion.RERT] = 15,
            [EfxVersion.MHRiseSB] = 15,
            [EfxVersion.RE4] = 14,
            [EfxVersion.DD2] = 14, // ok
        },
        [EfxAttributeType.ParentOptionsExpression] = new() {
            [EfxVersion.DD2] = 15,
        },
        [EfxAttributeType.Spawn] = new() {
            [EfxVersion.RE3] = 2,
            [EfxVersion.RE2] = 1,
            [EfxVersion.DMC5] = 1,
            [EfxVersion.RE7] = 9,
            [EfxVersion.MHRise] = 3,
            [EfxVersion.RE8] = 3,
            [EfxVersion.RERT] = 3,
            [EfxVersion.MHRiseSB] = 3,
            [EfxVersion.RE4] = 16,
            [EfxVersion.DD2] = 16, // mostly ok
        },
        [EfxAttributeType.SpawnExpression] = new() {
            [EfxVersion.RE3] = 3,
            [EfxVersion.RE2] = 2,
            [EfxVersion.DMC5] = 2,
            [EfxVersion.RE7] = 10,
            [EfxVersion.MHRise] = 4,
            [EfxVersion.RE8] = 4,
            [EfxVersion.RERT] = 4,
            [EfxVersion.MHRiseSB] = 4,
            [EfxVersion.RE4] = 17,
            [EfxVersion.DD2] = 17, // ok
        },
        [EfxAttributeType.EmitterColor] = new() {
            [EfxVersion.RE3] = 15,
            [EfxVersion.MHRise] = 16,
            [EfxVersion.RE8] = 16,
            [EfxVersion.RERT] = 16,
            [EfxVersion.MHRiseSB] = 16,
            [EfxVersion.RE4] = 18,
            [EfxVersion.DD2] = 18,
        },
        [EfxAttributeType.EmitterColorClip] = new() {
            [EfxVersion.RE3] = 16,
            [EfxVersion.MHRise] = 17,
            [EfxVersion.RE8] = 17,
            [EfxVersion.RERT] = 17,
            [EfxVersion.MHRiseSB] = 17,
            [EfxVersion.RE4] = 19,
            [EfxVersion.DD2] = 19,
        },
        // DD2 probably added a EmitterColorExpression here, unused
        [EfxAttributeType.PtSort] = new() {
            [EfxVersion.MHRise] = 18,
            [EfxVersion.RE8] = 18,
            [EfxVersion.RERT] = 18,
            [EfxVersion.MHRiseSB] = 18,
            [EfxVersion.RE4] = 20,
            [EfxVersion.DD2] = 21, // ok
        },
        [EfxAttributeType.TypeBillboard2D] = new() {
            [EfxVersion.RE3] = 17,
            [EfxVersion.RE2] = 13,
            [EfxVersion.DMC5] = 13,
            [EfxVersion.RE7] = 11,
            [EfxVersion.MHRise] = 19,
            [EfxVersion.RE8] = 19,
            [EfxVersion.RERT] = 19,
            [EfxVersion.MHRiseSB] = 19,
            [EfxVersion.RE4] = 21,
            [EfxVersion.DD2] = 22, // ok
        },
        [EfxAttributeType.TypeBillboard2DExpression] = new() {
            [EfxVersion.RE3] = 18,
            [EfxVersion.RE2] = 14,
            [EfxVersion.DMC5] = 14,
            [EfxVersion.RE7] = 12,
            [EfxVersion.MHRise] = 20,
            [EfxVersion.RE8] = 20,
            [EfxVersion.RERT] = 20,
            [EfxVersion.MHRiseSB] = 20,
            [EfxVersion.RE4] = 22,
            [EfxVersion.DD2] = 23, // ok
        },
        [EfxAttributeType.TypeBillboard3D] = new() {
            [EfxVersion.RE3] = 19,
            [EfxVersion.RE2] = 15,
            [EfxVersion.DMC5] = 15,
            [EfxVersion.RE7] = 13,
            [EfxVersion.MHRise] = 21,
            [EfxVersion.RE8] = 21,
            [EfxVersion.RERT] = 21,
            [EfxVersion.MHRiseSB] = 21,
            [EfxVersion.RE4] = 23,
            [EfxVersion.DD2] = 24,
        },
        [EfxAttributeType.TypeBillboard3DExpression] = new() {
            [EfxVersion.RE3] = 20,
            [EfxVersion.RE2] = 16,
            [EfxVersion.DMC5] = 16,
            [EfxVersion.RE7] = 14,
            [EfxVersion.MHRise] = 22,
            [EfxVersion.RE8] = 22,
            [EfxVersion.RERT] = 22,
            [EfxVersion.MHRiseSB] = 22,
            [EfxVersion.RE4] = 24,
            [EfxVersion.DD2] = 25,
        },
        [EfxAttributeType.TypeBillboard3DMaterial] = new() {
            [EfxVersion.RE3] = 21,
            [EfxVersion.MHRise] = 23,
            [EfxVersion.RE8] = 23,
            [EfxVersion.RERT] = 23,
            [EfxVersion.MHRiseSB] = 23,
            [EfxVersion.RE4] = 25,
            [EfxVersion.DD2] = 26,
        },
        [EfxAttributeType.TypeBillboard3DMaterialClip] = new() {
            [EfxVersion.RE3] = 22,
            [EfxVersion.MHRise] = 24,
            [EfxVersion.RE8] = 24,
            [EfxVersion.RERT] = 24,
            [EfxVersion.MHRiseSB] = 24,
            [EfxVersion.RE4] = 26,
            [EfxVersion.DD2] = 27,
        },
        [EfxAttributeType.TypeBillboard3DMaterialExpression] = new() {
            [EfxVersion.RE3] = 23,
            [EfxVersion.MHRise] = 25,
            [EfxVersion.RE8] = 25,
            [EfxVersion.RERT] = 25,
            [EfxVersion.MHRiseSB] = 25,
            [EfxVersion.RE4] = 27,
            [EfxVersion.DD2] = 28,
        },
        [EfxAttributeType.TypeMesh] = new() {
            [EfxVersion.RE3] = 24,
            [EfxVersion.RE2] = 17,
            [EfxVersion.DMC5] = 17,
            [EfxVersion.RE7] = 15,
            [EfxVersion.MHRise] = 26,
            [EfxVersion.RE8] = 26,
            [EfxVersion.RERT] = 26,
            [EfxVersion.MHRiseSB] = 26,
            [EfxVersion.RE4] = 28,
            [EfxVersion.DD2] = 29,
        },
        [EfxAttributeType.TypeMeshClip] = new() {
            [EfxVersion.RE3] = 25,
            [EfxVersion.RE2] = 18,
            [EfxVersion.DMC5] = 18,
            [EfxVersion.RE7] = 16,
            [EfxVersion.MHRise] = 27,
            [EfxVersion.RE8] = 27,
            [EfxVersion.RERT] = 27,
            [EfxVersion.MHRiseSB] = 27,
            [EfxVersion.RE4] = 29,
            [EfxVersion.DD2] = 30,
        },
        [EfxAttributeType.TypeMeshExpression] = new() {
            [EfxVersion.RE3] = 26,
            [EfxVersion.RE2] = 19,
            [EfxVersion.DMC5] = 19,
            [EfxVersion.RE7] = 17,
            [EfxVersion.MHRise] = 28,
            [EfxVersion.RE8] = 28,
            [EfxVersion.RERT] = 28,
            [EfxVersion.MHRiseSB] = 28,
            [EfxVersion.RE4] = 30,
            [EfxVersion.DD2] = 31,
        },

        [EfxAttributeType.TypeRibbonFollow] = new() {
            [EfxVersion.RE3] = 27,
            [EfxVersion.RE2] = 20,
            [EfxVersion.DMC5] = 20,
            [EfxVersion.RE7] = 18,
            [EfxVersion.MHRise] = 29,
            [EfxVersion.RE8] = 29,
            [EfxVersion.RERT] = 29,
            [EfxVersion.MHRiseSB] = 29,
            [EfxVersion.RE4] = 31,
            [EfxVersion.DD2] = 32, // guess
        },
        [EfxAttributeType.TypeRibbonLength] = new() {
            [EfxVersion.RE3] = 28,
            [EfxVersion.RE2] = 21,
            [EfxVersion.DMC5] = 21,
            [EfxVersion.RE7] = 19,
            [EfxVersion.MHRise] = 30,
            [EfxVersion.RE8] = 30,
            [EfxVersion.RERT] = 30,
            [EfxVersion.MHRiseSB] = 30,
            [EfxVersion.RE4] = 32,
            [EfxVersion.DD2] = 33,
        },
        [EfxAttributeType.TypeRibbonChain] = new() {
            [EfxVersion.RE3] = 29,
            [EfxVersion.RE2] = 22,
            [EfxVersion.DMC5] = 22,
            [EfxVersion.RE7] = 20,
            [EfxVersion.MHRise] = 31,
            [EfxVersion.RE8] = 31,
            [EfxVersion.RERT] = 31,
            [EfxVersion.MHRiseSB] = 31,
            [EfxVersion.RE4] = 33,
            [EfxVersion.DD2] = 34, // seems ok
        },
        [EfxAttributeType.TypeRibbonFixEnd] = new() {
            [EfxVersion.RE3] = 30,
            [EfxVersion.RE2] = 23,
            [EfxVersion.DMC5] = 23,
            [EfxVersion.MHRise] = 32,
            [EfxVersion.RE8] = 32,
            [EfxVersion.RERT] = 32,
            [EfxVersion.MHRiseSB] = 32,
            [EfxVersion.RE4] = 34, // ok
            [EfxVersion.DD2] = 35, // guess
        },
        [EfxAttributeType.TypeRibbonLightweight] = new() {
            [EfxVersion.RE3] = 31,
            [EfxVersion.RE2] = 24,
            [EfxVersion.DMC5] = 24,
            [EfxVersion.MHRise] = 33,
            [EfxVersion.RE8] = 33,
            [EfxVersion.RERT] = 33,
            [EfxVersion.MHRiseSB] = 33,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 36, // guess
        },
        [EfxAttributeType.TypeRibbonParticle] = new() {
            [EfxVersion.MHRise] = 34,
            [EfxVersion.RE8] = 34,
            [EfxVersion.RERT] = 34,
            [EfxVersion.MHRiseSB] = 34,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 37, // guess
        },

        [EfxAttributeType.TypeRibbonFollowMaterial] = new() {
            [EfxVersion.MHRise] = 35,
            [EfxVersion.RE8] = 35,
            [EfxVersion.RERT] = 35,
            [EfxVersion.MHRiseSB] = 35,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 38, // guess
        },
        [EfxAttributeType.TypeRibbonFollowMaterialClip] = new() {
            [EfxVersion.MHRise] = 36,
            [EfxVersion.RE8] = 36,
            [EfxVersion.RERT] = 36,
            [EfxVersion.MHRiseSB] = 36,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 39, // guess
        },
        [EfxAttributeType.TypeRibbonFollowMaterialExpression] = new() {
            [EfxVersion.MHRise] = 37,
            [EfxVersion.RE8] = 37,
            [EfxVersion.RERT] = 37,
            [EfxVersion.MHRiseSB] = 37,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 40, // guess
        },
        [EfxAttributeType.TypeRibbonLengthMaterial] = new() {
            [EfxVersion.MHRise] = 38,
            [EfxVersion.RE8] = 38,
            [EfxVersion.RERT] = 38,
            [EfxVersion.MHRiseSB] = 38,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 41, // guess
        },
        [EfxAttributeType.TypeRibbonLengthMaterialClip] = new() {
            [EfxVersion.MHRise] = 39,
            [EfxVersion.RE8] = 39,
            [EfxVersion.RERT] = 39,
            [EfxVersion.MHRiseSB] = 39,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 42, // guess
        },
        [EfxAttributeType.TypeRibbonLengthMaterialExpression] = new() {
            [EfxVersion.MHRise] = 40,
            [EfxVersion.RE8] = 40,
            [EfxVersion.RERT] = 40,
            [EfxVersion.MHRiseSB] = 40,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 43, // guess
        },
        [EfxAttributeType.TypeRibbonChainMaterial] = new() {
            [EfxVersion.MHRise] = 41,
            [EfxVersion.RE8] = 41,
            [EfxVersion.RERT] = 41,
            [EfxVersion.MHRiseSB] = 41,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 44, // guess
        },
        [EfxAttributeType.TypeRibbonChainMaterialClip] = new() {
            [EfxVersion.MHRise] = 42,
            [EfxVersion.RE8] = 42,
            [EfxVersion.RERT] = 42,
            [EfxVersion.MHRiseSB] = 42,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 45, // guess
        },
        [EfxAttributeType.TypeRibbonChainMaterialExpression] = new() {
            [EfxVersion.MHRise] = 43,
            [EfxVersion.RE8] = 43,
            [EfxVersion.RERT] = 43,
            [EfxVersion.MHRiseSB] = 43,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 46, // guess
        },
        [EfxAttributeType.TypeRibbonFixEndMaterial] = new() {
            [EfxVersion.MHRise] = 44,
            [EfxVersion.RE8] = 44,
            [EfxVersion.RERT] = 44,
            [EfxVersion.MHRiseSB] = 44,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 47, // guess
        },
        [EfxAttributeType.TypeRibbonFixEndMaterialClip] = new() {
            [EfxVersion.MHRise] = 45,
            [EfxVersion.RE8] = 45,
            [EfxVersion.RERT] = 45,
            [EfxVersion.MHRiseSB] = 45,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 48, // guess
        },
        [EfxAttributeType.TypeRibbonFixEndMaterialExpression] = new() {
            [EfxVersion.MHRise] = 46,
            [EfxVersion.RE8] = 46,
            [EfxVersion.RERT] = 46,
            [EfxVersion.MHRiseSB] = 46,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 49, // guess
        },
        [EfxAttributeType.TypeRibbonLightweightMaterial] = new() {
            [EfxVersion.MHRise] = 47,
            [EfxVersion.RE8] = 47,
            [EfxVersion.RERT] = 47,
            [EfxVersion.MHRiseSB] = 47,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 50, // guess
        },
        [EfxAttributeType.TypeRibbonLightweightMaterialClip] = new() {
            [EfxVersion.MHRise] = 48,
            [EfxVersion.RE8] = 48,
            [EfxVersion.RERT] = 48,
            [EfxVersion.MHRiseSB] = 48,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 51, // guess
        },
        [EfxAttributeType.TypeRibbonLightweightMaterialExpression] = new() {
            [EfxVersion.MHRise] = 49,
            [EfxVersion.RE8] = 49,
            [EfxVersion.RERT] = 49,
            [EfxVersion.MHRiseSB] = 49,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 52, // guess
        },
        [EfxAttributeType.TypeStrainRibbonMaterial] = new() {
            [EfxVersion.MHRise] = 50,
            [EfxVersion.RE8] = 50,
            [EfxVersion.RERT] = 50,
            [EfxVersion.MHRiseSB] = 50,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 53,
        },
        [EfxAttributeType.TypeStrainRibbonMaterialClip] = new() {
            [EfxVersion.MHRise] = 51,
            [EfxVersion.RE8] = 51,
            [EfxVersion.RERT] = 51,
            [EfxVersion.MHRiseSB] = 51,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 54, // guess
        },
        [EfxAttributeType.TypeStrainRibbonMaterialExpression] = new() {
            [EfxVersion.MHRise] = 52,
            [EfxVersion.RE8] = 52,
            [EfxVersion.RERT] = 52,
            [EfxVersion.MHRiseSB] = 52,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 55,
        },
        [EfxAttributeType.TypeRibbonFollowExpression] = new() {
            [EfxVersion.RE3] = 32,
            [EfxVersion.RE2] = 25,
            [EfxVersion.DMC5] = 25,
            [EfxVersion.RE7] = 21,
            [EfxVersion.MHRise] = 53,
            [EfxVersion.RE8] = 53,
            [EfxVersion.RERT] = 53,
            [EfxVersion.MHRiseSB] = 53,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 59,
        },
        [EfxAttributeType.TypeRibbonLengthExpression] = new() {
            [EfxVersion.RE3] = 33,
            [EfxVersion.RE2] = 26,
            [EfxVersion.DMC5] = 26,
            [EfxVersion.RE7] = 22,
            [EfxVersion.MHRise] = 54,
            [EfxVersion.RE8] = 54,
            [EfxVersion.RERT] = 54,
            [EfxVersion.MHRiseSB] = 54,
            [EfxVersion.RE4] = 58,
            [EfxVersion.DD2] = 60, // guess
        },
        [EfxAttributeType.TypeRibbonChainExpression] = new() {
            [EfxVersion.RE3] = 34,
            [EfxVersion.RE2] = 27,
            [EfxVersion.DMC5] = 27,
            [EfxVersion.RE7] = 23,
            [EfxVersion.MHRise] = 55,
            [EfxVersion.RE8] = 55,
            [EfxVersion.RERT] = 55,
            [EfxVersion.MHRiseSB] = 55,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 61, // guess
        },
        [EfxAttributeType.TypeRibbonFixEndExpression] = new() {
            [EfxVersion.RE3] = 35,
            [EfxVersion.RE2] = 28,
            [EfxVersion.DMC5] = 28,
            [EfxVersion.MHRise] = 56,
            [EfxVersion.RE8] = 56,
            [EfxVersion.RERT] = 56,
            [EfxVersion.MHRiseSB] = 56,
            [EfxVersion.RE4] = 60,
            [EfxVersion.DD2] = 62, // guess
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
            [EfxVersion.DD2] = 64,
        },

        [EfxAttributeType.TypePolygon] = new() {
            [EfxVersion.RE3] = 36,
            [EfxVersion.RE2] = 29,
            [EfxVersion.DMC5] = 29,
            [EfxVersion.RE7] = 24,
            [EfxVersion.MHRise] = 57,
            [EfxVersion.RE8] = 59,
            [EfxVersion.RERT] = 59,
            [EfxVersion.MHRiseSB] = 59,
            [EfxVersion.RE4] = 63,
            [EfxVersion.DD2] = 65,
        },
        [EfxAttributeType.TypePolygonClip] = new() {
            [EfxVersion.RE3] = 37,
            [EfxVersion.RE2] = 30,
            [EfxVersion.DMC5] = 30,
            [EfxVersion.RE7] = 25,
            [EfxVersion.MHRise] = 58,
            [EfxVersion.RE8] = 60,
            [EfxVersion.RERT] = 60,
            [EfxVersion.MHRiseSB] = 60,
            [EfxVersion.RE4] = 64,
            [EfxVersion.DD2] = 66, // guess
        },
        [EfxAttributeType.TypePolygonExpression] = new() {
            [EfxVersion.RE3] = 38,
            [EfxVersion.RE2] = 31,
            [EfxVersion.DMC5] = 31,
            [EfxVersion.RE7] = 26,
            [EfxVersion.MHRise] = 59,
            [EfxVersion.RE8] = 61,
            [EfxVersion.RERT] = 61, // ok
            [EfxVersion.MHRiseSB] = 61,
            [EfxVersion.RE4] = 65, // ok
            [EfxVersion.DD2] = 67, // ok
        },
        [EfxAttributeType.TypePolygonMaterial] = new() {
            [EfxVersion.MHRise] = 60,
            [EfxVersion.RE8] = 62,
            [EfxVersion.RERT] = 62, // guess
            [EfxVersion.MHRiseSB] = 62,
            [EfxVersion.RE4] = UNKNOWN, // 66
            [EfxVersion.DD2] = 68, // ok
            // [EfxVersion.DD2] = 74, // guess
        },
        [EfxAttributeType.TypePolygonMaterialExpression] = new() {
            [EfxVersion.RE8] = 63,
            [EfxVersion.RERT] = 63, // guess
            [EfxVersion.RE4] = UNKNOWN, // 67
            // [EfxVersion.DD2] = 69, // guess
        },
        [EfxAttributeType.TypeRibbonTrail] = new() {
            [EfxVersion.RE3] = 39,
            [EfxVersion.RE2] = 32,
            [EfxVersion.DMC5] = 32,
            [EfxVersion.RE7] = 27,
            [EfxVersion.RE8] = 64,
            [EfxVersion.RERT] = 65, // testing
            [EfxVersion.MHRise] = 61,
            [EfxVersion.RE4] = 69, // ok
            // [EfxVersion.DD2] = 70, // guess
        },

        [EfxAttributeType.TypePolygonTrail] = new() {
            [EfxVersion.RE3] = 40,
            [EfxVersion.RE2] = 33, // ok
            [EfxVersion.DMC5] = 33,
            [EfxVersion.RE7] = 28,
            [EfxVersion.MHRise] = 62,
            [EfxVersion.RE8] = 65,
            [EfxVersion.RERT] = 66, // ok
            [EfxVersion.MHRiseSB] = 66,
            [EfxVersion.RE4] = 70,
            // [EfxVersion.RE4] = UNKNOWN, // 71?
            [EfxVersion.DD2] = 72, // ok
        },
        [EfxAttributeType.TypePolygonTrailExpression] = new() {
            [EfxVersion.RE8] = 66,
            [EfxVersion.RE4] = 71,
            [EfxVersion.DD2] = 73, // probably
        },
        [EfxAttributeType.TypePolygonTrailMaterial] = new() {
            [EfxVersion.MHRise] = 63,
            [EfxVersion.RE8] = 67,
            [EfxVersion.RERT] = 67, // guess
            [EfxVersion.MHRiseSB] = 67,
            [EfxVersion.RE4] = 73, // guess
            [EfxVersion.DD2] = 74,
        },
        [EfxAttributeType.TypePolygonTrailMaterialExpression] = new() {
            [EfxVersion.RE8] = 68,
            [EfxVersion.RE4] = 74, // guess
            [EfxVersion.DD2] = 76,
        },

        [EfxAttributeType.TypeNoDraw] = new() {
            [EfxVersion.RE3] = 41,
            [EfxVersion.RE2] = 34,
            [EfxVersion.DMC5] = 34,
            [EfxVersion.RE7] = 29,
            [EfxVersion.MHRise] = 64,
            [EfxVersion.RE8] = 69,
            [EfxVersion.RERT] = 70,
            [EfxVersion.MHRiseSB] = 70,
            [EfxVersion.RE4] = 75, // ok
            [EfxVersion.DD2] = 77, // ok
        },
        [EfxAttributeType.TypeNoDrawExpression] = new() {
            [EfxVersion.RE3] = 42,
            [EfxVersion.RE2] = 35,
            [EfxVersion.DMC5] = 35,
            [EfxVersion.MHRise] = 65,
            [EfxVersion.RE8] = 70,
            [EfxVersion.RERT] = 71,
            [EfxVersion.MHRiseSB] = 71,
            [EfxVersion.RE4] = 76,
            [EfxVersion.DD2] = 78, // ok
        },
        [EfxAttributeType.Velocity2DDelayFrame] = new() {
            [EfxVersion.RE3] = 43,
            [EfxVersion.MHRise] = 66,
            [EfxVersion.RE8] = 71,
            [EfxVersion.RERT] = 72,
            [EfxVersion.MHRiseSB] = 72,
            [EfxVersion.RE4] = 77,
        },
        [EfxAttributeType.Velocity2D] = new() {
            [EfxVersion.RE3] = 44,
            [EfxVersion.RE2] = 36,
            [EfxVersion.DMC5] = 36,
            [EfxVersion.RE7] = 30,
            [EfxVersion.MHRise] = 67,
            [EfxVersion.RE8] = 72,
            [EfxVersion.RERT] = 73,
            [EfxVersion.MHRiseSB] = 73,
            [EfxVersion.RE4] = 78,
            [EfxVersion.DD2] = 80, // ok
        },
        [EfxAttributeType.Velocity2DExpression] = new() {
            [EfxVersion.RE3] = 45,
            [EfxVersion.RE2] = 37,
            [EfxVersion.DMC5] = 37,
            [EfxVersion.RE7] = 31,
            [EfxVersion.MHRise] = 68,
            [EfxVersion.RE8] = 73,
            [EfxVersion.RERT] = 74,
            [EfxVersion.MHRiseSB] = 74,
            [EfxVersion.RE4] = 79,
            [EfxVersion.DD2] = 81, // ok
        },
        [EfxAttributeType.Velocity3DDelayFrame] = new() {
            [EfxVersion.RE3] = 46,
            [EfxVersion.MHRise] = 69,
            [EfxVersion.RE8] = 74,
            [EfxVersion.RERT] = 75,
            [EfxVersion.MHRiseSB] = 75,
            [EfxVersion.RE4] = 80,
            [EfxVersion.DD2] = 82, // guess
        },
        [EfxAttributeType.Velocity3D] = new() {
            [EfxVersion.RE3] = 47,
            [EfxVersion.RE2] = 38,
            [EfxVersion.DMC5] = 38,
            [EfxVersion.RE7] = 32,
            [EfxVersion.MHRise] = 70,
            [EfxVersion.RE8] = 75,
            [EfxVersion.RERT] = 76,
            [EfxVersion.MHRiseSB] = 76,
            [EfxVersion.RE4] = 81,
            [EfxVersion.DD2] = 83,
        },
        [EfxAttributeType.Velocity3DExpression] = new() {
            [EfxVersion.RE3] = 48,
            [EfxVersion.RE2] = 39,
            [EfxVersion.DMC5] = 39,
            [EfxVersion.RE7] = 33,
            [EfxVersion.MHRise] = 71,
            [EfxVersion.RE8] = 76,
            [EfxVersion.RERT] = 77,
            [EfxVersion.MHRiseSB] = 77,
            [EfxVersion.RE4] = 82,
            [EfxVersion.DD2] = 84, // guess
        },
        [EfxAttributeType.RotateAnimDelayFrame] = new() {
            [EfxVersion.RE3] = 49,
            [EfxVersion.MHRise] = 72,
            [EfxVersion.RE8] = 77,
            [EfxVersion.RERT] = 78,
            [EfxVersion.MHRiseSB] = 78,
            [EfxVersion.RE4] = 83,
            [EfxVersion.DD2] = 85, // guess
        },
        [EfxAttributeType.RotateAnim] = new() {
            [EfxVersion.RE3] = 50,
            [EfxVersion.RE2] = 40,
            [EfxVersion.DMC5] = 40,
            [EfxVersion.RE7] = 34,
            [EfxVersion.MHRise] = 73,
            [EfxVersion.RE8] = 78,
            [EfxVersion.RERT] = 79,
            [EfxVersion.MHRiseSB] = 79,
            [EfxVersion.RE4] = 84,
            [EfxVersion.DD2] = 86,
        },
        [EfxAttributeType.RotateAnimExpression] = new() {
            [EfxVersion.RE3] = 51,
            [EfxVersion.RE2] = 41,
            [EfxVersion.DMC5] = 41,
            [EfxVersion.RE7] = 35,
            [EfxVersion.MHRise] = 74,
            [EfxVersion.RE8] = 79,
            [EfxVersion.RERT] = 80,
            [EfxVersion.MHRiseSB] = 80,
            [EfxVersion.RE4] = 85,
            [EfxVersion.DD2] = 87, // probably
        },
        [EfxAttributeType.ScaleAnimDelayFrame] = new() {
            [EfxVersion.RE3] = 52,
            [EfxVersion.MHRise] = 75,
            [EfxVersion.RE8] = 80,
            [EfxVersion.RERT] = 81,
            [EfxVersion.MHRiseSB] = 81,
            [EfxVersion.RE4] = 86,
            [EfxVersion.DD2] = 88, // ok
        },
        [EfxAttributeType.ScaleAnim] = new() {
            [EfxVersion.RE3] = 53,
            [EfxVersion.RE2] = 42,
            [EfxVersion.DMC5] = 42,
            [EfxVersion.RE7] = 36,
            [EfxVersion.MHRise] = 76,
            [EfxVersion.RE8] = 81,
            [EfxVersion.RERT] = 82,
            [EfxVersion.MHRiseSB] = 82,
            [EfxVersion.RE4] = 87,
            [EfxVersion.DD2] = 89,
        },
        [EfxAttributeType.ScaleAnimExpression] = new() {
            [EfxVersion.RE3] = 54,
            [EfxVersion.RE2] = 43,
            [EfxVersion.DMC5] = 43,
            [EfxVersion.RE7] = 37,
            [EfxVersion.MHRise] = 77,
            [EfxVersion.RE8] = 82,
            [EfxVersion.RERT] = 83,
            [EfxVersion.MHRiseSB] = 83,
            [EfxVersion.RE4] = 88, // unused
            [EfxVersion.DD2] = 90,
        },
        [EfxAttributeType.VanishArea3D] = new() {
            [EfxVersion.RE3] = 55,
            [EfxVersion.MHRise] = 78,
            [EfxVersion.RE8] = 83,
            [EfxVersion.RERT] = 84,
            [EfxVersion.MHRiseSB] = 84,
            [EfxVersion.RE4] = 89,
            [EfxVersion.DD2] = 91,
        },
        [EfxAttributeType.VanishArea3DExpression] = new() {
            [EfxVersion.RE3] = 56,
            [EfxVersion.MHRise] = 79,
            [EfxVersion.RE8] = 84,
            [EfxVersion.RERT] = 85,
            [EfxVersion.MHRiseSB] = 85,
            [EfxVersion.RE4] = 90, // unused
            [EfxVersion.DD2] = 92,
        },
        [EfxAttributeType.Life] = new() {
            [EfxVersion.RE3] = 57,
            [EfxVersion.RE2] = 44,
            [EfxVersion.DMC5] = 44,
            [EfxVersion.RE7] = 38,
            [EfxVersion.MHRise] = 80, // TODO handle mhr enums past here
            [EfxVersion.RE8] = 85,
            [EfxVersion.RERT] = 86,
            [EfxVersion.MHRiseSB] = 86,
            [EfxVersion.RE4] = 91,
            [EfxVersion.DD2] = 93, // ok
        },
        [EfxAttributeType.LifeExpression] = new() {
            [EfxVersion.RE3] = 58,
            [EfxVersion.RE2] = 45,
            [EfxVersion.DMC5] = 45,
            [EfxVersion.RE7] = 39,
            [EfxVersion.MHRise] = 81,
            [EfxVersion.RE8] = 86,
            [EfxVersion.RERT] = 87,
            [EfxVersion.MHRiseSB] = 87,
            [EfxVersion.RE4] = 92,
            [EfxVersion.DD2] = 94, // ok
        },
        [EfxAttributeType.UVSequence] = new() {
            [EfxVersion.RE3] = 59,
            [EfxVersion.RE2] = 46,
            [EfxVersion.DMC5] = 46,
            [EfxVersion.RE7] = 40,
            [EfxVersion.MHRise] = 82,
            [EfxVersion.RE8] = 87,
            [EfxVersion.RERT] = 88,
            [EfxVersion.MHRiseSB] = 88,
            [EfxVersion.RE4] = 93,
            [EfxVersion.DD2] = 95, // ok
        },
        [EfxAttributeType.UVSequenceModifier] = new() {
            [EfxVersion.RE3] = 60,
            [EfxVersion.MHRise] = 83,
            [EfxVersion.RE8] = 88,
            [EfxVersion.RERT] = 89,
            [EfxVersion.MHRiseSB] = 89,
            [EfxVersion.RE4] = 94,
            [EfxVersion.DD2] = 96, // ok
        },
        [EfxAttributeType.UVSequenceExpression] = new() {
            [EfxVersion.RE3] = 61,
            [EfxVersion.RE2] = 47,
            [EfxVersion.DMC5] = 47,
            [EfxVersion.RE7] = 41,
            [EfxVersion.MHRise] = 84,
            [EfxVersion.RE8] = 89,
            [EfxVersion.RERT] = 90,
            [EfxVersion.MHRiseSB] = 90,
            [EfxVersion.RE4] = 95,
            [EfxVersion.DD2] = 97, // guess
        },
        [EfxAttributeType.UVScroll] = new() {
            [EfxVersion.RE3] = 62,
            [EfxVersion.RE2] = 48,
            [EfxVersion.DMC5] = 48,
            [EfxVersion.MHRise] = 85,
            [EfxVersion.RE8] = 90,
            [EfxVersion.RERT] = 91,
            [EfxVersion.MHRiseSB] = 91,
            [EfxVersion.RE4] = 96,
            [EfxVersion.DD2] = 98, // ok
        },
        [EfxAttributeType.TextureUnit] = new() {
            [EfxVersion.RE3] = 63,
            [EfxVersion.RE2] = 49,
            [EfxVersion.DMC5] = 49,
            [EfxVersion.MHRise] = 86,
            [EfxVersion.RE8] = 91,
            [EfxVersion.RERT] = 92,
            [EfxVersion.MHRiseSB] = 92,
            [EfxVersion.RE4] = 97,
            [EfxVersion.DD2] = 99, // ok
        },
        [EfxAttributeType.TextureUnitExpression] = new() {
            [EfxVersion.RE3] = 64,
            [EfxVersion.MHRise] = 87,
            [EfxVersion.RE8] = 92,
            [EfxVersion.RERT] = 93,
            [EfxVersion.MHRiseSB] = 93,
            [EfxVersion.RE4] = 98, // unused
            [EfxVersion.DD2] = 100,
        },
        [EfxAttributeType.TextureFilter] = new() {
            [EfxVersion.RE3] = 65,
            [EfxVersion.MHRise] = 88,
            [EfxVersion.RE8] = 93,
            [EfxVersion.RERT] = 94,
            [EfxVersion.MHRiseSB] = 94,
            [EfxVersion.RE4] = 99,
            [EfxVersion.DD2] = 101, // guess
        },
        [EfxAttributeType.EmitterShape2D] = new() {
            [EfxVersion.RE3] = 66,
            [EfxVersion.RE2] = 50,
            [EfxVersion.DMC5] = 50,
            [EfxVersion.RE7] = 42,
            [EfxVersion.MHRise] = 89,
            [EfxVersion.RE8] = 94,
            [EfxVersion.RERT] = 95,
            [EfxVersion.MHRiseSB] = 95,
            [EfxVersion.RE4] = 100,
            [EfxVersion.DD2] = 102, // ok
        },
        [EfxAttributeType.EmitterShape2DExpression] = new() {
            [EfxVersion.RE3] = 67,
            [EfxVersion.RE2] = 51,
            [EfxVersion.DMC5] = 51,
            [EfxVersion.RE7] = 43,
            [EfxVersion.MHRise] = 90,
            [EfxVersion.RE8] = 95,
            [EfxVersion.RERT] = 96,
            [EfxVersion.MHRiseSB] = 96,
            [EfxVersion.RE4] = 101,
            [EfxVersion.DD2] = 103, // ok
        },
        [EfxAttributeType.EmitterShape3D] = new() {
            [EfxVersion.RE3] = 68,
            [EfxVersion.RE2] = 52,
            [EfxVersion.DMC5] = 52,
            [EfxVersion.RE7] = 44,
            [EfxVersion.MHRise] = 91,
            [EfxVersion.RE8] = 96,
            [EfxVersion.RERT] = 97,
            [EfxVersion.MHRiseSB] = 97,
            [EfxVersion.RE4] = 102,
            [EfxVersion.DD2] = 104, // ok
        },
        [EfxAttributeType.EmitterShape3DExpression] = new() {
            [EfxVersion.RE3] = 69,
            [EfxVersion.RE2] = 53,
            [EfxVersion.DMC5] = 53,
            [EfxVersion.RE7] = 45,
            [EfxVersion.MHRise] = 92,
            [EfxVersion.RE8] = 97,
            [EfxVersion.RERT] = 98,
            [EfxVersion.MHRiseSB] = 98,
            [EfxVersion.RE4] = 103,
            [EfxVersion.DD2] = 105, // ok
        },
        [EfxAttributeType.AlphaCorrection] = new() {
            [EfxVersion.RE3] = 70,
            [EfxVersion.RE2] = 54,
            [EfxVersion.DMC5] = 54,
            [EfxVersion.RE7] = 46,
            [EfxVersion.MHRise] = 93,
            [EfxVersion.RE8] = 98,
            [EfxVersion.RERT] = 99,
            [EfxVersion.MHRiseSB] = 99,
            [EfxVersion.RE4] = 104,
            [EfxVersion.DD2] = 106, // guess
        },
        [EfxAttributeType.ContrastHighlighter] = new() {
            [EfxVersion.RE3] = 71,
            [EfxVersion.MHRise] = 94,
            [EfxVersion.RE8] = 99,
            [EfxVersion.RERT] = 100,
            [EfxVersion.MHRiseSB] = 100,
            [EfxVersion.RE4] = 105, // probably
            [EfxVersion.DD2] = 107, // guess
        },
        [EfxAttributeType.ColorGrading] = new() {
            [EfxVersion.RE3] = 72,
            [EfxVersion.MHRise] = 95,
            [EfxVersion.RE8] = 100,
            [EfxVersion.RERT] = 101,
            [EfxVersion.MHRiseSB] = 101,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 108, // ok
        },
        [EfxAttributeType.Blink] = new() {
            [EfxVersion.MHRise] = 96,
            [EfxVersion.RE8] = 101,
            [EfxVersion.RERT] = 102,
            [EfxVersion.MHRiseSB] = 102,
            [EfxVersion.RE4] = 108,
            [EfxVersion.DD2] = 110,
        },

        [EfxAttributeType.Noise] = new() {
            [EfxVersion.MHRise] = 97,
            [EfxVersion.RE8] = 102,
            [EfxVersion.RERT] = 103,
            [EfxVersion.MHRiseSB] = 103,
            [EfxVersion.RE4] = 109,
            [EfxVersion.DD2] = 111,
        },
        [EfxAttributeType.NoiseExpression] = new() {
            [EfxVersion.RERT] = 104,
            [EfxVersion.MHRiseSB] = 104,
            [EfxVersion.RE4] = 110,
            [EfxVersion.DD2] = 112,
        },

        [EfxAttributeType.TexelChannelOperator] = new() {
            [EfxVersion.RE3] = 73,
            [EfxVersion.MHRise] = 98,
            [EfxVersion.RE8] = 103,
            [EfxVersion.RERT] = 105,
            [EfxVersion.MHRiseSB] = 105,
            [EfxVersion.RE4] = 111,
            [EfxVersion.DD2] = 113,
        },
        [EfxAttributeType.TexelChannelOperatorClip] = new() {
            [EfxVersion.RE3] = 74,
            [EfxVersion.MHRise] = 99,
            [EfxVersion.RE8] = 104,
            [EfxVersion.RERT] = 106,
            [EfxVersion.MHRiseSB] = 106,
            [EfxVersion.RE4] = 112, // guess
            [EfxVersion.DD2] = 114, // probably
        },
        [EfxAttributeType.TexelChannelOperatorExpression] = new() {
            [EfxVersion.RE3] = 75,
            [EfxVersion.MHRise] = 100,
            [EfxVersion.RE8] = 105,
            [EfxVersion.RERT] = 107,
            [EfxVersion.MHRiseSB] = 107,
            [EfxVersion.RE4] = 113, // guess
            [EfxVersion.DD2] = 115, // probably
        },
        [EfxAttributeType.TypeStrainRibbon] = new() {
            [EfxVersion.RE3] = 76,
            [EfxVersion.RE2] = 55,
            [EfxVersion.DMC5] = 55,
            [EfxVersion.RE7] = 47,
            [EfxVersion.MHRise] = 101,
            [EfxVersion.RE8] = 106,
            [EfxVersion.RERT] = 108,
            [EfxVersion.MHRiseSB] = 108,
            // [EfxVersion.RE4] = 114,
            // [EfxVersion.DD2] = 116,
        },
        [EfxAttributeType.UnknownRE4_114TypeStrainRibbon] = new() {
            [EfxVersion.RE4] = 114, // structure looks vaguely similar to TypeStrainRibbon, but big reodering
            [EfxVersion.DD2] = 116,
        },
        [EfxAttributeType.TypeStrainRibbonExpression] = new() {
            [EfxVersion.RE3] = 77,
            [EfxVersion.RE2] = 56,
            [EfxVersion.DMC5] = 56,
            [EfxVersion.RE7] = 48,
            [EfxVersion.MHRise] = 102,
            [EfxVersion.RE8] = 107,
            [EfxVersion.RERT] = 109,
            [EfxVersion.MHRiseSB] = 109,
            // [EfxVersion.RE4] = 115,
            // [EfxVersion.DD2] = 117,
        },
        [EfxAttributeType.UnknownDD2_117TypeStrainRibbonExpression] = new() {
            [EfxVersion.DD2] = 117,
        },

        [EfxAttributeType.TypeLightning3D] = new() {
            [EfxVersion.RE3] = 78,
            [EfxVersion.RE2] = 57,
            [EfxVersion.DMC5] = 57,
            [EfxVersion.MHRise] = 103,
            [EfxVersion.RE8] = 108,
            [EfxVersion.RERT] = 110,
            [EfxVersion.MHRiseSB] = 110,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 118,
        },
        [EfxAttributeType.TypeLightning3DExpression] = new() {
            [EfxVersion.MHRise] = 104,
            [EfxVersion.RE8] = 109,
            [EfxVersion.RERT] = 111,
            [EfxVersion.MHRiseSB] = 111,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 119,
        },
        [EfxAttributeType.TypeLightning3DMaterial] = new() {
            [EfxVersion.MHRise] = 105,
            [EfxVersion.RE8] = 110,
            [EfxVersion.RERT] = 112,
            [EfxVersion.MHRiseSB] = 112,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 120,
        },
        [EfxAttributeType.TypeLightning3DMaterialClip] = new() {
            [EfxVersion.DD2] = 121,
        },
        [EfxAttributeType.TypeLightning3DMaterialExpression] = new() {
            [EfxVersion.DD2] = 122,
        },
        [EfxAttributeType.ShaderSettings] = new() {
            [EfxVersion.RE3] = 79,
            [EfxVersion.RE2] = 58,
            [EfxVersion.DMC5] = 58,
            [EfxVersion.RE7] = 49,
            [EfxVersion.MHRise] = 106,
            [EfxVersion.RE8] = 111,
            [EfxVersion.RERT] = 113,
            [EfxVersion.MHRiseSB] = 113,
            [EfxVersion.RE4] = 123,
            [EfxVersion.DD2] = 126,
        },
        [EfxAttributeType.ShaderSettingsExpression] = new() {
            [EfxVersion.RE3] = 80,
            [EfxVersion.RE2] = 59,
            [EfxVersion.DMC5] = 59,
            [EfxVersion.RE7] = 50,
            [EfxVersion.MHRise] = 107,
            [EfxVersion.RE8] = 112,
            [EfxVersion.RERT] = 114, // unused
            [EfxVersion.MHRiseSB] = 114, // unused
            [EfxVersion.RE4] = 124, // unused
            [EfxVersion.DD2] = 127, // guess
        },
        [EfxAttributeType.Distortion] = new() {
            [EfxVersion.RE3] = 81,
            [EfxVersion.RE2] = 60,
            [EfxVersion.DMC5] = 60,
            [EfxVersion.RE7] = 51,
            [EfxVersion.MHRise] = 108,
            [EfxVersion.RE8] = 113,
            [EfxVersion.RERT] = 115,
            [EfxVersion.MHRiseSB] = 115,
            [EfxVersion.RE4] = 125,
            [EfxVersion.DD2] = 128,
        },
        [EfxAttributeType.DistortionExpression] = new() {
            [EfxVersion.MHRise] = 109,
            [EfxVersion.RE8] = 114,
            [EfxVersion.RERT] = 116,
            [EfxVersion.MHRiseSB] = 116,
            [EfxVersion.RE4] = 126, // unused
            [EfxVersion.DD2] = 129,
        },
        [EfxAttributeType.VolumetricLighting] = new() {
            [EfxVersion.MHRise] = 110,
            [EfxVersion.RE8] = 115,
            [EfxVersion.RE4] = 127, // unused
            [EfxVersion.DD2] = UNKNOWN, // 130
        },
        [EfxAttributeType.RenderTarget] = new() { // ok
            [EfxVersion.RE3] = 82,
            [EfxVersion.RE2] = 61,
            [EfxVersion.DMC5] = 61,
            [EfxVersion.RE7] = 52,
            [EfxVersion.MHRise] = 111,
            [EfxVersion.RE8] = 116,
            [EfxVersion.RERT] = 118,
            [EfxVersion.MHRiseSB] = 118,
            [EfxVersion.RE4] = 128,
            [EfxVersion.DD2] = UNKNOWN, // 131
        },
        [EfxAttributeType.UnitCulling] = new() {
            [EfxVersion.RE3] = 98,
            [EfxVersion.RE2] = 77,
            [EfxVersion.DMC5] = 77,
            [EfxVersion.RE7] = 67,
            [EfxVersion.MHRise] = 128,
            [EfxVersion.RE8] = 133,
            [EfxVersion.RERT] = 135,
            [EfxVersion.MHRiseSB] = 136,
            [EfxVersion.RE4] = 130,
            [EfxVersion.DD2] = 133, // guess based on UnitCullingExpression parameter names; struct is also identical
        },
        [EfxAttributeType.UnitCullingExpression] = new() {
            [EfxVersion.RE4] = UNSUPPORTED,
            [EfxVersion.DD2] = 134,
        },
        [EfxAttributeType.PtLife] = new() { // ok
            [EfxVersion.RE3] = 83,
            [EfxVersion.RE2] = 62,
            [EfxVersion.DMC5] = 62,
            [EfxVersion.RE7] = 53,
            [EfxVersion.MHRise] = 112,
            [EfxVersion.RE8] = 117,
            [EfxVersion.RERT] = 119,
            [EfxVersion.MHRiseSB] = 120,
            [EfxVersion.RE4] = 131,
            [EfxVersion.DD2] = 135,
        },
        [EfxAttributeType.PtBehavior] = new() {
            [EfxVersion.RE3] = 84,
            [EfxVersion.RE2] = 63,
            [EfxVersion.DMC5] = 63,
            [EfxVersion.RE7] = 54,
            [EfxVersion.MHRise] = 113,
            [EfxVersion.RE8] = 118,
            [EfxVersion.RERT] = 120,
            [EfxVersion.MHRiseSB] = 121,
            [EfxVersion.RE4] = 132,
            [EfxVersion.DD2] = 136, // ok
        },
        [EfxAttributeType.PtBehaviorClip] = new() {
            [EfxVersion.RE3] = 85,
            [EfxVersion.RE2] = 64,
            [EfxVersion.DMC5] = 64,
            [EfxVersion.RE7] = 55,
            [EfxVersion.MHRise] = 114,
            [EfxVersion.RE8] = 119,
            [EfxVersion.RERT] = 121, // probably, unused
            [EfxVersion.MHRiseSB] = 122,
            [EfxVersion.RE4] = 133, // probably, unused
            [EfxVersion.DD2] = 137, // probably
        },
        [EfxAttributeType.PlayEfx] = new() {
            [EfxVersion.RE3] = 86,
            [EfxVersion.RE2] = 65,
            [EfxVersion.DMC5] = 65,
            [EfxVersion.RE7] = 56,
            [EfxVersion.MHRise] = 115,
            [EfxVersion.RE8] = 120,
            [EfxVersion.RERT] = 122,
            [EfxVersion.MHRiseSB] = 123,
            [EfxVersion.RE4] = 134, // guess
            [EfxVersion.DD2] = 138,
        },
        [EfxAttributeType.FadeByAngle] = new() {
            [EfxVersion.RE3] = 87,
            [EfxVersion.RE2] = 66,
            [EfxVersion.DMC5] = 66,
            [EfxVersion.RE7] = 57,
            [EfxVersion.MHRise] = 116,
            [EfxVersion.RE8] = 121,
            [EfxVersion.RERT] = 123,
            [EfxVersion.MHRiseSB] = 124,
            [EfxVersion.RE4] = 135,
            [EfxVersion.DD2] = 139,
        },
        [EfxAttributeType.FadeByAngleExpression] = new() {
            [EfxVersion.RE3] = 88,
            [EfxVersion.RE2] = 67,
            [EfxVersion.DMC5] = 67,
            [EfxVersion.RE7] = 58,
            [EfxVersion.MHRise] = 117,
            [EfxVersion.RE8] = 122,
            [EfxVersion.RERT] = 124,
            [EfxVersion.MHRiseSB] = 125,
            [EfxVersion.RE4] = 136,
            [EfxVersion.DD2] = 140, // guess
        },
        [EfxAttributeType.FadeByEmitterAngle] = new() {
            [EfxVersion.RE3] = 89,
            [EfxVersion.RE2] = 68,
            [EfxVersion.DMC5] = 68,
            [EfxVersion.MHRise] = 118,
            [EfxVersion.RE8] = 123,
            [EfxVersion.RERT] = 125,
            [EfxVersion.MHRiseSB] = 126,
            [EfxVersion.RE4] = 137, // probably
            [EfxVersion.DD2] = 141, // guess
        },
        [EfxAttributeType.FadeByDepth] = new() {
            [EfxVersion.RE3] = 90,
            [EfxVersion.RE2] = 69,
            [EfxVersion.DMC5] = 69,
            [EfxVersion.RE7] = 59,
            [EfxVersion.MHRise] = 119,
            [EfxVersion.RE8] = 124,
            [EfxVersion.RERT] = 126,
            [EfxVersion.MHRiseSB] = 127,
            [EfxVersion.RE4] = 138,
            [EfxVersion.DD2] = 142, // ok
        },
        [EfxAttributeType.FadeByDepthExpression] = new() {
            [EfxVersion.RE3] = 91,
            [EfxVersion.RE2] = 70,
            [EfxVersion.DMC5] = 70,
            [EfxVersion.RE7] = 60,
            [EfxVersion.MHRise] = 120,
            [EfxVersion.RE8] = 125,
            [EfxVersion.RERT] = 127,
            [EfxVersion.MHRiseSB] = 128,
            [EfxVersion.RE4] = 139,
            [EfxVersion.DD2] = 143, // guess
        },
        [EfxAttributeType.FadeByOcclusion] = new() {
            [EfxVersion.RE3] = 92,
            [EfxVersion.RE2] = 71,
            [EfxVersion.DMC5] = 71,
            [EfxVersion.RE7] = 61,
            [EfxVersion.MHRise] = 121,
            [EfxVersion.RE8] = 126,
            [EfxVersion.RERT] = 128,
            [EfxVersion.MHRiseSB] = 129,
            [EfxVersion.RE4] = 140,
            [EfxVersion.DD2] = 144, // ok
        },
        [EfxAttributeType.FadeByOcclusionExpression] = new() {
            [EfxVersion.RE3] = 93,
            [EfxVersion.RE2] = 72,
            [EfxVersion.DMC5] = 72,
            [EfxVersion.RE7] = 62,
            [EfxVersion.MHRise] = 122,
            [EfxVersion.RE8] = 127,
            [EfxVersion.RERT] = 129,
            [EfxVersion.MHRiseSB] = 130,
            [EfxVersion.RE4] = 141,
            [EfxVersion.DD2] = 145, // guess
        },
        [EfxAttributeType.UnknownDD2_146New] = new() {
            [EfxVersion.DD2] = 146,
        },
        [EfxAttributeType.FakeDoF] = new() {
            [EfxVersion.RE3] = 94,
            [EfxVersion.RE2] = 73,
            [EfxVersion.DMC5] = 73,
            [EfxVersion.RE7] = 63,
            [EfxVersion.MHRise] = 123,
            [EfxVersion.RE8] = 128,
            [EfxVersion.RERT] = 130, // guess
            [EfxVersion.MHRiseSB] = 131, // guess
            [EfxVersion.RE4] = 142,
            [EfxVersion.DD2] = 147, // ok
        },
        [EfxAttributeType.LuminanceBleed] = new() {
            [EfxVersion.RE3] = 95,
            [EfxVersion.RE2] = 74,
            [EfxVersion.DMC5] = 74,
            [EfxVersion.RE7] = 64,
            [EfxVersion.MHRise] = 124,
            [EfxVersion.RE8] = 129,
            [EfxVersion.RERT] = 131,
            [EfxVersion.MHRiseSB] = 132, // guess
            [EfxVersion.RE4] = 143,
            [EfxVersion.DD2] = 148, // guess
        },

        [EfxAttributeType.ScaleByDepth] = new() {
            [EfxVersion.MHRise] = 125,
            [EfxVersion.RE8] = 130,
            [EfxVersion.MHRiseSB] = 133,
            [EfxVersion.RE4] = 144,
            [EfxVersion.DD2] = 149,
        },
        [EfxAttributeType.TypeNodeBillboard] = new() {
            [EfxVersion.RE3] = 96,
            [EfxVersion.RE2] = 75,
            [EfxVersion.DMC5] = 75,
            [EfxVersion.RE7] = 65,
            [EfxVersion.MHRise] = 126,
            [EfxVersion.RE8] = 131,
            [EfxVersion.RERT] = 133,
            [EfxVersion.MHRiseSB] = 134,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 150,
        },
        [EfxAttributeType.TypeNodeBillboardExpression] = new() {
            [EfxVersion.RE3] = 97,
            [EfxVersion.RE2] = 76,
            [EfxVersion.DMC5] = 76,
            [EfxVersion.RE7] = 66,
            [EfxVersion.MHRise] = 127,
            [EfxVersion.RE8] = 132,
            [EfxVersion.RERT] = 134,
            [EfxVersion.MHRiseSB] = 135,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = UNKNOWN, // 151
        },
        [EfxAttributeType.FluidEmitter2D] = new() {
            [EfxVersion.RE3] = 99,
            [EfxVersion.RE2] = 78,
            [EfxVersion.DMC5] = 78,
            [EfxVersion.RE7] = 68,
            [EfxVersion.RERT] = 136,
            [EfxVersion.MHRise] = 129,
            [EfxVersion.RE8] = 134,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 152,
        },
        [EfxAttributeType.FluidEmitter2DClip] = new() {
            [EfxVersion.RE8] = 135,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.FluidEmitter2DExpression] = new() {
            [EfxVersion.RE8] = 136,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 154,
        },
        [EfxAttributeType.FluidSimulator2D] = new() {
            [EfxVersion.RE3] = 100,
            [EfxVersion.RE2] = 79,
            [EfxVersion.DMC5] = 79,
            [EfxVersion.RE7] = 69,
            [EfxVersion.MHRise] = 130,
            [EfxVersion.RE8] = 137,
            [EfxVersion.RERT] = 139,
            [EfxVersion.RE4] = 150,
            [EfxVersion.DD2] = 155,
        },
        [EfxAttributeType.PlayEmitter] = new() {
            [EfxVersion.RE3] = 101,
            [EfxVersion.RE2] = 80,
            [EfxVersion.DMC5] = 80,
            [EfxVersion.RE7] = 70,
            [EfxVersion.MHRise] = 131,
            [EfxVersion.RE8] = 138,
            [EfxVersion.RERT] = 140,
            [EfxVersion.MHRiseSB] = 141,
            [EfxVersion.RE4] = 151,
            [EfxVersion.DD2] = 157,
        },
        [EfxAttributeType.PtTransform3D] = new() {
            [EfxVersion.RE3] = 102,
            [EfxVersion.RE2] = 81,
            [EfxVersion.DMC5] = 81,
            [EfxVersion.RE7] = 71,
            [EfxVersion.MHRise] = 132,
            [EfxVersion.RE8] = 139,
            [EfxVersion.RERT] = 141,
            [EfxVersion.MHRiseSB] = 142,
            [EfxVersion.RE4] = 152,
            [EfxVersion.DD2] = 158, // ok
        },
        [EfxAttributeType.PtTransform3DClip] = new() {
            [EfxVersion.RE3] = 103,
            [EfxVersion.RE2] = 82,
            [EfxVersion.DMC5] = 82,
            [EfxVersion.RE7] = 72,
            [EfxVersion.MHRise] = 133,
            [EfxVersion.RE8] = 140,
            [EfxVersion.RERT] = 142,
            [EfxVersion.MHRiseSB] = 143,
            [EfxVersion.RE4] = 153,
            [EfxVersion.DD2] = 159, // ok
        },
        [EfxAttributeType.PtTransform3DExpression] = new() {
            [EfxVersion.DD2] = 160, // ok
        },

        [EfxAttributeType.PtTransform2D] = new() {
            [EfxVersion.RE3] = 104,
            [EfxVersion.RE2] = 83,
            [EfxVersion.DMC5] = 83,
            [EfxVersion.RE7] = 73,
            [EfxVersion.MHRise] = 134,
            [EfxVersion.RE8] = 141,
            [EfxVersion.RE4] = 155,
            [EfxVersion.DD2] = 161,
        },
        [EfxAttributeType.PtTransform2DClip] = new() {
            [EfxVersion.RE3] = 105,
            [EfxVersion.RE2] = 84,
            [EfxVersion.DMC5] = 84,
            [EfxVersion.RE7] = 74,
            [EfxVersion.MHRise] = 135,
            [EfxVersion.RE8] = 142,
            [EfxVersion.RE4] = UNKNOWN,
        },

        [EfxAttributeType.PtVelocity3D] = new() {
            [EfxVersion.RE3] = 106,
            [EfxVersion.RE2] = 85,
            [EfxVersion.DMC5] = 85,
            [EfxVersion.RE7] = 75,
            [EfxVersion.MHRise] = 136,
            [EfxVersion.RE8] = 143,
            [EfxVersion.RERT] = 145, // should be correct
            [EfxVersion.MHRiseSB] = 146, // should be correct
            [EfxVersion.RE4] = 157,
            [EfxVersion.DD2] = 163,
        },
        [EfxAttributeType.PtVelocity3DClip] = new() {
            [EfxVersion.RE3] = 107,
            [EfxVersion.RE2] = 86,
            [EfxVersion.DMC5] = 86,
            [EfxVersion.RE7] = 76,
            [EfxVersion.MHRise] = 137,
            [EfxVersion.RE8] = 144,
            [EfxVersion.RERT] = 146,
            [EfxVersion.MHRiseSB] = 147,
            [EfxVersion.RE4] = 158, // unused
            [EfxVersion.DD2] = 164,
        },
        [EfxAttributeType.PtVelocity2D] = new() {
            [EfxVersion.RE3] = 108,
            [EfxVersion.RE2] = 87,
            [EfxVersion.DMC5] = 87,
            [EfxVersion.RE7] = 77,
            [EfxVersion.MHRise] = 138,
            [EfxVersion.RE8] = 145,
            [EfxVersion.RERT] = 147, // guess
            [EfxVersion.RE4] = 159, // probably
            [EfxVersion.DD2] = 165, // ok
        },
        [EfxAttributeType.PtVelocity2DClip] = new() {
            [EfxVersion.RE3] = 109,
            [EfxVersion.RE2] = 88,
            [EfxVersion.DMC5] = 88,
            [EfxVersion.RE7] = 78,
            [EfxVersion.MHRise] = 139,
            [EfxVersion.RE8] = 146,
            [EfxVersion.RERT] = 148, // guess
            [EfxVersion.RE4] = 160, // probably
            [EfxVersion.DD2] = 166, // guess
        },
        [EfxAttributeType.PtColliderAction] = new() {
            [EfxVersion.RE3] = 110,
            [EfxVersion.RE2] = 89,
            [EfxVersion.DMC5] = 89,
            [EfxVersion.RE7] = 79,
            [EfxVersion.MHRise] = 140,
            [EfxVersion.RE8] = 147,
            [EfxVersion.RERT] = 149,
            [EfxVersion.RE4] = 161,
            [EfxVersion.DD2] = 167, // guess
        },
        [EfxAttributeType.PtCollision] = new() {
            [EfxVersion.RE3] = 111,
            [EfxVersion.RE2] = 90,
            [EfxVersion.DMC5] = 90,
            [EfxVersion.RE7] = 80,
            [EfxVersion.MHRise] = 141,
            [EfxVersion.RE8] = 148,
            [EfxVersion.RERT] = 150,
            [EfxVersion.RE4] = 162,
            [EfxVersion.DD2] = 168, // ok
        },
        [EfxAttributeType.PtColor] = new() {
            [EfxVersion.RE3] = 112,
            [EfxVersion.RE2] = 91,
            [EfxVersion.DMC5] = 91,
            [EfxVersion.RE7] = 81,
            [EfxVersion.MHRise] = 142,
            [EfxVersion.RE8] = 149,
            [EfxVersion.RERT] = 151,
            [EfxVersion.MHRiseSB] = 152,
            [EfxVersion.RE4] = 164,
            [EfxVersion.DD2] = 170,
        },
        [EfxAttributeType.PtColorClip] = new() {
            [EfxVersion.RE3] = 113,
            [EfxVersion.RE2] = 92,
            [EfxVersion.DMC5] = 92,
            [EfxVersion.RE7] = 82,
            [EfxVersion.MHRise] = 143,
            [EfxVersion.RE8] = 150,
            [EfxVersion.RERT] = 152,
            [EfxVersion.MHRiseSB] = 153,
            [EfxVersion.RE4] = 165,
            [EfxVersion.DD2] = 171,
        },
        [EfxAttributeType.PtUvSequence] = new() {
            [EfxVersion.RE3] = 114,
            [EfxVersion.RE2] = 93,
            [EfxVersion.DMC5] = 93,
            [EfxVersion.RE7] = 83,
            [EfxVersion.MHRise] = 144,
            [EfxVersion.RE8] = 151,
            [EfxVersion.RERT] = 153,
            [EfxVersion.MHRiseSB] = 154,
            [EfxVersion.RE4] = 168,
            [EfxVersion.DD2] = 174, // ok
        },
        [EfxAttributeType.PtUvSequenceClip] = new() {
            [EfxVersion.RE3] = 115,
            [EfxVersion.RE2] = 94,
            [EfxVersion.DMC5] = 94,
            [EfxVersion.RE7] = 84,
            [EfxVersion.MHRise] = 145,
            [EfxVersion.RE8] = 152,
            [EfxVersion.RERT] = 154,
            [EfxVersion.RE4] = 169,
            [EfxVersion.MHRiseSB] = 155,
            [EfxVersion.DD2] = 175, // ok
        },
        [EfxAttributeType.MeshEmitter] = new() {
            [EfxVersion.RE3] = 116,
            [EfxVersion.RE2] = 95,
            [EfxVersion.DMC5] = 95,
            [EfxVersion.MHRise] = 146,
            [EfxVersion.RE8] = 153,
            [EfxVersion.RERT] = 155,
            [EfxVersion.MHRiseSB] = 156, // probably
            [EfxVersion.RE4] = 170, // probably
            [EfxVersion.DD2] = 176, // probably
        },
        [EfxAttributeType.MeshEmitterClip] = new() {
            [EfxVersion.RE3] = 117,
            [EfxVersion.RE2] = 96,
            [EfxVersion.DMC5] = 96,
            [EfxVersion.MHRise] = 147,
            [EfxVersion.RE8] = 154,
            [EfxVersion.RERT] = 156, // probably
            [EfxVersion.MHRiseSB] = 157, // guess
            [EfxVersion.RE4] = 171, // unused
        },
        [EfxAttributeType.MeshEmitterExpression] = new() {
            [EfxVersion.RE3] = 118,
            [EfxVersion.RE2] = 97,
            [EfxVersion.DMC5] = 97,
            [EfxVersion.MHRise] = 148,
            [EfxVersion.RE8] = 155,
            [EfxVersion.RERT] = 157, // probably
            [EfxVersion.MHRiseSB] = 158, // guess
            [EfxVersion.RE4] = 172, // unused
        },
        [EfxAttributeType.ScreenSpaceEmitter] = new() {
            [EfxVersion.MHRise] = 149,
            [EfxVersion.RE8] = 156,
            [EfxVersion.RERT] = 158, // guess
            [EfxVersion.MHRiseSB] = 159, // guess
            [EfxVersion.RE4] = 173,
            [EfxVersion.DD2] = 179,
        },
        // ScreenSpaceEmitterExpression in between maybe?
        [EfxAttributeType.VectorFieldParameter] = new() {
            [EfxVersion.RE3] = 119,
            [EfxVersion.RE2] = 98,
            [EfxVersion.DMC5] = 98,
            [EfxVersion.MHRise] = 150,
            [EfxVersion.RE8] = 157,
            [EfxVersion.RERT] = 159,
            [EfxVersion.MHRiseSB] = 160,
            [EfxVersion.RE4] = 175,
            [EfxVersion.DD2] = 181,
        },
        [EfxAttributeType.VectorFieldParameterClip] = new() {
            [EfxVersion.RE3] = 120,
            [EfxVersion.RE2] = 99,
            [EfxVersion.DMC5] = 99,
            [EfxVersion.MHRise] = 151,
            [EfxVersion.RE8] = 158,
            [EfxVersion.RERT] = 161,
            [EfxVersion.MHRiseSB] = 161,
            [EfxVersion.RE4] = 176, // probably, unused
            [EfxVersion.DD2] = 182,
        },
        [EfxAttributeType.VectorFieldParameterExpression] = new() {
            [EfxVersion.RE3] = 121,
            [EfxVersion.RE2] = 100,
            [EfxVersion.DMC5] = 100,
            [EfxVersion.MHRise] = 152,
            [EfxVersion.RE8] = 159,
            [EfxVersion.RERT] = 162,
            [EfxVersion.MHRiseSB] = 162,
            [EfxVersion.RE4] = 177, // probably
            [EfxVersion.DD2] = 183,
        },

        [EfxAttributeType.GlobalVectorField] = new() {
            [EfxVersion.MHRise] = 153,
            [EfxVersion.RE8] = 160,
            [EfxVersion.RE4] = UNKNOWN, // 178
            [EfxVersion.DD2] = UNKNOWN, // 184
        },
        [EfxAttributeType.GlobalVectorFieldClip] = new() {
            [EfxVersion.MHRise] = 154,
            [EfxVersion.RE8] = 161,
            [EfxVersion.RE4] = UNKNOWN, // 179
            [EfxVersion.DD2] = UNKNOWN, // 185
        },
        [EfxAttributeType.GlobalVectorFieldExpression] = new() {
            [EfxVersion.MHRise] = 155,
            [EfxVersion.RE8] = 162,
            [EfxVersion.RE4] = UNKNOWN, // 180
            [EfxVersion.DD2] = UNKNOWN, // 186
        },
        [EfxAttributeType.DirectionalFieldParameter] = new() {
            [EfxVersion.MHRise] = 156,
            [EfxVersion.RE8] = 163,
            [EfxVersion.RE4] = UNKNOWN, // 181
            [EfxVersion.DD2] = UNKNOWN, // 187
        },
        [EfxAttributeType.DirectionalFieldParameterClip] = new() {
            [EfxVersion.MHRise] = 157,
            [EfxVersion.RE8] = 164,
            [EfxVersion.RE4] = UNKNOWN, // 182
            [EfxVersion.DD2] = UNKNOWN, // 188
        },
        [EfxAttributeType.DirectionalFieldParameterExpression] = new() {
            [EfxVersion.MHRise] = 158,
            [EfxVersion.RE8] = 165,
            [EfxVersion.RE4] = UNKNOWN, //183
            [EfxVersion.DD2] = UNKNOWN, // 189
        },
        [EfxAttributeType.DepthOperator] = new() {
            [EfxVersion.RE3] = 122,
            [EfxVersion.RE2] = 101,
            [EfxVersion.DMC5] = 101,
            [EfxVersion.MHRise] = 159,
            [EfxVersion.RE8] = 166,
            [EfxVersion.RERT] = 168,
            [EfxVersion.MHRiseSB] = 168,
            [EfxVersion.RE4] = 184,
            [EfxVersion.DD2] = 190,
        },
        [EfxAttributeType.PlaneCollider] = new() {
            [EfxVersion.RE3] = 123,
            [EfxVersion.MHRise] = 160,
            [EfxVersion.RE8] = 167,
            [EfxVersion.RE4] = 185,
            [EfxVersion.DD2] = 191,
        },
        [EfxAttributeType.PlaneColliderExpression] = new() {
            [EfxVersion.RE3] = 124,
            [EfxVersion.MHRise] = 161,
            [EfxVersion.RE8] = 168,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 192, // guess
        },
        [EfxAttributeType.DepthOcclusion] = new() {
            [EfxVersion.RE3] = 125,
            [EfxVersion.MHRise] = 162,
            [EfxVersion.RE8] = 169,
            [EfxVersion.RE4] = 187,
            [EfxVersion.DD2] = 193,
        },
        [EfxAttributeType.ShapeOperator] = new() {
            [EfxVersion.RE3] = 126,
            [EfxVersion.RE2] = 102,
            [EfxVersion.DMC5] = 102,
            [EfxVersion.MHRise] = 163,
            [EfxVersion.RE8] = 170,
            [EfxVersion.RE4] = UNKNOWN, // 188
            [EfxVersion.DD2] = 194, // guess
        },
        [EfxAttributeType.ShapeOperatorExpression] = new() {
            [EfxVersion.RE3] = 127,
            [EfxVersion.RE2] = 103,
            [EfxVersion.DMC5] = 103,
            [EfxVersion.MHRise] = 164,
            [EfxVersion.RE8] = 171,
            [EfxVersion.RE4] = UNKNOWN, // 189
            [EfxVersion.DD2] = 195, // guess
        },
        [EfxAttributeType.WindInfluence3DDelayFrame] = new() {
            [EfxVersion.RE3] = 128,
            [EfxVersion.MHRise] = 165,
            [EfxVersion.RE8] = 172,
            [EfxVersion.RE4] = UNKNOWN, // 190
            [EfxVersion.DD2] = 196, // guess
        },
        [EfxAttributeType.WindInfluence3D] = new() {
            [EfxVersion.RE3] = 129,
            [EfxVersion.RE2] = 104,
            [EfxVersion.DMC5] = 104,
            [EfxVersion.MHRise] = 166,
            [EfxVersion.RE8] = 173,
            [EfxVersion.RE4] = 191,
            [EfxVersion.DD2] = 197,
        },
        [EfxAttributeType.Attractor] = new() {
            [EfxVersion.RE3] = 130,
            [EfxVersion.MHRise] = 167,
            [EfxVersion.RE8] = 174,
            [EfxVersion.RERT] = 176,
            [EfxVersion.MHRiseSB] = 177,
            [EfxVersion.RE4] = 192,
            [EfxVersion.DD2] = 198,
        },
        [EfxAttributeType.AttractorClip] = new() {
            [EfxVersion.RE3] = 131,
            [EfxVersion.MHRise] = 168,
            [EfxVersion.RE8] = 175,
            [EfxVersion.RERT] = 177, // probably
            [EfxVersion.MHRiseSB] = 178,
            [EfxVersion.RE4] = 193, // guess
            [EfxVersion.DD2] = 199,
        },
        [EfxAttributeType.AttractorExpression] = new() {
            [EfxVersion.RE3] = 132,
            [EfxVersion.MHRise] = 169,
            [EfxVersion.RE8] = 176,
            [EfxVersion.RERT] = 178, // probably
            [EfxVersion.MHRiseSB] = 179,
            [EfxVersion.RE4] = 194, // guess
            [EfxVersion.DD2] = 200,
        },
        // seems to be its own struct, not obviously related to any of the nearby cases
        [EfxAttributeType.UnknownRE4_195] = new() {
            [EfxVersion.RE4] = 195,
            // [EfxVersion.DD2] = UNKNOWN, // 201
        },

        [EfxAttributeType.UnknownRE4_197] = new() {
            [EfxVersion.RE4] = 197,
            [EfxVersion.DD2] = UNKNOWN, // 203
        },

        [EfxAttributeType.CustomComputeShader] = new() {
            [EfxVersion.RE3] = 133,
            [EfxVersion.MHRise] = 170,
            [EfxVersion.RE8] = 177,
        },
        [EfxAttributeType.EmitterPriority] = new() {
            [EfxVersion.RE3] = 142,
            [EfxVersion.RE2] = 108,
            [EfxVersion.DMC5] = 108,
            [EfxVersion.RE7] = 86,
            [EfxVersion.MHRise] = 182,
            [EfxVersion.RE8] = 192,
            [EfxVersion.RERT] = 181,
            [EfxVersion.MHRiseSB] = 182,
            [EfxVersion.RE4] = 200,
        },
        [EfxAttributeType.DrawOverlay] = new() {
            [EfxVersion.RE3] = 143,
            [EfxVersion.RE2] = 109,
            [EfxVersion.DMC5] = 109,
            [EfxVersion.MHRise] = 183,
            [EfxVersion.RE8] = 193,
            [EfxVersion.RERT] = 182,
            [EfxVersion.RE4] = 201,
        },
        [EfxAttributeType.VectorField] = new() {
            [EfxVersion.RE3] = 144,
            [EfxVersion.RE2] = 110,
            [EfxVersion.DMC5] = 110,
            [EfxVersion.MHRise] = 184,
            [EfxVersion.RE8] = 194,
            [EfxVersion.RE4] = UNKNOWN, // 202
        },
        [EfxAttributeType.VolumeField] = new() {
            [EfxVersion.RE3] = 145,
            [EfxVersion.RE2] = 111,
            [EfxVersion.DMC5] = 111,
            [EfxVersion.MHRise] = 185,
            [EfxVersion.RE8] = 195,
            [EfxVersion.RE4] = UNKNOWN, // 203
        },
        [EfxAttributeType.DirectionalField] = new() {
            [EfxVersion.MHRise] = 186,
            [EfxVersion.RE8] = 196,
            [EfxVersion.RE4] = UNKNOWN, // 204
        },
        [EfxAttributeType.AngularVelocity3DDelayFrame] = new() {
            [EfxVersion.RE3] = 146,
            [EfxVersion.MHRise] = 187,
            [EfxVersion.RE8] = 197,
            [EfxVersion.RERT] = 186, // guess
            [EfxVersion.MHRiseSB] = 187,
            [EfxVersion.RE4] = 205,
        },
        [EfxAttributeType.AngularVelocity3D] = new() {
            [EfxVersion.RE3] = 147,
            [EfxVersion.RE2] = 112,
            [EfxVersion.DMC5] = 112,
            [EfxVersion.MHRise] = 188,
            [EfxVersion.RE8] = 198,
            [EfxVersion.RERT] = 187,
            [EfxVersion.MHRiseSB] = 188,
            [EfxVersion.RE4] = 206,
        },

        [EfxAttributeType.PtAngularVelocity3D] = new() {
            [EfxVersion.RE3] = 148,
            [EfxVersion.DMC5] = 113,
            [EfxVersion.MHRise] = 189,
            [EfxVersion.RE8] = 199,
            [EfxVersion.RERT] = 189, // guess
            [EfxVersion.MHRiseSB] = 189,
            [EfxVersion.RE4] = UNKNOWN, // 207
        },
        [EfxAttributeType.PtAngularVelocity3DExpression] = new() {
            [EfxVersion.RE3] = 149,
            [EfxVersion.DMC5] = 114,
            [EfxVersion.MHRise] = 190,
            [EfxVersion.RE8] = 200,
            [EfxVersion.RE4] = UNKNOWN, // 208
        },
        [EfxAttributeType.AngularVelocity2DDelayFrame] = new() {
            [EfxVersion.RE3] = 150,
            [EfxVersion.MHRise] = 191,
            [EfxVersion.RE8] = 201,
            [EfxVersion.RERT] = 191,
            [EfxVersion.MHRiseSB] = 191,
            [EfxVersion.RE4] = UNKNOWN, // 209
        },
        [EfxAttributeType.AngularVelocity2D] = new() {
            [EfxVersion.RE3] = 151,
            [EfxVersion.RE2] = 113,
            [EfxVersion.DMC5] = 115,
            [EfxVersion.MHRise] = 192,
            [EfxVersion.RE8] = 202,
            [EfxVersion.RERT] = 192,
            [EfxVersion.MHRiseSB] = 192,
            [EfxVersion.RE4] = 210, // ok
        },
        [EfxAttributeType.PtAngularVelocity2D] = new() {
            [EfxVersion.RE3] = 152,
            [EfxVersion.DMC5] = 116,
            [EfxVersion.MHRise] = 193,
            [EfxVersion.RE8] = 203,
            // [EfxVersion.RERT] = 193,
            [EfxVersion.RE4] = UNKNOWN, // 211
        },
        [EfxAttributeType.PtAngularVelocity2DExpression] = new() {
            [EfxVersion.RE3] = 153,
            [EfxVersion.DMC5] = 117,
            [EfxVersion.MHRise] = 194,
            [EfxVersion.RE8] = 204,
            // [EfxVersion.RERT] = 194,
            [EfxVersion.RE4] = UNKNOWN, // 212
        },

        [EfxAttributeType.UnknownRE4_213] = new() {
            [EfxVersion.RE4] = 213,
        },
        [EfxAttributeType.UnknownSB_195] = new() { // probably same as RE4_213
            [EfxVersion.MHRiseSB] = 195,
            [EfxVersion.RERT] = 195,
        },

        [EfxAttributeType.IgnorePlayerColor] = new() {
            [EfxVersion.RE3] = 154,
            [EfxVersion.RE2] = 114,
            [EfxVersion.DMC5] = 118,
            [EfxVersion.MHRise] = 195,
            [EfxVersion.RE8] = 205,
            [EfxVersion.RERT] = 194,
            [EfxVersion.MHRiseSB] = 196,
            [EfxVersion.RE4] = UNKNOWN, // 200?
            [EfxVersion.DD2] = 213,
        },

        [EfxAttributeType.UnknownDD2_214] = new() {
            [EfxVersion.DD2] = 214,
        },
        [EfxAttributeType.UnknownDD2_218] = new() { // some sort of DelayFrame?
            [EfxVersion.DD2] = 218,
        },
        [EfxAttributeType.UnknownDD2_219] = new() {
            [EfxVersion.DD2] = 219,
        },
        [EfxAttributeType.UnknownDD2_220] = new() {
            [EfxVersion.DD2] = 220,
        },
        [EfxAttributeType.UnknownDD2_221Expression] = new() {
            [EfxVersion.DD2] = 221,
        },
        [EfxAttributeType.UnknownDD2_223] = new() {
            [EfxVersion.DD2] = 223,
        },
        [EfxAttributeType.UnknownDD2_224] = new() {
            [EfxVersion.DD2] = 224,
        },
        [EfxAttributeType.UnknownDD2_225Expression] = new() {
            [EfxVersion.DD2] = 225,
        },
        [EfxAttributeType.UnknownDD2_226] = new() {
            [EfxVersion.DD2] = 226,
        },

        [EfxAttributeType.ProceduralDistortionDelayFrame] = new() {
            [EfxVersion.RE3] = 155,
            [EfxVersion.MHRise] = 196,
            [EfxVersion.RE8] = 206,
            [EfxVersion.RERT] = 198,
            [EfxVersion.MHRiseSB] = 198,
            [EfxVersion.RE4] = 216, // ok
            [EfxVersion.DD2] = UNKNOWN, // 229? 218?
        },
        [EfxAttributeType.ProceduralDistortion] = new() {
            [EfxVersion.RE3] = 156,
            [EfxVersion.RE2] = 115,
            [EfxVersion.DMC5] = 119,
            [EfxVersion.MHRise] = 197,
            [EfxVersion.RE8] = 207,
            [EfxVersion.RERT] = 199,
            [EfxVersion.MHRiseSB] = 199,
            [EfxVersion.RE4] = 217,
            [EfxVersion.DD2] = 230, // values seem ok, though numbering is weird
        },
        [EfxAttributeType.ProceduralDistortionClip] = new() {
            [EfxVersion.RE3] = 157,
            [EfxVersion.RE2] = 116,
            [EfxVersion.DMC5] = 120,
            [EfxVersion.MHRise] = 198,
            [EfxVersion.RE8] = 208,
            [EfxVersion.RERT] = 200,
            [EfxVersion.MHRiseSB] = 200,
            // [EfxVersion.RE4] = 218, // guess
            [EfxVersion.DD2] = UNKNOWN, // 231
        },
        [EfxAttributeType.ProceduralDistortionExpression] = new() {
            [EfxVersion.RE3] = 158,
            [EfxVersion.MHRise] = 199,
            [EfxVersion.RE8] = 209,
            [EfxVersion.RERT] = 201,
            [EfxVersion.MHRiseSB] = 201,
            [EfxVersion.RE4] = 219, // guess
            [EfxVersion.DD2] = UNKNOWN, // 232
        },
        [EfxAttributeType.TestBehaviorUpdater] = new() {
            [EfxVersion.RE3] = 159,
            [EfxVersion.MHRise] = 200,
            [EfxVersion.RE8] = 210,
            [EfxVersion.MHRiseSB] = 202,
            [EfxVersion.RE4] = 220, // guess
        },
        [EfxAttributeType.StretchBlur] = new() {
            [EfxVersion.MHRise] = 201,
            [EfxVersion.RE8] = 211,
            [EfxVersion.MHRiseSB] = 203,
            [EfxVersion.RE4] = 221,
        },
        [EfxAttributeType.StretchBlurExpression] = new() {
            [EfxVersion.MHRise] = 202,
            [EfxVersion.RE8] = 212,
            [EfxVersion.MHRiseSB] = 204,
            [EfxVersion.RE4] = 222, // unused
        },

        [EfxAttributeType.EmitterHSV] = new() {
            [EfxVersion.MHRise] = 203,
            [EfxVersion.RE8] = 213,
            [EfxVersion.MHRiseSB] = 205,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.EmitterHSVExpression] = new() {
            [EfxVersion.MHRise] = 204,
            [EfxVersion.RE8] = 214,
            [EfxVersion.MHRiseSB] = 206,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.FlowMap] = new() {
            [EfxVersion.MHRise] = 205,
            [EfxVersion.RE8] = 215,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.RgbCommon] = new() {
            [EfxVersion.MHRise] = 206,
            [EfxVersion.RE8] = 216,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.RgbWater] = new() {
            [EfxVersion.MHRise] = 207,
            [EfxVersion.RE8] = 217,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.PtFreezer] = new() {
            [EfxVersion.RE8] = 218,
            [EfxVersion.RE4] = UNKNOWN,
        },

        [EfxAttributeType.UnknownDD2_239RgbColor] = new() {
            [EfxVersion.DD2] = 239, // rgbcommon?
        },
        [EfxAttributeType.UnknownDD2_239RgbColorExpression] = new() {
            [EfxVersion.DD2] = 240,
        },

        [EfxAttributeType.UnknownRE4_226] = new() {
            [EfxVersion.RE4] = 226,
        },
        [EfxAttributeType.EmitMask] = new() {
            [EfxVersion.MHRise] = 208,
            [EfxVersion.RE8] = 220,
            [EfxVersion.RE4] = UNKNOWN,
        },
        [EfxAttributeType.TypeModularBillboard] = new() {
            [EfxVersion.MHRise] = 209,
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

        [EfxAttributeType.UnknownDD2_231Clip] = new() {
            [EfxVersion.DD2] = 231,
        },

        [EfxAttributeType.UnknownDD2_232Expression] = new() {
            [EfxVersion.DD2] = 232,
        },

        [EfxAttributeType.UnknownRE4_228] = new() {
            [EfxVersion.RE4] = 228,
            [EfxVersion.DD2] = 241, // alternatively, RgbWater?
        },

        [EfxAttributeType.UnknownDD2_243] = new() { // PtFreezer?
            [EfxVersion.DD2] = 243,
        },

        [EfxAttributeType.AssignCSV] = new() {
            [EfxVersion.RE8] = 219,
        },
        [EfxAttributeType.UnknownRE4_231EfCsv] = new() {
            [EfxVersion.RE4] = 231,
            [EfxVersion.DD2] = 244,
        },
        [EfxAttributeType.UnknownDD2_245Efcsv] = new() {
            [EfxVersion.DD2] = 245,
        },

        [EfxAttributeType.UnknownDD2_247] = new() {
            [EfxVersion.DD2] = 247,
        },
        [EfxAttributeType.UnknownDD2_249] = new() {
            [EfxVersion.DD2] = 249,
        },
        [EfxAttributeType.UnknownDD2_250] = new() {
            [EfxVersion.DD2] = 250,
        },

#region TypeGpu
        [EfxAttributeType.TypeGpuBillboard] = new() {
            [EfxVersion.RE3] = 134,
            [EfxVersion.RE2] = 105,
            [EfxVersion.DMC5] = 105,
            [EfxVersion.RE7] = 85,
            [EfxVersion.MHRise] = 171,
            [EfxVersion.RE8] = 178,
            [EfxVersion.RERT] = 219, // testing
            [EfxVersion.MHRiseSB] = 221,
            [EfxVersion.RE4] = 243, // seems ok
            [EfxVersion.DD2] = 252, // seems ok
        },
        [EfxAttributeType.TypeGpuBillboardExpression] = new() {
            [EfxVersion.RE3] = 135,
            [EfxVersion.RE2] = 106,
            [EfxVersion.DMC5] = 106,
            [EfxVersion.MHRise] = 172,
            [EfxVersion.RE8] = 179,
            [EfxVersion.RERT] = 220,
            [EfxVersion.MHRiseSB] = 222,
            [EfxVersion.RE4] = 244, // testing
            [EfxVersion.DD2] = 253, // testing
        },
        [EfxAttributeType.TypeGpuPolygon] = new() {
            [EfxVersion.RE3] = 136,
            [EfxVersion.MHRise] = 173,
            [EfxVersion.RE8] = 180,
            [EfxVersion.RERT] = 221, // guess
            [EfxVersion.MHRiseSB] = 223,
            [EfxVersion.RE4] = 245, // seems ok
            [EfxVersion.DD2] = 254, // seems ok
        },
        [EfxAttributeType.TypeGpuPolygonExpression] = new() {
            [EfxVersion.RE8] = 181,
            [EfxVersion.RERT] = 222, // guess
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 255,
        },
        [EfxAttributeType.TypeGpuRibbonFollow] = new() {
            [EfxVersion.RE3] = 137,
            [EfxVersion.RE2] = 107,
            [EfxVersion.DMC5] = 107,
            [EfxVersion.MHRise] = 174,
            [EfxVersion.RE8] = 182,
            [EfxVersion.RERT] = 223, // testing
            // [EfxVersion.RERT] = 225,
            [EfxVersion.MHRiseSB] = 225,
            [EfxVersion.RE4] = 247, // seems fine
            [EfxVersion.DD2] = 256, // seems fine
        },
        [EfxAttributeType.TypeGpuRibbonFollowExpression] = new() {
            [EfxVersion.RE8] = 183,
            [EfxVersion.RERT] = 224, // guess
            [EfxVersion.RE4] = 248,
            [EfxVersion.DD2] = 257,
        },
        [EfxAttributeType.TypeGpuRibbonLength] = new() {
            [EfxVersion.RE3] = 138,
            [EfxVersion.MHRise] = 175,
            [EfxVersion.RE8] = 184,
            [EfxVersion.RERT] = 225, // guess
            [EfxVersion.MHRiseSB] = 227,
            [EfxVersion.RE4] = 249, // seems ok
            [EfxVersion.DD2] = 258, // seems ok
        },
        [EfxAttributeType.TypeGpuRibbonLengthExpression] = new() {
            [EfxVersion.RE8] = 185,
            [EfxVersion.RERT] = 226, // guess
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 259,
        },
        [EfxAttributeType.TypeGpuMesh] = new() {
            [EfxVersion.RE3] = 139,
            [EfxVersion.MHRise] = 176,
            [EfxVersion.RE8] = 186,
            [EfxVersion.RERT] = 227, // guess
            [EfxVersion.MHRiseSB] = 229,
            [EfxVersion.RE4] = 251,
            [EfxVersion.DD2] = 260,
        },
        [EfxAttributeType.TypeGpuMeshClip] = new() {
            [EfxVersion.DD2] = 261,
        },
        [EfxAttributeType.TypeGpuMeshExpression] = new() {
            [EfxVersion.RE3] = 140,
            [EfxVersion.MHRise] = 177,
            [EfxVersion.RE8] = 187,
            [EfxVersion.RERT] = 228, // guess
            [EfxVersion.MHRiseSB] = 230,
            [EfxVersion.RE4] = 252, // unused
            [EfxVersion.DD2] = 262,
        },
        [EfxAttributeType.TypeGpuMeshTrail] = new() {
            [EfxVersion.MHRise] = 178,
            [EfxVersion.RE8] = 188,
            [EfxVersion.RERT] = 229, // guess
            [EfxVersion.MHRiseSB] = 231,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 263, // guess
        },
        [EfxAttributeType.TypeGpuMeshTrailClip] = new() {
            [EfxVersion.MHRise] = 179,
            [EfxVersion.RE8] = 189,
            [EfxVersion.RERT] = 230, // guess
            [EfxVersion.MHRiseSB] = 232,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 264, // guess
        },
        [EfxAttributeType.TypeGpuMeshTrailExpression] = new() {
            [EfxVersion.MHRise] = 180,
            [EfxVersion.RE8] = 190,
            [EfxVersion.RERT] = 231, // guess
            [EfxVersion.MHRiseSB] = 233,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 265, // guess
        },
        [EfxAttributeType.UnknownRERT_233] = new() {
            [EfxVersion.RERT] = 233,
        },
        [EfxAttributeType.TypeGpuLightning3D] = new() {
            [EfxVersion.RE3] = 141,
            [EfxVersion.MHRise] = 181,
            [EfxVersion.RE8] = 191,
            [EfxVersion.MHRiseSB] = 234,
            [EfxVersion.RE4] = UNKNOWN,
            [EfxVersion.DD2] = 266,
        },
        #endregion

        // max item marker enum entry, not an actual object type
        [EfxAttributeType.ItemNum] = new() {
            [EfxVersion.RE7] = 87,
            [EfxVersion.RE2] = 117,
            [EfxVersion.DMC5] = 121,
            [EfxVersion.RE3] = 160,
            [EfxVersion.MHRise] = UNKNOWN,
            [EfxVersion.RE8] = 226,
            [EfxVersion.RERT] = UNKNOWN,
            [EfxVersion.MHRiseSB] = UNKNOWN,
            [EfxVersion.SF6] = UNKNOWN,
            [EfxVersion.RE4] = 253,
            [EfxVersion.DD2] = 267,
            [EfxVersion.MHWilds] = UNKNOWN,
        },
    };
}
