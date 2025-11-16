using System.Buffers;
using System.Numerics;
using System.Text.RegularExpressions;
using ReeLib.DDS;

namespace ReeLib.DDS
{
    public struct DDSHeader
    {
        public uint magic;
        public uint size;
        public HeaderFlags flags;
        public uint height;
        public uint width;
        public uint pitchOrLinearSize;
        public uint depth;
        public int mipMapCount;
        public DDSPixelFormat PixelFormat;
        public DDSCaps Caps1;
        public DDSCaps2 Caps2;
        public bool IsHasDX10;
        public DX10Header DX10;

        public int BitsPerPixel => IsHasDX10 ? DX10.Format.GetBitsPerPixel() : PixelFormat.FourCC.GetBitsPerPixel();
    }

    public struct DDSPixelFormat
    {
        public uint Size;
        public PixelFormatFlags Flags;
        public DDSFourCC FourCC;
        public uint RGBBitCount;
        public uint RBitMask;
        public uint GBitMask;
        public uint BBitMask;
        public uint ABitMask;
    }

    public struct DX10Header
    {
        public DxgiFormat Format;
        public ResourceDimension ResourceDimension;
        public DDSMisc MiscFlags;
        public uint ArraySize;
        public DDSMisc2 MiscFlags2;
    }

    public enum ResourceDimension
    {
        UNKNOWN = 0,
        BUFFER = 1,
        TEXTURE1D = 2,
        TEXTURE2D = 3,
        TEXTURE3D = 4
    }

    public enum DDSMisc
    {
        TEXTURECUBE = 0x4,
    }

    public enum DDSMisc2
    {
        ALPHA_MODE_UNKNOWN = 0x00,
        ALPHA_MODE_STRAIGHT = 0x01,
        ALPHA_MODE_PREMULTIPLIED = 0x02,
        ALPHA_MODE_OPAQUE = 0x03,
        ALPHA_MODE_CUSTOM = 0x04,
    }

    public enum DxgiFormat
    {
        R32G32B32A32_TYPELESS = 1,
        R32G32B32A32_FLOAT = 2,
        R32G32B32A32_UINT = 3,
        R32G32B32A32_SINT = 4,
        R32G32B32_TYPELESS = 5,
        R32G32B32_FLOAT = 6,
        R32G32B32_UINT = 7,
        R32G32B32_SINT = 8,
        R16G16B16A16_TYPELESS = 9,
        R16G16B16A16_FLOAT = 0XA,
        R16G16B16A16_UNORM = 0XB,
        R16G16B16A16_UINT = 0XC,
        R16G16B16A16_SNORM = 0XD,
        R16G16B16A16_SINT = 0XE,
        R32G32_TYPELESS = 0XF,
        R32G32_FLOAT = 0X10,
        R32G32_UINT = 0X11,
        R32G32_SINT = 0X12,
        R32G8X24_TYPELESS = 0X13,
        D32_FLOAT_S8X24_UINT = 0X14,
        R32_FLOAT_X8X24_TYPELESS = 0X15,
        X32_TYPELESS_G8X24_UINT = 0X16,
        R10G10B10A2_TYPELESS = 0X17,
        R10G10B10A2_UNORM = 0X18,
        R10G10B10A2_UINT = 0X19,
        R11G11B10_FLOAT = 0X1A,
        R8G8B8A8_TYPELESS = 0X1B,
        R8G8B8A8_UNORM = 0X1C,
        R8G8B8A8_UNORM_SRGB = 0X1D,
        R8G8B8A8_UINT = 0X1E,
        R8G8B8A8_SNORM = 0X1F,
        R8G8B8A8_SINT = 0X20,
        R16G16_TYPELESS = 0X21,
        R16G16_FLOAT = 0X22,
        R16G16_UNORM = 0X23,
        R16G16_UINT = 0X24,
        R16G16_SNORM = 0X25,
        R16G16_SINT = 0X26,
        R32_TYPELESS = 0X27,
        D32_FLOAT = 0X28,
        R32_FLOAT = 0X29,
        R32_UINT = 0X2A,
        R32_SINT = 0X2B,
        R24G8_TYPELESS = 0X2C,
        D24_UNORM_S8_UINT = 0X2D,
        R24_UNORM_X8_TYPELESS = 0X2E,
        X24_TYPELESS_G8_UINT = 0X2F,
        R8G8_TYPELESS = 0X30,
        R8G8_UNORM = 0X31,
        R8G8_UINT = 0X32,
        R8G8_SNORM = 0X33,
        R8G8_SINT = 0X34,
        R16_TYPELESS = 0X35,
        R16_FLOAT = 0X36,
        D16_UNORM = 0X37,
        R16_UNORM = 0X38,
        R16_UINT = 0X39,
        R16_SNORM = 0X3A,
        R16_SINT = 0X3B,
        R8_TYPELESS = 0X3C,
        R8_UNORM = 0X3D,
        R8_UINT = 0X3E,
        R8_SNORM = 0X3F,
        R8_SINT = 0X40,
        A8_UNORM = 0X41,
        R1_UNORM = 0X42,
        R9G9B9E5_SHAREDEXP = 0X43,
        R8G8_B8G8_UNORM = 0X44,
        G8R8_G8B8_UNORM = 0X45,
        BC1_TYPELESS = 0X46,
        BC1_UNORM = 0X47,
        BC1_UNORM_SRGB = 0X48,
        BC2_TYPELESS = 0X49,
        BC2_UNORM = 0X4A,
        BC2_UNORM_SRGB = 0X4B,
        BC3_TYPELESS = 0X4C,
        BC3_UNORM = 0X4D,
        BC3_UNORM_SRGB = 0X4E,
        BC4_TYPELESS = 0X4F,
        BC4_UNORM = 0X50,
        BC4_SNORM = 0X51,
        BC5_TYPELESS = 0X52,
        BC5_UNORM = 0X53,
        BC5_SNORM = 0X54,
        B5G6R5_UNORM = 0X55,
        B5G5R5A1_UNORM = 0X56,
        B8G8R8A8_UNORM = 0X57,
        B8G8R8X8_UNORM = 0X58,
        R10G10B10_XR_BIAS_A2_UNORM = 0X59,
        B8G8R8A8_TYPELESS = 0X5A,
        B8G8R8A8_UNORM_SRGB = 0X5B,
        B8G8R8X8_TYPELESS = 0X5C,
        B8G8R8X8_UNORM_SRGB = 0X5D,
        BC6H_TYPELESS = 0X5E,
        BC6H_UF16 = 0X5F,
        BC6H_SF16 = 0X60,
        BC7_TYPELESS = 0X61,
        BC7_UNORM = 0X62,
        BC7_UNORM_SRGB = 0X63,

        VIAEXTENSION = 0X400,
        ASTC4X4_TYPELESS = 0X401,
        ASTC4X4_UNORM = 0X402,
        ASTC4X4_UNORM_SRGB = 0X403,
        ASTC5X4_TYPELESS = 0X404,
        ASTC5X4_UNORM = 0X405,
        ASTC5X4_UNORM_SRGB = 0X406,
        ASTC5X5_TYPELESS = 0X407,
        ASTC5X5_UNORM = 0X408,
        ASTC5X5_UNORM_SRGB = 0X409,
        ASTC6X5_TYPELESS = 0X40A,
        ASTC6X5_UNORM = 0X40B,
        ASTC6X5_UNORM_SRGB = 0X40C,
        ASTC6X6_TYPELESS = 0X40D,
        ASTC6X6_UNORM = 0X40E,
        ASTC6X6_UNORM_SRGB = 0X40F,
        ASTC8X5_TYPELESS = 0X410,
        ASTC8X5_UNORM = 0X411,
        ASTC8X5_UNORM_SRGB = 0X412,
        ASTC8X6_TYPELESS = 0X413,
        ASTC8X6_UNORM = 0X414,
        ASTC8X6_UNORM_SRGB = 0X415,
        ASTC8X8_TYPELESS = 0X416,
        ASTC8X8_UNORM = 0X417,
        ASTC8X8_UNORM_SRGB = 0X418,
        ASTC10X5_TYPELESS = 0X419,
        ASTC10X5_UNORM = 0X41A,
        ASTC10X5_UNORM_SRGB = 0X41B,
        ASTC10X6_TYPELESS = 0X41C,
        ASTC10X6_UNORM = 0X41D,
        ASTC10X6_UNORM_SRGB = 0X41E,
        ASTC10X8_TYPELESS = 0X41F,
        ASTC10X8_UNORM = 0X420,
        ASTC10X8_UNORM_SRGB = 0X421,
        ASTC10X10_TYPELESS = 0X422,
        ASTC10X10_UNORM = 0X423,
        ASTC10X10_UNORM_SRGB = 0X424,
        ASTC12X10_TYPELESS = 0X425,
        ASTC12X10_UNORM = 0X426,
        ASTC12X10_UNORM_SRGB = 0X427,
        ASTC12X12_TYPELESS = 0X428,
        ASTC12X12_UNORM = 0X429,
        ASTC12X12_UNORM_SRGB = 0X42A,

        FORCE_UINT = 0X7FFFFFFF,
    }

    [Flags]
    public enum DDSCaps
    {
        COMPLEX = 0x8,
        MIPMAP = 0x400000,
        TEXTURE = 0x1000
    }

    [Flags]
    public enum DDSCaps2
    {
        CUBEMAP = 0x200,
        CUBEMAP_POSITIVEX = 0x400,
        CUBEMAP_NEGATIVEX = 0x800,
        CUBEMAP_POSITIVEY = 0x1000,
        CUBEMAP_NEGATIVEY = 0x2000,
        CUBEMAP_POSITIVEZ = 0x4000,
        CUBEMAP_NEGATIVEZ = 0x8000,
        VOLUME = 0x200000
    }

    [Flags]
    public enum HeaderFlags : uint
    {
        NONE = 0x00,
        CAPS = 0x01,
        HEIGHT = 0x02,
        WIDTH = 0x04,
        PITCH = 0x08,
        PIXELFORMAT = 0x1000,
        MIPMAPCOUNT = 0x20000,
        LINEARSIZE = 0x80000,
        DEPTH = 0x800000
    }

    [Flags]
    public enum PixelFormatFlags
    {
        NONE = 0x00,
        ALPHAPIXELS = 0x01,
        ALPHA = 0x02,
        FOURCC = 0x04,
        RGB = 0x40,
        YUV = 0x200,
        LUMINANCE = 0x20000
    }

    public enum DDSFourCC
    {
        NONE = 0x00,
        DXT1 = 0x31545844,
        DXT2 = 0x32545844,
        DXT3 = 0x33545844,
        DXT4 = 0x34545844,
        DXT5 = 0x35545844,
        DX10 = 0x30315844,
    }
}

namespace ReeLib
{
    public static partial class DDSFileExtensions
    {
        public static bool IsASTCFormat(this DxgiFormat format) => (format & DxgiFormat.VIAEXTENSION) != 0 && format != DxgiFormat.FORCE_UINT;
        public static bool IsBlockCompressedFormat(this DxgiFormat format) => _bcFormats.Contains(format);
        public static bool IsRGBFormat(this DxgiFormat format) => !IsASTCFormat(format) && !IsBlockCompressedFormat(format) && format != DxgiFormat.FORCE_UINT;

        public static int GetBitsPerPixel(this DDSFourCC format) => format switch { DDSFourCC.DXT1 or DDSFourCC.DXT4 => 8, _ => 16 };

        public static int GetBitsPerPixel(this DxgiFormat format)
        {
            if (IsBlockCompressedFormat(format)) return format.GetCompressedBlockSize();
            if (IsRGBFormat(format)) return RGBBitsPerPixel.GetValueOrDefault(format);
            throw new NotImplementedException();
        }

        public static int GetImageSize(this DxgiFormat format, int width, int height)
        {
            return format.IsBlockCompressedFormat()
                ? (int)((width + 3) / 4) * (int)((height + 3) / 4) * format.GetCompressedBlockSize()
                : (int)(width * height * (format.GetBitsPerPixel() / 8));
        }

        public static int GetPitch(this DxgiFormat format, int width, int height)
        {
            return format.IsBlockCompressedFormat()
                ? (int)((width + 3) / 4) * format.GetCompressedBlockSize()
                : (int)(width * (format.GetBitsPerPixel() / 8));
        }

        public static int GetCompressedBlockSize(this DxgiFormat format)
        {
            if (_bcFormats8.Contains(format)) return 8;
            if (_bcFormats16.Contains(format)) return 16;

            throw new Exception("DXGI format is not a BC format: " + format);
        }

        public static int CalculateMipCount(int width, int height)
        {
            return BitOperations.Log2(BitOperations.RoundUpToPowerOf2((uint)Math.Max(width, height)));
        }

        private static readonly HashSet<DxgiFormat> _astcFormats = Enum.GetValues<DxgiFormat>().Where(v => v.IsASTCFormat()).ToHashSet();
        private static readonly HashSet<DxgiFormat> _bcFormats = Enum.GetValues<DxgiFormat>().Where(v => v.ToString().StartsWith("BC")).ToHashSet();
        private static readonly HashSet<DxgiFormat> _bcFormats8 = Enum.GetValues<DxgiFormat>().Where(v => v.ToString().StartsWith("BC1") || v.ToString().StartsWith("BC4")).ToHashSet();
        private static readonly HashSet<DxgiFormat> _bcFormats16 = _bcFormats.Except(_bcFormats8).ToHashSet();
        private static readonly HashSet<DxgiFormat> _rgbFormats = Enum.GetValues<DxgiFormat>().Where(v => !v.IsASTCFormat() && !v.IsBlockCompressedFormat() && v != DxgiFormat.FORCE_UINT).ToHashSet();

        [GeneratedRegex("([RGBAX])(\\d+)")]
        private static partial Regex RgbBitsRegex();
        private static int CountBitsInFormat(DxgiFormat fmt) => RgbBitsRegex().Matches(fmt.ToString()).Sum(m => int.Parse(m.Groups[2].ValueSpan));
        private static readonly Dictionary<DxgiFormat, int> RGBBitsPerPixel = _rgbFormats.ToDictionary(k => k, k => CountBitsInFormat(k));
    }

    public class DDSFile : BaseFile
    {
        public const uint Magic = 0x20534444;

        public DDSHeader Header;

        private int DataStartOffset => Header.IsHasDX10 ? 148 : 128;

        public DDSFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            handler.ReadRange(ref Header.magic, ref Header.mipMapCount);
            handler.Seek(76); // reserved 11 * 4b
            handler.ReadRange(ref Header.PixelFormat, ref Header.Caps2);
            handler.Skip(12); // caps3, caps4, reserved
            if (Header.PixelFormat.FourCC == DDSFourCC.DX10) {
                Header.IsHasDX10 = true;
                handler.Read(ref Header.DX10);
                if ((Header.DX10.Format & DxgiFormat.VIAEXTENSION) != 0) {
                    throw new NotImplementedException("ASTC textures not supported for format " + Header.DX10.Format);
                }
                if (Header.DX10.ResourceDimension != ResourceDimension.TEXTURE2D) {
                    throw new NotImplementedException("Only 2D textures supported");
                }
            } else {
                throw new NotImplementedException("Not implemented FourCC pixel format " + Header.PixelFormat.FourCC);
            }

            return true;
        }


        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.WriteRange(ref Header.magic, ref Header.mipMapCount);
            handler.Seek(76);
            handler.WriteRange(ref Header.PixelFormat, ref Header.Caps2);
            handler.Skip(12); // skip caps3, 4, reserved
            if (Header.IsHasDX10) {
                handler.Write(ref Header.DX10);
            }

            return true;
        }

        public MipMapLevelData GetMipMapData(int mipLevel)
        {
            var it = CreateMipMapIterator();
            while (it.CurrentMipLevel != mipLevel && it.Skip()) ;

            if (it.CurrentMipLevel == mipLevel) {
                var data = new MipMapLevelData();
                if (it.Next(ref data)) {
                    return data;
                }
            }

            return default;
        }

        public DdsMipMapIterator CreateMipMapIterator()
        {
            FileHandler.Seek(DataStartOffset);
            return new DdsMipMapIterator(FileHandler, Header.width, Header.height, Header.mipMapCount, Header.BitsPerPixel, Header.DX10.Format.IsBlockCompressedFormat());
        }

        public ref struct MipMapLevelData
        {
            public uint width;
            public uint height;
            public int pitch;
            public Span<byte> data;

            public override string ToString() => $"{width}x{height}: {data.Length}b";
        }

        public struct DdsMipMapIterator : IDisposable
        {
            private uint w;
            private uint h;
            private int mip;

            internal int blockSize;
            internal FileHandler handler;

            public int totalMipMapCount;
            public int CurrentMipLevel => mip;

            public readonly bool IsCompressed { get; }
            public int CurrentCompressedMipSize => IsCompressed ? (int)((w + 3) / 4) * (int)((h + 3) / 4) * blockSize : (int)(w * h * (blockSize / 8));

            private int CurrentPitch => IsCompressed ? (int)((w + 3) / 4) * blockSize : (int)(w * (blockSize / 8));

            private byte[]? bytes;

            public DdsMipMapIterator(FileHandler handler, uint width, uint height, int totalMipCount, int blockSize, bool isCompressed)
            {
                w = width;
                h = height;
                this.blockSize = blockSize;
                this.handler = handler;
                this.IsCompressed = isCompressed;
                totalMipMapCount = totalMipCount;
            }

            public bool Skip()
            {
                if (mip >= totalMipMapCount) {
                    return false;
                }

                mip++;
                if (h == 0 && w == 0) {
                    return false;
                }
                h = Math.Max(1, h);
                w = Math.Max(1, w);
                handler.Skip(CurrentCompressedMipSize);
                h >>= 1;
                w >>= 1;
                return true;
            }

            public bool Next(ref MipMapLevelData data)
            {
                if (mip >= totalMipMapCount) {
                    return false;
                }

                mip++;
                if (h == 0 || w == 0) {
                    data.data = Span<byte>.Empty;
                    return false;
                }

                var size = CurrentCompressedMipSize;
                this.bytes ??= ArrayPool<byte>.Shared.Rent(size);
                data.height = h;
                data.width = w;
                data.pitch = CurrentPitch;

                h >>= 1;
                w >>= 1;

                handler.ReadBytes(bytes, size);

                data.data = bytes.AsSpan(0, (int)size);
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
