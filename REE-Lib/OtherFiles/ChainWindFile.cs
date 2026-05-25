using System.Numerics;

namespace ReeLib.ChainWind
{
    public class ChainWindSubItem : ReadWriteModel
    {
        public Vector3 direction;
        public float param1;
        public float param2;
        public float param3;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref direction);
            action.Do(ref param1);
            action.Do(ref param2);
            action.Do(ref param3);
            action.Null(8);
            return true;
        }

        public override string ToString() => $"{direction} | {param1} {param2} {param3}";
    }

    public class ChainWindItem : ReadWriteModel
    {
        public float num;
        internal int itemCount;
        internal long offset;

        public List<ChainWindSubItem> Items { get; } = new();

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref num);
            action.Do(ref itemCount);
            action.Do(ref offset);
            return true;
        }

        public override string ToString() => $"Chain Wind {num}";
    }
}

namespace ReeLib
{
    using ReeLib.ChainWind;

    public class ChainWindFile(FileHandler handler) : BaseFile(handler)
    {
        public List<ChainWindItem> Items { get; } = new();

        public const uint Magic = 0x64776863;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var version = handler.Read<int>();
            var magic = handler.Read<int>();
            if (magic != Magic) {
                throw new InvalidDataException($"{handler.FilePath} Not a Chain Wind file");
            }
            handler.ReadNull(8);
            var count = (int)handler.Read<long>();
            var offset = handler.Read<long>();
            handler.Seek(offset);
            Items.Read(handler, count);
            foreach (var item in Items) {
                handler.Seek(item.offset);
                item.Items.Read(handler, item.itemCount);
            }
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(0);
            handler.Write(Magic);
            handler.WriteNull(8);
            handler.Write((long)Items.Count);
            handler.Write(handler.Tell() + 8);
            Items.Write(handler);

            foreach (var item in Items) {
                item.offset = handler.Tell();
                item.itemCount = item.Items.Count;
                item.Items.Write(handler);
            }

            foreach (var item in Items) {
                item.Rewrite(handler);
            }
            return true;
        }
    }
}