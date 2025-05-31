using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.Main;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeNoDraw, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTypeNoDraw : EFXAttribute
{
	public EFXAttributeTypeNoDraw() : base(EfxAttributeType.TypeNoDraw) { }

	public uint unkn0;
	public via.Color color1;
	public via.Color color2;
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
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeNoDrawExpression, EfxVersion.RE3, EfxVersion.RE4)]
public partial class EFXAttributeTypeNoDrawExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeNoDrawExpression() : base(EfxAttributeType.TypeNoDrawExpression) { }

	// TODO bitset versioning
	// re3: 2 = alpha
	// re3rt: 3 = alpha
	// [RszClassInstance] public readonly BitSet expressionBits = new BitSet(16) { BitNameDict = new () {
	// 	[1] = nameof(color),
	// 	[2] = nameof(colorRand),
	// 	[3] = nameof(alpha),
	// 	[4] = nameof(alphaRand),
	// } };
	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(16) { BitNameDict = new () {
		[1] = nameof(color),
		[2] = nameof(alpha),
	} };
    public ExpressionAssignType color;
	[RszVersion('>', EfxVersion.RE3)]
    public ExpressionAssignType colorRand;
    public ExpressionAssignType alpha;
	[RszVersion('>', EfxVersion.RE3)]
    public ExpressionAssignType alphaRand;
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

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnitCulling, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RERT)]
public partial class EFXAttributeUnitCulling : EFXAttribute
{
	public EFXAttributeUnitCulling() : base(EfxAttributeType.UnitCulling) { }

	public uint cullingFlags;
	public float unkn1; // 1-6 seem like hashes sometimes, may be some sort of conditional substruct
	public float unkn2;
	public float unkn3;
	public float cullingRadius1;
	public float cullingRadius2;
	public float cullingRadius3;
	public float unkn7;
	public float unkn8;
	public float unkn9;
    [RszVersion(EfxVersion.RE8)]
	public float cullingDistance;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnitCullingExpression, EfxVersion.DD2)]
public partial class EFXAttributeUnitCullingExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeUnitCullingExpression() : base(EfxAttributeType.UnitCullingExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(11) { BitNameDict = new() {
        [7] = nameof(cullingRadius),
        [11] = nameof(cullingDistance),
    } };
    public ExpressionAssignType unkn1;
    public ExpressionAssignType unkn2;
    public ExpressionAssignType unkn3;
    public ExpressionAssignType unkn4;
    public ExpressionAssignType unkn5;
    public ExpressionAssignType unkn6;
    public ExpressionAssignType cullingRadius;
    public ExpressionAssignType unkn8;
    public ExpressionAssignType unkn9;
    public ExpressionAssignType unkn10;
    public ExpressionAssignType cullingDistance;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}
