using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ReeLib.Bhvt;
using ReeLib.Common;
using ReeLib.Motfsm2;


namespace ReeLib.Bhvt
{
    public class Header : BaseModel
    {
        public uint magic = BhvtFile.Magic;
        public uint hash;
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

        public long referencePrefabGameObjectsOffset;

        public GameVersion Version { get; set; }

        public Header(GameVersion version)
        {
            Version = version;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref magic);
            handler.Read(ref hash);
            handler.ReadRange(ref nodeOffset, ref resourcePathsOffset);
            if (Version >= GameVersion.re3)
            {
                handler.Read(ref userdataPathsOffset);
            }

            handler.Read(ref variableOffset);
            handler.Read(ref baseVariableOffset);
            handler.Read(ref referencePrefabGameObjectsOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref magic);
            handler.Write(ref hash);
            handler.WriteRange(ref nodeOffset, ref resourcePathsOffset);
            if (Version >= GameVersion.re3)
            {
                handler.Write(ref userdataPathsOffset);
            }

            handler.Write(ref variableOffset);
            handler.Write(ref baseVariableOffset);
            handler.Write(ref referencePrefabGameObjectsOffset);
            return true;
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

        public BHVTId(int id, bool isStatic)
        {
            this.id = (ushort)id;
            this.idType = isStatic ? (byte)64 : (byte)0;
        }

        public bool HasValue => id != ushort.MaxValue;

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

        public override string ToString() => $"{id} {ukn} {idType}";
    }

    public abstract class TwoDimensionContainer : BaseModel
    {
        internal abstract int FieldCount { get; }
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
            if (Count == 0) return true;
            return handler.WriteArray(data);
        }

        protected abstract bool LoadData(uint[,] data);
        protected abstract uint[,] DumpData();
    }


    public class NAction
    {
        public uint Action;
        public uint ActionEx;

        public RszInstance? Instance { get; set; }

        public override string ToString() => $"<{ActionEx}> {Instance?.ToString() ?? "NULL"}";
    }


    public class NActions : TwoDimensionContainer
    {
        internal override int FieldCount => 2;
        public List<NAction> Actions { get; set; } = [];

        protected override bool LoadData(uint[,] data)
        {
            Actions.Clear();
            for (int i = 0; i < Count; i++)
            {
                Actions.Add(new NAction
                {
                    Action = data[0, i],
                    ActionEx = data[1, i],
                });
            }
            return true;
        }

        protected override uint[,] DumpData()
        {
            Count = Actions.Count;
            uint[,] data = new uint[FieldCount, Count];
            for (int i = 0; i < Count; i++)
            {
                data[0, i] = Actions[i].Action;
                data[1, i] = Actions[i].ActionEx;
            }
            return data;
        }

        public override string ToString() => $"Actions ({Actions.Count})";
    }


    public class NChild
    {
        internal NodeID ChildId;
        internal BHVTId ConditionID;

        public BHVTNode? ChildNode { get; set; }
        public RszInstance? Condition { get; set; }

        public override string ToString() => $"{ChildNode} [{Condition?.ToString() ?? "-"}]";
    }


    public class NChilds : TwoDimensionContainer
    {
        internal override int FieldCount => 3;
        public List<NChild> Children { get; } = [];

        protected override bool LoadData(uint[,] data)
        {
            Children.Clear();
            for (int i = 0; i < Count; i++)
            {
                Children.Add(new NChild
                {
                    ChildId = new NodeID { ID = data[0, i], exID = data[1, i]},
                    ConditionID = (BHVTId)data[2, i]
                });
            }
            return true;
        }

        protected override uint[,] DumpData()
        {
            Count = Children.Count;
            uint[,] data = new uint[FieldCount, Count];
            for (int i = 0; i < Count; i++)
            {
                var child = Children[i];
                data[0, i] = child.ChildId.ID;
                data[1, i] = child.ChildId.exID;
                data[2, i] = (uint)child.ConditionID;
            }
            return data;
        }
    }


    public class NState
    {
        public List<BHVTId> mStates = new();
        internal NodeID targetNodeID;
        internal BHVTId transitionConditionID;
        public uint transitionMapID;
        public uint mStatesEx;

        public BHVTNode? TargetNode { get; set; }
        public RszInstance? Condition { get; set; }
        public TransitionData? TransitionData { get; set; }
    }


    public class NStates : BaseModel
    {
        public List<NState> States { get; } = [];

        protected override bool DoRead(FileHandler handler)
        {
            States.Clear();
            var count = handler.Read<int>();
            for (int i = 0; i < count; i++)
            {
                var subcount = handler.Read<int>();
                var state = new NState();
                state.mStates.ReadStructList(handler, subcount);
                States.Add(state);
            }
            if (count == 0) return true;
            uint[,] data = new uint[5, count];
            handler.ReadArray(data);

            for (int i = 0; i < count; i++)
            {
                var state = States[i];
                state.targetNodeID = new NodeID(data[0, i], data[3, i]);
                state.transitionConditionID = (BHVTId)data[1, i];
                state.transitionMapID = data[2, i];
                state.mStatesEx = data[4, i];
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(States.Count);
            if (States.Count == 0) return true;
            uint[,] data = new uint[5, States.Count];
            for (int i = 0; i < States.Count; i++)
            {
                var state = States[i];
                handler.Write(state.mStates.Count);
                state.mStates.Write(handler);
                data[0, i] = state.targetNodeID.ID;
                data[1, i] = (uint)state.transitionConditionID;
                data[2, i] = state.transitionMapID;
                data[3, i] = state.targetNodeID.exID;
                data[4, i] = state.mStatesEx;
            }
            handler.WriteArray(data);
            return true;
        }
    }


    public class NAllState
    {
        internal NodeID targetNodeId;
        internal BHVTId transitionConditionId;
        public uint transitionMapID;
        public uint mAllTransitionAttributes;

        public BHVTNode? TargetState { get; set; }
        public TransitionData? TransitionData { get; set; }
        public RszInstance? Condition { get; set; }
    }


    public class NAllStates : TwoDimensionContainer
    {
        internal override int FieldCount => 5;
        public List<NAllState> AllStates { get; set; } = [];

        protected override bool LoadData(uint[,] data)
        {
            AllStates.Clear();
            for (int i = 0; i < Count; i++)
            {
                AllStates.Add(new NAllState
                {
                    targetNodeId = new NodeID(data[0, i], data[3, i]),
                    transitionConditionId = (BHVTId)data[1, i],
                    transitionMapID = data[2, i],
                    mAllTransitionAttributes = data[4, i],
                });
            }
            return true;
        }

        protected override uint[,] DumpData()
        {
            Count = AllStates.Count;
            uint[,] data = new uint[FieldCount, Count];
            for (int i = 0; i < Count; i++)
            {
                var state = AllStates[i];
                data[0, i] = state.targetNodeId.ID;
                data[1, i] = (uint)state.transitionConditionId;
                data[2, i] = state.transitionMapID;
                data[3, i] = state.targetNodeId.exID;
                data[4, i] = state.mAllTransitionAttributes;
            }
            return data;
        }
    }


    public class NTransition
    {
        public List<uint> transitionEvents { get; } = new();
        internal uint startNodeId;
        internal BHVTId conditionId;
        internal uint startNodeExId;

        public BHVTNode? StartNode { get; set; }
        public RszInstance? Condition { get; set; }

        public override string ToString() => $" => {StartNode?.ToString() ?? "NULL"}";
    }


    public class NTransitions(GameVersion version) : BaseModel
    {
        public List<NTransition> Transitions { get; } = [];

        public GameVersion Version { get; } = version;

        protected override bool DoRead(FileHandler handler)
        {
            Transitions.Clear();
            var count = handler.Read<int>();
            if (count == 0) return true;

            for (int i = 0; i < count; i++)
            {
                var transition = new NTransition();
                if (Version >= GameVersion.re3)
                {
                    var eventCount = handler.Read<int>();
                    transition.transitionEvents.ReadStructList(handler, eventCount);
                    // note: I haven't seen any of the stateNodeIDs be not 0, so I'm not 100% sure what this actually is
                    DataInterpretationException.ThrowIfNotEqualValues<uint>(CollectionsMarshal.AsSpan(transition.transitionEvents));
                }
                Transitions.Add(transition);
            }

            uint[,] data = new uint[3, count];
            handler.ReadArray(data);
            for (int i = 0; i < count; i++)
            {
                Transitions[i].startNodeId = data[0, i];
                Transitions[i].conditionId = (BHVTId)data[1, i];
                Transitions[i].startNodeExId = data[2, i];
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(Transitions.Count);
            if (Transitions.Count == 0) return true;
            var hasStartEx = Version >= GameVersion.re3;
            uint[,] data = new uint[3, Transitions.Count];
            for (int i = 0; i < Transitions.Count; i++)
            {
                var transition = Transitions[i];
                if (Version >= GameVersion.re3)
                {
                    handler.Write(transition.transitionEvents.Count);
                    transition.transitionEvents.Write(handler);
                }
                data[0, i] = (uint)transition.startNodeId;
                data[1, i] = (uint)transition.conditionId;
                data[2, i] = (uint)transition.startNodeExId;
            }
            handler.WriteArray(data);
            return true;
        }
    }

    public struct NodeID
    {
        public uint ID;
        public uint exID;

        public static readonly NodeID Unset = new NodeID(uint.MaxValue, 0);

        public NodeID(uint id, uint exID)
        {
            ID = id;
            this.exID = exID;
        }

        public readonly bool IsUnset => ID == uint.MaxValue;

        public static explicit operator ulong(NodeID value)
        {
            ulong result = 0;
            Unsafe.As<ulong, NodeID>(ref result) = value;
            return result;
        }

        public override string ToString() => $"{ID} <{exID}>";
    }

    public class BHVTNode : BaseModel
    {
        public NodeID ID;
        internal int nameOffset;
        public string Name { get; set; } = string.Empty;
        public NodeID ParentID;
        internal int SelectorID;
        internal List<BHVTId> SelectorCallerIDs { get; } = new();
        public BHVTId SelectorCallerConditionID;
        public int Priority;
        public NodeAttribute Attributes = NodeAttribute.IsEnabled|NodeAttribute.IsRestartable|NodeAttribute.IsFSMNode;
        internal int ReferenceTreeIndex;

        public NActions Actions { get; } = new();
        public RszInstance? Selector { get; set; }
        public RszInstance? SelectorCallerCondition { get; set; }
        public List<RszInstance> SelectorCallers { get; } = new();
        public NChilds Children { get; } = new();

        public WorkFlags WorkFlags;
        public uint mNameHash;
        public uint mFullnameHash;
        public List<uint> Tags { get; } = new();
        public bool isBranch;
        public bool isEnd;
        public NStates States { get; } = new();
        public NTransitions Transitions { get; }
        public NAllStates AllStates { get; } = new();
        public UVarFile? ReferenceTree { get; set; }

        public int unknownAI;
        public int AI_Path;

        public override string ToString() => Name;

        public BHVTNode(GameVersion version)
        {
            Transitions = new(version);
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref ID);
            handler.Read(ref nameOffset);
            handler.Read(ref ParentID);
            Children.Read(handler);
            handler.Read(ref SelectorID);
            int callersCount = handler.Read<int>();
            SelectorCallerIDs.Clear();
            SelectorCallerIDs.ReadStructList(handler, callersCount);
            handler.Read(ref SelectorCallerConditionID);
            Actions.Read(handler);
            handler.Read(ref Priority);

            bool isAIFile = handler.FilePath != null && handler.FilePath.Contains(".bhvt");
            if (!isAIFile)
            {
                handler.Read(ref Attributes);
                if (Attributes.HasFlag(NodeAttribute.IsFSMNode))
                {
                    handler.Read(ref WorkFlags);
                    handler.Read(ref mNameHash);
                    handler.Read(ref mFullnameHash);

                    Tags.ReadStructList(handler, handler.Read<int>());
                    handler.Read(ref isBranch);
                    handler.Read(ref isEnd);
                }
                else
                {
                    handler.ReadNull(2);
                }

                States.Read(handler);
                Transitions.Read(handler);

                if (!Attributes.HasFlag(NodeAttribute.HasReferenceTree))
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

        public void ReadName(FileHandler handler, long namePoolStart)
        {
            // 前面4字节是pool长度
            Name = handler.ReadWString(namePoolStart + 4 + nameOffset * 2);
        }

        public void WriteName(FileHandler handler)
        {
            nameOffset = handler.StringTableAdd(Name, false).TableOffset;
            handler.Write(Start + Marshal.SizeOf<NodeID>(), nameOffset);
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref ID);
            handler.Write(ref nameOffset);
            handler.Write(ref ParentID);
            Children.Write(handler);
            handler.Write(ref SelectorID);
            handler.Write(SelectorCallerIDs.Count);
            SelectorCallerIDs.Write(handler);
            handler.Write(ref SelectorCallerConditionID);
            Actions.Write(handler);
            handler.Write(ref Priority);

            bool isAIFile = handler.FilePath != null && handler.FilePath.Contains(".bhvt");
            if (!isAIFile)
            {
                handler.Write(ref Attributes);
                if (Attributes.HasFlag(NodeAttribute.IsFSMNode))
                {
                    handler.Write(ref WorkFlags);
                    handler.Write(ref mNameHash);
                    handler.Write(ref mFullnameHash);
                    handler.Write(Tags.Count);
                    Tags.Write(handler);
                    handler.Write(ref isBranch);
                    handler.Write(ref isEnd);
                }
                else
                {
                    handler.WriteNull(2);
                }

                States.Write(handler);
                Transitions.Write(handler);

                if (!Attributes.HasFlag(NodeAttribute.HasReferenceTree))
                {
                    AllStates.Write(handler);
                }

                handler.Write(ref ReferenceTreeIndex);
            }
            else
            {
                handler.Write(ref unknownAI);
                handler.Write(ref AI_Path);
            }
            return true;
        }
    }


    public class BhvtObjectIndexTable : BaseModel
    {
        public int[] ActionsObjectTable = [];
        public int[] StaticActionsObjectTable = [];

        protected override bool DoRead(FileHandler handler)
        {
            var actionsCount = handler.Read<int>();
            ActionsObjectTable = handler.ReadArray<int>(actionsCount);
            var staticActionsCount = handler.Read<int>();
            StaticActionsObjectTable = handler.ReadArray<int>(staticActionsCount);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ActionsObjectTable.Length);
            handler.WriteArray(ActionsObjectTable);
            handler.Write(StaticActionsObjectTable.Length);
            handler.WriteArray(StaticActionsObjectTable);
            return true;
        }
    }

    public class BhvtGameObjectReference : BaseModel
    {
        public Guid guid;
        public int[] values = [];

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guid);
            var count = handler.Read<int>();
            values = handler.ReadArray<int>(count);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref guid);
            handler.Write(values.Length);
            handler.WriteArray<int>(values);
            return true;
        }

        public override string ToString() => guid.ToString();
    }
}


namespace ReeLib
{
    public class BhvtFile(RszFileOption option, FileHandler fileHandler) : BaseRszFile(option, fileHandler)
    {
        public const uint Magic = 0x54564842;
        public const string Extension = ".bhvt";

        public Header Header { get; } = new Header(option.Version);
        /// <summary>
        /// List of via.behaviortree.Action
        /// </summary>
        public RSZFile ActionRsz { get; } = new(option, fileHandler);
        /// <summary>
        /// List of via.behaviortree.Selector
        /// </summary>
        public RSZFile SelectorRsz { get; } = new(option, fileHandler);
        /// <summary>
        /// List of via.behaviortree.SelectorCaller
        /// </summary>
        public RSZFile SelectorCallerRsz { get; } = new(option, fileHandler);
        /// <summary>
        /// List of via.behaviortree.Condition
        /// </summary>
        public RSZFile ConditionsRsz { get; } = new(option, fileHandler);
        /// <summary>
        /// List of via.behaviortree.TransitionEvent
        /// </summary>
        public RSZFile TransitionEventRsz { get; } = new(option, fileHandler);
        public RSZFile ExpressionTreeConditionsRsz { get; } = new(option, fileHandler);
        /// <summary>
        /// List of "static" via.behaviortree.Action
        /// </summary>
        public RSZFile StaticActionRsz { get; } = new(option, fileHandler);
        public RSZFile StaticSelectorCallerRsz { get; } = new(option, fileHandler);
        public RSZFile StaticConditionsRsz { get; } = new(option, fileHandler);
        public RSZFile StaticTransitionEventRsz { get; } = new(option, fileHandler);
        public RSZFile StaticExpressionTreeConditionsRsz { get; } = new(option, fileHandler);

        public List<BHVTNode> Nodes { get; } = new();
        public BHVTNode RootNode { get; private set; } = new BHVTNode(option.Version) { Name = "root", ParentID = new NodeID(uint.MaxValue, 0) };
        public UVarFile Variable { get; } = new UVarFile(fileHandler) { Embedded = true };
        public List<UVarFile> ReferenceTrees { get; } = new();
        public BhvtObjectIndexTable ActionObjectTable { get; set; } = new ();
        // only used in fsmv2 files
        public List<BhvtGameObjectReference> GameObjectReferences { get; set; } = new ();

        private const int Condition_IDFieldIndex = 0;
        private const int Action_IDFieldIndex = 1;

        private IEnumerable<RSZFile> GetRSZFiles()
        {
            yield return ActionRsz;
            yield return StaticActionRsz;
            yield return ConditionsRsz;
            yield return StaticConditionsRsz;
            yield return SelectorCallerRsz;
            yield return StaticSelectorCallerRsz;
            yield return SelectorRsz;
            yield return TransitionEventRsz;
            yield return StaticTransitionEventRsz;
            yield return ExpressionTreeConditionsRsz;
            yield return StaticExpressionTreeConditionsRsz;
        }

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

            Nodes.Clear();

            handler.Seek(header.nodeOffset);
            var nodeCount = handler.Read<int>();
            for (int i = 0; i < nodeCount; i++)
            {
                BHVTNode node = new(header.Version);
                if (!node.Read(handler)) return false;
                node.ReadName(handler, header.stringOffset);
                Nodes.Add(node);
            }

            ActionObjectTable.Read(handler);

            ReadRsz(ActionRsz, header.actionOffset);
            ReadRsz(SelectorRsz, header.selectorOffset);
            ReadRsz(SelectorCallerRsz, header.selectorCallerOffset);
            ReadRsz(ConditionsRsz, header.conditionsOffset);
            ReadRsz(TransitionEventRsz, header.transitionEventOffset);
            ReadRsz(ExpressionTreeConditionsRsz, header.expressionTreeConditionsOffset);
            ReadRsz(StaticActionRsz, header.staticActionOffset);
            ReadRsz(StaticSelectorCallerRsz, header.staticSelectorCallerOffset);
            ReadRsz(StaticConditionsRsz, header.staticConditionsOffset);
            ReadRsz(StaticTransitionEventRsz, header.staticTransitionEventOffset);
            ReadRsz(StaticExpressionTreeConditionsRsz, header.staticExpressionTreeConditionsOffset);

            handler.Seek(header.variableOffset);
            Variable.FileHandler = handler;
            Variable.ReadNoSeek();

            handler.Seek(header.baseVariableOffset);
            int mReferenceTreeCount = handler.ReadInt();
            if (mReferenceTreeCount > 0)
            {
                for (int i = 0; i < mReferenceTreeCount; i++)
                {
                    long referenceTreeOffset = handler.ReadInt64();
                    var curOffset = handler.Tell();
                    handler.Seek(referenceTreeOffset);
                    UVarFile uVarFile = new(handler);
                    uVarFile.ReadNoSeek();
                    ReferenceTrees.Add(uVarFile);
                    handler.Seek(curOffset);
                }
            }

            handler.Seek(header.referencePrefabGameObjectsOffset);
            var refPfbCount = handler.Read<int>();
            GameObjectReferences.Read(handler, refPfbCount);

            SetupReferences();
            return true;
        }

        protected override bool DoWrite()
        {
            FlattenInstances();

            var handler = FileHandler;
            var header = Header;
            header.Write(handler);
            header.nodeOffset = handler.Tell();
            handler.Write(Nodes.Count);
            foreach (var node in Nodes)
            {
                node.Write(handler);
            }

            foreach (var node in Nodes) node.WriteName(handler);

            ActionObjectTable.Write(handler);

            WriteRsz(ActionRsz, header.actionOffset = Utils.Align16((int)handler.Tell()));
            WriteRsz(SelectorRsz, header.selectorOffset = Utils.Align16((int)handler.Tell()));
            WriteRsz(SelectorCallerRsz, header.selectorCallerOffset = Utils.Align16((int)handler.Tell()));
            WriteRsz(ConditionsRsz, header.conditionsOffset = Utils.Align16((int)handler.Tell()));
            WriteRsz(TransitionEventRsz, header.transitionEventOffset = Utils.Align16((int)handler.Tell()));
            WriteRsz(ExpressionTreeConditionsRsz, header.expressionTreeConditionsOffset = Utils.Align16((int)handler.Tell()));
            WriteRsz(StaticActionRsz, header.staticActionOffset = Utils.Align16((int)handler.Tell()));
            WriteRsz(StaticSelectorCallerRsz, header.staticSelectorCallerOffset = Utils.Align16((int)handler.Tell()));
            WriteRsz(StaticConditionsRsz, header.staticConditionsOffset = Utils.Align16((int)handler.Tell()));
            WriteRsz(StaticTransitionEventRsz, header.staticTransitionEventOffset = Utils.Align16((int)handler.Tell()));
            WriteRsz(StaticExpressionTreeConditionsRsz, header.staticExpressionTreeConditionsOffset = Utils.Align16((int)handler.Tell()));

            handler.Align(16);
            header.referencePrefabGameObjectsOffset = handler.Tell();
            handler.Write(GameObjectReferences.Count);
            GameObjectReferences.Write(handler);

            handler.Align(16);
            header.stringOffset = handler.Tell();
            handler.Skip(4);
            handler.StringTableFlush();
            handler.Write(header.stringOffset, (int)(handler.Tell() - header.stringOffset) / 2);

            header.resourcePathsOffset = handler.Tell();
            handler.Write(0); // TODO

            header.userdataPathsOffset = handler.Tell();
            handler.Write(0); // TODO

            header.variableOffset = handler.Tell();
            Variable.FileHandler = handler;
            Variable.WriteNoSeek();

            header.baseVariableOffset = handler.Tell();
            handler.Write(ReferenceTrees.Count);
            handler.Skip(ReferenceTrees.Count * sizeof(long));
            for (int i = 0; i < ReferenceTrees.Count; ++i)
            {
                handler.Align(16);
                handler.Write(header.baseVariableOffset + 4 + i * sizeof(long), handler.Tell());
                var tree = ReferenceTrees[i];

                tree.FileHandler = handler;
                tree.WriteNoSeek();
            }

            header.Write(handler, 0);
            return true;
        }

        private void SetupReferences()
        {
            var version = Option.Version;
            var nodeDict = Nodes.ToDictionary(n => (ulong)n.ID);
            var actions = new Dictionary<ulong, RszInstance>();
            foreach (var act in ActionRsz.ObjectList) {
                var id = Convert.ToUInt32(act.Values[Action_IDFieldIndex]);
                var exId = 0u;
                while (!actions.TryAdd((ulong)id | ((ulong)exId << 32), act)) {
                    exId++;
                }
            }

            var flagPairs = new Dictionary<NodeAttribute, int>();
            foreach (var node in Nodes)
            {
                foreach (var ch in node.Children.Children)
                {
                    var other = nodeDict[(ulong)ch.ChildId];
                    ch.ChildNode = other;
                    if (ch.ConditionID.HasValue)
                    {
                        ch.Condition = ch.ConditionID.idType == 64
                            ? StaticConditionsRsz.ObjectList[ch.ConditionID.id]
                            : ConditionsRsz.ObjectList[ch.ConditionID.id];

                        DataInterpretationException.DebugWarnIf(ch.ConditionID.idType == 64 && !StaticClasses.Contains(ch.Condition.RszClass.name), ch.Condition.RszClass.name);
                    }
                }

                flagPairs[node.Attributes] = flagPairs.GetValueOrDefault(node.Attributes) + 1;
                DataInterpretationException.DebugThrowIf(node.ReferenceTreeIndex == -1);

                if (node.ParentID.IsUnset) RootNode = node;

                if (node.SelectorID != -1)
                {
                    node.Selector = SelectorRsz.ObjectList[node.SelectorID];
                }
                if (node.SelectorCallerConditionID.HasValue)
                {
                    node.SelectorCallerCondition = node.SelectorCallerConditionID.idType == 64
                            ? StaticConditionsRsz.ObjectList[node.SelectorCallerConditionID.id]
                            : ConditionsRsz.ObjectList[node.SelectorCallerConditionID.id];
                }

                foreach (var callerId in node.SelectorCallerIDs)
                {
                    DataInterpretationException.DebugThrowIf(callerId.idType != 0);
                    var instance = SelectorCallerRsz.ObjectList[callerId.id];
                    node.SelectorCallers.Add(instance);

                    DataInterpretationException.DebugWarnIf(callerId.idType != 0, instance.RszClass.name);
                }
            }

            // DataInterpretationException.DebugThrowIf(nextRefTree != ReferenceTrees.Count);

            var isBhvt = FileHandler.FilePath?.Contains(".bhvt") == true;
            if (isBhvt)
            {
                var actionsDict = ActionRsz.ObjectList.ToDictionary(a => Convert.ToUInt32(a.Values[Action_IDFieldIndex]));
                var staticActionsDict = StaticActionRsz.ObjectList.ToDictionary(a => Convert.ToUInt32(a.Values[Action_IDFieldIndex]));
                foreach (var node in Nodes)
                {
                    // unlike the motfsm2 version, this one isn't consistent about order
                    // also unlike the motfsm2 version, the IDs seem actually unique here so we can just dictionary

                    foreach (var act in node.Actions.Actions)
                    {
                        if (actionsDict.TryGetValue(act.Action, out var instance)) {
                            act.Instance = instance;
                            DataInterpretationException.DebugThrowIf(StaticClasses.Contains(instance.RszClass.name));
                        } else if (staticActionsDict.TryGetValue(act.Action, out instance)) {
                            act.Instance = instance;
                            DataInterpretationException.DebugWarnIf(!StaticClasses.Contains(instance.RszClass.name), instance.RszClass.name);
                        } else {
                            throw new InvalidDataException("BHVT file missing action ID " + act.Action);
                        }
                    }
                }
            }
            else
            {
                int nextActionIndex = 0;
                int nextStaticActionIndex = 0;
                foreach (var node in Nodes)
                {
                    // match up the actions by order and not by hash/ID
                    // there are many cases of files with "duplicated" IDs that would otherwise get lost (likely due to motion banks)
                    // the exID also isn't indicative because it's sometimes > 0 while there's only one action with the given ID
                    // the motfsm2 bhvt serializer seems to be consistent about the order so probably the best solution

                    foreach (var act in node.Actions.Actions)
                    {
                        var instance = nextActionIndex < ActionRsz.ObjectList.Count ? ActionRsz.ObjectList[nextActionIndex] : null;
                        if (instance != null && Convert.ToUInt32(instance.Values[Action_IDFieldIndex]) == act.Action) {
                            act.Instance = instance;
                            nextActionIndex++;
                            DataInterpretationException.DebugThrowIf(StaticClasses.Contains(instance.RszClass.name));
                        } else {
                            instance = StaticActionRsz.ObjectList[nextStaticActionIndex++];
                            act.Instance = instance;
                            DataInterpretationException.DebugThrowIf(!StaticClasses.Contains(instance.RszClass.name));
                        }
                        DataInterpretationException.ThrowIfDifferent(act.Action, Convert.ToUInt32(instance.Values[Action_IDFieldIndex]));
                    }

                    foreach (var state in node.States.States)
                    {
                        state.TargetNode = nodeDict[(ulong)state.targetNodeID];

                        if (state.transitionConditionID.HasValue)
                        {
                            state.Condition = state.transitionConditionID.idType == 64
                                ? StaticConditionsRsz.ObjectList[state.transitionConditionID.id]
                                : ConditionsRsz.ObjectList[state.transitionConditionID.id];
                        }
                    }

                    foreach (var trans in node.Transitions.Transitions)
                    {
                        if (trans.startNodeId != uint.MaxValue)
                        {
                            var startTargetId = version <= GameVersion.dmc5 ? (ulong)trans.startNodeId : ((uint)trans.startNodeId | ((ulong)trans.startNodeExId << 32));
                            if (!nodeDict.TryGetValue(startTargetId, out var startNode))
                                throw new InvalidDataException("Could not find transition start node " + trans.startNodeId);

                            trans.StartNode = startNode;
                        }

                        if (trans.conditionId.HasValue)
                        {
                            trans.Condition = trans.conditionId.idType == 64
                                ? StaticConditionsRsz.ObjectList[trans.conditionId.id]
                                : ConditionsRsz.ObjectList[trans.conditionId.id];
                        }
                    }

                    foreach (var state in node.AllStates.AllStates)
                    {
                        state.TargetState = nodeDict[state.targetNodeId.ID];

                        if (state.transitionConditionId.HasValue)
                        {
                            state.Condition = state.transitionConditionId.idType == 64
                                ? StaticConditionsRsz.ObjectList[state.transitionConditionId.id]
                                : ConditionsRsz.ObjectList[state.transitionConditionId.id];
                        }
                    }
                }
            }
        }

        private void FlattenInstances()
        {
            var version = Header.Version;
            Nodes.Clear();
            Nodes.Add(RootNode);
            void RecurseHandleChildren(BHVTNode node)
            {
                foreach (var child in node.Children.Children) {
                    if (child.ChildNode == null) continue;

                    Nodes.Add(child.ChildNode);
                }

                // note: the original files have the nodes in an undefined, random order, probably roughly in creation order
                // to make editing easier, we just rebuild the tree anew on save, probably shouldn't cause issues
                foreach (var child in node.Children.Children) {
                    if (child.ChildNode == null) continue;

                    RecurseHandleChildren(child.ChildNode);
                }
            }
            RecurseHandleChildren(RootNode);

            // TODO ensure we store all RSZ instances back in
            foreach (var rsz in GetRSZFiles()) rsz.ClearObjects();

            foreach (var node in Nodes)
            {
                foreach (var ch in node.Children.Children)
                {
                    ch.ChildId = ch.ChildNode?.ID ?? NodeID.Unset;
                    if (ch.Condition != null)
                    {
                        var rsz = StaticClasses.Contains(ch.Condition.RszClass.name) ? StaticConditionsRsz : ConditionsRsz;
                        rsz.AddToObjectTable(ch.Condition);
                        ch.ConditionID = new BHVTId(ch.Condition.ObjectTableIndex, rsz == StaticConditionsRsz);
                    }
                }

                if (node.Selector == null)
                {
                    node.SelectorID = -1;
                }
                else
                {
                    SelectorRsz.AddToObjectTable(node.Selector);
                    node.SelectorID = node.Selector.ObjectTableIndex;
                }

                if (node.SelectorCallerCondition != null)
                {
                    var rsz = StaticClasses.Contains(node.SelectorCallerCondition.RszClass.name) ? StaticConditionsRsz : ConditionsRsz;
                    rsz.AddToObjectTable(node.SelectorCallerCondition);
                    node.SelectorCallerConditionID = new BHVTId(node.SelectorCallerCondition.ObjectTableIndex, rsz == StaticConditionsRsz);
                }

                node.SelectorCallerIDs.Clear();
                foreach (var caller in node.SelectorCallers)
                {
                    var rsz = StaticClasses.Contains(caller.RszClass.name) ? StaticSelectorCallerRsz : SelectorCallerRsz;
                    rsz.AddToObjectTable(caller);
                    node.SelectorCallerIDs.Add(new BHVTId(caller.ObjectTableIndex, false));
                }


                foreach (var act in node.Actions.Actions)
                {
                    if (act.Instance != null)
                    {
                        var rsz = StaticClasses.Contains(act.Instance.RszClass.name) ? StaticActionRsz : ActionRsz;
                        rsz.AddToObjectTable(act.Instance);
                        act.Action = (uint)act.Instance.Values[Action_IDFieldIndex];
                    }
                }

                foreach (var state in node.States.States)
                {
                    state.targetNodeID = state.TargetNode?.ID ?? NodeID.Unset;
                    if (state.TargetNode == null) continue;

                    if (state.Condition != null)
                    {
                        var conditionRsz = StaticClasses.Contains(state.Condition.RszClass.name) ? StaticConditionsRsz : ConditionsRsz;
                        conditionRsz.AddToObjectTable(state.Condition);
                        state.transitionConditionID = new BHVTId(state.Condition.ObjectTableIndex, StaticClasses.Contains(state.Condition.RszClass.name));
                    }
                }

                foreach (var state in node.AllStates.AllStates)
                {
                    if (state.TargetState != null)
                    {
                        state.targetNodeId = state.TargetState.ID;
                    }

                    if (state.Condition != null)
                    {
                        var conditionRsz = StaticClasses.Contains(state.Condition.RszClass.name) ? StaticConditionsRsz : ConditionsRsz;
                        conditionRsz.AddToObjectTable(state.Condition);
                        state.transitionConditionId = new BHVTId(state.Condition.ObjectTableIndex, StaticClasses.Contains(state.Condition.RszClass.name));
                    }
                }

                foreach (var trans in node.Transitions.Transitions)
                {
                    if (trans.StartNode != null)
                    {
                        trans.startNodeId = trans.StartNode.ID.ID;
                        trans.startNodeExId  = trans.StartNode.ID.exID;
                    }

                    if (trans.Condition != null)
                    {
                        var conditionRsz = StaticClasses.Contains(trans.Condition.RszClass.name) ? StaticConditionsRsz : ConditionsRsz;
                        conditionRsz.AddToObjectTable(trans.Condition);
                        trans.conditionId = new BHVTId(trans.Condition.ObjectTableIndex, StaticClasses.Contains(trans.Condition.RszClass.name));
                    }
                }

            }

            foreach (var rsz in GetRSZFiles())
            {
                rsz.RebuildInstanceList();
                rsz.RebuildInstanceInfo();
            }
        }


        private static readonly HashSet<string> StaticClasses = [
            "via.behaviortree.action.SetInt",
            "via.behaviortree.action.SetBool",
            "via.behaviortree.action.SetFloat",
            "via.behaviortree.action.SetTrigger",
            "via.behaviortree.action.AddRotation",
            "via.behaviortree.action.Trace",

            // .motfsm2 statics
            "app.ropeway.enemy.common.fsmv2.action.EmCommonFsmAction_AttackControl",
            "app.ropeway.enemy.common.fsmv2.action.EmCommonMFsmAction_SetCondition",
            "app.ropeway.enemy.common.fsmv2.action.NotifyActionEnd",
            "app.ropeway.enemy.common.fsmv2.action.StartNavigation",
            "app.ropeway.enemy.common.fsmv2.action.EmCommonFsmAction_MovePosition",
            "app.ropeway.enemy.common.fsmv2.action.EmCommonFsmAction_SetGroundState",
            "app.ropeway.enemy.common.fsmv2.action.EmCommonMFsmAction_AttachAssetPackage",
            "app.ropeway.enemy.common.fsmv2.action.EmCommonMFsmAction_GM_CheckPlayerContactedDoor",
            "app.ropeway.enemy.common.fsmv2.action.EmCommonMFsmAction_GM_ConstInteractTarget",
            "app.ropeway.enemy.common.fsmv2.action.EmCommonMFsmAction_GM_Interact",
            "app.ropeway.enemy.common.fsmv2.action.EmCommonMFsmAction_GM_InteractAction",
            "app.ropeway.enemy.common.fsmv2.action.EmCommonMFsmAction_GM_Interpolation",
            "app.ropeway.enemy.common.fsmv2.action.EmCommonMFsmAction_GM_ResetKeepGimmick",
            "app.ropeway.enemy.common.fsmv2.action.EmCommonMFsmAction_GM_SearchInteractTarget",
            "app.ropeway.enemy.common.fsmv2.action.EmCommonMFsmAction_GameOver",
            "app.ropeway.enemy.common.fsmv2.action.EmCommonMFsmAction_GameRankMotionSpeedControl",
            "app.ropeway.enemy.common.fsmv2.action.EmCommonMFsmAction_TransformRagdoll",
            "app.ropeway.enemy.common.fsmv2.action.EmCommonMFsmAction_UsedSupportItem",
            "app.ropeway.enemy.common.fsmv2.action.EnemyCharacterControllerSwitch",
            "app.ropeway.enemy.common.fsmv2.action.HideDeadEnemys",
            "app.ropeway.enemy.common.fsmv2.action.MonitorSupportItem",
            "app.ropeway.enemy.common.fsmv2.action.MonitorSupportItemInvoke",
            "app.ropeway.enemy.common.fsmv2.action.MonitorSupportItemToEnemy",
            "app.ropeway.enemy.common.fsmv2.action.NotifyRoleActionEnd",
            "app.ropeway.enemy.common.fsmv2.action.SetAttackInitiativeEnable",
            "app.ropeway.enemy.em0000.fsmv2.action.Em0000MFsmAction_BiteDamageHit",
            "app.ropeway.enemy.em0000.fsmv2.action.Em0000MFsmAction_CheckKnockingMaterial",
            "app.ropeway.enemy.em0000.fsmv2.action.Em0000MFsmAction_CheckRoleActionExec",
            "app.ropeway.enemy.em0000.fsmv2.action.Em0000MFsmAction_CheckRoleStartFlag",
            "app.ropeway.enemy.em0000.fsmv2.action.Em0000MFsmAction_KnifeConst2Neck",
            "app.ropeway.enemy.em0000.fsmv2.action.Em0000MFsmAction_SetDisorder",
            "app.ropeway.enemy.em0000.fsmv2.action.Em0000MFsmAction_TransformRagdoll",
            "app.ropeway.enemy.em3000.fsmv2.action.Em3000FsmAction_HoldDamageHit",
            "app.ropeway.enemy.em3000.fsmv2.action.Em3000FsmAction_TransformRagdoll",
            "app.ropeway.enemy.em5000.fsmv2.action.Em5000FsmAction_TransformRagdoll",
            "app.ropeway.enemy.em6000.fsmv2.action.Em6000FsmAction_HoldDamageHit",
            "app.ropeway.enemy.em6200.fsmv2.action.Em6200FsmAction_HoldDamageHit",
            "app.ropeway.fsmv2.LookActionTarget",
            "app.ropeway.fsmv2.ResetRagdoll",
            "app.ropeway.fsmv2.ResetRagdollOnThisMotion",
            "app.ropeway.fsmv2.SetMotionIntervalParam",
            "app.ropeway.fsmv2.SyncJackStateAction",
            "app.ropeway.fsmv2.enemy.action.Em0000MFsmAction_Burst",
            "app.ropeway.fsmv2.enemy.stateaction.Em4000FsmAction_HoldDamageHit",
            "offline.enemy.common.fsmv2.action.EmCommonFsmAction_AttackControl",
            "offline.enemy.common.fsmv2.action.EmCommonFsmAction_MovePosition",
            "offline.enemy.common.fsmv2.action.EmCommonFsmAction_SetGroundState",
            "offline.enemy.common.fsmv2.action.EmCommonMFsmAction_AttachAssetPackage",
            "offline.enemy.common.fsmv2.action.EmCommonMFsmAction_GM_CheckPlayerContactedDoor",
            "offline.enemy.common.fsmv2.action.EmCommonMFsmAction_GM_ConstInteractTarget",
            "offline.enemy.common.fsmv2.action.EmCommonMFsmAction_GM_Interact",
            "offline.enemy.common.fsmv2.action.EmCommonMFsmAction_GM_InteractAction",
            "offline.enemy.common.fsmv2.action.EmCommonMFsmAction_GM_ResetKeepGimmick",
            "offline.enemy.common.fsmv2.action.EmCommonMFsmAction_GM_SearchInteractTarget",
            "offline.enemy.common.fsmv2.action.EmCommonMFsmAction_GameOver",
            "offline.enemy.common.fsmv2.action.EmCommonMFsmAction_GameRankMotionSpeedControl",
            "offline.enemy.common.fsmv2.action.EmCommonMFsmAction_SetCondition",
            "offline.enemy.common.fsmv2.action.EmCommonMFsmAction_SetDeadParam",
            "offline.enemy.common.fsmv2.action.EmCommonMFsmAction_TransformRagdoll",
            "offline.enemy.common.fsmv2.action.EmCommonMFsmAction_UsedSupportItem",
            "offline.enemy.common.fsmv2.action.EnemyCharacterControllerSwitch",
            "offline.enemy.common.fsmv2.action.HideDeadEnemys",
            "offline.enemy.common.fsmv2.action.MonitorSupportItem",
            "offline.enemy.common.fsmv2.action.MonitorSupportItemInvoke",
            "offline.enemy.common.fsmv2.action.MonitorSupportItemToEnemy",
            "offline.enemy.common.fsmv2.action.NotifyActionEnd",
            "offline.enemy.common.fsmv2.action.NotifyRoleActionEnd",
            "offline.enemy.common.fsmv2.action.SetAttackInitiativeEnable",
            "offline.enemy.common.fsmv2.action.StartNavigation",
            "offline.enemy.em0000.fsmv2.action.Em0000MFsmAction_CheckKnockingMaterial",
            "offline.enemy.em0000.fsmv2.action.Em0000MFsmAction_CheckRoleActionExec",
            "offline.enemy.em0000.fsmv2.action.Em0000MFsmAction_CheckRoleStartFlag",
            "offline.enemy.em0000.fsmv2.action.Em0000MFsmAction_SetDisorder",
            "offline.enemy.em0000.fsmv2.action.Em0000MFsmAction_TransformRagdoll",
            "offline.enemy.em3000.fsmv2.action.Em3000FsmAction_HoldDamageHit",
            "offline.enemy.em3000.fsmv2.action.Em3000FsmAction_TransformRagdoll",
            "offline.escape.enemy.common.fsmv2.action.EsEm7000MFsmAction_Sleep",
            "offline.escape.enemy.em0000.EsEnemyMFsmAction_BurstStrike",
            "offline.escape.enemy.em0000.EsEnemyMFsmAction_ClearDisorder",
            "offline.escape.enemy.em0000.EsEnemyMFsmAction_Falling",
            "offline.escape.enemy.em0000.EsEnemyMFsmAction_FinishCustomJacked",
            "offline.escape.enemy.em0000.EsEnemyMFsmAction_FinishEvent",
            "offline.escape.enemy.em0000.EsEnemyMFsmAction_RailingDead",
            "offline.escape.enemy.em0000.EsEnemyMFsmAction_WaitAfterWalk",
            "offline.escape.enemy.em0000.EsEnemyMFsmAction_WakeupHold",
            "offline.escape.enemy.em3400.fsmv2.action.EsEm3400MFsmAction_ACT_APPEAR",
            "offline.escape.enemy.em3400.fsmv2.action.EsEm3400MFsmAction_ACT_APPEAR_WAIT",
            "offline.escape.enemy.em3400.fsmv2.action.EsEm3400MFsmAction_ACT_END_MOVE",
            "offline.escape.enemy.em3400.fsmv2.action.EsEm3400MFsmAction_ACT_HIDE",
            "offline.escape.enemy.em3400.fsmv2.action.EsEm3400MFsmAction_ACT_MOVE",
            "offline.escape.enemy.em3400.fsmv2.action.EsEm3400MFsmAction_ACT_THREAT",
            "offline.escape.enemy.em3400.fsmv2.action.EsEm3400MFsmAction_ACT_TURN",
            "offline.escape.enemy.em3400.fsmv2.action.EsEm3400MFsmAction_ATK_SWALLOW",
            "offline.escape.enemy.em3400.fsmv2.action.EsEm3400MFsmAction_ATK_TAIL",
            "offline.escape.enemy.em3400.fsmv2.action.EsEm3400MFsmAction_DEAD_ASSIGN_MOTION",
            "offline.escape.enemy.em3400.fsmv2.action.EsEm3400MFsmAction_DMG_BURN",
            "offline.escape.enemy.em3400.fsmv2.action.EsEm3400MFsmAction_DMG_MOUTH_OPEN",
            "offline.escape.enemy.em3400.fsmv2.action.EsEm3400MFsmAction_GMK_SINGLE_DOOR_OPEN",
            "offline.escape.enemy.em3400.fsmv2.action.EsEm3400MFsmAction_SET_APPEAR",
            "offline.escape.enemy.em3400.fsmv2.action.EsEm3400MFsmAction_SET_APPEAR_WAIT",
            "offline.escape.enemy.em3500.fsmv2.action.EsEm3500MFsmAction_ACT_MOVE",
            "offline.escape.enemy.em3500.fsmv2.action.EsEm3500MFsmAction_ApplyDie",
            "offline.escape.enemy.em3500.fsmv2.action.EsEm3500MFsmAction_ApplySuspend",
            "offline.escape.enemy.em3500.fsmv2.action.EsEm3500MFsmAction_CallEffect",
            "offline.escape.enemy.em3500.fsmv2.action.EsEm3500MFsmAction_EndRoleHatch",
            "offline.escape.enemy.em3500.fsmv2.action.EsEm3500MFsmAction_GimmickNestHoleIn",
            "offline.escape.enemy.em3500.fsmv2.action.EsEm3500MFsmAction_GimmickNestHoleOut",
            "offline.escape.enemy.em3500.fsmv2.action.EsEm3500MFsmAction_Resume",
            "offline.escape.fsm.EsMFsmAction_ProcessingSiteGate",
            "offline.escape.fsm.EsMFsmAction_SiegeWarfareClearGuestActorInEvent",
            "offline.escape.fsm.EsMFsmAction_SiegeWarfareUpdateStayArea",
            "offline.fsmv2.LookActionTarget",
            "offline.fsmv2.ResetRagdoll",
            "offline.fsmv2.ResetRagdollOnThisMotion",
            "offline.fsmv2.SyncJackStateAction",
            "offline.fsmv2.enemy.action.Em0000MFsmAction_Burst",
            "offline.fsmv2.enemy.stateaction.Em4000FsmAction_HoldDamageHit",
            "chainsaw.BehaviorTreeAction_MFSM_AppPlayMotion",
            "chainsaw.BehaviorTreeAction_MFSM_AppPlayMotionOptionalLayer",

            // .bhvt/AI statics
            "app.ropeway.behaviortree.enemy.action.Em4000BtActionSet",
            "app.ropeway.enemy.common.behaviortree.action.EmCommonBtAction_SetNaviRadius",
            "app.ropeway.enemy.common.behaviortree.action.EmCommonBtAction_SetTarget",
            "app.ropeway.enemy.common.behaviortree.action.EmCommonBtAction_SkipRestrictMoveStart",
            "app.ropeway.enemy.common.behaviortree.action.EndOfFlowTree",
            "app.ropeway.enemy.common.behaviortree.action.EndOfForceMove",
            "app.ropeway.enemy.common.behaviortree.action.EnemyActionBreak",
            "app.ropeway.enemy.common.behaviortree.action.EnemyNavigationStart",
            "app.ropeway.enemy.common.behaviortree.action.EnemyTruncateAction",
            "app.ropeway.enemy.common.behaviortree.action.FlowTreeStandby",
            "app.ropeway.enemy.common.behaviortree.action.GetLoiteringPosition",
            "app.ropeway.enemy.common.behaviortree.action.RequestDefaultAction",
            "app.ropeway.enemy.common.behaviortree.action.WaitEnemyThinkStart",
            "app.ropeway.enemy.common.behaviortree.action.WaitLoseEnd",
            "app.ropeway.enemy.common.behaviortree.action.WaitRoleModeEnd",
            "app.ropeway.enemy.em0000.behaviortree.action.Em0000ActionSet",
            "app.ropeway.enemy.em3000.behaviortree.action.Em3000BtActionSet",
            "app.ropeway.enemy.em3000.behaviortree.action.Em3000BtAction_WaitLoseEnd",
            "app.ropeway.enemy.em5000.behaviortree.action.Em5000ActionSet",
            "app.ropeway.enemy.em6000.behaviortree.action.Em6000BtActionSet",
            "app.ropeway.enemy.em6000.behaviortree.action.Em6000BtReservedActionSet",
            "app.ropeway.enemy.em6200.behaviortree.action.Em6200ActionSet",
            "app.ropeway.enemy.em6200.behaviortree.action.Em6200BtReservedActionSet",
            "app.ropeway.enemy.em6300.behaviortree.action.Em6300ActionSet",
            "app.ropeway.enemy.em7000.behaviortree.action.Em7000BtActionSet",
            "app.ropeway.enemy.em7100.behaviortree.action.Em7100ActionSet",
            "app.ropeway.enemy.em7200.behaviortree.action.Em7200ActionSet",
            "app.ropeway.enemy.em7300.behaviortree.action.Em7300ActionSet",
            "app.ropeway.enemy.em9000.behaviortree.action.Em9000EndOfForceMove",
            "offline.behaviortree.enemy.action.Em4000BtActionSet",
            "offline.behaviortree.enemy.action.Em7000BtActionSet",
            "offline.enemy.common.behaviortree.action.EmCommonBtAction_SetNaviRadius",
            "offline.enemy.common.behaviortree.action.EmCommonBtAction_SetTarget",
            "offline.enemy.common.behaviortree.action.EndOfFlowTree",
            "offline.enemy.common.behaviortree.action.EndOfForceMove",
            "offline.enemy.common.behaviortree.action.EnemyActionBreak",
            "offline.enemy.common.behaviortree.action.EnemyNavigationStart",
            "offline.enemy.common.behaviortree.action.EnemyTruncateAction",
            "offline.enemy.common.behaviortree.action.FlowTreeStandby",
            "offline.enemy.common.behaviortree.action.GetLoiteringPosition",
            "offline.enemy.common.behaviortree.action.RequestDefaultAction",
            "offline.enemy.common.behaviortree.action.WaitEnemyThinkStart",
            "offline.enemy.common.behaviortree.action.WaitLoseEnd",
            "offline.enemy.common.behaviortree.action.WaitRoleModeEnd",
            "offline.enemy.em0000.behaviortree.action.Em0000ActionSet",
            "offline.enemy.em3000.behaviortree.action.Em3000BtActionSet",
            "offline.enemy.em3000.behaviortree.action.Em3000BtAction_WaitLoseEnd",
            "offline.escape.enemy.em3300.behaviortree.action.EsEm3300ActionSet",
            "offline.escape.enemy.em3300.behaviortree.action.EsEm3300BtAction_ExecSetFindState",
            "offline.escape.enemy.em3300.behaviortree.action.EsEm3300BtAction_SetMoveState",
            "offline.escape.enemy.em3300.behaviortree.action.EsEm3300BtAction_SetThreatType",
            "offline.escape.enemy.em3300.behaviortree.action.EsEm3300BtAfterAttackActionSet",
            "offline.escape.enemy.em3300.behaviortree.action.EsEm3300BtEnableAttackActionSetting",
            "offline.escape.enemy.em3300.behaviortree.action.EsEm3300BtReserveActionSet",
            "offline.escape.enemy.em3300.behaviortree.action.EsEm3300BtReserveChargeActionSetting",
            "offline.escape.enemy.em3400.behaviortree.action.EsEm3400ActionSet",
            "offline.escape.enemy.em3400.behaviortree.action.EsEm3400BtReserveActionSet",
            "offline.escape.enemy.em3400.behaviortree.action.EsEm3400BtReserveActionSetting",
            "offline.escape.enemy.em3400.behaviortree.action.EsEm3400SetHomesteadPosition",
            "offline.escape.enemy.em3500.behaviortree.action.EsEm3500BtAction_AttackThink",
            "offline.escape.enemy.em3500.behaviortree.action.EsEm3500BtAction_EscapeThink",
            "offline.escape.enemy.em3500.behaviortree.action.EsEm3500BtAction_NoneThinkIdle",
            "offline.escape.enemy.em3500.behaviortree.action.EsEm3500BtAction_NoneThinkMoveAround",
            "offline.escape.enemy.em3500.behaviortree.action.EsEm3500BtAction_ResetAttention",
            "offline.escape.enemy.em3500.behaviortree.action.EsEm3500BtAction_SelectReserveAction_ForAfterAttackEnd",
            "offline.escape.enemy.em3500.behaviortree.action.EsEm3500BtAction_SelectReserveAction_ForAfterDamageEnd",
            "offline.escape.enemy.em3500.behaviortree.action.EsEm3500BtAction_SelectReserveAction_ForAfterFind",
            "offline.escape.enemy.em3500.behaviortree.action.EsEm3500BtAction_SetVariablesCurrentThinkType",
            "offline.escape.enemy.em3500.behaviortree.action.EsEm3500BtAction_SetVariablesReserveActionID",

            // conditions
            "via.behaviortree.ConditionUserVariable",
            "app.ropeway.enemy.common.behaviortree.condition.CaseRange",
            "app.ropeway.enemy.common.behaviortree.condition.CheckGroundState",
            "app.ropeway.enemy.common.behaviortree.condition.EmCommonBtCondition_CaseLoiteringTbl",
            "app.ropeway.enemy.common.behaviortree.condition.EmCommonBtCondition_ForceMoveType",
            "app.ropeway.enemy.common.behaviortree.condition.EmCommonBtCondition_MoveEnd",
            "app.ropeway.enemy.common.behaviortree.condition.RegulateThinkCondition",
            "app.ropeway.enemy.em0000.fsmv2.condition.Em0000MFsmCondition_CanStandup",
            "app.ropeway.enemy.em0000.fsmv2.condition.Em0000MFsmCondition_HasDisorder",
            "offline.enemy.common.behaviortree.condition.CaseRange",
            "offline.enemy.common.behaviortree.condition.CheckGroundState",
            "offline.enemy.common.behaviortree.condition.EmCommonBtCondition_CaseLoiteringTbl",
            "offline.enemy.common.behaviortree.condition.EmCommonBtCondition_ForceMoveType",
            "offline.enemy.common.behaviortree.condition.EmCommonBtCondition_MoveEnd",
            "offline.enemy.common.behaviortree.condition.EnemyFixedActionEnd",
            "offline.enemy.common.behaviortree.condition.RegulateThinkCondition",
            "offline.enemy.em0000.fsmv2.condition.Em0000MFsmCondition_CanStandup",
            "offline.enemy.em0000.fsmv2.condition.Em0000MFsmCondition_HasDisorder",
            "offline.enemy.em7000.behaviortree.condition.Em7000BtCondition_CheckEndMove",
            "offline.enemy.em7000.behaviortree.condition.Em7000EscapeActionCategory",
            "offline.escape.enemy.em3300.behaviortree.condition.EsEm3300BtCondition_CheckChargeAction",
            "offline.escape.enemy.em3300.behaviortree.condition.EsEm3300BtCondition_CheckFlag",
            "offline.escape.enemy.em3300.behaviortree.condition.EsEm3300BtCondition_IsActThreat",
            "offline.escape.enemy.em3300.behaviortree.condition.EsEm3300BtCondition_IsAttackContinue",
            "offline.escape.enemy.em3300.behaviortree.condition.EsEm3300BtCondition_IsAttackExecute",
            "offline.escape.enemy.em3300.behaviortree.condition.EsEm3300BtCondition_IsReturnMove",
            "offline.escape.enemy.em3400.behaviortree.condition.EsEm3400BtCondition_ForceMoveEnd",
            "offline.escape.enemy.em3400.behaviortree.condition.EsEm3400BtCondition_IsAbleActionThreat",
            "offline.escape.enemy.em3400.behaviortree.condition.EsEm3400BtCondition_IsAbleAttackSwallow",
            "offline.escape.enemy.em3400.behaviortree.condition.EsEm3400BtCondition_IsAbleAttackSwallowShot",
            "offline.escape.enemy.em3400.behaviortree.condition.EsEm3400BtCondition_IsAbleAttackTail",
            "offline.escape.enemy.em3400.behaviortree.condition.EsEm3400BtCondition_IsAbleAttentionEndMove",
            "offline.escape.enemy.em3400.behaviortree.condition.EsEm3400BtCondition_IsAbleAttentionStartMove",
            "offline.escape.enemy.em3400.behaviortree.condition.EsEm3400BtCondition_IsAbleEndMove",
            "offline.escape.enemy.em3400.behaviortree.condition.EsEm3400BtCondition_IsAbleHide",
            "offline.escape.enemy.em3400.behaviortree.condition.EsEm3400BtCondition_IsAbleStartMove",
            "offline.escape.enemy.em3400.behaviortree.condition.EsEm3400BtCondition_IsAttackSwallowEnd",
            "offline.escape.enemy.em3400.behaviortree.condition.EsEm3400BtCondition_IsFarTarget",
            "offline.escape.enemy.em3400.behaviortree.condition.EsEm3400BtCondition_IsNeedTurn",
            "offline.escape.enemy.em3400.behaviortree.condition.EsEm3400BtCondition_IsStateOpenMouth",
            "offline.escape.enemy.em3500.behaviortree.condition.EsEm3500BtCondition_IsMoveTarget",

        ];
    }
}
