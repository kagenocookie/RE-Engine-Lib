namespace ReeLib
{
    public abstract class BaseRszFile : BaseFile
    {
        public RszFileOption Option { get; set; }

        public RszParser RszParser => Option.RszParser;
        public virtual RSZFile? GetRSZ() => null;
        public bool Changed { get; protected set; }
        public bool StructChanged { get; protected set; }

        public BaseRszFile(RszFileOption option, FileHandler fileHandler)
            : base(fileHandler)
        {
            Option = option;
        }

        protected void ReadRsz(RSZFile rsz, long offset)
        {
            rsz.FileHandler = FileHandler.WithOffset(offset);
            rsz.Read();
        }

        protected RSZFile ReadRsz(long offset)
        {
            RSZFile rsz = new(Option, FileHandler.WithOffset(offset));
            rsz.Read();
            return rsz;
        }

        protected bool WriteRsz(RSZFile rsz, long offset)
        {
            // 内部偏移是从0开始算的
            return rsz.WriteTo(FileHandler.WithOffset(offset), false);
        }
    }
}
