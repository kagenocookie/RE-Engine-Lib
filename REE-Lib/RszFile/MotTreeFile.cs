using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using ReeLib.Clip;
using ReeLib.Common;
using ReeLib.InternalAttributes;
using ReeLib.Mot;
using ReeLib.MotTree;

namespace ReeLib.MotTree
{
    public enum MotTreeVersion
    {
        RE2 = 4,
        RE3 = 5,
        RE8 = 10,
        RE_RT = 13,
        RE4 = 19,
        DD2 = 20,
    }

    public enum MotionTreeParamType
    {
        Bool = 0x0,
        Int8 = 0x1,
        UInt8 = 0x2,
        Int16 = 0x3,
        UInt16 = 0x4,
        Int32 = 0x5,
        UInt32 = 0x6,
        Int64 = 0x7,
        Uint64 = 0x8,
        Float = 0x9,
        Double = 0xA,
        StrAscii = 0xB,
        StrUTF16 = 0xC,
        ExtraData = 0xD,
        Hermite = 0xE,
        Guid = 0xF,
        Vec2 = 0x10,
        Vec3 = 0x11,
        Vec4 = 0x12,
        Matrix = 0x13,
    }

    public enum MotionTreeLinkType
    {
        Unknown = 0,
        Motion = 1,
        Param = 2,
    }

    public class MotionTreeNodeParameter
    {
        public string? name;
        public MotionTreeParamType type;
        public uint hash;
        public object? value;

        internal void Read(FileHandler handler, MotTreeVersion version)
        {
            if (version <= MotTreeVersion.RE8) {
                name = handler.ReadOffsetAsciiString();
            }
            handler.Read(ref type);
            handler.Read(ref hash);
            ReadValue(handler, handler.Read<long>());
        }

        internal void Write(FileHandler handler, MotTreeVersion version)
        {
            if (version <= MotTreeVersion.RE8) {
                handler.WriteOffsetAsciiString(name ?? "");
            }
            handler.Write(ref type);
            handler.Write(ref hash);
            WriteValue(handler);
            // handler.WriteOffsetContent(hh => WriteValue(hh));
        }

        internal void ReadValue(FileHandler handler, long valueOrOffset)
        {
            switch (type)
            {
                case MotionTreeParamType.Bool:
                    value = valueOrOffset != 0;
                    break;
                case MotionTreeParamType.Int32:
                    value = (int)valueOrOffset;
                    break;
                case MotionTreeParamType.UInt32:
                    value = (uint)valueOrOffset;
                    break;
                case MotionTreeParamType.Float:
                    value = BitConverter.Int32BitsToSingle((int)valueOrOffset);
                    break;
                case MotionTreeParamType.StrUTF16:
                    value = handler.ReadWString(valueOrOffset);
                    break;
                case MotionTreeParamType.ExtraData:
                    {
                        using var _ = handler.SeekJumpBack(valueOrOffset);
                        value = handler.ReadArray<byte>((int)hash);
                    }
                    break;
                default:
                    throw new NotImplementedException("Unsupported motree param type " + type);
            }
        }

        internal void WriteValue(FileHandler handler)
        {
            switch (type)
            {
                case MotionTreeParamType.Bool:
                    handler.Write((bool)value!);
                    break;
                case MotionTreeParamType.Int32:
                    handler.Write((int)value!);
                    break;
                case MotionTreeParamType.UInt32:
                    handler.Write((uint)value!);
                    break;
                case MotionTreeParamType.Float:
                    handler.Write((float)value!);
                    break;
                case MotionTreeParamType.StrUTF16:
                    handler.WriteOffsetWString((string?)value ?? "");
                    break;
                case MotionTreeParamType.ExtraData:
                    {
                        var arr = (byte[])value!;
                        if (arr.Length != hash) throw new Exception("Invalid motion tree parameter: hash must be equal to data array size");
                        handler.WriteOffsetContent((hh) => {
                            hh.WriteArray(arr);
                            hh.Align(16);
                        });
                    }
                    break;
                default:
                    throw new NotImplementedException("Unsupported motree param type " + type);
            }
            handler.Align(8);
        }

        public override string ToString() => $"[{name ?? hash.ToString()}]: {value}";
    }

    public enum MotionTreeNodeType : byte
    {
        Unknown = 0,
        GameObject = 1,
        Component = 2,
        Folder = 3,
    }

    public class MotionTreeNode : BaseModel
    {
        public MotTreeVersion version;

        public MotionTreeNodeType nodeType;
        public string typeName = "";
        public string name = "";
        public uint nameHash;
        public uint typeHash;
        public uint extraHash1;
        public uint extraHash2;
        public byte flags;

        // name hash examples:
        // 3868588456: Motion
        // 2732277690: Variables
        // 2814588301: Normalize

        public List<string> Tags { get; } = new(0);

        public List<MotionTreeNodeParameter> Parameters { get; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            if (version <= MotTreeVersion.RE8)
            {
                handler.ReadOffsetAsciiString(out typeName);
            }

            handler.ReadOffsetWString(out name);

            var tagOffsetsOffset = handler.Read<long>();
            var tagHashTableOffsetMaybe = handler.Read<long>();
            var dataOffset = handler.Read<long>();
            handler.Read(ref typeHash);
            handler.Read(ref nameHash);
            handler.Read(ref extraHash1);
            if (version == MotTreeVersion.RE_RT) handler.Read(ref extraHash2);

            var tagCount = handler.Read<byte>();
            var paramCount = handler.Read<byte>();
            handler.Read(ref nodeType);
            handler.Read(ref flags);
            if (version == MotTreeVersion.RE_RT) handler.ReadNull(4);

            if (tagCount > 0)
            {
                using var __ = handler.SeekJumpBack(tagOffsetsOffset);
                handler.Seek(tagOffsetsOffset);
                for (int i = 0; i < tagCount; ++i) Tags.Add(handler.ReadOffsetWString());
                // alpha's template has these marked as hashes, but they seem to just be 0s and always just 1 of them
                // maybe in mh games
                handler.Seek(tagHashTableOffsetMaybe);
                handler.ReadNull(4 * tagCount);
            }

            using var _ = handler.SeekJumpBack(dataOffset);
            for (int i = 0; i < paramCount; ++i) {
                var param = new MotionTreeNodeParameter();
                param.Read(handler, version);
                Parameters.Add(param);
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (version <= MotTreeVersion.RE8)
            {
                handler.Skip(8); // typeName
                // handler.ReadOffsetAsciiString(out typeName);
            }

            handler.Skip(8); // name
            if (Tags.Count > 0)
            {
                // note: untested correctness for count > 0
                handler.WriteOffsetContent((hh) => {
                    var start = hh.Tell();

                    handler.Skip(Tags.Count * sizeof(long)); // skip string offsets
                    handler.Skip(Tags.Count * sizeof(uint)); // skip string hashes
                    handler.Align(8);
                    foreach (var tag in Tags) {
                        hh.Write(start, handler.Tell());
                        hh.WriteWString(tag);
                        start += sizeof(long);
                    }
                });
                handler.WriteOffsetContent((hh) => {
                    foreach (var tag in Tags) handler.Write(0);
                    handler.Align(8);
                });
            }
            else
            {
                handler.WriteNull(16);
            }
            handler.Skip(8); // var dataOffset = handler.Write<long>();
            handler.Write(ref typeHash);
            handler.Write(ref nameHash);
            handler.Write(ref extraHash1);
            if (version == MotTreeVersion.RE_RT) handler.Write(ref extraHash2);

            handler.Write((byte)Tags.Count);
            handler.Write((byte)Parameters.Count);
            handler.Write(ref nodeType);
            handler.Write(ref flags);
            if (version == MotTreeVersion.RE_RT) handler.WriteNull(4);

            return true;
        }

        public void WriteTypes(FileHandler handler)
        {
            if (version <= MotTreeVersion.RE8)
            {
                handler.Write(Start, handler.Tell());
            }

            handler.WriteAsciiString(typeName);
        }

        public void WriteNames(FileHandler handler)
        {
            handler.Write(Start + (version <= MotTreeVersion.RE8 ? 8 : 0), handler.Tell());
            handler.WriteWString(name);
        }

        public void WriteParameters(FileHandler handler)
        {
            handler.Write(Start + 24 + (version <= MotTreeVersion.RE8 ? 8 : 0), handler.Tell());
            foreach (var param in Parameters)
            {
                param.Write(handler, version);
            }
        }

        public override string ToString() => $"[{typeName}] {(string.IsNullOrEmpty(name) ? nameHash.ToString() : name)} ({nodeType})";
    }

    [RszGenerate]
    public partial class MotionTreeLink : BaseModel
    {
        public int inputNodeIndex;
        public int inputPinNo;
        public int outputNodeIndex;
        public int outputPinNo;
        [RszPaddingAfter(4)]
        public MotionTreeLinkType type;

        [RszIgnore] public Guid inputGuid;
        [RszIgnore] public Guid outputGuid;

        protected override bool DoRead(FileHandler handler)
        {
            DefaultRead(handler);
            var inputGuidOffset = handler.Read<long>();
            var outputGuidOffset = handler.Read<long>();
            if (inputGuidOffset > 0) inputGuid = handler.Read<Guid>(inputGuidOffset);
            if (outputGuidOffset > 0) outputGuid = handler.Read<Guid>(outputGuidOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            DefaultWrite(handler);
            if (inputGuid == Guid.Empty) handler.Write(0L); else handler.WriteOffsetContent((hh) => hh.Write(inputGuid));
            if (outputGuid == Guid.Empty) handler.Write(0L); else handler.WriteOffsetContent((hh) => hh.Write(outputGuid));
            return true;
        }

        public override string ToString() => $"{type} {inputNodeIndex} => {outputNodeIndex}";
    }

    public struct TreeIndexPair
    {
        public int source;
        public int target;

        public override string ToString() => $"{source} => {target}";
    }
}


namespace ReeLib
{
    public class MotTreeFile(FileHandler fileHandler)
        : MotFileBase(fileHandler)
    {
        public const uint Magic = 0x6572746D;
        public const string Extension = ".mottree";

        public override KnownFileFormats MotType => KnownFileFormats.MotionTree;
        public override string Name { get; set; } = string.Empty;

        public MotTreeVersion version;

        public string UvarPath { get; set; } = string.Empty;
        public string ResourcePath { get; set; } = string.Empty;
        public List<TreeIndexPair> Indices { get; } = new();
        public List<TreeIndexPair> MotionIDRemaps { get; } = new();
        public List<MotionTreeNode> Nodes { get; } = new();
        public List<MotionTreeLink> Links { get; } = new();

        public short uknCount1;
        public short uknCount2;
        public short expandedMotionsCount;

        protected override bool DoRead()
        {
            Indices.Clear();
            MotionIDRemaps.Clear();
            var handler = FileHandler;
            handler.Read(ref version);
            if (handler.Read<int>() != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a motion tree file");
            }
            handler.ReadNull(8);
            UvarPath = handler.ReadOffsetWStringNullable() ?? "";
            ResourcePath = handler.ReadOffsetWStringNullable() ?? "";

            var nodeOffset = handler.Read<long>();
            var linkDataOffset = handler.Read<long>();
            var nameOffset = handler.Read<long>();
            Name = handler.ReadWString(nameOffset);

            long indicesOffset = 0;
            if (version > MotTreeVersion.RE_RT)
            {
                indicesOffset = handler.Read<long>();
            }
            var nodeIdTableOffset = handler.Read<long>();
            var nodeCount = handler.Read<short>();
            var linkCount = handler.Read<short>();
            handler.Read(ref uknCount1);
            handler.Read(ref uknCount2);
            var basicMotionsCount = handler.Read<short>();

            // "expandedMotionsCount" includes sub motion counts (e.g. SelectMotionNode can have 2~any amount of motions itself), always (?) zero in earlier games
            // always 0 for <= RE_RT
            handler.Read(ref expandedMotionsCount);

            handler.ReadNull(4);

            // TODO figure out exactly why or how the counts here work
            // reference: dd2 ch00_000_com_tree StartMoveRight (18 + 14 + 22 nodes + 27 links)
            // one other case has real count 9 while SelectMotionNode has itself 9 + another normal MotionNode for a grand total of 10 actual motions...
            if (indicesOffset > 0)
            {
                handler.Seek(indicesOffset);
                var count = (int)((nodeIdTableOffset - indicesOffset) / 4);
                for (int i = 0; i < count; ++i)
                {
                    Indices.Add(new TreeIndexPair() {
                        source = handler.Read<short>(),
                        target = handler.Read<short>(),
                    });
                }
            }

            if (basicMotionsCount > 0 && nodeIdTableOffset > 0)
            {
                handler.Seek(nodeIdTableOffset);
                // Motion = +1 node
                // SelectMotion = +2~any nodes
                var nodeSize = version switch {
                    < MotTreeVersion.RE_RT => 4,
                    < MotTreeVersion.RE4 => 6,
                    _ => 8,
                };
                var realCount = (nameOffset - nodeIdTableOffset) / nodeSize;
                for (int i = 0; i < realCount; ++i)
                {
                    var pair = new TreeIndexPair();
                    pair.target = version < MotTreeVersion.RE_RT ? handler.Read<short>() : handler.Read<int>();
                    pair.source = version < MotTreeVersion.RE4 ? handler.Read<short>() : handler.Read<int>();
                    MotionIDRemaps.Add(pair);
                }
                handler.Align(8);
                DataInterpretationException.ThrowIfDifferent(handler.Tell(), nameOffset);
            }

            Nodes.Clear();
            Links.Clear();
            handler.Seek(nodeOffset);
            for (int i = 0; i < nodeCount; ++i)
            {
                var node = new MotionTreeNode() { version = version };
                node.Read(handler);
                Nodes.Add(node);
            }

            if (version > MotTreeVersion.RE8)
            {
                foreach (var node in Nodes)
                {
                    node.typeName = handler.ReadAsciiString(jumpBack: false);
                }
            }

            handler.Seek(linkDataOffset);
            Links.Read(handler, linkCount);

            handler.Align(16);
            handler.OffsetContentTableFlush();

            return true;
        }

        protected override bool DoWrite()
        {
            FileHandler handler = FileHandler;

            handler.Write(ref version);
            handler.Write(Magic);
            handler.WriteNull(8);
            if (string.IsNullOrEmpty(UvarPath)) handler.WriteNull(8); else handler.WriteOffsetWString(UvarPath);
            if (string.IsNullOrEmpty(ResourcePath)) handler.WriteNull(8); else handler.WriteOffsetWString(ResourcePath);
            var nodeOffsetOffset = handler.Tell();
            handler.Skip(24); // nodeOffset, linksOffset, nameOffset
            if (version > MotTreeVersion.RE_RT)
            {
                handler.Skip(8); // indicesOffset
            }
            var idTableOffset = handler.Tell();
            handler.Skip(8);

            handler.Write((short)Nodes.Count);
            handler.Write((short)Links.Count);
            handler.Write(ref uknCount1);
            handler.Write(ref uknCount2);
            handler.Write((short)Nodes.Count(n => n.typeName == "MotionNode")); // basicMotionsCount
            handler.Write(ref expandedMotionsCount);
            handler.WriteNull(4);

            if (version > MotTreeVersion.RE_RT)
            {
                handler.Align(8);
                handler.Write(nodeOffsetOffset + 24, handler.Tell());
                Indices.Write(handler);
            }

            handler.Align(8);
            handler.Write(idTableOffset, handler.Tell());
            foreach (var ind in MotionIDRemaps)
            {
                if (version < MotTreeVersion.RE_RT) handler.Write((short)ind.target); else handler.Write(ind.target);
                if (version < MotTreeVersion.RE4) handler.Write((short)ind.source); else handler.Write(ind.source);
            }
            handler.Align(8);
            handler.Write(nodeOffsetOffset + 16, handler.Tell());
            handler.WriteWString(Name);

            handler.Align(8);
            handler.Write(nodeOffsetOffset, handler.Tell());
            foreach (var node in Nodes) node.Write(handler);
            foreach (var node in Nodes) node.WriteTypes(handler);
            foreach (var node in Nodes) node.WriteNames(handler);

            handler.Align(8);
            foreach (var node in Nodes) node.WriteParameters(handler);
            handler.AsciiStringTableFlush();
            handler.StringTableFlush();
            handler.Align(16);
            handler.OffsetContentTableFlush();

            handler.Align(16);
            handler.Write(nodeOffsetOffset + 8, handler.Tell());
            foreach (var link in Links) link.Write(handler);
            handler.Align(16);
            handler.OffsetContentTableFlush();

            return false;
        }

        public override string ToString() => Name;
    }
}
