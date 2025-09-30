using ReeLib.Clip;
using ReeLib.Common;
using ReeLib.Mot;
using ReeLib.Motlist;

namespace ReeLib.Motlist
{
    public enum MotlistVersion
    {
        RE7 = 60,
        RE2_DMC5 = 85,
        RE3 = 99,
        MHR_DEMO = 484,
        RE8 = 486,
        RE2_RT = 524,
        RE3_RT = RE2_RT,
        RE7_RT = RE2_RT,
        MHR = 528,
        SF6 = 653,
        RE4 = 663,
        DD2 = 751,
        MHWILDS = 992,
    }

    public static class MotlistExtensions
    {
        public static MotVersion GetMotVersion(this MotlistVersion motlist) => motlist switch {
            MotlistVersion.RE7 => MotVersion.RE7,
            MotlistVersion.RE2_DMC5 => MotVersion.RE2_DMC5,
            MotlistVersion.RE3 => MotVersion.RE3,
            MotlistVersion.MHR_DEMO => MotVersion.MHR_DEMO,
            MotlistVersion.RE8 => MotVersion.RE8,
            MotlistVersion.RE2_RT => MotVersion.RE2_RT,
            MotlistVersion.MHR => MotVersion.MHR,
            MotlistVersion.SF6 => MotVersion.SF6,
            MotlistVersion.RE4 => MotVersion.RE4,
            MotlistVersion.DD2 => MotVersion.DD2,
            MotlistVersion.MHWILDS => MotVersion.MHWILDS,
            _ => MotVersion.MHWILDS,
        };

        public static ClipVersion GetClipVersion(this MotVersion ver) => ver switch {
            MotVersion.RE7 => ClipVersion.RE7,
            MotVersion.RE2_DMC5 => ClipVersion.RE2_DMC5,
            MotVersion.RE3 => ClipVersion.RE3,
            MotVersion.MHR_DEMO => ClipVersion.MHR_DEMO,
            MotVersion.RE8 => ClipVersion.RE8,
            MotVersion.RE2_RT => ClipVersion.RE2_RT,
            MotVersion.MHR => ClipVersion.MHR,
            MotVersion.SF6 => ClipVersion.SF6,
            MotVersion.RE4 => ClipVersion.RE4,
            MotVersion.DD2 => ClipVersion.DD2,
            MotVersion.MHWILDS => ClipVersion.MHWilds,
            _ => ClipVersion.MHWilds,
        };
    }

    public class Header : BaseModel
    {
        public MotlistVersion version;
        public uint magic = MotlistFile.Magic;
        public int uknValue; // dmc5
        public long pointersOffset;
        public long motionIndicesOffset;
        public int numMots;

        public string MotListName { get; set; } = string.Empty;
        public string? BaseMotListPath { get; set; }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref version);
            handler.Read(ref magic);
            handler.Read(ref uknValue);
            handler.ReadNull(4);
            handler.Read(ref pointersOffset);
            handler.Read(ref motionIndicesOffset);
            MotListName = handler.ReadOffsetWString();
            if (version > MotlistVersion.RE7)
            {
                var offset = handler.Read<long>();
                BaseMotListPath = offset > 0 ? handler.ReadWString(offset) : null;
            }
            handler.Read(ref numMots);
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
            handler.WriteOffsetWString(MotListName);
            if (version > MotlistVersion.RE7)
            {
                if (!string.IsNullOrEmpty(BaseMotListPath))
                {
                    handler.WriteOffsetWString(BaseMotListPath);
                }
                else
                {
                    handler.WriteNull(8);
                }
            }
            handler.Write(ref numMots);
            // handler.Skip(2);
            handler.StringTableFlush();
            return true;
        }
    }


    public class MotIndex : BaseModel
    {
        public long motClipOffset;  // may point to MotClip
        public ushort motNumber;
        public ushort Switch;

        public uint uknData1;
        public uint uknData2;
        public byte uknCount1 = 1; // seems to be 1 for most of the files, setting it as default for now
        public byte ukn2;
        public byte ukn3;
        public byte extraClipCount;
        public uint[] data;

        public MotFileBase? MotFile { get; set; }
        public List<MotClip> MotClips { get; set; } = new (0);

        public MotlistVersion Version { get; set; }

        private int UnknownDataCount => Version > MotlistVersion.RE8 ? 12 : Version == MotlistVersion.RE7 ? 0 : 3;

        public MotIndex(MotlistVersion version)
        {
            Version = version;
            data = new uint[UnknownDataCount];
        }

        protected override bool DoRead(FileHandler handler)
        {
            if (Version > MotlistVersion.RE7) handler.Read(ref motClipOffset);
            handler.Read(ref motNumber);
            handler.Read(ref Switch);
            if (Version > MotlistVersion.RE8)
            {
                handler.Read(ref uknData1);
                handler.Read(ref uknData2);
                handler.Read(ref uknCount1);
                handler.Read(ref ukn2);
                handler.Read(ref ukn3);
                handler.Read(ref extraClipCount);
                data = handler.ReadArray<uint>(UnknownDataCount);
            }
            else if (Version > MotlistVersion.RE7)
            {
                data = handler.ReadArray<uint>(UnknownDataCount);
                extraClipCount = (byte)(motClipOffset > 0 ? 1 : 0);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (Version > MotlistVersion.RE7) handler.Write(ref motClipOffset);
            handler.Write(ref motNumber);
            handler.Write(ref Switch);
            if (Version > MotlistVersion.RE8)
            {
                handler.Write(ref uknData1);
                handler.Write(ref uknData2);
                handler.Write(ref uknCount1);
                handler.Write(ref ukn2);
                handler.Write(ref ukn3);
                handler.Write(ref extraClipCount);
                handler.WriteArray(data);
            }
            else if (Version > MotlistVersion.RE7)
            {
                handler.WriteArray(data);
            }
            return true;
        }

        public override string ToString() => $"[MotID {motNumber}] [Motion: {MotFile?.ToString() ?? "-- "}] {(MotClips.Count == 0 ? "" : $"[ExtraClips: {MotClips.Count}]")}";
    }
}


namespace ReeLib
{
    public class MotlistFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public const uint Magic = 0x74736C6D;
        public const string Extension = ".motlist";

        public Header Header { get; } = new();
        public List<MotFileBase> MotFiles { get; } = new();
        public List<MotIndex> Motions { get; } = new();

        protected override bool DoRead()
        {
            MotFiles.Clear();
            Motions.Clear();
            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a motlist file");
            }
            handler.Seek(header.pointersOffset);
            long[] motOffsets = handler.ReadArray<long>(header.numMots);

            Dictionary<long, MotFileBase> motions = new();
            MotFile? headerMot = null;
            for (int i = 0; i < motOffsets.Length; i++)
            {
                if (motOffsets[i] == 0)
                {
                    continue;
                }
                if (motions.ContainsKey(motOffsets[i])) continue;

                var fileHandler = handler.WithOffset(motOffsets[i]);
                var magic = fileHandler.ReadInt(4);
                if (magic == MotFile.Magic) {
                    MotFile motFile = new(fileHandler);
                    motFile.Embedded = true;
                    motFile.Read();
                    motFile.ReadBones(headerMot);
                    headerMot ??= motFile;
                    MotFiles.Add(motFile);
                    motions[motOffsets[i]] = motFile;
                } else if (magic == MotTreeFile.Magic) {
                    MotTreeFile tree = new MotTreeFile(fileHandler);
                    tree.Embedded = true;
                    tree.Read();
                    MotFiles.Add(tree);
                    motions[motOffsets[i]] = tree;
                }
                // NOTE: MotionFacial also exists, haven't seen it in motlists yet though
            }

            handler.Seek(header.motionIndicesOffset);
            for (int i = 0; i < motOffsets.Length; i++)
            {
                MotIndex motIndex = new(header.version);
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
                if (motIndex.motClipOffset > 0 && motIndex.extraClipCount > 0)
                {
                    handler.Seek(motIndex.motClipOffset);
                    var headerOffsets = handler.ReadArray<long>(motIndex.extraClipCount);
                    foreach (var off in headerOffsets) {
                        handler.Seek(off);
                        var clip = new MotClip();
                        clip.Read(handler);
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
            var foundMotFile = false;
            foreach (var mot in MotFiles)
            {
                var motOffset = handler.Tell();
                motFileDict[mot] = motOffset;
                mot.FileHandler = handler.WithOffset(motOffset);
                mot.Write();
                // "we only need one bone list header per motlist even if some mots use different bones" - capcom dev, apparently
                var skipBoneList = header.version >= MotlistVersion.RE3 && foundMotFile;
                if (mot is MotFile motFile && !skipBoneList)
                {
                    motFile.WriteBones();
                    motFile.Header.motSize = 0;
                    motFile.FileHandler.Write(12, 0);
                    foundMotFile = true;
                }
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
                        motIndex.MotClips[i].Write(handler);
                    }

                    handler.Write(motIndex.Start, motIndex.motClipOffset);
                    motIndex.Write(handler, motIndex.Start);
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
                Log.Error($"Mot {replacedFile} not found in motlist");
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
