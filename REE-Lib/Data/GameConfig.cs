namespace ReeLib;

public class GameConfig
{
    public GameConfig(GameIdentifier game)
    {
        Game = game;
    }

    public GameConfig(GameIdentifier game, LocalResources resources)
    {
        Game = game;
        _resources = resources;
    }

    public GameIdentifier Game { get; }
    public GameName BuiltInGame => Game.GameEnum;

    private LocalResources? _resources;

    /// <summary>
    /// Gets the local resource metadata for the current game. If it has not yet been initialized, it will be fetched and updated.
    /// Will fetch and download from remote if the data is missing with a blocking HTTP request, and read cached files from the local disk otherwise.
    /// </summary>
    public LocalResources Resources => _resources ??= ResourceRepository.UpdateAndGet(Game);

    public string GamePath { get; set; } = string.Empty;
    public string ChunkPath { get; set; } = string.Empty;
    public string? Il2cppDumpPath { get; set; }
    public string[] PakFiles { get; set; } = [];

    public string[] RszPatchFiles => Resources.LocalPaths.RszPatchFiles;

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
                case "rszjsonpath": Resources.LocalPaths.RszPatchFiles = value.Split(',', StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries).Select(Path.GetFullPath).ToArray(); break;
                case "il2cpp":
                case "il2cpppath": Resources.LocalPaths.Il2cppCache = Path.GetFullPath(value); break;
                case "filelist":
                case "filelistpath": Resources.LocalPaths.FileList = Path.GetFullPath(value); break;
                case "pakfiles":
                case "paklist":
                    PakFiles = value.Split(',', StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries)
                        .Select(Path.GetFullPath)
                        .ToArray();
                    break;
            }
        }
    }

    public string? GuessIl2cppDumpPath() => Il2cppDumpPath ?? (string.IsNullOrEmpty(GamePath) ? null : Path.Combine(Path.GetDirectoryName(GamePath) ?? string.Empty, "il2cpp_dump.json"));

    /// <summary>
    /// Create a new config from the public remote game resource repository.
    /// </summary>
    public static GameConfig CreateFromRepository(GameIdentifier game)
    {
        var config = new GameConfig(game);
        config._resources = ResourceRepository.UpdateAndGet(game);
        return config;
    }
}
