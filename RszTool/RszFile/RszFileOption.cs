namespace RszTool
{
    public class RszFileOption
    {
        public GameName GameName { get; set; }
        public GameVersion Version { get; set; }
        public RszParser RszParser { get; set; }
        public EnumParser EnumParser { get; set; }

        public RszFileOption(GameName gameName)
        {
            GameName = gameName;
            if (!Enum.TryParse(gameName.ToString(), out GameVersion version))
            {
                throw new Exception($"GameVersion {GameName} not found.");
            }
            Version = version;
            RszParser = RszParser.GetInstance($"rsz{gameName}.json");
            EnumParser = EnumParser.GetInstance($"Data\\Enums\\{gameName}_enum.json");
        }
    }
}
