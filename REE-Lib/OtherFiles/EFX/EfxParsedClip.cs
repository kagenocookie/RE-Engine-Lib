namespace ReeLib.Efx.Structs.Common;

public class EfxClipList
{
    public EfxClipPlaybackType loopType;
    public readonly List<EfxParsedClip> clips = new();

    public float Duration => clips.Count == 0 ? 0 : clips.Max(c => c.Duration);
    public int FrameCount => clips.Count == 0 ? 0 : clips.Sum(c => c.frames.Count);

    public EfxParsedClip AddClip(int index, int clipBit)
    {
        var lastClip = index < clips.Count ? clips[index] : clips.LastOrDefault();
        var clip = new EfxParsedClip() { clipTargetBit = clipBit, valueType = lastClip?.valueType ?? ClipValueType.Float };
        clip.frames.Add(new EfxParsedClipFrame() { data = new EfxClipFrame() {
            frameTime = 0,
            type = FrameInterpolationType.Type2
        }});

        clips.Insert(index, clip);
        return clip;
    }

    public void RemoveClip(int index)
    {
        clips.RemoveAt(index);
    }
}

public class EfxParsedClip
{
    public int clipTargetBit;
    public ClipValueType valueType;
    public readonly List<EfxParsedClipFrame> frames = new();
    public float Duration => frames.Count == 0 ? 0 : frames.Max(f => f.data.frameTime);
}

public class EfxParsedClipFrame
{
    public EfxClipFrame data;
    public EfxClipInterpolationTangents tangents;

    public override string ToString() => data.ToString();
}
