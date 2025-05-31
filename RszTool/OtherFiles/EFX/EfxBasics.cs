using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.Basic;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Spawn, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeSpawn : EFXAttribute
{
	public EFXAttributeSpawn() : base(EfxAttributeType.Spawn) { }

	public uint maxParticles;
	public uint rawMaxParticles;
	public via.Int2 spawnNum;
	public via.Int2 intervalFrame;
	public uint useSpawnFrame;
	public via.RangeI spawnFrame;

	[RszVersion(">", EfxVersion.RE3, EndAt = nameof(re4_unkn6))]
	public via.RangeI loopNum;

	public via.Int2 emitterdelayFrame;

	public uint ringBufferMode;

	[RszVersion(EfxVersion.DD2, EndAt = nameof(dd2_unkn2))]
	public short dd2_unkn1;
	public byte dd2_unkn2;

	[RszVersion(EfxVersion.RERT, EndAt = nameof(re4_unkn6))]
	public uint sb_unkn3;

	[RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn6))]
	public uint re4_unkn0;
	public float re4_unkn1;
	public uint re4_unkn2;
	public uint re4_unkn3;
	public uint re4_unkn4;
	[RszVersion("<", EfxVersion.DD2, EndAt = nameof(re4_unkn6))]
	public uint re4_unkn5;
	public uint re4_unkn6;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.SpawnExpression, EfxVersion.RE3, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeSpawnExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression => expressions;
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

	public via.Int3 RelationPos;
	public via.Int3 RelationRot;
	public via.Int3 RelationScl;
	[RszVersion('<', EfxVersion.RE8)]
	public uint ParticleUseLocal_re7;
	[RszVersion(EfxVersion.RE8, EndAt = nameof(unkn8))]
	public byte ParticleUseLocal;
	public float ConstInheritRate;
	public float ConstInheritVariation;
	public uint ConstFrame;
	public uint ConstFrameVariation;
	public uint unkn6;
	public uint unkn7;
	public float unkn8;

	[RszInlineWString] public string? boneName;

    public string? ParentBone { get; set; }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ParentOptionsExpression, EfxVersion.DD2)]
public partial class EFXAttributeParentOptionsExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeParentOptionsExpression() : base(EfxAttributeType.ParentOptionsExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(7);
    public ExpressionAssignType unkn1_1;
    public ExpressionAssignType unkn1_2;
    public ExpressionAssignType unkn1_3;
    public ExpressionAssignType unkn1_4;
    public ExpressionAssignType unkn1_5;
    public ExpressionAssignType unkn1_6;
    public ExpressionAssignType unkn1_7;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Life, EfxVersion.RE7, EfxVersion.DMC5, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeLife : EFXAttribute
{
	public via.Int2 AppearFrameRange;
	public via.Int2 KeepFrameRange;
	public via.Int2 VanishFrameRange;
	public uint unkn7; // Might be KeepHoldFrame
	public uint unkn8; // Might be KeepHoldFrameVariation
	[RszVersion(EfxVersion.RE2)]
	public uint unkn9;

	public EFXAttributeLife() : base(EfxAttributeType.Life) { }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.LifeExpression, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeLifeExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression => expressions;
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
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TextureUnit, EfxVersion.DMC5, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTextureUnit : EFXAttribute
{
	public EFXAttributeTextureUnit() : base(EfxAttributeType.TextureUnit) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public int unkn1_2;
	public float unkn1_3;
	public UndeterminedFieldType unkn1_4;
	public UndeterminedFieldType unkn1_5;
	public float unkn1_6;
	public UndeterminedFieldType unkn1_7;
	public float unkn1_8;
	public float unkn1_9;
	public float unkn1_10;
	public float unkn1_11;
	public float unkn1_12;
	public UndeterminedFieldType unkn1_13;
	public float unkn1_14;
	public UndeterminedFieldType unkn1_15;
	public float unkn1_16;
	public float unkn1_17;
	public UndeterminedFieldType unkn1_18;
	public float unkn1_19;
	public float unkn1_20;
	public UndeterminedFieldType unkn1_21;
	public float unkn1_22;
	public float unkn1_23;
	public float unkn1_24;
	public float unkn1_25;
	public UndeterminedFieldType unkn1_26;
	public UndeterminedFieldType unkn1_27;
	public UndeterminedFieldType unkn1_28;
	public UndeterminedFieldType unkn1_29;
	public float unkn1_30;
	public UndeterminedFieldType unkn1_31;
	public float unkn1_32;
	public UndeterminedFieldType unkn1_33;
	public uint unkn1_34;
	public uint unkn1_35;
	public UndeterminedFieldType unkn1_36;
	public UndeterminedFieldType unkn1_37;
	public float unkn1_38;
	public float unkn1_39;
	public uint unkn1_40;
	public via.Color unkn1_41;
	public float unkn1_42;
	public float unkn1_43;
	public UndeterminedFieldType unkn1_44;
	public float unkn1_45;
	public float unkn1_46;
	public float unkn1_47;
	public float unkn1_48;
	public float unkn1_49;
	public float unkn1_50;
	public float unkn1_51;
	public UndeterminedFieldType unkn1_52;
	public float unkn1_53;
	public UndeterminedFieldType unkn1_54;
	public float unkn1_55;
	public float unkn1_56;
	public float unkn1_57;
	public float unkn1_58;
	public float unkn1_59;
	public UndeterminedFieldType unkn1_60;
	public float unkn1_61;
	public UndeterminedFieldType unkn1_62;
	public float unkn1_63;
	public UndeterminedFieldType unkn1_64;
	public float unkn1_65;
	public UndeterminedFieldType unkn1_66;
	public float unkn1_67;
	public UndeterminedFieldType unkn1_68;
	public float unkn1_69;
	public UndeterminedFieldType unkn1_70;
	public float unkn1_71;
	public UndeterminedFieldType unkn1_72;
	public float unkn1_73;
	public float unkn1_74;
	public UndeterminedFieldType unkn1_75;
	public UndeterminedFieldType unkn1_76;
	public float unkn1_77;
	public float unkn1_78;
	public uint unkn1_79;
	public via.Color unkn1_80;
	public float unkn1_81;
	public UndeterminedFieldType unkn1_82;
	public UndeterminedFieldType unkn1_83;
	public float unkn1_84;
	public float unkn1_85;
	public UndeterminedFieldType unkn1_86;
	public UndeterminedFieldType unkn1_87;
	public float unkn1_88;
	public UndeterminedFieldType unkn1_89;
	public float unkn1_90;
	public UndeterminedFieldType unkn1_91;
	public float unkn1_92;
	public UndeterminedFieldType unkn1_93;
	public UndeterminedFieldType unkn1_94;
	public UndeterminedFieldType unkn1_95;
	public float unkn1_96;
	public UndeterminedFieldType unkn1_97;
	public float unkn1_98;
	public UndeterminedFieldType unkn1_99;
	public float unkn1_100;
	public UndeterminedFieldType unkn1_101;
	public float unkn1_102;
	public UndeterminedFieldType unkn1_103;
	public UndeterminedFieldType unkn1_104;
	public UndeterminedFieldType unkn1_105;
	public UndeterminedFieldType unkn1_106;
	public UndeterminedFieldType unkn1_107;
	public float unkn1_108;
	public UndeterminedFieldType unkn1_109;
	public float unkn1_110;
	public UndeterminedFieldType unkn1_111;
	public UndeterminedFieldType unkn1_112;
	public uint unkn1_113;
	public UndeterminedFieldType unkn1_114;
	public UndeterminedFieldType unkn1_115;
	public float unkn1_116;
	public float unkn1_117;
	public uint uvs0PathCharCount;
	public uint uvs1PathCharCount;
	public uint uvs2PathCharCount;
	[RszInlineWString(nameof(uvs0PathCharCount))] public string? uvs0Path;
	[RszInlineWString(nameof(uvs1PathCharCount))] public string? uvs1Path;
	[RszInlineWString(nameof(uvs2PathCharCount))] public string? uvs2Path;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TextureUnitExpression, EfxVersion.DD2)]
public partial class EFXAttributeTextureUnitExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;
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

	public float unkn1;
	public float unkn2;
	public float unkn3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UVScroll, EfxVersion.RE4)]
public partial class EFXAttributeUVScroll : EFXAttribute
{
	public EFXAttributeUVScroll() : base(EfxAttributeType.UVScroll) { }

	public uint unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	public UndeterminedFieldType unkn1_6;
	public float unkn1_7;
	public UndeterminedFieldType unkn1_8;
	public float unkn1_9;
	public float unkn1_10;
	public float unkn1_11;
	public float unkn1_12;

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UVSequence, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeUVSequence : EFXAttribute
{
	public EFXAttributeUVSequence() : base(EfxAttributeType.UVSequence) { }

	public uint unkn1;//<comment = "Might	 be SequenceNum">;
	public uint unkn2;//<comment = "Might be SequenceNumVariation">;
	public uint startingFrame;
	public uint startingFrameRandom;
	public float animationSpeed;
	public float animationSpeedRandom;
	public uint mode;//<comment="0 - Show only starting frame,1 - Looped Animation, 2 - Play once and disappear after last frame, 3 - Play once and stay on last frame until end of duration in Life struct.">;
	[RszInlineWString] public string? uvsPath;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UVSequenceModifier, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeUVSequenceModifier : EFXAttribute
{
	public EFXAttributeUVSequenceModifier() : base(EfxAttributeType.UVSequenceModifier) { }

	public uint unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float minSpeed;
	public float minSpeedRandom;
	public float maxSpeed;
	public float maxSpeedRandom;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UVSequenceExpression, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeUVSequenceExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression => expressions;
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
	uint unkn1;
	float unkn2_0;
	float unkn2_1;
	float unkn2_2;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ShaderSettings, EfxVersion.RE7, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeShaderSettings : EFXAttribute
{
	public EFXAttributeShaderSettings() : base(EfxAttributeType.ShaderSettings) { }

	public float unkn1;
	public uint unkn2;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(sb_unkn1))]
	public float sb_unkn0;
	public float sb_unkn1;

	public float unkn3;
	public uint unkn4;
	[RszVersion("==", EfxVersion.RE7)]
	public uint unkn5;
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
	public uint unkn19;
    [RszVersion(EfxVersion.RERT, EndAt = nameof(sb_unkn5))]
	public float sb_unkn2;
	public float sb_unkn3;
	public float sb_unkn4;
	public float sb_unkn5;

    [RszVersion(EfxVersion.RE4)]
	public uint re4_unkn4;
    [RszVersion(EfxVersion.RE2)]
	public float unkn20;

    [RszVersion(EfxVersion.RE3, EndAt = nameof(sb_unkn8))]
	public float unkn21;
    [RszVersion(">", EfxVersion.RERT, EndAt = nameof(sb_unkn12))]
	public float sb_unkn9;
	public float sb_unkn10;
	public float sb_unkn11;
	public float sb_unkn12;
	public uint unkn22;
    [RszVersion(EfxVersion.DD2)] public uint dd2_unkn;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(sb_unkn8))]
	public float unkn23;
    [RszVersion("==", EfxVersion.RE8)]
	public float re8_unkn1;
    [RszVersion("==", EfxVersion.RE8)]
	public float re8_unkn2;
    [RszVersion("!=", EfxVersion.RE8)]
	public uint unkn24;
	public uint unkn25;

    [RszVersion(EfxVersion.RERT, EndAt = nameof(sb_unkn8))]
	public uint sb_unkn6;
	public uint sb_unkn7;
	public uint sb_unkn8;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ShaderSettingsExpression, EfxVersion.DMC5, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.DD2)]
public partial class EFXAttributeShaderSettingsExpression : RszTool.Efx.EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;
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

	[RszInlineWString] public string? efxPath;
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PlayEmitter, EfxVersion.DMC5, EfxVersion.RE4)]
public sealed partial class EFXAttributePlayEmitter : EFXAttribute, IDisposable
{
	public EFXAttributePlayEmitter() : base(EfxAttributeType.PlayEmitter) { }

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
		var start = handler.Position;
		handler.Skip(sizeof(uint));
		efxrData?.WriteTo(handler.WithOffset(handler.Position), false);
		handler.Write(start, (uint)(handler.Position - start));
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
}
