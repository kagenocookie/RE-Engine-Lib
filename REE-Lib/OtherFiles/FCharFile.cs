using ReeLib.Common;
using ReeLib.InternalAttributes;
using System.Text.Json;

namespace ReeLib.FChar
{
    [RszGenerate, RszAutoReadWrite]
    public partial class FighterDataHeader : BaseModel
    {
        public uint version;
        internal uint magic = FCharFile.Magic;
    }

    internal class FighterDataOffsets : BaseModel
    {
        internal long idTblOffset;
        internal long parentIdTblOffset;
        internal long actionListTblOffset;
        internal long dataIdTblOffset;
        internal long dataListTblOffset;
        internal long commonDataIdTblOffset;
        internal long commonDataListOffset;
        internal long objectTblOffset;
        internal long objectEndOffset;

        protected override bool DoRead(FileHandler handler)
        {
            idTblOffset = handler.ReadInt64();
            parentIdTblOffset = handler.ReadInt64();
            actionListTblOffset = handler.ReadInt64();
            dataIdTblOffset = handler.ReadInt64();
            dataListTblOffset = handler.ReadInt64();
            commonDataIdTblOffset = handler.ReadInt64();
            commonDataListOffset = handler.ReadInt64();
            objectTblOffset = handler.ReadInt64();
            objectEndOffset = handler.ReadInt64();
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(idTblOffset);
            handler.Write(parentIdTblOffset);
            handler.Write(actionListTblOffset);
            handler.Write(dataIdTblOffset);
            handler.Write(dataListTblOffset);
            handler.Write(commonDataIdTblOffset);
            handler.Write(commonDataListOffset);
            handler.Write(objectTblOffset);
            handler.Write(objectEndOffset);
            return true;
        }
    }

    internal class FighterDataOffsetsV18 : BaseModel
    {
        internal long extendDataTblOffset;

        protected override bool DoRead(FileHandler handler)
        {
            extendDataTblOffset = handler.ReadInt64();
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(extendDataTblOffset);
            return true;
        }
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class FighterKeyData : BaseModel
    {
        public uint startFrame;
        public uint endFrame;
    }

    public class FighterKeyDataList : BaseModel
    {
        internal long dataTblOffset;
        public List<FighterKeyData> dataTbl = [];
        internal long objectTblOffset;
        internal long objectEndOffset;
        public RSZFile obj;
        public uint objectCount;

        protected override bool DoRead(FileHandler handler)
        {
            dataTblOffset = handler.ReadInt64();
            objectTblOffset = handler.ReadInt64();
            objectEndOffset = handler.ReadInt64();
            objectCount = handler.ReadUInt();

            handler.Seek(dataTblOffset);
            dataTbl.Read(handler, (int)objectCount);

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(dataTblOffset);
            handler.Write(objectTblOffset);
            handler.Write(objectEndOffset);

            handler.Write(dataTbl.Count);

            dataTblOffset = handler.AlignTell();
            dataTbl.Write(handler);

            return true;
        }

        public object SerializeJson()
        {
            return new { dataTbl, obj = obj.ObjectList };
        }
    }

    public class FighterActionData : BaseModel
    {
        internal long dataTblOffset;
        public OffsetList<FighterKeyDataList> dataTbl = new();
        internal long objectTblOffset;
        internal long objectEndOffset;
        public List<RSZFile> objTbl = [];
        public uint actionID;
        public uint frames;
        internal uint keyCount;
        public uint refActionID;

        protected override bool DoRead(FileHandler handler)
        {
            dataTblOffset = handler.ReadInt64();
            objectTblOffset = handler.ReadInt64();
            objectEndOffset = handler.ReadInt64();
            keyCount = handler.ReadUInt();
            var actionDataCount = handler.ReadUInt();
            actionID = handler.ReadUInt();
            frames = handler.ReadUInt();
            refActionID = handler.ReadUInt();

            handler.Seek(dataTblOffset);
            dataTbl.count = (int)actionDataCount;
            dataTbl.Read(handler);

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(dataTblOffset);
            handler.Write(objectTblOffset);
            handler.Write(objectEndOffset);

            handler.Write(objTbl.Count);
            handler.Write(dataTbl.items.Count);
            handler.Write(actionID);
            handler.Write(frames);
            handler.Write(refActionID);

            dataTblOffset = handler.AlignTell();
            dataTbl.Write(handler);

            return true;
        }

        public object SerializeJson()
        {
            var dataTbl = this.dataTbl.items.Select(data => data.SerializeJson()).ToList();
            var objTbl = this.objTbl.Select(obj => obj.ObjectList).ToList();
            return new { dataTbl, objTbl, actionID, frames,
                refActionId = refActionID };
        }
    }

    public class FighterActionListData : BaseModel
    {
        internal long dataTblOffset;
        public OffsetList<FighterActionData> dataTbl = new();
        internal long objectTblOffset;
        internal long objectEndOffset;
        public List<RSZFile> objTbl = [];
        internal uint objectCount;

        protected override bool DoRead(FileHandler handler)
        {
            dataTblOffset = handler.ReadInt64();
            objectTblOffset = handler.ReadInt64();
            objectEndOffset = handler.ReadInt64();
            var actionCount = handler.ReadUInt();
            objectCount = handler.ReadUInt();

            handler.Seek(dataTblOffset);
            dataTbl.count = (int)actionCount;
            dataTbl.Read(handler);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(dataTblOffset);
            handler.Write(objectTblOffset);
            handler.Write(objectEndOffset);

            handler.Write(dataTbl.items.Count);
            handler.Write(objTbl.Count);

            dataTblOffset = handler.AlignTell();
            dataTbl.Write(handler);

            return true;
        }

        public object SerializeJson()
        {
            var dataTbl = this.dataTbl.items.Select(data => data.SerializeJson()).ToList();
            var objTbl = this.objTbl.Select(obj => obj.ObjectList).ToList();

            return new { dataTbl, objTbl };
        }
    }

    public class FighterDataListData : BaseModel
    {
        internal long idTblOffset;
        public List<uint> idTbl = [];
        public RSZFile obj;
        internal long objectTblOffset;
        internal long objectEndOffset;

        protected override bool DoRead(FileHandler handler)
        {
            idTblOffset = handler.ReadInt64();
            objectTblOffset = handler.ReadInt64();
            objectEndOffset = handler.ReadInt64();
            var objCount = handler.ReadUInt();

            handler.Seek(idTblOffset);
            idTbl.ReadStructList(handler, (int)objCount);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(idTblOffset);
            handler.Write(objectTblOffset);
            handler.Write(objectEndOffset);

            handler.Write(idTbl.Count);

            idTblOffset = handler.AlignTell();
            idTbl.Write(handler);

            return true;
        }

        public object SerializeJson()
        {
            return new { idTbl, obj = obj.ObjectList };
        }
    }

    public class OffsetList<T> : BaseModel where T : BaseModel, new()
    {
        public int count;
        public List<long> offsets = [];
        public List<T> items = [];

        protected override bool DoRead(FileHandler handler)
        {
            offsets.ReadStructList(handler, count);
            handler.Align(16);
            for (var i = 0; i < count; i++) {
                handler.Seek(offsets[i]);
                var item = new T();
                item.Read(handler);
                items.Add(item);
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            count = items.Count;
            if (offsets.Count < count) {
                offsets.AddRange(new long[count - offsets.Count]);
            }
            else if (offsets.Count > count) {
                offsets.RemoveRange(count, offsets.Count - count);
            }
            for (var i = 0; i < count; i++) {
                handler.WriteInt64(offsets[i]);
            }
            handler.Align(16);
            for (var i = 0; i < count; i++) {
                offsets[i] = handler.Tell();
                items[i].Write(handler);
            }

            return true;
        }
    }

    public class OffsetListWStr : BaseModel
    {
        internal int count;
        internal List<long> offsets = [];
        public List<string> items = [];

        protected override bool DoRead(FileHandler handler)
        {
            offsets.ReadStructList(handler, count);
            handler.Align(16);
            for (var i = 0; i < count; i++) {
                handler.Seek(offsets[i]);
                items.Add(handler.ReadWString());
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            count = items.Count;
            if (offsets.Count < count) {
                offsets.AddRange(new long[count - offsets.Count]);
            }
            else if (offsets.Count > count) {
                offsets.RemoveRange(count, offsets.Count - count);
            }
            for (var i = 0; i < count; i++) {
                handler.WriteInt64(offsets[i]);
            }
            handler.Align(16);
            for (var i = 0; i < count; i++) {
                offsets[i] = handler.Tell();
                handler.WriteWString(items[i]);
            }

            return true;
        }
    }
}

namespace ReeLib
{
    using FChar;
    using System.Linq;

    [Serializable]
    public class FCharFile : BaseRszFile
    {
        public FighterDataHeader Header = new();
        internal FighterDataOffsetsV18 OffsetsV18 = new();
        internal FighterDataOffsets Offsets = new();
        public List<uint> ExtendDataTbl = [];
        public List<uint> IdTbl = [];
        public List<uint> ParentIdTbl = [];
        public OffsetList<FighterActionListData> ActionListTbl = new();
        public List<uint> DataListIdTbl = [];
        public OffsetList<FighterDataListData> DataListTbl = new();
        public List<uint> CommonDataIdTbl = [];
        public OffsetListWStr CommonDataList = new();
        public List<RSZFile> ObjTbl = [];

        public const uint Magic = 0x72686366;

        public FCharFile(RszFileOption option, FileHandler fileHandler) : base(option, fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);

            if (Header.magic != Magic) {
                throw new InvalidDataException($"{handler.FilePath} is not an fchar file");
            }

            var version = Header.version;

            if (version >= 18) OffsetsV18.Read(handler);
            Offsets.Read(handler);
            var ObjCount = handler.ReadUInt();
            var StyleCount = handler.ReadUInt();
            var DataCount = handler.ReadUInt();
            var ResourceCount = handler.ReadUInt();
            uint ExtendDataCount = 0;
            if (version >= 18) ExtendDataCount = handler.ReadUInt();

            if (version >= 18) {
                handler.Seek(OffsetsV18.extendDataTblOffset);
                ExtendDataTbl.ReadStructList(handler, (int)ExtendDataCount);
            }

            handler.Seek(Offsets.idTblOffset);
            IdTbl.ReadStructList(handler, (int)StyleCount);
            handler.Seek(Offsets.parentIdTblOffset);
            ParentIdTbl.ReadStructList(handler, (int)StyleCount);

            handler.Seek(Offsets.actionListTblOffset);
            ActionListTbl.count = (int)StyleCount;
            ActionListTbl.Read(handler);

            foreach (var actionList in ActionListTbl.items) {
                handler.Seek(actionList.objectTblOffset);
                for (var i = 0; i < actionList.objectCount; i++) {
                    var rsz = new RSZFile(Option, handler);
                    ReadRsz(rsz, handler.Tell());
                    actionList.objTbl.Add(rsz);
                }

                foreach (var action in actionList.dataTbl.items) {
                    handler.Seek(action.objectTblOffset);
                    for (var i = 0; i < action.keyCount; i++) {
                        var rsz = new RSZFile(Option, handler);
                        ReadRsz(rsz, handler.Tell());
                        action.objTbl.Add(rsz);
                    }

                    foreach (var key in action.dataTbl.items) {
                        handler.Seek(key.objectTblOffset);
                        key.obj = new RSZFile(Option, handler);
                        ReadRsz(key.obj, handler.Tell());
                    }
                }
            }

            handler.Seek(Offsets.dataIdTblOffset);
            DataListIdTbl.ReadStructList(handler, (int)DataCount);

            handler.Seek(Offsets.dataListTblOffset);
            DataListTbl.count = (int)DataCount;
            DataListTbl.Read(handler);

            foreach (var dataList in DataListTbl.items) {
                handler.Seek(dataList.objectTblOffset);
                dataList.obj = new RSZFile(Option, handler);
                ReadRsz(dataList.obj, handler.Tell());
            }

            handler.Seek(Offsets.commonDataIdTblOffset);
            CommonDataIdTbl.ReadStructList(handler, (int)ResourceCount);

            handler.Seek(Offsets.commonDataListOffset);
            CommonDataList.count = (int)ResourceCount;
            CommonDataList.Read(handler);

            handler.Seek(Offsets.objectTblOffset);
            for (var i = 0; i < ObjCount; i++) {
                var rsz = new RSZFile(Option, handler);
                ReadRsz(rsz, handler.Tell());
                ObjTbl.Add(rsz);
            }

            return true;
        }

        public void WriteToJson()
        {
            var options = new JsonSerializerOptions {
                WriteIndented = true,
                IncludeFields = true
            };
            options.Converters.Add(new RszInstanceJsonConverterSimple(RszParser));
            var actionListTbl = ActionListTbl.items.Select(actionList => actionList.SerializeJson()).ToList();
            var dataListTbl = DataListTbl.items.Select(data => data.SerializeJson()).ToList();
            var objTbl = ObjTbl.Select(obj => obj.ObjectList).ToList();

            var combined = new {
                extendDataTbl = ExtendDataTbl,
                idTbl = IdTbl,
                parentIdTbl = ParentIdTbl,
                actionListTbl,
                dataListIdTbl = DataListIdTbl,
                dataListTbl,
                resourceIdTbl = CommonDataIdTbl,
                resourceListTbl = CommonDataList.items,
                objTbl
            };
            var output = JsonSerializer.Serialize(combined, options);
            File.WriteAllText(FileHandler.FilePath + ".json", output);
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            Header.Write(handler);

            if (Header.version >= 18) {
                OffsetsV18.Write(handler);
            }

            Offsets.Write(handler);

            handler.Write(ObjTbl.Count);
            handler.Write(IdTbl.Count);
            handler.Write(DataListIdTbl.Count);
            handler.Write(CommonDataIdTbl.Count);
            if (Header.version >= 18) handler.Write(ExtendDataTbl.Count);

            if (Header.version >= 18) {
                if (ExtendDataTbl.Count > 0) OffsetsV18.extendDataTblOffset = handler.Tell();
                ExtendDataTbl.Write(handler);
            }

            Offsets.idTblOffset = handler.Tell();
            IdTbl.Write(handler);
            Offsets.parentIdTblOffset = handler.Tell();
            ParentIdTbl.Write(handler);

            Offsets.actionListTblOffset = handler.AlignTell();
            ActionListTbl.Write(handler);

            foreach (var actionList in ActionListTbl.items) {
                actionList.objectTblOffset = handler.Tell();
                foreach (var obj in actionList.objTbl) {
                    WriteRsz(obj, handler.Tell());
                }

                actionList.objectEndOffset = handler.Tell();

                foreach (var action in actionList.dataTbl.items) {
                    action.objectTblOffset = handler.Tell();
                    foreach (var obj in action.objTbl) {
                        WriteRsz(obj, handler.Tell());
                    }

                    action.objectEndOffset = handler.Tell();

                    foreach (var key in action.dataTbl.items) {
                        key.objectTblOffset = handler.Tell();
                        WriteRsz(key.obj, handler.Tell());
                        key.objectEndOffset = handler.Tell();
                    }
                }
            }

            ActionListTbl.Rewrite(handler);

            Offsets.dataIdTblOffset = handler.AlignTell();
            DataListIdTbl.Write(handler);

            Offsets.dataListTblOffset = handler.AlignTell();
            DataListTbl.Write(handler);

            foreach (var dataList in DataListTbl.items) {
                dataList.objectTblOffset = handler.Tell();
                WriteRsz(dataList.obj, handler.Tell());
                dataList.objectEndOffset = handler.Tell();
            }

            DataListTbl.Rewrite(handler);

            Offsets.commonDataIdTblOffset = handler.AlignTell();
            CommonDataIdTbl.Write(handler);

            Offsets.commonDataListOffset = handler.AlignTell();
            CommonDataList.Write(handler);
            CommonDataList.Rewrite(handler);

            Offsets.objectTblOffset = handler.AlignTell();
            foreach (var obj in ObjTbl) {
                WriteRsz(obj, handler.Tell());
            }

            Offsets.objectEndOffset = handler.Tell();

            if (Header.version >= 18) {
                OffsetsV18.Rewrite(handler);
            }

            Offsets.Rewrite(handler);

            WriteToJson();
            return true;
        }
    }
}
