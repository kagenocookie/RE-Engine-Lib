namespace ReeLib;

using System;
using ReeLib.Pak;

/// <summary>
/// Provides fast entry lookup of any resources inside a set of PAK files. Can read and keep track of all PAK entries.<br/>
/// Thread safe except for methods noted otherwise. Base class methods may not be thread safe either.
/// </summary>
public class CachedMemoryPakReader : PakReader, IDisposable
{
    private sealed record class PakEntryCache(PakFile file, PakEntry entry);

    private volatile Dictionary<ulong, PakEntryCache>? cachedEntries;
    private MemoryStream memoryStream = new();
    public int MatchedEntryCount => cachedEntries?.Count ?? 0;
    public IEnumerable<string> CachedPaths => cachedEntries?.Values.Select(e => e.entry.path ?? ("__Unknown/" + e.entry.CombinedHash.ToString("X16"))) ?? Array.Empty<string>();

    private readonly object _lock = new();
    private const int PakCacheTimeout = 30000;

    /// <summary>
    /// Reads a file into a MemoryStream.
    /// </summary>
    public MemoryStream? GetFile(string filepath)
    {
        var hash = PakUtils.GetFilepathHash(filepath);
        return GetFile(hash);
    }

    /// <summary>
    /// Reads a file into a MemoryStream.
    /// </summary>
    public MemoryStream? GetFile(ulong filepathHash)
    {
        if (cachedEntries == null)
        {
            CacheEntries();
            if (cachedEntries == null) throw new Exception("Failed to generate cache of PAK entries");
        }

        if (cachedEntries.TryGetValue(filepathHash, out var entry))
        {
            var memoryStream = new MemoryStream();
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.SetLength(0);
            using var fs = File.OpenRead(entry.file.filepath);
            PakFile.ReadEntry(entry.entry, fs, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        return null;
    }

    public bool FileExists(string filepath) => FileExists(PakUtils.GetFilepathHash(filepath));
    public bool FileExists(ulong filepathHash)
    {
        if (cachedEntries == null)
        {
            CacheEntries();
            if (cachedEntries == null) throw new Exception("Failed to generate cache of PAK entries");
        }

        return cachedEntries.ContainsKey(filepathHash);
    }

    /// <summary>
    /// Reads a file into a MemoryStream using a reused file stream.<br/>
    /// Not thread safe.<br/><br/>
    /// The returned stream is transient, do not hold a permanent reference to it - copy any data you need somewhere else.
    /// </summary>
    public MemoryStream? GetFileCached(ulong filepathHash)
    {
        if (cachedEntries == null)
        {
            CacheEntries();
            if (cachedEntries == null) throw new Exception("Failed to generate cache of PAK entries");
        }

        if (cachedEntries.TryGetValue(filepathHash, out var entry))
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.SetLength(0);
            entry.file.Read(entry.entry, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        return null;
    }

    /// <summary>
    /// Scan all PAK files and cache lookup info for every file entry hash.
    /// </summary>
    public void CacheEntries(bool assignEntryPaths = false)
    {
        lock(_lock)  {
            if (cachedEntries?.Count > 0) {
                return;
            }

            cachedEntries = new();
            foreach (var pak in EnumeratePaks(assignEntryPaths))
            {
                foreach (var entry in pak.Entries)
                {
                    var hash = entry.CombinedHash;
                    if (!cachedEntries.ContainsKey(hash)) {
                        cachedEntries.Add(hash, new PakEntryCache(pak, entry));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Creates a new reader instance based on the currently cached data. Use for access to non-thread-safe operations when you need thread safety.
    /// Must call <see cref="CacheEntries"/> before to ensure a consistent state.
    /// </summary>
    public CachedMemoryPakReader Clone()
    {
        if (!(cachedEntries?.Count > 0))
        {
            throw new Exception("PAK file entry cache has not been built yet");
        }

        var reader = new CachedMemoryPakReader();
        reader.cachedEntries = cachedEntries;
        reader.PakFilePriority = PakFilePriority;
        reader.Filter = Filter;
        reader.MaxThreads = MaxThreads;
        reader.EnableConsoleLogging = EnableConsoleLogging;
        return reader;
    }

    /// <summary>
    /// Clears the current pak and entry cache.
    /// Not thread safe.
    /// </summary>
    public void Clear()
    {
        cachedEntries?.Clear();
        searchedPaths.Clear();
    }

    public void Dispose()
    {
        Clear();
        GC.SuppressFinalize(this);
    }
}
