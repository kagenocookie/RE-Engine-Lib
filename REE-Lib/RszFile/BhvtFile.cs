using System.Runtime.CompilerServices;
using ReeLib.Bhvt;
using ReeLib.Motfsm2;


namespace ReeLib.Bhvt
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

        public long referencePrefabGameObjectsOffset;

        public GameVersion Version { get; set; }

        public Header(GameVersion version)
        {
            Version = version;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref magic);
            handler.Skip(4);
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
        public NodeID ChildId;
        public BHVTId ConditionID;

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
        public NodeID targetNodeID;
        public BHVTId TransitionConditionID;
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
                state.TransitionConditionID = (BHVTId)data[1, i];
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
                data[1, i] = (uint)state.TransitionConditionID;
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
        public uint mAllState;
        public BHVTId mAllTransition;
        public uint mAllTransitionID;
        public uint mAllStateEx;
        public BHVTId mAllTransitionAttributes;
    }


    public class NAllStates : TwoDimensionContainer
    {
        internal override int FieldCount => 5;
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
        public List<uint> mStartTransitionEvents { get; } = new();
        public BHVTId mStartState;
        public BHVTId mStartStateTransition;
        public uint mStartStateEx;

        public List<RszInstance> Events { get; } = new();
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
            for (int i = 0; i < count; i++)
            {
                var state = new NTransition();
                if (Version >= GameVersion.mhrise)
                {
                    var eventCount = handler.Read<int>();
                    state.mStartTransitionEvents.ReadStructList(handler, eventCount);
                }
                else
                {
                    // the earlier versions only had one ID, migrate to list so we have a consistent structure
                    var id = handler.Read<uint>();
                    state.mStartTransitionEvents.Clear();
                    if (id != 0) state.mStartTransitionEvents.Add(id);
                }
                Transitions.Add(state);
            }
            if (count == 0) return true;

            uint[,] data;
            if (Version >= GameVersion.re3)
            {
                data = new uint[3, count];
                handler.ReadArray(data);
                for (int i = 0; i < count; i++)
                {
                    Transitions[i] = new NTransition();
                    Transitions[i].mStartState = (BHVTId)data[0, i];
                    Transitions[i].mStartStateTransition = (BHVTId)data[1, i];
                    Transitions[i].mStartStateEx = data[2, i];
                }
            }
            else
            {
                data = new uint[2, count];
                handler.ReadArray(data);
                for (int i = 0; i < count; i++)
                {
                    Transitions[i] = new NTransition();
                    Transitions[i].mStartState = (BHVTId)data[0, i];
                    Transitions[i].mStartStateTransition = (BHVTId)data[1, i];
                }
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(Transitions.Count);
            if (Transitions.Count == 0) return true;
            var hasStartEx = Version >= GameVersion.re3;
            uint[,] data = new uint[hasStartEx ? 3 : 2, Transitions.Count];
            for (int i = 0; i < Transitions.Count; i++)
            {
                var transition = Transitions[i];
                if (Version >= GameVersion.re3)
                {
                    handler.Write(transition.mStartTransitionEvents.Count == 0 ? 0 : transition.mStartTransitionEvents[0]);
                }
                else
                {
                    handler.Write(transition.mStartTransitionEvents.Count);
                    transition.mStartTransitionEvents.Write(handler);
                }
                data[0, i] = (uint)transition.mStartState;
                data[1, i] = (uint)transition.mStartStateTransition;
                if (hasStartEx) data[2, i] = transition.mStartStateEx;
            }
            handler.WriteArray(data);
            return true;
        }
    }

    public struct NodeID
    {
        public uint ID;
        public uint exID;

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
        public NodeAttribute Attributes;

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
        public int ReferenceTreeIndex;

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
                    handler.Skip(2);
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

        public int NodeCount;
        public List<BHVTNode> Nodes { get; } = new();
        public List<BHVTNode> RootNodes { get; } = new();
        public UVarFile? Variable { get; set; }
        public List<UVarFile> ReferenceTrees { get; } = new();

        private const int Condition_IDFieldIndex = 0;
        private const int Action_IDFieldIndex = 1;

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
            RootNodes.Clear();

            handler.Seek(header.nodeOffset);
            handler.Read(ref NodeCount);
            for (int i = 0; i < NodeCount; i++)
            {
                BHVTNode node = new(header.Version);
                if (!node.Read(handler)) return false;
                node.ReadName(handler, header.stringOffset);
                Nodes.Add(node);
            }

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

            // mNamePool
            handler.Seek(header.stringOffset);
            List<string> mNamePool = ReadStringPool();

            // mPathNamePool
            // handler.Seek(header.resourcePathsOffset);
            // int numPaths = handler.ReadInt();
            // List<string> mPathNamePool = ReadStringPool();
            // var inheritOffset = header.Version >= GameVersion.dd2;

            handler.Seek(header.variableOffset);
            Variable ??= new UVarFile(handler) { Embedded = true };
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

            SetupReferences();
            return true;
        }

        private void SetupReferences()
        {
            var nodeDict = Nodes.ToDictionary(n => (ulong)n.ID);
            var actions = new Dictionary<ulong, RszInstance>();
            foreach (var act in ActionRsz.ObjectList) {
                var id = Convert.ToUInt32(act.Values[Action_IDFieldIndex]);
                var exId = 0u;
                while (!actions.TryAdd((ulong)id | ((ulong)exId << 32), act)) {
                    exId++;
                }
            }
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

                if (node.ParentID.IsUnset) RootNodes.Add(node);

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

                    DataInterpretationException.DebugWarnIf(callerId.idType == 64 && !StaticClasses.Contains(instance.RszClass.name), instance.RszClass.name);
                }
            }

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
                        if (state.targetNodeID.ID != 0)
                        {
                            state.TargetNode = nodeDict[(ulong)state.targetNodeID];
                        }
                        DataInterpretationException.DebugThrowIf(state.TargetNode == null);
                        if (state.TransitionConditionID.HasValue)
                        {
                            state.Condition = state.TransitionConditionID.idType == 64
                                ? StaticConditionsRsz.ObjectList[state.TransitionConditionID.id]
                                : ConditionsRsz.ObjectList[state.TransitionConditionID.id];
                        }
                    }
                }
            }
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
