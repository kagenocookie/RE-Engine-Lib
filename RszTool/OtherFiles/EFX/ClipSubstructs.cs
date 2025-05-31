using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.Common;

// might be coded as a "Playback / loop trigger" flag enum; -1 = everything triggers a loop, 0 = nothing does (manual?)
public enum EfxClipPlaybackType {
    Looping = -1,
    Unknown = 0,
    NonLooping = 2,
    Type4 = 4,
}

public struct EfxClipHeader
{
    public uint frameCount;
    public uint unkn1;
}

/// <summary>
/// This being an interpolation is just an educated guess. It's certainly not the value type, doesn't seem like a "keyframe type" thing either
/// (linear, quadratic, ease-in-ease-out, logarithmic, exponential, bezier, ...?)
/// Maybe the same enum as .clip files?
/// </summary>
public enum FrameInterpolationType
{
    Type1 = 1, // found at end
    Type2 = 2, // found in start, middle, end; found as 2-only list
    Type3 = 3,
    /// <summary>
    /// Most likely a bezier curve. Has an additional tangent data section in the main clip struct.
    /// </summary>
    Type5 = 5,
    Type13 = 13, // (dmc5 only)
}

public struct EfxFloatClipFrame
{
    public float frameTime;
    public FrameInterpolationType type;
    public float value;
}

public struct EfxColorClipFrame
{
    public float frameTime;
    public FrameInterpolationType type;
    public int value;
}

/// <summary>
/// There's one of these for each type 5 frame in efx structs, probably the tangents
/// </summary>
public struct EfxClipInterpolationTangents
{
    // out_y, out_x, in_y, in_x?
    public float value1;
    public float value2;
    public float value3;
    public float value4;

    public override string ToString() => $"{value1}, {value2}, {value3}, {value4}";
}

public struct EfxMaterialClip_Struct4
{
    public uint mdfPropertyHash;
    public int unkn1; // values: 0, 1, 2, 3;  NOT a clip index
    public int unkn2; // values: 0, 25, 28;   NOT a clip index
    public int unkn3; // value always 5;      maybe same 5 as the clip header unknown?
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion))]
public abstract partial class EfxClipDataBase<TClipFrameType> : BaseModel where TClipFrameType : unmanaged
{
    public EfxClipPlaybackType loopType;
    public float clipDuration;
	[RszArraySizeField(nameof(clips))] public int clipCount;
	[RszArraySizeField(nameof(frames))] public int frameCount;
	[RszArraySizeField(nameof(interpolationData))] public int interpolationDataCount;
	[RszByteSizeField(nameof(clips))] public int clipDataSize;
	[RszByteSizeField(nameof(frames))] public int frameDataSize;
	[RszByteSizeField(nameof(interpolationData))] public int interpolationDataSize;
    [RszFixedSizeArray(nameof(clipCount))] public EfxClipHeader[]? clips;
    [RszFixedSizeArray(nameof(frameCount))] public TClipFrameType[]? frames;
    [RszFixedSizeArray(nameof(interpolationDataCount))] public EfxClipInterpolationTangents[]? interpolationData;

    public override string ToString() => $"({clipCount} clips, {frameCount} frames{(interpolationDataCount > 0 ? $", {interpolationDataCount} interp" : "")})";
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class EfxClipData : EfxClipDataBase<EfxFloatClipFrame>
{
	public static new IEnumerable<(string name, Type type)> GetFieldList(EfxVersion Version) => EfxClipDataBase<EfxFloatClipFrame>.GetFieldList(Version);
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class EfxColorClipData : EfxClipDataBase<EfxColorClipFrame>
{
	public static new IEnumerable<(string name, Type type)> GetFieldList(EfxVersion Version) => EfxClipDataBase<EfxColorClipFrame>.GetFieldList(Version);
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion))]
public partial class EfxMaterialClipData : BaseModel
{
    [RszIgnore] public EfxVersion Version;

    public EfxMaterialClipData() { }
    public EfxMaterialClipData(EfxVersion version)
    {
        Version = version;
    }

    public int Length => clipCount;

    public EfxClipPlaybackType loopType;
    public float clipDuration;
	[RszArraySizeField(nameof(clips))] public int clipCount;
	[RszArraySizeField(nameof(frames))] public int frameCount;
	[RszArraySizeField(nameof(interpolationData))] public int interpolationDataCount;
    [RszByteSizeField(nameof(clips))] public int clipDataSize;
    [RszByteSizeField(nameof(frames))] public int frameDataSize;
    [RszByteSizeField(nameof(interpolationData))] public int interpolationDataSize;
	[RszVersion(EfxVersion.RE4)]
    [RszArraySizeField(nameof(mdfProperties))] public int mdfPropertyCount;
    [RszVersion(EfxVersion.RE3)]
    [RszArraySizeField(nameof(indices))] public int indicesCount;

    [RszFixedSizeArray(nameof(clipCount))] public EfxClipHeader[]? clips;
    [RszFixedSizeArray(nameof(frameCount))] public EfxFloatClipFrame[]? frames;
    [RszFixedSizeArray(nameof(interpolationDataCount))] public EfxClipInterpolationTangents[]? interpolationData;
    [RszFixedSizeArray(nameof(Version), ">=", EfxVersion.RE4, "?", nameof(mdfPropertyCount), ":", nameof(clipCount))]
    public EfxMaterialClip_Struct4[]? mdfProperties;
    [RszVersion(EfxVersion.RE3)]
    // NOT mdf property index (vaguely similar but inconsistent correlation)
    [RszFixedSizeArray(nameof(indicesCount))] public uint[]? indices;

    public override string ToString() => $"({clipCount} clips, {frameCount} frames, props: {mdfPropertyCount}, indices: {indicesCount})";
}
