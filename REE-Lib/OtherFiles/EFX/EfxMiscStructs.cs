using System.Numerics;
using ReeLib.Efx.Enums;
using ReeLib.Efx.Structs.Common;
using ReeLib.InternalAttributes;
using ReeLib.via;

namespace ReeLib.Efx.Structs.Misc;

public enum DistortionType
{
	Blur = 0,
	Refract = 1,
	BlurTexture = 2
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.DepthOperator, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeDepthOperator : ReeLib.Efx.EFXAttribute
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
    [RszVersion(EfxVersion.MHWilds)] public float mhws_unkn1;
    public float unkn6;
    public UndeterminedFieldType unkn7;
    public float unkn8;
    public UndeterminedFieldType unkn9;
    public float radians10;
    [RszVersion(EfxVersion.MHWilds)] public UndeterminedFieldType mhws_unkn2;
    public float unkn11;
    public UndeterminedFieldType unkn12;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Distortion, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeDistortion : EFXAttribute
{
	public EFXAttributeDistortion() : base(EfxAttributeType.Distortion) { }

	public DistortionType DistortionType;
	public float Influence;

	[RszVersion(EfxVersion.RE3, EndAt = nameof(SpecularColor))]
	public float SpecularPower;
	public float SpecularIntensity;
	[RszVersion(EfxVersion.RE8)]
	public via.Color SpecularColor;

	[RszVersion(EfxVersion.DD2, EndAt = nameof(FadeScreenEdge))]
	public float AlphaBlend;
	public bool FadeScreenEdge;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.DistortionExpression, EfxVersion.DD2)]
public partial class EFXAttributeDistortionExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeDistortionExpression() : base(EfxAttributeType.DistortionExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(5);
	public ExpressionAssignType unkn1;
	public ExpressionAssignType unkn2;
	public ExpressionAssignType unkn3;
	public ExpressionAssignType unkn4;
	public ExpressionAssignType unkn5;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ProceduralDistortion, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeProceduralDistortion : ReeLib.Efx.EFXAttribute
{
    public EFXAttributeProceduralDistortion() : base(EfxAttributeType.ProceduralDistortion) { }

    [RszVersion(EfxVersion.RE8)]//TODO Verify older versions
	public uint Flags;
	public float UScale;
	public float VScale;
	public float WaveFrequency;
	public float WaveAmplitude;
	public float WaveAmplitudeCoef;

    [RszVersion(EfxVersion.RE8, EndAt = nameof(RadialOscillationAmplitudeNoiseAmplitude))]
	public float WaveOffset;
	public float WaveTimer;
	public float SpinInit;
	public float SpinMax;
	public float SpinTimer;
	public float SpinTimerCoef;
	public float RadialOscillationFrequency;
	public float RadialOscillationAmplitude;
	public float RadialOscillationFrequencyNoiseFrequency;
	public float RadialOscillationFrequencyNoiseAmplitude;
	public float RadialOscillationAmplitudeNoiseFrequency;
	public float RadialOscillationAmplitudeNoiseAmplitude;
	public float RadialOscillationTimer;
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

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(7);
    public UndeterminedFieldType unkn1;
	[RszClassInstance] public EfxClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ProceduralDistortionExpression, EfxVersion.DD2)]
public partial class EFXAttributeProceduralDistortionExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeProceduralDistortionExpression() : base(EfxAttributeType.ProceduralDistortionExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(18);
	public ExpressionAssignType unkn1;
	public ExpressionAssignType unkn2;
	public ExpressionAssignType unkn3;
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
	public ExpressionAssignType unkn15;
	public ExpressionAssignType unkn16;
	public ExpressionAssignType unkn17;
	public ExpressionAssignType unkn18;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FakeDoF, EfxVersion.DMC5)]
public partial class EFXAttributeFakeDoF : ReeLib.Efx.EFXAttribute
{
    public EFXAttributeFakeDoF() : base(EfxAttributeType.FakeDoF) { }

    public float NearDistance;
    public float MaxScale;
    public float MinAlpha;
    public uint UsingMipLevel;
    public float VanishDistance;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.WindInfluence3D, EfxVersion.RE8, EfxVersion.RE4)]
public partial class EFXAttributeWindInfluence3D : EFXAttribute
{
	public EFXAttributeWindInfluence3D() : base(EfxAttributeType.WindInfluence3D) { }

	public via.Range InfluenceRate;
	public via.Range InfluenceCoef;
	public via.RangeI InfluenceFrame;
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

	public uint Flags;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PlaneCollider, EfxVersion.RE8, EfxVersion.RE4)]
public partial class EFXAttributePlaneCollider : EFXAttribute
{
	public EFXAttributePlaneCollider() : base(EfxAttributeType.PlaneCollider) { }

    public uint Flags;
	public Vector3 Position;
    public Vector3 Rotation;
    public via.RangeI BounceNum;
    public via.Range BounceRate;
    public Vector3 FaceNormalRotation;
    public via.RangeI IdleTime;

}


[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FixRandomGenerator, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeFixRandomGenerator : EFXAttribute
{
	public EFXAttributeFixRandomGenerator() : base(EfxAttributeType.FixRandomGenerator) { }

	public int useRandomSeedTableCount;
	public int randomSeedTable0;//TODO Make this into a fixed array of 8?
	public int randomSeedTable1;
	public int randomSeedTable2;
	public int randomSeedTable3;
	[RszVersion(EfxVersion.RE3, EndAt = nameof(tableSelectionGroup))]
	public int randomSeedTable4;
	[RszVersion(">", EfxVersion.RE3, EndAt = nameof(tableSelectionGroup))]
	public int randomSeedTable5;
	public int randomSeedTable6;
	public int randomSeedTable7;
	public int tableSelectionGroup;//This value may be incorrect for older games, could be in a different spot
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EffectShader, EfxVersion.DD2)]
public partial class EFXAttributeEffectOptimizeShader : EFXAttribute
{
	public EFXAttributeEffectOptimizeShader() : base(EfxAttributeType.EffectShader) { }

	public uint unknShaderCRCHash0;
	public uint unknShaderCRCHash1;
	public uint unknShaderCRCHash2;
    public uint unknShaderCRCHash3;
    public uint unknShaderCRCHash4;
    public uint unknShaderCRCHash5;
    public UndeterminedFieldType unkn3;

    public uint unknShaderCRCHash6;
    public uint unknShaderCRCHash7;
    public uint unknShaderCRCHash8;
    public uint unknShaderCRCHash9;
    public uint unknShaderCRCHash10;
    public uint unknShaderCRCHash11;
    public UndeterminedFieldType unkn10;

    public uint unknShaderCRCHash12;
    public uint unknShaderCRCHash13;
    public uint unknShaderCRCHash14;
    public uint unknShaderCRCHash15;
    public uint unknShaderCRCHash16;
    public uint unknShaderCRCHash17;
    public UndeterminedFieldType unkn18;

	[RszVersion(EfxVersion.MHWilds, EndAt = nameof(unknShaderCRCHash20))]
    public uint unknShaderCRCHash18;
    public uint unknShaderCRCHash19;
    public uint unknShaderCRCHash20;

    public uint unknShaderCRCHash21;
    public ByteSet unkn20;

	[RszInlineWString] public string? shaderPath;

    public override string ToString() => !string.IsNullOrEmpty(shaderPath) ? shaderPath : type.ToString();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Attractor, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeAttractor : EFXAttribute, IBoneRelationAttribute
{
	public EFXAttributeAttractor() : base(EfxAttributeType.Attractor) { }

	public uint Flags;
	public Vector3 AttractPosition;
	public via.Range ForceScale;
    public via.Range ReversalForceScale;
	[RszVersion(EfxVersion.DD2, EndAt = nameof(ForceResist))]//TODO Verify older game fields
    public via.Range ReversalDistance;
	public float ForceResist;

	[RszVersion(EfxVersion.MHWilds, EndAt = nameof(endwilds))]
	public UndeterminedFieldType unknWild1;
	public UndeterminedFieldType unknWild2;
	public via.Range unknWild3;
	public via.Range unknWild4;
	public via.Range unknWild5;
	public via.Range unknWild6;
	public via.Range unknWild7;
	public via.Range unknWild8;
	public via.Range unknWild9;
	public UndeterminedFieldType unknWild10;
	public UndeterminedFieldType endwilds;

	[RszInlineWString] public string? boneName;

    public string? ParentBone { get; set; }

    public override string ToString() => !string.IsNullOrEmpty(boneName) ? boneName : type.ToString();
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
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeAttractorExpression() : base(EfxAttributeType.AttractorExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(11) { BitNameDict = new () {
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

	[RszVersion(EfxVersion.MHWilds)]
	public ExpressionAssignType unkn11_Wilds;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Blink, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeBlink : EFXAttribute
{
	public EFXAttributeBlink() : base(EfxAttributeType.Blink) { }

	[RszVersion(EfxVersion.DD2)]
	public uint Flags;
	public float MinRate;
	public float MaxRate;
	public via.Range LowFrequency;
    public via.Range LowFrequencyWidth;
    public via.Range HighFrequency;
    public via.Range HighFrequencyWidth;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ColorGrading, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeColorGrading : EFXAttribute
{
	public EFXAttributeColorGrading() : base(EfxAttributeType.ColorGrading) { }

	public via.Color HighBrightnessColor;
	public via.Color LowBrightnessColor;
	public float HighBrightnessIntensity;
	public float LowBrightnessIntensity;
	public float HighGradingBorder;
	public float LowGradingBorder;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ContrastHighlighter, EfxVersion.RE4)]
public partial class EFXAttributeContrastHighlighter : EFXAttribute
{
	public EFXAttributeContrastHighlighter() : base(EfxAttributeType.ContrastHighlighter) { }

	public float Threshold;
	public float PeakMultiplier;
	public float EdgeLimiter;
	public float LuminanceScale;
	public uint HighlightColor;
	public float HighlightIntensity;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.DrawOverlay, EfxVersion.DMC5, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.MHWilds)]
public partial class EFXAttributeDrawOverlay : EFXAttribute
{
	public EFXAttributeDrawOverlay() : base(EfxAttributeType.DrawOverlay) { }

	public uint Segment;
	public uint Priority;
	public uint CameraID;

	[RszVersion(EfxVersion.MHWilds)]
	public UndeterminedFieldType UnknWilds;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitMask, EfxVersion.RE4)]
public partial class EFXAttributeEmitMask : EFXAttribute
{
	public EFXAttributeEmitMask() : base(EfxAttributeType.EmitMask) { }

	public uint mask;
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

    public LuminanceBleedType BleedType;
	[RszVersion(EfxVersion.RE3)]
    public LuminanceBleedSamplingType SamplingType;
    public float Bleed;

    public float Slide;
    public float ColorScaler;
    public float ColorBias;
    public float TexelScaler;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Noise, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeNoise : EFXAttribute
{
	public EFXAttributeNoise() : base(EfxAttributeType.Noise) { }

	public via.Range LowFrequency;
	public via.Range LowFrequencyWidth;
	public via.Range HighFrequency;
	public via.Range HighFrequencyWidth;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.NoiseExpression, EfxVersion.RE4)]
public partial class EFXAttributeNoiseExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
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

    public uint Flags;
    public uint TargetStencil;//<comment="Doesn't seem to work when this value isn't two">;
    public float StrengthBlur;
    public Vector2 ScreenFadeRange;

    public Vector2 SamplingSize;
    public float AlphaIntensity;
    public float AdjustRotationBySpeed;
    public Vector3 SamplingOffset;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TexelChannelOperator, EfxVersion.RE4)]
public partial class EFXAttributeTexelChannelOperator : EFXAttribute
{
	public EFXAttributeTexelChannelOperator() : base(EfxAttributeType.TexelChannelOperator) { }

	public float HueShift;
	public float HueIntensity;
	public via.Color ShadeColor;
	public float ShadeAlphaBlendRate;
	public float Desaturate;
	public float AmbientColorIntensity;
	public float EmissiveRate;
	public float EmissivePower;
	public float Opacity;
	public float HueIntensityCurve;
	public uint UseIntEnvelope;
	public via.RangeI Appear;
    public via.RangeI Keep;
    public via.RangeI Vanish;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.VanishArea3D, EfxVersion.RE4)]
public partial class EFXAttributeVanishArea3D : EFXAttribute, IBoneRelationAttribute
{
	public EFXAttributeVanishArea3D() : base(EfxAttributeType.VanishArea3D) { }

	public uint Flags;
    [RszVersion(EfxVersion.MHWilds)]
    public Vector3 wilds_unkn0;
    public Vector3 AreaPosition;
    public Vector3 AreaScale;
    public Vector3 AreaAngle;
	public via.Range VanishFrame;
	[RszInlineWString] public string? JointName;

    public string? ParentBone { get; set; }

    public override string ToString() => !string.IsNullOrEmpty(JointName) ? JointName : type.ToString();
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.VanishArea3DExpression, EfxVersion.RE4)]
public partial class EFXAttributeVanishArea3DExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.RgbCommon, EfxVersion.RE4, EfxVersion.DD2, EfxVersion.MHWilds)]
public partial class EFXAttributeRgbCommon : EFXAttribute
{
	public EFXAttributeRgbCommon() : base(EfxAttributeType.RgbCommon) { }

	public uint Flags;
	public via.Color GreenChColor;
	public via.Color GreenChColorRange;
	public float GreenChIntensity;
	public float GreenChSaturate;
	public float GreenChCurve;
	public via.Color RedChColor;
	public via.Color RedChColorRange;

	public float RedChIntensity;
	public float RedChAlphaToB;
	public float Alpha;

	public bool UseGreenChLife;
    public via.RangeI GreenChAppearFrame;
    public via.RangeI GreenChKeepFrame;
    public via.RangeI GreenChVanishFrame;

    public bool GreenChLighting;
	public LifeType GreenChLifeType;

	public byte UseRedChLife;
    public via.RangeI RedChAppearFrame;
    public via.RangeI RedChKeepFrame;
    public via.RangeI RedChVanishFrame;

    public bool RedChLighting;
	public LifeType RedChLifeType;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.RgbCommonExpression, EfxVersion.DD2)]
public partial class EFXAttributeRgbCommonExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeRgbCommonExpression() : base(EfxAttributeType.RgbCommonExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(22) { BitNameDict = new() {
		[1] = "GreenChColor",
		[3] = "GreenChIntensity",
		[4] = "GreenChSaturate",
		[6] = "RedChColor",
		[8] = "RedChIntensity",
		[9] = "Alpha1",
		[10] = "Alpha2",
	} };
	public ExpressionAssignType particleColor;
	public ExpressionAssignType unkn2;
	public ExpressionAssignType colorIntensityGreen;
	public ExpressionAssignType colorSaturate;
	public ExpressionAssignType unkn5;
	public ExpressionAssignType particleColor2;
	public ExpressionAssignType unkn7;
	public ExpressionAssignType colorIntensityRed;
	public ExpressionAssignType alpha1;
	public ExpressionAssignType alpha2;
	public ExpressionAssignType unkn11;
	public ExpressionAssignType unkn12;
	public ExpressionAssignType unkn13;
	public ExpressionAssignType unkn14;
	public ExpressionAssignType unkn15;
	public ExpressionAssignType unkn16;
	public ExpressionAssignType unkn17;
	public ExpressionAssignType unkn18;
	public ExpressionAssignType unkn19;
	public ExpressionAssignType unkn20;
	public ExpressionAssignType unkn21;
	public ExpressionAssignType unkn22;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.RgbWater, EfxVersion.DD2, EfxVersion.MHWilds)]
public partial class EFXAttributeRgbWater : EFXAttribute
{
	public EFXAttributeRgbWater() : base(EfxAttributeType.RgbWater) { }

    public via.Color SpecularColor;
    [RszVersion(EfxVersion.MHWilds)]
    public float mhws_unkn;
    public float SpecularIntensity;

	public via.Color SheetColor;
	public float SheetIntensity;
	public float GtoB;
	public float Alpha;

	public bool UseSpecularLife;
	public via.RangeI SpecularAppearFrame;
    public via.RangeI SpecularKeepFrame;
    public via.RangeI SpecularVanishFrame;

    public LifeType SpecularLifeType;

    public byte UseSheetLife;
    public via.RangeI SheetAppearFrame;
    public via.RangeI SheetKeepFrame;
    public via.RangeI SheetVanishFrame;
    public LifeType SheetLifeType;

	public byte UseGtoBLife;
    public via.RangeI GtoBAppearFrame;
    public via.RangeI GtoBKeepFrame;
    public via.RangeI GtoBVanishFrame;
    public LifeType GtoBLifeType;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.RgbWaterExpression, EfxVersion.MHWilds)]
public partial class EFXAttributeRgbWaterExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeRgbWaterExpression() : base(EfxAttributeType.RgbWaterExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(13) { BitNameDict = new() {
		[1] = "GreenChColor",
		[3] = "GreenChIntensity",
		[4] = "GreenChSaturate",
		[6] = "RedChColor",
		[8] = "RedChIntensity",
		[9] = "Alpha1",
		[10] = "Alpha2",
	} };
	public ExpressionAssignType particleColor;
	public ExpressionAssignType unkn2;
	public ExpressionAssignType colorIntensityGreen;
	public ExpressionAssignType colorSaturate;
	public ExpressionAssignType unkn5;
	public ExpressionAssignType particleColor2;
	public ExpressionAssignType unkn7;
	public ExpressionAssignType colorIntensityRed;
	public ExpressionAssignType alpha1;
	public ExpressionAssignType alpha2;
	public ExpressionAssignType unkn11;
	public ExpressionAssignType unkn12;
	public ExpressionAssignType unkn13;
	public ExpressionAssignType unkn14;
	public ExpressionAssignType unkn15;
	public ExpressionAssignType unkn16;
	public ExpressionAssignType unkn17;
	public ExpressionAssignType unkn18;
	public ExpressionAssignType unkn19;
	public ExpressionAssignType unkn20;
	public ExpressionAssignType unkn21;
	public ExpressionAssignType unkn22;
	public ExpressionAssignType unkn23;
	public ExpressionAssignType unkn24;
	public ExpressionAssignType unkn25;
	public ExpressionAssignType unkn26;
	public ExpressionAssignType unkn27;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FlowMap, EfxVersion.MHWilds)]
public partial class EFXAttributeFlowMap : EFXAttribute
{
	public EFXAttributeFlowMap() : base(EfxAttributeType.FlowMap) { }

	public via.Range Strength;
	public via.Range StrengthCoef;
    public via.Range Speed;
    public via.Range SpeedCoef;

    [RszInlineWString] public string? flowmapMaskPath;

    public override string ToString() => !string.IsNullOrEmpty(flowmapMaskPath) ? flowmapMaskPath : type.ToString();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.AssignCSV, EfxVersion.RE4)]
public partial class EFXAttributeAssignCSV : EFXAttribute
{
	public EFXAttributeAssignCSV() : base(EfxAttributeType.AssignCSV) { }

    public bool RandomizeIndexOrder;
    public bool UseTableRange;
    public via.RangeI mTableRange;
    [RszInlineWString] public string? efcsvPostionListPath;
    [RszInlineWString] public string? efcsvRotationListPath;
    [RszInlineWString] public string? efcsvVelocityListPath;
    [RszInlineWString] public string? efcsvColorListPath;

    public override string ToString() => !string.IsNullOrEmpty(efcsvPostionListPath) ? efcsvPostionListPath : type.ToString();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.DestinationCSV, EfxVersion.DD2)]
public partial class EFXAttributeDestinationCSV : EFXAttribute
{
	public EFXAttributeDestinationCSV() : base(EfxAttributeType.DestinationCSV) { }

	public uint unkn1;
	public UndeterminedFieldType unkn2;
	public UndeterminedFieldType unkn3;
	public uint unkn4;
	public UndeterminedFieldType unkn5;
	public UndeterminedFieldType unkn6;
	public UndeterminedFieldType unkn7;
	[RszInlineWString] public string? efcsvPath;
	[RszInlineWString] public string? unknPath2;
	[RszInlineWString] public string? unknPath3;
	[RszInlineWString] public string? unknPath4;

    public override string ToString() => !string.IsNullOrEmpty(efcsvPath) ? efcsvPath : type.ToString();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TerrainSnap, EfxVersion.DD2, EfxVersion.MHWilds)]
public partial class EFXAttributeTerrainSnap : EFXAttribute
{
	public EFXAttributeTerrainSnap() : base(EfxAttributeType.TerrainSnap) { }

	public TerrainSnapType SnapType;
	public bool InitSnap;
	public via.Range HorizontalBounceRate;
    public via.Range VerticalBounceRate;
    public via.Range Offset;
    public bool IgnoreScale;
    public bool FinishParticleAngleApplyEmitterAngle;
    public float FinishParticleAngleMaxRad;
	public float FinishParticleAngleMinRad;
    public via.Range FinishParticleAngleFrame;
    public float KillParticleHeight;
	[RszVersionExact(EfxVersion.MHWilds)]
	public byte uknByte;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Trigger, EfxVersion.DD2)]
public partial class EFXAttributeTrigger : EFXAttribute
{
	public EFXAttributeTrigger() : base(EfxAttributeType.Trigger) { }

	public uint unkn1;
	public UndeterminedFieldType unkn2;
	public float unkn3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.DrawSubscene, EfxVersion.MHWilds)]
public partial class EFXAttributeDrawSubscene : EFXAttribute
{
	public EFXAttributeDrawSubscene() : base(EfxAttributeType.DrawSubscene) { }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.IgnoreSettings, EfxVersion.MHWilds)]
public partial class EFXAttributeIgnoreSettings : EFXAttribute
{
	public EFXAttributeIgnoreSettings() : base(EfxAttributeType.IgnoreSettings) { }

	public uint Flags;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.RepeatArea, EfxVersion.MHWilds)]
public partial class EFXAttributeRepeatArea : EFXAttribute
{
	public EFXAttributeRepeatArea() : base(EfxAttributeType.RepeatArea) { }

	public uint Flags;
	public Vector3 Area;
	public float Unkn;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Listener, EfxVersion.MHWilds)]
public partial class EFXAttributeListener : EFXAttribute
{
	public EFXAttributeListener() : base(EfxAttributeType.Listener) { }

	public uint Unkn1;
	public uint Unkn2;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Layout, EfxVersion.MHWilds)]
public partial class EFXAttributeLayout : EFXAttribute
{
	public EFXAttributeLayout() : base(EfxAttributeType.Layout) { }

	public uint flags1;
	public uint flags2;
	public uint Unkn3;

	// note: not 100% on the flag bits but most Wilds files seem to work with this
	[RszConditional("((flags1 & (8 | 4 | 16)) != 0)", EndAt = nameof(layoutDataFloats))]
	[RszInlineWString]
	public string? layoutName;
	[RszFixedSizeArray]
	public byte[]? layoutDataFloats;
}
