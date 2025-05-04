using System.Numerics;
using RszTool;
using RszTool.Efx.Structs.RE4;

namespace RszTool.Efx.Structs.DMC5;

public struct mid_cc {
    public via.Color color1;
    public via.Color color2;
	public float   ukn_float;// <name="Might be brightness">;
}

public struct large_cc {
	public via.Color   color;
	public via.Color   color2;
	public via.Color   color3;
	public float   Brightness;// <name="Might be brightness">;
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

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.Spawn, EfxVersion.DMC5)]
public partial class EFXAttributeSpawn : EFXAttribute
{
	public EFXAttributeSpawn() : base(EfxAttributeType.Spawn) { }

    public uint ukn1;
    public uint count1;
    public uint count2;
    public uint count3;
    public uint count4;
    public uint count5;
    public uint count6;
    public uint disappearTime1;
    public uint disappearTime2;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.Life, EfxVersion.DMC5)]
public partial class EFXAttributeLife : RszTool.Efx.EFXAttribute
{
    public EFXAttributeLife() : base(EfxAttributeType.Life) { }

    [RszFixedSizeArray(9)] uint[]? ukn;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.ShaderSettings, EfxVersion.DMC5)]
public partial class EFXAttributeShaderSettings : RszTool.Efx.EFXAttribute
{
    public EFXAttributeShaderSettings() : base(EfxAttributeType.ShaderSettings) { }

    float  saturation;// <name="Saturation">;
    float  ukn4;
    float  layerNeg;// <name="Layer Negative", comment="The smaller it is the more it gets in front of the other objects.">;
    uint layerPos;// <name="Layer Positive", comment="The bigger it is the more it gets in front of the other objects.">;
    [RszFixedSizeArray(4)] byte[]?  ukn8;
    float  ukn9;
    float  ukn10;
    float  ukn11;
    float  ukn12;
    float  ukn13;
    float  ukn14;
    [RszFixedSizeArray(3)] float[]?  ukn15;
    float  ukn16;
    float  ukn17;
    float  ukn18;
    uint ukn19;
    float  colorBrightness;// <name="Color Brightness">;
    //byte ukn[0x54 - 4];
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
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypeMesh, EfxVersion.DMC5)]
public partial class EFXAttributeTypeMesh : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeMesh() : base(EfxAttributeType.TypeMesh) { }

    uint ukn1;
    uint overriddenHashCount;
    mid_cc mid_color;
    [RszFixedSizeArray(8)] uint[]? ukn2;
    float rotationXMin;// <name="Rotation Minimum X">;
    float rotationXMax;// <name="Rotation Maximum X">;
    float rotationYMin;// <name="Rotation Minimum Y">;
    float rotationYMax;// <name="Rotation Y Maximum">;
    float rotationZMin;// <name="Rotation Minimum Z">;
    float rotationZMax;// <name="Rotation Z Maximum">;
    float scaleXMin;// <name="Scale Minimum X">;
    float scaleXMax;// <name="Scale X Maximum">;
    float scaleYMin;// <name="Scale Minimum Y">;
    float scaleYMax;// <name="Scale Y Maximum">;
    float scaleZMin;// <name="Scale Minimum Z">;
    float scaleZMax;// <name="Scale Z Maximum">;
    float scaleMultiplierMin;// <name="Scale Multiplier Minimum">;
    float scaleMultiplierMax;// <name="Scale Multiplier Maximum">;
    [RszFixedSizeArray(2)] uint[]? ukn3;
    public uint texCount;
    public uint meshPathLength;
    public uint mdfPathLength;
    public uint texPathBlockLength;
    [RszClassInstance, RszList(nameof(overriddenHashCount))] public List<MdfProperty_DMC5> properties = new();
    [RszInlineWString(nameof(meshPathLength))] public string? meshPath;
    [RszInlineWString(nameof(mdfPathLength))] public string? mdfPath;

    // [RszInlineWString(nameof(texPathLength)), RszConditional(nameof(texPathLength), '>', 0)] public string? tex;
	[RszInlineWString, RszList(nameof(texCount))] public string[]? texPaths;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypeRibbonFollow, EfxVersion.DMC5)]
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
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.EmitterShape3D, EfxVersion.DMC5)]
public partial class EFXAttributeEmitterShape3D : RszTool.Efx.EFXAttribute
{
    public EFXAttributeEmitterShape3D() : base(EfxAttributeType.EmitterShape3D) { }

    float spreadXMin;// <name="Spread X Minimum">;
    float spreadXMax;// <name="Spread X Maximum">;
    float spreadYMin;// <name="Spread Y Minimum">;
    float spreadYMax;// <name="Spread Y Maximum">;
    float spreadZMin;// <name="Spread Z Minimum">;
    float spreadZMax;// <name="Spread Z Maximum">;
    [RszFixedSizeArray(12)] uint[]? ukn1;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.PtCollision, EfxVersion.DMC5)]
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
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.Transform3DModifier, EfxVersion.DMC5)]
public partial class EFXAttributeTransform3DModifier : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTransform3DModifier() : base(EfxAttributeType.Transform3DModifier) { }

    [RszFixedSizeArray(55)] public uint[]? ukn;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.PtColorClip, EfxVersion.DMC5)]
public partial class EFXAttributePtColorClip : RszTool.Efx.EFXAttribute
{
    public EFXAttributePtColorClip() : base(EfxAttributeType.PtColorClip) { }

    uint secChunkID;// <name="Second Chunk Type ID">;
    uint ukn2;
    Cycle_Loop loop;// <name="Loop State", format=hex, bgcolor=0x99CCFF>;
    float  loopTime;// <name="Loop Duration", bgcolor=0xFF9BDE>;
    [RszFixedSizeArray(3)] uint[]? ukn3;

    uint firstChunkSize;// <name="First Chunk Size">;
    uint secondChunkSize;// <name="Second Chunk Size">;
    uint thirdChunkSize;// <name="Third Chunk Size">;
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
    //                                         float appearTime;// <name="Appear Time", bgcolor=0xA2A2A2>;
    //                                         uint ukn;
    //                                         if( colorBitCount_arr[3] > c5it){
    //                                             redPos[red] = FTell();
    //                                             red++;
    //                                         } else {
    //                                             alphaLessRedPos[alphaLessRed] = FTell();
    //                                             alphaLessRed++;
    //                                         }
    //                                         byte red;// <name="Red", bgcolor=0x9B9BFF>;
    //                                         [RszFixedSizeArray(3)] byte[]? ukn2;
    //                                     } redSt;// <name="Red">;
    //                                 }
    //                                 break;

    //                             case 2:
    //                                 for(c5it = 0; c5it < colorBitCount_arr[e]; c5it++){
    //                                     struct{
    //                                         float appearTime;// <name="Appear Time", bgcolor=0xA2A2A2>;
    //                                         uint ukn;
    //                                         if( colorBitCount_arr[3] > c5it){
    //                                             greenPos[green] = FTell();
    //                                             green++;
    //                                         } else {
    //                                             alphaLessGreenPos[alphaLessGreen] = FTell();
    //                                             alphaLessGreen++;
    //                                         }
    //                                         byte green;// <name="Green", bgcolor=0x67AA67>;
    //                                         [RszFixedSizeArray(3)] byte[]? ukn2;
    //                                     } greenSt;// <name="Green">;
    //                                 }
    //                                 break;

    //                             case 3:
    //                                 for(c5it = 0; c5it < colorBitCount_arr[e]; c5it++){
    //                                     struct{
    //                                         float appearTime;// <name="Appear Time", bgcolor=0xA2A2A2>;
    //                                         uint ukn;
    //                                         if( colorBitCount_arr[3] > c5it){
    //                                             bluePos[blue] = FTell();
    //                                             blue++;
    //                                         } else {
    //                                             alphaLessBluePos[alphaLessBlue] = FTell();
    //                                             alphaLessBlue++;
    //                                         }
    //                                         byte blue;// <name="Blue", bgcolor=0xFF9B9B>;
    //                                         [RszFixedSizeArray(3)] byte[]? ukn2;
    //                                     } blueSt;// <name="Blue">;
    //                                 }
    //                                 break;

    //                             case 4:
    //                                 for(c5it = 0; c5it < colorBitCount_arr[e]; c5it++){
    //                                     struct{
    //                                         float appearTime;// <name="Appear Time", bgcolor=0xA2A2A2>;
    //                                         uint ukn;
    //                                         alphaPos[alpha] = FTell();
    //                                         alpha++;
    //                                         byte alpha;// <name="Alpha", bgcolor=0x9BFFFF>;
    //                                         [RszFixedSizeArray(3)] byte[]? ukn2;
    //                                     } alphaSt;// <name="Alpha">;
    //                                 }
    //                                 break;
    //                             default:
    //                                 break;
    //                         }
    //                     }
    //                 }secondChunk;// <name="Second Chunk (Color & Alpha)">;
    //                 break;
    //             case 0x00000008:
    //                 for(c5it = 0; c5it < colorBitCount_arr[0]; c5it++){
    //                     struct{
    //                         float appearTime;// <name="Appear Time", bgcolor=0xA2A2A2>;
    //                         uint ukn;
    //                         onlyAlphaPos[onlyAlpha]= FTell();
    //                         onlyAlpha++;
    //                         byte alpha;// <name="Alpha", bgcolor=0x9BFFFF>;
    //                         [RszFixedSizeArray(3)] byte[]? ukn2;
    //                     } alphaSt;// <name="Alpha">;
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
    //                                         float appearTime;// <name="Appear Time", bgcolor=0xA2A2A2>;
    //                                         uint ukn;
    //                                         alphaLessRedPos[alphaLessRed] = FTell();
    //                                         alphaLessRed++;
    //                                         byte red;// <name="Red", bgcolor=0x9B9BFF>;
    //                                         [RszFixedSizeArray(3)] byte[]? ukn2;
    //                                     } redSt;// <name="Red">;
    //                                 }
    //                                 break;

    //                             case 2:
    //                                 for(c5it = 0; c5it < colorBitCount_arr[e]; c5it++){
    //                                     struct{
    //                                         float appearTime;// <name="Appear Time", bgcolor=0xA2A2A2>;
    //                                         uint ukn;
    //                                         alphaLessGreenPos[alphaLessGreen] = FTell();
    //                                         alphaLessGreen++;
    //                                         byte green;// <name="Green", bgcolor=0x67AA67>;
    //                                         [RszFixedSizeArray(3)] byte[]? ukn2;
    //                                     } greenSt;// <name="Green">;
    //                                 }
    //                                 break;

    //                             case 3:
    //                                 for(c5it = 0; c5it < colorBitCount_arr[e]; c5it++){
    //                                     struct{
    //                                         float appearTime;// <name="Appear Time", bgcolor=0xA2A2A2>;
    //                                         uint ukn;
    //                                         alphaLessBluePos[alphaLessBlue] = FTell();
    //                                         alphaLessBlue++;
    //                                         byte blue;// <name="Blue", bgcolor=0xFF9B9B>;
    //                                         [RszFixedSizeArray(3)] byte[]? ukn2;
    //                                     } blueSt;// <name="Blue">;
    //                                 }
    //                                 break;

    //                             default:
    //                                 break;
    //                         }
    //                     }
    //                 }secondChunk;// <name="Second Chunk (Color & Alpha)">;
    //                 break;
    //             default:
    //                 struct{
    //                     uint ukn[secondChunkSize/4];
    //                 } secondChunk;// <name="Second Chunk">;
    //                 break;
    //         }
    //     }
    // }
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.ParentOptions, EfxVersion.DMC5)]
public partial class EFXAttributeParentOptions : RszTool.Efx.EFXAttribute
{
    public EFXAttributeParentOptions() : base(EfxAttributeType.ParentOptions) { }

	public via.Int3 RelationPos;
	public via.Int3 RelationRot;
	public via.Int3 RelationScl;
    public uint boneCountMaybe;
    [RszInlineWString] public string? boneName;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.SpawnExpression, EfxVersion.DMC5)]
public partial class EFXAttributeSpawnExpression : RszTool.Efx.EFXAttribute
{
    public EFXAttributeSpawnExpression() : base(EfxAttributeType.SpawnExpression) { }

    [RszFixedSizeArray(5)] uint[]? ukn1;
	[RszClassInstance] public EFXExpressionListWrapper expressions = new();
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypeMeshClip, EfxVersion.DMC5)]
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
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypeRibbonFixEndExpression, EfxVersion.DMC5)]
public partial class EFXAttributeTypeRibbonFixEndExpression : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeRibbonFixEndExpression() : base(EfxAttributeType.TypeRibbonFixEndExpression) { }

    [RszFixedSizeArray(8)] uint[]? ukn1;
	[RszClassInstance] public EFXExpressionListWrapper expressions = new();
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypeBillboard2D, EfxVersion.DMC5)]
public partial class EFXAttributeTypeBillboard2D : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeBillboard2D() : base(EfxAttributeType.TypeBillboard2D) { }

    [RszFixedSizeArray(16)] uint[]? ukn;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.DepthOperator, EfxVersion.DMC5)]
public partial class EFXAttributeDepthOperator : RszTool.Efx.EFXAttribute
{
    public EFXAttributeDepthOperator() : base(EfxAttributeType.DepthOperator) { }

    [RszFixedSizeArray(3)] uint[]? ukn;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.ProceduralDistortion, EfxVersion.DMC5)]
public partial class EFXAttributeProceduralDistortion : RszTool.Efx.EFXAttribute
{
    public EFXAttributeProceduralDistortion() : base(EfxAttributeType.ProceduralDistortion) { }

    [RszFixedSizeArray(6)] uint[]? ukn;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypeMeshExpression, EfxVersion.DMC5)]
public partial class EFXAttributeTypeMeshExpression : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeMeshExpression() : base(EfxAttributeType.TypeMeshExpression) { }

    uint ukn1;
    uint part2Size;// <name="Second Part's Size">;
    [RszFixedSizeArray(22)] uint[]? ukn2;
    [RszConditional(nameof(Version), ">=", EfxVersion.RERT), RszFixedSizeArray(3)] uint[]? ukn_rert;
    uint part3Size;// <name="Third Part's Size">;
    [RszFixedSizeArray(nameof(part2Size), '/', 4)] public uint[]? part2;
    [RszFixedSizeArray(nameof(part3Size), '/', 4)] public uint[]? part3;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypePolygonClip, EfxVersion.DMC5)]
public partial class EFXAttributeTypePolygonClip : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypePolygonClip() : base(EfxAttributeType.TypePolygonClip) { }

    [RszFixedSizeArray(6)] uint[]? ukn1;
    uint part2Size;// <name="Second Part's Size">;
    uint part3Size;// <name="Third Part's Size">;
    uint part4Size;// <name="Fourth Part's Size">;
    uint part5Size;// <name="Fifth Part's Size">;

    [RszFixedSizeArray(nameof(part2Size))] public byte[]? part2;
    [RszFixedSizeArray(nameof(part3Size))] public byte[]? part3;
    [RszFixedSizeArray(nameof(part4Size))] public byte[]? part4;
    [RszFixedSizeArray(nameof(part5Size))] public byte[]? part5;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypeStrainRibbon, EfxVersion.DMC5)]
public partial class EFXAttributeTypeStrainRibbon : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeStrainRibbon() : base(EfxAttributeType.TypeStrainRibbon) { }

    uint ukn1;
    mid_cc mid_color;
    [RszFixedSizeArray(36)] uint[]? ukn2;
    [RszInlineWString] public string? boneName;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.MeshEmitterExpression, EfxVersion.DMC5)]
public partial class EFXAttributeMeshEmitterExpression : RszTool.Efx.EFXAttribute
{
    public EFXAttributeMeshEmitterExpression() : base(EfxAttributeType.MeshEmitterExpression) { }

    [RszFixedSizeArray(17)] uint[]? ukn1;
    // uint part2Size;// <name="Second Part's Size">;
    // [RszFixedSizeArray(nameof(part2Size))] public byte[]? part2;
	[RszClassInstance] public EFXExpressionListWrapper expressions = new();
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.ShaderSettingsExpression, EfxVersion.DMC5)]
public partial class EFXAttributeShaderSettingsExpression : RszTool.Efx.EFXAttribute
{
    public EFXAttributeShaderSettingsExpression() : base(EfxAttributeType.ShaderSettingsExpression) { }

    [RszFixedSizeArray(2)] uint[]? ukn1;

    // uint part2Size;// <name="Second Part's Size">;
    // [RszFixedSizeArray(nameof(part2Size))] public byte[]? part2;
	[RszClassInstance] public EFXExpressionListWrapper expressions = new();
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.MeshEmitterClip, EfxVersion.DMC5)]
public partial class EFXAttributeMeshEmitterClip : RszTool.Efx.EFXAttribute
{
    public EFXAttributeMeshEmitterClip() : base(EfxAttributeType.MeshEmitterClip) { }

    [RszFixedSizeArray(7)] uint[]? ukn1;
    uint part2Size;// <name="Second Part's Size">;
    uint part3Size;// <name="Third Part's Size">;
    uint part4Size;// <name="Fourth Part's Size">;

    [RszFixedSizeArray(nameof(part2Size))] public byte[]? part2;
    [RszFixedSizeArray(nameof(part3Size))] public byte[]? part3;
    [RszFixedSizeArray(nameof(part4Size))] public byte[]? part4;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypePolygonExpression, EfxVersion.DMC5)]
public partial class EFXAttributeTypePolygonExpression : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypePolygonExpression() : base(EfxAttributeType.TypePolygonExpression) { }

    [RszFixedSizeArray(19)] uint[]? ukn1;
	[RszClassInstance] public EFXExpressionListWrapper expressions = new();
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypeLightning3D, EfxVersion.DMC5)]
public partial class EFXAttributeTypeLightning3D : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeLightning3D() : base(EfxAttributeType.TypeLightning3D) { }

    [RszFixedSizeArray(60)] uint[]? ukn;
    [RszFixedSizeArray(2)] byte[]? boneNameEmpty;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.FakeDoF, EfxVersion.DMC5)]
public partial class EFXAttributeFakeDoF : RszTool.Efx.EFXAttribute
{
    public EFXAttributeFakeDoF() : base(EfxAttributeType.FakeDoF) { }

    [RszFixedSizeArray(5)] uint[]? ukn;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypeNodeBillboard, EfxVersion.DMC5)]
public partial class EFXAttributeTypeNodeBillboard : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeNodeBillboard() : base(EfxAttributeType.TypeNodeBillboard) { }

    [RszFixedSizeArray(60)] uint[]? ukn;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.FluidEmitter2D, EfxVersion.DMC5)]
public partial class EFXAttributeFluidEmitter2D : RszTool.Efx.EFXAttribute
{
    public EFXAttributeFluidEmitter2D() : base(EfxAttributeType.FluidEmitter2D) { }

    [RszFixedSizeArray(8)] uint[]? ukn;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.PtTransform2D, EfxVersion.DMC5)]
public partial class EFXAttributePtTransform2D : RszTool.Efx.EFXAttribute
{
    public EFXAttributePtTransform2D() : base(EfxAttributeType.PtTransform2D) { }
    [RszFixedSizeArray(5)] uint[]? ukn;

}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.FluidSimulator2D, EfxVersion.DMC5)]
public partial class EFXAttributeFluidSimulator2D : RszTool.Efx.EFXAttribute
{
    public EFXAttributeFluidSimulator2D() : base(EfxAttributeType.FluidSimulator2D) { }

    [RszFixedSizeArray(46)] uint[]? ukn1;
    uint extraByteCount;
    uint path1Size;// <name="Texture Path 1 Size">;
    uint path2Size;// <name="Texture Path 2 Size">;
    uint path3Size;// <name="Texture Path 3 Size">;
    uint path4Size;// <name="Texture Path 4 Size">;
    uint path5Size;// <name="Texture Path 5 Size">;
    [RszInlineWString(nameof(path1Size))] public string? uvsPath1;
    [RszInlineWString(nameof(path2Size))] public string? uvsPath2;
    [RszInlineWString(nameof(path3Size))] public string? uvsPath3;
    [RszInlineWString(nameof(path4Size))] public string? uvsPath4;
    [RszInlineWString(nameof(path5Size))] public string? uvsPath5;
    [RszFixedSizeArray(nameof(extraByteCount))] public byte[]? extraBytes;
}