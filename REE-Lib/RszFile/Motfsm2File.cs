using ReeLib.Common;
using ReeLib.Motfsm2;

namespace ReeLib.Motfsm2
{
    public struct HeaderStruct
    {
        public uint version;
        public uint magic;
        public ulong skip;
        public long treeDataOffset;
        public long transitionMapOffset;
        public long transitionDataOffset;
        public long treeInfoPtr;
        public int transitionMapCount;
        public int transitionDataCount;
        public int startTransitionDataIndex;
    }


    public struct TransitionMap
    {
        public uint transitionId;
        public int dataIndex;

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
            handler.Skip(4);
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
            handler.Skip(4);
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
        public const string Extension2 = ".motfsm2";

        public StructModel<HeaderStruct> Header { get; } = new();
        private int treeDataSize;
        public BhvtFile? BhvtFile { get; private set; }
        public List<TransitionMap> TransitionMaps { get; } = new();
        public List<TransitionData> TransitionDatas { get; } = new();

        protected override bool DoRead()
        {
            var handler = FileHandler;
            if (!Header.Read(handler)) return false;
            ref var header = ref Header.Data;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a motfsm2 file");
            }

            handler.Seek(header.transitionMapOffset);
            TransitionMaps.ReadStructList(handler, header.transitionMapCount);

            handler.Seek(header.transitionDataOffset);
            TransitionDatas.Clear();
            for (int i = 0; i < header.transitionDataCount; i++)
            {
                TransitionData transitionData = new(Option.Version);
                transitionData.Read(handler);
                TransitionDatas.Add(transitionData);
            }

            handler.Read(header.treeInfoPtr, ref treeDataSize);
            BhvtFile ??= new(Option, handler.WithOffset(header.treeDataOffset));
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
            }

            return true;
        }

        protected override bool DoWrite()
        {
            throw new NotImplementedException();
        }
    }
}
