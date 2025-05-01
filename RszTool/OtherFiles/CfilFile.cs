using RszTool.Common;
using System.Collections.ObjectModel;

namespace RszTool
{
    public class CfilFile : BaseFile
    {
        public int maskCount;
        public long uknOffset;

        /// <summary>
        /// cfil.7+
        /// </summary>
        public Guid LayerGuid;
        public Guid[]? MaskGuids;

        /// <summary>
        /// cfil.3
        /// </summary>
        public int layerIndex;
        public int[]? MaskIDs;

        private const int Magic = 0x4c494643;

        public CfilFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var magic = handler.Read<int>();
            if (magic != Magic)
            {
                throw new Exception("Invalid CFIL file");
            }
            var version = handler.FileVersion;
            if (version == 3)
            {
                layerIndex = handler.Read<byte>();
                maskCount = handler.Read<byte>();
                handler.Skip(2);
                MaskIDs = handler.ReadArray<byte>(maskCount).Select(n => (int)n).ToArray();
                return true;
            }
            handler.Read(ref maskCount);
            handler.Skip(8);
            handler.Read(ref LayerGuid);
            handler.Skip(16); // always 0
            var guidListOffset = handler.Read<long>();
            handler.Read(ref uknOffset);
            if (MaskGuids == null || MaskGuids.Length != maskCount)
            {
                MaskGuids = new Guid[maskCount];
            }
            handler.ReadArray(MaskGuids);
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Magic);
            var version = handler.FileVersion;
            if (version == 3)
            {
                maskCount = MaskIDs?.Length ?? 0;
                handler.Write((byte)layerIndex);
                handler.Write((byte)maskCount);
                handler.Skip(2);
                if (maskCount == 0) {
                    handler.Write<int>(-1);
                } else {
                    foreach (var mask in MaskIDs!) handler.Write((byte)mask);
                    handler.Align(4);
                }
                // there's another 16 bytes after but they're all 0
                // either an empty guid or mask IDs is a fixed 20 size array
                handler.Write(0L);
                handler.Write(0L);
                return true;
            }

            maskCount = MaskGuids?.Length ?? 0;
            handler.Write(ref maskCount);
            handler.Skip(8);
            handler.Write(ref LayerGuid);
            handler.Skip(16); // always 0
            // since there's only one cfil header structure and everything else seems to just be 0, guids always start at 64
            var guidListOffset = 64L;
            handler.Write(ref guidListOffset);
            handler.Write(guidListOffset + maskCount * sizeof(long) * 2);
            handler.WriteArray(MaskGuids ?? Array.Empty<Guid>());
            return true;
        }
    }
}