using ReeLib.Efx.Structs.Common;
using ReeLib.InternalAttributes;
using ReeLib.via;

namespace ReeLib.Efx.Structs.Basic;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Spawn, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeSpawn : EFXAttribute
{
	public EFXAttributeSpawn() : base(EfxAttributeType.Spawn) { }

	public uint MaxParticles;
	public uint ParticleInterval;
	public via.Int2 SpawnNum;
	public via.Int2 IntervalFrame;

	[RszVersion('<', EfxVersion.DD2)]
	public uint UseSpawnFrameInt;

	[RszVersion(EfxVersion.DD2)] public bool UseSpawnFrame;
	public via.RangeI SpawnFrame;

	[RszVersion(">", EfxVersion.RE3, EndAt = nameof(OriginalMaxParticles))]
	public via.RangeI LoopNum;

	public via.Int2 EmitterDelayFrame;

	[RszVersion(EfxVersion.DD2, EndAt = nameof(UseRevival))]
	public bool RingBufferMode;
	public bool UseLoopNumRatio;
	public bool Interpolate;
	public float DistancePerSpawn;
	public bool UseRevival;
    //TODO FIX Check these are correct on older games
	[RszVersion(EfxVersion.RE4)]
    via.RangeI RevivalNum;
	[RszVersion(EfxVersion.DD2)]
    via.RangeI RevivalInterval;


	[RszVersionExact(EfxVersion.RE4, EndAt = nameof(re4_unkn4))] // TODO recheck
	public uint re4_unkn0;
	public float re4_unkn1;
	public uint re4_unkn2;
	public uint re4_unkn3;
	public uint re4_unkn4; // TODO need to verify where to place these relative to SpawnChanceFrame

	[RszVersion(EfxVersion.DD2)] public bool TrySpawnAllParticles;

	[RszVersion(EfxVersion.RE8)]
	public uint SpawnChanceFrame;
    [RszVersion(EfxVersion.DD2)] public bool InitializeFull;
    [RszVersion(EfxVersion.RERT)] public uint OriginalMaxParticles;

    /*
	[RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn6))]

	[RszVersion(EfxVersion.DD2)] public byte dd2_unkn5;
	public uint re4_unkn5;
	[RszVersion("<", EfxVersion.DD2, EndAt = nameof(re4_unkn6))]
	public uint re4_unkn6;
    */
    [RszVersion(EfxVersion.MHWilds)] public byte mhws_unkn_toggle;

    public override string ToString() => $"Count = {SpawnNum}";
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.SpawnExpression, EfxVersion.RE3, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeSpawnExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeSpawnExpression() : base(EfxAttributeType.SpawnExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(7) { BitNames = [nameof(spawnNum), nameof(spawnNumRange), nameof(intervalFrame), nameof(intervalFrameRange), nameof(emitterDelayFrame), nameof(emitterDelayFrameRange), nameof(speed) ] };
	public ExpressionAssignType spawnNum;
	public ExpressionAssignType spawnNumRange;
	public ExpressionAssignType intervalFrame;
	public ExpressionAssignType intervalFrameRange;
	[RszVersion(EfxVersion.RE3, EndAt = nameof(emitterDelayFrameRange))]
	public ExpressionAssignType emitterDelayFrame;
	public ExpressionAssignType emitterDelayFrameRange;
    [RszVersion(EfxVersion.RERT)]
	public ExpressionAssignType speed;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ParentOptions, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeParentOptions : EFXAttribute, IBoneRelationAttribute
{
	public EFXAttributeParentOptions() : base(EfxAttributeType.ParentOptions) { }

	public via.Int3 RelationPos;//TODO ENUM - ParentConstraint
	public via.Int3 RelationRot;
	public via.Int3 RelationScl;
	[RszVersion('<', EfxVersion.RE8)]
	public uint ParticleUseLocal_re7;
	[RszVersion(EfxVersion.RE8, EndAt = nameof(ConstInheritReleaseRate))]
	public byte ParticleUseLocal;
	public via.Range ConstInheritRate;
	public via.RangeI ConstFrame;
    public via.RangeI ConstReleaseFrame;
	public float ConstInheritReleaseRate;

	[RszVersion(EfxVersion.Pragmata)] public uint PragUkn1;
	[RszVersion(EfxVersion.Pragmata)] public uint PragUkn2;

	[RszInlineWString(ByteSize = true)] public string? BoneName;

    public string? ParentBone { get; set; }

    public override string ToString() => !string.IsNullOrEmpty(BoneName) ? BoneName : type.ToString();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ParentOptionsExpression, EfxVersion.DD2)]
public partial class EFXAttributeParentOptionsExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeParentOptionsExpression() : base(EfxAttributeType.ParentOptionsExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(8);
    public ExpressionAssignType unkn1_1;
    public ExpressionAssignType unkn1_2;
    public ExpressionAssignType unkn1_3;
    public ExpressionAssignType unkn1_4;
    public ExpressionAssignType unkn1_5;
    public ExpressionAssignType unkn1_6;
    public ExpressionAssignType unkn1_7;

	[RszVersion(EfxVersion.MHWilds)]
    public ExpressionAssignType unkn1_8;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Life, EfxVersion.RE7, EfxVersion.DMC5, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeLife : EFXAttribute
{
	public via.RangeI AppearFrame;
	public via.RangeI KeepFrame;
	public via.RangeI VanishFrame;
	public via.RangeI KeepHoldFrame;
	[RszVersion(EfxVersion.RE2)]
	public uint Flags;//VanishEvent flags

	public EFXAttributeLife() : base(EfxAttributeType.Life) { }

    public override string ToString() => $"Appear {AppearFrame}, Keep {KeepFrame}, Vanish {VanishFrame}";
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.LifeExpression, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeLifeExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeLifeExpression() : base(EfxAttributeType.LifeExpression) { }

	// re7 and dd2 have value + rand pairs: 1-2 = appear, 3-4 = keep, 5-6 = vanish
	// rest have just singular values
	// TODO bitset versioning
	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(6)  { BitNameDict = new () {
		[1] = nameof(appearLife),
		[2] = nameof(keepLife),
		[3] = nameof(vanishLife),
	} };

	public ExpressionAssignType appearLife;
	[RszVersion(nameof(Version), ">=", EfxVersion.DD2, "||", nameof(Version), "==", EfxVersion.RE7)]
	public ExpressionAssignType appearLifeRand;
	public ExpressionAssignType keepLife;
	[RszVersion(nameof(Version), ">=", EfxVersion.DD2, "||", nameof(Version), "==", EfxVersion.RE7)]
	public ExpressionAssignType keepLifeRand;
	public ExpressionAssignType vanishLife;
	[RszVersion(nameof(Version), ">=", EfxVersion.DD2, "||", nameof(Version), "==", EfxVersion.RE7)]
	public ExpressionAssignType vanishLifeRand;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion))]
public partial class TextureUnitData : BaseModel
{
	public uint unkn1;
	public via.Color color;
	public float unkn3;
	public float unkn4;
	public UndeterminedFieldType unkn5;
	public float unkn6;
	public float unkn7;
	public float unkn8;
	public float unkn9;
	public float unkn10;
	public float unkn11;
	public float unkn12;
	public UndeterminedFieldType unkn13;
	public float unkn14;
	public UndeterminedFieldType unkn15;
	public float unkn16;
	public float unkn17;
	public float unkn18;
	public float unkn19;
	public float unkn20;
	public UndeterminedFieldType unkn21;
	public float unkn22;
	public float unkn23;
	public float unkn24;
	public float unkn25;
	public float unkn26;
	public UndeterminedFieldType unkn27;
	public float unkn28;
	public UndeterminedFieldType unkn29;
	public float unkn30;
	public UndeterminedFieldType unkn31;
	public float unkn32;
	public UndeterminedFieldType unkn33;
	public uint unkn34;
	public uint unkn35;
	public UndeterminedFieldType unkn36;
	public UndeterminedFieldType unkn37;
	public float unkn38;
	public float unkn39;

    public override string ToString() => $"TextureUnitData: Color = {color}";
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TextureUnit, EfxVersion.DMC5, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTextureUnit : EFXAttribute
{
	public EFXAttributeTextureUnit() : base(EfxAttributeType.TextureUnit) { }

	public uint unkn1;
	[RszClassInstance] public TextureUnitData? texUnit1;
	[RszClassInstance] public TextureUnitData? texUnit2;
	[RszClassInstance] public TextureUnitData? texUnit3;

	[RszStringLengthField(nameof(uvs0Path))] public int uvs0PathCharCount;
	[RszStringLengthField(nameof(uvs1Path))] public int uvs1PathCharCount;
	[RszStringLengthField(nameof(uvs2Path))] public int uvs2PathCharCount;
	[RszInlineWString(nameof(uvs0PathCharCount))] public string? uvs0Path;
	[RszInlineWString(nameof(uvs1PathCharCount))] public string? uvs1Path;
	[RszInlineWString(nameof(uvs2PathCharCount))] public string? uvs2Path;

    public override string ToString() => $"UVS path: {uvs0Path}, {uvs1Path}, {uvs2Path}";
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TextureUnitExpression, EfxVersion.DD2)]
public partial class EFXAttributeTextureUnitExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTextureUnitExpression() : base(EfxAttributeType.TextureUnitExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(114);
	[RszFixedSizeArray(114)] public readonly ExpressionAssignType[] assignTypes = new ExpressionAssignType[114];
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TextureFilter, EfxVersion.RE4)]
public partial class EFXAttributeTextureFilter : EFXAttribute
{
	public EFXAttributeTextureFilter() : base(EfxAttributeType.TextureFilter) { }

	public float TexelAlphaRate;
	public float TexelAlphaHPThreshold;
	public float TexelAlphaHPMinValue;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UVScroll, EfxVersion.RE4)]
public partial class EFXAttributeUVScroll : EFXAttribute
{
	public EFXAttributeUVScroll() : base(EfxAttributeType.UVScroll) { }

	public uint Flags;
    public via.Range uScrollAdd;
    public via.Range vScrollAdd;
    public via.Range uScrollAddCoef;
    public via.Range vScrollAddCoef;
    public via.Range uScrollOffset;
    public via.Range vScrollOffset;

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UVSequence, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeUVSequence : EFXAttribute
{
	public EFXAttributeUVSequence() : base(EfxAttributeType.UVSequence) { }

	public via.RangeI SequenceNo;
    public via.RangeI PatternNo;
    public via.Range PlaySpeed;
	public uint Flags;//<comment="0 - Show only starting frame,1 - Looped Animation, 2 - Play once and disappear after last frame, 3 - Play once and stay on last frame until end of duration in Life struct.">;
	[RszInlineWString] public string? UVSPath;

    public override string ToString() => $"UVS path: {UVSPath}";
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UVSequenceModifier, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeUVSequenceModifier : EFXAttribute
{
	public EFXAttributeUVSequenceModifier() : base(EfxAttributeType.UVSequenceModifier) { }

	public uint Flags;
	public via.Range PlaySpeedInit;
    public via.Range PlaySpeedFinal;
    public via.Range PlaySpeedChangeTimeCoef;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UVSequenceExpression, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeUVSequenceExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeUVSequenceExpression() : base(EfxAttributeType.UVSequenceExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(6) { BitNameDict = new () {
		[1] = nameof(speed),
		[2] = nameof(speedRand),
		// [3] = nameof(unkn3),
		// [4] = nameof(unkn4),
		// [5] = nameof(unkn5),
		// [6] = nameof(unkn6),
	} };
	public ExpressionAssignType speed;
	public ExpressionAssignType speedRand;
	public ExpressionAssignType unkn3;
	public ExpressionAssignType unkn4;
	public ExpressionAssignType unkn5;
	public ExpressionAssignType unkn6;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.AlphaCorrection, EfxVersion.RE7, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeAlphaCorrection : EFXAttribute
{
	public EFXAttributeAlphaCorrection() : base(EfxAttributeType.AlphaCorrection) { }

	[RszVersion(EfxVersion.RE2)]
	uint Flags;
	float LowPass;
	float HighPass;
	float CurveConst;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ShaderSettings, EfxVersion.RE7, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeShaderSettings : EFXAttribute
{
	public EFXAttributeShaderSettings() : base(EfxAttributeType.ShaderSettings) { }

	public float Saturation;
	public uint unkn2;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(re8_unkn2))]
	public float re8_unkn1;
	public float re8_unkn2;

	public float LayerNegative;
    [RszVersion('<', EfxVersion.RE8)]
	public uint LayerPositive;
	[RszVersionExact(EfxVersion.RE7)]
	public uint toggle_re7;
    [RszVersion(EfxVersion.RE8)]
	public uint re8_unkn000;

	public via.Color color0;
	public float unkn6;
	public float unkn7;
	public float unkn8;
	public float unkn9;
	public float unkn10;
	public float unkn11;
	public float unkn12;
	public uint unkn13;
	public uint unkn14;
	public float unkn15;
	public float unkn16;
	[RszVersion(EfxVersion.RE2)]
	public float unkn17;

	[RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn3))]
	public float re4_unkn1;
	public float re4_unkn2;
	public float re4_unkn3;

    [RszVersion('<', EfxVersion.DD2)]
	public uint unkn19;

// the below fields aren't quite matched by name across the versions, the value ranges seem wildly different
    [RszVersion(EfxVersion.RERT, EndAt = nameof(sb_unkn5))]
	public float sb_unkn2;
	public float sb_unkn3;
	public float sb_unkn4;
	public float sb_unkn5;

    [RszVersionExact(EfxVersion.RE4)]
	public uint toggle_re4;
    [RszVersion(EfxVersion.DD2)]
	public float dd2_unkn1;
    [RszVersion(EfxVersion.RE2)]
	public float ColorBrightness;

    [RszVersion(EfxVersion.DD2)] public uint toggle_dd2;
    [RszVersion(EfxVersion.RE3, EndAt = nameof(unkn29))]
	public float unkn21;

    [RszVersion(">", EfxVersion.RERT, EndAt = nameof(sb_unkn12))]
	public float sb_unkn9;
	public float sb_unkn10;
	public float sb_unkn11;
	public float sb_unkn12;

    [RszVersion(EfxVersion.DD2)]
	public float dd2_unkn2;
	public uint unkn22;

    [RszVersionExact(EfxVersion.RE8, EndAt = nameof(re8_unkn5))]
	public float re8_unkn3;
	public float re8_unkn4;
	public float re8_unkn5;

    [RszVersion(EfxVersion.RE8)]
	public uint unkn24;

    [RszVersion(nameof(Version), ">=", EfxVersion.RERT, "&&", nameof(Version), "<", EfxVersion.MHWilds, EndAt = nameof(unkn29))]
	public uint unkn25;
	public uint unkn26;
	public uint unkn27;
	public uint unkn28;
	public uint unkn29;

	[RszVersion(EfxVersion.MHWilds, EndAt = nameof(mhws_unkn_short))] public float mhws_unkn1;
	public float mhws_unkn2;
	public short mhws_unkn_short;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ShaderSettingsExpression, EfxVersion.DMC5, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.DD2)]
public partial class EFXAttributeShaderSettingsExpression : ReeLib.Efx.EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

    public EFXAttributeShaderSettingsExpression() : base(EfxAttributeType.ShaderSettingsExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(12) { BitNameDict = new () {
		// [1] = nameof(volume), // re7/rt/dmc5/re8 - volume/volumeblend (alpha?); color - dd2
		// [2] = nameof(alpha), // alpha - dd2
		// [3] = nameof(translationZ), LightShadowRatio
		// [4] = nameof(rotationX), BackFaceLightRatio
		// [5] = nameof(rotationY),
		// [6] = nameof(rotationZ), 6,7,8 - dd2 vectorXYZ
		// [7] = nameof(scaleX),
		// [8] = nameof(scaleY),
		// [9] = nameof(scaleZ),
	} };
    public ExpressionAssignType ukn1;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(ukn7))]
    public ExpressionAssignType ukn2;
    public ExpressionAssignType ukn3;
    public ExpressionAssignType ukn4;
    public ExpressionAssignType ukn5;
    public ExpressionAssignType ukn6;
    public ExpressionAssignType ukn7;
    [RszVersion(EfxVersion.DD2, EndAt = nameof(ukn12))]
    public ExpressionAssignType ukn8;
    public ExpressionAssignType ukn9;
    public ExpressionAssignType ukn10;
    public ExpressionAssignType ukn11;
    public ExpressionAssignType ukn12;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PlayEfx, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributePlayEfx : EFXAttribute
{
	public EFXAttributePlayEfx() : base(EfxAttributeType.PlayEfx) { }

	// [RszVersion(EfxVersion.MHWilds)] public uint mhws_unkn;
	[RszInlineWString] public string? efxPath;

    public override string ToString() => $"Efx path: {efxPath}";
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PlayEmitter, EfxVersion.DMC5, EfxVersion.RE4)]
public sealed partial class EFXAttributePlayEmitter : EFXAttribute, IDisposable
{
	public EFXAttributePlayEmitter() : base(EfxAttributeType.PlayEmitter) { }

	// [RszVersion(EfxVersion.MHWilds)] public uint mhws_unkn;
	public uint efxrSize;
	[RszIgnore] public EfxFile? efxrData;

    protected override bool DoRead(FileHandler handler)
    {
		handler.Read(ref efxrSize);
		var end = handler.Position + efxrSize;
		efxrData = new EfxFile(handler.WithOffset(handler.Position));
		efxrData.Embedded = true;
		efxrData.Read();
		handler.Seek(end);
		return true;
    }

    protected override bool DoWrite(FileHandler handler)
    {
		var sizeMarker = handler.Position;
		handler.Skip(sizeof(uint));
		var start = handler.Position;
		efxrData?.WriteTo(handler.WithOffset(start), false);
		handler.Write(sizeMarker, (uint)(handler.Position - start));
		return true;
    }

    public void Dispose()
    {
		efxrData?.FileHandler.Dispose();
    }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.RenderTarget, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeRenderTarget : EFXAttribute
{
	public EFXAttributeRenderTarget() : base(EfxAttributeType.RenderTarget) { }

	public uint unkn_toggle;
	[RszInlineWString] public string? rtexPath;

    public override string ToString() => $"RenderTarget: {rtexPath}";
}
