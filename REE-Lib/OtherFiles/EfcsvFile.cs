namespace ReeLib
{
    public class EfcsvFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public List<Row> Rows { get; } = [];

        public class Row
        {
            public List<float> Values { get; init; } = new();

            public override string ToString() => string.Join(", ", Values.Take(6));
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var rows = handler.Read<int>();
            var cols = handler.Read<int>();
            for (int i = 0; i < rows; i++) {
                var row = new List<float>();
                row.ReadStructList(handler, cols);
                Rows.Add(new Row() { Values = row });
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Rows.Count);
            if (Rows.Count == 0) {
                handler.Write(0);
                return true;
            }

            var cols = Rows[0].Values.Count;
            if (!Rows.All(row => row.Values.Count == cols)) {
                throw new InvalidDataException($"All EFCSV rows need to have the same number of columns (expected {cols} based on first row)");
            }

            foreach (var row in Rows) {
                row.Values.Write(handler);
            }

            return true;
        }
    }
}