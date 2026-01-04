using System.Text.Json.Serialization;
using ReeLib.Common;

namespace ReeLib
{
    public abstract class BaseFile : IDisposable
    {
        [JsonIgnore]
        public long Size { get; protected set; }
        [JsonIgnore]
        public bool Embedded { get; set; }

        [JsonIgnore]
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
            return result;
        }

        public bool Write()
        {
            FileHandler.Seek(0);
            bool result = DoWrite();
            Size = FileHandler.Tell();
            return result;
        }

        public bool WriteNoSeek()
        {
            var start = FileHandler.Tell();
            bool result = DoWrite();
            Size = FileHandler.Tell() - start;
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
                Log.Error(e);
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
