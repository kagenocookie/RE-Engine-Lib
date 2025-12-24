using System.Numerics;
using ReeLib.Efx.Structs.Common;
using ReeLib.InternalAttributes;

namespace ReeLib.Efx.Structs.Field;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.VectorFieldParameter, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeVectorFieldParameter : EFXAttribute
{
	public EFXAttributeVectorFieldParameter() : base(EfxAttributeType.VectorFieldParameter) { }

	public Vector3 Parameter;
	public float Lacunarity;
	public float Gain;
	public float Blend;
	public float lfoRange;
	public float lfoTime;
	public Vector3 Rotation;
    public Vector3 Scale;
    public via.Range Coefficient;
	[RszVersion(EfxVersion.RE8, EndAt = nameof(Flags))]
	public float EdgeOffset;
	public float Falloff;
	public uint Flags;
	public int FieldIndex;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.VectorFieldParameterClip, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeVectorFieldParameterClip : EFXAttribute, IClipAttribute
{
    public EfxClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeVectorFieldParameterClip() : base(EfxAttributeType.VectorFieldParameterClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(13);
    uint unkn1;
	[RszClassInstance] public EfxClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.VectorFieldParameterExpression, EfxVersion.DMC5, EfxVersion.RE8, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeVectorFieldParameterExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeVectorFieldParameterExpression() : base(EfxAttributeType.VectorFieldParameterExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(20);
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
	[RszVersion('>', EfxVersion.DMC5, EndAt = nameof(unkn20))]
	public ExpressionAssignType unkn12;
	public ExpressionAssignType unkn13;
	public ExpressionAssignType unkn14;
	public ExpressionAssignType unkn15;
	public ExpressionAssignType unkn16;
	public ExpressionAssignType unkn17;
	public ExpressionAssignType unkn18;
	public ExpressionAssignType unkn19;
	public ExpressionAssignType unkn20;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.GlobalVectorField, EfxVersion.RE8)]
public partial class EFXAttributeGlobalVectorField : EFXAttribute
{
	public EFXAttributeGlobalVectorField() : base(EfxAttributeType.GlobalVectorField) { }

	public uint TargetCount;
	public float LocalToGlobalBlend;
	public float VelocityBlend;
	public Vector4 Weight;
    public Vector4 InfluenceFrame;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.DirectionalFieldParameter, EfxVersion.RE8)]
public partial class EFXAttributeDirectionalFieldParameter : EFXAttribute
{
	public EFXAttributeDirectionalFieldParameter() : base(EfxAttributeType.DirectionalFieldParameter) { }

	public uint Flags;
    public via.Range Speed;
    public via.Range SpeedCoef;
    public Vector2 SourcePointUV;
    public Vector2 SourceRadius;
    public Vector2 SinkPointUV;
    public Vector2 SinkRadius;
    public int FieldIndex;
}
