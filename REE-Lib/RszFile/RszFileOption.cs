namespace ReeLib
{
    public class RszFileOption
    {
        public GameName Version { get; set; }
        public RszParser RszParser { get; set; }

        public RszFileOption(GameName gameName, RszParser parser)
        {
            Version = gameName;
            RszParser = parser;
        }
    }
}
