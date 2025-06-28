namespace ReeLib;

public class GameConfig(GameIdentifier game)
{
    public GameIdentifier Game => game;
    public GameName BuiltInGame => game.GameEnum;

    public string GamePath { get; set; } = string.Empty;
    public string ChunkPath { get; set; } = string.Empty;
    public string RszJsonPath { get; set; } = string.Empty;
    public string Il2cppPath { get; set; } = string.Empty;
    public string FileListPath { get; set; } = string.Empty;
    public string[] PakFiles { get; set; } = [];

    public void LoadValues(params (string key, string value)[] values) => LoadValues(values.AsEnumerable());
    public void LoadValues(IEnumerable<KeyValuePair<string, string>> values) => LoadValues(values.Select(pair => (pair.Key, pair.Value)));
    public void LoadValues(IEnumerable<(string key, string value)> values)
    {
        foreach (var (key, value) in values)
        {
            switch (key.Replace("_", "").Replace(" ", "").ToLowerInvariant())
            {
                case "game":
                case "gamepath": GamePath = Path.GetFullPath(value.EndsWith(".exe") ? Path.GetDirectoryName(value)! : value); break;
                case "chunk":
                case "chunks":
                case "chunkpath":
                case "chunkspath": ChunkPath = Path.GetFullPath(value); break;
                case "rszjson":
                case "rszjsonpath": RszJsonPath = Path.GetFullPath(value); break;
                case "il2cpp":
                case "il2cpppath": Il2cppPath = Path.GetFullPath(value); break;
                case "filelist":
                case "filelistpath": FileListPath = Path.GetFullPath(value); break;
                case "pakfiles":
                case "paklist":
                    PakFiles = value.Split(',', StringSplitOptions.TrimEntries)
                        .Select(Path.GetFullPath)
                        .ToArray();
                    break;
            }
        }
    }
}
