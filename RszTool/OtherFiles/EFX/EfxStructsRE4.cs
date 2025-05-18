using System.Numerics;
using RszTool;
using RszTool.Efx.Structs.Common;
using RszTool.Efx.Structs.RERT;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.RE4;

public enum ExpressionOperator
{
	Addition = 0,
	Subtraction = 1,
	Multiplication = 2,
	Division = 3,
	Assign = 4,
	ForceWord = -1
}

public enum DistortionType
{
	Blur = 0,
	Refract = 1,
	BlurTexture = 2
}

public enum BlendType
{
    AlphaBlend = 0,
    Physical = 1,
    AddContrast = 2,
    EdgeBlend = 3,
    Multiply = 4,
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform2D, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTransform2D : EFXAttribute
{
	public Vector2 position;
	public float rotation;
	public Vector2 scale;

	public EFXAttributeTransform2D() : base(EfxAttributeType.Transform2D) { }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform2DModifierDelayFrame, EfxVersion.RE4)]
public partial class EFXAttributeTransform2DModifierDelayFrame : EFXAttribute
{
	public EFXAttributeTransform2DModifierDelayFrame() : base(EfxAttributeType.Transform2DModifierDelayFrame) { }

	public uint frameDelay;
	public uint unkn1;

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform2DExpression, EfxVersion.RE2, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeTransform2DExpression : EFXAttribute
{
	public EFXAttributeTransform2DExpression() : base(EfxAttributeType.Transform2DExpression) { }

	public uint unkn1_0;
	public uint unkn1_1;
	[RszVersion("!=", EfxVersion.DMC5, EndAt = nameof(unkn1_5))]
	public uint unkn1_2;
	public uint unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform3D, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTransform3D : EFXAttribute
{
	public EFXAttributeTransform3D() : base(EfxAttributeType.Transform3D) { }

	public Vector3 translation;
	public Vector3 rotation;
	public Vector3 scale;
	public int rotationOrder;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform3DModifierDelayFrame, EfxVersion.RE4)]
public partial class EFXAttributeTransform3DModifierDelayFrame : EFXAttribute
{
	public EFXAttributeTransform3DModifierDelayFrame() : base(EfxAttributeType.Transform3DModifierDelayFrame) { }

	public uint frameDelay;
	public uint unkn1;

}
// [RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform3DModifier, EfxVersion.RE4, EfxVersion.DD2)]
// public partial class EFXAttributeTransform3DModifier : EFXAttribute
// {
// 	public EFXAttributeTransform3DModifier() : base(EfxAttributeType.Transform3DModifier) { }

// 	public uint unkn1_0;
// 	public uint unkn1_1;
// 	public float unkn1_2;
// 	public float unkn1_3;
// 	public float unkn1_4;
// 	public float unkn1_5;
// 	public float unkn1_6;
// 	public float unkn1_7;
// 	public float unkn1_8;
// 	public float unkn1_9;
// 	public float unkn1_10;
// 	public float unkn1_11;
// 	public float unkn1_12;
// 	public float unkn1_13;
// 	public float unkn1_14;
// 	public float unkn1_15;
// 	public float unkn1_16;
// 	public float unkn1_17;
// 	public float unkn1_18;
// 	public float unkn1_19;
// 	public UndeterminedFieldType unkn1_20;
// 	public float unkn1_21;
// 	public UndeterminedFieldType unkn1_22;
// 	public float unkn1_23;
// 	public UndeterminedFieldType unkn1_24;
// 	public float unkn1_25;
// 	public UndeterminedFieldType unkn1_26;
// 	public float unkn1_27;
// 	public UndeterminedFieldType unkn1_28;
// 	public float unkn1_29;
// 	public UndeterminedFieldType unkn1_30;
// 	public float unkn1_31;
// 	public float unkn1_32;
// 	public float unkn1_33;
// 	public UndeterminedFieldType unkn1_34;
// 	public float unkn1_35;
// 	public float unkn1_36;
// 	public float unkn1_37;
// 	public float unkn1_38;
// 	public float unkn1_39;
// 	public float unkn1_40;
// 	public float unkn1_41;
// 	public UndeterminedFieldType unkn1_42;
// 	public float unkn1_43;
// 	public UndeterminedFieldType unkn1_44;
// 	public float unkn1_45;
// 	public UndeterminedFieldType unkn1_46;
// 	public float unkn1_47;
// 	public UndeterminedFieldType unkn1_48;
// 	public float unkn1_49;
// 	public UndeterminedFieldType unkn1_50;
// 	public float unkn1_51;
// 	public UndeterminedFieldType unkn1_52;
// 	public float unkn1_53;
// 	public UndeterminedFieldType unkn1_54;
// }
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform3DExpression, EfxVersion.DMC5, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTransform3DExpression : EFXAttribute
{
	public EFXAttributeTransform3DExpression() : base(EfxAttributeType.Transform3DExpression) { }

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
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	[RszClassInstance] public EFXExpressionListWrapperBase? expressions;
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeBillboard2DExpression, EfxVersion.RE7, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTypeBillboard2DExpression : EFXAttribute
{
	public EFXAttributeTypeBillboard2DExpression() : base(EfxAttributeType.TypeBillboard2DExpression) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	[RszVersion(EfxVersion.RE3, EndAt = nameof(unkn1_13))]
	public uint unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
    public uint unkn1_6;
    public uint unkn1_7;
    public uint unkn1_8;
    public uint unkn1_9;
    public uint unkn1_10;
    public uint unkn1_11;
    public uint unkn1_12;
	[RszVersion(">", EfxVersion.RERT)]
    public uint unkn1_13;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	[RszClassInstance] public EFXExpressionListWrapperBase? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Life, EfxVersion.RE7, EfxVersion.DMC5, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeLife : EFXAttribute
{
	public uint AppearFrameMin;
	public uint AppearFrameMax;
	public uint KeepFrameMin;
	public uint KeepFrameMax;
	public uint VanishFrameMin;
	public uint VanishFrameMax;
	public uint unkn7; // Might be KeepHoldFrame
	public uint unkn8; // Might be KeepHoldFrameVariation
	[RszVersion(EfxVersion.RE2)]
	public uint unkn9;

	public EFXAttributeLife() : base(EfxAttributeType.Life) { }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeNoDraw, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTypeNoDraw : EFXAttribute
{
	public EFXAttributeTypeNoDraw() : base(EfxAttributeType.TypeNoDraw) { }

	public uint unkn1_0;
	public via.Color color0;
	public via.Color color1;
	public uint unkn1_3;

	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
	public float unkn1_8;
	public float unkn1_9;
	public float unkn1_10;
	public float unkn1_11;
	public float unkn1_12;
	public float unkn1_13;
	public float unkn1_14;
	public float unkn1_15;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterShape3D, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeEmitterShape3D : EFXAttribute
{
	public EFXAttributeEmitterShape3D() : base(EfxAttributeType.EmitterShape3D) { }

	public float RangeXMin;
	public float RangeXMax;
	public float RangeYMin;
	public float RangeYMax;
	public float RangeZMin;
	public float RangeZMax;
	public uint ShapeType;
	public uint RangeDivideNum;
	public uint RangeDivideAxis;
	[RszVersion(">", EfxVersion.RE3, EndAt = nameof(unkn1_10))]
	public uint emitCount;
	public uint unkn1_10;
	public float LocalRotationX;
	public float LocalRotationY;
	public float LocalRotationZ;
	public uint RotationOrder;
	[RszVersion(EfxVersion.RE2, EndAt = nameof(unkn4_3))]
	[RszVersion(nameof(Version), "==", EfxVersion.MHRiseSB, "||", nameof(Version), "==", EfxVersion.RERT, "||", nameof(Version), "==", EfxVersion.RE8)]
	public byte unkn2;
	public uint unkn3;
	[RszVersion(EfxVersion.RE4)]
	public uint re4_unkn0;
	public float unkn4_0;
	public float unkn4_1;
	public float unkn4_2;
	public float unkn4_3;
}



[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Attractor, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeAttractor : EFXAttribute, IBoneRelationAttribute
{
	public EFXAttributeAttractor() : base(EfxAttributeType.Attractor) { }

	public uint unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.AttractorExpression, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeAttractorExpression : EFXAttribute
{
	public EFXAttributeAttractorExpression() : base(EfxAttributeType.AttractorExpression) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	public uint unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
	public uint unkn1_6;
	public uint unkn1_7;
	public uint unkn1_8;
	public uint unkn1_9;
	public uint unkn1_10;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.DistortionExpression, EfxVersion.RE4)]
public partial class EFXAttributeDistortionExpression : EFXAttribute
{
	public EFXAttributeDistortionExpression() : base(EfxAttributeType.DistortionExpression) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	public uint unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	[RszClassInstance] public EFXExpressionListWrapperBase? expressions;
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
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterColor, EfxVersion.RE4)]
public partial class EFXAttributeEmitterColor : EFXAttribute
{
	public EFXAttributeEmitterColor() : base(EfxAttributeType.EmitterColor) { }

	public uint unkn1;
	public uint unkn2;
	public via.Color color;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterPriority, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeEmitterPriority : EFXAttribute
{
	public EFXAttributeEmitterPriority() : base(EfxAttributeType.EmitterPriority) { }

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterShape2D, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeEmitterShape2D : EFXAttribute
{
	public EFXAttributeEmitterShape2D() : base(EfxAttributeType.EmitterShape2D) { }

	public float unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
	public uint unkn1_6;
	public float unkn1_7;

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterShape2DExpression, EfxVersion.RE4)]
public partial class EFXAttributeEmitterShape2DExpression : EFXAttribute
{
	public EFXAttributeEmitterShape2DExpression() : base(EfxAttributeType.EmitterShape2DExpression) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	public uint unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
	[RszClassInstance] public EFXExpressionListWrapper expressions = new();

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterShape3DExpression, EfxVersion.RE7, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE4)]
public partial class EFXAttributeEmitterShape3DExpression : EFXAttribute
{
	public EFXAttributeEmitterShape3DExpression() : base(EfxAttributeType.EmitterShape3DExpression) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	public uint unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
	public uint unkn1_6;
	[RszVersion('>', EfxVersion.RE7)]
	public uint unkn1_7;
	[RszVersion('>', EfxVersion.RE3, EndAt = nameof(unkn1_9))]
	public uint unkn1_8;
	public uint unkn1_9;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	[RszClassInstance] public EFXExpressionListWrapperBase? expressions;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FadeByAngle, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeFadeByAngle : EFXAttribute
{
	public EFXAttributeFadeByAngle() : base(EfxAttributeType.FadeByAngle) { }

	public uint unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FadeByAngleExpression, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeFadeByAngleExpression : EFXAttribute
{
	public EFXAttributeFadeByAngleExpression() : base(EfxAttributeType.FadeByAngleExpression) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
	[RszClassInstance] public EFXExpressionListWrapper expressions = new();
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FadeByDepth, EfxVersion.DMC5, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeFadeByDepth : EFXAttribute
{
	public EFXAttributeFadeByDepth() : base(EfxAttributeType.FadeByDepth) { }

	public float unkn1; // are these uints in DMC5?
	public float unkn2;
	public float unkn3;
	public float unkn4;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FadeByDepthExpression, EfxVersion.DMC5, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeFadeByDepthExpression : EFXAttribute
{
	public EFXAttributeFadeByDepthExpression() : base(EfxAttributeType.FadeByDepthExpression) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	public uint unkn1_3;
	public uint unkn1_4;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FadeByEmitterAngle, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeFadeByEmitterAngle : EFXAttribute
{
	public EFXAttributeFadeByEmitterAngle() : base(EfxAttributeType.FadeByEmitterAngle) { }

	public uint unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	[RszVersion(EfxVersion.DD2)]
	public UndeterminedFieldType unkn1_6;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FadeByOcclusion, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeFadeByOcclusion : EFXAttribute
{
	public EFXAttributeFadeByOcclusion() : base(EfxAttributeType.FadeByOcclusion) { }

	public float unkn1;
	public uint unkn2;
	public float unkn3;
	public float unkn4;

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.IgnorePlayerColor, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RERT)]
public partial class EFXAttributeIgnorePlayerColor : EFXAttribute
{
	public EFXAttributeIgnorePlayerColor() : base(EfxAttributeType.IgnorePlayerColor) { }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.LifeExpression, EfxVersion.RE7, EfxVersion.DMC5, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeLifeExpression : EFXAttribute
{
	public EFXAttributeLifeExpression() : base(EfxAttributeType.LifeExpression) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	public uint unkn1_3;
	[RszVersion(nameof(Version), ">=", EfxVersion.DD2, "||", nameof(Version), "==", EfxVersion.RE7, EndAt = nameof(dd2_unkn3))]
	public uint dd2_unkn1;
	public uint dd2_unkn2;
	public uint dd2_unkn3;

	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	[RszClassInstance] public EFXExpressionListWrapperBase? expressions;

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
public partial class EFXAttributeNoiseExpression : EFXAttribute
{
	public EFXAttributeNoiseExpression() : base(EfxAttributeType.NoiseExpression) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public uint unkn4;
    public uint unkn5;
    public uint unkn6;
    public uint unkn7;
    public uint unkn8;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PlayEfx, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributePlayEfx : EFXAttribute
{
	public EFXAttributePlayEfx() : base(EfxAttributeType.PlayEfx) { }

	[RszInlineWString] public string? efxPath;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ProceduralDistortionDelayFrame, EfxVersion.RE4)]
public partial class EFXAttributeProceduralDistortionDelayFrame : EFXAttribute
{
	public EFXAttributeProceduralDistortionDelayFrame() : base(EfxAttributeType.ProceduralDistortionDelayFrame) { }

	public uint frameDelay;
	public UndeterminedFieldType unkn;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtColor, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributePtColor : EFXAttribute
{
	public EFXAttributePtColor() : base(EfxAttributeType.PtColor) { }

	public uint unkn1;
	public uint unkn2;
	public via.Color color0;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtLife, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributePtLife : EFXAttribute
{
	public EFXAttributePtLife() : base(EfxAttributeType.PtLife) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public int actionIndex;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtSort, EfxVersion.RE8, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributePtSort : EFXAttribute
{
	public EFXAttributePtSort() : base(EfxAttributeType.PtSort) { }

	public uint unkn;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtTransform3D, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributePtTransform3D : EFXAttribute
{
	public EFXAttributePtTransform3D() : base(EfxAttributeType.PtTransform3D) { }

	public float unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public UndeterminedFieldType unkn1_4;
	public UndeterminedFieldType unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
	public float unkn1_8;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtUvSequence, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE4)]
public partial class EFXAttributePtUvSequence : EFXAttribute
{
	public EFXAttributePtUvSequence() : base(EfxAttributeType.PtUvSequence) { }

	public uint unkn1_0;
	public uint unkn1_1;
	[RszVersion("<=", EfxVersion.RE3)]
	public uint unkn1_2;
	public float unkn1_3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtVelocity3D, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributePtVelocity3D : EFXAttribute
{
	public EFXAttributePtVelocity3D() : base(EfxAttributeType.PtVelocity3D) { }

	public float unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	// [RszVersion(EfxVersion.DD2)]
	// public float unkn1_5;

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.RenderTarget, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeRenderTarget : EFXAttribute
{
	public EFXAttributeRenderTarget() : base(EfxAttributeType.RenderTarget) { }

	public uint unkn_toggle;
	[RszInlineWString] public string? rtexPath;

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.RgbCommon, EfxVersion.RE4)]
public partial class EFXAttributeRgbCommon : EFXAttribute
{
	public EFXAttributeRgbCommon() : base(EfxAttributeType.RgbCommon) { }

	[RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn3))]
    public uint re4_unkn0;
    public uint re4_unkn1;
    public uint re4_unkn2;
    public uint re4_unkn3;

	public via.Color color0;
	public float unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public via.Color unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
	public uint unkn1_7;
	public uint unkn1_8;
	public uint unkn1_9;
	public uint unkn1_10;
	public uint unkn1_11;
	public uint unkn1_12;
	public uint unkn1_13;
	public uint unkn1_14;
	public uint unkn1_15;
	public uint unkn1_16;
	public uint unkn1_17;
	public uint unkn1_18;
	public uint unkn1_19;
	public uint unkn1_20;
	public uint unkn1_21;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.RotateAnim, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeRotateAnim : EFXAttribute
{
	public EFXAttributeRotateAnim() : base(EfxAttributeType.RotateAnim) { }

	public uint unkn1_0;
	public float rotateSpeedX;
	public float rotateSpeedXRandom;
	public float rotateSpeedY;
	public float rotateSpeedYRandom;
	public float rotateSpeedZ;
	public float rotateSpeedZRandom;
	public float unkn4_2;
	public float unkn4_3;
	public float unkn4_4;
	public float unkn4_5;
	public float unkn4_6;
	public float unkn4_7;
	[RszVersion(EfxVersion.DD2, EndAt = nameof(unkn4_9))]
	public uint unkn4_8;
	public uint unkn4_9;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.RotateAnimDelayFrame, EfxVersion.RE4)]
public partial class EFXAttributeRotateAnimDelayFrame : EFXAttribute
{
	public EFXAttributeRotateAnimDelayFrame() : base(EfxAttributeType.RotateAnimDelayFrame) { }

	public uint frameDelay;
	public uint unkn2;

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.RotateAnimExpression, EfxVersion.RE7, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeRotateAnimExpression : EFXAttribute
{
	public EFXAttributeRotateAnimExpression() : base(EfxAttributeType.RotateAnimExpression) { }

	public uint ukn1_0;
	public uint ukn1_1;
	public uint ukn1_2;
	public uint ukn1_3;
	public uint ukn1_4;
	public uint ukn1_5;
	public uint ukn1_6;
	public uint ukn1_7;
	public uint ukn1_8;
	public uint ukn1_9;
	public uint ukn1_10;
	public uint ukn1_11;
	public uint ukn1_12;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ScaleAnim, EfxVersion.DMC5, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeScaleAnim : EFXAttribute
{
	public EFXAttributeScaleAnim() : base(EfxAttributeType.ScaleAnim) { }

	public float minScale;
	public float unkn1_1;
	public float maxScale;
	public float unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
	public float unkn1_8;
	public float unkn1_9;
	public float unkn1_10;
	public float unkn1_11;
	public float unkn1_12;
	public float unkn1_13;
	public float unkn1_14;
	public float unkn1_15;
	[RszVersion(EfxVersion.DD2, EndAt = nameof(unkn1_17))]
	public uint unkn1_16;
	public uint unkn1_17;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ScaleAnimDelayFrame, EfxVersion.RE4)]
public partial class EFXAttributeScaleAnimDelayFrame : EFXAttribute
{
	public EFXAttributeScaleAnimDelayFrame() : base(EfxAttributeType.ScaleAnimDelayFrame) { }

	public uint frameDelay;
	public uint unkn2;

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ScaleAnimExpression, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeScaleAnimExpression : EFXAttribute
{
	public EFXAttributeScaleAnimExpression() : base(EfxAttributeType.ScaleAnimExpression) { }

	public uint unkn1;
	public uint unkn2;
	public uint unkn3;
    [RszVersion(EfxVersion.RERT, EndAt = nameof(sb_unkn5))]
	public uint sb_unkn0;
	public uint sb_unkn1;
	public uint sb_unkn2;
	public uint sb_unkn3;
	public uint sb_unkn4;
	public uint sb_unkn5;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ScaleByDepth, EfxVersion.RE4)]
public partial class EFXAttributeScaleByDepth : EFXAttribute
{
	public EFXAttributeScaleByDepth() : base(EfxAttributeType.ScaleByDepth) { }

	public uint unkn1;
	public float unkn2_0;
	public float unkn2_1;
	public float unkn2_2;
	public float unkn2_3;
	public float unkn2_4;
	public float unkn2_5;

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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.SpawnExpression, EfxVersion.RE3, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeSpawnExpression : EFXAttribute
{
	public EFXAttributeSpawnExpression() : base(EfxAttributeType.SpawnExpression) { }

	public uint flags;
	ExpressionOperator spawnNum;
	ExpressionOperator spawnNumRange;
	ExpressionOperator intervalFrame;
	ExpressionOperator intervalFrameRange;
	[RszVersion(EfxVersion.RE3, EndAt = nameof(emitterDelayFrameRange))]
	ExpressionOperator emitterDelayFrame;
	ExpressionOperator emitterDelayFrameRange;
    [RszVersion(EfxVersion.RERT)]
	ExpressionOperator sb_unkn0;

	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	[RszClassInstance] public EFXExpressionListWrapperBase? expressions;
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TextureFilter, EfxVersion.RE4)]
public partial class EFXAttributeTextureFilter : EFXAttribute
{
	public EFXAttributeTextureFilter() : base(EfxAttributeType.TextureFilter) { }

	public float unkn1;
	public float unkn2;
	public float unkn3;

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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeBillboard3D, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTypeBillboard3D : EFXAttribute
{
	public EFXAttributeTypeBillboard3D() : base(EfxAttributeType.TypeBillboard3D) { }

	public uint unkn1;
	public via.Color color0;
	public via.Color color1;
	public float unknOpacityValue;

	public float ColorRate; // 1
	public float AlphaRate;
	public float Rotation;
	public float RotationVariation;
	public float SizeScaler;
	public float SizeScalerVariation;
	public float SizeX;
	public float SizeXVariation;
	public float SizeY;
	public float SizeYVariation; // 10

    public float unkn13;
	[RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;

	public uint unkn14;
    [RszVersion(EfxVersion.RE2, EndAt = nameof(unkn16))]
    public float unkn15;
	public float unkn16;

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeBillboard3DExpression, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTypeBillboard3DExpression : EFXAttribute
{
	public EFXAttributeTypeBillboard3DExpression() : base(EfxAttributeType.TypeBillboard3DExpression) { }

    [RszVersion(EfxVersion.RE4)]
	public uint re4_unkn0;

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	public uint unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
	public uint unkn1_6;
	public uint unkn1_7;
    [RszVersion(EfxVersion.RE2)]
	public uint unkn1_8;
    [RszVersion(EfxVersion.RE3, EndAt = nameof(unkn1_12))]
	public uint unkn1_9;
	public uint unkn1_10;
	public uint unkn1_11;
	public uint unkn1_12;

	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	[RszClassInstance] public EFXExpressionListWrapperBase? expressions;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuBillboard, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT)]
public partial class EFXAttributeTypeGpuBillboard : EFXAttribute
{
	public EFXAttributeTypeGpuBillboard() : base(EfxAttributeType.TypeGpuBillboard) { }

	public uint unkn1;
	public uint unkn2;
    [RszVersion(EfxVersion.RE8)]
	public uint unkn3;
	public via.Color color0;
	public via.Color color1;
	public float emission;
	public float unkn5;
	public float unkn6;
	public float unkn7;
	public float unkn8;
	public float effectScale;
	public float unkn10;
	public float unkn11;

    [RszVersion(EfxVersion.RERT, EndAt = nameof(sb_unkn7))]
	public float sb_unkn0;
	public float sb_unkn1;
	public float sb_unkn2;
	public float sb_unkn3;
	public float sb_unkn4;
	public float sb_unkn5;
    [RszVersion("==", EfxVersion.RERT, EndAt = nameof(sb_unkn7))]
	public float sb_unkn6;
	public float sb_unkn7;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuBillboardExpression, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTypeGpuBillboardExpression : EFXAttribute
{
	public EFXAttributeTypeGpuBillboardExpression() : base(EfxAttributeType.TypeGpuBillboardExpression) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	public uint unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
	public uint unkn1_6;
	public uint unkn1_7;
    [RszVersion(EfxVersion.RERT, EndAt = nameof(sb_unkn5))]
	public uint sb_unkn0;
	public uint sb_unkn1;
	public uint sb_unkn2;
	public uint sb_unkn3;
	public uint sb_unkn4;
	public uint sb_unkn5;
    [RszVersion(EfxVersion.DD2, EndAt = nameof(dd2_unkn5))]
	public uint dd2_unkn1;
	public uint dd2_unkn2;
	public uint dd2_unkn3;
	public uint dd2_unkn4;
	public uint dd2_unkn5;

	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuLightning3D, EfxVersion.RE4)]
public partial class EFXAttributeTypeGpuLightning3D : EFXAttribute
{
	public EFXAttributeTypeGpuLightning3D() : base(EfxAttributeType.TypeGpuLightning3D) { }

	public uint unkn1;
	public via.Color color0;
	public via.Color color1;
	public int unkn2_0;
	public float unkn2_1;
	public UndeterminedFieldType unkn2_2;
	public UndeterminedFieldType unkn2_3;
	public float unkn2_4;
	public uint unkn2_5;
	public uint unkn2_6;
	public UndeterminedFieldType unkn2_7;
	public float unkn2_8;
	public UndeterminedFieldType unkn2_9;
	public float unkn2_10;
	public UndeterminedFieldType unkn2_11;
	public float unkn2_12;
	public float unkn2_13;
	public UndeterminedFieldType unkn2_14;
	public float unkn2_15;
	public UndeterminedFieldType unkn2_16;
	public float unkn2_17;
	public UndeterminedFieldType unkn2_18;
	public float unkn2_19;
	public UndeterminedFieldType unkn2_20;
	public UndeterminedFieldType unkn2_21;
	public UndeterminedFieldType unkn2_22;
	public UndeterminedFieldType unkn2_23;
	public UndeterminedFieldType unkn2_24;
	public UndeterminedFieldType unkn2_25;
	public UndeterminedFieldType unkn2_26;
	public UndeterminedFieldType unkn2_27;
	public float unkn2_28;
	public UndeterminedFieldType unkn2_29;
	public UndeterminedFieldType unkn2_30;
	public float unkn2_31;
	public float unkn2_32;
	public UndeterminedFieldType unkn2_33;
	public float unkn2_34;
	public float unkn2_35;
	public float unkn2_36;
	public uint unkn2_37;
	public uint unkn2_38;
	public float unkn2_39;
	public float unkn2_40;
	public uint unkn2_41;
	public float unkn2_42;
	public float unkn2_43;
	public float unkn2_44;
	public float unkn2_45;
	public float unkn2_46;
	public float unkn2_47;
	public uint unkn2_48;
	public uint unkn2_49;
	public uint unkn2_50;
	public uint unkn2_51;
	public uint unkn2_52;
	public uint unkn2_53;
	public uint unkn2_54;
	public float unkn2_55;
	public float unkn2_56;
	public float unkn2_57;
	public float unkn2_58;
	public float unkn2_59;
	public float unkn2_60;
	public uint unkn2_61;
	public uint unkn2_62;
	public uint unkn2_63;
	public uint unkn2_64;
	public uint unkn2_65;
	public uint unkn2_66;
	public uint unkn2_67;
	public float unkn2_68;
	public float unkn2_69;
	public float unkn2_70;
	public float unkn2_71;
	public float unkn2_72;
	public float unkn2_73;
	public uint unkn2_74;
	public uint unkn2_75;
	public uint unkn2_76;
	public uint unkn2_77;
	public uint unkn2_78;
	public uint unkn2_79;
	public uint unkn2_80;

	public uint str1Len;
	public uint str2Len;
	public uint str3Len;
	[RszInlineWString(-1)] public string? str1;
	[RszInlineWString(-1)] public string? str2;
	[RszInlineWString(-1)] public string? str3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuPolygon, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT)]
public partial class EFXAttributeTypeGpuPolygon : EFXAttribute
{
	public EFXAttributeTypeGpuPolygon() : base(EfxAttributeType.TypeGpuPolygon) { }

	public uint unkn1;
	public uint unkn2;
	[RszVersion('>', EfxVersion.RE3)]
	public uint unkn3;
	public via.Color color0;
	public via.Color color1;
	public via.Color color2;
	public via.Color color3;
	public float unkn4_2;
	public float unkn4_3;
	public float unkn4_4;
	public float unkn4_5;
	public float unkn4_6;
	public float unkn4_7;
	public float unkn4_8;
	public float unkn4_9;
	[RszVersion("<", EfxVersion.RERT, EndAt = nameof(unkn4_15))]
	public float unkn4_10;
	public float unkn4_11;
	public float unkn4_12;
	public float unkn4_13;
	public float unkn4_14;
	public float unkn4_15;

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuRibbonFollow, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE4)]
public partial class EFXAttributeTypeGpuRibbonFollow : EFXAttribute
{
	public EFXAttributeTypeGpuRibbonFollow() : base(EfxAttributeType.TypeGpuRibbonFollow) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	public uint unkn1_3;
	[RszVersion('>', EfxVersion.DMC5)]
	public uint unkn1_4;
	public via.Color color0;
	public via.Color color1;
	public float unkn2_0;

	public float unkn2_1;
	public float unkn2_2;
	public float unkn2_3;
	public float unkn2_4;
	public float unkn2_5;
	public float unkn2_6;
	[RszVersion('>', EfxVersion.RE3)]
	public float unkn2_7;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuRibbonLength, EfxVersion.RE8)]
public partial class EFXAttributeTypeGpuRibbonLength : EFXAttribute
{
	public EFXAttributeTypeGpuRibbonLength() : base(EfxAttributeType.TypeGpuRibbonLength) { }

	public uint unkn1;
	public uint unkn2;
	public via.Color color0;
	public via.Color color1;
	public float unkn3_0;
	public float unkn3_1;
	public uint unkn3_2;
	public float unkn3_3;
	public float unkn3_4;
	public float unkn3_5;
	public float unkn3_6;
	public float unkn3_7;
	public float unkn3_8;
	public uint unkn3_9;
	public float unkn3_10;
	public float unkn3_11;
	public uint unkn3_12;
	public float unkn3_13;
	public float unkn3_14;
	public uint unkn3_15;
	public uint unkn3_16;
	public float unkn3_17;
	public uint unkn3_18;
	public float unkn3_19;
	public uint unkn3_20;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeLightning3D, EfxVersion.MHRiseSB, EfxVersion.DD2)]
public partial class EFXAttributeTypeLightning3D : EFXAttribute, IBoneRelationAttribute
{
	public EFXAttributeTypeLightning3D() : base(EfxAttributeType.TypeLightning3D) { }

	// MHR: The unknBitFlag values change between base game and sunbreak, even on files that have no changes. Game will crash when value is set wrong.
	// DMC5: likely determines some nested struct type (shape?), as the value types are sometimes int and sometimes float: vfx\effectediter\efd_cutscene\efd_cs_mission01\efd_m01_100\efd_03_cs_m01_100_00_021.efx.1769672
	public uint unknBitFlag;
	public via.Color color0;
	public via.Color color1;
	public float unkn2_0;
	public float unkn2_1;
	public float unkn2_2;
	public float unkn2_3;
	[RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;

    public float unkn2_4;
	public float unkn2_5;
	public float unkn2_6;
	public float unkn2_7;
	public float unkn2_8;

	// this section types seem to change - likely some dynamic struct based on the bit flag
	public UndeterminedFieldType unkn2_9;
	public UndeterminedFieldType unkn2_10;
	public UndeterminedFieldType unkn2_11;
	public UndeterminedFieldType unkn2_12;
	public UndeterminedFieldType unkn2_13;
	public UndeterminedFieldType unkn2_14;
	public UndeterminedFieldType unkn2_15;

	public float unkn2_16;
	public float unkn2_17;
	public float unkn2_18;
	public float unkn2_19;
	public float unkn2_20;
	public float unkn2_21;
	public float unkn2_22;
	public float unkn2_23;
	public float unkn2_24;
	public float unkn2_25;
	public float unkn2_26;
	public float unkn2_27;
	public float unkn2_28;
	public float unkn2_29;
	public float unkn2_30;
	public float unkn2_31;
	public float unkn2_32;
	public float unkn2_33;
	public float unkn2_34;
	public float unkn2_35;
	public float unkn2_36;
	public float unkn2_37;
	public float unkn2_38;
	public float unkn2_39;
	public float unkn2_40;
	public float unkn2_41;
	public float unkn2_42;
	public float unkn2_43;
	public float unkn2_44;
	public float unkn2_45;
	public float unkn2_46;
	public float unkn2_47;
	public uint unkn2_48;
	public uint unkn2_49;
	public float unkn2_50;
	public float unkn2_51;
	public float unkn2_52;
	public float unkn2_53;
	public float unkn2_54;
	public float unkn2_55;
	[RszVersion(EfxVersion.RE3, EndAt = nameof(unkn2_72))]
	public float unkn2_56;
	public float unkn2_57;
	public float unkn2_58;
	public float unkn2_59;
	public float unkn2_60;
	public float unkn2_61;
	public float unkn2_62;
	public float unkn2_63;
	[RszVersion(EfxVersion.DD2)]
	public float dd2_unkn;
	public uint unkn2_64;
	public uint unkn2_65;
	public uint unkn2_66;
	public float unkn2_67;
	public float unkn2_68;
	public float unkn2_69;
	public uint unkn2_70;

	public float unkn2_71;
	public float unkn2_72;
	[RszInlineWString] public string? boneName;

    public string? ParentBone { get; set; }
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeLightning3DExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeLightning3DExpression : EFXAttribute
{
	public EFXAttributeTypeLightning3DExpression() : base(EfxAttributeType.TypeLightning3DExpression) { }

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
	public uint unkn19;
	public uint unkn20;
	public uint unkn21;
	public uint unkn22;
	public uint unkn23;
	public uint unkn24;
	public uint unkn25;
	public uint unkn26;
	public uint unkn27;
	public uint unkn28;
	public uint unkn29;

	[RszVersion(EfxVersion.DD2, EndAt = nameof(unkn49))]
    public uint unkn30;
    public uint unkn31;
    public uint unkn32;
    public uint unkn33;
    public uint unkn34;
    public uint unkn35;
    public uint unkn36;
    public uint unkn37;
    public uint unkn38;
    public uint unkn39;
    public uint unkn40;
    public uint unkn41;
    public uint unkn42;
    public uint unkn43;
    public uint unkn44;
    public uint unkn45;
    public uint unkn46;
    public uint unkn47;
    public uint unkn48;
    public uint unkn49;

	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeNoDrawExpression, EfxVersion.RE3, EfxVersion.RE4)]
public partial class EFXAttributeTypeNoDrawExpression : EFXAttribute
{
	public EFXAttributeTypeNoDrawExpression() : base(EfxAttributeType.TypeNoDrawExpression) { }

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
	[RszVersion('>', EfxVersion.RE3, EndAt = nameof(unkn16))]
    public uint unkn15;
    public uint unkn16;

	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	[RszClassInstance] public EFXExpressionListWrapperBase? expressions;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeNodeBillboard, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RERT, EfxVersion.DD2)]
public partial class EFXAttributeTypeNodeBillboard : EFXAttribute
{
	public EFXAttributeTypeNodeBillboard() : base(EfxAttributeType.TypeNodeBillboard) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	[RszVersion(EfxVersion.RE2)]
	public uint unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
	public float unkn1_8;
	public float unkn1_9;

	public via.Color color1;
	[RszVersion(EfxVersion.RE2)]
	public via.Color color2;
	public float unkn2_1;
	[RszVersion(EfxVersion.RE2, EndAt = nameof(re2_unkn1_2))]
	public float re2_unkn1_1;
	public float re2_unkn1_2;
	public float unkn2_2;
	public float unkn2_3;
	public float unkn2_4;
	public float unkn2_5;
	public float unkn2_6;
	public float unkn2_7;

	public via.Color color3;
	[RszVersion(EfxVersion.RE2)]
	public via.Color color4;
	public float unkn3_1;
	[RszVersion(EfxVersion.RE2, EndAt = nameof(re2_unkn2_2))]
	public float re2_unkn2_1;
	public float re2_unkn2_2;
	public float unkn3_2;
	public float unkn3_3;
	public float unkn3_4;
	public float unkn3_5;
	public float unkn3_6;
	public float unkn3_7;

	public via.Color color5;
	[RszVersion(EfxVersion.RE2)]
	public via.Color color6;
	public float unkn4_1;
	[RszVersion(EfxVersion.RE2, EndAt = nameof(re2_unkn3_2))]
	public float re2_unkn3_1;
	public float re2_unkn3_2;
	public float unkn4_2;
	public float unkn4_3;
	public float unkn4_4;
	public float unkn4_5;
	public float unkn4_6;
	public float unkn4_7;

	public via.Color color7;
	[RszVersion(EfxVersion.RE2)]
	public via.Color color8;
	public float unkn5_1;
	[RszVersion(EfxVersion.RE2, EndAt = nameof(re2_unkn4_2))]
	public float re2_unkn4_1;
	public float re2_unkn4_2;
	public float unkn5_2;
	public float unkn5_3;
	public float unkn5_4;
	public float unkn5_5;

	public uint unkn1_52;
	public float unkn1_53;
	public float unkn1_54;
	public float unkn1_55;
	public float unkn1_56;
	public uint unkn1_57;
	public uint unkn1_58;
	[RszVersion(EfxVersion.RERT, EndAt = nameof(unkn1_60))]
	public uint unkn1_59;
	public uint unkn1_60;
	public float unkn1_61;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeNodeBillboardExpression, EfxVersion.DMC5, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeTypeNodeBillboardExpression : EFXAttribute
{
	public EFXAttributeTypeNodeBillboardExpression() : base(EfxAttributeType.TypeNodeBillboardExpression) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	public uint unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
	public uint unkn1_6;
	public uint unkn1_7;
	public uint unkn1_8;
	public uint unkn1_9;
	public uint unkn1_10;
	public uint unkn1_11;
	public uint unkn1_12;
	public uint unkn1_13;
	public uint unkn1_14;
	public uint unkn1_15;
	public uint unkn1_16;
	public uint unkn1_17;
	public uint unkn1_18;
	public uint unkn1_19;
	public uint unkn1_20;
	public uint unkn1_21;
	public uint unkn1_22;
	public uint unkn1_23;
	public uint unkn1_24;
	public uint unkn1_25;
	public uint unkn1_26;
	public uint unkn1_27;
	public uint unkn1_28;
	public uint unkn1_29;
	public uint unkn1_30;
	public uint unkn1_31;
	public uint unkn1_32;
	public uint unkn1_33;
	public uint unkn1_34;
	public uint unkn1_35;
	public uint unkn1_36;
	public uint unkn1_37;
	public uint unkn1_38;
	public uint unkn1_39;
	public uint unkn1_40;
	public uint unkn1_41;
	public uint unkn1_42;
	public uint unkn1_43;
	[RszClassInstance] public EFXExpressionListWrapper expressions = new();
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygon, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTypePolygon : EFXAttribute
{
	public EFXAttributeTypePolygon() : base(EfxAttributeType.TypePolygon) { }

	public uint mFlags;
	public via.Color color0;
	public via.Color color1;
    public float unkn2;

	public float unkn3;
	public float unkn4_0;
	[RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn1))]
	public float re4_unkn0;
	public uint re4_unkn1;
	public uint unkn4_1;
    public float unkn4_3;
	public float unkn4_4;
	public float unkn4_5;
	public float unkn4_6;
	public float unkn4_7;
	public float unkn4_8;
	public float unkn4_9;
	public float unkn4_10;
	public float unkn4_11;
	public float unkn4_12;
	public float unkn4_13;
	public float unkn4_14;
	public float unkn4_15;
	public float unkn4_16;
    [RszVersion('<', EfxVersion.RE4)]
	public float unkn4_17;
    [RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;
    [RszVersion(EfxVersion.RE4)]
	public uint re4_unkn2;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygonTrail, EfxVersion.RE2, EfxVersion.RERT)]
public partial class EFXAttributeTypePolygonTrail : EFXAttribute
{
	public EFXAttributeTypePolygonTrail() : base(EfxAttributeType.TypePolygonTrail) { }

	public uint unkn1;
	public via.Color color1;
	public via.Color color2;
	public float unkn4;

	public float unkn5;
	public float unkn6;
	public float unkn7;
    [RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;
	[RszVersion(EfxVersion.DD2)]
	public float dd2_unkn;

	public uint unkn8;
	public float unkn9;
	public float unkn10;
	public float unkn11;
	public uint unkn12;
	public uint unkn13;
	public uint unkn14;
	public uint unkn15;
	public via.Color color3;
	public via.Color color4;
    public via.Color color5;
	public float unkn19;
	public float unkn20;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFixEnd, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonFixEnd : EFXAttribute
{
	public EFXAttributeTypeRibbonFixEnd() : base(EfxAttributeType.TypeRibbonFixEnd) { }

	public uint unkn1;
	public via.Color color0;
	public via.Color color1;
	public float unkn2_0;
	public float unkn2_1;
	public float unkn2_2;
	public float unkn2_3;
    [RszVersion(EfxVersion.RE4)]
	public uint re4_unkn0;
	public float unkn2_4;
	public float unkn2_5;
	public float unkn2_6;
	public float unkn2_7;
	public float unkn2_8;
	[RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;
    [RszVersion(EfxVersion.DD2)]
	public float dd2_unkn0;
    public uint unkn2_9;
	public float unkn2_10;
	public float unkn2_11;
	public uint unkn2_12;
	public uint unkn2_13;
	public uint unkn2_14;
	public float unkn2_15;
	public int unkn2_16;
	public int unkn2_17;
	public int unkn2_18;
	public float unkn2_19;
	public float unkn2_20;
	public float unkn2_21;
	public float unkn2_22;
	public float unkn2_23;
	public float unkn2_24;
	public float unkn2_25;
    [RszVersion(EfxVersion.RE3, EndAt = nameof(unkn2_27))]
	public float unkn2_26;
	public float unkn2_27;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFollow, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeTypeRibbonFollow : EFXAttribute
{
	public EFXAttributeTypeRibbonFollow() : base(EfxAttributeType.TypeRibbonFollow) { }

	public uint emission;
	public via.Color color0;
	public via.Color color1;
	public float unkn4;
	public float unkn5;
	public float unkn6;
	public float unkn7;
	[RszVersion(EfxVersion.RE4)]
	public uint unkn8;
	public float unkn9;
	public float unkn10;
	public float trailWidth;
	public float unkn12;
	public float sb_unkn0;
	[RszVersion(nameof(Version), ">=", EfxVersion.RE2, "&&", nameof(Version), "<=", EfxVersion.RE8)]
	public uint unkn14_re2;
	[RszVersion(nameof(Version), "<", EfxVersion.RE2, "||", nameof(Version), ">", EfxVersion.RE8)] // else
	public float unkn14;
	[RszVersion(EfxVersion.DD2)]
	public float dd2__unkn0;
	[RszVersion(EfxVersion.RERT)]
	public int rert_unkn0;
	public float unkn15;
	[RszVersion(EfxVersion.RE8)]
	public float rert_unkn1;
	public uint trailLength;
    [RszVersion(EfxVersion.RE3)]
	public uint something;
    [RszVersion("==", EfxVersion.RE7)]
	public float unkn17;

	public int ukn18;

    [RszVersion(EfxVersion.RE2, EndAt = nameof(unkn26))]
    public via.Color color2;
	public via.Color unkn19;
	public via.Color unkn20;
	public float unkn21;
	public float unkn22;
	public float unkn23;
	public float unkn24;
	public float unkn25;
	public float unkn26;
    [RszVersion(EfxVersion.RE2)]
	public float re2_unkn;
    [RszVersion(EfxVersion.RE3, EndAt = nameof(re3_unkn2))]
	public float re3_unkn1;
	public float re3_unkn2;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(rert_unkn3))]
	public float rert_unkn2;
	public UndeterminedFieldType rert_unkn3;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFollowExpression, EfxVersion.DMC5, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonFollowExpression : EFXAttribute
{
	public EFXAttributeTypeRibbonFollowExpression() : base(EfxAttributeType.TypeRibbonFollowExpression) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	public uint unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
	public uint unkn1_6;
	public uint unkn1_7;
    [RszVersion("==", EfxVersion.MHRiseSB, EndAt = nameof(sb_unkn1))]
	public float sb_unkn0;
	public float sb_unkn1;

    [RszVersion(EfxVersion.DD2, EndAt = nameof(unkn1_17))]
	public uint unkn1_8;
	public uint unkn1_9;
	public uint unkn1_10;
	public uint unkn1_11;
	public uint unkn1_12;
	public uint unkn1_13;
	public uint unkn1_14;
	public uint unkn1_15;
	public uint unkn1_16;
	public uint unkn1_17;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonLength, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeTypeRibbonLength : EFXAttribute
{
	public EFXAttributeTypeRibbonLength() : base(EfxAttributeType.TypeRibbonLength) { }

	public uint unkn1_0;
	public via.Color color0;
	public via.Color color1;
	public float unkn1_3;

	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
    [RszVersion(EfxVersion.RE4)]
	public uint unkn1_7;
	public float unkn1_8;
	public float unkn1_9;
	public float unkn1_10;
	public float unkn1_11;
    [RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;
	public float unkn1_12;
    [RszVersion(EfxVersion.RE2)]
	public uint unkn1_13;
	public float unkn1_14;
	public float unkn1_15;
	public uint unkn1_16;
	public float unkn1_17;
	public float unkn1_18;
	public float unkn1_19;
	public float unkn1_20;
	public float unkn1_21;
	public float unkn1_22;
	public float unkn1_23;
	public float unkn1_24;
	public float unkn1_25;
	public float unkn1_26;

	[RszVersion(EfxVersion.RE2, EndAt = nameof(sb_unkn2))]
	public via.Color color2_1;
	public via.Color color2_2;
	public via.Color color2_3;
	public float unkn1_29;

	public float unkn1_30;
	public float unkn1_31;
	public float unkn1_32;
	public float unkn1_33;
	public float unkn1_34;
	public float unkn1_35;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(sb_unkn2))]
	public float sb_unkn1;
	public float sb_unkn2;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonLengthExpression, EfxVersion.RE7, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeTypeRibbonLengthExpression : EFXAttribute
{
	public EFXAttributeTypeRibbonLengthExpression() : base(EfxAttributeType.TypeRibbonLengthExpression) { }

	public uint unkn1;
	public uint unkn2;
	public uint unkn3;
	public uint unkn4;
	public uint unkn5;
	public uint unkn6;
	[RszVersion(EfxVersion.RE2, EndAt = nameof(unkn8))]
	public uint unkn7;
	public uint unkn8;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(sb_unkn1))]
	public uint sb_unkn0;
	public uint sb_unkn1;
    [RszVersion(">", EfxVersion.RERT, EndAt = nameof(sb_unkn7))]
	public uint sb_unkn2;
	public uint sb_unkn3;
	public uint sb_unkn4;
	public uint sb_unkn5;
	public uint sb_unkn6;
	public uint sb_unkn7;
    [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn4))]
	public uint re4_unkn1;
	public uint re4_unkn2;
	public uint re4_unkn3;
	public uint re4_unkn4;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	[RszClassInstance] public EFXExpressionListWrapperBase? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonLightweight, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTypeRibbonLightweight : EFXAttribute
{
	public EFXAttributeTypeRibbonLightweight() : base(EfxAttributeType.TypeRibbonLightweight) { }

	public uint unkn1;
	public via.Color color0;
	public via.Color color1;
	public float unkn2_0;
	public uint unkn2_1;
	public uint unkn2_2;
	public float unkn2_3;
	public float unkn2_4;
	public float unkn2_5;
	public float unkn2_6;
    [RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;
	public float unkn2_7;
    [RszVersion(EfxVersion.DD2)]
	public float dd2_unkn;
	public uint unkn2_8;
    [RszVersion(EfxVersion.RE3, EndAt = nameof(unkn2_10))]
	public float unkn2_9;
	public float unkn2_10;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonTrail, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.RE3, EfxVersion.RE4)]
public partial class EFXAttributeTypeRibbonTrail : EFXAttribute
{
	public EFXAttributeTypeRibbonTrail() : base(EfxAttributeType.TypeRibbonTrail) { }

	public uint unkn0;
	[RszVersion(EfxVersion.RE8)]
    public uint unkn1;
    public via.Color color0;
    public via.Color color1;
    public float unkn4;
    public float unkn5;
    public float unkn6;
    public float unkn7;
    public float unkn8;
    public float unkn9;
	[RszVersion(EfxVersion.RE8)]
    public float unkn10;
    public uint unkn11;
    public uint unkn12;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeStrainRibbon, EfxVersion.RE8, EfxVersion.RERT)]
public partial class EFXAttributeTypeStrainRibbon : EFXAttribute, IBoneRelationAttribute
{
	public EFXAttributeTypeStrainRibbon() : base(EfxAttributeType.TypeStrainRibbon) { }

	public uint unkn1_0;
	public via.Color color0;
	public via.Color color1;
	public float unkn1_3;

	public uint unkn1_4;
	public uint unkn1_5;
	public float unkn1_6;
    [RszVersion(EfxVersion.DD2, EndAt = nameof(dd2_extra1))]
	public uint dd2_extra1;
	public float unkn1_7;
	public float unkn1_8;
	public float unkn1_9;
	public float unkn1_10;
	public float unkn1_11;
    [RszVersion(EfxVersion.DD2, EndAt = nameof(dd2_extra2_2))]
	public float dd2_extra2_1;
	public float dd2_extra2_2;
	public uint unkn1_12;
    [RszVersion(EfxVersion.DD2, EndAt = nameof(dd2_extra3_4))]
	public uint dd2_extra3_1;
	public uint dd2_extra3_2;
	public uint dd2_extra3_3;
	public uint dd2_extra3_4;
	public float unkn1_13;
	public float unkn1_14;
	public float unkn1_15;
	public float unkn1_16;
	public float unkn1_17;
    [RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;

    [RszVersion(EfxVersion.DD2, EndAt = nameof(dd2_extra4_4))]
	public float dd2_extra4_1;
	public float dd2_extra4_2;
	public float dd2_extra4_3;
	public float dd2_extra4_4;
	public uint unkn1_18;
	public uint unkn1_19;
    [RszVersion(EfxVersion.RERT)]
	public uint unkn1_20;
    [RszVersion('<', EfxVersion.RERT)]
	public float unkn1_20_re8;
	public float unkn1_21;

	public via.Color color2;
	public via.Color color3;
	public via.Color color4;
	public float unkn1_25;
	public float unkn1_26;
	public float unkn1_27;
	public float unkn1_28;
	public float unkn1_29;
	public uint unkn1_30;
	public float unkn1_31;
	public uint unkn1_32;
	public uint unkn1_33;
	public uint unkn1_34;
	public float unkn1_35;
	public float unkn1_36;
	public float unkn1_37;
	public float unkn1_38;
	public float unkn1_39;
	public float unkn1_40;
	public uint unkn1_41;
	public float unkn1_42;
	public float unkn1_43;
	public float unkn1_44;
	public float unkn1_45;
	public float unkn1_46;
	public float unkn1_47;
	public uint unkn1_48;
	public uint unkn1_49;
	public uint unkn1_50;
	public uint unkn1_51;
	public uint unkn1_52;
	public float unkn1_53;
	public float unkn1_54;
	public float unkn1_55;
	public uint unkn1_56;
	public byte unkn2;
	public float unkn3_0;
	public float sb_unkn1;

	public float unkn3_1;

    public float unkn3_2;
    public float unkn3_3;
	[RszInlineWString] public string? boneName;

    public string? ParentBone { get; set; }
}


[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeStrainRibbonExpression, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTypeStrainRibbonExpression : EFXAttribute
{
	public EFXAttributeTypeStrainRibbonExpression() : base(EfxAttributeType.TypeStrainRibbonExpression) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	public uint unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
	public uint unkn1_6;
	public uint unkn1_7;
	public uint unkn1_8;
	public uint unkn1_9;
	public uint unkn1_10;
	public uint unkn1_11;
	public uint unkn1_12;
	public uint unkn1_13;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(sb_unkn1))]
	public uint sb_unkn0;
	public uint sb_unkn1;

	[RszClassInstance] public EFXExpressionListWrapper expressions = new();

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeStrainRibbonMaterialExpression, EfxVersion.RE4)]
public partial class EFXAttributeTypeStrainRibbonMaterialExpression : EFXAttribute
{
	public EFXAttributeTypeStrainRibbonMaterialExpression() : base(EfxAttributeType.TypeStrainRibbonMaterialExpression) { }

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
public partial class EFXAttributeUVSequenceExpression : EFXAttribute
{
	public EFXAttributeUVSequenceExpression() : base(EfxAttributeType.UVSequenceExpression) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	public uint unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
	public uint unkn1_6;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnitCulling, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RERT)]
public partial class EFXAttributeUnitCulling : EFXAttribute
{
	public EFXAttributeUnitCulling() : base(EfxAttributeType.UnitCulling) { }

	public uint unkn1_0;
	public float unkn1_1; // 1-6 seem like hashes sometimes, may be some sort of conditional substruct
	public float unkn1_2;
	public float unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
	public float unkn1_8;
	public float unkn1_9;
    [RszVersion(EfxVersion.RE8)]
	public float unkn1_10;

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
public partial class EFXAttributeVanishArea3DExpression : EFXAttribute
{
	public EFXAttributeVanishArea3DExpression() : base(EfxAttributeType.VanishArea3DExpression) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	public uint unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
	public uint unkn1_6;
	public uint unkn1_7;
	public uint unkn1_8;
	public uint unkn1_9;
	public uint unkn1_10;
	public uint unkn1_11;
	public uint unkn1_12;
	public uint unkn1_13;
	public uint unkn1_14;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.VectorFieldParameter, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeVectorFieldParameter : EFXAttribute
{
	public EFXAttributeVectorFieldParameter() : base(EfxAttributeType.VectorFieldParameter) { }

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
	public float unkn1_10;
	public float unkn1_11;
	public float unkn1_12;
	public float unkn1_13;
	public float unkn1_14;
	[RszVersion(EfxVersion.RE8, EndAt = nameof(unkn1_17))]
	public float unkn1_15;
	public UndeterminedFieldType unkn1_16;
	public float unkn1_17;
	public uint unkn1_18;
	public int unkn1_19;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.VectorFieldParameterExpression, EfxVersion.DMC5, EfxVersion.RE8, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeVectorFieldParameterExpression : EFXAttribute
{
	public EFXAttributeVectorFieldParameterExpression() : base(EfxAttributeType.VectorFieldParameterExpression) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	public uint unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
	public uint unkn1_6;
	public uint unkn1_7;
	public uint unkn1_8;
	public uint unkn1_9;
	public uint unkn1_10;
	public uint unkn1_11;
	[RszVersion('>', EfxVersion.DMC5, EndAt = nameof(unkn1_20))]
	public uint unkn1_12;
	public uint unkn1_13;
	public uint unkn1_14;
	public uint unkn1_15;
	public uint unkn1_16;
	public uint unkn1_17;
	public uint unkn1_18;
	public uint unkn1_19;
	public uint unkn1_20;

	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Velocity2D, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeVelocity2D : EFXAttribute
{
	public EFXAttributeVelocity2D() : base(EfxAttributeType.Velocity2D) { }

	public float unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public float unkn1_4;

	public float unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
    [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn2))]
	public float re4_unkn0;
	public float re4_unkn1;
	public float re4_unkn2;

	public float unkn2_0;
	public float unkn2_1;
	public uint unkn2_2;
	public float unkn2_3;

	public float unkn2_4;
    [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn2_1))]
	public UndeterminedFieldType re4_unkn2_0;
	public UndeterminedFieldType re4_unkn2_1;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Velocity2DExpression, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeVelocity2DExpression : EFXAttribute
{
	public EFXAttributeVelocity2DExpression() : base(EfxAttributeType.Velocity2DExpression) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public uint unkn4;
    public uint unkn5;
    public uint unkn6;
    public uint unkn7;
    public uint unkn8;

    [RszVersion(EfxVersion.RE4, EndAt = nameof(unkn2_4))]
    public uint unkn2_0;
    public uint unkn2_1;
    public uint unkn2_2;
    public uint unkn2_3;
    public uint unkn2_4;

	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	[RszClassInstance] public EFXExpressionListWrapperBase? expressions;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Velocity3D, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeVelocity3D : EFXAttribute
{
	public EFXAttributeVelocity3D() : base(EfxAttributeType.Velocity3D) { }

	[RszVersion(EfxVersion.RE2)]
	public uint unkn1;
	public float unkn2_0;
	public float unkn2_1;
	public float unkn2_2;
	public float unkn2_3;
	public float unkn2_4;
	public float unkn2_5;
	public float unkn2_6;
	public float unkn2_7;
	public float unkn2_8;
	public float unkn2_9;
	[RszVersion(EfxVersion.RE4, EndAt = nameof(unkn2_11))]
	public uint unkn2_10;
	public uint unkn2_11;
	public float unkn2_12;
	public float unkn2_13;
	public float unkn2_14;
	public float unkn2_15;
	public float re4_unkn0;
	public float re4_unkn1;

    public uint unkn3;
	public float unkn4_0;
	[RszVersion(EfxVersion.RE2)]
	public float unkn4_1;

    [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn3))]
	public uint re4_unkn2;
	public uint re4_unkn3;

	public float unkn4_2;
	[RszVersion(EfxVersion.RE2, EndAt = nameof(unkn4_5))]
	public float unkn4_3;
	public float unkn4_4;
	public float unkn4_5;

	[RszVersion(EfxVersion.RE8, EndAt = nameof(sb_unkn1))]
	public float sb_unkn0;
	public float sb_unkn1;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Velocity3DDelayFrame, EfxVersion.RE4)]
public partial class EFXAttributeVelocity3DDelayFrame : EFXAttribute
{
	public EFXAttributeVelocity3DDelayFrame() : base(EfxAttributeType.Velocity3DDelayFrame) { }

	public uint frameDelay;
	public uint unkn2;

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Velocity3DExpression, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeVelocity3DExpression : EFXAttribute
{
	public EFXAttributeVelocity3DExpression() : base(EfxAttributeType.Velocity3DExpression) { }

	public uint unkn1_0;
	public uint indicesCount;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(rert_unkn1_7))]
	public float rert_unkn1_1;
	public float rert_unkn1_2;
	public float rert_unkn1_3;
	public float rert_unkn1_4;
	public float rert_unkn1_5;
	public float rert_unkn1_6;
	public uint rert_unkn1_7;

	public uint unkn1_2;
	public uint unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
	public uint unkn1_6;
	public uint unkn1_7;
	public uint unkn1_8;
	public uint unkn1_9;
	public uint unkn1_10;
	public uint unkn1_11;
	public uint unkn1_12;

    [RszVersion(">", EfxVersion.RE3, EndAt = nameof(re4_unkn3))]
	public uint unkn1_20;

    [RszVersion(">", EfxVersion.RERT, EndAt = nameof(sb_unkn2))]
	public uint sb_unkn0;
	public uint sb_unkn1;
	public uint sb_unkn2;

    [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn3))]
	public uint re4_unkn0;
	public uint re4_unkn1;
	public uint re4_unkn2;
	public uint re4_unkn3;

	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;

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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.AngularVelocity2D, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeAngularVelocity2D : EFXAttribute
{
	public EFXAttributeAngularVelocity2D() : base(EfxAttributeType.AngularVelocity2D) { }

	public uint unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public uint unkn1_4;
	public float unkn1_5;
	public uint unkn1_6;
	public float unkn1_7;
	public float unkn1_8;
	public float unkn1_9;
	public float unkn1_10;
	public float unkn1_11;
	public float unkn1_12;
	public float unkn1_13;
	public float unkn1_14;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.AngularVelocity3D, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeAngularVelocity3D : EFXAttribute
{
	public EFXAttributeAngularVelocity3D() : base(EfxAttributeType.AngularVelocity3D) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
	public float unkn1_8;
	public float unkn1_9;
	public float unkn1_10;
	public float unkn1_11;
	public float unkn1_12;
	public float unkn1_13;
	public float unkn1_14;
	public float unkn1_15;
	public float unkn1_16;
	public float unkn1_17;
	public float unkn1_18;
	public float unkn1_19;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.AngularVelocity3DDelayFrame, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeAngularVelocity3DDelayFrame : EFXAttribute
{
	public EFXAttributeAngularVelocity3DDelayFrame() : base(EfxAttributeType.AngularVelocity3DDelayFrame) { }

	public uint frameDelay;
	public uint unkn1;
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
		// if (handler.Position != end) throw new Exception("Probably misread embedded efx!");
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonChain, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonChain : EFXAttribute
{
	public EFXAttributeTypeRibbonChain() : base(EfxAttributeType.TypeRibbonChain) { }

	public uint unkn1;
	public via.Color color0;
	public via.Color color1;
	public float unkn2_0;

	public float unkn2_1;
	public UndeterminedFieldType unkn2_2;
	public float unkn2_3;
	public float unkn2_4;
	public float unkn2_5;
	public float unkn2_6;
	public float unkn2_7;
	public float unkn2_8;
	public uint unkn2_9;
    [RszVersion(EfxVersion.RERT)]
	public float rert_unkn0;
    public float unkn2_10;

    // [RszVersion(EfxVersion.RERT, EndAt = nameof(sb_unkn0))]
	// public float sb_unkn0;
	public float unkn2_11;
	public float unkn2_12;
	public float unkn2_13;
	public float unkn2_14;
	public UndeterminedFieldType unkn2_15;
	public float unkn2_16;
	public float unkn2_17;
	public uint unkn2_18;
	public float unkn2_19;
	public float unkn2_20;
	public float unkn2_21;
	[RszVersion(EfxVersion.RERT)]
	public float sb_unkn1;

    public float unkn2_22;
	public float unkn2_23;
	public float unkn2_24;
	public float unkn2_25;
	public float unkn2_26;
	public float unkn2_27;
	public float unkn2_28;
	public float unkn2_29;
	public float unkn2_30;
	public float unkn2_31;
	public UndeterminedFieldType unkn2_32;
	public via.Color unkn2_33;
	public via.Color unkn2_34;
	public via.Color unkn2_35;
	public float unkn2_36;
	public float unkn2_37;
	public float unkn2_38;
	public float unkn2_39;
	public float unkn2_40;
	public float unkn2_41;
	public float unkn2_42;
	public float unkn2_43;
	public float unkn2_44;
	public float unkn2_45;
	public float unkn2_46;
	public float unkn2_47;
    [RszVersion(EfxVersion.RE3, EndAt = nameof(re4_unkn0))]
	public float unkn2_48;
	public float unkn2_49;

    [RszVersion(EfxVersion.RERT)]
	public float sb_unkn2;
    [RszVersion(EfxVersion.RE4)]
	public float re4_unkn0;

    [RszVersion(EfxVersion.DD2, EndAt = nameof(dd2_unkn7))]
	public float dd2_unkn0;
	public float dd2_unkn1;
	public float dd2_unkn2;
	public float dd2_unkn3;
	public float dd2_unkn4;
	public float dd2_unkn5;
	public float dd2_unkn6;
	public float dd2_unkn7;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.AttractorClip, EfxVersion.RE8)]
public partial class EFXAttributeAttractorClip : EFXAttribute
{
	public EFXAttributeAttractorClip() : base(EfxAttributeType.AttractorClip) { }

	public uint unkn0;
	public uint unkn1;
	public uint unkn2;
	public float unkn3;
	public uint unkn4;
	public uint unkn5;
	public uint unkn6;
	[RszArraySizeField(nameof(substruct1))] public int substruct1Length;
	[RszArraySizeField(nameof(substruct2))] public int substruct2Length;
	[RszArraySizeField(nameof(substruct3))] public int substruct3Length;
	[RszFixedSizeArray(nameof(substruct1Length), '/', 4)] public int[]? substruct1;
	[RszFixedSizeArray(nameof(substruct2Length), '/', 4)] public int[]? substruct2;
	[RszFixedSizeArray(nameof(substruct3Length), '/', 4)] public float[]? substruct3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterColorClip, EfxVersion.RE4)]
public partial class EFXAttributeEmitterColorClip : EFXAttribute
{
	public EFXAttributeEmitterColorClip() : base(EfxAttributeType.EmitterColorClip) { }

	public uint unkn0;
	public uint unkn1;
	public uint unkn2;
	public float unkn3;
	public uint unkn4;
	public uint unkn5;
	public uint unkn6;
	[RszArraySizeField(nameof(substruct1))] public int substruct1Length;
	[RszArraySizeField(nameof(substruct2))] public int substruct2Length;
	[RszArraySizeField(nameof(substruct3))] public int substruct3Length;
	[RszFixedSizeArray(nameof(substruct1Length), '/', 4)] public int[]? substruct1;
	[RszFixedSizeArray(nameof(substruct2Length), '/', 4)] public int[]? substruct2;
	[RszFixedSizeArray(nameof(substruct3Length), '/', 4)] public float[]? substruct3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ProceduralDistortionClip, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeProceduralDistortionClip : EFXAttribute
{
    public EFXAttributeProceduralDistortionClip() : base(EfxAttributeType.ProceduralDistortionClip) { }

    public uint unkn0;
    public uint unkn1;
    public int unkn2;
    public float unkn3;
    public uint unkn4;
    public uint unkn5;
    public uint unkn6;
	[RszArraySizeField(nameof(substruct1))] public int substruct1Length;
	[RszArraySizeField(nameof(substruct2))] public int substruct2Length;
	[RszArraySizeField(nameof(substruct3))] public int substruct3Length;
    [RszFixedSizeArray(nameof(substruct1Length), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Length), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Length), '/', 4)] public float[]? substruct3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtTransform3DClip, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributePtTransform3DClip : EFXAttribute
{
    public EFXAttributePtTransform3DClip() : base(EfxAttributeType.PtTransform3DClip) { }

    public uint unkn0;
    public uint unkn1;
    public int unkn2;
    public float unkn3;
    public uint unkn4;
    public uint unkn5;
    public uint unkn6;
	[RszArraySizeField(nameof(substruct1))] public int substruct1Length;
	[RszArraySizeField(nameof(substruct2))] public int substruct2Length;
	[RszArraySizeField(nameof(substruct3))] public int substruct3Length;
    [RszFixedSizeArray(nameof(substruct1Length), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Length), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Length), '/', 4)] public float[]? substruct3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtUvSequenceClip, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributePtUvSequenceClip : EFXAttribute
{
    public EFXAttributePtUvSequenceClip() : base(EfxAttributeType.PtUvSequenceClip) { }

    uint unkn0;
    uint unkn1;
    int unkn2;
    float unkn3;
    uint unkn4;
    uint unkn5;
    uint unkn6;

	[RszArraySizeField(nameof(substruct1))] public int substruct1Length;
	[RszArraySizeField(nameof(substruct2))] public int substruct2Length;
	[RszArraySizeField(nameof(substruct3))] public int substruct3Length;
    [RszFixedSizeArray(nameof(substruct1Length), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Length), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Length), '/', 4)] public float[]? substruct3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.MeshEmitter, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeMeshEmitter : EFXAttribute
{
    public EFXAttributeMeshEmitter() : base(EfxAttributeType.MeshEmitter) { }

	public uint unkn0;
	public float unkn1;
	public uint unkn2;
	public uint unkn3;
	public uint unkn4;
	public float unkn5;
	public float unkn6;
	public float unkn7;

    public uint unkn1_0;
    public uint unkn1_1;
    public uint unkn1_2;
    public uint unkn1_3;
    public uint unkn1_4;
    public uint unkn1_5;
    public uint unkn1_6;
    public uint unkn1_7;
    public uint unkn1_8;
    public uint unkn1_9;

	public float unkn9;
	public float unkn10;
	[RszVersion('>', EfxVersion.RERT, EndAt = nameof(unkn16))]
	public uint unkn11;
	public uint unkn12;
	public uint unkn13;
	public uint unkn14;
	public float unkn15;
	public float unkn16;

    [RszStringLengthField(nameof(mapPath))] public int mapPathLength;
    [RszStringLengthField(nameof(meshPath))] public int meshPathLength;
    [RszStringLengthField(nameof(mdfPath))] public int mdfPathLength;
	[RszVersion('>', EfxVersion.DMC5), RszStringLengthField(nameof(texPath))] public int texPathLength;
    [RszStringLengthField(nameof(maskPath))] public int maskPathLength;
	[RszVersion(EfxVersion.DD2), RszStringLengthField(nameof(name))] public int nameLength;
	[RszInlineWString(nameof(mapPathLength))] public string? mapPath; // mapName: NormalRoughnessMap
	[RszInlineWString(nameof(meshPathLength))] public string? meshPath;
	[RszInlineWString(nameof(mdfPathLength))] public string? mdfPath;
	[RszVersion('>', EfxVersion.DMC5), RszInlineWString(nameof(texPathLength))]
	public string? texPath;
	[RszInlineWString(nameof(maskPathLength))] public string? maskPath; // note: might be a path to something else; RE4 doesn't use this one at all

	[RszVersion(EfxVersion.DD2), RszInlineWString(nameof(nameLength))]
	public string? name;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtVelocity3DClip, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributePtVelocity3DClip : EFXAttribute
{
    public EFXAttributePtVelocity3DClip() : base(EfxAttributeType.PtVelocity3DClip) { }

    uint unkn0;
    uint unkn1;
    int unkn2;
    float unkn3;
    uint unkn4;
    uint unkn5;
    uint unkn6;

	[RszArraySizeField(nameof(substruct1))] public int substruct1Length;
	[RszArraySizeField(nameof(substruct2))] public int substruct2Length;
	[RszArraySizeField(nameof(substruct3))] public int substruct3Length;
    [RszFixedSizeArray(nameof(substruct1Length), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Length), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Length), '/', 4)] public float[]? substruct3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TexelChannelOperatorClip, EfxVersion.RE4)]
public partial class EFXAttributeTexelChannelOperatorClip : EFXAttribute
{
    public EFXAttributeTexelChannelOperatorClip() : base(EfxAttributeType.TexelChannelOperatorClip) { }

    uint unkn0;
    uint unkn1;
    uint unkn2;
    float unkn3;
    uint unkn4;
    uint unkn5;
    uint unkn6;
	[RszArraySizeField(nameof(substruct1))] public int substruct1Length;
	[RszArraySizeField(nameof(substruct2))] public int substruct2Length;
	[RszArraySizeField(nameof(substruct3))] public int substruct3Length;
    [RszFixedSizeArray(nameof(substruct1Length), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Length), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Length), '/', 4)] public float[]? substruct3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform3DClip, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTransform3DClip : EFXAttribute
{
    public EFXAttributeTransform3DClip() : base(EfxAttributeType.Transform3DClip) { }

	uint unkn0;
    uint unkn1;
    int unkn2;
    float unkn3;
    uint unkn4;
    uint unkn5;
    uint unkn6;
	[RszArraySizeField(nameof(substruct1))] public int substruct1Length;
	[RszArraySizeField(nameof(substruct2))] public int substruct2Length;
	[RszArraySizeField(nameof(substruct3))] public int substruct3Length;
    [RszFixedSizeArray(nameof(substruct1Length), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Length), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Length), '/', 4)] public float[]? substruct3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeMeshExpression, EfxVersion.RE7, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTypeMeshExpression : EFXAttribute
{
    public EFXAttributeTypeMeshExpression() : base(EfxAttributeType.TypeMeshExpression) { }

    public uint substructCount;
    public uint substructLength;
	[RszVersion(nameof(Version), "==", EfxVersion.RE3, "||", nameof(Version), "==", EfxVersion.RE8, "||", nameof(Version), "==", EfxVersion.RERT)]
    public uint indicesCount;
    public uint unkn4;
    public uint unkn5_0;
    public uint unkn5_1;
    public uint unkn5_2;
    public uint unkn5_3;
    public uint unkn5_4;
    public uint unkn5_5;
    public uint unkn5_6;
    public uint unkn5_7;
    public uint unkn5_8;
    public uint unkn5_9;
    public uint unkn5_10;
    public uint unkn5_11;
    public uint unkn5_12;
    public uint unkn5_13;
    public uint unkn5_14;
    public uint unkn5_15;
    public uint unkn5_16;
    public uint unkn5_17;
    public uint unkn5_18;
	[RszVersion(EfxVersion.RE2, EndAt = nameof(unkn5_21))]
    public uint unkn5_19;
    public uint unkn5_20;
	[RszVersion(">", EfxVersion.RE3, EndAt = nameof(unkn5_23))]
    public uint unkn5_21;
	[RszVersion(nameof(Version), "==", EfxVersion.RE8, "||", nameof(Version), "==", EfxVersion.RERT, "||", nameof(Version), "==", EfxVersion.RE4)]
    public uint unkn5_22;
	[RszVersion(nameof(Version), "==", EfxVersion.RE4)]
    public uint unkn5_23;
	[RszVersion(EfxVersion.RE4)]
    public uint unkn5_24; // NOTE: is this the substructCount target?

	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		nameof(Version), ">=", EfxVersion.RE4, typeof(EFXExpressionListWrapper2),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
	[RszVersion(EfxVersion.DD2)]
    public uint unkn5_25;

	[RszVersion(EfxVersion.DD2), RszClassInstance] public EFXExpressionListWrapper2? expression2;

	[RszVersion(nameof(Version), "==", EfxVersion.RE2, "||", nameof(Version), "==", EfxVersion.DMC5, "||", nameof(Version), "==", EfxVersion.RE3, "||", nameof(Version), "==", EfxVersion.RE8, "||", nameof(Version), "==", EfxVersion.RERT)]
	[RszList(nameof(substructCount)), RszClassInstance] public List<EFXExpression4>? expression3;
	[RszVersion(nameof(Version), "==", EfxVersion.RE3, "||", nameof(Version), "==", EfxVersion.RE8, "||", nameof(Version), "==", EfxVersion.RERT)]
	[RszFixedSizeArray(nameof(indicesCount))] public uint[]? indices;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygonClip, EfxVersion.DMC5)]
public partial class EFXAttributeTypePolygonClip : EFXAttribute
{
    public EFXAttributeTypePolygonClip() : base(EfxAttributeType.TypePolygonClip) { }

    public uint unkn0;
    public uint unkn1;
    public int unkn2;
    public float unkn3;
    public uint unkn4;
    public uint unkn5;
	[RszArraySizeField(nameof(substruct0))] public int substruct0Length;
	[RszArraySizeField(nameof(substruct1))] public int substruct1Length;
	[RszArraySizeField(nameof(substruct2))] public int substruct2Length;
	[RszArraySizeField(nameof(substruct3))] public int substruct3Length;
    [RszFixedSizeArray(nameof(substruct0Length))] public byte[]? substruct0;
    [RszFixedSizeArray(nameof(substruct1Length), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Length), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Length), '/', 4)] public float[]? substruct3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.VectorFieldParameterClip, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeVectorFieldParameterClip : EFXAttribute
{
    public EFXAttributeVectorFieldParameterClip() : base(EfxAttributeType.VectorFieldParameterClip) { }

    uint unkn0;
    uint unkn1;
    int unkn2;
    float unkn3;
    uint unkn4;
    uint unkn5;
    uint unkn6;
	[RszArraySizeField(nameof(substruct1))] public int substruct1Length;
	[RszArraySizeField(nameof(substruct2))] public int substruct2Length;
	[RszArraySizeField(nameof(substruct3))] public int substruct3Length;
    [RszFixedSizeArray(nameof(substruct1Length), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Length), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Length), '/', 4)] public float[]? substruct3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtColliderAction, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributePtColliderAction : EFXAttribute
{
    public EFXAttributePtColliderAction() : base(EfxAttributeType.PtColliderAction) { }

    public uint dataFlags;

    public float unkn1;
    public float unkn2;
    public uint linkedAction;

	// found in RE2RT
	[RszConditional("(dataFlags & (1 << 10)) != 0", EndAt = nameof(ukn_flag2_11))]
	public uint ukn_flag2_0; // likely some sort of shape type + data
    public float ukn_flag2_2;
    public float ukn_flag2_3;
    public float ukn_flag2_4;
    public float ukn_flag2_5;
	public uint ukn_flag2_6;
    public float ukn_flag2_7;
    public float ukn_flag2_8;
    public float ukn_flag2_9;
    public float ukn_flag2_10;
    public float ukn_flag2_11;

	[RszVersion(EfxVersion.DD2, EndAt = nameof(dd2_unkn2))]
	public uint dd2_unkn0;
	public uint dd2_unkn1;
	public uint dd2_unkn2;

	[RszConditional("(dataFlags & (1 << 1)) != 0"), RszInlineWString]
	public string? unknString_flag2;

	[RszConditional("(dataFlags & (1 << 0)) != 0"), RszInlineWString]
	public string? unknString_flag1;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtCollision, EfxVersion.RE7, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributePtCollision : EFXAttribute
{
    public EFXAttributePtCollision() : base(EfxAttributeType.PtCollision) { }

    public byte stringBitFlag;//<format=binary,comment = "bitflag, determines whether there will be strings read">;
    public byte unkn0_1;
    public byte unkn0_2;
    public byte unkn0_3;
    public float unkn1;
    public float unkn2;
    public uint unkn3;
    public uint unkn4;
    public float unkn5;
    public float unkn6;
    public float unkn7;
    public float unkn8;
    public uint unkn9;
	[RszVersion('>', EfxVersion.RE7, EndAt = nameof(unkn16))]
    public float unkn10;
    public float unkn11;
    public float unkn12;
    public float unkn13;
	[RszVersion('>', EfxVersion.RE3, EndAt = nameof(unkn16))]
    public float unkn14;
	[RszVersion('>', EfxVersion.RERT, EndAt = nameof(unkn16))]
    public uint unkn15;
    public float unkn16;
	[RszConditional("(stringBitFlag & (1 << 0)) != 0"), RszInlineWString]
	public string? unknString0;

	[RszConditional("(stringBitFlag & (1 << 1)) != 0"), RszInlineWString]
	public string? unknString1;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeMeshClip, EfxVersion.RE3, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeTypeMeshClip : EFXAttribute
{
    public EFXAttributeTypeMeshClip() : base(EfxAttributeType.TypeMeshClip) { }

	public uint unkn1;
    public uint part2Size;
    public int unkn3;
    public float unkn4;
	[RszArraySizeField(nameof(substruct1))] public int substruct1Count;
	[RszArraySizeField(nameof(substruct2))] public int substruct2Count;
	[RszArraySizeField(nameof(substruct3))] public int substruct3Count;

    public uint substruct1Length;
    public uint substruct2Length;
    public uint substruct3Length;
	[RszVersion(EfxVersion.RE4)]
    public uint substruct4CountMaybe;
    [RszVersion(EfxVersion.RE3), RszArraySizeField(nameof(indices))] public int indicesCount;

    [RszFixedSizeArray(nameof(substruct1Count))] public MeshClip_Struct1[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Count))] public MeshClip_Struct2[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Count))] public MeshClip_Struct3[]? substruct3;
    [RszFixedSizeArray(nameof(substruct1Count))] public MeshClip_Struct4[]? substruct4;
    [RszVersion(EfxVersion.RE3), RszFixedSizeArray(nameof(indicesCount))] public uint[]? indices;

    public struct MeshClip_Struct0
    {
        public uint unkn0;
        public uint unkn1;
        public uint unkn2;
        public uint unkn3;
    }

    public struct MeshClip_Struct1
    {
        public uint unkn0;
        public uint unkn1;
    }

    public struct MeshClip_Struct2
    {
        public float unkn0;
        public uint unkn1;

		// RE3RT
        public float unkn2;
		// RE4?
        // public uint unkn2;
    }

    public struct MeshClip_Struct3
    {
		// RE3RT
        public float unkn0;
        public float unkn1;
        public float unkn2;
        public float unkn3;
		// RE4?
        // public float unkn0;
        // public uint unkn1;
        // public uint unkn2;
        // public int unkn3;
    }

    public struct MeshClip_Struct4
    {
        public uint hash;
        public uint unkn1;
        public uint unkn2;
        public int unkn3;
    }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownSB_195, EfxVersion.MHRiseSB)]
public partial class EFXAttributeunknSBStruct195 : EFXAttribute
{
    public EFXAttributeunknSBStruct195() : base(EfxAttributeType.UnknownSB_195) { }

    public uint unkn1;
    [RszVersion(EfxVersion.RE4)]
    public uint re4_unkn0;
    public float unkn2_0;
	[RszArraySizeField(nameof(substruct))] public int substructCount;
    [RszFixedSizeArray(nameof(substructCount))] public EFXAttributeunknSBStruct195_Substruct[]? substruct;

    public float unkn13;
    public float unkn14;
    public float unkn15;

    public struct EFXAttributeunknSBStruct195_Substruct
    {
        public float unkn0;
        public float unkn1;
        public float unkn2;
        public float unkn3;
        public float unkn4;
        public float unkn5;
        public float unkn6;
        public float unkn7;
        public float unkn11;
        public float unkn12;
        public float unkn13;
    }
}

public enum PtBehaviorPropType
{
    PropFloat = 9,
    PropFloat2 = 19,
    PropFloat3 = 11,
    PropRange = 10,
    PropUint = 4,
    PropInt = 14,
    PropPrefabpath = 17,
    PropWstring = 21,
    PropEnum = 18,
    PropColor = 15,
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariableDataBase : BaseModel
{
	[RszIgnore] public EfxVersion Version;

	public PtBehaviorVariableDataBase(EfxVersion version) { Version = version; }

	public int unkn;
	public int size;
	[RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn1))]
	public short re4_unkn0;
	public short re4_unkn1;

	protected bool EnsureMinimumSize(int minSize)
	{
		if (size < minSize) size = minSize;
		return true;
	}
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariableDataColor : PtBehaviorVariableDataBase
{
	public PtBehaviorVariableDataColor(EfxVersion version) : base(version) { }

	public via.Color color;
	[RszFixedSizeArray(nameof(size), - 4)] public byte[]? restData;

    protected override bool DoRead(FileHandler handler) => base.DoRead(handler) && DefaultRead(handler);
    protected override bool DoWrite(FileHandler handler) => EnsureMinimumSize(4) && base.DoWrite(handler) && DefaultWrite(handler);
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariableInteger : PtBehaviorVariableDataBase
{
	public PtBehaviorVariableInteger(EfxVersion version) : base(version) { }

	public int value;
	[RszFixedSizeArray(nameof(size), - 4)] public byte[]? restData;

    protected override bool DoRead(FileHandler handler) => base.DoRead(handler) && DefaultRead(handler);
    protected override bool DoWrite(FileHandler handler) => EnsureMinimumSize(4) && base.DoWrite(handler) && DefaultWrite(handler);
}
[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariableFloat : PtBehaviorVariableDataBase
{
	public PtBehaviorVariableFloat(EfxVersion version) : base(version) { }

	public float value;
	[RszFixedSizeArray(nameof(size), - 4)] public byte[]? restData;

    protected override bool DoRead(FileHandler handler) => base.DoRead(handler) && DefaultRead(handler);
    protected override bool DoWrite(FileHandler handler) => EnsureMinimumSize(4) && base.DoWrite(handler) && DefaultWrite(handler);
}
[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariableFloat2 : PtBehaviorVariableDataBase
{
	public PtBehaviorVariableFloat2(EfxVersion version) : base(version) { }

	public Vector2 Vec;
	[RszFixedSizeArray(nameof(size), - 8)] public byte[]? restData;

    protected override bool DoRead(FileHandler handler) => base.DoRead(handler) && DefaultRead(handler);
    protected override bool DoWrite(FileHandler handler) => EnsureMinimumSize(8) && base.DoWrite(handler) && DefaultWrite(handler);
}
[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariableFloat3 : PtBehaviorVariableDataBase
{
	public PtBehaviorVariableFloat3(EfxVersion version) : base(version) { }

	public Vector3 vec;
	[RszFixedSizeArray(nameof(size), - 12)] public byte[]? restData;

    protected override bool DoRead(FileHandler handler) => base.DoRead(handler) && DefaultRead(handler);
    protected override bool DoWrite(FileHandler handler) => EnsureMinimumSize(12) && base.DoWrite(handler) && DefaultWrite(handler);
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariableDataPrefabPath : PtBehaviorVariableDataBase
{
	public PtBehaviorVariableDataPrefabPath(EfxVersion version) : base(version) { }

	[RszInlineWString(nameof(size))] public string? prefabPath;

    protected override bool DoRead(FileHandler handler) => base.DoRead(handler) && DefaultRead(handler);
    protected override bool DoWrite(FileHandler handler) => EnsureMinimumSize(prefabPath?.Length ?? 2) && base.DoWrite(handler) && DefaultWrite(handler);
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariableDataPrefabUnknown : PtBehaviorVariableDataBase
{
	public PtBehaviorVariableDataPrefabUnknown(EfxVersion version) : base(version) { }

	[RszFixedSizeArray(nameof(size))] public byte[]? data;

    protected override bool DoRead(FileHandler handler) => base.DoRead(handler) && DefaultRead(handler);
    protected override bool DoWrite(FileHandler handler) => EnsureMinimumSize(data?.Length ?? 0) && base.DoWrite(handler) && DefaultWrite(handler);
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariable : BaseModel
{
	[RszIgnore] public EfxVersion Version;

	public PtBehaviorVariable(EfxVersion version) { Version = version; }

	public int varSize;
	public PtBehaviorPropType dataType;
	// public uint unkn2;
	// public int dataSize;
	// [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn1))]
	// public short re4_unkn0;
	// public short re4_unkn1;

	// TODO - possibly replaces re4_unkn0, re4_unkn1
	// if(varID == 0x0F){
	//     spos[i] = FTell();
	//     i++;
	//     via.Color small_color;
	//     char var[varSize-16];// <name="Variable", bgcolor=0xA3BECC, open=suppress>;
	// }else{
	//     char  variable[varSize-12];// <name="Vairable", bgcolor=0xD6D6D6, open=suppress>;
	// }
	[RszClassInstance, RszConstructorParams(nameof(Version)), RszSwitch(
		nameof(dataType), "==", PtBehaviorPropType.PropColor, typeof(PtBehaviorVariableDataColor),
		nameof(dataType), "==", PtBehaviorPropType.PropPrefabpath, typeof(PtBehaviorVariableDataPrefabPath),
		nameof(dataType), "==", PtBehaviorPropType.PropInt, typeof(PtBehaviorVariableInteger),
		nameof(dataType), "==", PtBehaviorPropType.PropEnum, typeof(PtBehaviorVariableInteger),
		nameof(dataType), "==", PtBehaviorPropType.PropFloat, typeof(PtBehaviorVariableFloat),
		nameof(dataType), "==", PtBehaviorPropType.PropFloat2, typeof(PtBehaviorVariableFloat2),
		nameof(dataType), "==", PtBehaviorPropType.PropFloat3, typeof(PtBehaviorVariableFloat3),
		typeof(PtBehaviorVariableDataPrefabUnknown)
	)]
	public PtBehaviorVariableDataBase? variable;

	// [RszFixedSizeArray(nameof(dataSize))] public byte[]? data;
	[RszInlineString(-1)] public string? behaviorProperty;

	protected override bool DoRead(FileHandler handler)
	{
		DefaultRead(handler);
		return true;
	}

	protected override bool DoWrite(FileHandler handler)
	{
		DefaultWrite(handler);
		return true;
	}
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtBehavior, EfxVersion.RE4)]
public partial class EFXAttributePtBehavior : EFXAttribute
{
    public EFXAttributePtBehavior() : base(EfxAttributeType.PtBehavior) { }

    public uint unkn1;
    [RszStringLengthField(nameof(behaviorString))] public int behaviorStringLength;
    [RszArraySizeField(nameof(properties))] public int varCount;
    [RszInlineString(nameof(behaviorStringLength))]
	public string? behaviorString;

	[RszClassInstance, RszList(nameof(varCount)), RszConstructorParams(nameof(Version))]
	public readonly List<PtBehaviorVariable> properties = new();
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuMesh, EfxVersion.RE8, EfxVersion.RE4)]
public partial class EFXAttributeTypeGpuMesh : EFXAttribute
{
    public EFXAttributeTypeGpuMesh() : base(EfxAttributeType.TypeGpuMesh) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public via.Color color0;
    public via.Color color1;
    public float unkn6;
    public uint unkn7;
	[RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn2))]
    public uint re4_unkn1;
    public uint re4_unkn2;
	[RszVersion("==", EfxVersion.RE8)]
    public float re8_unkn1;
    public float unkn10;
    public float unkn11;
    public float unkn12;
	[RszVersion(EfxVersion.RE4, EndAt = nameof(unkn16))]
    public uint unkn13;
    public uint unkn14;
    public uint unkn15;
    public uint unkn16;
    public float unkn17;
    public float unkn18;
    public float unkn19;
    public float unkn20;
    public float unkn21;
    public float unkn22;
    public float unkn23;
    public float unkn24;
    public float unkn25;
    public float unkn26;
	[RszVersion(EfxVersion.RE4, EndAt = nameof(unkn31))]
    public float unkn27;
    public float unkn28;
    public float unkn29;
    public float unkn30;
    public uint unkn31;
	[RszArraySizeField(nameof(texturePaths))] public int texCount;
    [RszInlineWString] public string? meshPath;
    [RszInlineWString] public string? unknPath;
    [RszInlineWString] public string? mdfPath;
    [RszByteSizeField(nameof(unknData))] public uint unknDataSize;
    [RszFixedSizeArray(nameof(unknDataSize))] public byte[]? unknData;

	public int texBlockLength;
	[RszList(nameof(texCount)), RszInlineWString] public string[]? texturePaths;

	protected override bool DoRead(FileHandler handler)
    {
		DefaultRead(handler);
        return true;
    }

    protected override bool DoWrite(FileHandler handler)
    {
		texBlockLength = texturePaths?.Sum(t => (t.Length + 1) * 2) ?? 2;
		DefaultWrite(handler);
		return true;
    }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeMesh, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTypeMesh : EFXAttribute
{
    public EFXAttributeTypeMesh() : base(EfxAttributeType.TypeMesh) { }

    public byte unkn1_A;
    public byte unkn1_B;
    public byte unkn1_C;
    public byte unkn1_D;
    [RszVersion(EfxVersion.RE4)]
	public uint re4_unkn0;
    public via.Color color0;
    public via.Color color1;
    public float unkn4;
    public via.Color color2;
    public float unkn6;
    public uint frameCount;
    public uint startingFrameMin;
    public uint startingFrameMax;
    public float animationSpeedMin;
    public float animationSpeedMax;
    public float acceleration;
    public float accelerationRange;
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
    public uint unkn23;
    public uint unkn24;
    [RszVersion(EfxVersion.RE4)]
    public uint re4_unkn1;
    [RszVersion(EfxVersion.DD2)]
    public float dd2_unkn1;
    [RszArraySizeField(nameof(texPaths))] public int texCount;
    [RszVersion(EfxVersion.DD2)]
    public uint dd2_unkn2;
    [RszInlineWString] public string? meshPath;
    [RszInlineWString] public string? unknPath;
    [RszInlineWString] public string? mdfPath;

	[RszArraySizeField(nameof(properties))] public int propertiesDataSize;
	[RszClassInstance, RszList(nameof(propertiesDataSize), '/', 32), RszConstructorParams(nameof(Version))]
	public List<MdfProperty> properties = new();

	public int texPathBlockLength;
	[RszInlineWString, RszList(nameof(texCount))] public string[]? texPaths;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_70New, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_70 : EFXAttribute
{
	public EFXAttributeUnknownRE4_70() : base(EfxAttributeType.UnknownRE4_70New) { }

    public uint unkn1;
    public via.Color unkn2;
    public via.Color unkn3;
    public float unkn4;
    public UndeterminedFieldType unkn5;
    public UndeterminedFieldType unkn6;
    public float unkn7;
    [RszVersion(EfxVersion.RE4)]
    public uint re4_unkn1;
    public float unkn8;
    [RszVersion(EfxVersion.RE4)]
    public uint re4_unkn2;
    public float unkn9;
    public float unkn10;
    [RszVersion(EfxVersion.RE4)]
    public float re4_unkn3;
    public uint unkn11;
    public uint unkn12;

    [RszVersion(EfxVersion.RE4, EndAt = nameof(unkn20))]
    public uint unkn13;
    public uint unkn14;
    public int unkn15;
    public int unkn16;
    public int unkn17;
    public float unkn19;
    public float unkn20;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_114TypeStrainRibbon, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_114 : EFXAttribute
{
	public EFXAttributeUnknownRE4_114() : base(EfxAttributeType.UnknownRE4_114TypeStrainRibbon) { }

    public uint unkn0_0;
    public uint unkn0_1;
    public uint unkn0_2;
    public uint unkn0_3;
    public float unkn1_0;
    public float unkn1_1;
    public float unkn1_2;
    public int unkn2;
    public float unkn3_0;
    public float unkn3_1;
    public float unkn3_2;
    public float unkn3_3;
    public float unkn3_4;
    public uint unkn3_5;
    public uint unkn4_0;
    public float unkn4_1;
    public float unkn4_2;
    public via.Color color0;
    public via.Color color1;
    public via.Color color2;

	public float unkn5_0;
    public float unkn5_1;
    public float unkn5_2;
    public float unkn5_3;
    public float unkn5_4;
    public float unkn5_5;
    public float unkn5_6;
    public float unkn5_7;
    public uint unkn5_8;
    public float unkn5_9;
    public float unkn5_10;
    public float unkn5_11;
    public float unkn5_12;
    public float unkn5_13;
    public float unkn5_14;
    public float unkn5_15;
    public float unkn5_16;
    public float unkn5_17;
    public float unkn5_18;
    public float unkn5_19;
    public float unkn5_20;
    public float unkn5_21;
    public float unkn5_22;
    public float unkn5_23;
    public uint unkn5_24;
    public float unkn5_25;
    public float unkn5_26;
    public uint unkn5_27;
    public float unkn5_28;
    public float unkn5_29;
    public float unkn5_30;
    public float unkn5_31;

    public int unkn6;
	public float unkn7_0;
    public float unkn7_1;
    public float unkn7_2;
    public float unkn7_3;
    public float unkn7_4;
    public float unkn7_5;
	[RszInlineWString] public string? name;
	public int unkn8_0;
	public uint unkn8_color1;
	public uint unkn8_color2;
	public float unkn8_3;
	public float unkn8_4;
	public int unkn8_5;
	public float unkn8_6;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_130, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_130 : EFXAttribute
{
	public EFXAttributeUnknownRE4_130() : base(EfxAttributeType.UnknownRE4_130) { }

    public int unkn0;
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_134, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_134 : EFXAttribute
{
    public EFXAttributeUnknownRE4_134() : base(EfxAttributeType.UnknownRE4_134) { }

	[RszInlineWString] public string? efxPath;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_184DepthOperator, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_184DepthOperator : EFXAttribute
{
	public EFXAttributeUnknownRE4_184DepthOperator() : base(EfxAttributeType.UnknownRE4_184DepthOperator) { }

    public uint unkn0;
    public float unkn1;
    public float unkn2;
    public float unkn3;
    public float unkn4;
    public UndeterminedFieldType unkn5;
    public float unkn6;
    public UndeterminedFieldType unkn7;
    public float unkn8;
    public UndeterminedFieldType unkn9;
    public float unkn10;
    public float unkn11;
    public UndeterminedFieldType unkn12;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_195, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeUnknownRE4_195 : EFXAttribute
{
	public EFXAttributeUnknownRE4_195() : base(EfxAttributeType.UnknownRE4_195) { }

    public float unkn0;
    public float unkn1;
    public float unkn2;
    public float unkn3;
    public uint unkn4;
    public uint unkn5;
	[RszVersion("!=", EfxVersion.DD2)]
    ushort unkn6;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_197, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_197 : EFXAttribute
{
	public EFXAttributeUnknownRE4_197() : base(EfxAttributeType.UnknownRE4_197) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public float unkn4;
    public float unkn5;
    public float unkn6;
    public float unkn7;
    public byte unkn8;
    public byte unkn9;
    public byte unkn10;
    public float unkn11;
    public float unkn12;
    public float unkn13;
    public float unkn14;
    public float unkn15;
    public float unkn16;
    public float unkn17;
    public float unkn18;
    public float unkn19;
    public byte unkn20;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_210AngularVelocity2D, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_210 : EFXAttribute
{
	public EFXAttributeUnknownRE4_210() : base(EfxAttributeType.UnknownRE4_210AngularVelocity2D) { }

    public uint unkn0;
    public float unkn1;
    public UndeterminedFieldType unkn2;
    public float unkn3;
    public UndeterminedFieldType unkn4;
    public float unkn5;
    public UndeterminedFieldType unkn6;
    public UndeterminedFieldType unkn7;
    public float unkn8;
    public UndeterminedFieldType unkn9;
    public float unkn10;
    public UndeterminedFieldType unkn12;
    public float unkn13;
    public UndeterminedFieldType unkn14;
    public float unkn15;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_213, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_213 : EFXAttribute
{
    public EFXAttributeUnknownRE4_213() : base(EfxAttributeType.UnknownRE4_213) { }

    public UndeterminedFieldType unkn1;
	[RszArraySizeField(nameof(substruct1))] public int substruct1Count;
    public float unkn2_0;
	[RszArraySizeField(nameof(substruct2))] public int substruct2Count;

    [RszFixedSizeArray(nameof(substruct1Count))] public UnknownRE4_213_Substruct1[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Count))] public UnknownRE4_213_Substruct2[]? substruct2;

    public struct UnknownRE4_213_Substruct1
    {
        public float unkn0;
        public float unkn1;
        public float unkn2;
        public float unkn3;
        public float unkn4;
    }

    public struct UnknownRE4_213_Substruct2
    {
        public float unkn0;
        public float unkn1;
        public float unkn2;
        public float unkn3;
        public float unkn4;
        public float unkn5;
        public float unkn6;
    }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_226, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_226 : EFXAttribute
{
	public EFXAttributeUnknownRE4_226() : base(EfxAttributeType.UnknownRE4_226) { }

    public uint unkn0;
    public via.Color color0;
    public via.Color color1;
    public float unkn3;
    public float unkn4;
    public float unkn5;
    public via.Color color2;
    public via.Color color3;
    public float unkn8;
    public float unkn9;
    public float unkn10;
    public float unkn11;
    public float unkn12;
    public float unkn13;
    public float unkn14;
    public float unkn15;
    public float unkn16;
    public uint unkn17;
    public float unkn18;
    public float unkn19;
    public float unkn20;
    public float unkn21;
    public float unkn22;
    public float unkn23;
    public uint unkn24;
    public float unkn25;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_228, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_228 : EFXAttribute
{
    public EFXAttributeUnknownRE4_228() : base(EfxAttributeType.UnknownRE4_228) { }

	public uint hash1;
	public float unkn0;
	public uint hash2;
	public float unkn1;
	public float unkn2;
	public float unkn3;

	[RszFixedSizeArray(87)] public byte[]? unkn4;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_231EfCsv, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_231 : EFXAttribute
{
	public EFXAttributeUnknownRE4_231() : base(EfxAttributeType.UnknownRE4_231EfCsv) { }

    public ushort unkn0;
    public uint unkn1;
    public uint unkn2;
    [RszInlineWString] public string? efcsvPath;
    [RszInlineWString] public string? unknString0;
    [RszInlineWString] public string? unknString1;
    [RszInlineWString] public string? unknString2;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_243_UnknGPUBillboard, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeUnknownRE4_243_UnknGPUBillboard : EFXAttribute
{
	public EFXAttributeUnknownRE4_243_UnknGPUBillboard() : base(EfxAttributeType.UnknownRE4_243_UnknGPUBillboard) { }

    public uint unkn0;
    public via.Color color0;
    public via.Color color1;
    public float unkn3;
    public float unkn4;
    public UndeterminedFieldType unkn5;
    public float unkn6;
    public uint unkn7;
    public uint unkn8;
	[RszVersion('<', EfxVersion.DD2)]
    public uint unkn9;
    public float unkn10;
    public float unkn11;
    public float unkn12;
    public float unkn13;
    public float unkn14;
    public float unkn15;
    public float unkn16;
    public float unkn17;
    public float unkn18;
    public float unkn79;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_244Expression, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_244 : EFXAttribute
{
	public EFXAttributeUnknownRE4_244() : base(EfxAttributeType.UnknownRE4_244Expression) { }

    public uint unkn1_0;
    public uint unkn1_1;
    public uint unkn1_2;
    public uint unkn1_3;
    public uint unkn1_4;
    public uint unkn1_5;
    public uint unkn1_6;
    public uint unkn1_7;
    public uint unkn1_8;
    public uint unkn1_9;
    public uint unkn1_10;
    public uint unkn1_12;
    public uint unkn1_13;
    public uint unkn1_14;
    public uint unkn1_15;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_245_UnknType, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_245_UnknType : EFXAttribute
{
	public EFXAttributeUnknownRE4_245_UnknType() : base(EfxAttributeType.UnknownRE4_245_UnknType) { }

    public UndeterminedFieldType unkn0;
    public via.Color color0;
    public via.Color color1;
    public float unkn3;

    public UndeterminedFieldType unkn4;
    public UndeterminedFieldType unkn5;
    public float unkn6;

    public uint unkn7;
    public UndeterminedFieldType unkn8;
    public uint unkn9;
    public float unkn10;
    public UndeterminedFieldType unkn11;
    public UndeterminedFieldType unkn12;
    public float unkn13;
    public UndeterminedFieldType unkn14;
    public UndeterminedFieldType unkn15;
    public float unkn16;
    public float unkn17;
    public float unkn18;
    public UndeterminedFieldType unkn19;
    public float unkn20;
    public UndeterminedFieldType unkn21;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_247_UnknTypeGPU, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeUnknownRE4_247_UnknTypeGPU : EFXAttribute
{
	public EFXAttributeUnknownRE4_247_UnknTypeGPU() : base(EfxAttributeType.UnknownRE4_247_UnknTypeGPU) { }

    public uint unkn0;
    public via.Color color0;
    public via.Color color1;
    public float unkn3;
    public UndeterminedFieldType unkn4;
    public UndeterminedFieldType unkn5;
    public float unkn6;

    public uint unkn7;
    public uint unkn8;
    public uint unkn9;
    public uint unkn10;
	[RszVersion('<', EfxVersion.DD2)]
    public uint unkn11;

    public float unkn12;
    public float unkn13;
    public float unkn14;
    public float unkn15;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_248_UnknTypeGpuExpression, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_248 : EFXAttribute
{
	public EFXAttributeUnknownRE4_248() : base(EfxAttributeType.UnknownRE4_248_UnknTypeGpuExpression) { }

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
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_249_UnknTypeB, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeUnknownRE4_249_UnknTypeB : EFXAttribute
{
	public EFXAttributeUnknownRE4_249_UnknTypeB() : base(EfxAttributeType.UnknownRE4_249_UnknTypeB) { }

    public uint unkn0;
    public via.Color color0;
    public via.Color color1;
    public float unkn3;
    public float unkn4;
    public float unkn5;
    public float unkn6;
    public uint unkn7;
	[RszVersion('<', EfxVersion.DD2)]
    public uint unkn8;
    public float unkn9;
    public float unkn10;
    public float unkn11;
    public float unkn12;
    public float unkn13;
    public uint unkn14;
    public float unkn15;
    public float unkn16;
    public uint unkn17;
    public float unkn18;
    public float unkn19;
    public float unkn20;
    public float unkn21;
    public float unkn22;
    public float unkn23;
    public float unkn24;
    public float unkn25;
}
