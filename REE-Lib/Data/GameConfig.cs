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
                case "gamepath": GamePath = string.IsNullOrEmpty(value) ? "" : Path.GetFullPath(value.EndsWith(".exe") ? Path.GetDirectoryName(value)! : value).NormalizeFilepath(); break;
                case "chunk":
                case "chunks":
                case "chunkpath":
                case "chunkspath": ChunkPath = string.IsNullOrEmpty(value) ? "" : Path.GetFullPath(value).NormalizeFilepath(); break;
                case "rszjson":
                case "rszjsonpath":
                    Resources.LocalPaths.RszPatchFiles = value.Split('|', StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries)
                        .Select(s => Path.GetFullPath(s).NormalizeFilepath())
                        .ToArray();
                        break;
                case "il2cpp":
                case "il2cpppath": Resources.LocalPaths.Il2cppCache = string.IsNullOrEmpty(value) ? "" : Path.GetFullPath(value).NormalizeFilepath(); break;
                case "filelist":
                case "filelistpath": Resources.LocalPaths.FileList = string.IsNullOrEmpty(value) ? "" : Path.GetFullPath(value).NormalizeFilepath(); break;
                case "pakfiles":
                case "paklist":
                    PakFiles = string.IsNullOrEmpty(value) ? [] : value.Split('|', StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries)
                        .Select(s => Path.GetFullPath(s).NormalizeFilepath())
                        .ToArray();
                    break;
            }
        }
    }

    public string? GuessIl2cppDumpPath()
    {
        if (Il2cppDumpPath != null) return Il2cppDumpPath;
        if (string.IsNullOrEmpty(GamePath)) {
            return null;
        }
        var basepath = GamePath;
        if (basepath.EndsWith(".exe")) {
            basepath = Path.GetDirectoryName(basepath)!;
        }
        return Path.Combine(basepath, "il2cpp_dump.json");
    }

    public GameConfig Clone()
    {
        return new GameConfig(Game, _resources!) {
            GamePath = GamePath,
            ChunkPath = ChunkPath,
            Il2cppDumpPath = Il2cppDumpPath,
            PakFiles = PakFiles,
        };
    }

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
