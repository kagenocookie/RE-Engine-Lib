namespace ReeLib;

using System;
using System.Text;

public class PakWriter
{
    public bool IncludeFileListManifest { get; set; } = true;

    /// <summary>
    /// <nativePath, diskPath>
    /// </summary>
    private readonly Dictionary<string, FileContentSource> fileList = new();

    private class FileContentSource
    {
        public string? filepath;
        public byte[]? content;
        public Stream? sourceStream;

        public FileContentSource(string filepath)
        {
            this.filepath = filepath;
        }

        public FileContentSource(byte[] content)
        {
            this.content = content;
        }
        public FileContentSource(Stream content)
        {
            this.sourceStream = content;
        }

        public void WriteToStream(Stream stream)
        {
            if (content != null) {
                stream.Write(content);
            } else if (!string.IsNullOrEmpty(filepath) && File.Exists(filepath)) {
                using var fs = File.OpenRead(filepath);
                fs.CopyTo(stream);
            } else if (sourceStream != null) {
                sourceStream.CopyTo(stream);
            } else {
                throw new Exception("Missing file content");
            }
        }
    }

    public void ClearFileList()
    {
        fileList.Clear();
    }

    public void AddFilesFromDirectory(string dir, bool includeSubfolders)
    {
        if (!Directory.Exists(dir)) return;

        foreach (var file in Directory.EnumerateFiles(dir, "*.*", includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)) {
            var relativePath = Path.GetRelativePath(dir, file);
            fileList[PakUtils.NormalizePath(relativePath).ToLowerInvariant()] = new FileContentSource(file);
        }
    }

    public void AddFile(string nativePath, string filepath)
    {
        fileList[PakUtils.NormalizePath(nativePath).ToLowerInvariant()] = new FileContentSource(filepath);
    }

    public void AddFile(string nativePath, byte[] content)
    {
        fileList[PakUtils.NormalizePath(nativePath).ToLowerInvariant()] = new FileContentSource(content);
    }

    public void SaveTo(string pakFilepath)
    {
        using var pak = new PakFile() { filepath = pakFilepath };

        pak.Header.majorVersion = 4;
        pak.Header.fileCount = fileList.Count + (IncludeFileListManifest ? 1 : 0);

        pak.OpenWrite();

        if (IncludeFileListManifest)
        {
            var manifest = GenerateManifestFile();
            var stream = pak.StartWriteFile(PakUtils.ManifestFilepath);
            stream.Write(Encoding.Default.GetBytes(manifest));
            pak.FinishWriteFile();
        }

        foreach (var (path, contents) in fileList)
        {
            var stream = pak.StartWriteFile(path);
            contents.WriteToStream(stream);
            pak.FinishWriteFile();
        }

        pak.WriteContents();
    }

    private string GenerateManifestFile()
    {
        var sb = new StringBuilder();

        sb.AppendLine(PakUtils.ManifestFilepath);
        foreach (var file in fileList.Keys.Order())
        {
            sb.AppendLine(file);
        }

        return sb.ToString();
    }
}