namespace ReeLib.Efx.Enums;
public enum AlphaCorrectionMode
{
    AlphaCorrectionMode_Disable = 0,
    AlphaCorrectionMode_AlphaCurve = 1,
    AlphaCorrectionMode_AlphaThreshold = 2,
    AlphaCorrectionMode_LifePower = 3,
}

public enum AxisType
{
    AxisType_PositiveX = 0,
    AxisType_PositiveY = 1,
    AxisType_PositiveZ = 2,
    AxisType_NegativeX = 3,
    AxisType_NegativeY = 4,
    AxisType_NegativeZ = 5,
}

public enum AxisXYZ
{
    AxisXYZ_X = 0,
    AxisXYZ_Y = 1,
    AxisXYZ_Z = 2,
}

public enum BlendType
{
    BlendType_AlphaBlend = 0,
    BlendType_Physical = 1,
    BlendType_AddContrast = 2,
    BlendType_EdgeBlend = 3,
    BlendType_Multiply = 4,
}

public enum CullingMode
{
    CullingMode_Draw = 0,
    CullingMode_DrawAndMove = 1,
}

public enum CullingTarget
{
    CullingTarget_Emitter = 0,
    CullingTarget_Particle = 1,
}

public enum CurveType
{
    CurveType_Clamp = 0,
    CurveType_Rescale = 1,
}

public enum DistanceCullingType
{
    DistanceCullingType_Default = 0,
    DistanceCullingType_Manual = 1,
    DistanceCullingType_Unlimited = 2,
}

public enum DivideType
{
    DivideType_Standard = 0,
    DivideType_Fractal = 1,
}

public enum EffectBoundsType
{
    EffectBoundsType_None = 0,
    EffectBoundsType_Sphere = 1,
    EffectBoundsType_AABB = 2,
    EffectBoundsType_OBB = 3,
}

public enum EmitType
{
    EmitType_RadialLegacy = 0,
    EmitType_Directional = 1,
    EmitType_Radial = 2,
}

public enum EmitterColorOperator
{
    EmitterColorOperator_Overwrite = 0,
    EmitterColorOperator_Multiply = 1,
}

public enum EmitterDimType
{
    EmitterDimType_2D = 0,
    EmitterDimType_3D = 1,
}

public enum ExternType
{
    ExternType_Float = 0,
    ExternType_Color = 1,
    ExternType_LimitedFloat = 2,
    ExternType_Bool = 3,
    ExternType_String = 4,
}

public enum ExpressionOperator
{
    ExpressionOperator_Addition = 0,
    ExpressionOperator_Subtraction = 1,
    ExpressionOperator_Multiplication = 2,
    ExpressionOperator_Division = 3,
    ExpressionOperator_Assign = 4,
    ExpressionOperator_ForceWord = -1
}

public enum FadeBlend
{
    FadeBlend_Max = 0,
    FadeBlend_Min = 1,
    FadeBlend_Mul = 2,
}

public enum FieldType
{
    FieldType_None = 0,
    FieldType_Vector = 1,
    FieldType_Volume = 2,
    FieldType_GlobalVectorOnly = 3,
}

public enum Finish
{
    Finish_None = 0,
    Finish_Kill = 1,
    Finish_Freeze = 2,
}

public enum FlipType
{
    FlipType_None = 0,
    FlipType_Flip = 1,
    FlipType_RandomFlip = 2,
}

public enum FrontDirectionType
{
    FrontDirectionType_ParallelCamera = 0,
    FrontDirectionType_ToCamera = 1,
}

public enum GroupType
{
    GroupType_Free = 0,
    GroupType_EffectCollision = 1,
    GroupType_UserVariable = 2,
    GroupType_RootUserVariable = 3,
    GroupType_Invalid = -1,
}

public enum ItemType
{
    ItemType_Unknown = 0,
    ItemType_FixRandomGenerator = 1,
    ItemType_EffectShader = 2,
    ItemType_VortexelHeatReceiver = 3,
    ItemType_Transform2D = 4,
    ItemType_Transform2DModifierDelayFrame = 5,
    ItemType_Transform2DModifier = 6,
    ItemType_Transform2DClip = 7,
    ItemType_Transform2DExpression = 8,
    ItemType_Transform3D = 9,
    ItemType_Transform3DModifierDelayFrame = 10,
    ItemType_Transform3DModifier = 11,
    ItemType_Transform3DClip = 12,
    ItemType_Transform3DExpression = 13,
    ItemType_ParentOptions = 14,
    ItemType_ParentOptionsExpression = 15,
    ItemType_Spawn = 16,
    ItemType_SpawnExpression = 17,
    ItemType_EmitterColor = 18,
    ItemType_EmitterColorClip = 19,
    ItemType_Layout = 20,
    ItemType_PtSort = 21,
    ItemType_TypeBillboard2D = 22,
    ItemType_TypeBillboard2DExpression = 23,
    ItemType_TypeBillboard3D = 24,
    ItemType_TypeBillboard3DExpression = 25,
    ItemType_TypeBillboard3DMaterial = 26,
    ItemType_TypeBillboard3DMaterialClip = 27,
    ItemType_TypeBillboard3DMaterialExpression = 28,
    ItemType_TypeMesh = 29,
    ItemType_TypeMeshClip = 30,
    ItemType_TypeMeshExpression = 31,
    ItemType_TypeRibbonFollow = 32,
    ItemType_TypeRibbonLength = 33,
    ItemType_TypeRibbonChain = 34,
    ItemType_TypeRibbonFixEnd = 35,
    ItemType_TypeRibbonLightweight = 36,
    ItemType_TypeRibbonParticle = 37,
    ItemType_TypeRibbonFollowMaterial = 38,
    ItemType_TypeRibbonFollowMaterialClip = 39,
    ItemType_TypeRibbonFollowMaterialExpression = 40,
    ItemType_TypeRibbonLengthMaterial = 41,
    ItemType_TypeRibbonLengthMaterialClip = 42,
    ItemType_TypeRibbonLengthMaterialExpression = 43,
    ItemType_TypeRibbonChainMaterial = 44,
    ItemType_TypeRibbonChainMaterialClip = 45,
    ItemType_TypeRibbonChainMaterialExpression = 46,
    ItemType_TypeRibbonFixEndMaterial = 47,
    ItemType_TypeRibbonFixEndMaterialClip = 48,
    ItemType_TypeRibbonFixEndMaterialExpression = 49,
    ItemType_TypeRibbonLightweightMaterial = 50,
    ItemType_TypeRibbonLightweightMaterialClip = 51,
    ItemType_TypeRibbonLightweightMaterialExpression = 52,
    ItemType_TypeStrainRibbonMaterial = 53,
    ItemType_TypeStrainRibbonMaterialClip = 54,
    ItemType_TypeStrainRibbonMaterialExpression = 55,
    ItemType_TypeRibbonParticleMaterial = 56,
    ItemType_TypeRibbonParticleMaterialClip = 57,
    ItemType_TypeRibbonParticleMaterialExpression = 58,
    ItemType_TypeRibbonFollowExpression = 59,
    ItemType_TypeRibbonLengthExpression = 60,
    ItemType_TypeRibbonChainExpression = 61,
    ItemType_TypeRibbonFixEndExpression = 62,
    ItemType_TypeRibbonLightweightExpression = 63,
    ItemType_TypeRibbonParticleExpression = 64,
    ItemType_TypePolygon = 65,
    ItemType_TypePolygonClip = 66,
    ItemType_TypePolygonExpression = 67,
    ItemType_TypePolygonMaterial = 68,
    ItemType_TypePolygonMaterialClip = 69,
    ItemType_TypePolygonMaterialExpression = 70,
    ItemType_TypeRibbonTrail = 71,
    ItemType_TypePolygonTrail = 72,
    ItemType_TypePolygonTrailExpression = 73,
    ItemType_TypePolygonTrailMaterial = 74,
    ItemType_TypePolygonTrailMaterialClip = 75,
    ItemType_TypePolygonTrailMaterialExpression = 76,
    ItemType_TypeNoDraw = 77,
    ItemType_TypeNoDrawExpression = 78,
    ItemType_Velocity2DDelayFrame = 79,
    ItemType_Velocity2D = 80,
    ItemType_Velocity2DExpression = 81,
    ItemType_Velocity3DDelayFrame = 82,
    ItemType_Velocity3D = 83,
    ItemType_Velocity3DExpression = 84,
    ItemType_RotateAnimDelayFrame = 85,
    ItemType_RotateAnim = 86,
    ItemType_RotateAnimExpression = 87,
    ItemType_ScaleAnimDelayFrame = 88,
    ItemType_ScaleAnim = 89,
    ItemType_ScaleAnimExpression = 90,
    ItemType_VanishArea3D = 91,
    ItemType_VanishArea3DExpression = 92,
    ItemType_Life = 93,
    ItemType_LifeExpression = 94,
    ItemType_UVSequence = 95,
    ItemType_UVSequenceModifier = 96,
    ItemType_UVSequenceExpression = 97,
    ItemType_UVScroll = 98,
    ItemType_TextureUnit = 99,
    ItemType_TextureUnitExpression = 100,
    ItemType_TextureFilter = 101,
    ItemType_EmitterShape2D = 102,
    ItemType_EmitterShape2DExpression = 103,
    ItemType_EmitterShape3D = 104,
    ItemType_EmitterShape3DExpression = 105,
    ItemType_AlphaCorrection = 106,
    ItemType_ContrastHighlighter = 107,
    ItemType_ColorGrading = 108,
    ItemType_ColorGradingExpression = 109,
    ItemType_Blink = 110,
    ItemType_Noise = 111,
    ItemType_NoiseExpression = 112,
    ItemType_TexelChannelOperator = 113,
    ItemType_TexelChannelOperatorClip = 114,
    ItemType_TexelChannelOperatorExpression = 115,
    ItemType_TypeStrainRibbon = 116,
    ItemType_TypeStrainRibbonExpression = 117,
    ItemType_TypeLightning3D = 118,
    ItemType_TypeLightning3DExpression = 119,
    ItemType_TypeLightning3DMaterial = 120,
    ItemType_TypeLightning3DMaterialClip = 121,
    ItemType_TypeLightning3DMaterialExpression = 122,
    ItemType_TypeLightningExpensive = 123,
    ItemType_PtLightningColliderAction = 124,
    ItemType_PtLightningBranchAction = 125,
    ItemType_ShaderSettings = 126,
    ItemType_ShaderSettingsExpression = 127,
    ItemType_Distortion = 128,
    ItemType_DistortionExpression = 129,
    ItemType_VolumetricLighting = 130,
    ItemType_RenderTarget = 131,
    ItemType_DrawSubscene = 132,
    ItemType_UnitCulling = 133,
    ItemType_UnitCullingExpression = 134,
    ItemType_PtLife = 135,
    ItemType_PtBehavior = 136,
    ItemType_PtBehaviorClip = 137,
    ItemType_PlayEfx = 138,
    ItemType_FadeByAngle = 139,
    ItemType_FadeByAngleExpression = 140,
    ItemType_FadeByEmitterAngle = 141,
    ItemType_FadeByDepth = 142,
    ItemType_FadeByDepthExpression = 143,
    ItemType_FadeByOcclusion = 144,
    ItemType_FadeByOcclusionExpression = 145,
    ItemType_FadeByRootCulling = 146,
    ItemType_FakeDoF = 147,
    ItemType_LuminanceBleed = 148,
    ItemType_ScaleByDepth = 149,
    ItemType_TypeNodeBillboard = 150,
    ItemType_TypeNodeBillboardExpression = 151,
    ItemType_FluidEmitter2D = 152,
    ItemType_FluidEmitter2DClip = 153,
    ItemType_FluidEmitter2DExpression = 154,
    ItemType_FluidSimulator2D = 155,
    ItemType_FluidParticle2DSimulator = 156,
    ItemType_PlayEmitter = 157,
    ItemType_PtTransform3D = 158,
    ItemType_PtTransform3DClip = 159,
    ItemType_PtTransform3DExpression = 160,
    ItemType_PtTransform2D = 161,
    ItemType_PtTransform2DClip = 162,
    ItemType_PtVelocity3D = 163,
    ItemType_PtVelocity3DClip = 164,
    ItemType_PtVelocity2D = 165,
    ItemType_PtVelocity2DClip = 166,
    ItemType_PtColliderAction = 167,
    ItemType_PtCollision = 168,
    ItemType_PtCollisionInfluence = 169,
    ItemType_PtColor = 170,
    ItemType_PtColorClip = 171,
    ItemType_PtColorMixer = 172,
    ItemType_PtColorMixerClip = 173,
    ItemType_PtUvSequence = 174,
    ItemType_PtUvSequenceClip = 175,
    ItemType_MeshEmitter = 176,
    ItemType_MeshEmitterClip = 177,
    ItemType_MeshEmitterExpression = 178,
    ItemType_ScreenSpaceEmitter = 179,
    ItemType_Listener = 180,
    ItemType_VectorFieldParameter = 181,
    ItemType_VectorFieldParameterClip = 182,
    ItemType_VectorFieldParameterExpression = 183,
    ItemType_GlobalVectorField = 184,
    ItemType_GlobalVectorFieldClip = 185,
    ItemType_GlobalVectorFieldExpression = 186,
    ItemType_DirectionalFieldParameter = 187,
    ItemType_DirectionalFieldParameterClip = 188,
    ItemType_DirectionalFieldParameterExpression = 189,
    ItemType_DepthOperator = 190,
    ItemType_PlaneCollider = 191,
    ItemType_PlaneColliderExpression = 192,
    ItemType_DepthOcclusion = 193,
    ItemType_ShapeOperator = 194,
    ItemType_ShapeOperatorExpression = 195,
    ItemType_WindInfluence3DDelayFrame = 196,
    ItemType_WindInfluence3D = 197,
    ItemType_Attractor = 198,
    ItemType_AttractorClip = 199,
    ItemType_AttractorExpression = 200,
    ItemType_PtVortexelWind = 201,
    ItemType_PtVortexelWindExpression = 202,
    ItemType_VortexelWindEmitter = 203,
    ItemType_VortexelWindEmitterExpression = 204,
    ItemType_PtVortexelHeatSource = 205,
    ItemType_VortexelCollider = 206,
    ItemType_VortexelColliderExpression = 207,
    ItemType_VortexelIndoorMask = 208,
    ItemType_VortexelIndoorMaskExpression = 209,
    ItemType_PtVortexelPhysics = 210,
    ItemType_PtVortexelPhysicsSimple = 211,
    ItemType_PtVortexelSnap = 212,
    ItemType_EmitterPriority = 213,
    ItemType_DrawOverlay = 214,
    ItemType_VectorField = 215,
    ItemType_VolumeField = 216,
    ItemType_DirectionalField = 217,
    ItemType_AngularVelocity3DDelayFrame = 218,
    ItemType_AngularVelocity3D = 219,
    ItemType_PtAngularVelocity3D = 220,
    ItemType_PtAngularVelocity3DExpression = 221,
    ItemType_AngularVelocity2DDelayFrame = 222,
    ItemType_AngularVelocity2D = 223,
    ItemType_PtAngularVelocity2D = 224,
    ItemType_PtAngularVelocity2DExpression = 225,
    ItemType_PtPathTranslate = 226,
    ItemType_IgnorePlayerColor = 227,
    ItemType_IgnoreSettings = 228,
    ItemType_ProceduralDistortionDelayFrame = 229,
    ItemType_ProceduralDistortion = 230,
    ItemType_ProceduralDistortionClip = 231,
    ItemType_ProceduralDistortionExpression = 232,
    ItemType_TestBehaviorUpdater = 233,
    ItemType_StretchBlur = 234,
    ItemType_StretchBlurExpression = 235,
    ItemType_EmitterHSV = 236,
    ItemType_EmitterHSVExpression = 237,
    ItemType_FlowMap = 238,
    ItemType_RgbCommon = 239,
    ItemType_RgbCommonExpression = 240,
    ItemType_RgbWater = 241,
    ItemType_RgbWaterExpression = 242,
    ItemType_PtFreezer = 243,
    ItemType_AssignCSV = 244,
    ItemType_DestinationCSV = 245,
    ItemType_GpuPhysics = 246,
    ItemType_TerrainSnap = 247,
    ItemType_RepeatArea = 248,
    ItemType_EmitMask = 249,
    ItemType_ParticleTimer = 250,
    ItemType_CustomComputeShader = 251,
    ItemType_TypeGpuBillboard = 252,
    ItemType_TypeGpuBillboardExpression = 253,
    ItemType_TypeGpuPolygon = 254,
    ItemType_TypeGpuPolygonExpression = 255,
    ItemType_TypeGpuRibbonFollow = 256,
    ItemType_TypeGpuRibbonFollowExpression = 257,
    ItemType_TypeGpuRibbonLength = 258,
    ItemType_TypeGpuRibbonLengthExpression = 259,
    ItemType_TypeGpuMesh = 260,
    ItemType_TypeGpuMeshClip = 261,
    ItemType_TypeGpuMeshExpression = 262,
    ItemType_TypeGpuMeshTrail = 263,
    ItemType_TypeGpuMeshTrailClip = 264,
    ItemType_TypeGpuMeshTrailExpression = 265,
    ItemType_TypeGpuLightning3D = 266,
    ItemType_RayTracingTarget = 267,
    ItemType_PointCloudEmitter = 268,
    ItemType_DumpGpuParticle = 269,
    ItemType_RibbonJointOffsetVelocity = 270,
    ItemType_RibbonJointScale = 271,
    ItemType_FluidParticleEmitter = 272,
    ItemType_FluidParticleEmitterTarget = 273,
    ItemType_ItemNum = 274,
}

public enum LifeType
{
    LifeType_None = 0,
    LifeType_AppearOnly = 1,
    LifeType_SyncKeepHold = 2,
    LifeType_KeepHold = 3,
    LifeType_FinishKeep = 4,
}

public enum LightingType
{
    LightingType_None = 0,
    LightingType_Vertex = 1,
    LightingType_Vertex2x2 = 2,
    LightingType_Vertex4x4 = 3,
    LightingType_Vertex8x8 = 4,
    LightingType_PerPixel = 5,
    LightingType_ForceWord = -1,
}

public enum LightningType
{
    LightningType_Hit = 0,
    LightningType_Strike = 1,
    LightningType_Both = 2,
}

public enum LuminanceBleedSamplingType
{
    LuminanceBleedSamplingType_Default = 0,
    LuminanceBleedSamplingType_NoSubpixel = 1,
}

public enum LuminanceBleedType
{
    LuminanceBleedType_None = 0,
    LuminanceBleedType_Transparent = 1,
    LuminanceBleedType_PostTransparent = 2,
}

public enum MaterialShadingType
{
    MaterialShadingType_Standard = 0,
    MaterialShadingType_Decal = 1,
    MaterialShadingType_SeparateAlphaDecal = 2,
    MaterialShadingType_DecalWithMetallic = 3,
    MaterialShadingType_DecalNRMR = 4,
    MaterialShadingType_Transparent = 5,
    MaterialShadingType_Distortion = 6,
    MaterialShadingType_PrimitiveMesh = 7,
    MaterialShadingType_PrimitiveSolidMesh = 8,
    MaterialShadingType_Water = 9,
    MaterialShadingType_SpeedTree = 10,
    MaterialShadingType_GUI = 11,
    MaterialShadingType_GUIMesh = 12,
    MaterialShadingType_GUIMeshTransparent = 13,
    MaterialShadingType_ExpensiveTransparent = 14,
    MaterialShadingType_Forward = 15,
    MaterialShadingType_RenderTarget = 16,
    MaterialShadingType_PostProcess = 17,
    MaterialShadingType_PrimitiveMaterial = 18,
    MaterialShadingType_PrimitiveSolidMaterial = 19,
    MaterialShadingType_PrimitiveSolidMaterialExpensive = 20,
    MaterialShadingType_SpineMaterial = 21,
    MaterialShadingType_VolumetricFog = 22,
    MaterialShadingType_ShellFurMaterial = 23,
    MaterialShadingType_VolumeDecal = 24,
    MaterialShadingType_AlembicMesh = 25,
    MaterialShadingType_AlembicMeshForward = 26,
    MaterialShadingType_AlembicMeshTransparent = 27,
    MaterialShadingType_MarchingCubes = 28,
    MaterialShadingType_MarchingCubesForward = 29,
    MaterialShadingType_MarchingCubesTransparent = 30,
    MaterialShadingType_Strands = 31,
    MaterialShadingType_NFXTransparent = 32,
    MaterialShadingType_Cloudscape2 = 33,
    MaterialShadingType_CloudscapeReserved = 34,
    MaterialShadingType_VolumeSolidMaterial = 35,
    MaterialShadingType_Max = 36,
}

public enum MaterialType
{
    MaterialType_Default = 0,
    MaterialType_VertexAnimation = 1,
}

public enum MieLightingType
{
    MieLightingType_None = 0,
    MieLightingType_Vertex = 1,
    MieLightingType_Vertex2x2 = 2,
    MieLightingType_Vertex4x4 = 3,
    MieLightingType_Vertex8x8 = 4,
    MieLightingType_Vertex16x16 = 5,
    MieLightingType_Vertex32x32 = 6,
    MieLightingType_ForceWord = -1,
}

public enum NodeBillboardShapeType
{
    NodeBillboardShapeType_Box = 0,
    NodeBillboardShapeType_Sphere = 1,
    NodeBillboardShapeType_Num = 2,
}

public enum NodeBillboardType
{
    NodeBillboardType_Bezier = 0,
    NodeBillboardType_Spline = 1,
    NodeBillboardType_Num = 2,
}

public enum NormalFromEmitterType
{
    NormalFromEmitterType_None = 0,
    NormalFromEmitterType_Sphere = 1,
    NormalFromEmitterType_SphereInitOnly = 2,
    NormalFromEmitterType_Cylinder = 3,
    NormalFromEmitterType_CylinderInitOnly = 4,
}

public enum Operator
{
    Operator_EmitOnSurface = 0,
    Operator_BounceOnSurface = 1,
    Operator_StickToSurface = 2,
    Operator_StickToSurfaceHighQuality = 3,
    Operator_EmitOnSurfaceHighQuality = 4,
}

public enum ParentConstraint
{
    ParentConstraint_None = 0,
    ParentConstraint_Follow = 1,
    ParentConstraint_Initialization = 2,
    ParentConstraint_ForceWord = -1,
}

public enum PathTranslateType
{
    PathTranslateType_Loop = 0,
    PathTranslateType_Freeze = 1,
    PathTranslateType_Turn = 2,
    PathTranslateType_Through = 3,
    PathTranslateType_FinishFreeze = 4,
    PathTranslateType_FinishThrough = 5,
    PathTranslateType_Kill = 6,
}

public enum PlayOrder
{
    PlayOrder_Forward = 0,
    PlayOrder_Reverse = 1,
    PlayOrder_RandomReverse = 2,
}

public enum PlayType
{
    PlayType_Pause = 0,
    PlayType_Loop = 1,
    PlayType_Finish = 2,
    PlayType_Play = 3,
}

public enum PtColorOperator
{
    PtColorOperator_Overwrite = 0,
    PtColorOperator_Multiply = 1,
}

public enum PtLifeStatus
{
    PtLifeStatus_Initialize = 0,
    PtLifeStatus_Appear = 1,
    PtLifeStatus_Keep = 2,
    PtLifeStatus_Vanish = 3,
    PtLifeStatus_Terminate = 4,
    PtLifeStatus_Unknown = -1,
}

public enum Repeat
{
    Repeat_None = 0,
    Repeat_U = 1,
    Repeat_V = 2,
    Repeat_UV = 3,
}

public enum Rotate90
{
    Rotate90_None = 0,
    Rotate90_Rotate = 1,
    Rotate90_RandomRotate = 2,
}

public enum RotationCorrectType
{
    RotationCorrectType_None = 0,
    RotationCorrectType_ParallelCamera = 1,
    RotationCorrectType_ParallelCameraAxisY = 2,
    RotationCorrectType_ToCamera = 3,
    RotationCorrectType_ToCameraAxisY = 4,
}

public enum RotationOrder
{
    RotationOrder_XYZ = 0,
    RotationOrder_YZX = 1,
    RotationOrder_ZXY = 2,
    RotationOrder_ZYX = 3,
    RotationOrder_YXZ = 4,
    RotationOrder_XZY = 5,
}

public enum SamplingAxisTarget
{
    SamplingAxisTarget_Emitter = 0,
    SamplingAxisTarget_Camera = 1,
}

public enum SamplingPositionType
{
    SamplingPositionType_Emitter = 0,
    SamplingPositionType_Joint = 1,
    SamplingPositionType_Particle = 2,
}

public enum ShadowCastMode
{
    ShadowCastMode_A = 1,
    ShadowCastMode_B = 2,
}

public enum ShadowType
{
    ShadowType_None = 0,
    ShadowType_Enable = 1,
    ShadowType_ShadowOnly = 2,
    ShadowType_Translucent = 3,
    ShadowType_TranslucentShadowOnly = 4,
}

public enum Shape2DType
{
    Shape2DType_Square = 0,
    Shape2DType_Circle = 1,
}

public enum Shape3DType
{
    Shape3DType_Box = 0,
    Shape3DType_Sphere = 1,
    Shape3DType_Cylinder = 2,
}

public enum ShapeType
{
    ShapeType_Line = 0,
    ShapeType_Ring = 1,
}

public enum SolidBodyShapeType
{
    SolidBodyShapeType_Sphere = 0,
    SolidBodyShapeType_Box = 1,
    SolidBodyShapeType_Card = 2,
    SolidBodyShapeType_Tetrahedron = 3,
    SolidBodyShapeType_Capsule = 4,
}

public enum SortType
{
    SortType_ByPosition = 0,
    SortType_ByAlpha = 1,
    SortType_None = 2,
}

public enum SourceSinkType
{
    SourceSinkType_Circle = 0,
    SourceSinkType_Rectangle = 1,
}

public enum SpinType
{
    SpinType_Reset = 0,
    SpinType_Reverse = 1,
}

public enum TerrainSnapType
{
    TerrainSnapType_UnderSnap = 0,
    TerrainSnapType_AlwaysSnap = 1,
}

public enum TextureType
{
    TextureType_IBL = 0,
    TextureType_CubeMap = 1,
    TextureType_LocalCubeMap = 2,
    TextureType_TransparentHDR = 3,
    TextureType_Max = 4,
    TextureType_Untyped = -1,
}

public enum Topology
{
    Topology_PointList = 1,
    Topology_LineList = 2,
    Topology_LineStrip = 3,
    Topology_TriangleList = 4,
    Topology_TriangleStrip = 5,
    Topology_LineListAdj = 6,
    Topology_LineStripAdj = 7,
    Topology_TriangleListAdj = 8,
    Topology_TriangleStripAdj = 9,
    Topology_PatchList_ControlPoint1 = 10,
    Topology_PatchList_ControlPoint2 = 11,
    Topology_PatchList_ControlPoint3 = 12,
    Topology_PatchList_ControlPoint4 = 13,
    Topology_PatchList_ControlPoint5 = 14,
    Topology_PatchList_ControlPoint6 = 15,
    Topology_PatchList_ControlPoint7 = 16,
    Topology_PatchList_ControlPoint8 = 17,
    Topology_PatchList_ControlPoint9 = 18,
    Topology_PatchList_ControlPoint10 = 19,
    Topology_PatchList_ControlPoint11 = 20,
    Topology_PatchList_ControlPoint12 = 21,
    Topology_PatchList_ControlPoint13 = 22,
    Topology_PatchList_ControlPoint14 = 23,
    Topology_PatchList_ControlPoint15 = 24,
    Topology_PatchList_ControlPoint16 = 25,
    Topology_PatchList_ControlPoint17 = 26,
    Topology_PatchList_ControlPoint18 = 27,
    Topology_PatchList_ControlPoint19 = 28,
    Topology_PatchList_ControlPoint20 = 29,
    Topology_PatchList_ControlPoint21 = 30,
    Topology_PatchList_ControlPoint22 = 31,
    Topology_PatchList_ControlPoint23 = 32,
    Topology_PatchList_ControlPoint24 = 33,
    Topology_PatchList_ControlPoint25 = 34,
    Topology_PatchList_ControlPoint26 = 35,
    Topology_PatchList_ControlPoint27 = 36,
    Topology_PatchList_ControlPoint28 = 37,
    Topology_PatchList_ControlPoint29 = 38,
    Topology_PatchList_ControlPoint30 = 39,
    Topology_PatchList_ControlPoint31 = 40,
    Topology_PatchList_ControlPoint32 = 41,
}

public enum VanishEvent
{
    VanishEvent_SelfTimer = 0,
    VanishEvent_KeepHold = 1,
    VanishEvent_FinishKeep = 2,
}

public enum VelocityAttenuationType
{
    VelocityAttenuationType_Unknown = 0,
}

public enum VelocityDirectionType
{
    VelocityDirectionType_Unknown = 0,
}

public enum VelocityEmitType
{
    VelocityEmitType_Unknown = 0,
}

public enum VelocityShapeType
{
    VelocityShapeType_Unknown = 0,
}

public enum VelocityType
{
    VelocityType_Direction = 0,
    VelocityType_Normal = 1,
    VelocityType_Radial = 2,
    VelocityType_Spread = 3,
    VelocityType_ScreenSpace = 4,
    VelocityType_Max = 5,
}

public enum VisualType
{
    VisualType_Fixed = 0,
    VisualType_LifeLinkage = 1,
    VisualType_LifeLinkageFixAlpha = 2,
}

public enum VortexelColliderShapeType
{
    VortexelColliderShapeType_Box = 0,
    VortexelColliderShapeType_Sphere = 1,
    VortexelColliderShapeType_Capsule = 2,
}

public enum VortexelIndoorMaskShapeType
{
    VortexelIndoorMaskShapeType_Box = 0,
    VortexelIndoorMaskShapeType_Sphere = 1,
    VortexelIndoorMaskShapeType_Capsule = 2,
}

