using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.Misc;

public enum DistortionType
{
	Blur = 0,
	Refract = 1,
	BlurTexture = 2
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.DepthOperator, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeDepthOperator : RszTool.Efx.EFXAttribute
{
    public EFXAttributeDepthOperator() : base(EfxAttributeType.DepthOperator) { }

    public uint unkn0;
    public float unkn1;
    public float unkn2;
    [RszVersion(EfxVersion.RE8)]
    public float unkn3;

    [RszVersion(EfxVersion.RE4, EndAt = nameof(unkn12))]
    public float unkn4;
    public UndeterminedFieldType unkn5;
    public float unkn6;
    public UndeterminedFieldType unkn7;
    public float unkn8;
    public UndeterminedFieldType unkn9;
    public float radians10;
    public float unkn11;
    public UndeterminedFieldType unkn12;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Distortion, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeDistortion : EFXAttribute
{
	public EFXAttributeDistortion() : base(EfxAttributeType.Distortion) { }

	public DistortionType distortionType;
	public float unkn2_strength;

	[RszVersion(EfxVersion.RE3, EndAt = nameof(color))]
	public float unkn3;
	public float unkn4;
	[RszVersion(EfxVersion.RE8)]
	public via.Color color;

	[RszVersion(EfxVersion.DD2, EndAt = nameof(dd2_unkn2))]
	public float dd2_unkn1;
	public byte dd2_unkn2;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.DistortionExpression, EfxVersion.DD2)]
public partial class EFXAttributeDistortionExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeDistortionExpression() : base(EfxAttributeType.DistortionExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(5);
	public ExpressionAssignType unkn1;
	public ExpressionAssignType unkn2;
	public ExpressionAssignType unkn3;
	public ExpressionAssignType colorG;
	public ExpressionAssignType unkn5;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ProceduralDistortionDelayFrame, EfxVersion.RE4)]
public partial class EFXAttributeProceduralDistortionDelayFrame : EFXAttribute
{
	public EFXAttributeProceduralDistortionDelayFrame() : base(EfxAttributeType.ProceduralDistortionDelayFrame) { }

	public uint frameDelay;
	public UndeterminedFieldType unkn;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ProceduralDistortionClip, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeProceduralDistortionClip : EFXAttribute, IClipAttribute
{
    public EfxClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeProceduralDistortionClip() : base(EfxAttributeType.ProceduralDistortionClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(1);
    public uint unkn1;
    public int unkn2;
	[RszClassInstance] public EfxClipData clipData = new();
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.WindInfluence3D, EfxVersion.RE8, EfxVersion.RE4)]
public partial class EFXAttributeWindInfluence3D : EFXAttribute
{
	public EFXAttributeWindInfluence3D() : base(EfxAttributeType.WindInfluence3D) { }

	public float unkn1;
	public float unkn2;
	public float unkn3;
	public float unkn4;
	public uint unkn5;
	public uint unkn6;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.WindInfluence3DDelayFrame, EfxVersion.DD2)]
public partial class EFXAttributeWindInfluence3DDelayFrame : EFXAttribute
{
	public EFXAttributeWindInfluence3DDelayFrame() : base(EfxAttributeType.WindInfluence3DDelayFrame) { }

	public uint frameDelay;
	public uint unkn2;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.DepthOcclusion, EfxVersion.RE8)]
public partial class EFXAttributeDepthOcclusion : EFXAttribute
{
	public EFXAttributeDepthOcclusion() : base(EfxAttributeType.DepthOcclusion) { }

	public uint unkn;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PlaneCollider, EfxVersion.RE8, EfxVersion.RE4)]
public partial class EFXAttributePlaneCollider : EFXAttribute
{
	public EFXAttributePlaneCollider() : base(EfxAttributeType.PlaneCollider) { }

	public uint unkn1;
	public UndeterminedFieldType unkn2;
	public float unkn3;
	public UndeterminedFieldType unkn4;
	public UndeterminedFieldType unkn5;
	public UndeterminedFieldType unkn6;
	public float unkn7;
	public uint unkn8;
	public uint unkn9;
	public float unkn10;
	public float unkn11;
	public UndeterminedFieldType unkn12;
	public UndeterminedFieldType unkn13;
	public UndeterminedFieldType unkn14;
	public uint unkn15;
	public uint unkn16;
}


[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FixRandomGenerator, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeFixRandomGenerator : EFXAttribute
{
	public EFXAttributeFixRandomGenerator() : base(EfxAttributeType.FixRandomGenerator) { }

	public uint unkn1_0;
	public int unkn1_1; // probably a seed
	public int unkn1_2;
	public int unkn1_3; // probably a seed
	public int unkn1_4;
	[RszVersion(EfxVersion.RE3, EndAt = nameof(unkn1_9))]
	public int unkn1_5;
	[RszVersion(">", EfxVersion.RE3, EndAt = nameof(unkn1_9))]
	public int unkn1_6;
	public int unkn1_7;
	public int unkn1_8;
	public int unkn1_9;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EffectOptimizeShader, EfxVersion.DD2)]
public partial class EFXAttributeEffectOptimizeShader : EFXAttribute
{
	public EFXAttributeEffectOptimizeShader() : base(EfxAttributeType.EffectOptimizeShader) { }

	public uint unknShaderCRCHash0;
	public uint unknShaderCRCHash1;
	public uint unknShaderCRCHash2;

	[RszVersion(EfxVersion.DD2, EndAt = nameof(unkn20))]
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
    public uint unkn12;
    public uint unkn13;
    public uint unkn14;
    public uint unkn15;
    public uint unkn16;
    public uint unkn17;
    public uint unkn18;
    public uint unkn19;
    public via.Color unkn20;

	[RszInlineWString] public string? shaderPath;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Attractor, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeAttractor : EFXAttribute, IBoneRelationAttribute
{
	public EFXAttributeAttractor() : base(EfxAttributeType.Attractor) { }

	public uint unkn1_0;
	public float positionX;
	public float positionY;
	public float positionZ;
	public float attractionForce;
	public float unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
	[RszVersion(EfxVersion.DD2, EndAt = nameof(unkn1_10))]
	public float unkn1_8;
	public float unkn1_9;
	public float unkn1_10;
	[RszInlineWString] public string? boneName;

    public string? ParentBone { get; set; }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.AttractorClip, EfxVersion.RE8)]
public partial class EFXAttributeAttractorClip : EFXAttribute, IClipAttribute
{
    public EfxClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

	public EFXAttributeAttractorClip() : base(EfxAttributeType.AttractorClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(7);
	public uint unkn1;
	[RszClassInstance] public EfxClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.AttractorExpression, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeAttractorExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeAttractorExpression() : base(EfxAttributeType.AttractorExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(10) { BitNameDict = new () {
		[1] = nameof(posX),
		[2] = nameof(posY),
		[3] = nameof(posZ),
		[4] = nameof(posX_2),
		[5] = nameof(posY_2),
		[6] = nameof(posZ_2),
		[7] = nameof(attractionForce),
	} };
	public ExpressionAssignType posX;
	public ExpressionAssignType posY;
	public ExpressionAssignType posZ;
	public ExpressionAssignType posX_2;
	public ExpressionAssignType posY_2;
	public ExpressionAssignType posZ_2;
	public ExpressionAssignType attractionForce;
	public ExpressionAssignType unkn8;
	public ExpressionAssignType unkn9;
	public ExpressionAssignType unkn10;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Blink, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeBlink : EFXAttribute
{
	public EFXAttributeBlink() : base(EfxAttributeType.Blink) { }

	[RszVersion(EfxVersion.DD2)]
	public uint dd2_unkn1;
	public float unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
	public float unkn1_8;
	public float unkn1_9;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ColorGrading, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeColorGrading : EFXAttribute
{
	public EFXAttributeColorGrading() : base(EfxAttributeType.ColorGrading) { }

	public via.Color color0;//Unconfirmed
	public via.Color color1;//Unconfirmed
	public float unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ContrastHighlighter, EfxVersion.RE4)]
public partial class EFXAttributeContrastHighlighter : EFXAttribute
{
	public EFXAttributeContrastHighlighter() : base(EfxAttributeType.ContrastHighlighter) { }

	public float unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public uint unkn1_4;
	public float unkn1_5;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.DrawOverlay, EfxVersion.DMC5, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeDrawOverlay : EFXAttribute
{
	public EFXAttributeDrawOverlay() : base(EfxAttributeType.DrawOverlay) { }

	public uint unkn0_0;
	public uint unkn0_1;
	public uint unkn0_2;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitMask, EfxVersion.RE4)]
public partial class EFXAttributeEmitMask : EFXAttribute
{
	public EFXAttributeEmitMask() : base(EfxAttributeType.EmitMask) { }

	public uint unkn1;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.IgnorePlayerColor, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RERT)]
public partial class EFXAttributeIgnorePlayerColor : EFXAttribute
{
	public EFXAttributeIgnorePlayerColor() : base(EfxAttributeType.IgnorePlayerColor) { }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.LuminanceBleed, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeLuminanceBleed : EFXAttribute
{
	public EFXAttributeLuminanceBleed() : base(EfxAttributeType.LuminanceBleed) { }

	public uint unkn0;
	[RszVersion(EfxVersion.RE3)]
    public uint unkn1_toggle;
    public float unkn2;

    public float unkn3;
    public float unkn4;
    public float unkn5;
    public float unkn6;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Noise, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeNoise : EFXAttribute
{
	public EFXAttributeNoise() : base(EfxAttributeType.Noise) { }

	public float unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
	public float unkn1_7;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.NoiseExpression, EfxVersion.RE4)]
public partial class EFXAttributeNoiseExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeNoiseExpression() : base(EfxAttributeType.NoiseExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(8);
    public ExpressionAssignType unkn1;
    public ExpressionAssignType unkn2;
    public ExpressionAssignType unkn3;
    public ExpressionAssignType unkn4;
    public ExpressionAssignType unkn5;
    public ExpressionAssignType unkn6;
    public ExpressionAssignType unkn7;
    public ExpressionAssignType unkn8;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.StretchBlur, EfxVersion.RE4)]
public partial class EFXAttributeStretchBlur : EFXAttribute
{
	public EFXAttributeStretchBlur() : base(EfxAttributeType.StretchBlur) { }

    public uint blurRotation;
    public uint two;//<comment="Doesn't seem to work when this value isn't two">;
    public float blurAmount;
    public float unkn4;
    public float unkn5;
    public float unknBlurScaleX;
    public float unknBlurScaleY;
    public float blurOpacity;
    public float unkn9;
    public float unkn10;
    public float unkn11;
    public float unkn12;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TexelChannelOperator, EfxVersion.RE4)]
public partial class EFXAttributeTexelChannelOperator : EFXAttribute
{
	public EFXAttributeTexelChannelOperator() : base(EfxAttributeType.TexelChannelOperator) { }

	public float unkn1_0;
	public float unkn1_1;
	public via.Color unkn1_2;
	public float unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
	public float unkn1_8;
	public float unkn1_9;
	public uint unkn1_10;
	public uint unkn1_11;
	public uint unkn1_12;
	public uint unkn1_13;
	public uint unkn1_14;
	public uint unkn1_15;
	public uint unkn1_16;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.VanishArea3D, EfxVersion.RE4)]
public partial class EFXAttributeVanishArea3D : EFXAttribute, IBoneRelationAttribute
{
	public EFXAttributeVanishArea3D() : base(EfxAttributeType.VanishArea3D) { }

	public uint unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
	public float unkn1_8;
	public UndeterminedFieldType unkn1_9;
	public float unkn1_10;
	public float unkn1_11;
	[RszInlineWString] public string? boneName;

    public string? ParentBone { get; set; }
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.VanishArea3DExpression, EfxVersion.RE4)]
public partial class EFXAttributeVanishArea3DExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeVanishArea3DExpression() : base(EfxAttributeType.VanishArea3DExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(14) { BitNameDict = new() {
		[1] = nameof(positionX),
		[2] = nameof(positionY),
		[3] = nameof(positionZ),

		// [8] = nameof(ceilingPos),
	} };
	public ExpressionAssignType positionX;
	public ExpressionAssignType positionY;
	public ExpressionAssignType positionZ;
	public ExpressionAssignType unkn4;
	public ExpressionAssignType unkn5;
	public ExpressionAssignType unkn6;
	public ExpressionAssignType unkn7;
	public ExpressionAssignType unkn8;
	public ExpressionAssignType unkn9;
	public ExpressionAssignType unkn10;
	public ExpressionAssignType unkn11;
	public ExpressionAssignType unkn12;
	public ExpressionAssignType unkn13;
	public ExpressionAssignType unkn14;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TexelChannelOperatorClip, EfxVersion.RE4)]
public partial class EFXAttributeTexelChannelOperatorClip : EFXAttribute, IClipAttribute
{
    public EfxClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeTexelChannelOperatorClip() : base(EfxAttributeType.TexelChannelOperatorClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(2);
    uint unkn1;
	[RszClassInstance] public EfxClipData clipData = new();
}
