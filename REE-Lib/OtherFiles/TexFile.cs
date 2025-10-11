using System.Numerics;
using ReeLib.DDS;

namespace ReeLib.Tex
{
    public partial class Header : BaseModel
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
        public int swizzleControl;
        public uint cubemapMarker;
        public TexFlags flags;

        public byte swizzleHeightDepth;
        public byte swizzleWidth;
        public ushort null1;
        public ushort seven;
        public ushort one;

        public int BitsPerPixel => format.GetBitsPerPixel();
        public bool IsPowerOfTwo => BitOperations.IsPow2(width) && BitOperations.IsPow2(height);

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref magic);
            handler.Read(ref version);
            handler.Read(ref width);
            handler.Read(ref height);
            handler.Read(ref depth);
            var Version = TexFile.GetInternalVersion(version);
            if (Version > 20) {
                handler.Read(ref imageCount);
                handler.Read(ref mipHeaderSize);
                mipCount = (byte)(mipHeaderSize / 16);
            } else {
                handler.Read(ref mipCount);
                handler.Read(ref imageCount);
            }
            handler.Read(ref format);
            handler.Read(ref swizzleControl);
            handler.Read(ref cubemapMarker);
            handler.Read(ref flags);
            if (Version > 27) {
                handler.Read(ref swizzleHeightDepth);
                handler.Read(ref swizzleWidth);
                handler.Read(ref null1);
                handler.Read(ref seven);
                handler.Read(ref one);
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
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
    using ReeLib.Tex;

    public class TexFile : BaseFile
    {
        public const uint Magic = 0x00584554;

        public Header Header = new();
        public List<MipHeader> Mips { get; } = new();

        public static int GetInternalVersion(int version) => version switch {
            190820018 => 20, // re3
            _ => version,
        };

        public TexFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);
            if (Header.depth > 1) {
                throw new Exception("Depth > 1 textures not supported");
            }
            for (int i = 0; i < Header.mipCount * Header.imageCount; ++i) {
                var mip = new MipHeader();
                handler.Read(ref mip);
                Mips.Add(mip);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            throw new NotImplementedException();
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
                }
            };
        }

        public TexMipMapIterator CreateIterator(int imageIndex = 0)
        {
            FileHandler.Seek(Mips[imageIndex * Header.mipCount].offset);
            return new TexMipMapIterator(FileHandler, Mips, Header.mipCount, Header.width, Header.height, Header.format.GetCompressedBlockSize());
        }

        public struct TexMipMapIterator : IDisposable
        {
            private FileHandler handler;
            private List<MipHeader> mips;
            private int maxMipMapLevel;
            private uint w;
            private uint h;
            private int mip;
            private int blockSize;

            private byte[]? bytes;

            public TexMipMapIterator(FileHandler handler, List<MipHeader> mips, int maxMipMapLevel, int w, int h, int blockSize)
            {
                this.handler = handler;
                this.mips = mips;
                this.maxMipMapLevel = maxMipMapLevel;
                this.blockSize = blockSize;
                this.w = (uint)w;
                this.h = (uint)h;
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

                var size = (int)((w + 3) / 4) * (int)((h + 3) / 4) * blockSize;
                int realPitchSize = (int)(size / h * 4);

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
                    for (int i = 0; i < h; i += 4) {
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
