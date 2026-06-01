using ReeLib.Common;
using ReeLib.InternalAttributes;
using ReeLib.FCmnDataLs;

namespace ReeLib.FCmnDataLs
{
    [RszGenerate, RszAutoReadWrite]
    public partial class FighterCommonDataHeader : BaseModel
    {
        public uint version;
        internal uint magic = FCmnDataLsFile.Magic;
    }

    internal class FighterCommonDataOffsets : BaseModel
    {
        internal long guidTblOffset;
        internal long idTblOffset;
        internal long objectTblOffset;
        internal long objectEndOffset;

        protected override bool DoRead(FileHandler handler)
        {
            guidTblOffset = handler.ReadInt64();
            idTblOffset = handler.ReadInt64();
            objectTblOffset = handler.ReadInt64();
            objectEndOffset = handler.ReadInt64();
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(guidTblOffset);
            handler.Write(idTblOffset);
            handler.Write(objectTblOffset);
            handler.Write(objectEndOffset);
            return true;
        }
    }
}

namespace ReeLib
{
    using System.Linq;

    [Serializable]
    public class FCmnDataLsFile : BaseRszFile
    {
        public FighterCommonDataHeader Header = new();
        internal FighterCommonDataOffsets Offsets = new();
        public List<Guid> GuidTbl = [];
        public List<uint> IdTbl = [];
        public RSZFile Obj;

        private JsonSerializerOptions _options = new() {
            WriteIndented = true,
            IncludeFields = true
        };

        public const uint Magic = 0x74646366;

        public FCmnDataLsFile(RszFileOption option, FileHandler fileHandler) : base(option, fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);

            if (Header.magic != Magic) {
                throw new InvalidDataException($"{handler.FilePath} is not an fchar file");
            }

            Offsets.Read(handler);
            var ObjCount = handler.ReadUInt();

            handler.Seek(Offsets.guidTblOffset);
            GuidTbl.ReadStructList(handler, (int)ObjCount);
            handler.Seek(Offsets.idTblOffset);
            IdTbl.ReadStructList(handler, (int)ObjCount);

            handler.Seek(Offsets.objectTblOffset);
            Obj = new RSZFile(Option, handler);
            ReadRsz(Obj, handler.Tell());

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            Header.Write(handler);

            Offsets.Write(handler);

            handler.Write(GuidTbl.Count);

            Offsets.guidTblOffset = handler.Tell();
            GuidTbl.Write(handler);
            Offsets.idTblOffset = handler.Tell();
            IdTbl.Write(handler);

            Offsets.objectTblOffset = handler.AlignTell();
            WriteRsz(Obj, handler.Tell());

            Offsets.objectEndOffset = handler.Tell();

            Offsets.Rewrite(handler);

            WriteToJson();
            return true;
        }
    }
}
