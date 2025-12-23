using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using ReeLib.Clip;
using ReeLib.Common;
using ReeLib.InternalAttributes;
using ReeLib.Mot;
using ReeLib.Motlist;

namespace ReeLib.Motcam
{
    public enum MotcamVersion
    {
        RE7 = 4,
        RE2_DMC5 = 7,
        RE_RT = 9,
        SF6 = 12,
        Pragmata = 13,
    }

    public static class McamlistExtensions
    {
        public static MotVersion GetMotVersion(this MotcamVersion Mcamlist) => Mcamlist switch {
            < MotcamVersion.RE2_DMC5 => MotVersion.RE7,
            MotcamVersion.RE2_DMC5 => MotVersion.RE2_DMC5,
            MotcamVersion.RE_RT => MotVersion.RE_RT,
            MotcamVersion.SF6 => MotVersion.SF6,
            MotcamVersion.Pragmata => MotVersion.Pragmata,
            _ => MotVersion.Pragmata,
        };
    }

    public class MotcamHeader : ReadWriteModel
    {
        public MotcamVersion version;

        internal uint magic = MotcamFile.Magic;
        internal long dataOffset1;
        internal long dataOffset2;

        public ushort frameRate;
        public float frameCount;
        public float blending = -1;
        public float uknFloat = -1;
        public ushort uknExtra;

        public string motName = string.Empty;

        protected override sealed bool ReadWrite<THandler>(THandler action)
        {
            action.Then(ref version)
                 !.Then(ref magic)
                 !.Null(8)
                 !.Then(ref dataOffset1)
                 !.Then(ref dataOffset2)
                 !.Null(16);
            if (version == MotcamVersion.RE7)
            {
                action.Null(8);
            }
            else
            {
                action.HandleOffsetWString(ref motName);
            }
            action.Then(ref uknExtra)
                 !.Then(ref frameRate)
                 !.Then(ref frameCount)
                 !.Then(ref blending)
                 !.Then(version >= MotcamVersion.RE_RT, ref uknFloat);

            if (version == MotcamVersion.RE7)
            {
                action.HandleInlineWString(ref motName);
            }

            return true;
        }

        public void CopyValuesFrom(MotcamHeader source)
        {
            version = source.version;
            frameRate = source.frameRate;
            frameCount = source.frameCount;
            uknFloat = source.uknFloat;
            blending = source.blending;
            uknExtra = source.uknExtra;
            if (string.IsNullOrEmpty(motName)) {
                motName = source.motName;
            }
        }
    }

    public class MotcamClip : BaseModel
    {
        public short boneIndex;
        public Mot.TrackFlag trackFlags;
        public byte uknIndex;
        public uint boneHash;
        public float uknFloat = 1f;

        public Mot.Track? Translation { get; set; }
        public Mot.Track? Rotation { get; set; }

        public bool HasTranslation => Translation != null && (trackFlags & Mot.TrackFlag.Translation) != 0;
        public bool HasRotation => Rotation != null && (trackFlags & Mot.TrackFlag.Rotation) != 0;

        public MotcamVersion Version;

        public MotcamClip(MotcamVersion version)
        {
            Version = version;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref boneIndex);
            handler.Read(ref trackFlags);
            handler.Read(ref uknIndex);
            handler.Read(ref boneHash);
            uint translationStartOffset;
            if (Version == MotcamVersion.RE2_DMC5)
            {
                handler.Read(ref uknFloat);
                handler.ReadNull(4);
                translationStartOffset = handler.Read<uint>();
                handler.ReadNull(8);
            }
            else
            {
                translationStartOffset = handler.Read<uint>();
                if (Version == MotcamVersion.RE7) handler.ReadNull(4);
            }

            handler.Seek(translationStartOffset);
            if (trackFlags.HasFlag(Mot.TrackFlag.Translation))
            {
                Translation = new(Version.GetMotVersion(), Mot.TrackValueType.Vector3);
                Translation.Read(handler);
            }
            if (trackFlags.HasFlag(Mot.TrackFlag.Rotation))
            {
                Rotation = new(Version.GetMotVersion(), Mot.TrackValueType.Quaternion);
                Rotation.Read(handler);
            }
            Translation?.ReadFrameDataTranslation(handler);
            Rotation?.ReadFrameDataRotation(handler);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref boneIndex);
            handler.Write(ref trackFlags);
            handler.Write(ref uknIndex);
            handler.Write(ref boneHash);

            long offsetStart;
            if (Version == MotcamVersion.RE2_DMC5)
            {
                handler.Write(ref uknFloat);
                handler.WriteNull(4);
                offsetStart = handler.Tell();
                handler.Skip(4); // translationStartOffset
                handler.WriteNull(8);
            }
            else
            {
                offsetStart = handler.Tell();
                handler.Skip(4); // translationStartOffset
                if (Version == MotcamVersion.RE7) handler.WriteNull(4);
            }

            handler.Write(offsetStart, (uint)(handler.Tell())); // write translationStartOffset
            Translation?.Write(handler);
            Rotation?.Write(handler);
            // update the clip header with our newly found offset
            return true;
        }

        public void WriteOffsetContents(FileHandler handler)
        {
            Translation?.WriteOffsetContents(handler);
            Rotation?.WriteOffsetContents(handler);
        }

        public void ChangeVersion(MotcamVersion version)
        {
            Version = version;
            Translation?.ChangeVersion(version.GetMotVersion());
            Rotation?.ChangeVersion(version.GetMotVersion());
        }

        public override string ToString() => $"[{(Translation == null ? "": "T")}{(Rotation == null ? "": "R")}]";
    }
}


namespace ReeLib
{
    using ReeLib.Motcam;

    public class MotcamFile(FileHandler fileHandler)
        : MotFileBase(fileHandler)
    {
        public const uint Magic = 0x6D61636D;

        public MotcamHeader Header { get; } = new();
        public MotcamClip? Clip1 { get; set; }
        public MotcamClip? Clip2 { get; set; }

        public override string Name { get => Header.motName; set => Header.motName = value; }

        public override KnownFileFormats MotType => KnownFileFormats.Motion;

        protected override bool DoRead()
        {
            Clip1 = null;
            Clip2 = null;
            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a MOTCAM file");
            }

            if (header.dataOffset1 > 0)
            {
                handler.Seek(header.dataOffset1);
                Clip1 = new MotcamClip(header.version);
                Clip1.Read(handler);
            }

            if (header.dataOffset2 > 0)
            {
                handler.Seek(header.dataOffset2);
                Clip2 = new MotcamClip(header.version);
                Clip2.Read(handler);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            FileHandler handler = FileHandler;
            var header = Header;
            header.Write(handler);
            handler.StringTableFlush();

            handler.Align(16);
            header.dataOffset1 = handler.Tell();
            Clip1?.Write(handler);
            Clip1?.WriteOffsetContents(handler);

            handler.Align(16);
            header.dataOffset2 = handler.Tell();
            Clip2?.Write(handler);
            Clip2?.WriteOffsetContents(handler);

            header.Write(handler, 0);
            return true;
        }

        public override string ToString() => $"{Header.motName}";

        public void ChangeVersion(MotcamVersion version)
        {
            Clip1?.ChangeVersion(version);
            Clip2?.ChangeVersion(version);
        }

        public void CopyValuesFrom(MotFile source, bool replaceBehaviorClips)
        {
            ChangeVersion(Header.version);
        }
    }
}
