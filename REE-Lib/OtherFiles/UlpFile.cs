namespace ReeLib
{
    public class UlpFile(FileHandler handler) : BaseFile(handler)
    {
        public List<string> Shaders { get; } = new();

        protected override bool DoRead()
        {
            var handler = FileHandler;
            for (int i = 0; i < 6; i++) {
                var str = handler.ReadOffsetWStringNullable();
                if (str != null) Shaders.Add(str);
            }
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            for (int i = 0; i < 6; i++) {
                if (i < Shaders.Count) {
                    handler.WriteOffsetWString(Shaders[i]);
                } else {
                    handler.Write(0L);
                }
            }
            handler.StringTableFlush();
            return true;
        }
    }
}