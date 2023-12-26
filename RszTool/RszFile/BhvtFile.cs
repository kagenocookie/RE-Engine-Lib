using RszTool.Bhvt;


namespace RszTool.Bhvt
{
    public class Header : BaseModel
    {
        public uint magic;
        // Skip(4);
        public long nodeOffset;
        public long actionOffset;
        public long selectorOffset;
        public long selectorCallerOffset;
        public long conditionsOffset;
        public long transitionEventOffset;
        public long expressionTreeConditionsOffset;
        public long staticActionOffset;
        public long staticSelectorCallerOffset;
        public long staticConditionsOffset;
        public long staticTransitionEventOffset;
        public long staticExpressionTreeConditionsOffset;
        public long stringOffset;
        public long resourcePathsOffset;
        public long userdataPathsOffset;
        public long variableOffset;
        public long baseVariableOffset;

        long referencePrefabGameObjectsOffset;

        public GameVersion Version { get; set; }

        public Header(GameVersion version)
        {
            Version = version;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref magic);
            handler.ReadRange(ref nodeOffset, ref baseVariableOffset);
            if (Version > GameVersion.dmc5)
            {
                handler.Read(ref referencePrefabGameObjectsOffset);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }
}


namespace RszTool
{
    public class BhvtFile(RszFileOption option, FileHandler fileHandler) : BaseRszFile(option, fileHandler)
    {
        public const uint Magic = 0x54564842;
        public const string Extension = ".bhvt";

        public Header Header { get; } = new Header(option.Version);
        public RSZFile? ActionRsz { get; private set; }
        public RSZFile? SelectorRsz { get; private set; }
        public RSZFile? SelectorCallerRsz { get; private set; }
        public RSZFile? ConditionsRsz { get; private set; }
        public RSZFile? TransitionEventRsz { get; private set; }
        public RSZFile? ExpressionTreeConditionsRsz { get; private set; }
        public RSZFile? StaticActionRsz { get; private set; }
        public RSZFile? StaticSelectorCallerRsz { get; private set; }
        public RSZFile? StaticConditionsRsz { get; private set; }
        public RSZFile? StaticTransitionEventRsz { get; private set; }
        public RSZFile? StaticExpressionTreeConditionsRsz { get; private set; }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a BHVT file");
            }

            ActionRsz = ReadRsz(header.actionOffset);
            SelectorRsz = ReadRsz(header.selectorOffset);
            SelectorCallerRsz = ReadRsz(header.selectorCallerOffset);
            ConditionsRsz = ReadRsz(header.conditionsOffset);
            TransitionEventRsz = ReadRsz(header.transitionEventOffset);
            ExpressionTreeConditionsRsz = ReadRsz(header.expressionTreeConditionsOffset);
            StaticActionRsz = ReadRsz(header.staticActionOffset);
            StaticSelectorCallerRsz = ReadRsz(header.staticSelectorCallerOffset);
            StaticConditionsRsz = ReadRsz(header.staticConditionsOffset);
            StaticTransitionEventRsz = ReadRsz(header.staticTransitionEventOffset);
            StaticExpressionTreeConditionsRsz = ReadRsz(header.staticExpressionTreeConditionsOffset);

            return true;
        }

        protected override bool DoWrite()
        {
            throw new NotImplementedException();
        }
    }
}
