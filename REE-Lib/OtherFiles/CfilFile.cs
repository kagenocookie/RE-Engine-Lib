using ReeLib.Common;

namespace ReeLib
{
    public class CfilFile : BaseFile
    {
        public Guid LayerGuid;
        public Guid[] MaskGuids = [];

        public const int Magic = 0x4c494643;

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
                var layerIndex = handler.Read<byte>();
                LayerGuid = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, layerIndex);

                var maskCountRE7 = handler.Read<byte>();
                handler.Skip(2);
                MaskGuids = handler.ReadArray<byte>(maskCountRE7).Select(n => new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, n)).ToArray();
                return true;
            }

            var maskCount = handler.Read<int>();
            handler.ReadNull(8);
            handler.Read(ref LayerGuid);
            handler.ReadNull(16);
            var guidListOffset = handler.Read<long>();
            var endOffset = handler.Read<long>();
            DataInterpretationException.DebugThrowIf(endOffset != handler.FileSize());
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
                var maskCountRE7 = MaskGuids.Length;

                var bytes = MemoryUtils.StructureAsBytes(ref LayerGuid);
                handler.Write(bytes[^1]);
                handler.Write((byte)maskCountRE7);
                handler.Skip(2);
                if (maskCountRE7 == 0) {
                    handler.Write<int>(-1);
                } else {
                    foreach (var mask in MaskGuids) {
                        var maskTmp = mask;
                        var tmp = MemoryUtils.StructureAsBytes(ref maskTmp);
                        handler.Write(tmp[^1]);
                    }
                    handler.Align(4);
                }
                // there's another 16 bytes after but they're all 0
                // either an empty guid or mask IDs is a fixed 20 size array
                handler.Write(0L);
                handler.Write(0L);
                return true;
            }

            var maskCount = MaskGuids?.Length ?? 0;
            handler.Write(ref maskCount);
            handler.WriteNull(8);
            handler.Write(ref LayerGuid);
            handler.WriteNull(16);

            var offsetsStart = handler.Tell();
            handler.Skip(16);

            handler.Write(offsetsStart, handler.Tell());
            handler.WriteArray(MaskGuids ?? Array.Empty<Guid>());

            handler.Write(offsetsStart + 8, handler.Tell());

            return true;
        }
    }
}