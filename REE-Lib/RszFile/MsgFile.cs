using ReeLib.Common;
using ReeLib.Msg;

using Header = ReeLib.StructModel<ReeLib.Msg.HeaderStruct>;

namespace ReeLib.Msg
{
    public enum Language
    {
        Japanese,
        English,
        French,
        Italian,
        German,
        Spanish,
        Russian,
        Polish,
        Dutch,
        Portuguese,
        PortugueseBr,
        Korean,
        TraditionalChinese,  // only this
        SimplifiedChinese,  // and this
        Finnish,
        Swedish,
        Danish,
        Norwegian,
        Czech,
        Hungarian,
        Slovak,
        Arabic,
        Turkish,
        Bulgarian,
        Greek,
        Romanian,
        Thai,
        Ukrainian,
        Vietnamese,
        Indonesian,
        Fiction,
        Hindi,
        LatinAmericanSpanish,
        Max,
    }


    public enum AttributeValueType
    {
        NullString = -1,
        Long,
        Double,
        String,
    }


    public class AttributeItem
    {
        public string Name { get; set; } = string.Empty;
        public AttributeValueType ValueType { get; set; }
    }


    public struct HeaderStruct
    {
        public uint version;
        public uint magic;  // GMSG
        public long headerOffset;  // 0x10
        public int entryCount;
        public int attributeCount;
        public int langCount;
        public uint padding;  // align to 8
        public long dataOffset;  // encrypted string pool
        public long unknDataOffset;  // seems to be always zero
        public long langOffset;
        public long attributeOffset;
        public long attributeNameOffset;
    }


    public class SubEntryHeader(Header header) : BaseModel
    {
        public Guid guid;
        public uint unknown;
        public uint hashOrIndex;
        public string entryName = string.Empty;
        public long attributeOffset;
        public object? attributeValue;
        public List<long>? ContentOffsetsByLangs { get; set; }

        public Header MsgHeader { get; set; } = header;

        // patch back to attributeOffset
        public long AttributeOffsetStart { get; private set; } = -1;
        public long ContentOffsetsByLangsStart { get; private set; } = -1;

        /// <summary>
        /// True if hashOrIndex is murmurhash3 of the entry Name
        /// </summary>
        public bool IsHash => MsgHeader.Data.version > 15;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guid);
            handler.Read(ref unknown);
            handler.Read(ref hashOrIndex);
            handler.ReadOffsetWString(out entryName);
            handler.Read(ref attributeOffset);

            ContentOffsetsByLangs ??= new();
            ContentOffsetsByLangs.Clear();
            handler.ReadList(ContentOffsetsByLangs, MsgHeader.Data.langCount);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (IsHash) hashOrIndex = MurMur3HashUtils.GetHash(entryName);
            handler.Write(ref guid);
            handler.Write(ref unknown);
            handler.Write(ref hashOrIndex);
            handler.WriteOffsetWString(entryName);
            AttributeOffsetStart = handler.Tell();
            handler.Write(ref attributeOffset);
            ContentOffsetsByLangsStart = handler.Tell();
            handler.Skip(8 * MsgHeader.Data.langCount);
            return true;
        }
    }


    public class SubEntry(SubEntryHeader header, List<AttributeItem> attributeItems)
    {
        public SubEntryHeader Header { get; } = header;
        public List<AttributeItem> AttributeItems { get; } = attributeItems;
        public object[]? AttributeValues { get; set; }
        public string[] Strings { get; } = new string[(int)Language.Max];

        public bool Read(FileHandler handler)
        {
            var header = Header;
            if (!header.Read(handler)) return false;
            int langCount = header.MsgHeader.Data.langCount;
            for (int i = 0; i < langCount; i++)
            {
                Strings[i] = handler.ReadWString(header.ContentOffsetsByLangs![i]);
            }
            AttributeValues = new object[AttributeItems.Count];
            handler.Seek(header.attributeOffset);
            for (int i = 0; i < AttributeItems.Count; i++)
            {
                AttributeValues[i] = AttributeItems[i].ValueType switch
                {
                    AttributeValueType.Long => handler.ReadInt64(),
                    AttributeValueType.Double => handler.ReadDouble(),
                    AttributeValueType.NullString or AttributeValueType.String => handler.ReadOffsetWString(),
                    _ => throw new Exception($"Unknown attribute value type: {AttributeItems[i].ValueType}"),
                };
            }
            return true;
        }

        public bool WriteHeader(FileHandler handler)
        {
            return Header.Write(handler);
        }

        public bool WriteAttributes(FileHandler handler)
        {
            if (AttributeValues is null)
            {
                throw new NullReferenceException("AttributeValues is null");
            }
            var header = Header;
            header.attributeOffset = handler.Tell();
            if (header.AttributeOffsetStart == -1) throw new InvalidOperationException("AttributeOffsetStart is not set");
            if (header.ContentOffsetsByLangsStart == -1) throw new InvalidOperationException("ContentOffsetsByLangsStart is not set");
            handler.Write(header.AttributeOffsetStart, header.attributeOffset);
            for (int i = 0; i < AttributeItems.Count; i++)
            {
                switch (AttributeItems[i].ValueType)
                {
                    case AttributeValueType.Long: handler.Write((long)AttributeValues[i]); break;
                    case AttributeValueType.Double: handler.Write((double)AttributeValues[i]); break;
                    case AttributeValueType.NullString:
                    case AttributeValueType.String:
                        handler.WriteOffsetWString((string)AttributeValues[i]);
                        break;
                }
            }

            int langCount = header.MsgHeader.Data.langCount;
            long pos = handler.Tell();
            handler.Seek(header.ContentOffsetsByLangsStart);
            for (int i = 0; i < langCount; i++)
            {
                header.ContentOffsetsByLangs![i] = handler.Tell();
                handler.WriteOffsetWString(Strings[i]);
            }
            handler.Seek(pos);
            return true;
        }

        public override string ToString()
        {
            return Header.entryName;
        }
    }
}


namespace ReeLib
{
    public class MsgFile(RszFileOption option, FileHandler fileHandler) : BaseRszFile(option, fileHandler)
    {
        public const uint Magic = 0x47534D47;
        public const string Extension = ".msg";

        private static readonly byte[] KEY = [207, 206, 251, 248, 236, 10, 51, 102, 147, 169, 29, 147, 80, 57, 95, 9];

        public Header Header { get; } = new();
        public Language[]? Languages { get; set; }
        public List<AttributeItem> AttributeItems { get; } = new();
        public List<SubEntry> SubEntryList { get; } = new();

        private static void Decrypt(byte[] data)
        {
            byte b = 0;
            var num = 0;
            var num2 = 0;
            do
            {
                var b2 = b;
                b = data[num2];
                var num3 = num++ & 15;
                data[num2] = (byte)(b2 ^ b ^ KEY[num3]);
                num2 = num;
            } while (num < data.Length);
        }

        private static void Encrypt(byte[] data)
        {
            byte b = 0;
            var num = 0;
            var num2 = 0;
            do
            {
                var b2 = data[num2];
                var num3 = num++ & 15;
                data[num2] = (byte)(b2 ^ b ^ KEY[num3]);
                b = data[num2];
                num2 = num;
            } while (num < data.Length);
        }

        protected override bool DoRead()
        {
            AttributeItems.Clear();
            SubEntryList.Clear();

            var handler = FileHandler;
            ref var header = ref Header.Data;
            if (!Header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a MSG file");
            }

            // Decrypt
            byte[] data = handler.ReadBytes(header.dataOffset, (int)(handler.FileSize() - header.dataOffset));
            if (header.version > 12)
            {
                Decrypt(data);
            }
            handler.WriteBytes(header.dataOffset, data);

            long[] entryOffsets = handler.ReadArray<long>(header.entryCount);

            // Read languages
            handler.Seek(header.langOffset);
            Languages = handler.ReadArray<Language>(header.langCount);

            // Read attribute types
            handler.Seek(header.attributeOffset);
            var attributeValueTypes = handler.ReadArray<AttributeValueType>(header.attributeCount);
            for (int i = 0; i < header.attributeCount; i++)
            {
                AttributeItems.Add(new AttributeItem
                {
                    Name = handler.ReadOffsetWString(),
                    ValueType = attributeValueTypes[i],
                });
            }

            // Read sub entries
            for (int i = 0; i < header.entryCount; i++)
            {
                handler.Seek(entryOffsets[i]);
                SubEntry item = new(new SubEntryHeader(Header), AttributeItems);
                item.Read(handler);
                SubEntryList.Add(item);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            FileHandler handler = FileHandler;
            handler.Clear();
            ref var header = ref Header.Data;
            handler.Seek(Header.Size);

            header.entryCount = SubEntryList.Count;
            header.attributeCount = AttributeItems.Count;
            header.langCount = Languages!.Length;

            long entryOffsetsStart = handler.Tell();
            long[] entryOffsets = new long[header.entryCount];
            handler.Skip(header.entryCount * 8);

            header.unknDataOffset = handler.Tell();
            handler.Write(0L);

            header.langOffset = handler.Tell();
            handler.WriteArray(Languages);

            handler.Align(16);
            header.attributeOffset = handler.Tell();
            for (int i = 0; i < AttributeItems.Count; i++)
            {
                handler.Write(AttributeItems[i].ValueType);
            }
            for (int i = 0; i < AttributeItems.Count; i++)
            {
                handler.WriteOffsetWString(AttributeItems[i].Name);
            }

            // entry
            for (int i = 0; i < SubEntryList.Count; i++)
            {
                entryOffsets[i] = handler.Tell();
                SubEntryList[i].WriteHeader(handler);
            }
            for (int i = 0; i < SubEntryList.Count; i++)
            {
                SubEntryList[i].WriteAttributes(handler);
            }
            {
                using var back = handler.SeekJumpBack(entryOffsetsStart);
                handler.WriteArray(entryOffsets);
            }

            header.dataOffset = handler.Tell();
            handler.StringTableFlush();
            // Encrypt
            byte[] data = handler.ReadBytes(header.dataOffset, (int)(handler.FileSize() - header.dataOffset));
            if (header.version > 12)
            {
                Encrypt(data);
            }
            handler.WriteBytes(header.dataOffset, data);

            Header.Write(handler, 0);
            return true;
        }
    }
}
