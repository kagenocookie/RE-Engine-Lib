using System.Numerics;
using RszTool;
using RszTool.Efx.Structs.RE4;
using RszTool.Efx.Structs.RERT;

namespace RszTool.Efx.Structs.DMC5;

public struct mid_cc {
    public via.Color color1;
    public via.Color color2;
	public float   ukn_float;
}

public struct large_cc {
	public via.Color   color;
	public via.Color   color2;
	public via.Color   color3;
	public float   Brightness;
}

enum Cycle_Loop {
    LOOP = -1,
    NOT_LOOP = 2
}

enum Animation_Mode : uint {
    starting_frame_only = 0,
	looped_animation = 1,
	animate_once = 2,
	animate_once_and_stay_on_last_untill_2C = 3,

	starting_frame_only_flipped_vertically = 4,
	looped_animation_flipped_vertically = 5,
	animate_once_flipped_vertically = 6,
	animate_once_and_stay_on_last_untill_2C_flipped_vertically = 7,

	starting_frame_only_random_flipped_vertically = 8,
	looped_animation_random_flipped_vertically = 9,
	animate_once_random_flipped_vertically = 10,
	animate_once_and_stay_on_last_untill_2C_random_flipped_vertically = 11,

	starting_frame_only_dup = 12,
	looped_animation_dup = 13,
	animate_once_dup = 14,
	animate_once_and_stay_on_last_untill_2C_dup = 15,

	starting_frame_only_flipped_horizontally = 16,
	looped_animation_flipped_horizontally = 17,
	animate_once_flipped_horizontally = 18,
	animate_once_and_stay_on_last_untill_2C_flipped_horizontally = 19,

	starting_frame_only_rotated_180 = 20,
	looped_animation_rotated_180 = 21,
	animate_once_rotated_180 = 22,
	animate_once_and_stay_on_last_untill_2C_rotated_180 = 23,

	starting_frame_only_random_rotated_180 = 24,
	looped_animation_random_rotated_180 = 25,
	animate_once_random_rotated_180 = 26,
	animate_once_and_stay_on_last_untill_2C_random_rotated_180 = 27,

	starting_frame_only_flipped_horizontally_dup = 28,
	looped_animation_flipped_horizontally_dup = 29,
	animate_once_flipped_horizontally_dup = 30,
	animate_once_and_stay_on_last_untill_2C_flipped_horizontally_dup = 31,

	starting_frame_only_random_flipped_horizontally = 32,
	looped_animation_random_flipped_horizontally = 33,
	animate_once_random_flipped_horizontally = 34,
	animate_once_and_stay_on_last_untill_2C_random_flipped_horizontally = 35,

	starting_frame_only_flipped_vertically_random_flipped_horizontally = 36,
	looped_animation_flipped_vertically_random_flipped_horizontally = 37,
	animate_once_flipped_vertically_random_flipped_horizontally = 38,
	animate_once_and_stay_on_last_untill_2C_flipped_vertically_random_flipped_horizontally = 39
}

public enum MaterialParameterTypeDmc5 : short
{
	None = 0,
	Float = 1,
	FloatRange = 2,
	Integer = 3,
}

[RszGenerate, RszAutoReadWrite]
public partial class MdfProperty_DMC5 : BaseModel
{
	public uint PropertyNameUTF8Hash;
	public ushort parameterCount;
	public MaterialParameterTypeDmc5 parameterType;
    // 4 + 1 = 4 floats
    // 1 + 1 = 2 floats
    // 1 + 2 = 2 floats
    // 1 + 3 = 2 ints or 1 int
	public int ukn1;

	[RszFixedSizeArray(16)] public byte[]? propertyValue;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeMesh, EfxVersion.RE7, EfxVersion.DMC5)]
public partial class EFXAttributeTypeMesh : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeMesh() : base(EfxAttributeType.TypeMesh) { }

    uint ukn1;
    uint overriddenHashCount;
    mid_cc mid_color;
    [RszFixedSizeArray(8)] uint[]? ukn2;
    float rotationXMin;
    float rotationXMax;
    float rotationYMin;
    float rotationYMax;
    float rotationZMin;
    float rotationZMax;
    float scaleXMin;
    float scaleXMax;
    float scaleYMin;
    float scaleYMax;
    float scaleZMin;
    float scaleZMax;
    float scaleMultiplierMin;
    float scaleMultiplierMax;
    [RszConditional(nameof(Version), '>', EfxVersion.RE7, EndAt = nameof(texCount))]
    uint ukn3;
    uint ukn4;
    public uint texCount;
    public uint meshPathLength;
    public uint mdfPathLength;
    public uint texPathBlockLength;
    [RszClassInstance, RszList(nameof(overriddenHashCount))] public List<MdfProperty_DMC5> properties = new();
    [RszInlineWString(nameof(meshPathLength))] public string? meshPath;
    [RszInlineWString(nameof(mdfPathLength))] public string? mdfPath;

    // [RszInlineWString(nameof(texPathLength)), RszConditional(nameof(texPathLength), '>', 0)] public string? tex;
    [RszConditional(nameof(Version), '>', EfxVersion.RE7)]
	[RszInlineWString, RszList(nameof(texCount))] public string[]? texPaths;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFollow, EfxVersion.DMC5)]
public partial class EFXAttributeTypeRibbonFollow : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeRibbonFollow() : base(EfxAttributeType.TypeRibbonFollow) { }

    uint ukn1;
    mid_cc mid_color;

    [RszFixedSizeArray(2)] float[]? ukn3;
    float scaleXMin; //doubt
    float scaleXMax; //doubt
    float scaleYMin; //doubt
    float scaleYMax; //doubt
    float scaleZMin; //doubt
    float scaleZMax; //doubt

    uint ukn4;
    float ukn5;
    [RszFixedSizeArray(2)] uint[]? ukn6;
    large_cc largel_color;
    [RszFixedSizeArray(6)] float[]? ukn7;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtCollision, EfxVersion.DMC5)]
public partial class EFXAttributePtCollision : RszTool.Efx.EFXAttribute
{
    public EFXAttributePtCollision() : base(EfxAttributeType.PtCollision) { }

    uint ukn00;
    float  ukn0;
    float  ukn1;
    [RszFixedSizeArray(2)] uint[]? ukn2;
    float  ukn3;
    float  ukn4;
    float  ukn5;
    float  ukn6;

    [RszFixedSizeArray(2)] uint[]? ukn7;
    float  ukn8;
    [RszFixedSizeArray(2)] uint[]? ukn9;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform3DModifier, EfxVersion.DMC5)]
public partial class EFXAttributeTransform3DModifier : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTransform3DModifier() : base(EfxAttributeType.Transform3DModifier) { }

    [RszFixedSizeArray(55)] public uint[]? ukn;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtColorClip, EfxVersion.DMC5)]
public partial class EFXAttributePtColorClip : RszTool.Efx.EFXAttribute
{
    public EFXAttributePtColorClip() : base(EfxAttributeType.PtColorClip) { }

    uint secChunkID;
    uint ukn2;
    Cycle_Loop loop;
    float  loopTime;
    [RszFixedSizeArray(3)] uint[]? ukn3;

    uint firstChunkSize;
    uint secondChunkSize;
    uint thirdChunkSize;
    [RszFixedSizeArray(nameof(firstChunkSize), '/', 4)] public uint[]? substruct1; // colorBitCount + intCount, apparently
    [RszFixedSizeArray(nameof(secondChunkSize), '/', 4)] public int[]? substruct2; // TODO very varying content
    [RszFixedSizeArray(nameof(thirdChunkSize), '/', 4)] public uint[]? substruct3;

    // private void Read() {
    //     if(0 < secondChunkSize){
    //         backPoseSC = FTell();
    //         switch (secChunkID) {
    //             case 0x0000000F:
    //                 struct{
    //                     for(e = 0; e < 4; e++){
    //                         switch(e+1){
    //                             case 1:
    //                                 for(c5it = 0; c5it < colorBitCount_arr[e]; c5it++){
    //                                     struct{
    //                                         float appearTime;
    //                                         uint ukn;
    //                                         if( colorBitCount_arr[3] > c5it){
    //                                             redPos[red] = FTell();
    //                                             red++;
    //                                         } else {
    //                                             alphaLessRedPos[alphaLessRed] = FTell();
    //                                             alphaLessRed++;
    //                                         }
    //                                         byte red;
    //                                         [RszFixedSizeArray(3)] byte[]? ukn2;
    //                                     } redSt;
    //                                 }
    //                                 break;

    //                             case 2:
    //                                 for(c5it = 0; c5it < colorBitCount_arr[e]; c5it++){
    //                                     struct{
    //                                         float appearTime;
    //                                         uint ukn;
    //                                         if( colorBitCount_arr[3] > c5it){
    //                                             greenPos[green] = FTell();
    //                                             green++;
    //                                         } else {
    //                                             alphaLessGreenPos[alphaLessGreen] = FTell();
    //                                             alphaLessGreen++;
    //                                         }
    //                                         byte green;
    //                                         [RszFixedSizeArray(3)] byte[]? ukn2;
    //                                     } greenSt;
    //                                 }
    //                                 break;

    //                             case 3:
    //                                 for(c5it = 0; c5it < colorBitCount_arr[e]; c5it++){
    //                                     struct{
    //                                         float appearTime;
    //                                         uint ukn;
    //                                         if( colorBitCount_arr[3] > c5it){
    //                                             bluePos[blue] = FTell();
    //                                             blue++;
    //                                         } else {
    //                                             alphaLessBluePos[alphaLessBlue] = FTell();
    //                                             alphaLessBlue++;
    //                                         }
    //                                         byte blue;
    //                                         [RszFixedSizeArray(3)] byte[]? ukn2;
    //                                     } blueSt;
    //                                 }
    //                                 break;

    //                             case 4:
    //                                 for(c5it = 0; c5it < colorBitCount_arr[e]; c5it++){
    //                                     struct{
    //                                         float appearTime;
    //                                         uint ukn;
    //                                         alphaPos[alpha] = FTell();
    //                                         alpha++;
    //                                         byte alpha;
    //                                         [RszFixedSizeArray(3)] byte[]? ukn2;
    //                                     } alphaSt;
    //                                 }
    //                                 break;
    //                             default:
    //                                 break;
    //                         }
    //                     }
    //                 }secondChunk;
    //                 break;
    //             case 0x00000008:
    //                 for(c5it = 0; c5it < colorBitCount_arr[0]; c5it++){
    //                     struct{
    //                         float appearTime;
    //                         uint ukn;
    //                         onlyAlphaPos[onlyAlpha]= FTell();
    //                         onlyAlpha++;
    //                         byte alpha;
    //                         [RszFixedSizeArray(3)] byte[]? ukn2;
    //                     } alphaSt;
    //                 }
    //                 break;
    //             case 0x00000007:
    //             case 0x0000000E:
    //                 struct{
    //                     for(e = 0; e < 3; e++){
    //                         switch(e+1){
    //                             case 1:
    //                                 for(c5it = 0; c5it < colorBitCount_arr[e]; c5it++){
    //                                     struct{
    //                                         float appearTime;
    //                                         uint ukn;
    //                                         alphaLessRedPos[alphaLessRed] = FTell();
    //                                         alphaLessRed++;
    //                                         byte red;
    //                                         [RszFixedSizeArray(3)] byte[]? ukn2;
    //                                     } redSt;
    //                                 }
    //                                 break;

    //                             case 2:
    //                                 for(c5it = 0; c5it < colorBitCount_arr[e]; c5it++){
    //                                     struct{
    //                                         float appearTime;
    //                                         uint ukn;
    //                                         alphaLessGreenPos[alphaLessGreen] = FTell();
    //                                         alphaLessGreen++;
    //                                         byte green;
    //                                         [RszFixedSizeArray(3)] byte[]? ukn2;
    //                                     } greenSt;
    //                                 }
    //                                 break;

    //                             case 3:
    //                                 for(c5it = 0; c5it < colorBitCount_arr[e]; c5it++){
    //                                     struct{
    //                                         float appearTime;
    //                                         uint ukn;
    //                                         alphaLessBluePos[alphaLessBlue] = FTell();
    //                                         alphaLessBlue++;
    //                                         byte blue;
    //                                         [RszFixedSizeArray(3)] byte[]? ukn2;
    //                                     } blueSt;
    //                                 }
    //                                 break;

    //                             default:
    //                                 break;
    //                         }
    //                     }
    //                 }secondChunk;
    //                 break;
    //             default:
    //                 struct{
    //                     uint ukn[secondChunkSize/4];
    //                 } secondChunk;
    //                 break;
    //         }
    //     }
    // }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ParentOptions, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5)]
public partial class EFXAttributeParentOptions : RszTool.Efx.EFXAttribute
{
    public EFXAttributeParentOptions() : base(EfxAttributeType.ParentOptions) { }

	public via.Int3 RelationPos;
	public via.Int3 RelationRot;
	public via.Int3 RelationScl;
    public uint boneCountMaybe;
    [RszInlineWString] public string? boneName;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeMeshClip, EfxVersion.DMC5)]
public partial class EFXAttributeTypeMeshClip : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeMeshClip() : base(EfxAttributeType.TypeMeshClip) { }

    uint ukn1;
    uint part2Size;
    [RszFixedSizeArray(5)] uint[]? ukn2;
    uint part3Size;
    uint part4Size;
    uint part5Size;
    [RszFixedSizeArray(nameof(part2Size), '*', 16)] public byte[]? part2;
    [RszFixedSizeArray(nameof(part3Size))] public byte[]? part3;
    [RszFixedSizeArray(nameof(part4Size))] public byte[]? part4;
    [RszFixedSizeArray(nameof(part5Size))] public byte[]? part5;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFixEndExpression, EfxVersion.DMC5)]
public partial class EFXAttributeTypeRibbonFixEndExpression : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeRibbonFixEndExpression() : base(EfxAttributeType.TypeRibbonFixEndExpression) { }

    [RszFixedSizeArray(8)] uint[]? ukn1;
	[RszClassInstance] public EFXExpressionListWrapper expressions = new();
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeBillboard2D, EfxVersion.DMC5)]
public partial class EFXAttributeTypeBillboard2D : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeBillboard2D() : base(EfxAttributeType.TypeBillboard2D) { }

    [RszFixedSizeArray(16)] uint[]? ukn;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.DepthOperator, EfxVersion.DMC5)]
public partial class EFXAttributeDepthOperator : RszTool.Efx.EFXAttribute
{
    public EFXAttributeDepthOperator() : base(EfxAttributeType.DepthOperator) { }

    [RszFixedSizeArray(3)] uint[]? ukn;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ProceduralDistortion, EfxVersion.DMC5)]
public partial class EFXAttributeProceduralDistortion : RszTool.Efx.EFXAttribute
{
    public EFXAttributeProceduralDistortion() : base(EfxAttributeType.ProceduralDistortion) { }

    [RszFixedSizeArray(6)] uint[]? ukn;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeMeshExpression, EfxVersion.DMC5)]
public partial class EFXAttributeTypeMeshExpression : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeMeshExpression() : base(EfxAttributeType.TypeMeshExpression) { }

    uint ukn1;
    uint part2Size;
    [RszFixedSizeArray(22)] uint[]? ukn2;
    [RszConditional(nameof(Version), ">=", EfxVersion.RERT), RszFixedSizeArray(3)] uint[]? ukn_rert;
    uint part3Size;
    [RszFixedSizeArray(nameof(part2Size), '/', 4)] public uint[]? part2;
    [RszFixedSizeArray(nameof(part3Size), '/', 4)] public uint[]? part3;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygonClip, EfxVersion.DMC5)]
public partial class EFXAttributeTypePolygonClip : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypePolygonClip() : base(EfxAttributeType.TypePolygonClip) { }

    [RszFixedSizeArray(6)] uint[]? ukn1;
    uint part2Size;
    uint part3Size;
    uint part4Size;
    uint part5Size;

    [RszFixedSizeArray(nameof(part2Size))] public byte[]? part2;
    [RszFixedSizeArray(nameof(part3Size))] public byte[]? part3;
    [RszFixedSizeArray(nameof(part4Size))] public byte[]? part4;
    [RszFixedSizeArray(nameof(part5Size))] public byte[]? part5;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeStrainRibbon, EfxVersion.RE7, EfxVersion.DMC5)]
public partial class EFXAttributeTypeStrainRibbon : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeStrainRibbon() : base(EfxAttributeType.TypeStrainRibbon) { }

    uint ukn1;
    mid_cc mid_color;
    public float ukn2_0;
    public float ukn2_1;
    public float ukn2_2;
    public float ukn2_3;
    public float ukn2_4;
    public float ukn2_5;
    public float ukn2_6;
    public float ukn2_7;
    public uint ukn2_8;
    public float ukn2_9;
    public float ukn2_10;
    public float ukn2_11;
    public float ukn2_12;
    public float ukn2_13;
    public float ukn2_14;
    public float ukn2_15;
    public float ukn2_16;
    public float ukn2_17;
    public float ukn2_18;
    public uint ukn2_19;
    public uint ukn2_20;
    public uint ukn2_21;
    public float ukn2_22;
    public float ukn2_23;
    public UndeterminedFieldType ukn2_24;
    public uint ukn2_25;
    public float ukn2_26;
    public uint ukn2_27;
    [RszConditional(nameof(Version), ">=", EfxVersion.DMC5)]
    [RszFixedSizeArray(8)] uint[]? ukn3;
    [RszInlineWString] public string? boneName;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.MeshEmitterExpression, EfxVersion.DMC5)]
public partial class EFXAttributeMeshEmitterExpression : RszTool.Efx.EFXAttribute
{
    public EFXAttributeMeshEmitterExpression() : base(EfxAttributeType.MeshEmitterExpression) { }

    [RszFixedSizeArray(17)] uint[]? ukn1;
	[RszClassInstance] public EFXExpressionListWrapper expressions = new();
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ShaderSettingsExpression, EfxVersion.DMC5, EfxVersion.RERT, EfxVersion.DD2)]
public partial class EFXAttributeShaderSettingsExpression : RszTool.Efx.EFXAttribute
{
    public EFXAttributeShaderSettingsExpression() : base(EfxAttributeType.ShaderSettingsExpression) { }

    public uint ukn0;
    public uint ukn1;
    [RszConditional(nameof(Version), ">=", EfxVersion.RERT, EndAt = nameof(ukn7))]
    public uint ukn2;
    public uint ukn3;
    public uint ukn4;
    public uint ukn5;
    public uint ukn6;
    public uint ukn7;
    [RszConditional(nameof(Version), ">=", EfxVersion.DD2, EndAt = nameof(dd2_ukn4))]
    public uint dd2_ukn0;
    public uint dd2_ukn1;
    public uint dd2_ukn2;
    public uint dd2_ukn3;
    public uint dd2_ukn4;

	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	[RszClassInstance] public EFXExpressionListWrapperBase? expressions;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.MeshEmitterClip, EfxVersion.DMC5)]
public partial class EFXAttributeMeshEmitterClip : RszTool.Efx.EFXAttribute
{
    public EFXAttributeMeshEmitterClip() : base(EfxAttributeType.MeshEmitterClip) { }

    [RszFixedSizeArray(7)] uint[]? ukn1;
    uint part2Size;
    uint part3Size;
    uint part4Size;

    [RszFixedSizeArray(nameof(part2Size))] public byte[]? part2;
    [RszFixedSizeArray(nameof(part3Size))] public byte[]? part3;
    [RszFixedSizeArray(nameof(part4Size))] public byte[]? part4;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygonExpression, EfxVersion.DMC5)]
public partial class EFXAttributeTypePolygonExpression : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypePolygonExpression() : base(EfxAttributeType.TypePolygonExpression) { }

    [RszFixedSizeArray(19)] uint[]? ukn1;
	[RszClassInstance] public EFXExpressionListWrapper expressions = new();
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeLightning3D, EfxVersion.RE7, EfxVersion.DMC5)]
public partial class EFXAttributeTypeLightning3D : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeLightning3D() : base(EfxAttributeType.TypeLightning3D) { }

	public uint unknBitFlag;
	public via.Color color0;
	public via.Color color1;
	public float unkn2_0;
	public UndeterminedFieldType unkn2_1;
	public UndeterminedFieldType unkn2_2;
	public UndeterminedFieldType unkn2_3;

    public float unkn2_4;
	public UndeterminedFieldType unkn2_5;
	public float unkn2_6;
	public UndeterminedFieldType unkn2_7;
	public uint unkn2_8;
	public UndeterminedFieldType unkn2_9; // either int or float (DMC5) [3, 0.1]
	public UndeterminedFieldType unkn2_10; // either int or float (DMC5) [5, 0.12]
	public UndeterminedFieldType unkn2_11; // either int or float (DMC5) [1, 0.3]
	public UndeterminedFieldType unkn2_12; // either int or float (DMC5) [1, 0.4]
	public float unkn2_13;
	public UndeterminedFieldType unkn2_14;
	public float unkn2_15;
	public float unkn2_16;
	public float unkn2_17;
	public UndeterminedFieldType unkn2_18;
	public float unkn2_19;
	public UndeterminedFieldType unkn2_20;
	public float unkn2_21;
	public UndeterminedFieldType unkn2_22;
	public float unkn2_23;
	public float unkn2_24;
	public float unkn2_25;
	public UndeterminedFieldType unkn2_26;
	public float unkn2_27;
	public UndeterminedFieldType unkn2_28;
	public float unkn2_29;
	public UndeterminedFieldType unkn2_30;
	public float unkn2_31;
	public UndeterminedFieldType unkn2_32;
	public UndeterminedFieldType unkn2_33;
	public float unkn2_34;
	public UndeterminedFieldType unkn2_35;
	public UndeterminedFieldType unkn2_36;
	public UndeterminedFieldType unkn2_37;
	public float unkn2_38;
	public UndeterminedFieldType unkn2_39;
	public float unkn2_40;
	public float unkn2_41;
	public float unkn2_42;
	public float unkn2_43;
	public float unkn2_44;
	public float unkn2_45;
	public float unkn2_46;
	public float unkn2_47;
	public float unkn2_48;
	public uint unkn2_49;
	public UndeterminedFieldType unkn2_50;
	public UndeterminedFieldType unkn2_51;
	public UndeterminedFieldType unkn2_52;
	public UndeterminedFieldType unkn2_53;
	public UndeterminedFieldType unkn2_54;
	public uint unkn2_55;
    [RszVersion(EfxVersion.DMC5)]
	public uint unkn2_56;
    [RszFixedSizeArray(2)] byte[]? boneNameEmpty;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FakeDoF, EfxVersion.DMC5)]
public partial class EFXAttributeFakeDoF : RszTool.Efx.EFXAttribute
{
    public EFXAttributeFakeDoF() : base(EfxAttributeType.FakeDoF) { }

    [RszFixedSizeArray(5)] uint[]? ukn;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FluidEmitter2D, EfxVersion.DMC5)]
public partial class EFXAttributeFluidEmitter2D : RszTool.Efx.EFXAttribute
{
    public EFXAttributeFluidEmitter2D() : base(EfxAttributeType.FluidEmitter2D) { }

    [RszFixedSizeArray(8)] uint[]? ukn;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtTransform2D, EfxVersion.DMC5)]
public partial class EFXAttributePtTransform2D : RszTool.Efx.EFXAttribute
{
    public EFXAttributePtTransform2D() : base(EfxAttributeType.PtTransform2D) { }
    [RszFixedSizeArray(5)] uint[]? ukn;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FluidSimulator2D, EfxVersion.DMC5, EfxVersion.RERT)]
public partial class EFXAttributeFluidSimulator2D : RszTool.Efx.EFXAttribute
{
    public EFXAttributeFluidSimulator2D() : base(EfxAttributeType.FluidSimulator2D) { }

    [RszFixedSizeArray(45)] uint[]? ukn1;
    [RszConditional(nameof(Version), "==", EfxVersion.DMC5)]
    uint ukn2;
    [RszConditional(nameof(Version), "==", EfxVersion.RE7)]
    uint ukn3;
    uint extraByteCount;
    uint path1Size;
    uint path2Size;
    uint path3Size;
    uint path4Size;
    [RszConditional(nameof(Version), "<=", EfxVersion.DMC5)]//dmc5, re7
    uint path5Size;
    [RszInlineWString(nameof(path1Size))] public string? uvsPath1;
    [RszInlineWString(nameof(path2Size))] public string? uvsPath2;
    [RszInlineWString(nameof(path3Size))] public string? uvsPath3;
    [RszInlineWString(nameof(path4Size))] public string? uvsPath4;
    [RszConditional(nameof(Version), "<=", EfxVersion.DMC5), RszInlineWString(nameof(path5Size))] public string? uvsPath5;
    [RszFixedSizeArray(nameof(extraByteCount))] public byte[]? extraBytes;
    // [RszFixedSizeArray(129)] public uint[]? ukndata;
}