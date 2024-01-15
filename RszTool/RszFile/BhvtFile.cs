using System.Diagnostics;
using System.Runtime.CompilerServices;
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
            handler.Skip(4);
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


    public enum BHVTlvl
    {
        All = -1,
        Actions = 0,
        Selectors = 1,
        SelectorCallers = 2,
        Conditions = 3,
        TransitionEvents = 4,
        ExpressionTreeConditions = 5,
        StaticActions = 6,
        StaticSelectorCallers = 7,
        StaticConditions = 8,
        StaticTransitionEvents = 9,
        StaticExpressionTreeConditions = 10,
        Transition = 11,
        Paths = 12,
        Tags = 13,
        NameHash = 14
    }


    [Flags]
    public enum NodeAttribute : ushort
    {
        IsEnabled = 0x1,
        IsRestartable = 0x2,
        HasReferenceTree = 0x4,
        BubblesChildEnd = 0x8,
        SelectOnce = 0x10,
        IsFSMNode = 0x20,
        TraverseToLeaf = 0x40,
    }


    [Flags]
    public enum WorkFlags : ushort
    {
        IsNotifiedEnd = 0x1,
        HasEvaluated = 0x2,
        HasSelected = 0x4,
        IsCalledActionPrestart = 0x8,
        IsCalledActionStart = 0x10,
        IsNotifiedUnderLayerEnd = 0x20,
        IsBranchState = 0x40,
        IsEndState = 0x80,
        IsStartedSelector = 0x100,
        OverridedSelector = 0x200,
        DuplicatedAction = 0x400,
        IsAsRestartable = 0x800,
    }


    public struct BHVTId
    {
        public ushort id;
        public byte ukn;
        public byte idType;

        public static explicit operator BHVTId(uint value)
        {
            BHVTId result = new();
            Unsafe.As<BHVTId, uint>(ref result) = value;
            return result;
        }

        public static explicit operator uint(BHVTId value)
        {
            uint result = 0;
            Unsafe.As<uint, BHVTId>(ref result) = value;
            return result;
        }
    }


    public class TAGS : BaseModel
    {
        public int tagsCount;  // listSize=1
        public List<uint> Tags { get; } = new();  // taghash list

        protected override bool DoRead(FileHandler handler)
        {
            // if !finished
            handler.Read(ref tagsCount);
            for (int i = 0; i < tagsCount; i++)
            {
                Tags.Add(handler.ReadUInt());
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public abstract class TwoDimensionCountainer : BaseModel
    {
        public abstract int FieldCount { get; }
        public int Count { get; set; }

        protected override bool DoRead(FileHandler handler)
        {
            Count = handler.ReadInt();
            if (Count <= 0) return true;
            if (Count > 1024) throw new InvalidDataException($"{this}.{nameof(Count)} {Count} > 1024");
            uint[,] data = new uint[FieldCount, Count];
            handler.ReadArray(data);
            return LoadData(data);
        }

        protected override bool DoWrite(FileHandler handler)
        {
            uint[,] data = DumpData();
            handler.Write(Count);
            return handler.WriteArray(data);
        }

        protected abstract bool LoadData(uint[,] data);
        protected abstract uint[,] DumpData();
    }


    public class NAction
    {
        public uint Action;
        public uint ActionEx;
    }


    public class NActions : TwoDimensionCountainer
    {
        public override int FieldCount => 2;
        public NAction[] Actions { get; set; } = [];

        protected override bool LoadData(uint[,] data)
        {
            NAction[] actions = new NAction[Count];
            for (int i = 0; i < Count; i++)
            {
                actions[i] = new NAction
                {
                    Action = data[0, i],
                    ActionEx = data[1, i],
                };
            }
            Actions = actions;
            return true;
        }

        protected override uint[,] DumpData()
        {
            Count = Actions.Length;
            uint[,] data = new uint[FieldCount, Count];
            for (int i = 0; i < Count; i++)
            {
                data[0, i] = Actions[i].Action;
                data[1, i] = Actions[i].ActionEx;
            }
            return data;
        }
    }


    public class NChild
    {
        public uint ChildNode;
        public uint ChildNodeEx;
        public uint Conditions;
    }


    public class NChilds : TwoDimensionCountainer
    {
        public override int FieldCount => 3;
        public NChild[] Children { get; set; } = [];

        protected override bool LoadData(uint[,] data)
        {
            NChild[] children = new NChild[Count];
            for (int i = 0; i < Count; i++)
            {
                children[i] = new NChild
                {
                    ChildNode = data[0, i],
                    ChildNodeEx = data[1, i],
                    Conditions = data[2, i]
                };
            }
            Children = children;
            return true;
        }

        protected override uint[,] DumpData()
        {
            Count = Children.Length;
            uint[,] data = new uint[FieldCount, Count];
            for (int i = 0; i < Count; i++)
            {
                var child = Children[i];
                data[0, i] = child.ChildNode;
                data[1, i] = child.ChildNodeEx;
                data[2, i] = child.Conditions;
            }
            return data;
        }
    }


    public class NState
    {
        public BHVTId mStates;
        public uint mTransitions;
        public BHVTId TransitionConditions;
        public uint TransitionMaps;
        public uint mTransitionAttributes;
        public uint mStatesEx;
    }


    public class NStates : TwoDimensionCountainer
    {
        public override int FieldCount => 6;
        public NState[] States { get; set; } = [];

        protected override bool LoadData(uint[,] data)
        {
            NState[] states = new NState[Count];
            for (int i = 0; i < Count; i++)
            {
                states[i] = new NState
                {
                    mStates = (BHVTId)data[0, i],
                    mTransitions = data[1, i],
                    TransitionConditions = (BHVTId)data[2, i],
                    TransitionMaps = data[3, i],
                    mTransitionAttributes = data[4, i],
                    mStatesEx = data[5, i],
                };
            }
            States = states;
            return true;
        }

        protected override uint[,] DumpData()
        {
            Count = States.Length;
            uint[,] data = new uint[FieldCount, Count];
            for (int i = 0; i < Count; i++)
            {
                var state = States[i];
                data[0, i] = (uint)state.mStates;
                data[1, i] = state.mTransitions;
                data[2, i] = (uint)state.TransitionConditions;
                data[3, i] = state.TransitionMaps;
                data[4, i] = state.mTransitionAttributes;
                data[5, i] = state.mStatesEx;
            }
            return data;
        }
    }


    public class NAllState
    {
        public uint mAllState;
        public BHVTId mAllTransition;
        public uint mAllTransitionID;
        public uint mAllStateEx;
        public BHVTId mAllTransitionAttributes;
    }


    public class NAllStates : TwoDimensionCountainer
    {
        public override int FieldCount => 5;
        public NAllState[] AllStates { get; set; } = [];

        protected override bool LoadData(uint[,] data)
        {
            NAllState[] states = new NAllState[Count];
            for (int i = 0; i < Count; i++)
            {
                states[i] = new NAllState
                {
                    mAllState = data[0, i],
                    mAllTransition = (BHVTId)data[1, i],
                    mAllTransitionID = data[2, i],
                    mAllStateEx = data[3, i],
                    mAllTransitionAttributes = (BHVTId)data[4, i],
                };
            }
            AllStates = states;
            return true;
        }

        protected override uint[,] DumpData()
        {
            Count = AllStates.Length;
            uint[,] data = new uint[FieldCount, Count];
            for (int i = 0; i < Count; i++)
            {
                var state = AllStates[i];
                data[0, i] = state.mAllState;
                data[1, i] = (uint)state.mAllTransition;
                data[2, i] = state.mAllTransitionID;
                data[3, i] = state.mAllStateEx;
                data[4, i] = (uint)state.mAllTransitionAttributes;
            }
            return data;
        }
    }


    public class NTransition
    {
        public uint mStartTransitionEvent;
        public uint mStartState;
        public BHVTId mStartStateTransition;
        public uint mStartStateEx;
    }


    public class NTransitions(GameVersion version) : TwoDimensionCountainer
    {
        public NTransition[] Transitions { get; set; } = [];

        public GameVersion Version { get; } = version;

        public override int FieldCount
        {
            get
            {
                if (Version == GameVersion.re8 || Version == GameVersion.mhrise)
                {
                    throw new NotImplementedException();
                }
                else if (Version == GameVersion.re2 || Version == GameVersion.dmc5)
                {
                    return 3;
                }
                return 4;
            }
        }

        protected override bool LoadData(uint[,] data)
        {
            NTransition[] transitions = new NTransition[Count];
            if (Version == GameVersion.re8 || Version == GameVersion.mhrise)
            {
                // TODO fakeStateList
            }
            for (int i = 0; i < Count; i++)
            {
                transitions[i] = new NTransition
                {
                    mStartTransitionEvent = data[0, i],
                    mStartState = data[1, i],
                    mStartStateTransition = (BHVTId)data[2, i]
                };
                if (!(Version == GameVersion.re2 || Version == GameVersion.dmc5))
                {
                    transitions[i].mStartStateEx = data[3, i];
                }
            }
            Transitions = transitions;
            return true;
        }

        protected override uint[,] DumpData()
        {
            Count = Transitions.Length;
            uint[,] data = new uint[FieldCount, Count];
            for (int i = 0; i < Count; i++)
            {
                var transition = Transitions[i];
                data[0, i] = transition.mStartTransitionEvent;
                data[1, i] = transition.mStartState;
                data[2, i] = (uint)transition.mStartStateTransition;
                if (!(Version == GameVersion.re2 || Version == GameVersion.dmc5))
                {
                    data[3, i] = transition.mStartStateEx;
                }
            }
            return data;
        }
    }


    public class BHVTNode : BaseModel
    {
        public uint ID;
        public uint exID;
        public int nameOffset;
        public string Name { get; set; } = string.Empty;
        public uint Parent;
        public uint ParentEx;
        public NChilds NChilds { get; } = new();
        public BHVTId SelectorID;
        public int SelectorCallersCount;
        public BHVTId[] SelectorCallers = [];
        public BHVTId SelectorCallerConditionID;
        public NActions Actions { get; } = new();
        public int Priority;
        public NodeAttribute NodeAttribute;
        public WorkFlags WorkFlags;
        public uint mNameHash;
        public uint mFullnameHash;
        public TAGS Tags { get; } = new();
        public bool isBranch;
        public bool isEnd;

        public NStates States { get; } = new();
        public NAllStates AllStates { get; } = new();
        public NTransitions Transitions { get; }
        public int ReferenceTreeIndex;

        public int unknownAI;
        public int AI_Path;

        public BHVTNode(GameVersion version)
        {
            Transitions = new(version);
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref ID);
            handler.Read(ref exID);
            handler.Read(ref nameOffset);
            handler.Read(ref Parent);
            handler.Read(ref ParentEx);
            NChilds.Read(handler);
            handler.Read(ref SelectorID);
            handler.Read(ref SelectorCallersCount);
            if (SelectorCallersCount > 0)
            {
                SelectorCallers = new BHVTId[SelectorCallersCount];
                handler.ReadArray(SelectorCallers);
            }
            handler.Read(ref SelectorCallerConditionID);
            Actions.Read(handler);
            handler.Read(ref Priority);

            bool isAIFile = handler.FilePath != null && handler.FilePath.Contains(".bhvt");
            if (!isAIFile)
            {
                handler.Read(ref NodeAttribute);
                if (NodeAttribute.HasFlag(NodeAttribute.IsFSMNode))
                {
                    handler.Read(ref WorkFlags);
                    handler.Read(ref mNameHash);
                    handler.Read(ref mFullnameHash);

                    Tags.Read(handler);
                    handler.Read(ref isBranch);
                    handler.Read(ref isEnd);
                }
                else
                {
                    handler.Skip(2);
                }

                States.Read(handler);
                Transitions.Read(handler);

                if (!NodeAttribute.HasFlag(NodeAttribute.HasReferenceTree))
                {
                    AllStates.Read(handler);
                }

                handler.Read(ref ReferenceTreeIndex);
            }
            else
            {
                handler.Read(ref unknownAI);
                handler.Read(ref AI_Path);
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public class Zeros : BaseModel
    {
        public int ActionsObjectTableCount;
        public int StaticActionsObjectTableCount;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref ActionsObjectTableCount);
            if (ActionsObjectTableCount > 0)
            {
                uint[] zeroes = handler.ReadArray<uint>(ActionsObjectTableCount);
            }
            handler.Read(ref StaticActionsObjectTableCount);
            if (StaticActionsObjectTableCount > 0)
            {
                uint[] zeroes = handler.ReadArray<uint>(StaticActionsObjectTableCount);
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

        public int NodeCount;
        public List<BHVTNode> Nodes { get; } = new();
        public UVarFile? Variable { get; set; }
        public List<UVarFile> ReferenceTrees { get; } = new();

        protected override bool DoRead()
        {
            ReferenceTrees.Clear();
            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a BHVT file");
            }

            handler.Seek(header.nodeOffset);
            handler.Read(ref NodeCount);
            for (int i = 0; i < NodeCount; i++)
            {
                BHVTNode node = new(header.Version);
                if (!node.Read(handler)) return false;
                Nodes.Add(node);
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

            // mNamePool
            handler.Seek(header.stringOffset);
            List<string> mNamePool = ReadStringPool();

            handler.Seek(header.variableOffset);
            Variable ??= new(Option, handler) { Embedded = true };
            Variable.Read();

            handler.Seek(header.baseVariableOffset);
            int mReferenceTreeCount = handler.ReadInt();
            if (mReferenceTreeCount > 0)
            {
                for (int i = 0; i < mReferenceTreeCount; i++)
                {
                    long referenceTreeOffset = handler.ReadInt64();
                    UVarFile uVarFile = new(Option, handler.WithOffset(referenceTreeOffset));
                    uVarFile.Read();
                    ReferenceTrees.Add(uVarFile);
                }
            }

            return true;
        }

        private List<string> ReadStringPool()
        {
            var handler = FileHandler;
            List<int> offsets = new();
            List<string> stringPool = new();
            int poolSize = handler.ReadInt();
            long start = handler.Tell();
            long end = start + poolSize * 2;
            long current;
            while ((current = handler.Tell()) < end)
            {
                offsets.Add((int)(current - start) / 2);
                string text = handler.ReadWString(jumpBack: false);
                stringPool.Add(text);
                if (text.Length == 0) break;
            }
            return stringPool;
        }

        protected override bool DoWrite()
        {
            throw new NotImplementedException();
        }
    }
}
