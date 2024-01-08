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


    public class NAction
    {
        public uint Action;
        public uint ActionEx;
    }


    public class NActions : BaseModel
    {
        public const int FieldCount = 2;
        public int actionCount;
        public NAction[] Actions { get; set; } = [];

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref actionCount);
            if (actionCount <= 0) return true;
            if (actionCount > 1024) throw new InvalidDataException($"{nameof(actionCount)} {actionCount} > 1024");
            uint[,] data = new uint[FieldCount, actionCount];
            handler.ReadArray(data);
            NAction[] actions = new NAction[actionCount];
            for (int i = 0; i < actionCount; i++)
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

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public class NChild
    {
        public uint ChildNode;
        public uint ChildNodeEx;
        public uint Conditions;
    }


    public class NChilds : BaseModel
    {
        public const int FieldCount = 3;
        public int childCount;
        public NChild[] Children { get; set; } = [];

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref childCount);
            if (childCount <= 0) return true;
            if (childCount > 1024) throw new InvalidDataException($"{nameof(childCount)} {childCount} > 1024");
            uint[,] data = new uint[FieldCount, childCount];
            handler.ReadArray(data);
            NChild[] children = new NChild[childCount];
            for (int i = 0; i < childCount; i++)
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

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
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


    public class NStates : BaseModel
    {
        public const int FieldCount = 6;
        public int stateCount;
        public NState[] States { get; set; } = [];

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref stateCount);
            if (stateCount <= 0) return true;
            if (stateCount > 1024) throw new InvalidDataException($"{nameof(stateCount)} {stateCount} > 1024");
            uint[,] data = new uint[FieldCount, stateCount];
            handler.ReadArray(data);
            NState[] states = new NState[stateCount];
            for (int i = 0; i < stateCount; i++)
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

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
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


    public class NAllStates : BaseModel
    {
        public const int FieldCount = 5;
        public int stateCount;
        public NAllState[] AllStates { get; set; } = [];

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref stateCount);
            if (stateCount <= 0) return true;
            if (stateCount > 1024) throw new InvalidDataException($"{nameof(stateCount)} {stateCount} > 1024");
            uint[,] data = new uint[FieldCount, stateCount];
            handler.ReadArray(data);
            NAllState[] states = new NAllState[stateCount];
            for (int i = 0; i < stateCount; i++)
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

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public class NTransition
    {
        public uint mStartTransitionEvent;
        public uint mStartState;
        public BHVTId mStartStateTransition;
        public uint mStartStateEx;
    }


    public class NTransitions(GameVersion version) : BaseModel
    {
        public int transitionCount;
        public NTransition[] Transitions { get; set; } = [];

        public GameVersion Version { get; } = version;

        public int FieldCount
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

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref transitionCount);
            if (transitionCount <= 0) return true;
            if (transitionCount > 1024) throw new InvalidDataException($"{nameof(transitionCount)} {transitionCount} > 1024");
            uint[,] data = new uint[FieldCount, transitionCount];
            handler.ReadArray(data);
            NTransition[] transitions = new NTransition[transitionCount];
            if (Version == GameVersion.re8 || Version == GameVersion.mhrise)
            {
                // TODO fakeStateList
            }
            for (int i = 0; i < transitionCount; i++)
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

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
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

        protected override bool DoWrite()
        {
            throw new NotImplementedException();
        }
    }
}
