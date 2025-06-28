namespace ReeLib;

public class GameConfig(GameIdentifier game)
{
    public GameIdentifier Game => game;
    public string GamePath { get; set; } = string.Empty;
    public string ChunkPath { get; set; } = string.Empty;
    public string RszJsonPath { get; set; } = string.Empty;
    public string Il2cppPath { get; set; } = string.Empty;
    public string FileListPath { get; set; } = string.Empty;
    public string[] PakFiles { get; set; } = [];

    public void LoadValues(params (string key, string value)[] values) => LoadValues((IEnumerable<(string key, string value)>)values);
    public void LoadValues(IEnumerable<(string key, string value)> values)
    {
        foreach (var (key, value) in values)
        {
            switch (key.Replace("_", "").Replace(" ", "").ToLowerInvariant())
            {
                case "game":
                case "gamepath": GamePath = value; break;
                case "chunk":
                case "chunks":
                case "chunkpath":
                case "chunkspath": ChunkPath = value; break;
                case "rszjson":
                case "rszjsonpath": RszJsonPath = value; break;
                case "il2cpp":
                case "il2cpppath": Il2cppPath = value; break;
                case "filelist":
                case "filelistpath": FileListPath = value; break;
                case "pakfiles":
                case "paklist":
#if NET5_0_OR_GREATER
                    PakFiles = value.Split(',', StringSplitOptions.TrimEntries);
#else
                    PakFiles = value.Split(',').Select(f => f.Trim()).ToArray();
#endif
                    break;
            }
        }
    }
}
