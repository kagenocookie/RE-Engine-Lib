namespace ReeLib
{
    public abstract class BaseFile : IDisposable
    {
        public long Size { get; protected set; }
        public bool Embedded { get; set; }

        public FileHandler FileHandler { get; set; }

        public BaseFile(FileHandler fileHandler)
        {
            FileHandler = fileHandler;
        }

        ~BaseFile()
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
            FileHandler.Seek(0);
            bool result = DoRead();
            Size = FileHandler.Tell();
            // Console.WriteLine($"{this} Start: {Start}, Read size: {Size}");
            return result;
        }

        /// <summary>
        /// Read data starting from the current offset.
        /// </summary>
        /// <returns></returns>
        public bool ReadNoSeek()
        {
            var start = FileHandler.Tell();
            bool result = DoRead();
            Size = FileHandler.Tell() - start;
            // Console.WriteLine($"{this} Start: {Start}, Read size: {Size}");
            return result;
        }

        public bool Write()
        {
            FileHandler.Seek(0);
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
            try
            {
                FileHandler = handler;
                result = Write();
                if (result && saveFile)
                {
                    handler.Save();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = false;
            }
            FileHandler = originHandler;
            return result;
        }

        public bool WriteTo(string path)
        {
            using var file = File.Create(path);
            var handler = new FileHandler(file, path);
            return WriteTo(handler);
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
    }
}
