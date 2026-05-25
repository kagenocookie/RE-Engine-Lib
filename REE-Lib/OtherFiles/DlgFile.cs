using ReeLib.Common;

namespace ReeLib.Dlg
{
    public class Header : ReadWriteModel
    {
        public uint magic = DlgFile.Magic;
        public int version;
        public uint hash1;
        public uint hash2;
        internal long itemsOffset;
        internal long guidItemsOffset;
        internal long tmlCountMaybe;
        internal long tmlPathOffset;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref magic);
            action.Do(ref version);
            action.Do(ref hash1);
            action.Do(ref hash2);
            action.Do(ref itemsOffset);
            action.Do(ref guidItemsOffset);
            action.Null(8);
            action.Do(ref tmlCountMaybe);
            Log.WarnIf(tmlCountMaybe != 1, "We may have multiple paths here");
            action.Do(ref tmlPathOffset);
            return true;
        }
    }

    public class DialogueItem : ReadWriteModel
    {
        internal int paramCount;
        public Guid guid;
        internal long nodesRszOffset;
        internal long paramGuidsOffset;
        internal long paramsRszOffset;
        public string? dialogueTimelinePath;

        public List<Guid> ParamGuids { get; } = new();

        public RSZFile? NodesUserData;
        public RSZFile? ParametersUserData;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref paramCount);
            action.Null(4);
            action.Do(ref guid);
            action.Do(ref nodesRszOffset);
            action.Do(ref paramGuidsOffset);
            action.Do(ref paramsRszOffset);
            action.HandleOffsetWString(ref dialogueTimelinePath, true);
            return true;
        }

        public override string ToString() => $"[{guid}] {dialogueTimelinePath}";
    }

    public struct DialogueGuids
    {
        public Guid guid1;
        public Guid guid2;
        public Guid guid3;

        public readonly override string ToString() => $"[{guid1}] {guid2} / {guid3}";
    }
}

namespace ReeLib
{
    using ReeLib.Dlg;

    public class DlgFile(RszFileOption options, FileHandler handler) : BaseRszFile(options, handler)
    {
        public Header Header { get; } = new();
        public List<DialogueItem> Items { get; set; } = new();
        public List<DialogueGuids> GuidItems { get; set; } = new();
        public string TimelineFilePath { get; set; } = "";

        public const uint Magic = 0x00474C44;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var header = Header;
            header.Read(handler);
            if (header.magic != Magic) {
                throw new InvalidDataException($"{handler.FilePath} Not a Dialog file");
            }

            TimelineFilePath = handler.ReadWString(header.tmlPathOffset);
            handler.Seek(header.itemsOffset);
            var offsets = handler.ReadArray<long>((int)handler.Read<long>());
            foreach (var offset in offsets) {
                var item = new DialogueItem();
                handler.Seek(offset);
                item.Read(handler);
                Items.Add(item);
                if (item.nodesRszOffset != 0) {
                    item.NodesUserData = new RSZFile(Option, handler);
                    ReadRsz(item.NodesUserData, item.nodesRszOffset);
                }

                handler.Seek(item.paramGuidsOffset);
                item.ParamGuids.ReadStructList(handler, item.paramCount);

                if (item.paramsRszOffset != 0) {
                    item.ParametersUserData = new RSZFile(Option, handler);
                    ReadRsz(item.ParametersUserData, item.paramsRszOffset);
                }
            }

            handler.Seek(header.guidItemsOffset);
            offsets = handler.ReadArray<long>((int)handler.Read<long>());
            foreach (var offset in offsets) {
                handler.Seek(offset);
                var item = handler.Read<DialogueGuids>();
                GuidItems.Add(item);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var header = Header;
            header.Write(handler);
            handler.StringTableAdd(TimelineFilePath, false);

            header.itemsOffset = handler.Tell();
            handler.Write((long)Items.Count);
            var offsetsOffset = handler.Tell();
            handler.Skip(Items.Count * 8);

            header.guidItemsOffset = handler.Tell();
            handler.Write((long)GuidItems.Count);
            var guidOffsetsOffset = handler.Tell();
            handler.Skip(GuidItems.Count * 8);

            foreach (var item in Items) {
                handler.Write(offsetsOffset + 8 * Items.IndexOf(item), handler.Tell());
                item.Write(handler);
            }

            for (int i = 0; i < GuidItems.Count; i++) {
                var item = GuidItems[i];
                handler.Write(guidOffsetsOffset + 8 * i, handler.Tell());
                handler.Write(item);
            }

            foreach (var item in Items) {
                if (item.NodesUserData != null) {
                    WriteRsz(item.NodesUserData, item.nodesRszOffset = handler.AlignTell(8));
                }
            }

            foreach (var item in Items) {
                item.paramGuidsOffset = handler.AlignTell(8);
                item.ParamGuids.Write(handler);
            }

            foreach (var item in Items) {
                if (item.ParametersUserData != null) {
                    WriteRsz(item.ParametersUserData, item.paramsRszOffset = handler.AlignTell(8));
                }
            }

            header.tmlPathOffset = handler.AlignTell();
            handler.StringTableFlush();

            header.Rewrite(handler);
            return true;
        }
    }
}