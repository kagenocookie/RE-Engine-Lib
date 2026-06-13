namespace ReeLib;

public readonly struct PlatformIdentifier
{
    public static readonly PlatformIdentifier Unknown = new PlatformIdentifier(Platform.Unknown, "");
    public static readonly PlatformIdentifier X64 = new PlatformIdentifier(Platform.X64, "natives/x64/");
    public static readonly PlatformIdentifier Steam = new PlatformIdentifier(Platform.Steam, "natives/STM/");
    public static readonly PlatformIdentifier EpicGames = new PlatformIdentifier(Platform.EpicGames, "natives/EGS/");
    public static readonly PlatformIdentifier MicrosoftGames = new PlatformIdentifier(Platform.MicrosoftGames, "natives/MSG/");
    public static readonly PlatformIdentifier Mac = new PlatformIdentifier(Platform.Mac, "natives/Mac/");

    public static readonly PlatformIdentifier PS4 = new PlatformIdentifier(Platform.PS4, "natives/PS4/");
    public static readonly PlatformIdentifier PS5 = new PlatformIdentifier(Platform.PS5, "natives/PS5/");
    public static readonly PlatformIdentifier Switch = new PlatformIdentifier(Platform.NintendoSwitch, "natives/NSW/");

    public static readonly PlatformIdentifier Android = new PlatformIdentifier(Platform.Android, "natives/Android/");
    public static readonly PlatformIdentifier IOS = new PlatformIdentifier(Platform.IOS, "natives/iOS/");

    public static readonly PlatformIdentifier[] Desktop = [ Steam, EpicGames, MicrosoftGames, Mac ];
    public static readonly PlatformIdentifier[] Console = [ PS4, PS5 ];
    public static readonly PlatformIdentifier[] Mobile = [ Android, IOS ];
    public static readonly PlatformIdentifier[] All = [ .. Desktop, .. Console, .. Mobile ];
    public static readonly PlatformIdentifier[] NonDesktop = [ .. Console, .. Mobile ];

    private static readonly PlatformIdentifier[] X64Array = [ X64 ];

    public static readonly GameName[] X64Games = [GameName.re7, GameName.re2, GameName.dmc5];
    public static readonly GameName[] EGSGames = [GameName.re9];
    public static readonly GameName[] MacGames = [GameName.re4, GameName.re8];

    public readonly string basePath;
    public readonly Platform id;

    public PlatformIdentifier(Platform id, string basePath)
    {
        this.id = id;
        this.basePath = basePath;
    }

    public static bool IsX64Game(GameName game) => game is GameName.re7 or GameName.re2 or GameName.dmc5;
    public static PlatformIdentifier[] GetAvailableDesktopPlatforms(GameName game)
    {
        if (IsX64Game(game)) return X64Array;

        return Desktop;
    }
    public static PlatformIdentifier GetDefaultIdentifier(GameName game) => IsX64Game(game) ? X64 : Steam;
    public static PlatformIdentifier FindPlatformIdentifierFromPath(ReadOnlySpan<char> path)
    {
        if (path.Length < "natives/stm/".Length) {
            return Unknown;
        }

        // note: in order to make it go faster, assume we received no unknown platforms
        // thus, we can switch based on the known set using the first identifiable character instead of matching the full strings
        switch (path[8]) {
            case 's': case 'S':
                return Steam;
            case 'p': case 'P':
                return path[9] == '4' ? PS4 : PS5;
            case 'n': case 'N':
                return Switch;
            case 'a': case 'A':
                return Android;
            case 'm': case 'M':
                return path[10] is 's' or 'S' ? MicrosoftGames : Mac;
            case 'e': case 'E':
                return EpicGames;
            case 'i': case 'I':
                return IOS;
            case 'x': case 'X':
                return X64;
        }

        return Unknown;
    }

    public static implicit operator Platform(PlatformIdentifier plat) => plat.id;
    public static implicit operator int(PlatformIdentifier id) => (int)id.id;

    public static implicit operator PlatformIdentifier(Platform platformId) => platformId switch {
        Platform.Steam => Steam,
        Platform.EpicGames => EpicGames,
        Platform.MicrosoftGames => MicrosoftGames,
        Platform.X64 => X64,
        Platform.PS4 => PS4,
        Platform.PS5 => PS5,
        Platform.NintendoSwitch => Switch,
        Platform.Mac => Mac,
        Platform.Android => Android,
        Platform.IOS => IOS,
        _ => new PlatformIdentifier(platformId, "natives/STM/"),
    };

    public override int GetHashCode() => (int)id;
    public override bool Equals(object? obj) => obj is PlatformIdentifier id && id.id == id;
    public static bool operator ==(PlatformIdentifier left, PlatformIdentifier right) => left.id == right.id;
    public static bool operator !=(PlatformIdentifier left, PlatformIdentifier right) => left.id != right.id;

    public override string ToString() => id.ToString();
}

public enum Platform
{
    Unknown = 0,
    X64,
    Steam,
    EpicGames,
    PS5,
    PS4,
    MicrosoftGames,
    Android,
    IOS,
    NintendoSwitch,
    Mac,
}
