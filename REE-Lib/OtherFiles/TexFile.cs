using ReeLib.DDS;

namespace ReeLib.Tex
{
	public enum TexSerializerVersion
	{
		Unknown,
		RE7_8,
        MHRise_28,
        MHWilds,
	}

    public partial class Header : ReadWriteModel
    {
        public uint magic = TexFile.Magic;
        public int version;
        public short width;
        public short height;
        public short depth;
        public byte imageCount;
        public byte mipHeaderSize;
        public byte mipCount;

        public DxgiFormat format;
        public int swizzleControl = -1;
        public uint cubemapMarker;
        public TexFlags flags;

        public byte swizzleHeightDepth;
        public byte swizzleWidth;
        public ushort null1;
        public ushort seven;
        public ushort one;

        public int BitsPerPixel => format.GetBitsPerPixel();

        internal const int MipHeaderSize = 16;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref magic);
            action.Do(ref version);
            action.Do(ref width);
            action.Do(ref height);
            action.Do(ref depth);
            var Version = TexFile.GetSerializerVersion(version, action.Handler.FileVersion);
            if (Version >= TexSerializerVersion.MHRise_28) {
                // TODO some tex have 0 imageCount but still contain data (re4 debugsplatindex.tex.143221013) and mip header size 17
                action.Do(ref imageCount);
                action.Do(ref mipHeaderSize);
                mipCount = (byte)(mipHeaderSize / MipHeaderSize);
            } else {
                action.Do(ref mipCount);
                action.Do(ref imageCount);
            }
            action.Do(ref format);
            action.Do(ref swizzleControl);
            action.Do(ref cubemapMarker);
            action.Do(ref flags);
            if (Version >= TexSerializerVersion.MHRise_28) {
                action.Do(ref swizzleHeightDepth);
                action.Do(ref swizzleWidth);
                action.Do(ref null1);
                action.Do(ref seven);
                action.Do(ref one);
            }

            return true;
        }
    }

    public struct MipHeader
    {
        public long offset;
        public int pitch;
        public int size;
    }

    [Flags]
    public enum TexFlags
    {
        IsStreaming = 1,
        Ukn7 = (1 << 7),
        Ukn8 = (1 << 8),
        Ukn9 = (1 << 9),
        Ukn10 = (1 << 10),
        Ukn14 = (1 << 14),
    }
}

namespace ReeLib
{
    using System.Buffers;
    using ReeLib.Common;
    using ReeLib.Tex;

    public class TexFile : BaseFile
    {
        public const uint Magic = 0x00584554;

        public Header Header = new();
        public List<MipHeader> Mips { get; } = new();

        internal int TotalTextureDataLength => Mips.Count == 0 ? 0 : (int)(Mips[^1].offset + Mips[^1].size - Mips[0].offset);

        public TexSerializerVersion CurrentSerializerVersion =>
            VersionHashLookups.TryGetValue(FileHandler.FileVersion, out var config) || VersionHashLookups.TryGetValue(Header.version, out config)
            ? config.serializerVersion
			: TexSerializerVersion.Unknown;

        public string? CurrentVersionConfig =>
            VersionHashLookups.TryGetValue(FileHandler.FileVersion, out var config) || VersionHashLookups.TryGetValue(Header.version, out config)
            ? Versions.FirstOrDefault(kv => kv.Value == config).Key
			: null;

		private readonly record struct TexVersionConfig(int fileVersion, TexSerializerVersion serializerVersion, GameName[] games);

        private static readonly Dictionary<string, TexVersionConfig> Versions = new()
		{
			{ "RE7", new (8, TexSerializerVersion.RE7_8, [GameName.re7]) },
			{ "RE2", new (10, TexSerializerVersion.RE7_8, [GameName.re2]) },
			{ "DMC5", new (11, TexSerializerVersion.RE7_8, [GameName.dmc5]) },
			{ "RE3", new (190820018, TexSerializerVersion.RE7_8, [GameName.re3]) },

			{ "MHRISE", new (28, TexSerializerVersion.MHRise_28, [GameName.mhrise]) },
			{ "RE8", new (30, TexSerializerVersion.MHRise_28, [GameName.re8]) },
			{ "RE2/3 RT", new (34, TexSerializerVersion.MHRise_28, [GameName.re2rt, GameName.re3rt]) },
			{ "RE7RT", new (35, TexSerializerVersion.MHRise_28, [GameName.re7rt]) },

			{ "RE4 / SF6", new (143221013, TexSerializerVersion.MHRise_28, [GameName.re4, GameName.sf6]) },

			{ "DD2", new (760230703, TexSerializerVersion.MHRise_28, [GameName.dd2]) },

			{ "DR", new (240606151, TexSerializerVersion.MHRise_28, [GameName.drdr]) },
			{ "MHWILDS", new (241106027, TexSerializerVersion.MHWilds, [GameName.mhwilds]) }, // TODO support tex gdeflate
		};

		public static readonly string[] AllVersionConfigs = Versions.OrderBy(kv => kv.Value.serializerVersion).Select(kv => kv.Key).ToArray();
		public static readonly string[] AllVersionConfigsWithExtension = Versions.OrderBy(kv => kv.Value.serializerVersion).Select(kv => $"{kv.Key} (.tex.{kv.Value.fileVersion})").ToArray();

		private static readonly Dictionary<GameName, string[]> versionsPerGame = Enum.GetValues<GameName>().ToDictionary(
			game => game,
			game => Versions.Where(kv => kv.Value.games.Contains(game)).Select(pair => pair.Key).ToArray()
		);

		private static readonly Dictionary<int, TexVersionConfig> VersionHashLookups = Versions.ToDictionary(v => v.Value.fileVersion, kv => kv.Value);

		internal static TexSerializerVersion GetSerializerVersion(int internalVersion, int fileVersion)
			// on match failure, assume latest format for anything unknown - in case of newer games
			=> VersionHashLookups.TryGetValue(internalVersion, out var vvv) ? vvv.serializerVersion :
                VersionHashLookups.TryGetValue(fileVersion, out vvv) ? vvv.serializerVersion : TexSerializerVersion.MHWilds;

		public static string[] GetGameVersionConfigs(GameName game) => versionsPerGame.GetValueOrDefault(game) ?? AllVersionConfigs;
		public static TexSerializerVersion GetPrimarySerializerVersion(GameName game) => Versions[GetGameVersionConfigs(game)[0]].serializerVersion;
		public static TexSerializerVersion GetSerializerVersion(string exportConfig) => Versions.TryGetValue(exportConfig, out var cfg) ? cfg.serializerVersion : TexSerializerVersion.Unknown;

        public static int GetFileExtension(string exportConfig) => Versions.TryGetValue(exportConfig, out var cfg) ? cfg.fileVersion : 0;

        public TexFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        public void ChangeVersion(string versionConfig)
        {
			if (!Versions.TryGetValue(versionConfig, out var config)) {
				Log.Error("Unknown mesh version config " + versionConfig);
				return;
			}

            var headerSize = Header.Size;
			Header.version = config.fileVersion;
            var handler = FileHandler;
            Header.Write(handler, 0);
            if (headerSize != 0 && headerSize != Header.Size && Mips.Count > 0)
            {
                // relocate mip offsets and data
                var mipOffsetsDelta = Header.Size - headerSize;
                handler.Seek(Mips[0].offset);
                var originalData = handler.ReadArray<byte>(TotalTextureDataLength);

                for (int i = 0; i < Mips.Count; i++)
                {
                    var mip = Mips[i];
                    mip.offset -= mipOffsetsDelta;
                    Mips[i] = mip;
                }

                handler.Seek(Mips[0].offset);
                Mips.Write(handler);
                handler.WriteArray(originalData);
            }
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);
            if (Header.depth > 1) {
                throw new NotSupportedException("Depth > 1 textures not supported");
            }

            Mips.Clear();
            for (int i = 0; i < Header.mipCount * Header.imageCount; ++i) {
                var mip = new MipHeader();
                handler.Read(ref mip);
                Mips.Add(mip);
            }

            var serializerVersion = CurrentSerializerVersion;
            if (serializerVersion >= TexSerializerVersion.MHWilds && Mips.Count > 0)
            {
                handler.Seek(Mips[0].offset);
                var compressedSize = (int)(handler.FileSize() - handler.Tell());
                var decompressedSize = TotalTextureDataLength;
                var compressedBytes = ArrayPool<byte>.Shared.Rent(compressedSize);
                handler.ReadArray(compressedBytes);
                handler.Seek(Mips[0].offset);
                // TODO gdeflate contents and write back
                // using var gdeflateStream = GDeflate.Decompress(compressedBytes, compressedSize);
                // gdeflateStream.CopyTo(handler.Stream);

                ArrayPool<byte>.Shared.Return(compressedBytes);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            Header.Write(handler);

            // NOTE: the tex data in the stream should be up to date if the proper manipulation methods were used, write header only

            // we're unable to write compressed data through this because the tex file is intended to be decompressed while in memory
            // if we were to compress now, we don't have a way of automatically re-decompressing afterwards

            return true;
        }

        public void WriteTo(FileHandler output)
        {
            // all the header/mip/tex data should've already been up to date, therefore we just need to write everything out here

            Header.Write(output);
            Mips.Write(output);

            FileHandler.Seek(Mips[0].offset);
            output.Seek(Mips[0].offset);

            if (CurrentSerializerVersion >= TexSerializerVersion.MHWilds && Mips.Count > 0)
            {
                // TODO gdeflate compress
                FileHandler.Stream.CopyTo(output.Stream);
            }
            else
            {
                FileHandler.Stream.CopyTo(output.Stream);
            }
        }

        public int GetBestMipLevelForDimensions(int width, int height)
        {
            var targetMip = 0;
            var mipW = Header.width;
            var mipH = Header.height;
            while (mipW > width && mipH > height && Header.mipCount > targetMip + 1) {
                mipW /= 2;
                mipH /= 2;
                targetMip++;
            }
            return targetMip;
        }

        public void LoadDataFromDDS(DDSFile source)
        {
            var handler = FileHandler;

            handler.Seek(0);
            // update header data
            Header.format = source.Header.DX10.Format;
            Header.width = (short)source.Header.width;
            Header.height = (short)source.Header.height;
            Header.depth = 1;
            Header.imageCount = 1;
            Header.mipCount = (byte)source.Header.mipMapCount;
            Header.mipHeaderSize = (byte)(Header.mipCount * Header.MipHeaderSize);
            Header.swizzleControl = -1;
            Header.Write(handler);

            var mipHeaderOffet = handler.Tell();
            handler.Skip(Header.mipHeaderSize);

            Mips.Clear();
            var dataStartOffset = handler.Tell();
            // truncate any existing data
            handler.Stream.SetLength(dataStartOffset);

            var mipIterator = source.CreateMipMapIterator();
            var mipData = new DDSFile.MipMapLevelData();
            while (mipIterator.Next(ref mipData))
            {
                var mip = new MipHeader();
                mip.offset = handler.Tell();

                int realPitchSize = (int)(!mipIterator.IsCompressed ? mipData.data.Length / mipData.height : mipData.data.Length / mipData.height * 4);
                // round up to nearest multiple of 256 because REE does that
                var paddedPitchSize = (int)(Math.Ceiling(realPitchSize / 256f) * 256f);
                var pitchPadding = paddedPitchSize - realPitchSize;
                mip.pitch = paddedPitchSize;
                if (pitchPadding == 0)
                {
                    handler.WriteSpan<byte>(mipData.data);
                }
                else
                {
                    // write line per line
                    for (int i = 0; i < mipData.data.Length; i += realPitchSize)
                    {
                        handler.WriteSpan<byte>(mipData.data.Slice(i, realPitchSize));
                        handler.WriteNull(pitchPadding);
                    }
                }
                mip.size = (int)(handler.Tell() - mip.offset);
                Mips.Add(mip);
            }

            handler.Seek(mipHeaderOffet);
            Mips.Write(handler);
        }

        public void SaveAsDDS(string filepath, int imageIndex = 0, int startMipMap = 0, int maxMipCount = int.MaxValue)
        {
            using var fs = File.Create(filepath);
            ConvertToDDS(new FileHandler(fs, filepath));
        }

        public DDSFile ConvertToDDS(FileHandler? newFileHandler = null, int imageIndex = 0, int startMipMap = 0, int maxMipCount = int.MaxValue)
        {
            newFileHandler ??= new FileHandler();

            var dds = new DDSFile(newFileHandler);
            dds.Header = ToDDSHeader();
            var mips = 0;
            dds.Write();
            var iterator = CreateIterator(imageIndex, startMipMap);
            var data = new DDSFile.MipMapLevelData();
            while (iterator.Next(ref data)) {
                if (mips == 0) {
                    dds.Header.width = data.width;
                    dds.Header.height = data.height;
                }
                mips++;
                newFileHandler.Stream.Write(data.data);
                if (mips >= maxMipCount) break;
            }
            dds.Header.mipMapCount = mips;
            newFileHandler.Seek(0);
            dds.Write();
            iterator.Dispose();
            return dds;
        }

        public DDSFile.MipMapLevelData GetMipMapData(int level, int imageIndex = 0)
        {
            var mipHeader = Mips[imageIndex * Header.mipCount + level];
            return new DDSFile.MipMapLevelData() {
                height = Math.Max(1, (uint)Header.height >> level),
                width = Math.Max(1, (uint)Header.width >> level),
                data = FileHandler.ReadBytes(mipHeader.offset, mipHeader.size)
            };
        }

        public DDSHeader ToDDSHeader()
        {
            return new DDSHeader() {
                magic = DDSFile.Magic,
                size = 124,
                flags = HeaderFlags.CAPS | HeaderFlags.HEIGHT | HeaderFlags.WIDTH | HeaderFlags.MIPMAPCOUNT | HeaderFlags.LINEARSIZE,
                width = (uint)Header.width,
                height = (uint)Header.height,
                depth = 1,
                pitchOrLinearSize = (uint)(Header.width * Header.height),
                mipMapCount = Header.mipCount,
                PixelFormat = new DDSPixelFormat() {
                    FourCC = DDSFourCC.DX10,
                    Size = 32,
                    Flags = PixelFormatFlags.FOURCC,
                    RGBBitCount = 0,
                    RBitMask = 0,
                    GBitMask = 0,
                    BBitMask = 0,
                    ABitMask = 0,
                },
                Caps1 = DDSCaps.TEXTURE | DDSCaps.MIPMAP,
                DX10 = new DX10Header() {
                    Format = Header.format,
                    ArraySize = 1,
                    MiscFlags = 0,
                    MiscFlags2 = DDSMisc2.ALPHA_MODE_UNKNOWN,
                    ResourceDimension = ResourceDimension.TEXTURE2D,
                },
                IsHasDX10 = true,
            };
        }

        public TexMipMapIterator CreateIterator(int imageIndex = 0, int mipMapLevel = 0)
        {
            if (imageIndex >= Header.imageCount || mipMapLevel >= Header.mipCount) return new TexMipMapIterator();
            FileHandler.Seek(Mips[imageIndex * Header.mipCount + mipMapLevel].offset);
            var w = Math.Max(1, Header.width >> mipMapLevel);
            var h = Math.Max(1, Header.height >> mipMapLevel);
            var compressed = Header.format.IsBlockCompressedFormat();
            if (!compressed) {
                return new TexMipMapIterator(FileHandler, Mips, Header.mipCount, w, h, Header.BitsPerPixel, false) { mip = mipMapLevel };
            }
            return new TexMipMapIterator(FileHandler, Mips, Header.mipCount, w, h, Header.format.GetCompressedBlockSize(), true) { mip = mipMapLevel };
        }

        public struct TexMipMapIterator : IDisposable
        {
            private FileHandler handler;
            private List<MipHeader> mips;
            private int maxMipMapLevel;
            private uint w;
            private uint h;
            internal int mip;
            private int blockSize;
            private bool isCompressed;

            private byte[]? bytes;

            private int CurrentCompressedMipSize => isCompressed ? (int)((w + 3) / 4) * (int)((h + 3) / 4) * blockSize : (int)(w * h * (blockSize / 8));

            public TexMipMapIterator(FileHandler handler, List<MipHeader> mips, int maxMipMapLevel, int w, int h, int blockSize, bool compressed)
            {
                this.handler = handler;
                this.mips = mips;
                this.maxMipMapLevel = maxMipMapLevel;
                this.blockSize = blockSize;
                this.w = (uint)w;
                this.h = (uint)h;
                isCompressed = compressed;
            }

            public bool Next(ref DDSFile.MipMapLevelData data)
            {
                if (mip >= maxMipMapLevel) {
                    data.data = Span<byte>.Empty;
                    return false;
                }

                var pitch = mips[mip++].pitch;
                if (h == 0 || w == 0) {
                    data.data = Span<byte>.Empty;
                    return false;
                }

                var size = CurrentCompressedMipSize;
                int realPitchSize = (int)(!isCompressed ? size / h : size / h * 4);

                if (bytes == null) {
                    bytes = ArrayPool<byte>.Shared.Rent(size);
                }
                data.height = h;
                data.width = w;

                var offset = 0;
                var strideOffset = pitch - realPitchSize;
                if (strideOffset == 0) {
                    handler.ReadBytes(bytes, size);
                } else {
                    var pixelBlockSize = isCompressed ? 4 : 1;
                    for (int i = 0; i < h; i += pixelBlockSize) {
                        handler.Stream.Read(bytes, offset, realPitchSize);
                        handler.Skip(strideOffset);
                        offset += realPitchSize;
                    }
                }

                h >>= 1;
                w >>= 1;
                data.data = bytes.AsSpan().Slice(0, (int)size);
                return true;
            }

            public void Dispose()
            {
                if (bytes != null) {
                    ArrayPool<byte>.Shared.Return(bytes);
                }
            }
        }
    }
}
