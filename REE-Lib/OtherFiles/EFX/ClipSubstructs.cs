using System.Runtime.InteropServices;
using ReeLib.Common;
using ReeLib.InternalAttributes;

namespace ReeLib.Efx.Structs.Common;

// might be coded as a "Playback / loop trigger" flag enum; -1 = everything triggers a loop, 0 = nothing does (manual?)
public enum EfxClipPlaybackType {
    Looping = -1,
    Unknown = 0,
    NonLooping = 2,
    Type4 = 4,
}

public enum ClipValueType : int
{
    Int = 3,
    Float = 5,
}

public struct EfxClipHeader
{
    public int frameCount;
    public ClipValueType valueType;

    public override string ToString() => $"{frameCount} {valueType}";
}

/// <summary>
/// This being an interpolation is just an educated guess. It's certainly not the value type, doesn't seem like a "keyframe type" thing either
/// (linear, quadratic, ease-in-ease-out, logarithmic, exponential, bezier, ...?)
/// Maybe the same enum as .clip files?
/// </summary>
public enum FrameInterpolationType
{
	Unknown = 0,
    Type1 = 1, // found at end
    Type2 = 2, // found in start, middle, end; found as 2-only list
    Type3 = 3,
    /// <summary>
    /// Most likely a bezier curve. Has an additional tangent data section in the main clip struct.
    /// </summary>
    Bezier = 5,
    Type13 = 13, // (dmc5 only)
}

public struct EfxClipFrame
{
    public float frameTime;
    public FrameInterpolationType type;
    private float value;

    public EfxClipFrame()
    {
		type = FrameInterpolationType.Type2;
    }

    public int IntValue { get => BitConverter.SingleToInt32Bits(value); set => BitConverter.Int32BitsToSingle(value); }
	public float FloatValue { get => value; set => this.value = value; }

	public float AsFloat(ClipValueType type) => type switch {
		ClipValueType.Int => IntValue,
		ClipValueType.Float => FloatValue,
		_ => FloatValue,
	};

	public void FromFloat(ClipValueType type, float val)
	{
		if (type == ClipValueType.Int) {
			value = BitConverter.Int32BitsToSingle((int)val);
		} else {
			value = val;
		}
	}

    public override string ToString() => frameTime + ": " + (BitConverter.SingleToInt32Bits(value) is int intval && intval >= 0 && intval <= 255 ? intval : value);
}

/// <summary>
/// Represents the tangents of a Bezier (Type5) clip keyframe.
/// </summary>
public struct EfxClipInterpolationTangents
{
    public float out_x;
    public float out_y;
    public float in_x;
    public float in_y;

    public override string ToString() => $"{out_x}, {out_y}, {in_x}, {in_y}";
}

public struct EfxMaterialClip_Struct4
{
    public uint mdfPropertyHash;
    public int unkn1; // values: 0, 1, 2, 3;  NOT a clip index
    public int unkn2; // values: 0, 25, 28;   NOT a clip index
	/// <summary>
	/// Likely to be <see cref="ClipValueType"/> enum - always 5 in known files.
	/// </summary>
    public int unkn3;

    public EfxMaterialClip_Struct4()
    {
		unkn3 = 5;
    }
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class EfxClipData : BaseModel
{
    public EfxClipPlaybackType loopType;
    public float clipDuration;
	[RszArraySizeField(nameof(clips))] public int clipCount;
	[RszArraySizeField(nameof(frames))] public int frameCount;
	[RszArraySizeField(nameof(interpolationData))] public int interpolationDataCount;
	[RszByteSizeField(nameof(clips))] public int clipDataSize;
	[RszByteSizeField(nameof(frames))] public int frameDataSize;
	[RszByteSizeField(nameof(interpolationData))] public int interpolationDataSize;
    [RszIgnore, RszFixedSizeArray(nameof(clipCount))] public EfxClipHeader[]? clips;
    [RszIgnore, RszFixedSizeArray(nameof(frameCount))] public EfxClipFrame[]? frames;
    [RszIgnore, RszFixedSizeArray(nameof(interpolationDataCount))] public EfxClipInterpolationTangents[]? interpolationData;

	[RszIgnore] private EfxClipList? parsedClips;
	public bool IsParsed => parsedClips != null;
	public EfxClipList ParsedClip { get => parsedClips ??= ParseClip(); }

    public override string ToString() => $"({clipCount} clips, {frameCount} frames{(interpolationDataCount > 0 ? $", {interpolationDataCount} interp" : "")})";

	protected void ReadHeader(FileHandler handler)
	{
		handler.Read(ref loopType);
		handler.Read(ref clipDuration);
		handler.Read(ref clipCount);
		handler.Read(ref frameCount);
		handler.Read(ref interpolationDataCount);
		handler.Read(ref clipDataSize);
		handler.Read(ref frameDataSize);
		handler.Read(ref interpolationDataSize);
	}

	protected void ReadData(FileHandler handler)
	{
		clips = new EfxClipHeader[clipCount];
		for (int i = 0; i < clipCount; ++i) {
			clips[i] = new EfxClipHeader() {
				frameCount = handler.Read<int>(),
				valueType = handler.Read<ClipValueType>(),
			};
		}
		frames = handler.ReadArray<EfxClipFrame>((int)(frameCount));
		interpolationData = handler.ReadArray<EfxClipInterpolationTangents>((int)(interpolationDataCount));
	}

	public EfxClipList ParseClip()
	{
		parsedClips ??= new EfxClipList();
		clipCount = clips!.Length;
		frameCount = frames!.Length;
		interpolationDataCount = interpolationData!.Length;

        int frameIndex = 0;
        int tangentIndex = 0;
        for (int i = 0; i < clipCount; ++i) {
            var clip = clips[i];
            var newclip = new EfxParsedClip() {
                clipTargetBit = -1,
                valueType = clip.valueType,
            };

            for (var f = 0; f < clip.frameCount; ++f) {
                var frame = frames[frameIndex++];
                newclip.frames.Add(new EfxParsedClipFrame() {
                    data = frame,
                    tangents = frame.type == FrameInterpolationType.Bezier ? interpolationData[tangentIndex++] : default,
                });
            }

            parsedClips.clips.Add(newclip);
        }

		return parsedClips;
	}

	public void SetFromClipList(EfxClipList list)
	{
		parsedClips = list;
		clipCount = list.clips.Count;
        clipDataSize = clipCount * Marshal.SizeOf<EfxClipHeader>();
		if (clips == null || clips.Length != clipCount) clips = new EfxClipHeader[clipCount];
		frameCount = list.FrameCount;
        frameDataSize = frameCount * Marshal.SizeOf<EfxClipFrame>();
		if (frames == null || frames.Length != frameCount) frames = new EfxClipFrame[frameCount];
		interpolationDataCount = list.clips.Sum(f => f.frames.Count(f => f.data.type == FrameInterpolationType.Bezier));
        interpolationDataSize = interpolationDataCount * Marshal.SizeOf<EfxClipInterpolationTangents>();
		if (interpolationData == null || interpolationData.Length != interpolationDataCount) interpolationData = new EfxClipInterpolationTangents[interpolationDataCount];

        clipDuration = 0f;
        int clip_i = 0, frame_i = 0, interpo_i = 0;
        foreach (var clip in list.clips) {
            clips[clip_i++] = new EfxClipHeader() { valueType = clip.valueType, frameCount = clip.frames.Count };
            foreach (var frame in clip.frames) {
                frames[frame_i++] = frame.data;
                if (frame.data.type == FrameInterpolationType.Bezier) {
                    interpolationData[interpo_i++] = frame.tangents;
                }
                if (frame.data.frameTime > clipDuration) {
                    clipDuration = frame.data.frameTime;
                }
            }
        }
	}

	protected void WriteHeader(FileHandler handler)
	{
		handler.Write(ref loopType);
		handler.Write(ref clipDuration);
		clipCount = clips?.Length ?? 0;
		handler.Write(ref clipCount);
		frameCount = frames?.Length ?? 0;
		handler.Write(ref frameCount);
		interpolationDataCount = interpolationData?.Length ?? 0;
		handler.Write(ref interpolationDataCount);
		handler.Write(ref clipDataSize);
		handler.Write(ref frameDataSize);
		handler.Write(ref interpolationDataSize);
	}
	protected void WriteData(FileHandler handler)
	{
		clips ??= new EfxClipHeader[clipCount];
		for (int i = 0; i < clipCount; ++i) {
			handler.Write(clips[i].frameCount);
			handler.Write(clips[i].valueType);
		}
		frames ??= new EfxClipFrame[frameCount];
		handler.WriteArray(frames);
		interpolationData ??= new EfxClipInterpolationTangents[interpolationDataCount];
		handler.WriteArray(interpolationData);
	}

    protected override bool DoRead(FileHandler handler)
    {
		ReadHeader(handler);
		ReadData(handler);
		return true;
    }

    protected override bool DoWrite(FileHandler handler)
    {
		WriteHeader(handler);
		WriteData(handler);
        return true;
    }

	protected static IEnumerable<(string name, Type type)> GetFieldListHeader(EfxVersion Version)
	{
		yield return (nameof(loopType), typeof(EfxClipPlaybackType));
		yield return (nameof(clipDuration), typeof(float));
		yield return (nameof(clipCount), typeof(int));
		yield return (nameof(frameCount), typeof(int));
		yield return (nameof(interpolationDataCount), typeof(int));
		yield return (nameof(clipDataSize), typeof(int));
		yield return (nameof(frameDataSize), typeof(int));
		yield return (nameof(interpolationDataSize), typeof(int));
	}

	protected static IEnumerable<(string name, Type type)> GetFieldListData(EfxVersion Version)
	{
		yield return (nameof(clips), typeof(EfxClipHeader[]));
		yield return (nameof(frames), typeof(EfxClipFrame[]));
		yield return (nameof(interpolationData), typeof(EfxClipInterpolationTangents[]));
	}

    public static IEnumerable<(string name, Type type)> GetFieldList(EfxVersion Version)
    {
		foreach (var f in GetFieldListHeader(Version)) yield return f;
		foreach (var f in GetFieldListData(Version)) yield return f;
	}

    public EfxClipData AssignFromList(EfxClipList src)
    {
        if (clips == null || frames == null) return this;

        this.loopType = src.loopType;
        clips = new EfxClipHeader[src.clips.Count];
        clipCount = clips.Length;
        clipDataSize = clipCount * Marshal.SizeOf<EfxClipHeader>();
        frames = new EfxClipFrame[src.clips.Sum(c => c.frames.Count)];
        frameCount = frames.Length;
        frameDataSize = frames.Length * Marshal.SizeOf<EfxClipFrame>();
        interpolationData = new EfxClipInterpolationTangents[src.clips.Sum(c => c.frames.Count(f => f.data.type == FrameInterpolationType.Bezier))];
        interpolationDataCount = interpolationData.Length;
        interpolationDataSize = interpolationData.Length * Marshal.SizeOf<EfxClipInterpolationTangents>();
        clipDuration = 0;
        int clip_i = 0, frame_i = 0, interpo_i = 0;
        foreach (var clip in src.clips) {
            clips[clip_i++] = new EfxClipHeader() { valueType = clip.valueType, frameCount = clip.frames.Count };
            foreach (var frame in clip.frames) {
                frames[frame_i++] = frame.data;
                if (frame.data.type == FrameInterpolationType.Bezier) {
                    interpolationData[interpo_i++] = frame.tangents;
                }
                if (frame.data.frameTime > clipDuration) {
                    clipDuration = frame.data.frameTime;
                }
            }
        }

        return this;
    }
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class EfxMaterialClipData : EfxClipData
{
    [RszIgnore] public EfxVersion Version;

    public EfxMaterialClipData() { }
    public EfxMaterialClipData(EfxVersion version)
    {
        Version = version;
    }

    public int Length => clipCount;

	[RszVersion(EfxVersion.RE4)]
    [RszArraySizeField(nameof(mdfProperties))] public int mdfPropertyCount;
    [RszVersion(EfxVersion.RE3)]
    [RszArraySizeField(nameof(indices))] public int indicesCount;

    [RszFixedSizeArray(nameof(Version), ">=", EfxVersion.RE4, "?", nameof(mdfPropertyCount), ":", nameof(clipCount))]
    public EfxMaterialClip_Struct4[]? mdfProperties;
    [RszVersion(EfxVersion.RE3)]
    // NOT mdf property index (vaguely similar but inconsistent correlation)
    [RszFixedSizeArray(nameof(indicesCount))] public uint[]? indices;

	protected override bool DoRead(FileHandler handler)
	{
		ReadHeader(handler);
		if (Version >= EfxVersion.RE4) { // end at: mdfPropertyCount
			handler.Read(ref mdfPropertyCount);
		} else {
			mdfPropertyCount = clipCount;
		}

		if (Version >= EfxVersion.RE3) { // end at: indicesCount
			handler.Read(ref indicesCount);
		}

		ReadData(handler);
		mdfProperties = handler.ReadArray<EfxMaterialClip_Struct4>(mdfPropertyCount);
		if (Version >= EfxVersion.RE3) { // end at: indices
			indices = handler.ReadArray<uint>((int)(indicesCount));
		}

		return true;
	}
	protected override bool DoWrite(FileHandler handler)
	{
		WriteHeader(handler);
		if (Version >= EfxVersion.RE4) { // end at: mdfPropertyCount
			mdfPropertyCount = mdfProperties?.Length ?? 0;
			handler.Write(ref mdfPropertyCount);
		} else {
			mdfPropertyCount = clips?.Length ?? 0;
		}

		if (Version >= EfxVersion.RE3) { // end at: indicesCount
			indicesCount = indices?.Length ?? 0;
			handler.Write(ref indicesCount);
		}

		WriteData(handler);
		mdfProperties ??= new EfxMaterialClip_Struct4[mdfPropertyCount];
		handler.WriteArray(mdfProperties);
		if (Version >= EfxVersion.RE3) { // end at: indices
			indices ??= new uint[indicesCount];
			handler.WriteArray(indices);
		}

		return true;
	}

    public override string ToString() => $"({clipCount} clips, {frameCount} frames, props: {mdfPropertyCount}, indices: {indicesCount})";

	public static new IEnumerable<(string name, Type type)> GetFieldList(EfxVersion Version)
	{
		foreach (var f in GetFieldListHeader(Version)) yield return f;
		if (Version >= EfxVersion.RE4) {
			yield return (nameof(mdfPropertyCount), typeof(int));
		}

		if (Version >= EfxVersion.RE3) {
			yield return (nameof(indicesCount), typeof(int));
		}

		foreach (var f in GetFieldListData(Version)) yield return f;
		yield return (nameof(mdfProperties), typeof(EfxMaterialClip_Struct4[]));
		if (Version >= EfxVersion.RE3) {
			yield return (nameof(indices), typeof(uint[]));
		}
	}
}
