namespace ReeLib
{
    public class RszFileOption
    {
        public GameName GameName { get; set; }
        public GameVersion Version { get; set; }
        public RszParser RszParser { get; set; }

        public RszFileOption(GameName gameName, string? rszJsonFilepath = null)
        {
            GameName = gameName;
            if (!Enum.TryParse(gameName.ToString(), out GameVersion version))
            {
                throw new Exception($"GameVersion {GameName} not found.");
            }
            Version = version;
            RszParser = RszParser.GetInstance(rszJsonFilepath ?? $"rsz{gameName}.json");
        }

        // NOTE: temporary - will probably remove this class and pass in a Workspace everywhere
        public RszFileOption(GameName gameName, RszParser parser)
        {
            GameName = gameName;
            if (!Enum.TryParse(gameName.ToString(), out GameVersion version))
            {
                version = GameVersion.unknown;
            }
            Version = version;
            RszParser = parser;
        }
    }
}
