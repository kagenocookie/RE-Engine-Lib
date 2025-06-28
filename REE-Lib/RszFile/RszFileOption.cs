namespace ReeLib
{
    public class RszFileOption
    {
        public GameName GameName { get; set; }
        public GameVersion Version { get; set; }
        public RszParser RszParser { get; set; }

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
