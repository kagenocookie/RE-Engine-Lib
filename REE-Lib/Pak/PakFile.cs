using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using ReeLib.Common;
using Standart.Hash.xxHash;

namespace ReeLib.Pak
{
	public enum CompressionType
	{
        None = 0,
        Deflate = 1,
        Zstd = 2,
	}

    public enum EncryptionType
    {
        None = 0,
        Type1 = 1,
        Type2 = 2,
        Type3 = 3,
        Type4 = 4,
        Invalid = 5,
    }

    public struct Header
    {
        public uint magic;
        public byte majorVersion;
        public byte minorVersion;
        public PakFeatureFlags featureFlags;
        public int fileCount;
        public uint fingerprint;
    }

    [Flags]
    public enum PakFeatureFlags : short
    {
        EncryptEntryList = 8,
        ExtraInteger = 16,
        All = EncryptEntryList|ExtraInteger,
    }

    public class PakEntry
    {
        public uint hashLowercase;
        public uint hashUppercase;
        public long offset;
        public long compressedSize;
        public long decompressedSize;
        public long checksum;
        public CompressionType compression;
        public EncryptionType encryption;

        public string? path;

        public PakEntry()
        {
        }

        public PakEntry(string filepath)
        {
            path = filepath;
            hashUppercase = MurMur3HashUtils.GetHash(filepath.ToUpperInvariant());
            hashLowercase = MurMur3HashUtils.GetHash(filepath.ToLowerInvariant());
        }

        public ulong CombinedHash => (ulong)hashUppercase << 32 | hashLowercase;

        public void ReadVer2(FileHandler handler)
        {
            offset = handler.ReadInt64();
            compressedSize = handler.ReadInt64();
            hashUppercase = handler.ReadUInt();
            hashLowercase = handler.ReadUInt();
        }

        public void ReadVer4(FileHandler handler)
        {
            hashLowercase = handler.ReadUInt();
            hashUppercase = handler.ReadUInt();
            offset = handler.ReadInt64();
            compressedSize = handler.ReadInt64();
            decompressedSize = handler.ReadInt64();
            var attributes = handler.ReadInt64();
            checksum = handler.ReadInt64();

            compression = (CompressionType)(attributes & 0xf);
            encryption = (EncryptionType)((attributes & 0x00ff0000) >> 16);

            if (compression != CompressionType.None && compression != CompressionType.Deflate && compression != CompressionType.Zstd)
            {
                throw new NotImplementedException("Unsupported PAK entry compression type " + compression);
            }
        }

        public void WriteVer4(FileHandler handler)
        {
            handler.WriteUInt(hashLowercase);
            handler.WriteUInt(hashUppercase);
            handler.WriteInt64(offset);
            handler.WriteInt64(compressedSize);
            handler.WriteInt64(decompressedSize);
            uint attributes = (uint)compression & 0xf | ((uint)encryption << 16);
            handler.WriteInt64(attributes);
            handler.WriteInt64(checksum);
        }
    }

    internal static class Encryption
    {
        // encryption keys and base logic by Ekey/REE.PAK.Tool
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static BigInteger UnsignedBigInteger(Span<byte> bytes)
        {
            return new BigInteger(bytes, true);
        }

        private static readonly BigInteger keyModulus = UnsignedBigInteger([
            0x7D, 0x0B, 0xF8, 0xC1, 0x7C, 0x23, 0xFD, 0x3B, 0xD4, 0x75, 0x16, 0xD2, 0x33, 0x21, 0xD8, 0x10,
            0x71, 0xF9, 0x7C, 0xD1, 0x34, 0x93, 0xBA, 0x77, 0x26, 0xFC, 0xAB, 0x2C, 0xEE, 0xDA, 0xD9, 0x1C,
            0x89, 0xE7, 0x29, 0x7B, 0xDD, 0x8A, 0xAE, 0x50, 0x39, 0xB6, 0x01, 0x6D, 0x21, 0x89, 0x5D, 0xA5,
            0xA1, 0x3E, 0xA2, 0xC0, 0x8C, 0x93, 0x13, 0x36, 0x65, 0xEB, 0xE8, 0xDF, 0x06, 0x17, 0x67, 0x96,
            0x06, 0x2B, 0xAC, 0x23, 0xED, 0x8C, 0xB7, 0x8B, 0x90, 0xAD, 0xEA, 0x71, 0xC4, 0x40, 0x44, 0x9D,
            0x1C, 0x7B, 0xBA, 0xC4, 0xB6, 0x2D, 0xD6, 0xD2, 0x4B, 0x62, 0xD6, 0x26, 0xFC, 0x74, 0x20, 0x07,
            0xEC, 0xE3, 0x59, 0x9A, 0xE6, 0xAF, 0xB9, 0xA8, 0x35, 0x8B, 0xE0, 0xE8, 0xD3, 0xCD, 0x45, 0x65,
            0xB0, 0x91, 0xC4, 0x95, 0x1B, 0xF3, 0x23, 0x1E, 0xC6, 0x71, 0xCF, 0x3E, 0x35, 0x2D, 0x6B, 0xE3,
        ]);

        private static readonly BigInteger keyExponent = new BigInteger([
            0x01, 0x00, 0x01, 0x00
        ]);

        private static readonly BigInteger resourceModulus = UnsignedBigInteger([
            0x13, 0xD7, 0x9C, 0x89, 0x88, 0x91, 0x48, 0x10, 0xD7, 0xAA, 0x78, 0xAE, 0xF8, 0x59, 0xDF, 0x7D,
            0x3C, 0x43, 0xA0, 0xD0, 0xBB, 0x36, 0x77, 0xB5, 0xF0, 0x5C, 0x02, 0xAF, 0x65, 0xD8, 0x77, 0x03,
        ]);

        private static readonly BigInteger resourceExponent = UnsignedBigInteger([
            0xC0, 0xC2, 0x77, 0x1F, 0x5B, 0x34, 0x6A, 0x01, 0xC7, 0xD4, 0xD7, 0x85, 0x2E, 0x42, 0x2B, 0x3B,
            0x16, 0x3A, 0x17, 0x13, 0x16, 0xEA, 0x83, 0x30, 0x30, 0xDF, 0x3F, 0xF4, 0x25, 0x93, 0x20, 0x01,
        ]);

        public static void DecryptKey(Span<byte> key)
        {
            BigInteger m_EncryptedKey = UnsignedBigInteger(key);
            BigInteger result = BigInteger.ModPow(m_EncryptedKey, keyExponent, keyModulus);
            key.Clear();
            result.TryWriteBytes(key, out _);
        }

        [SkipLocalsInit]
        public unsafe static byte[] DecryptResource(byte[] compressedBytes, ref int size)
        {
            using var reader = new MemoryStream(compressedBytes, 0, size);
            int blockCount = (size - 8) / 128;
            var byteSize = blockCount * 8;

            Span<byte> buffer1 = stackalloc byte[64];
            Span<byte> buffer2 = stackalloc byte[64];
            reader.Read(buffer1.Slice(0, 8));
            size = (int)BitConverter.ToInt64(buffer1.Slice(0, 8));

            var outBytes = ArrayPool<byte>.Shared.Rent((int)(size + 1));
            var outSpan = outBytes.AsSpan();
            outSpan.Clear();

            for (int offset = 0; offset < byteSize; offset += 8)
            {
                reader.Read(buffer1);
                reader.Read(buffer2);

                var key = new BigInteger(buffer1);
                var data = new BigInteger(buffer2);

                var mod = BigInteger.ModPow(key, resourceExponent, resourceModulus);
                var result = BigInteger.Divide(data, mod);

                result.TryWriteBytes(outSpan[offset..], out _);
            }

            return outBytes;
        }

        public static void DecryptPakEntryData(byte[] buffer, Span<byte> key)
        {
            Encryption.DecryptKey(key);
            if (key.Length > 0)
            {
                var size = buffer.Length;
                for (int i = 0; i < size; i++)
                {
                    buffer[i] ^= (byte)(i + key[i % 32] * key[i % 29]);
                }
            }
        }
    }

    internal static class Compression
    {
        public static void DecompressZstd(byte[] compressedData, int size, Stream outputStream)
        {
            using var memoryStream = new MemoryStream(compressedData, 0, size);
            using var stream = new ZstdSharp.DecompressionStream(memoryStream);
            stream.CopyTo(outputStream);
        }

        public static void DecompressDeflate(byte[] compressedData, int size, Stream outputStream)
        {
            using var memoryStream = new MemoryStream(compressedData, 0, size);
            using var stream = new System.IO.Compression.DeflateStream(memoryStream, System.IO.Compression.CompressionMode.Decompress);
            stream.CopyTo(outputStream);
        }
    }

    internal static class Hashing
    {
        public static long GetEntryDataHash(Stream data)
        {
            var hash64 = xxHash64.ComputeHash(data, 8192, 0xffffffff);
            var hash32 = xxHash32.ComputeHash(BitConverter.GetBytes(hash64), 0xffffffff);
            return (long)hash64 << 32 | (long)hash32;
        }
    }
}

namespace ReeLib
{
    using System;
    using ReeLib.Common;
    using ReeLib.Pak;

    public class PakFile : IDisposable
    {
        public Pak.Header Header;
        public readonly List<PakEntry> Entries = new();
        public string filepath = string.Empty;
        private Stream? pakStream;
        private MemoryStream? entryTempStream;

        public const uint Magic = 0x414B504B;

        private const int FileHeaderSize = 16;

        private static readonly HashSet<(byte, byte)> supportedVersions = new()
        {
            (4, 0),
            (4, 1),
            (2, 0)
        };

        private int EntryHeaderSize => Header.majorVersion switch
        {
            4 => 48,
            2 => 24,
            _ => throw new Exception()
        };

        public void ReadContents(string filename, Dictionary<ulong, string>? expectedPaths = null)
        {
            using var fs = File.OpenRead(filename);
            ReadContents(fs, expectedPaths);
            fs.Close();
        }

        public void ReadContents(Stream stream, Dictionary<ulong, string>? expectedPaths = null)
        {
            stream.Read(MemoryUtils.StructureAsBytes(ref Header));

            if (Header.magic != Magic)
            {
                throw new InvalidDataException("File is not a valid PAK file");
            }

            if (!supportedVersions.Contains((Header.majorVersion, Header.minorVersion)))
            {
                throw new InvalidDataException($"Unsupported PAK version {Header.majorVersion}.{Header.minorVersion}");
            }

            if (Header.featureFlags != 0 && Header.featureFlags != PakFeatureFlags.EncryptEntryList && Header.featureFlags != PakFeatureFlags.All)
            {
                throw new InvalidDataException($"Unsupported PAK encryption type {Header.featureFlags}");
            }

            Entries.Clear();
            if (Header.fileCount == 0) return;

            var entrySize = EntryHeaderSize;
            var entryListDataSize = Header.fileCount * entrySize;
            var entryTable = ArrayPool<byte>.Shared.Rent(entryListDataSize);
            stream.Read(entryTable, 0, entryListDataSize);

            if ((Header.featureFlags & PakFeatureFlags.ExtraInteger) != 0)
            {
                stream.Seek(4, SeekOrigin.Current);
            }

            if (Header.featureFlags != 0)
            {
                byte[] key = ArrayPool<byte>.Shared.Rent(128);
                stream.Read(key);
                Encryption.DecryptPakEntryData(entryTable, key);
                ArrayPool<byte>.Shared.Return(key);
            }

            using var reader = new MemoryStream(entryTable);
            var entryHandler = new FileHandler(reader);
            if (Header.majorVersion == 4)
            {
                for (int i = 0; i < Header.fileCount; ++i)
                {
                    var entry = new PakEntry();
                    entry.ReadVer4(entryHandler);
                    if (expectedPaths != null)
                    {
                        if (expectedPaths.TryGetValue(entry.CombinedHash, out var path))
                        {
                            entry.path = path;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    Entries.Add(entry);
                }
            }
            else if (Header.majorVersion == 2)
            {
                for (int i = 0; i < Header.fileCount; ++i)
                {
                    var entry = new PakEntry();
                    entry.ReadVer2(entryHandler);
                    if (expectedPaths != null)
                    {
                        if (expectedPaths.TryGetValue(entry.CombinedHash, out var path))
                        {
                            entry.path = path;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    Entries.Add(entry);
                }
            }

            ArrayPool<byte>.Shared.Return(entryTable);
        }

        public void Read<TStream>(PakEntry entry, TStream outStream) where TStream : Stream
        {
            pakStream ??= File.OpenRead(filepath);

            ReadEntry(entry, pakStream, outStream);
        }

        internal static void ReadEntry<TStream>(PakEntry entry, Stream readStream, TStream outStream) where TStream : Stream
        {
            readStream.Seek(entry.offset, SeekOrigin.Begin);
            if (entry.compression == CompressionType.None) {
                var size = (int)entry.decompressedSize;
                var bytes = ArrayPool<byte>.Shared.Rent(size);
                readStream.Read(bytes, 0, size);
                outStream.Write(bytes, 0, size);
                ArrayPool<byte>.Shared.Return(bytes);
            }
            else
            {
                var size = (int)entry.compressedSize;
                var bytes = ArrayPool<byte>.Shared.Rent(size);
                readStream.Read(bytes, 0, size);

                if (entry.encryption != EncryptionType.None)
                {
                    var decrypted = Encryption.DecryptResource(bytes, ref size);
                    ArrayPool<byte>.Shared.Return(bytes);
                    bytes = decrypted;
                }

                switch (entry.compression)
                {
                    case CompressionType.Deflate:
                        Compression.DecompressDeflate(bytes, size, outStream);
                        break;
                    case CompressionType.Zstd:
                        Compression.DecompressZstd(bytes, size, outStream);
                        break;
                    default:
                        outStream.Write(bytes, 0, size);
                        break;
                }

                ArrayPool<byte>.Shared.Return(bytes);
            }
        }

        /// <summary>
        /// Opens a file stream for the current filepath and moves it to the offset of the first file. The file count and version in the header must be correct before calling this method.
        /// </summary>
        public void OpenWrite()
        {
            var dir = Path.GetDirectoryName(filepath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            pakStream = File.Create(filepath);
            pakStream.Seek(FileHeaderSize + EntryHeaderSize * Header.fileCount, SeekOrigin.Begin);

            if ((Header.featureFlags & PakFeatureFlags.ExtraInteger) != 0)
            {
                pakStream.Seek(4, SeekOrigin.Current);
            }
        }

        public void WriteContents()
        {
            ArgumentNullException.ThrowIfNull(pakStream);
            pakStream.Seek(0, SeekOrigin.Begin);

            Header.magic = Magic;
            Header.featureFlags = 0;
            // the Header.fingerprint doesn't seem to matter
            // hopefully we won't need to encrypt the entry list either
            pakStream.Write(MemoryUtils.StructureAsBytes(ref Header));

            var entrySize = EntryHeaderSize;
            var entryListDataSize = Header.fileCount * entrySize;

            var handler = new FileHandler(pakStream);
            foreach (var entry in Entries.OrderBy(e => e.CombinedHash))
            {
                entry.WriteVer4(handler);
            }
        }

        public Stream StartWriteFile(string nativePath)
        {
            return StartWriteFile(new PakEntry(nativePath));
        }

        public Stream StartWriteFile(PakEntry entry)
        {
            ArgumentNullException.ThrowIfNull(pakStream);
            Entries.Add(entry);
            entry.offset = pakStream.Position;
            entryTempStream ??= new MemoryStream();
            entryTempStream.Seek(0, SeekOrigin.Begin);
            entryTempStream.SetLength(0);
            return entryTempStream;
        }

        public void FinishWriteFile()
        {
            ArgumentNullException.ThrowIfNull(pakStream);
            ArgumentNullException.ThrowIfNull(entryTempStream);
            var entry = Entries.Last();
            entry.decompressedSize = entryTempStream.Length;

            if (entry.compression != CompressionType.None)
            {
                throw new NotImplementedException("Pak entry compression not supported");
            }

            entryTempStream.Seek(0, SeekOrigin.Begin);
            entry.checksum = Hashing.GetEntryDataHash(entryTempStream);

            entryTempStream.Seek(0, SeekOrigin.Begin);
            // NOTE: this is where we'd apply compression/encryption if we wanted it
            entryTempStream.CopyTo(pakStream);

            entry.compressedSize = pakStream.Position - entry.offset;
        }

        public void Close()
        {
            pakStream?.Dispose();
            pakStream = null;
        }

        public void Dispose()
        {
            pakStream?.Dispose();
            pakStream = null;
            GC.SuppressFinalize(this);
        }
    }
}