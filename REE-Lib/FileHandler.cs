using ReeLib.Common;
using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ReeLib
{
    public class FileHandler : IDisposable
    {
        public string? FilePath { get; private set; }
        public Stream Stream { get; private set; }
        public long Offset { get; set; }
        public bool IsMemory => Stream is MemoryStream;
        public StringTable? StringTable { get; private set; }
        public StringTable? AsciiStringTable { get; private set; }
        public OffsetContentTable? OffsetContentTable { get; private set; }
        private Sunday? searcher;
        private int fileVersion = -1;

        public int FileVersion {
            get {
                if (fileVersion != -1) return fileVersion;
                if (FilePath == null) return 0;
                return fileVersion = PathUtils.ParseFileFormat(FilePath).version;
            }
            set => fileVersion = value;
        }

        public long Position => Stream.Position;

        private static readonly byte[] zeroes = new byte[256];

        public FileHandler() : this(new MemoryStream())
        {
        }

        public FileHandler(string path, bool holdFile = false)
        {
            Open(path, holdFile);
            Stream ??= new MemoryStream();
        }

        public FileHandler(Stream stream, string? filepath = null)
        {
            Stream = stream;
            FilePath = filepath;
        }

        ~FileHandler()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stream.Dispose();
            }
        }

        public void Open(string path, bool holdFile = false)
        {
            FilePath = path;
            FileStream fileStream = new(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (!holdFile) {
                Stream = new MemoryStream((int)fileStream.Length);
                fileStream.CopyTo(Stream);
                fileStream.Dispose();
                Stream.Position = 0;
            } else {
                Stream = fileStream;
            }
        }

        public void Reopen()
        {
            if (FilePath != null)
            {
                Stream.Dispose();
                Open(FilePath, !IsMemory);
            }
        }

        public FileHandler AsMemory()
        {
            var newStream = new MemoryStream();
            long pos = Stream.Position;
            Stream.Position = 0;
            Stream.CopyTo(newStream);
            Stream.Position = pos;
            newStream.Position = pos;
            return new FileHandler(newStream) { Offset = Offset };
        }

        public FileHandler WithOffset(long offset)
        {
            return new FileHandler(Stream) { Offset = Offset + offset, FileVersion = FileVersion, FilePath = FilePath };
        }

        public void Save(string? path = null)
        {
            if ((path == null || path == FilePath) && !IsMemory)
            {
                Stream.Flush();
            }
            else
            {
                path ??= FilePath;
                if (path == null) return;
                using FileStream fileStream = File.Create(path);
                long pos = Stream.Position;
                Stream.Position = 0;
                Stream.CopyTo(fileStream);
                Stream.Position = pos;
            }
        }

        public void SaveAs(string path)
        {
            FilePath = path;
            FileStream fileStream = File.Create(path);
            long pos = Stream.Position;
            Stream.Position = 0;
            Stream.CopyTo(fileStream);
            Stream.Position = pos;
            if (!IsMemory)
            {
                Stream = fileStream;
            }
            else
            {
                fileStream.Dispose();
            }
        }

        public void Clear()
        {
            Stream.SetLength(0);
            Stream.Flush();
        }

        public long FileSize()
        {
            return Stream.Length;
        }

        public void Seek(long tell)
        {
            Stream.Position = tell + Offset;
        }

        public void Align(int alignment)
        {
            long delta = Stream.Position % alignment;
            if (delta != 0)
            {
                Stream.Position += alignment - delta;
            }
        }

        public void SeekOffsetAligned(int offset, int align = 4)
        {
            Seek(Utils.AlignSize(Tell() + offset, align));
        }

        public long Tell()
        {
            return Stream.Position - Offset;
        }

        public void Skip(long skip)
        {
            Stream.Seek(skip, SeekOrigin.Current);
        }

        public void ReadNull(int count)
        {
            #if DEBUG
            switch (count) {
                case 0: break;
                case 1: DataInterpretationException.ThrowIfNotZero(Read<byte>()); break;
                case 2: DataInterpretationException.ThrowIfNotZero(Read<ushort>()); break;
                case 4: DataInterpretationException.ThrowIfNotZero(Read<uint>()); break;
                case 8: DataInterpretationException.ThrowIfNotZero(Read<long>()); break;
                default:
                    Span<byte> bytes = stackalloc byte[count];
                    Stream.Read(bytes);
                    for (int i = 0; i < bytes.Length; ++i) DataInterpretationException.ThrowIfNotZero(bytes[i]);
                    break;
            }
            #else
            Stream.Seek((long)count, SeekOrigin.Current);
            #endif
        }

        public void CheckRange()
        {
            if (Stream.Position > Stream.Length)
            {
                throw new IndexOutOfRangeException($"Seek out of range {Stream.Position} > {Stream.Length}");
            }
        }

        public readonly struct JumpBackGuard : IDisposable
        {
            private readonly FileHandler handler;
            private readonly long position;
            private readonly bool jumpBack;

            public JumpBackGuard(FileHandler handler, bool jumpBack = true)
            {
                this.handler = handler;
                position = handler.Tell();
                this.jumpBack = jumpBack;
            }

            public void Dispose()
            {
                if (jumpBack)
                {
                    handler.Seek(position);
                }
            }
        }

        public JumpBackGuard SeekJumpBack(bool jumpBack = true)
        {
            return new JumpBackGuard(this, jumpBack);
        }

        public JumpBackGuard SeekJumpBack(long tell, bool jumpBack = true)
        {
            var defer = new JumpBackGuard(this, jumpBack);
            Seek(tell);
            return defer;
        }

        public byte ReadByte()
        {
            return (byte)Stream.ReadByte();
        }

        public byte ReadByte(long tell, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            return (byte)Stream.ReadByte();
        }

        public sbyte ReadSByte()
        {
            return (sbyte)Stream.ReadByte();
        }

        public sbyte ReadSByte(long tell, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            return (sbyte)Stream.ReadByte();
        }

        public char ReadChar()
        {
            return Read<char>();
        }

        public char ReadChar(long tell, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            return Read<char>();
        }

        public bool ReadBoolean()
        {
            return Stream.ReadByte() != 0;
        }

        public bool ReadBoolean(long tell, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            return Stream.ReadByte() != 0;
        }

        public short ReadShort()
        {
            return Read<short>();
        }

        public short ReadShort(long tell, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            return Read<short>();
        }

        public ushort ReadUShort()
        {
            return Read<ushort>();
        }

        public ushort ReadUShort(long tell, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            return Read<ushort>();
        }

        public int ReadInt(long tell, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            return Read<int>();
        }

        public int ReadInt()
        {
            return Read<int>();
        }

        public uint ReadUInt(long tell, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            return Read<uint>();
        }

        public uint ReadUInt()
        {
            return Read<uint>();
        }

        public long ReadInt64()
        {
            return Read<long>();
        }

        public long ReadInt64(long tell, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            return Read<long>();
        }

        public ulong ReadUInt64()
        {
            return Read<ulong>();
        }

        public ulong ReadUInt64(long tell, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            return Read<ulong>();
        }

        public float ReadFloat(long tell, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            return Read<float>();
        }

        public float ReadFloat()
        {
            return Read<float>();
        }

        public double ReadDouble(long tell, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            return Read<double>();
        }

        public double ReadDouble()
        {
            return Read<double>();
        }

        public int ReadBytes(byte[] buffer, int length = -1)
        {
            return Stream.Read(buffer, 0, length == -1 ? buffer.Length : length);
        }

        public int ReadBytes(long tell, byte[] buffer, int length = -1, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            return Stream.Read(buffer, 0, length == -1 ? buffer.Length : length);
        }

        public byte[] ReadBytes(long tell, int length, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            var buffer = new byte[length];
            Stream.Read(buffer);
            return buffer;
        }

        public void WriteByte(byte value)
        {
            Stream.WriteByte(value);
        }

        public void WriteByte(long tell, byte value, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            Stream.WriteByte(value);
        }

        public void WriteSByte(sbyte value)
        {
            Stream.WriteByte((byte)value);
        }

        public void WriteSByte(long tell, sbyte value, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            Stream.WriteByte((byte)value);
        }

        public void WriteChar(char value)
        {
            Write(value);
        }

        public void WriteChar(long tell, char value, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            Write(value);
        }

        public void WriteBoolean(bool value)
        {
            Stream.WriteByte(value ? (byte)1 : (byte)0);
        }

        public void WriteBoolean(long tell, bool value, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            Stream.WriteByte(value ? (byte)1 : (byte)0);
        }

        public void WriteShort(short value)
        {
            Write(value);
        }

        public void WriteShort(long tell, short value, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            Write(value);
        }

        public void WriteUShort(ushort value)
        {
            Write(value);
        }

        public void WriteUShort(long tell, ushort value, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            Write(value);
        }

        public void WriteInt(int value)
        {
            Write(value);
        }

        public void WriteInt(long tell, int value, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            Write(value);
        }

        public void WriteUInt(uint value)
        {
            Write(value);
        }

        public void WriteUInt(long tell, uint value, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            Write(value);
        }

        public void WriteInt64(long value)
        {
            Write(value);
        }

        public void WriteInt64(long tell, long value, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            Write(value);
        }

        public void WriteUInt64(ulong value)
        {
            Write(value);
        }

        public void WriteUInt64(long tell, ulong value, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            Write(value);
        }

        public void WriteBytes(byte[] buffer, int length = -1)
        {
            Stream.Write(buffer, 0, length == -1 ? buffer.Length : length);
        }

        public void WriteBytes(long tell, byte[] buffer, int length = -1, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            WriteBytes(buffer, length);
        }

        public void WritePaddingUntil(long endOffset)
        {
            var padding = (int)(endOffset - Tell());
            if (padding > 0) WriteNull(padding);
            Seek(endOffset);
        }

        public void WriteNull(int count)
        {
            Debug.Assert(count <= zeroes.Length);
            Stream.Write(zeroes, 0, count);
        }

        public void FillBytes(byte value, int length)
        {
            if (length < 0)
            {
                // throw new ArgumentOutOfRangeException(nameof(length), $"{nameof(length)} must > 0");
                return;
            }
            if (length < 256)
            {
                Span<byte> bytes = stackalloc byte[length];
                bytes.Fill(value);
                WriteSpan<byte>(bytes);
            }
            else
            {
                Span<byte> bytes = stackalloc byte[256];
                bytes.Fill(value);
                int count = length >> 8;
                for (int i = 0; i < count; i++)
                {
                    WriteSpan<byte>(bytes);
                }
                if ((length & 0xFF) != 0)
                {
                    WriteSpan<byte>(bytes.Slice(0, length & 0xFF));
                }
            }
        }

        public void FillBytes(long tell, byte value, int length, bool jumpBack = true)
        {
            using var defer = SeekJumpBack(tell, jumpBack);
            FillBytes(value, length);
        }

        public static string MarshalStringTrim(string text)
        {
            int n = text.IndexOf('\0');
            if (n != -1)
            {
                text = text.Substring(0, n);
            }
            return text;
        }

        private static string DecodeString(Span<byte> buffer, Encoding encoding)
        {
            return encoding.GetString(buffer);
        }

        public string ReadAsciiString(long pos = -1, int charCount = -1, bool jumpBack = true)
        {
            long originPos = Tell();
            if (pos != -1) Seek(pos);
            string? result = null;
            if (charCount > 1024)
            {
                throw new Exception($"{nameof(charCount)} {charCount} too large");
            }
            if (charCount == -1) charCount = 128;
            Span<byte> buffer = charCount <= 128 ? stackalloc byte[charCount] : new byte[charCount];
            int readCount = Stream.Read(buffer);
            if (readCount != 0)
            {
                int n = buffer.IndexOf((byte)0);
                if (n != -1)
                {
                    result = n == 0 ? "" : DecodeString(buffer.Slice(0, n), Encoding.ASCII);
                }
            }
            else
            {
                result = "";
            }
            if (result == null)
            {
                StringBuilder sb = new();
                sb.Append(DecodeString(buffer, Encoding.ASCII));
                do
                {
                    readCount = Stream.Read(buffer);
                    if (readCount != 0)
                    {
                        int n = buffer.IndexOf((byte)0);
                        sb.Append(DecodeString(n != -1 ? buffer.Slice(0, n) : buffer, Encoding.ASCII));
                        if (n != -1) break;
                    }
                } while (readCount == buffer.Length);
                result = sb.ToString();
            }
            Seek(jumpBack ? originPos : originPos + result.Length + 1);
            return result;
        }

        public bool WriteAsciiString(string text)
        {
            return WriteBytes(Encoding.ASCII.GetBytes(text)) && Write<byte>(0);
        }

        public string ReadWString(long pos = -1, int charCount = -1, bool jumpBack = true)
        {
            long originPos = Tell();
            if (pos != -1) Seek(pos);
            string? result = null;
            if (charCount > 1024 || charCount < -1)
            {
                throw new Exception($"{nameof(charCount)} {charCount} too large");
            }
            if (charCount == -1) charCount = 128;
            Span<char> buffer = charCount <= 128 ? stackalloc char[charCount] : new char[charCount];
            Span<byte> bytes = MemoryMarshal.AsBytes(buffer);
            int readCount = Stream.Read(bytes);
            if (readCount != 0)
            {
                int n = buffer.IndexOf((char)0);
                if (n != -1)
                {
                    result = n == 0 ? "" : buffer.Slice(0, n).ToString();
                }
            }
            else
            {
                result = "";
            }
            if (result == null)
            {
                StringBuilder sb = new();
                sb.Append(buffer);
                do
                {
                    readCount = Stream.Read(bytes);
                    if (readCount != 0)
                    {
                        int n = buffer.IndexOf((char)0);
                        sb.Append(n != -1 ? buffer.Slice(0, n): buffer);
                        if (n != -1) break;
                    }
                } while (readCount == bytes.Length);
                result = sb.ToString();
            }
            Seek(jumpBack ? originPos : originPos + result.Length * 2 + 2);
            return result;
        }

        public int ReadWStringLength(long pos = -1, int maxLen = -1, bool jumpBack = true)
        {
            long originPos = Tell();
            if (pos != -1) Seek(pos);
            int length = 0;
            char newByte = ReadChar();
            while (newByte != 0)
            {
                length++;
                newByte = ReadChar();
            }
            if (jumpBack) Seek(originPos);
            return length;
        }

        /// <summary>
        /// Reads a length-prefixed UTF-16 string with 4 byte padding at the end.
        /// </summary>
        public string ReadInlineWString()
        {
            var count = Read<int>();
            if (count == 0) return string.Empty;
            var str = ReadWString(-1, count, false);
            Align(4);
            return str;
        }

        public bool WriteWString(string text)
        {
            return WriteSpan(text.AsSpan()) && Write<ushort>(0);
        }

        /// <summary>
        /// Writes a length-prefixed UTF-16 string with 4 byte padding at the end.
        /// </summary>
        public bool WriteInlineWString(string text)
        {
            if (text.Length == 0) return Write(0);

            Write(text.Length + 1);
            WriteWString(text);
            var pos = (int)Tell();
            WriteNull(Utils.Align4(pos) - pos);
            return true;
        }

        public string ReadUTF8String(long pos = -1, bool jumpBack = true, int maxByteSize = 128)
        {
            long originPos = Tell();
            if (pos != -1) Seek(pos);
            string? result = null;
            if (maxByteSize > 1024)
            {
                throw new ArgumentException($"{maxByteSize} too large", nameof(maxByteSize));
            }
            Span<byte> buffer = maxByteSize <= 128 ? stackalloc byte[maxByteSize] : new byte[maxByteSize];
            int readCount = Stream.Read(buffer);
            int byteSize = 0;
            if (readCount != 0)
            {
                byteSize = buffer.IndexOf((byte)0);
                if (byteSize == -1)
                {
                    // making multiple fixed size buffer reads like the other string methods may break up in the middle of a UTF8 character, throw exception instead to be safe for now
                    throw new Exception("Could not determine UTF8 string length");
                }
                result = byteSize == 0 ? "" : DecodeString(buffer.Slice(0, byteSize), Encoding.UTF8);
            }
            else
            {
                result = "";
            }
            Seek(jumpBack ? originPos : originPos + byteSize + 1);
            return result;
        }

        public bool WriteUTF8String(string text)
        {
            return WriteBytes(Encoding.UTF8.GetBytes(text)) && Write<byte>(0);
        }

        public T Read<T>() where T : unmanaged
        {
            T value = default;
            Debug.Assert(Stream.Position < Stream.Length);
            Stream.Read(MemoryUtils.StructureAsBytes(ref value));
            return value;
        }

        public T Read<T>(long tell, bool jumpBack = true) where T : unmanaged
        {
            long pos = Tell();
            Seek(tell);
            T value = Read<T>();
            if (jumpBack) Seek(pos);
            return value;
        }

        public int Read<T>(ref T value) where T : unmanaged
        {
            return Stream.Read(MemoryUtils.StructureAsBytes(ref value));
        }

        public int Read<T>(long tell, ref T value, bool jumpBack = true) where T : unmanaged
        {
            long pos = Tell();
            Seek(tell);
            int result = Read(ref value);
            if (jumpBack) Seek(pos);
            return result;
        }

        public bool Write<T>(T value) where T : unmanaged
        {
            Stream.Write(MemoryUtils.StructureAsBytes(ref value));
            return true;
        }

        public bool Write<T>(long tell, T value, bool jumpBack = true) where T : unmanaged
        {
            long pos = Tell();
            Seek(tell);
            bool result = Write(value);
            if (jumpBack) Seek(pos);
            return result;
        }

        public bool Write<T>(ref T value) where T : unmanaged
        {
            Stream.Write(MemoryUtils.StructureAsBytes(ref value));
            return true;
        }

        public bool Write<T>(long tell, ref T value, bool jumpBack = true) where T : unmanaged
        {
            long pos = Tell();
            Seek(tell);
            bool result = Write(ref value);
            if (jumpBack) Seek(pos);
            return result;
        }

        public unsafe object ReadObject(Type type)
        {
            if (type == typeof(bool))
            {
                return ReadBoolean();
            }
            else if (type == typeof(byte))
            {
                return ReadByte();
            }
            else if (type == typeof(sbyte))
            {
                return ReadSByte();
            }
            int size = Marshal.SizeOf(type);
            byte[] buffer = ArrayPool<byte>.Shared.Rent(size);
            try
            {
                Stream.Read(buffer, 0, size);
                fixed (byte* p = buffer)
                {
                    return Marshal.PtrToStructure((IntPtr)p, type)!;
                }
            }
            finally { ArrayPool<byte>.Shared.Return(buffer); }
        }

        public unsafe bool WriteObject(object obj)
        {
            if (obj is bool boolValue)
            {
                WriteBoolean(boolValue);
                return true;
            }
            else if (obj is byte byteValue)
            {
                WriteByte(byteValue);
                return true;
            }
            else if (obj is sbyte sbyteValue)
            {
                WriteSByte(sbyteValue);
                return true;
            }
            int size = Marshal.SizeOf(obj);
            byte[] buffer = ArrayPool<byte>.Shared.Rent(size);
            try
            {
                fixed (byte* p = buffer)
                {
                    Marshal.StructureToPtr(obj, (IntPtr)p, false);
                }
                Stream.Write(buffer, 0, size);
                return true;
            }
            finally { ArrayPool<byte>.Shared.Return(buffer); }
        }

        /// <summary>
        /// 获取start..end之间的数据，长度在2-256
        /// 涉及unsafe操作，注意内存范围和对齐
        /// </summary>
        public static Span<byte> GetRangeSpan<TS, TE>(ref TS start, ref TE end) where TS : unmanaged where TE : unmanaged
        {
            unsafe
            {
                var startPtr = (nint)Unsafe.AsPointer(ref start);
                var endPtr = (nint)Unsafe.AsPointer(ref end) + Unsafe.SizeOf<TE>();
                int size = (int)(endPtr - startPtr);
                if (size < 2 || size > 256)
                {
                    throw new InvalidDataException($"Size {size} is out of range [2, 256]");
                }
                return new Span<byte>((void*)startPtr, size);
            }
        }

        /// <summary>
        /// 读取数据到start..end，长度在2-256
        /// 涉及unsafe操作，注意内存范围和对齐
        /// </summary>
        public int ReadRange<TS, TE>(ref TS start, ref TE end) where TS : unmanaged where TE : unmanaged
        {
            return Stream.Read(GetRangeSpan(ref start, ref end));
        }

        /// <summary>
        /// 写入start..end范围内的数据，长度在2-256
        /// 涉及unsafe操作，注意内存范围和对齐
        /// </summary>
        public bool WriteRange<TS, TE>(ref TS start, ref TE end) where TS : unmanaged where TE : unmanaged
        {
            Stream.Write(GetRangeSpan(ref start, ref end));
            return true;
        }

        public byte[] ReadBytes(int length)
        {
            byte[] buffer = new byte[length];
            Stream.Read(buffer);
            return buffer;
        }

        public bool WriteBytes(byte[] buffer)
        {
            Stream.Write(buffer);
            return true;
        }

        /// <summary>读取数组</summary>
        public T[] ReadArray<T>(int length) where T : unmanaged
        {
            if (length == 0) return [];
            T[] array = new T[length];
            Stream.Read(MemoryMarshal.AsBytes((Span<T>)array));
            return array;
        }

        /// <summary>读取数组</summary>
        public bool ReadArray<T>(T[] array) where T : unmanaged
        {
            Stream.Read(MemoryMarshal.AsBytes((Span<T>)array));
            return true;
        }

        /// <summary>读取二维数组</summary>
        public bool ReadArray<T>(T[,] array) where T : unmanaged
        {
            int length = array.GetLength(0) * array.GetLength(1);
            Stream.Read(MemoryMarshal.AsBytes(MemoryUtils.CreateSpan(ref array[0, 0], length)));
            return true;
        }

        /// <summary>读取数组</summary>
        public bool ReadArray<T>(T[] array, int start = 0, int length = -1) where T : unmanaged
        {
            if (length == -1 || length > array.Length - start)
            {
                length = array.Length - start;
            }
            Stream.Read(MemoryMarshal.AsBytes(array.AsSpan(start, length)));
            return true;
        }

        public void ReadOffsetArray<T>(ref T[] array, int length) where T : unmanaged
        {
            var offset = Read<long>();
            var pos = Tell();
            Seek(offset);
            if (array == null || array.Length != length) array = new T[length];
            if (length == 0) return;

            Stream.Read(MemoryMarshal.AsBytes((Span<T>)array));
            Seek(pos);
        }

        /// <summary>读取列表</summary>
        public bool ReadList<T>(List<T> list, int count) where T : unmanaged
        {
            for (int i = 0; i < count; i++)
            {
                list.Add(Read<T>());
            }
            return true;
        }

        /// <summary>读取数组</summary>
        public bool ReadSpan<T>(Span<T> span) where T : unmanaged
        {
            Stream.Write(MemoryMarshal.AsBytes(span));
            return true;
        }

        /// <summary>写入数组</summary>
        public bool WriteArray<T>(T[] array) where T : unmanaged
        {
            Stream.Write(MemoryMarshal.AsBytes((ReadOnlySpan<T>)array));
            return true;
        }

        /// <summary>写入数组</summary>
        public bool WriteArray<T>(T[] array, int start = 0, int length = -1) where T : unmanaged
        {
            if (length == -1 || length > array.Length - start)
            {
                length = array.Length - start;
            }
            Stream.Write(MemoryMarshal.AsBytes(new ReadOnlySpan<T>(array, start, length)));
            return true;
        }

        /// <summary>写入二维数组</summary>
        public bool WriteArray<T>(T[,] array) where T : unmanaged
        {
            int length = array.GetLength(0) * array.GetLength(1);
            Stream.Write(MemoryMarshal.AsBytes(MemoryUtils.CreateSpan(ref array[0, 0], length)));
            return true;
        }

        public void WriteOffsetArray<T>(T[] array, ref long offsetPosition) where T : unmanaged
        {
            Write(offsetPosition, Tell());
            offsetPosition += sizeof(long);
            WriteArray(array);
        }

        /// <summary>写入列表</summary>
        public bool WriteList<T>(List<T> list) where T : unmanaged
        {
            for (int i = 0; i < list.Count; i++)
            {
                Write(list[i]);
            }
            return true;
        }

        /// <summary>写入数组</summary>
        public bool WriteSpan<T>(ReadOnlySpan<T> span) where T : unmanaged
        {
            Stream.Write(MemoryMarshal.AsBytes(span));
            return true;
        }

        /// <summary>查找字节数组</summary>
        public long FindBytes(byte[] pattern, in SearchParam param = default)
        {
            const int PAGE_SIZE = 8192;
            byte[] buffer = ArrayPool<byte>.Shared.Rent(PAGE_SIZE);

            long addr = param.start;
            int sizeAligned = pattern.Length; // 数据大小对齐4字节

            if ((sizeAligned & 3) != 0)
            {
                sizeAligned += 4 - (sizeAligned & 3);
            }

            long end = param.end;
            if (end == -1)
            {
                end = FileSize();
            }

            searcher ??= new();
            searcher.Update(pattern, param.wildcard);

            while (addr < end)
            {
                int readCount = ReadBytes(addr, buffer, PAGE_SIZE);
                if (readCount != 0)
                {
                    int result = searcher.Search(buffer, 0, readCount, param.ordinal);
                    if (result != -1)
                    {
                        ArrayPool<byte>.Shared.Return(buffer);
                        return addr + result;
                    }
                }
                addr += PAGE_SIZE - sizeAligned;
            }

            ArrayPool<byte>.Shared.Return(buffer);
            return -1;
        }

        public long FindFirst(byte[] pattern, in SearchParam param = default)
        {
            return FindBytes(pattern, param);
        }

        public long FindFirst(string pattern, in SearchParam param = default, Encoding? encoding = null)
        {
            encoding ??= Encoding.Unicode;
            return FindBytes(encoding.GetBytes(pattern), param);
        }

        public long FindFirst<T>(T pattern, in SearchParam param = default) where T : unmanaged
        {
            return FindBytes(MemoryUtils.StructureRefToBytes(ref pattern), param);
        }

        public void InsertBytes(Span<byte> buffer, long position)
        {
            var stream = Stream;
            // 将当前位置保存到临时变量中
            long currentPosition = stream.Position;

            // 将流的位置设置为插入点
            stream.Position = position;

            // 将后续数据读取到临时缓冲区
            byte[] tempBuffer = new byte[stream.Length - position];
            int bytesRead = stream.Read(tempBuffer, 0, tempBuffer.Length);

            // 将插入数据写入到流中
            stream.Position = position;
            stream.Write(buffer);

            // 将后续数据写入到流中
            stream.Write(tempBuffer, 0, bytesRead);

            // 将流的位置恢复到原始位置
            stream.Position = currentPosition;
        }

        public string ReadOffsetAsciiString()
        {
            return ReadAsciiString(ReadInt64());
        }

        public void ReadOffsetAsciiString(out string text)
        {
            text = ReadAsciiString(ReadInt64());
        }

        public string ReadOffsetWString()
        {
            return ReadWString(ReadInt64());
        }

        public string? ReadOffsetWStringNullable()
        {
            var offs = ReadInt64();
            return offs == 0 ? null : ReadWString(offs);
        }

        public void ReadOffsetWString(out string text)
        {
            long offset = ReadInt64();
            text = offset == 0 ? string.Empty : ReadWString(offset);
        }

        public void WriteOffsetAsciiString(string text)
        {
            AsciiStringTableAdd(text);
            WriteInt64(0);
        }

        public void WriteOffsetWString(string text)
        {
            StringTableAdd(text);
            WriteInt64(0);
        }
        public void WriteOffsetGuidArray(Guid[] guids)
        {
            OffsetContentTableAdd((handler) => handler.WriteArray(guids));
            WriteInt64(0);
        }

        [return: NotNullIfNotNull(nameof(text))]
        public StringTableItem? StringTableAdd(string? text, bool addOffset = true)
        {
            if (text != null)
            {
                StringTable ??= new();
                return StringTable.Add(text, addOffset ? Tell() : -1);
            }
            return null;
        }

        /// <summary>
        /// 写入字符串表字符串和偏移
        /// </summary>
        public void StringTableFlush()
        {
            StringTable?.Flush(this);
        }

        /// <summary>
        /// 写入字符串表的字符串
        /// </summary>
        public void StringTableWriteStrings()
        {
            StringTable?.WriteStrings(this);
        }

        /// <summary>
        /// 写入字符串表的偏移，并清空数据
        /// </summary>
        public void StringTableFlushOffsets()
        {
            StringTable?.FlushOffsets(this);
        }

        public void WriteOffsetContent(Action<FileHandler> write)
        {
            OffsetContentTableAdd(write);
            Skip(sizeof(long));
        }

        [return: NotNullIfNotNull(nameof(text))]
        public StringTableItem? AsciiStringTableAdd(string? text, bool addOffset = true)
        {
            if (text != null)
            {
                AsciiStringTable ??= new() { IsAscii = true };
                return AsciiStringTable.Add(text, addOffset ? Tell() : -1);
            }
            return null;
        }

        /// <summary>
        /// 写入字符串表字符串和偏移
        /// </summary>
        public void AsciiStringTableFlush()
        {
            AsciiStringTable?.Flush(this);
        }

        /// <summary>
        /// 写入字符串表的字符串
        /// </summary>
        public void AsciiStringTableWriteStrings()
        {
            AsciiStringTable?.WriteStrings(this);
        }

        /// <summary>
        /// 写入字符串表的偏移，并清空数据
        /// </summary>
        public void AsciiStringTableFlushOffsets()
        {
            AsciiStringTable?.FlushOffsets(this);
        }

        public OffsetContent OffsetContentTableAdd(Action<FileHandler> write, bool addOffset = true)
        {
            OffsetContentTable ??= new();
            return OffsetContentTable.Add(write, addOffset ? Tell() : -1);
        }

        public void OffsetContentTableAddAlign(int align)
        {
            OffsetContentTable ??= new();
            OffsetContentTable.Add((handler) => handler.Align(align), -1);
        }

        /// <summary>
        /// 写入字符串表字符串和偏移
        /// </summary>
        public void OffsetContentTableFlush()
        {
            OffsetContentTable?.Flush(this);
        }

        /// <summary>
        /// 写入字符串表的字符串
        /// </summary>
        public void OffsetContentTableWriteContents()
        {
            OffsetContentTable?.WriteContents(this);
        }

        /// <summary>
        /// 写入字符串表的偏移，并清空数据
        /// </summary>
        public void OffsetContentTableFlushOffsets()
        {
            OffsetContentTable?.FlushOffsets(this);
        }
    }


    public class StringTableItem
    {
        public string Text { get; }
        /// <summary>
        /// 引用改字符串的偏移集合
        /// </summary>
        public HashSet<long> OffsetStart { get; } = new();
        /// <summary>
        /// 字符串在整个文件中的偏移
        /// </summary>
        public long TextStart { get; set; } = -1;
        /// <summary>
        /// 在字符串表中的偏移，约定按字符长度，而不是字节长度
        /// </summary>
        public int TableOffset { get; set; }

        public StringTableItem(string text)
        {
            Text = text;
        }

        public void Write(FileHandler handler, bool isAscii = false)
        {
            if (isAscii)
            {
                handler.WriteAsciiString(Text);
            }
            else
            {
                handler.WriteWString(Text);
            }
        }

        public override string ToString() => Text;
    }


    /// <summary>
    /// 待写入的字符串表
    /// </summary>
    public class StringTable : IEnumerable<StringTableItem>
    {
        private List<StringTableItem> Items { get; } = new();
        public Dictionary<string, StringTableItem> StringMap { get; } = new();
        public bool IsAscii { get; init; }
        public int Count => Items.Count;
        public int LastTableOffset { get; set; }

        public void Clear()
        {
            Items.Clear();
            StringMap.Clear();
            LastTableOffset = 0;
        }

        public StringTableItem Add(string text, long offset)
        {
            if (!StringMap.TryGetValue(text, out var item))
            {
                StringMap[text] = item = new(text) { TableOffset = LastTableOffset };
                Items.Add(item);
                LastTableOffset += text.Length + 1;
            }
            if (offset != -1)
            {
                item.OffsetStart.Add(offset);
            }
            return item;
        }

        public void Flush(FileHandler handler)
        {
            if (Count == 0) return;
            foreach (var item in Items)
            {
                item.TextStart = handler.Tell();
                foreach (var offsetStart in item.OffsetStart)
                {
                    handler.WriteInt64(offsetStart, item.TextStart);
                }
                handler.Seek(item.TextStart);
                item.Write(handler, IsAscii);
            }
            Clear();
        }

        public void WriteStrings(FileHandler handler)
        {
            if (Count == 0) return;
            foreach (var item in Items)
            {
                item.TextStart = handler.Tell();
                item.Write(handler, IsAscii);
            }
        }

        public void FlushOffsets(FileHandler handler)
        {
            if (Count == 0) return;
            foreach (var item in Items)
            {
                if (item.TextStart == -1)
                {
                    throw new Exception($"StringStart of {item.Text} not set");
                }
                foreach (var offsetStart in item.OffsetStart)
                {
                    handler.WriteInt64(offsetStart, item.TextStart);
                }
            }
            Clear();
        }

        public IEnumerator<StringTableItem> GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
    }


    public class OffsetContent
    {
        public long OffsetStart { get; set; }
        public long Offset { get; set; }
        public Action<FileHandler> Write { get; set; }

        public OffsetContent(Action<FileHandler> write)
        {
            Write = write;
        }
    }


    public class OffsetContentTable : IEnumerable<OffsetContent>
    {
        public List<OffsetContent> Items { get; } = new();
        public int Count => Items.Count;

        public void Clear()
        {
            Items.Clear();
        }

        public OffsetContent Add(Action<FileHandler> write, long offset)
        {
            var item = new OffsetContent(write)
            {
                OffsetStart = offset
            };
            Items.Add(item);
            return item;
        }

        public void Flush(FileHandler handler)
        {
            if (Count == 0) return;
            foreach (var item in Items)
            {
                if (item.OffsetStart == -1)
                {
                    item.Write(handler);
                }
                else
                {
                    item.Offset = handler.Tell();
                    handler.WriteInt64(item.OffsetStart, item.Offset);
                    item.Write(handler);
                }
            }
            Clear();
        }

        public void WriteContents(FileHandler handler)
        {
            if (Count == 0) return;
            foreach (var item in Items)
            {
                item.Offset = handler.Tell();
                item.Write(handler);
            }
        }

        public void FlushOffsets(FileHandler handler)
        {
            if (Count == 0) return;
            foreach (var item in Items)
            {
                if (item.Offset == -1)
                {
                    throw new Exception($"Offset of {item} not set");
                }
                handler.WriteInt64(item.OffsetStart, item.Offset);
            }
            Clear();
        }

        public IEnumerator<OffsetContent> GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
    }


    public interface IFileHandlerAction
    {
        FileHandler Handler { get; }
        bool Success { get; }
        bool Handle<T>(ref T value) where T : unmanaged;
        IFileHandlerAction Null(int count);
        IFileHandlerAction HandleOffsetWString([NotNull] ref string? value, bool writeEmptyAsNull = false);
    }


    public static class IFileHandlerActionExtension
    {
        public static IFileHandlerAction Do<T>(this IFileHandlerAction action, ref T value) where T : unmanaged
        {
            action.Handle(ref value);
            return action;
        }

        public static IFileHandlerAction Do<T>(this IFileHandlerAction action, bool condition, ref T value) where T : unmanaged
        {
            return condition ? action.Do(ref value) : action;
        }

        public static IFileHandlerAction Skip(this IFileHandlerAction action, long skip)
        {
            action.Handler.Skip(skip);
            return action;
        }

        public static IFileHandlerAction? Then<T>(this IFileHandlerAction action, ref T value) where T : unmanaged
        {
            if (action.Handle(ref value))
            {
                return action;
            }
            return null;
        }

        public static IFileHandlerAction? Then<T>(this IFileHandlerAction action, bool condition, ref T value) where T : unmanaged
        {
            return condition ? action.Then(ref value) : action;
        }
    }


    public struct FileHandlerRead(FileHandler handler) : IFileHandlerAction
    {
        public FileHandler Handler { get; set; } = handler;
        public int LastResult { get; set; } = -1;
        public readonly bool Success => LastResult != 0;

        public bool Handle<T>(ref T value) where T : unmanaged
        {
            LastResult = Handler.Read(ref value);
            return Success;
        }

        public readonly IFileHandlerAction HandleOffsetWString([NotNull] ref string? value, bool writeEmptyAsNull)
        {
            Handler.ReadOffsetWString(out value);
            return this;
        }

        public IFileHandlerAction Null(int count)
        {
            Handler.ReadNull(count);
            return this;
        }
    }


    public struct FileHandlerWrite(FileHandler handler) : IFileHandlerAction
    {
        public FileHandler Handler { get; set; } = handler;
        public bool Success { get; set; }

        public bool Handle<T>(ref T value) where T : unmanaged
        {
            Success = Handler.Write(ref value);
            return Success;
        }

        public readonly IFileHandlerAction HandleOffsetWString([NotNull] ref string? value, bool writeEmptyAsNull)
        {
            if (!writeEmptyAsNull || !string.IsNullOrEmpty(value))
            {
                Handler.WriteOffsetWString(value ??= "");
            }
            else
            {
                Handler.Write(0L);
                value ??= null!;
            }
            return this;
        }

        public IFileHandlerAction Null(int count)
        {
            Handler.WriteNull(count);
            return this;
        }
    }
}
