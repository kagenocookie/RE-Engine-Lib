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
        public uint version;
        public uint magic;
        public long padding;
        public long pointersOffset; // AssetsPointer in Tyrant
        public long motionIndicesOffset;
        public long motListNameOffset;
        public long UnkPadding;
        public int numMots;

        public MotlistVersion Version { get; set; }
        public string MotListName { get; set; } = string.Empty;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref version);
            handler.Read(ref magic);
            handler.Read(ref padding);
            handler.Read(ref pointersOffset);
            handler.Read(ref motionIndicesOffset);
            handler.Read(ref motListNameOffset);
            Version = (MotlistVersion)version;
            if (Version > MotlistVersion.RE7)
            {
                handler.Read(ref UnkPadding);
            }
            handler.Read(ref numMots);
            MotListName = handler.ReadWString(motListNameOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref version);
            handler.Write(ref magic);
            handler.Write(ref padding);
            handler.Write(ref pointersOffset);
            handler.Write(ref motionIndicesOffset);
            handler.WriteOffsetWString(MotListName);
            Version = (MotlistVersion)version;
            if (Version > MotlistVersion.RE7)
            {
                handler.Write(ref UnkPadding);
            }
            handler.Write(ref numMots);
            handler.Skip(2);
            handler.StringTableFlush();
            return true;
        }
    }


    public class MotIndex : BaseModel
    {
        public long motClipOffset;  // may point to MotClip
        public ushort motNumber;
        public ushort Switch;
        public uint[] data;

        public MotFileBase? MotFile { get; set; }
        public MotClip? MotClip { get; set; }

        public MotlistVersion Version { get; set; }

        public int DataCount => Version > MotlistVersion.RE8 ? 15 : Version == MotlistVersion.RE7 ? 0 : 3;

        public MotIndex(MotlistVersion version)
        {
            Version = version;
            data = new uint[DataCount];
        }

        protected override bool DoRead(FileHandler handler)
        {
            if (Version > MotlistVersion.RE7) handler.Read(ref motClipOffset);
            handler.Read(ref motNumber);
            handler.Read(ref Switch);
            int dataCount = DataCount;
            if (dataCount > 0) {
                data = handler.ReadArray<uint>(dataCount);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (Version > MotlistVersion.RE7) handler.Write(ref motClipOffset);
            handler.Write(ref motNumber);
            handler.Write(ref Switch);
            if (data.Length > 0) {
                handler.WriteArray(data);
            }
            return true;
        }

        public override string ToString() => $"[MotID {motNumber}] [Motion: {MotFile?.ToString() ?? "-- "}] {(MotClip == null ? "" : "[ExtraClip]")}";
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

            Dictionary<long, MotFile> motions = new();
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
                    // MotTreeFile mtre = new MotTreeFile(fileHandler);
                    // mtre.Read();
                    throw new NotSupportedException("MotTree motions are not supported");
                }
                // NOTE: MotionFacial also exists, haven't seen it in motlists yet though
            }

            handler.Seek(header.motionIndicesOffset);
            for (int i = 0; i < motOffsets.Length; i++)
            {
                MotIndex motIndex = new(header.Version);
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
                if (motIndex.motClipOffset > 0)
                {
                    handler.Seek(motIndex.motClipOffset);
                    long motClipOffset = handler.Read<long>();
                    handler.Seek(motClipOffset);
                    motIndex.MotClip = new();
                    motIndex.MotClip.Read(handler);
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
                var isFirst = motFileDict.Count == 0;
                var motOffset = handler.Tell();
                motFileDict[mot] = motOffset;
                mot.FileHandler = handler.WithOffset(motOffset);
                mot.Write();
                // "we only need one bone list header per motlist even if some mots use different bones" - capcom dev, apparently
                var skipBoneList = !isFirst && header.Version >= MotlistVersion.RE3;
                if (mot is MotFile motFile && !skipBoneList)
                {
                    motFile.WriteBones();
                    motFile.Header.motSize = 0;
                    motFile.FileHandler.Write(12, 0);
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
                if (motIndex.MotClip != null)
                {
                    motIndex.motClipOffset = handler.Tell();
                    handler.Write(handler.Tell() + 16);
                    handler.Write(0L);
                    motIndex.MotClip.Write(handler);
                    handler.Write(motIndex.Start, motIndex.motClipOffset);
                }
            }

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
