using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.Main;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeMesh, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3)]
public partial class EFXAttributeTypeMesh : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeMesh() : base(EfxAttributeType.TypeMesh) { }

    public uint ukn1;
    public uint overriddenHashCount;
    public via.Color color1;
    public via.Color color2;
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

[RszGenerate, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeMesh, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2, EfxVersion.MHWilds)]
public partial class EFXAttributeTypeMeshV2 : EFXAttribute
{
    public EFXAttributeTypeMeshV2() : base(EfxAttributeType.TypeMesh) { }

    public uint ukn1;
    [RszVersion(EfxVersion.RE4)]
	public uint re4_unkn0;
    public via.Color color1;
    public via.Color color2;
    public float unkn4;
    public via.Color color3;
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
    [RszVersion(EfxVersion.MHWilds)]
    public uint mhws_unkn1;
    [RszVersion(EfxVersion.DD2)]
    public float dd2_unkn1;
    [RszArraySizeField(nameof(texPaths))] public int texCount;
    [RszVersion(EfxVersion.DD2)]
    public uint dd2_unkn2;
    [RszInlineWString] public string? meshPath;
    [RszInlineWString] public string? unknPath;
    [RszInlineWString] public string? mdfPath;

	[RszByteSizeField(nameof(properties))] public int propertiesDataSize;
	[RszClassInstance, RszList(nameof(propertiesDataSize), '/', 32), RszConstructorParams(nameof(Version))]
	public List<MdfProperty> properties = new();

	public int texPathBlockLength;
	[RszInlineWString, RszList(nameof(texCount))] public string[]? texPaths;

    protected override bool DoRead(FileHandler handler) => DefaultRead(handler);
    protected override bool DoWrite(FileHandler handler)
    {
        propertiesDataSize = properties.Count * MdfProperty.GetSize(Version);
        return DefaultWrite(handler);
    }
}


[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeMeshClip, EfxVersion.RE3, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeTypeMeshClip : EFXAttribute, IMaterialClipAttribute
{
    public EfxMaterialClipData MaterialClip => clipData;
    EfxClipData IClipAttribute.Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeTypeMeshClip() : base(EfxAttributeType.TypeMeshClip) { }

	/// <summary>
	/// Always 0 pre-RE4
	/// </summary>
	[RszClassInstance] public readonly BitSet clipBits = new BitSet(13);

	[RszVersion("<=", EfxVersion.DMC5), RszArraySizeField(nameof(clipData))] public int mdfPropertyCount;
	[RszVersion(nameof(Version), ">=", EfxVersion.RE3, "&&", nameof(Version), "<=", EfxVersion.RERT), RszArraySizeField(nameof(clipData))] // else if
	public int mdfPropertyCountDouble;
	[RszVersion(EfxVersion.RE4)] public uint unkn1; // else

	[RszClassInstance, RszConstructorParams(nameof(Version))] public readonly EfxMaterialClipData clipData = new();
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeMeshExpression, EfxVersion.RE7, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTypeMeshExpression : EFXAttribute, IExpressionAttribute, IMaterialExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;
	public BitSet ExpressionBits => expressionBits;

    public EFXAttributeTypeMeshExpression() : base(EfxAttributeType.TypeMeshExpression) { }

	[RszVersion(nameof(Version), ">=", EfxVersion.RE2, "&&", nameof(Version), "<", EfxVersion.RE4, EndAt = nameof(matExpressionSize))]
    [RszArraySizeField(nameof(materialExpressionsList))] public int matExpressionCount;
    [RszByteSizeField(nameof(materialExpressionsList))] public uint matExpressionSize;

	[RszVersion(nameof(Version), ">=", EfxVersion.RE3, "&&", nameof(Version), "<", EfxVersion.RE4)]
    public int indicesCount;

    // dd2, likely re4 TODO versioned bitset
	// [RszClassInstance] public readonly BitSet expressionBits = new BitSet(25) { BitNameDict = new () {
    //     [1] = nameof(color1),
    //     [2] = nameof(color1Rand),
    //     [3] = nameof(alpha),
    //     [4] = nameof(alphaRand),

    //     [16] = "scale",
    //     [17] = "scaleRand",
    //     [18] = "sizeX",
    //     [19] = "sizeXRange",
    //     [20] = "sizeY",
    //     [21] = "sizeYRange",
    //     [22] = "sizeY",
    //     [23] = "sizeYRange",
	// }};
    // re7, dmc5:
	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(25) { BitNameDict = new () {
		[1] = nameof(color1),
		[2] = nameof(color1Rand),
		[3] = nameof(alpha),
		[4] = nameof(alphaRand),
		[6] = "speed",
		[7] = "speedRand",

        [14] = "scale",
        [15] = "scaleRand",
		[16] = "sizeX",
        [17] = "sizeXRange",
		[18] = "sizeY",
		[19] = "sizeYRange",
		[20] = "sizeY",
		[21] = "sizeYRange",
	}};
    public ExpressionAssignType color1;
    public ExpressionAssignType color1Rand;
    public ExpressionAssignType alpha;
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
    public ExpressionAssignType unkn17;
    public ExpressionAssignType unkn18;
    public ExpressionAssignType unkn19;
    public ExpressionAssignType unkn20;
    public ExpressionAssignType unkn21;
	[RszVersion(">", EfxVersion.RE3, EndAt = nameof(unkn22))]
    public ExpressionAssignType unkn22;
	[RszVersion(EfxVersion.RE8)]
    public ExpressionAssignType unkn23;
	[RszVersion(EfxVersion.RE4, EndAt = nameof(unkn25))]
    public ExpressionAssignType unkn24;
    public ExpressionAssignType unkn25;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;

	// the pre-RE4 struct here was retarded, so we need some manual finessing to make it behave nicely
	// it's manually handled for pre-RE4 to give us identical code everwhere else despite having separate size fields
	[RszVersion(EfxVersion.RE4, EndAt = nameof(materialExpressions))]
    [RszArraySizeField(nameof(materialExpressions))] public int materialExpressionsCount;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXMaterialExpressionList? materialExpressions;

	[RszList(nameof(matExpressionCount)), RszClassInstance, RszVersion(nameof(Version), ">=", EfxVersion.RE2, "&&", nameof(Version), '<', EfxVersion.RE4)]
	[RszIgnore]
	public List<EFXMaterialExpression>? materialExpressionsList;
	[RszFixedSizeArray(nameof(indicesCount)), RszVersion(nameof(Version), ">=", EfxVersion.RE3, "&&", nameof(Version), "<", EfxVersion.RE4)]
	[RszIgnore]
	public uint[]? materialIndices;

    protected override bool DoRead(FileHandler handler)
    {
        DefaultRead(handler);

		if (Version >= EfxVersion.RE2 && Version < EfxVersion.RE4) {
			materialExpressionsList ??= new();
			for (int i = 0; i < matExpressionCount; ++i) {
				var item = new EFXMaterialExpression(Version);
				item.Read(handler);
				materialExpressionsList.Add(item);
			}
			materialExpressions ??= new EFXMaterialExpressionList(Version) {
				solverSize = matExpressionSize,
			};
			materialExpressions.expressions = materialExpressionsList;

			if (Version >= EfxVersion.RE3) {
				materialIndices = handler.ReadArray<uint>((int)(indicesCount));
				materialExpressions ??= new EFXMaterialExpressionList(Version);
				materialExpressions.indices = materialIndices;
				materialExpressions.indexCount = indicesCount;
			}
		}

		return true;
    }

    protected override bool DoWrite(FileHandler handler)
    {
		if (Version >= EfxVersion.RE2 && Version < EfxVersion.RE4) {
			if (materialExpressions == null) {// TODO verify works
				// prob nothing to do here
			} else {
				materialExpressionsList = materialExpressions.expressions;
				matExpressionCount = materialExpressions.ExpressionCount;

				if (Version >= EfxVersion.RE3) {
					materialIndices = materialExpressions.indices;
				}
			}
		}
		var start = handler.Tell();
		indicesCount = materialExpressions?.indices?.Length ?? 0;
		DefaultWrite(handler);

		if (Version >= EfxVersion.RE2 && Version < EfxVersion.RE4 && materialExpressionsList != null) {
			var arrayStart = handler.Tell();
			materialExpressionsList?.Write(handler);
			var arrayEnd = handler.Tell();
			matExpressionSize = (uint)(arrayEnd - arrayStart);
			handler.Write(start + sizeof(uint), matExpressionSize);
		}

		if (Version >= EfxVersion.RE3 && Version < EfxVersion.RE4) {
			materialIndices ??= new uint[indicesCount];
			handler.WriteArray(materialIndices);
		}

		return true;
    }

	public static IEnumerable<(string name, Type type)> GetFieldList(EfxVersion Version)
	{
		foreach (var f in GetFieldListDefault(Version)) yield return f;

		if (Version >= EfxVersion.RE2 && Version < EfxVersion.RE4) { // end at: materialExpressionsList
			yield return (nameof(materialExpressionsList), typeof(List<EFXMaterialExpression>));
		}

		if (Version >= EfxVersion.RE3 && Version < EfxVersion.RE4) { // end at: indices
			yield return (nameof(materialIndices), typeof(uint[]));
		}
	}
}


[RszGenerate, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuMesh, EfxVersion.RE8, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTypeGpuMesh : EFXAttribute
{
    public EFXAttributeTypeGpuMesh() : base(EfxAttributeType.TypeGpuMesh) { }

    public byte unkn0_1;
    public byte unkn0_2;
    public byte unkn0_3;
    public byte unkn0_4;
	[RszVersion(EfxVersion.MHWilds)]
    public float mhws_unkn1;
	[RszVersion(EfxVersion.MHWilds)]
    public float mhws_unkn2;
    public uint unkn1;
    public uint unkn2;
	[RszVersion('<', EfxVersion.DD2)]
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
    public float unkn13;
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
	[RszVersion(EfxVersion.DD2)]
    public float dd2_unkn;

	[RszArraySizeField(nameof(texturePaths))] public int texCount;
	[RszVersion(EfxVersion.DD2)]
    public uint dd2_unkn2;
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuMeshClip, EfxVersion.DD2)]
public partial class EFXAttributeTypeGpuMeshClip : EFXAttribute, IMaterialClipAttribute
{
    public EfxMaterialClipData MaterialClip => clipData;
    EfxClipData IClipAttribute.Clip => clipData;
    public BitSet ClipBits => clipBits;

	public EFXAttributeTypeGpuMeshClip() : base(EfxAttributeType.TypeGpuMeshClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(1);
    public uint unkn1;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public readonly EfxMaterialClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuMeshExpression, EfxVersion.RE8)]
public partial class EFXAttributeTypeGpuMeshExpression : EFXAttribute, IExpressionAttribute, IMaterialExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeGpuMeshExpression() : base(EfxAttributeType.TypeGpuMeshExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(19) { BitNameDict = new () {
		[1] = nameof(color),
		[2] = nameof(colorRand),
		[3] = nameof(alpha),
		[4] = nameof(alphaRate),
		[12] = nameof(size),
		[13] = nameof(sizeRand),
	} };
	public ExpressionAssignType color;
	public ExpressionAssignType colorRand;
	public ExpressionAssignType alpha;
	public ExpressionAssignType alphaRate;
	public ExpressionAssignType unkn5;
	public ExpressionAssignType unkn6;
	public ExpressionAssignType unkn7;
	public ExpressionAssignType unkn8;
	public ExpressionAssignType unkn9;
	public ExpressionAssignType unkn10;
	public ExpressionAssignType unkn11;
	public ExpressionAssignType size;
	public ExpressionAssignType sizeRand;
	public ExpressionAssignType unkn14;
	public ExpressionAssignType unkn15;
	public ExpressionAssignType unkn16;
	public ExpressionAssignType unkn17;
	public ExpressionAssignType unkn18;
	public ExpressionAssignType unkn19;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;

    [RszVersion(EfxVersion.RERT, EndAt = nameof(materialExpressions))]
	public uint materialPropertyCount;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXMaterialExpressionList? materialExpressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuMeshTrail, EfxVersion.RE8)]
public partial class EFXAttributeTypeGpuMeshTrail : EFXAttribute
{
	public EFXAttributeTypeGpuMeshTrail() : base(EfxAttributeType.TypeGpuMeshTrail) { }

	public uint unkn0;
    public uint unkn1;
    public uint unkn2;
	[RszVersion('<', EfxVersion.DD2)]
    public uint unkn3;
    public via.Color color0;
    public via.Color color1;
    public float unkn4;
    public uint unkn5;
	[RszVersion(EfxVersion.DD2)]
    public uint dd2_unkn1;
    public UndeterminedFieldType unkn6;
    public UndeterminedFieldType unkn7;
    public float unkn8;
    public UndeterminedFieldType unkn9;
    public UndeterminedFieldType unkn10;
    public UndeterminedFieldType unkn11;
    public float unkn12;
    public UndeterminedFieldType unkn13;
    public float unkn14;
    public UndeterminedFieldType unkn15;
    public float unkn16;
    public UndeterminedFieldType unkn17;
    public float unkn18;
	[RszVersion(EfxVersion.DD2)]
    public uint dd2_unkn2;
    public UndeterminedFieldType unkn19;
    public UndeterminedFieldType unkn20;
    public UndeterminedFieldType unkn21;
    public uint unkn22_count;
    public UndeterminedFieldType unkn23;
    public uint unkn24;
    public UndeterminedFieldType unkn25;
    public UndeterminedFieldType unkn26;
    public UndeterminedFieldType unkn27;
    public UndeterminedFieldType unkn28;
    public UndeterminedFieldType unkn29;
    public float unkn30;
    public float unkn31;
    public UndeterminedFieldType unkn32;
    public UndeterminedFieldType unkn33;
    public UndeterminedFieldType unkn34;
    public UndeterminedFieldType unkn35;
    public uint unkn36;
    public uint unkn37;
    public UndeterminedFieldType unkn38;
    public float unkn39;
    public float unkn40;
    public uint unkn41; // "rest" array length?
    public UndeterminedFieldType unkn42;
	[RszInlineWString] public string? meshPath;
	[RszInlineWString] public string? uknPath;
	[RszInlineWString] public string? mdfPath;

    public uint dataSize;
	// [RszFixedSizeArray(nameof(dataSize), '/', 4)] public uint[]? data;
    public uint texBlockLength;
	// [RszInlineWString, RszList(nameof(texCount))] public string[]? texPaths;
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuMeshTrail, EfxVersion.DD2)]
public partial class EFXAttributeTypeGpuMeshTrailV2 : EFXAttribute
{
	public EFXAttributeTypeGpuMeshTrailV2() : base(EfxAttributeType.TypeGpuMeshTrail) { }

	public uint unknFlags;
	public uint unkn1;
	public uint unkn2;
	public via.Color color1;
	public via.Color color2;
	public float unkn5;
	public uint unkn6;
	public uint unkn7;
	public uint unkn8;
	public float unkn9;
	public float unkn10;
	public float unkn11;
	public UndeterminedFieldType unkn12;
	public uint unkn13;
	public UndeterminedFieldType unkn14;
	public uint unkn15;
	public UndeterminedFieldType unkn16;
	public UndeterminedFieldType unkn17;
	public UndeterminedFieldType unkn18;
	public UndeterminedFieldType unkn19;
	public UndeterminedFieldType unkn20;
	public UndeterminedFieldType unkn21;
	public float unkn22;
	public UndeterminedFieldType unkn23;
	public float unkn24;
	public UndeterminedFieldType unkn25;
	public float unkn26;
	public UndeterminedFieldType unkn27;
	public float unkn28;
	public UndeterminedFieldType unkn29;
	public uint unkn30;
	public float unkn31;
	public uint texCount;
	public UndeterminedFieldType unkn32;
	public uint unkn33;
	public uint unkn34;
	public uint unkn35;
	public float unkn36;
	public uint unkn37;
	public uint unkn38;
	public uint unkn39;
	public float unkn40;
	public float unkn41;
	public float unkn42;
	public float unkn43;
	public float unkn44;
	public UndeterminedFieldType unkn45;
	public UndeterminedFieldType unkn46;
	public float unkn47;
	public float unkn48;
	public float unkn49;
	public float unkn50;
	public float unkn51;
	public float unkn52;
	public float unkn53;
	public UndeterminedFieldType unkn54;
	public float unkn55;
	public UndeterminedFieldType unkn56;
	[RszInlineWString] public string? meshPath;
	[RszInlineWString] public string? unkPath;
	[RszInlineWString] public string? mdfPath;
	[RszByteSizeField(nameof(properties))] public int propertiesDataSize;
	[RszClassInstance, RszList(nameof(propertiesDataSize), '/', 32), RszConstructorParams(nameof(Version))]
	public List<MdfProperty> properties = new();
	public uint texBlockLength;
	[RszInlineWString, RszList(nameof(texCount))] public string[]? texPaths;

    protected override bool DoRead(FileHandler handler) => DefaultRead(handler);
    protected override bool DoWrite(FileHandler handler)
    {
        propertiesDataSize = properties.Count * MdfProperty.GetSize(Version);
        return DefaultWrite(handler);
    }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuMeshTrailClip, EfxVersion.DD2)]
public partial class EFXAttributeTypeGpuMeshTrailClip : EFXAttribute, IMaterialClipAttribute
{
    public EfxMaterialClipData MaterialClip => clipData;
    EfxClipData IClipAttribute.Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeTypeGpuMeshTrailClip() : base(EfxAttributeType.TypeGpuMeshTrailClip) { }

	/// <summary>
	/// Marks what is clipped with this attributes.
	/// Bit: 0x4: unknown non-mdf property
	/// Bits 0x0f0000 (and possibly all bits after as well): up to 4 mdf properties
	/// </summary>
	[RszClassInstance] public readonly BitSet clipBits = new BitSet(23) { BitNameDict = new() {
		[17] = "MdfProperty1",
		[18] = "MdfProperty2",
		[19] = "MdfProperty3",
		[20] = "MdfProperty4",
		[21] = "MdfProperty5",
		[22] = "MdfProperty6",
		[23] = "MdfProperty7",
	}};
    public uint unkn1;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public readonly EfxMaterialClipData clipData = new();
}


[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuMeshTrailExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeGpuMeshTrailExpression : EFXAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeGpuMeshTrailExpression() : base(EfxAttributeType.TypeGpuMeshTrailExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(21) { BitNameDict = new () {
		[2] = nameof(color1R),
		[3] = nameof(color1G),
		[4] = nameof(color1B),
	}};
    public ExpressionAssignType unkn1;
    public ExpressionAssignType color1R;
    public ExpressionAssignType color1G;
    public ExpressionAssignType color1B;
    public ExpressionAssignType unkn5;
    public ExpressionAssignType unkn6;
    public ExpressionAssignType unkn7;
    public ExpressionAssignType unkn8;
    public ExpressionAssignType unkn9;
    public ExpressionAssignType unkn10;
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
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
	public int materialExpressionCount;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXMaterialExpressionList? materialExpressions;
}
