namespace ReeLib;

using System;
using ReeLib.Pak;

/// <summary>
/// Provides fast entry lookup of any resources inside a set of PAK files. Can read and keep track of all PAK entries.
/// </summary>
public class CachedMemoryPakReader : PakReader, IDisposable
{
    private sealed record class PakEntryCache(PakFile file, PakEntry entry);

    private Dictionary<ulong, PakFile>? cachedPaks;
    private Dictionary<ulong, PakEntryCache>? cachedEntries;
    private MemoryStream memoryStream = new();

    /// <summary>
    /// Reads a file into a MemoryStream.
    /// The returned stream is transient, do not hold a permanent reference to it - copy any data you need somewhere else.
    /// </summary>
    public MemoryStream? GetFile(string filepath)
    {
        var hash = PakUtils.GetFilepathHash(filepath);
        return GetFile(hash);
    }

    /// <summary>
    /// Reads a file into a MemoryStream.
    /// The returned stream is transient, do not hold a permanent reference to it - copy any data you need somewhere else.
    /// </summary>
    public MemoryStream? GetFile(ulong filepathHash)
    {
        if (cachedEntries == null || cachedPaks == null)
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
    /// Scan all PAK files and cache lookup info for each file entry.
    /// </summary>
    public void CacheEntries()
    {
        Clear();
        cachedEntries = new();
        foreach (var pak in EnumeratePaks())
        {
            foreach (var entry in pak.Entries)
            {
                var hash = entry.CombinedHash;
                cachedEntries.Add(hash, new PakEntryCache(pak, entry));
                searchedPaths.Remove(hash);
            }

            if (EnableConsoleLogging) Console.WriteLine("Finished caching contents of " + pak.filepath);
            if (searchedPaths.Count == 0) break;
        }
    }

    public void Clear()
    {
        if (cachedPaks != null)
        {
            foreach (var pak in cachedPaks.Values)
            {
                pak.Dispose();
            }
            cachedPaks.Clear();
        }
        cachedEntries?.Clear();
    }

    public void Dispose()
    {
        Clear();
    }
}
