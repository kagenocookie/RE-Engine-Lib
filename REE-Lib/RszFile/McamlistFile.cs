using ReeLib.Clip;
using ReeLib.Common;
using ReeLib.Mot;
using ReeLib.Motcam;
using ReeLib.Motlist;
using ReeLib.MotTree;

namespace ReeLib.Mcamlist
{
    public enum McamlistVersion
    {
        RE7 = 7,
        RE2 = 13,
        RE3 = 14,
        RE_RT = 19,
        SF6 = 22,
        Pragmata = 24,
    }

    public static class McamlistExtensions
    {
        public static MotlistVersion GetMotlistVersion(this McamlistVersion Mcamlist) => Mcamlist switch {
            McamlistVersion.RE7 => MotlistVersion.RE7,
            McamlistVersion.RE2 => MotlistVersion.RE2_DMC5,
            McamlistVersion.RE_RT => MotlistVersion.RE_RT,
            McamlistVersion.SF6 => MotlistVersion.SF6,
            McamlistVersion.Pragmata => MotlistVersion.Pragmata,
            _ => MotlistVersion.Pragmata,
        };

        public static MotcamVersion GetMotcamVersion(this McamlistVersion Mcamlist) => Mcamlist switch {
            McamlistVersion.RE7 => MotcamVersion.RE7,
            McamlistVersion.RE2 => MotcamVersion.RE2_DMC5,
            McamlistVersion.RE_RT => MotcamVersion.RE_RT,
            McamlistVersion.SF6 => MotcamVersion.SF6,
            McamlistVersion.Pragmata => MotcamVersion.Pragmata,
            _ => MotcamVersion.Pragmata,
        };
    }

    public class Header : BaseModel
    {
        public McamlistVersion version;
        public uint magic = McamlistFile.Magic;
        public int uknValue; // dmc5
        public long pointersOffset;
        public long motionIndicesOffset;
        public long uknOffset;
        public int numMots;
        public short uknNum;

        public string Name { get; set; } = string.Empty;
        public string? BaseMcamlistPath { get; set; }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref version);
            handler.Read(ref magic);
            handler.Read(ref uknValue);
            handler.ReadNull(4);
            handler.Read(ref pointersOffset);
            handler.Read(ref motionIndicesOffset);
            Name = handler.ReadOffsetWString();
            if (version > McamlistVersion.RE7)
            {
                var offset = handler.Read<long>();
                BaseMcamlistPath = offset > 0 ? handler.ReadWString(offset) : null;
            }
            if (version >= McamlistVersion.Pragmata)
            {
                handler.Read(ref uknOffset);
                DataInterpretationException.DebugWarnIf(uknOffset > 0);
            }
            handler.Read(ref numMots);
            if (version >= McamlistVersion.RE_RT)
            {
                handler.Read(ref uknNum);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref version);
            handler.Write(ref magic);
            handler.Write(ref uknValue);
            handler.WriteNull(4);
            handler.Write(ref pointersOffset);
            handler.Write(ref motionIndicesOffset);
            handler.WriteOffsetWString(Name);
            if (version > McamlistVersion.RE7)
            {
                if (!string.IsNullOrEmpty(BaseMcamlistPath))
                {
                    handler.WriteOffsetWString(BaseMcamlistPath);
                }
                else
                {
                    handler.WriteNull(8);
                }
            }
            if (version >= McamlistVersion.Pragmata)
            {
                handler.Write(ref uknOffset);
            }
            handler.Write(ref numMots);
            if (version >= McamlistVersion.RE_RT)
            {
                handler.Write(ref uknNum);
            }
            handler.StringTableFlush();
            return true;
        }
    }

    public class McamIndex : MotIndex
    {
        public McamIndex(McamlistVersion version) : base (version.GetMotlistVersion())
        {
        }

        public override string ToString() => $"[MotCamID {motNumber}] [Motion: {MotFile?.ToString() ?? "-- "}]";
    }
}


namespace ReeLib
{
    using ReeLib.Mcamlist;

    public class McamlistFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public const uint Magic = 0x74736C63;

        public Header Header { get; } = new();
        public List<MotFileBase> MotFiles { get; } = new();
        public List<McamIndex> Motions { get; } = new();

        public MotFileBase? Find(string motName)
        {
            foreach (var mot in MotFiles)
            {
                if (mot.Name == motName) return mot;
            }
            return null;
        }

        public string[] FindDanglingMotFiles()
        {
            var set = MotFiles.ToHashSet();
            foreach (var mot in Motions)
            {
                if (mot.MotFile != null) set.Remove(mot.MotFile);
            }

            return set.Select(m => m.Name).ToArray();
        }

        protected override bool DoRead()
        {
            MotFiles.Clear();
            Motions.Clear();
            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a Mcamlist file");
            }
            handler.Seek(header.pointersOffset);
            long[] motOffsets = handler.ReadArray<long>(header.numMots);

            Dictionary<long, MotFileBase> motions = new();
            for (int i = 0; i < motOffsets.Length; i++)
            {
                if (motOffsets[i] == 0)
                {
                    continue;
                }
                if (motions.ContainsKey(motOffsets[i])) continue;

                var fileHandler = handler.WithOffset(motOffsets[i]);
                var magic = fileHandler.ReadInt(4);
                if (magic == MotcamFile.Magic) {
                    MotcamFile motFile = new(fileHandler);
                    motFile.Embedded = true;
                    motFile.Read();
                    MotFiles.Add(motFile);
                    motions[motOffsets[i]] = motFile;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            handler.Seek(header.motionIndicesOffset);
            for (int i = 0; i < motOffsets.Length; i++)
            {
                McamIndex motIndex = new(header.version);
                motIndex.Read(handler);
                Motions.Add(motIndex);
                var motOffset = motOffsets[i];
                if (motOffset != 0)
                {
                    motIndex.MotFile = motions[motOffset];
                }
            }

            foreach (var motIndex in Motions)
            {
                DataInterpretationException.DebugThrowIf(motIndex.extraClipCount > 0);
                if (motIndex.motClipOffset > 0 && motIndex.extraClipCount > 0)
                {
                    handler.Seek(motIndex.motClipOffset);
                    var headerOffsets = handler.ReadArray<long>(motIndex.extraClipCount);
                    foreach (var off in headerOffsets)
                    {
                        handler.Seek(off);
                        var motclipHandler = handler;
                        if (Header.version >= McamlistVersion.Pragmata)
                        {
                            motclipHandler = handler.WithOffset(handler.Tell());
                        }
                        var clip = new MotClip();
                        clip.Read(motclipHandler);
                        motIndex.MotClips.Add(clip);
                    }
                }
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var header = Header;
            header.Write(handler);
            handler.Align(16);
            header.pointersOffset = handler.Tell();
            header.numMots = Motions.Count;
            handler.Skip(header.numMots * sizeof(long));
            handler.Align(16);

            var motFileDict = new Dictionary<MotFileBase, long>();
            foreach (var mot in MotFiles)
            {
                var motOffset = handler.Tell();
                motFileDict[mot] = motOffset;
                mot.FileHandler = handler.WithOffset(motOffset);
                mot.Write();
                handler.Align(16);
            }

            handler.Write(24, header.motionIndicesOffset = handler.Tell());
            for (int i = 0; i < Motions.Count; i++)
            {
                var motion = Motions[i];
                if (motion.MotFile == null)
                {
                    handler.Write(header.pointersOffset + i * sizeof(long), 0L);
                    motion.Write(handler);
                    continue;
                }

                if (motFileDict.TryGetValue(motion.MotFile, out var motOffset))
                {
                    handler.Write(header.pointersOffset + i * sizeof(long), motOffset);
                }
                else
                {
                    throw new Exception($"Mot file for motion ID {motion.motNumber} not found");
                }

                motion.Write(handler);
            }

            handler.Align(16);

            foreach (var motIndex in Motions)
            {
                if (motIndex.MotClips.Count != 0)
                {
                    handler.Align(8);
                    motIndex.extraClipCount = (byte)motIndex.MotClips.Count;
                    motIndex.motClipOffset = handler.Tell();
                    handler.Skip(motIndex.extraClipCount * 8);
                    handler.Align(16);
                    for (int i = 0; i < motIndex.MotClips.Count; ++i)
                    {
                        handler.Write(motIndex.motClipOffset + i * 8, handler.Tell());
                        var motclipHandler = handler;
                        if (header.version >= McamlistVersion.Pragmata)
                        {
                            motclipHandler = handler.WithOffset(handler.Tell());
                        }
                        motIndex.MotClips[i].Write(motclipHandler);
                    }

                    handler.Write(motIndex.Start, motIndex.motClipOffset);
                    motIndex.Rewrite(handler);
                }
            }
            header.Write(handler, 0);

            return true;
        }

        public bool ReplaceMotFile(MotFileBase replacedFile, MotFileBase newFile)
        {
            var motIndex = MotFiles.IndexOf(replacedFile);
            if (motIndex == -1)
            {
                Log.Error($"Mot {replacedFile} not found in Mcamlist");
                return false;
            }

            MotFiles[motIndex] = newFile;
            foreach (var mots in Motions) {
                if (mots.MotFile == replacedFile) {
                    mots.MotFile = newFile;
                }
            }
            return true;
        }
    }
}
