using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.DMC5;

public struct mid_cc {
    public via.Color color1;
    public via.Color color2;
	public float  ukn_float;
}

public struct large_cc {
	public via.Color   color;
	public via.Color   color2;
	public via.Color   color3;
	public float  Brightness;
}

public enum Cycle_Loop {
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeMesh, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3)]
public partial class EFXAttributeTypeMesh : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeMesh() : base(EfxAttributeType.TypeMesh) { }

    uint ukn1;
    uint overriddenHashCount;
    public via.Color color0;
    public via.Color color1;
    public float unkn2;
    [RszVersion(EfxVersion.RE2)]
    public uint frameCount;
    public uint startingFrameMin;
    public uint startingFrameMax;
    public float animationSpeedMin;
    public float animationSpeedMax;
    public uint animationMode;
    public uint unkn13;
    public uint unkn14;
    public float rotationX;
    public float rotationXVariation;
    public float rotationY;
    public float rotationYVariation;
    public float rotationZ;
    public float rotationZVariation;
    public float scaleX;
    public float scaleXVariation;
    public float scaleY;
    public float scaleYVariation;
    public float scaleZ;
    public float scaleZVariation;
    public float scaleMultiplier;
    public float scaleMultiplierVariation;
    uint ukn3;

    [RszVersion(EfxVersion.RE2, EndAt = nameof(texCount))]
    uint ukn4;
    public uint texCount;

    [RszStringLengthField(nameof(meshPath))] public int meshPathLength;
    [RszStringLengthField(nameof(mdfPath))] public int mdfPathLength;
    public uint texPathBlockLength;
    [RszClassInstance, RszList(nameof(overriddenHashCount)), RszConstructorParams(nameof(Version))]
    public List<MdfProperty> properties = new();

    [RszInlineWString(nameof(meshPathLength))] public string? meshPath;
    [RszInlineWString(nameof(mdfPathLength))] public string? mdfPath;

    [RszVersion(EfxVersion.RE2)]
	[RszInlineWString, RszList(nameof(texCount))] public string[]? texPaths;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform3DModifier, EfxVersion.DMC5)]
public partial class EFXAttributeTransform3DModifier : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTransform3DModifier() : base(EfxAttributeType.Transform3DModifier) { }

    public uint unkn0;
    public uint unkn1;
    public float unkn2;
    public float unkn3;
    public float unkn4;
    public float unkn5;
    public float unkn6;
    public float unkn7;
    public float unkn8;
    public float unkn9;
    public float unkn10;
    public float unkn11;
    public float unkn12;
    public float unkn13;
    public UndeterminedFieldType unkn14;
    public float unkn15;
    public UndeterminedFieldType unkn16;
    public float unkn17;
    public UndeterminedFieldType unkn18;
    public float unkn19;
    public float unkn20;
    public float unkn21;
    public UndeterminedFieldType unkn22;
    public float unkn23;
    public float unkn24;
    public float unkn25;
    public UndeterminedFieldType unkn26;
    public float unkn27;
    public float unkn28;
    public float unkn29;
    public UndeterminedFieldType unkn30;
    public float unkn31;
    public float unkn32;
    public float unkn33;
    public UndeterminedFieldType unkn34;
    public float unkn35;
    public float unkn36;
    public float unkn37;
    public float unkn38;
    public float unkn39;
    public float unkn40;
    public float unkn41;
    public float unkn42;
    public float unkn43;
    public float unkn44;
    public float unkn45;
    public UndeterminedFieldType unkn46;
    public float unkn47;
    public UndeterminedFieldType unkn48;
    public float unkn49;
    public UndeterminedFieldType unkn50;
    public float unkn51;
    public float unkn52;
    public float unkn53;
    public UndeterminedFieldType unkn54;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtColorClip, EfxVersion.DMC5)]
public partial class EFXAttributePtColorClip : RszTool.Efx.EFXAttribute
{
    public EFXAttributePtColorClip() : base(EfxAttributeType.PtColorClip) { }

    public uint secChunkID;
    public uint unkn2;
    public Cycle_Loop loop;
    public float loopTime;
    public uint unkn3;
    public uint unkn4;
    public uint unkn5;

	[RszArraySizeField(nameof(substruct1))] public int substruct1Length;
	[RszArraySizeField(nameof(substruct2))] public int substruct2Length;
	[RszArraySizeField(nameof(substruct3))] public int substruct3Length;
    [RszFixedSizeArray(nameof(substruct1Length), '/', 4)] public int[]? substruct1; // colorBitCount + intCount, apparently
    [RszFixedSizeArray(nameof(substruct2Length), '/', 4)] public int[]? substruct2; // TODO very varying content
    [RszFixedSizeArray(nameof(substruct3Length), '/', 4)] public float[]? substruct3;

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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFixEndExpression, EfxVersion.RE8, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonFixEndExpression : RszTool.Efx.EFXAttribute, IExpressionAttribute
{
    public EFXExpressionContainer? Expression => expressions;

    public EFXAttributeTypeRibbonFixEndExpression() : base(EfxAttributeType.TypeRibbonFixEndExpression) { }

    public uint ukn1_0;
    public uint ukn1_1;
    public uint ukn1_2;
    public uint ukn1_3;
    public uint ukn1_4;
    public uint ukn1_5;
    public uint ukn1_6;
    public uint ukn1_7;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(ukn1_9))]
    public uint ukn1_8;
    public uint ukn1_9;
    [RszVersion(EfxVersion.RE4, EndAt = nameof(ukn1_17))]
    public uint ukn1_10;
    public uint ukn1_11;
    public uint ukn1_12;
    public uint ukn1_13;
    public uint ukn1_14;
    public uint ukn1_15;
    public uint ukn1_16;
    public uint ukn1_17;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper2),
		typeof(EFXExpressionListWrapper1)
	)]
	public EFXExpressionContainer expressions = null!;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeBillboard2D, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeTypeBillboard2D : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeBillboard2D() : base(EfxAttributeType.TypeBillboard2D) { }

    public uint unkn0;
    public via.Color unkn1;
    public via.Color unkn2;
    public float unkn3;
    public float unkn4;
    public float unkn5;
    public float unkn6;
    public float unkn7;
    public float unkn8;
    public float unkn9;
    public float unkn10;
    public float unkn11;
    public float unkn12;
    public float unkn13;
    public float unkn14;
    public uint unkn15;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.DepthOperator, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT)]
public partial class EFXAttributeDepthOperator : RszTool.Efx.EFXAttribute
{
    public EFXAttributeDepthOperator() : base(EfxAttributeType.DepthOperator) { }

    public uint unkn0;
    public float unkn1;
    public float unkn2;
    [RszVersion(EfxVersion.RE8)]
    public float unkn3;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ProceduralDistortion, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeProceduralDistortion : RszTool.Efx.EFXAttribute
{
    public EFXAttributeProceduralDistortion() : base(EfxAttributeType.ProceduralDistortion) { }

    [RszVersion(EfxVersion.RE4)]
	public uint re4_unkn_toggle;
	public float unkn0;
	public float unkn1;
	public float unkn2;
	public float unkn3;
	public float unkn4;
	public float unkn5;

    [RszVersion(EfxVersion.RE4, EndAt = nameof(unkn17))]
	public float unkn6;
	public float unkn7;
	public float unkn8;
	public UndeterminedFieldType unkn9;
	public float unkn10;
	public UndeterminedFieldType unkn11;
	public UndeterminedFieldType unkn12;
	public UndeterminedFieldType unkn13;
	public UndeterminedFieldType unkn14;
	public UndeterminedFieldType unkn15;
	public UndeterminedFieldType unkn16;
	public UndeterminedFieldType unkn17;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeStrainRibbon, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3)]
public partial class EFXAttributeTypeStrainRibbon : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeStrainRibbon() : base(EfxAttributeType.TypeStrainRibbon) { }

    uint ukn1;
	public via.Color color0;
	public via.Color color1;
	public float unkn1_3;
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
    public float ukn2_24;
    public uint ukn2_25;
    public float ukn2_26;
    public uint ukn2_27;
    [RszVersion(EfxVersion.RE2, EndAt = nameof(ukn3_7))]
    public uint ukn3_0;
    public uint ukn3_1;
    public float ukn3_2;
    public via.Color ukn3_3;
    public via.Color ukn3_4;
    public via.Color ukn3_5;
    public float ukn3_6;
    public float ukn3_7;
    [RszVersion(EfxVersion.RE3, EndAt = nameof(ukn4_1))]
    public float ukn4_0;
    public float ukn4_1;
    [RszInlineWString] public string? boneName;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.MeshEmitterExpression, EfxVersion.DMC5)]
public partial class EFXAttributeMeshEmitterExpression : RszTool.Efx.EFXAttribute, IExpressionAttribute
{
    public EFXExpressionContainer? Expression => expressions;

    public EFXAttributeMeshEmitterExpression() : base(EfxAttributeType.MeshEmitterExpression) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public uint unkn4;
    public uint unkn5;
    public uint unkn6;
    public uint unkn7;
    public uint unkn8;
    public uint unkn9;
    public uint unkn10;
    public uint unkn11;
    public uint unkn12;
    public uint unkn13;
    public uint unkn14;
    public uint unkn15;
    public uint unkn16;
	[RszClassInstance] public EFXExpressionListWrapper1 expressions = new();
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ShaderSettingsExpression, EfxVersion.DMC5, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.DD2)]
public partial class EFXAttributeShaderSettingsExpression : RszTool.Efx.EFXAttribute, IExpressionAttribute
{
    public EFXExpressionContainer? Expression => expressions;

    public EFXAttributeShaderSettingsExpression() : base(EfxAttributeType.ShaderSettingsExpression) { }

    public uint ukn0;
    public uint ukn1;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(ukn7))]
    public uint ukn2;
    public uint ukn3;
    public uint ukn4;
    public uint ukn5;
    public uint ukn6;
    public uint ukn7;
    [RszVersion(EfxVersion.DD2, EndAt = nameof(dd2_ukn4))]
    public uint dd2_ukn0;
    public uint dd2_ukn1;
    public uint dd2_ukn2;
    public uint dd2_ukn3;
    public uint dd2_ukn4;

	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper2),
		typeof(EFXExpressionListWrapper1)
	)]
	[RszClassInstance] public EFXExpressionContainer? expressions;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.MeshEmitterClip, EfxVersion.DMC5)]
public partial class EFXAttributeMeshEmitterClip : RszTool.Efx.EFXAttribute
{
    public EFXAttributeMeshEmitterClip() : base(EfxAttributeType.MeshEmitterClip) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public float unkn3;
    public uint unkn4;
    public uint unkn5;
    public uint unkn6;
    uint part2Size;
    uint part3Size;
    uint part4Size;

    [RszFixedSizeArray(nameof(part2Size))] public byte[]? part2;
    [RszFixedSizeArray(nameof(part3Size))] public byte[]? part3;
    [RszFixedSizeArray(nameof(part4Size))] public byte[]? part4;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygonExpression, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTypePolygonExpression : RszTool.Efx.EFXAttribute, IExpressionAttribute
{
    public EFXExpressionContainer? Expression => expressions;

    public EFXAttributeTypePolygonExpression() : base(EfxAttributeType.TypePolygonExpression) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public uint unkn4;
    public uint unkn5;
    public uint unkn6;
    public uint unkn7;
    public uint unkn8;
    public uint unkn9;
    public uint unkn10;
    public uint unkn11;
    public uint unkn12;
    public uint unkn13;
    public uint unkn14;
    public uint unkn15;
    public uint unkn16;
    public uint unkn17;
    public uint unkn18;
	[RszVersion(EfxVersion.RE4)]
	public uint unkn1_20;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper2),
		typeof(EFXExpressionListWrapper1)
	)]
	[RszClassInstance] public EFXExpressionContainer? expressions;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeLightning3D, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5)]
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
    short boneNameEmpty;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FakeDoF, EfxVersion.DMC5)]
public partial class EFXAttributeFakeDoF : RszTool.Efx.EFXAttribute
{
    public EFXAttributeFakeDoF() : base(EfxAttributeType.FakeDoF) { }

    public float unkn0;
    public float unkn1;
    public float unkn2;
    public uint unkn3;
    public float unkn4;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FluidEmitter2D, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE8)]
public partial class EFXAttributeFluidEmitter2D : RszTool.Efx.EFXAttribute
{
    public EFXAttributeFluidEmitter2D() : base(EfxAttributeType.FluidEmitter2D) { }

    public uint unkn0;
    [RszVersion(EfxVersion.DD2)]
    public uint dd2_unkn0;
    public float unkn1;
    public float unkn2;
    public float unkn3;
    public float unkn4;
    public float unkn5;
    public float unkn6;
    public float unkn7;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(unkn9))]
    public float unkn8;
    public float unkn9;
    [RszVersion(EfxVersion.DD2, EndAt = nameof(unkn11))]
	public float unkn10;
	public float unkn11;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtTransform2D, EfxVersion.DMC5, EfxVersion.RE8)]
public partial class EFXAttributePtTransform2D : RszTool.Efx.EFXAttribute
{
    public EFXAttributePtTransform2D() : base(EfxAttributeType.PtTransform2D) { }

    public float unkn0;
    public float unkn1;
    public UndeterminedFieldType unkn2;
    public float unkn3;
    public float unkn4;
}

public struct FloatWithColor
{
    public float value;
    public via.Color color;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FluidSimulator2D, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE8, EfxVersion.RERT)]
public partial class EFXAttributeFluidSimulator2D : RszTool.Efx.EFXAttribute
{
    public EFXAttributeFluidSimulator2D() : base(EfxAttributeType.FluidSimulator2D) { }

    public uint unkn1_0;
    public uint unkn1_1;
    public uint unkn1_2;
    public uint unkn1_3;
    public uint unkn1_4;

    public float unkn1_5;
    public float unkn1_6;
    public float unkn1_7;
    public float unkn1_8;

    public uint unkn1_9;
    public uint unkn1_10;
    public uint unkn1_11;
    public uint unkn1_12;

    public float unkn1_13;
    public float unkn1_14;
    public float unkn1_15;
    public float unkn1_16;
    public float unkn1_17;
    public float unkn1_18;

    public uint unkn1_19;
    public uint unkn1_20;
    public uint unkn1_21;
    public uint unkn1_22;
    [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn2))]
    public float re4_unkn1;
    public float re4_unkn2;

    public float unkn1_23;
    public float unkn1_24;
    public float unkn1_25;
    public float unkn1_26;
    public float unkn1_27;
    // NOTE: the fields here seem to have gotten order swapped around past this point
    // TODO figure out correct re-ordering, match up names and read/write procedures
    [RszVersion(EfxVersion.RE8, EndAt = nameof(unkn1_29_re8))]
    public float unkn1_28_re8;
    public float unkn1_29_re8;
    [RszVersion('<', EfxVersion.RE8, EndAt = nameof(unkn1_29_re3))] // else
    public int unkn1_28_re3;
    public int unkn1_29_re3;
    public float unkn1_30;
    public float unkn1_31;
    public float unkn1_32;
    public float unkn1_33;
    public float unkn1_34;
    public float unkn1_35;

    [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unk2_10))]
    public float re4_unk2_0;
    public float re4_unk2_1;
    public float re4_unk2_2;
    public float re4_unk2_3;
    public float re4_unk2_4;
    public float re4_unk2_5;
    public float re4_unk2_6;
    public float re4_unk2_7;
    public float re4_unk2_8;
    public float re4_unk2_9;
    public float re4_unk2_10;

    [RszVersion("<", EfxVersion.DD2)]
    public uint unkn1_36_v1;
    [RszVersion(EfxVersion.DD2)] // else
    public float unkn1_36_v2;

    public float unkn1_37;
    public float unkn1_38;
    [RszVersion(EfxVersion.RE8)]
    public float unkn1_39_re8;
    [RszVersion("<", EfxVersion.RE8)] // else
    public int unkn1_39_re3;
    public float unkn1_40;

    [RszVersion("<", EfxVersion.RE4)]
    public float unkn1_41;
    [RszVersion(EfxVersion.RE8)]
    public int unkn1_42_re8;
    [RszVersion("<", EfxVersion.RE8)] // else
    public float unkn1_42_re3;

    [RszVersion(EfxVersion.DD2, EndAt = nameof(dd2_unk2))]
    public float dd2_unk1;
    public uint dd2_unk2;

    [RszVersion("<", EfxVersion.RERT)]
    public UndeterminedFieldType unkn2_0;
    [RszVersion("!=", EfxVersion.RE8, EndAt = nameof(unkn2_2))]
    public uint unkn2_1;
    public uint unkn2_2;
    [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unk3_3))]
    public float re4_unk3_1;
    public float re4_unk3_2;
    public uint re4_unk3_3;

    public uint extraByteCount;
    public uint path1Size;
    public uint path2Size;
    public uint path3Size;
    public uint path4Size;
    [RszVersion(nameof(Version), "<=", EfxVersion.RE3, "||", nameof(Version), ">=", EfxVersion.RE4)]
    public uint path5Size;
    [RszInlineWString(nameof(path1Size))] public string? uvsPath1; // turbulence
    [RszInlineWString(nameof(path2Size))] public string? uvsPath2; // source density map
    [RszInlineWString(nameof(path3Size))] public string? uvsPath3;
    [RszInlineWString(nameof(path4Size))] public string? uvsPath4; // density colormap
    [RszVersion(nameof(Version), "<=", EfxVersion.RE3, "||", nameof(Version), ">=", EfxVersion.RE4), RszInlineWString(nameof(path5Size))]
    public string? uvsPath5; // projection diffuse map
    [RszFixedSizeArray(nameof(extraByteCount))] public byte[]? extraBytes;

    // may be an array - count in re4_unk3_3?
    [RszVersion(EfxVersion.DD2), RszFixedSizeArray(nameof(re4_unk3_3))]
    public FloatWithColor[]? gradient;
}
