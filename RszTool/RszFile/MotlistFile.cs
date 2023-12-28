using System.Numerics;
using RszTool.Motlist;

namespace RszTool.Motlist
{
    public enum MotlistVersion
    {
        RE7 = 60,
        RE2_DMC5 = 85,
        RE3 = 99,
        RE8 = 484,
    }

    public class Header : BaseModel
    {
        public uint version;
        public uint magic;
        public long padding;
        public long pointersOffset; // AssetsPointer in Tyrant
        public long colOffset; // UnkPointer
        public long motListNameOffset;
        public long Unkpadding;
        public int numOffset;

        public MotlistVersion Version { get; set; }
        public string MotListName { get; set; } = string.Empty;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref version);
            handler.Read(ref magic);
            handler.Read(ref padding);
            handler.Read(ref pointersOffset);
            handler.Read(ref colOffset);
            handler.Read(ref motListNameOffset);
            Version = (MotlistVersion)version;
            if (Version >= MotlistVersion.RE8)
            {
                handler.Read(ref Unkpadding);
            }
            handler.Read(ref numOffset);
            MotListName = handler.ReadWString(motListNameOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref version);
            handler.Write(ref magic);
            handler.Write(ref padding);
            handler.Write(ref pointersOffset);
            handler.Write(ref colOffset);
            handler.WriteOffsetWString(MotListName);
            Version = (MotlistVersion)version;
            if (Version >= MotlistVersion.RE8)
            {
                handler.Write(ref Unkpadding);
            }
            handler.Write(ref numOffset);
            return true;
        }
    }


    public class MotIndex : BaseModel
    {
        public uint unk1;
        public uint unk2;
        public ushort motNumber;
        public ushort Switch;
        uint[]? data;

        public MotlistVersion Version { get; set; }

        public MotIndex(MotlistVersion version)
        {
            Version = version;
        }

        public int DataCount => Version >= MotlistVersion.RE8 ? 15 : Version == MotlistVersion.RE7 ? 0 : 3;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref unk1);
            handler.Read(ref unk2);
            handler.Read(ref motNumber);
            handler.Read(ref Switch);
            int dataCount = DataCount;
            if (dataCount > 0)
            {
                data = handler.ReadArray<uint>(dataCount);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref unk1);
            handler.Write(ref unk2);
            handler.Write(ref motNumber);
            handler.Write(ref Switch);
            if (data != null)
            {
                handler.WriteArray(data);
            }
            return true;
        }
    }


    public class MotHeader : ReadWriteModel
    {
        public uint version;
        public uint magic;
        public uint motSize;
        public long boneHeaderOffsetStart; // BoneBaseDataPointer
        public long boneClipHeaderOffset; // BoneDataPointer

        public long clipFileOffset;
        public long jmapOffset;
        public long Offset2; // Extra Data Offset

        // public long namesOffset;
        public float frameCount;
        public float blending; // Set to 0 to enable repeating
        public float uknFloat1;
        public float uknFloat2;
        public ushort boneCount;
        public ushort boneClipCount;
        public byte clipCount;
        public byte uknPointer3Count;
        public ushort FrameRate;
        public ushort uknPointerCount;
        public ushort uknShort;

        public string motName = string.Empty;

        protected override bool ReadWrite(IFileHandlerAction action)
        {
            action.Then(ref version)
                 ?.Then(ref magic)
                 ?.Then(ref motSize)
                 ?.Then(ref boneHeaderOffsetStart)
                 ?.Then(ref boneClipHeaderOffset)
                 ?.Skip(8);
            if (version > 456)
            {
                action.Skip(8)
                     ?.Then(ref clipFileOffset)
                     ?.Then(ref jmapOffset)
                     ?.Then(ref Offset2);
            }
            else
            {
                action.Then(ref jmapOffset)
                     ?.Then(ref clipFileOffset)
                     ?.Skip(16)
                     ?.Then(ref Offset2);
            }
            action.HandleOffsetWString(ref motName)
                 ?.Then(ref frameCount)
                 ?.Then(ref blending)
                 ?.Then(ref uknFloat1)
                 ?.Then(ref uknFloat2)
                 ?.Then(ref boneCount)
                 ?.Then(ref boneClipCount)
                 ?.Then(ref clipCount)
                 ?.Then(ref uknPointer3Count)
                 ?.Then(ref FrameRate)
                 ?.Then(ref uknPointerCount)
                 ?.Then(ref uknShort);
            return true;
        }
    }


    public class BoneHeader : ReadWriteModel
    {
        public string boneName = string.Empty;
        public ulong parentOffs;
        public ulong childOffs;
        public ulong nextSiblingOffs;
        public Vector4 translation;
        public Quaternion quaternion;
        public uint Index;
        public uint boneHash;
        public ulong padding;

        protected override bool ReadWrite(IFileHandlerAction action)
        {
            action.HandleOffsetWString(ref boneName)
                 ?.Then(ref parentOffs)
                 ?.Then(ref childOffs)
                 ?.Then(ref nextSiblingOffs)
                 ?.Then(ref translation)
                 ?.Then(ref quaternion)
                 ?.Then(ref Index)
                 ?.Then(ref boneHash)
                 ?.Then(ref padding);
            return action.Success;
        }
    }


    public class BoneHeaders : BaseModel
    {
        public long boneHeaderOffset;
        public long boneHeaderCount;

        public List<BoneHeader> BoneHeaderList { get; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref boneHeaderOffset);
            handler.Read(ref boneHeaderCount);

            if (boneHeaderCount > 1000) throw new InvalidDataException($"boneHeaderCount {boneHeaderCount} > 1000");
            BoneHeaderList.Clear();
            BoneHeaderList.Read(handler, (int)boneHeaderCount);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            boneHeaderCount = BoneHeaderList.Count;
            handler.Write(ref boneHeaderOffset);
            handler.Write(ref boneHeaderCount);
            BoneHeaderList.Write(handler);
            handler.StringTableFlush();
            return true;
        }
    }


    [Flags]
    public enum TrackFlag : ushort
    {
        None,
        Translation = 1,
        Rotation = 2,
        Scale = 4
    }


    public class BoneClipHeader : BaseModel
    {
        public ushort boneIndex;
        public TrackFlag trackFlags;
        public uint boneHash;  // MurMur3
        public float uknFloat;  // always 1.0?
        public uint padding;
        public long trackHeaderOffset; // keysPointer

        public uint MotVersion { get; set; }

        public BoneClipHeader(uint motVersion)
        {
            MotVersion = motVersion;
        }

        protected override bool DoRead(FileHandler handler)
        {
            if (MotVersion == 65)
            {
                handler.ReadRange(ref boneIndex, ref trackHeaderOffset);
            }
            else
            {
                handler.Read(ref boneIndex);
                handler.Read(ref trackFlags);
                handler.Read(ref boneHash);
                if (MotVersion == 43)
                    handler.Read(ref trackHeaderOffset);
                else
                    trackHeaderOffset = handler.ReadUInt();
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (MotVersion == 65)
            {
                handler.WriteRange(ref boneIndex, ref trackHeaderOffset);
            }
            else
            {
                handler.Write(ref boneIndex);
                handler.Write(ref trackFlags);
                handler.Write(ref boneHash);
                if (MotVersion == 43)
                    handler.Write(ref trackHeaderOffset);
                else
                    handler.WriteUInt((uint)trackHeaderOffset);
            }
            return true;
        }
    }


    public class Track : BaseModel
    {
        public uint flags;
        public int keyCount;
        public uint frameRate;
        public float maxFrame;
        public long frameIndOffset;
        public long frameDataOffset;
        public long unpackDataOffset;

        public uint MotVersion { get; set; }

        public Track(uint motVersion)
        {
            MotVersion = motVersion;
        }

        public byte CompressionType => (byte)(flags >> 20);
        public uint KeyFrameDataType => flags & 0xF00000;
        public uint Compression => flags & 0xFF000;
        public uint UnkFlag => flags & 0xFFF;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref flags);
            handler.Read(ref keyCount);
            if (MotVersion >= 78)
            {
                frameIndOffset = handler.ReadUInt();
                frameDataOffset = handler.ReadUInt();
                unpackDataOffset = handler.ReadUInt();
            }
            else
            {
                handler.Read(ref frameRate);
                handler.Read(ref maxFrame);
                handler.Read(ref frameIndOffset);
                handler.Read(ref frameDataOffset);
                handler.Read(ref unpackDataOffset);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public enum TranslationDecompression
    {
        UnknownType,
        LoadVector3sFull,
        LoadVector3s5BitA,
        LoadVector3s10BitA,
        LoadVector3s21BitA,
        LoadVector3sXAxis,
        LoadVector3sYAxis,
        LoadVector3sZAxis,
        LoadVector3sXAxis16Bit,
        LoadVector3sYAxis16Bit,
        LoadVector3sZAxis16Bit,
        LoadVector3s5BitB,
        LoadVector3s10BitB,
        LoadVector3s21BitB,
        LoadVector3sXYZAxis16Bit,
        LoadVector3sXYZAxis,
    }


    public enum RotationDecompression
    {
        UnknownType,
        LoadQuaternionsFull,
        LoadQuaternions3Component,
        LoadQuaternions10Bit,
        LoadQuaternions16Bit,
        LoadQuaternions21Bit,
        LoadQuaternionsXAxis16Bit,
        LoadQuaternionsYAxis16Bit,
        LoadQuaternionsZAxis16Bit,
        LoadQuaternionsXAxis,
        LoadQuaternionsYAxis,
        LoadQuaternionsZAxis,
        LoadQuaternions5Bit,
        LoadQuaternions8Bit,
        LoadQuaternions13Bit,
        LoadQuaternions18Bit,
    }


    public enum ScaleDecompression
    {
        UnknownType,
        LoadVector3sFull,
        LoadVector3s5BitA,
        LoadVector3s10BitA,
        LoadScalesXYZ,
        LoadVector3s21BitA,
        LoadVector3sXAxis,
        LoadVector3sYAxis,
        LoadVector3sZAxis,
        LoadVector3sXAxis16Bit,
        LoadVector3sYAxis16Bit,
        LoadVector3sZAxis16Bit,
    }


    public class TRACKS : BaseModel
    {
        public Track? Translation { get; set; }
        public Track? Rotation { get; set; }
        public Track? Scale { get; set; }

        protected override bool DoRead(FileHandler handler)
        {
            throw new NotImplementedException();
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }
}


namespace RszTool
{
    public class MotlistFile(RszFileOption option, FileHandler fileHandler) : BaseRszFile(option, fileHandler)
    {
        public const uint Magic = 0x74736C6D;
        public const string Extension = ".motlist";

        protected override bool DoRead()
        {
            throw new NotImplementedException();
        }

        protected override bool DoWrite()
        {
            throw new NotImplementedException();
        }
    }


    public class MotFile(RszFileOption option, FileHandler fileHandler) : BaseRszFile(option, fileHandler)
    {
        public const uint Magic = 0x20746F6D;
        public const string Extension = ".mot";

        public MotHeader Header { get; } = new();
        public BoneHeaders BoneHeaders { get; } = new();
        public List<BoneClipHeader> BoneClipHeaders { get; } = new();

        protected override bool DoRead()
        {
            BoneClipHeaders.Clear();

            var handler = FileHandler;
            var header = Header;
            if (!Header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a MOT file");
            }

            if (header.boneHeaderOffsetStart > 0 && header.motSize == 0 || header.motSize > header.boneHeaderOffsetStart)
            {
                handler.Seek(header.boneHeaderOffsetStart);
                BoneHeaders.Read(handler);
            }
            
            if (header.boneClipCount > 0)
            {
                handler.Seek(header.boneClipHeaderOffset);
                for (int i = 0; i < header.boneClipCount; i++)
                {
                    BoneClipHeader item = new(header.version);
                    item.Read(handler);
                    BoneClipHeaders.Add(item);
                }
                handler.Align(16);
                foreach (var item in BoneClipHeaders)
                {
                    handler.Seek(item.trackHeaderOffset);
                }
            }
            return true;
        }

        protected override bool DoWrite()
        {
            throw new NotImplementedException();
        }
    }
}
