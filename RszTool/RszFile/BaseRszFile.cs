namespace RszTool
{
    public abstract class BaseRszFile : IDisposable
    {
        public RszFileOption Option { get; set; }
        public long Size { get; protected set; }

        public FileHandler FileHandler { get; set; }
        public RszParser RszParser => Option.RszParser;
        public virtual RSZFile? GetRSZ() => null;
        public bool Embedded { get; set; }
        public bool Changed { get; protected set; }
        public bool StructChanged { get; protected set; }

        public BaseRszFile(RszFileOption option, FileHandler fileHandler)
        {
            Option = option;
            FileHandler = fileHandler;
        }

        ~BaseRszFile()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                FileHandler.Dispose();
            }
        }

        /// <summary>
        /// Read data from file start from 0
        /// </summary>
        /// <returns></returns>
        public bool Read()
        {
            if (!Embedded) FileHandler.Seek(0);
            bool result = DoRead();
            Size = FileHandler.Tell();
            // Console.WriteLine($"{this} Start: {Start}, Read size: {Size}");
            return result;
        }

        public bool Write()
        {
            if (!Embedded) FileHandler.Seek(0);
            bool result = DoWrite();
            Size = FileHandler.Tell();
            // Console.WriteLine($"{this} Start: {Start}, Write size: {Size}");
            return result;
        }

        protected abstract bool DoRead();

        protected abstract bool DoWrite();

        public bool WriteTo(FileHandler handler, bool saveFile = true)
        {
            FileHandler originHandler = FileHandler;
            if (handler == originHandler)
            {
                return Write();
            }
            // save as new file
            bool result;
            FileHandler = handler;
            result = Write();
            if (result && saveFile)
            {
                handler.Save();
            }
            FileHandler = originHandler;
            return result;
        }

        public bool WriteTo(string path)
        {
            return WriteTo(new FileHandler(path, true));
        }

        public bool Save()
        {
            if (Write())
            {
                FileHandler.Save();
                return true;
            }
            return false;
        }

        public bool SaveAs(string path)
        {
            bool result = Write();
            FileHandler.SaveAs(path);
            return result;
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
