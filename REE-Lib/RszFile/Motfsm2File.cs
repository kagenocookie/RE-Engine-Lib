using ReeLib.Common;
using ReeLib.InternalAttributes;
using ReeLib.Motfsm2;

namespace ReeLib.Motfsm2
{
    [RszGenerate, RszAutoReadWrite]
    public partial class HeaderStruct : BaseModel
    {
        public uint version;
        [RszPaddingAfter(8)]
        public uint magic = Motfsm2File.Magic;
        public long treeDataOffset;
        public long transitionMapOffset;
        public long transitionDataOffset;
        public long treeInfoPtr;
        public int transitionMapCount;
        public int transitionDataCount;
        public int startTransitionDataIndex;
    }


    public struct TransitionMap : IComparable<TransitionMap>
    {
        public uint transitionId;
        public int dataIndex;

        public int CompareTo(TransitionMap other) => (transitionId, dataIndex).CompareTo((other.transitionId, other.dataIndex));

        public readonly override string ToString() => $"[{transitionId}] {dataIndex}";
    }


    public enum EndType {
        None = 0x0,
        EndOfMotion = 0x1,
        ExitFrame = 0x2,
        ExitFrameFromEnd = 0x3,
        SyncPoint = 0x4,
        SyncPointFromEnd = 0x5,
    }


    public enum InterpolationMode {
        None = 0x0,
        FrontFade = 0x1,
        CrossFade = 0x2,
        SyncCrossFade = 0x3,
        SyncPointCrossFade = 0x4,
        FrontOffsetFade = 0x5,
        InertiaFade = 0x6,
        FrontSpeedFade = 0x7,
    }


    public enum InterpolationCurve {
        Linear = 0x0,
        Smooth = 0x1,
        EaseIn = 0x2,
        EaseOut = 0x3,
    }


    public enum StartType {
        None = 0x0,
        Frame = 0x1,
        NormalizedTime = 0x2,
        SyncTime = 0x3,
        AutoSyncTime = 0x4,
        AutoSyncTimeSamePointCount = 0x5,
    }


    public class TransitionData : BaseModel
    {
        public uint id;
        public uint data;
        public float exitFrame;
        public float startFrame;
        public float interpolationFrame;
        // > dmc5
        public float contOnLayerSpeed;
        public float contOnLayerTimeout;
        public ushort contOnLayerNo;
        public ushort contOnLayerJointMaskId;

        public GameVersion Version { get; set; }

        public TransitionData(GameVersion version)
        {
            Version = version;
        }

        public EndType EndType
        {
            get => (EndType)Utils.BitsGet(data, 0, 4);
            set => data = Utils.BitsSet(data, 0, 4, (uint)value);
        }

        public InterpolationMode InterpolationMode
        {
            get => (InterpolationMode)Utils.BitsGet(data, 4, 4);
            set => data = Utils.BitsSet(data, 4, 4, (uint)value);
        }

        public InterpolationCurve InterpolationCurve
        {
            get => (InterpolationCurve)Utils.BitsGet(data, 8, 4);
            set => data = Utils.BitsSet(data, 8, 4, (uint)value);
        }

        public bool PrevMoveToEnd
        {
            get => Utils.BitsGet(data, 12, 1) != 0;
            set => data = Utils.BitsSet(data, 12, 1, value ? 1u : 0u);
        }

        public StartType StartType
        {
            get => (StartType)Utils.BitsGet(data, 13, 4);
            set => data = Utils.BitsSet(data, 13, 4, (uint)value);
        }

        public bool ElapsedTimeZero
        {
            get => Utils.BitsGet(data, 17, 1) != 0;
            set => data = Utils.BitsSet(data, 17, 1, value ? 1u : 0u);
        }

        public bool ContOnLayer
        {
            get => Utils.BitsGet(data, 18, 1) != 0;
            set => data = Utils.BitsSet(data, 18, 1, value ? 1u : 0u);
        }

        public uint ContOnLayerInterpCurve
        {
            get => Utils.BitsGet(data, 19, 4);
            set => data = Utils.BitsSet(data, 19, 4, value);
        }

        public uint EmptyBits
        {
            get => Utils.BitsGet(data, 23, 9);
            set => data = Utils.BitsSet(data, 23, 9, value);
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref id);
            handler.Read(ref data);
            handler.Read(ref exitFrame);
            handler.Read(ref startFrame);
            handler.Read(ref interpolationFrame);
            if (Version > GameVersion.dmc5)
            {
                handler.Read(ref contOnLayerSpeed);
                handler.Read(ref contOnLayerTimeout);
                handler.Read(ref contOnLayerNo);
                handler.Read(ref contOnLayerJointMaskId);
            }
            handler.ReadNull(4);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref id);
            handler.Write(ref data);
            handler.Write(ref exitFrame);
            handler.Write(ref startFrame);
            handler.Write(ref interpolationFrame);
            if (Version > GameVersion.dmc5)
            {
                handler.Write(ref contOnLayerSpeed);
                handler.Write(ref contOnLayerTimeout);
                handler.Write(ref contOnLayerNo);
                handler.Write(ref contOnLayerJointMaskId);
            }
            handler.WriteNull(4);
            return true;
        }

        public override string ToString() => $"{id} [{startFrame}-{exitFrame}]";
    }
}


namespace ReeLib
{
    public class Motfsm2File(RszFileOption option, FileHandler fileHandler) : BaseRszFile(option, fileHandler)
    {
        public const uint Magic = 0x3273666d;

        public HeaderStruct Header { get; } = new();
        public BhvtFile BhvtFile { get; private set; } = new(option, fileHandler);
        public List<TransitionMap> TransitionMaps { get; } = new();
        public List<TransitionData> TransitionDatas { get; } = new();

        public void ChangeTransitionMapping(uint sourceId, uint transitionDataId)
        {
            var dataIndex = TransitionDatas.FindIndex(d => d.id == transitionDataId);
            if (dataIndex == -1) {
                Log.Error("Attempted to change to unknown transition ID " + transitionDataId);
                return;
            }
            var newMap = new TransitionMap() { transitionId = sourceId, dataIndex = dataIndex };
            for (int i = 0; i < TransitionMaps.Count; i++) {
                var map = TransitionMaps[i];
                if (map.transitionId == sourceId) {
                    TransitionMaps[i] = newMap;
                    return;
                }
            }

            TransitionMaps.Add(newMap);
            TransitionMaps.Sort();
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            if (!Header.Read(handler)) return false;
            var header = Header;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a motfsm2 file");
            }

            handler.Seek(header.transitionMapOffset);
            TransitionMaps.Clear();
            TransitionMaps.ReadStructList(handler, header.transitionMapCount);

            handler.Seek(header.transitionDataOffset);
            TransitionDatas.Clear();
            for (int i = 0; i < header.transitionDataCount; i++)
            {
                TransitionData transitionData = new(Option.Version);
                transitionData.Read(handler);
                TransitionDatas.Add(transitionData);
            }

            var treeDataSize = handler.Read<int>(header.treeInfoPtr, false);
            BhvtFile.FileHandler = handler.WithOffset(header.treeDataOffset);
            BhvtFile.Read();

            var nodeDict = BhvtFile.Nodes.ToDictionary(n => (ulong)n.ID);
            var transitionMaps = TransitionMaps.ToDictionary(n => n.transitionId);
            foreach (var node in BhvtFile.Nodes)
            {
                foreach (var state in node.States.States)
                {
                    if (state.transitionMapID != 0)
                    {
                        state.TransitionData = TransitionDatas[transitionMaps[state.transitionMapID].dataIndex];
                    }
                }

                foreach (var state in node.AllStates.AllStates)
                {
                    if (state.transitionMapID != 0)
                    {
                        if (transitionMaps.TryGetValue(state.transitionMapID, out var transitionMap)) {
                            // there's no match sometimes (ID 4012098982 for 2 dmc5 files ¯\_(ツ)_/¯)
                            state.TransitionData = TransitionDatas[transitionMap.dataIndex];
                        }
                    }
                }
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var header = Header;

            header.transitionMapCount = TransitionMaps.Count;
            header.transitionDataCount = TransitionDatas.Count;

            header.Write(handler);

            handler.Align(16);
            header.treeDataOffset = handler.Tell();
            BhvtFile.FileHandler = handler.WithOffset(header.treeDataOffset);
            BhvtFile.Write();

            var treeSize = handler.Tell() - header.treeDataOffset;

            handler.Align(16);
            header.treeInfoPtr = handler.Tell();
            handler.Write(treeSize);

            handler.Align(16);
            header.transitionMapOffset = handler.Tell();
            TransitionMaps.Write(handler);

            handler.Align(16);
            header.transitionDataOffset = handler.Tell();
            TransitionDatas.Write(handler);

            header.Write(handler, 0);
            return true;
        }
    }
}
