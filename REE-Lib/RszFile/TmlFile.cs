using ReeLib.Clip;
using ReeLib.Common;
using ReeLib.MotTree;
using ReeLib.Tml;

namespace ReeLib.Tml
{
    /// <summary>
    /// Full clip data header.
    /// </summary>
    public class ClipHeader : ClipBaseHeader, IKeyValueContainer
    {
        public int rootTrackCount;
        public int clipGroupCount;
        public int nodeReorderCount;
        public int nodeCountPragmata;
        public int trackGroupCount;

        internal long rootTracksOffset;
        internal long groupsOffset;
        internal long sectionsOffset;
        internal long nodesReorderOffset;
        internal long nodesReorderOffset2;

        internal long owordOffset;
        internal long dataOffset;

        long IKeyValueContainer.DataOffset16B => owordOffset;

        protected override sealed bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref magic);
            action.Do(ref version);
            action.Do(ref numFrames);
            action.Do(ref rootTrackCount);
            action.Do(ref trackGroupCount);
            action.Do(version >= ClipVersion.RE8, ref clipGroupCount);
            action.Do(ref nodeReorderCount);
            action.Do(version >= ClipVersion.MHWilds, ref nodeCountPragmata);
            DataInterpretationException.DebugThrowIf(nodeCountPragmata != 0 && nodeCountPragmata != nodeReorderCount);
            DataInterpretationException.DebugThrowIf(nodeReorderCount != rootTrackCount);
            action.Do(ref trackCount);
            action.Do(ref propertyCount);
            action.Do(ref keysCount);
            if (version < ClipVersion.RE8) action.Null(4);
            action.Do(version <= ClipVersion.RE2_DMC5, ref guid);
            if (version >= ClipVersion.MHWilds)
            {
                action.Do(ref boolKeysCount);
                action.Do(ref actionKeysCount);
                action.Do(ref noHermiteKeysCount);
                DataInterpretationException.DebugThrowIf(uknCount3 > 0);
            }

            action.Do(ref rootTracksOffset);
            action.Do(ref groupsOffset);
            action.Do(version >= ClipVersion.RE8, ref sectionsOffset);
            action.Do(version >= ClipVersion.MHWilds, ref nodesReorderOffset2);
            action.Do(ref nodesReorderOffset);
            action.Do(ref tracksOffset);
            action.Do(ref propertiesOffset);
            action.Do(ref keysOffset);
            if (version >= ClipVersion.MHWilds)
            {
                action.Do(ref boolKeysOffset);
                action.Do(ref actionKeysOffset);
                action.Do(ref noHermiteKeysOffset);
            }
            action.Do(ref speedPointOffset);
            action.Do(ref hermiteDataOffset);
            action.Do(version != ClipVersion.RE4, ref bezier3DDataOffset);
            action.Do(version <= ClipVersion.RE2_DMC5, ref uknOffset);
            action.Do(ref clipInfoOffset);
            DataInterpretationException.DebugThrowIf(uknOffset != 0 && uknOffset != clipInfoOffset, "It's actually used??");

            action.Do(ref namesOffset);
            action.Do(ref unicodeNamesOffset);
            action.Do(ref owordOffset);
            action.Do(ref dataOffset);

            return true;
        }
    }

    public class TimelineTrackGroup(ClipVersion version)
    {
        public bool enabled;
        public int ukn;
        public string type = "";
        public string name = "";

        internal int clipGroupCount;
        internal int nodeCount;
        internal int nodeStartOffset;
        internal long clipGroupStartIndex;

        // public List<TimelineTrack> Tracks { get; } = new();
        public List<TimelineClipGroup> ClipGroups { get; } = new();
        public ClipVersion Version { get; set; } = version;

        internal void Read(FileHandler handler, long asciiOffset, long unicodeOffset)
        {
            handler.Read(ref enabled);
            handler.ReadNull(3);
            handler.Read(ref ukn);
            handler.Read(ref clipGroupCount);
            handler.Read(ref nodeCount);
            // note: type is stored in both ascii and utf16 string tables, should be safe to ignore one of them
            type = handler.ReadAsciiString(asciiOffset + handler.Read<long>());
            type = handler.ReadWString(unicodeOffset + handler.Read<long>() * 2);
            name = handler.ReadWString(unicodeOffset + handler.Read<long>() * 2);
            if (Version >= ClipVersion.RE_RT)
            {
                handler.Read(ref clipGroupStartIndex);
            }
            handler.Read(ref nodeStartOffset);
            handler.ReadNull(4);
        }

        internal void Write(FileHandler handler)
        {
            handler.Write(ref enabled);
            handler.WriteNull(3);
            handler.Write(ref ukn);
            handler.Write(ref clipGroupCount);
            handler.Write(ref nodeCount);
            handler.Write((long)handler.AsciiStringTableAdd(type, false).TableOffset);
            handler.Write((long)handler.StringTableAdd(type, false).TableOffset);
            handler.Write((long)handler.StringTableAdd(name, false).TableOffset);
            if (Version >= ClipVersion.RE_RT)
            {
                handler.Write(ref clipGroupStartIndex);
            }
            handler.Write(ref nodeStartOffset);
            handler.WriteNull(4);
        }

        public override string ToString() => type + ": " + name;
    }

    public class TimelineTrack : ClipTrack
    {
        public byte uknByte;
        public byte uknByte2;
        public byte uknByte3;
        public string Tag = "";

        public MotionTreeNodeType nodeType;

        internal long tagOffset;

        public List<TimelineTrack> TimelineChildTracks { get; } = new();
        public new List<ClipTrack> ChildTracks => throw new NotSupportedException($"Use {nameof(TimelineChildTracks)} for full clip tracks!");

        public TimelineTrack(ClipVersion version) : base(version)
        {
        }

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref childCount);
            action.Do(ref propCount);
            action.Do(ref startFrame);
            action.Do(ref endFrame);
            action.Do(ref guid1);
            if (Version <= ClipVersion.RE_RT)
            {
                action.Do(ref guid2);
            }

            action.Do(ref nodeType);
            action.Do(ref uknByte);
            action.Do(ref uknByte2);
            action.Do(ref uknByte3);

            action.Do(ref pragmataHash);
            action.Do(ref nameHash1);
            action.Do(ref nameUtf16Hash);

            if (Version == ClipVersion.RE7 || Version == ClipVersion.RE3)
            {
                action.Do(ref nameOffset);
            }
            else
            {
                action.Do(ref nameOffset);
                action.Do(ref tagOffset);
            }
            action.Do(ref childIndex);
            action.Do(ref propertyIndex);

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (Version > ClipVersion.RE2_DMC5) nameUtf16Hash = MurMur3HashUtils.GetHash(Name);
            return base.DoWrite(handler);
        }

        public new void ReadName(FileHandler handler, long unicodeOffset, long asciiOffset)
        {
            if (Version == ClipVersion.RE7 || Version == ClipVersion.RE3)
            {
                Name = handler.ReadWString(unicodeOffset + nameOffset * 2);
            }
            else if (Version == ClipVersion.RE2_DMC5)
            {
                Name = handler.ReadUTF8String(asciiOffset + nameOffset);
                var name2 = handler.ReadWString(unicodeOffset + tagOffset * 2);
            }
            else
            {
                Name = handler.ReadWString(unicodeOffset + nameOffset * 2);
                Tag = handler.ReadWString(unicodeOffset + tagOffset * 2);
            }
        }

        public new void WriteName(FileHandler handler)
        {
            if (Version == ClipVersion.RE7 || Version == ClipVersion.RE3)
            {
                nameOffset = handler.StringTableAdd(Name, false).TableOffset;
            }
            else if (Version == ClipVersion.RE2_DMC5)
            {
                // note: this is not _fully_ correct, as it's actually utf8, but there's only one dmc5 file that uses utf8 characters so it's whatever
                nameOffset = handler.AsciiStringTableAdd(Name, false).TableOffset;
                tagOffset = handler.StringTableAdd(Name, false).TableOffset;
            }
            else
            {
                nameOffset = handler.StringTableAdd(Name, false).TableOffset;
                tagOffset = handler.StringTableAdd(Tag, false).TableOffset;
            }
        }

        public override string ToString() => !string.IsNullOrEmpty(Name) ? Name : guid1.ToString();
    }

    public class TimelineClipGroup(ClipVersion version) : BaseModel
    {
        public float startFrame;
        public float endFrame;
        public float frameCount;
        private long nameOffset;

        internal long trackCount;
        internal long trackStartIndex;

        public string Name = "";

        public List<TimelineTrack> Tracks { get; } = new();

        public ClipVersion Version { get; set; } = version;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref startFrame);
            handler.Read(ref endFrame);
            handler.ReadNull(4);
            handler.Read(ref frameCount);
            if (Version >= ClipVersion.MHWilds)
            {
                handler.Read(ref trackCount);
            }
            handler.Read(ref nameOffset);
            if (Version >= ClipVersion.MHWilds)
            {
                handler.Read(ref trackStartIndex);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref startFrame);
            handler.Write(ref endFrame);
            handler.WriteNull(4);
            handler.Write(ref frameCount);
            if (Version >= ClipVersion.MHWilds)
            {
                handler.Write(ref trackCount);
            }
            handler.Write(nameOffset = handler.StringTableAdd(Name, false).TableOffset);
            if (Version >= ClipVersion.MHWilds)
            {
                handler.Write(ref trackStartIndex);
            }
            return true;
        }

        public void ReadName(FileHandler handler, long unicodeStringOffset)
        {
            Name = handler.ReadWString(unicodeStringOffset + nameOffset * 2);
        }

        public override string ToString() => Name;
    }
}


namespace ReeLib
{
    public class ClipFile : BaseFile
    {
        public ClipHeader Header { get; } = new();
        public List<TimelineTrackGroup> TrackGroups { get; } = new();
        public List<TimelineTrack> RootTracks { get; } = new();
        public List<TimelineTrack> AllTracks { get; } = new();

        public EmbeddedClip Clip { get; } = new EmbeddedClip();

        public const int Magic = 0x50494C43;

        public ClipFile(FileHandler fileHandler) : base(fileHandler)
        {
            Clip.Header = Header;
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);

            if (Header.magic != ClipFile.Magic)
            {
                throw new InvalidDataException("Not a valid TML file");
            }

            Clip.Clear();

            TrackGroups.Clear();
            AllTracks.Clear();
            RootTracks.Clear();

            handler.Seek(Header.rootTracksOffset);
            var rootOffsets = handler.ReadArray<long>(Header.rootTrackCount);

            if (Header.trackGroupCount > 0)
            {
                handler.Seek(Header.groupsOffset);
                for (int i = 0; i < Header.trackGroupCount; ++i)
                {
                    var track = new TimelineTrackGroup(Header.version);
                    track.Read(handler, Header.namesOffset, Header.unicodeNamesOffset);
                    TrackGroups.Add(track);
                }
            }

            var clipGroups = new List<TimelineClipGroup>();
            if (Header.clipGroupCount > 0)
            {
                handler.Seek(Header.sectionsOffset);
                for (int i = 0; i < Header.clipGroupCount; ++i)
                {
                    var item = new TimelineClipGroup(Header.version);
                    item.Read(handler);
                    item.ReadName(handler, Header.unicodeNamesOffset);
                    clipGroups.Add(item);
                }
            }

            if (Header.nodeReorderCount > 0)
            {
                handler.Seek(Header.nodesReorderOffset);
                var childOffsets = handler.ReadArray<long>(Header.nodeReorderCount);
                DataInterpretationException.ThrowIfArraysNotEqual<long>(childOffsets!.Order().ToArray(), rootOffsets!.Order().ToArray());

                if (Header.nodesReorderOffset2 > 0) {
                    handler.Seek(Header.nodesReorderOffset2);
                    var offsets2 = handler.ReadArray<long>(Header.nodeReorderCount);
                    DataInterpretationException.ThrowIfArraysNotEqual<long>(childOffsets!.Order().ToArray(), offsets2!.Order().ToArray());
                }
            }

            handler.Seek(Header.tracksOffset);
            for (int i = 0; i < Header.trackCount; ++i)
            {
                var node = new TimelineTrack(Header.version);
                node.Read(handler);
                node.ReadName(handler, Header.unicodeNamesOffset, Header.namesOffset);
                AllTracks.Add(node);
            }

            foreach (var off in rootOffsets)
            {
                // one dmc5 file has duplicate nodes where this fails - ignore, surely nobody's gonna edit this one
                var rootNode = AllTracks.First(n => n.Start == off);
                RootTracks.Add(rootNode);
            }

            Clip.ReadProperties(handler);

            // setup node children
            foreach (var node in AllTracks)
            {
                if (node.childCount != 0)
                    node.TimelineChildTracks.AddRange(AllTracks.Slice((int)node.childIndex, node.childCount));

                if (node.propCount != 0)
                    node.Properties.AddRange(Clip.Properties.Slice((int)node.propertyIndex, node.propCount));
            }

            foreach (var group in TrackGroups)
            {
                if (Header.version >= ClipVersion.MHWilds) {
                    group.ClipGroups.AddRange(clipGroups.Slice((int)group.clipGroupStartIndex, group.clipGroupCount));
                    foreach (var cgroup in group.ClipGroups) {
                        cgroup.Tracks.AddRange(RootTracks.Slice((int)cgroup.trackStartIndex, (int)cgroup.trackCount));
                    }
                    continue;
                }

                var tracks = RootTracks.Slice(group.nodeStartOffset, group.nodeCount);
                if (group.clipGroupCount == -1) {
                    DataInterpretationException.DebugWarnIf(Header.version >= ClipVersion.RE8);
                    var cgroup = new TimelineClipGroup(Header.version);
                    group.ClipGroups.Add(cgroup);
                    cgroup.Tracks.AddRange(tracks);
                    continue;
                }

                var cgroups = clipGroups.Slice((int)group.clipGroupStartIndex, group.clipGroupCount);
                if (cgroups.Count == 1) {
                    // some games have weird shit going on (re4 natives/stm/_chainsaw/ui/ui3000/mot/ac_timeline.tml.54002)
                    cgroups[0].Tracks.AddRange(tracks);
                } else {
                    foreach (var track in tracks) {
                        // note: in some cases there's an overlap in clip groups (tags) frame ranges, hence First() rather than Single()
                        // example re7rt: sm1324_fluorescentlight04a_lighted_rightareab1fhallway1_dynamic_03.tml.43001
                        var cgroup = cgroups.First(cg => (track.startFrame == -1 || track.startFrame >= cg.startFrame) && track.endFrame <= cg.endFrame);
                        cgroup.Tracks.Add(track);
                    }
                }
                group.ClipGroups.AddRange(cgroups);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            Header.trackGroupCount = TrackGroups.Count;
            Header.clipGroupCount = TrackGroups.Count == 0 || Header.version < ClipVersion.RE8 ? 0 : TrackGroups.Sum(g => g.ClipGroups.Count);
            Header.rootTrackCount = Header.nodeReorderCount = RootTracks.Count;
            Header.trackCount = AllTracks.Count;
            Header.propertyCount = Clip.Properties.Count;
            Header.keysCount = Clip.NormalKeys.Count;

            var handler = FileHandler;
            Header.Write(handler);

            static void FlattenTracks(List<TimelineTrack> tracks, List<TimelineTrack> allTracks)
            {
                allTracks.AddRange(tracks);
                foreach (var track in tracks)
                {
                    track.childCount = track.TimelineChildTracks.Count;
                    track.childIndex = track.childCount > 0 ? allTracks.Count : 0;
                    FlattenTracks(track.TimelineChildTracks, allTracks);
                }
            }
            AllTracks.Clear();
            RootTracks.Clear();
            var clipGroups = new List<TimelineClipGroup>();
            foreach (var group in TrackGroups) {
                group.nodeCount = group.ClipGroups.Sum(cg => cg.Tracks.Count);
                group.nodeStartOffset = RootTracks.Count;
                group.clipGroupCount = Header.version < ClipVersion.RE8 ? -1 : group.ClipGroups.Count;
                group.clipGroupStartIndex = clipGroups.Count;
                clipGroups.AddRange(group.ClipGroups);
                foreach (var cgroup in group.ClipGroups)
                {
                    cgroup.trackCount = cgroup.Tracks.Count;
                    cgroup.trackStartIndex = RootTracks.Count;
                    foreach (var track in cgroup.Tracks)
                    {
                        RootTracks.Add(track);
                        AllTracks.Add(track);
                        track.childCount = track.TimelineChildTracks.Count;
                        track.childIndex = track.childCount > 0 ? AllTracks.Count : 0;
                        FlattenTracks(track.TimelineChildTracks, AllTracks);
                    }
                }
            }

            Clip.ReFlattenProperties(AllTracks);

            Header.rootTracksOffset = handler.Tell();
            handler.Skip(RootTracks.Count * sizeof(long));

            Header.groupsOffset = handler.Tell();
            foreach (var group in TrackGroups)
            {
                group.Write(handler);
            }

            if (clipGroups.Count > 0 && Header.version >= ClipVersion.RE8)
            {
                handler.Tell();
                Header.sectionsOffset = handler.Tell();
                clipGroups.Write(handler);
            }

            Header.nodesReorderOffset2 = handler.Tell();
            if (Header.version >= ClipVersion.MHWilds)
            {
                // the new version has a second block that seems to be identical to the normal nodesReorderOffset
                // doing it this way should work for all versions, we'll just write the offsets to the same spot twice on older ones
                handler.Skip(RootTracks.Count * sizeof(long));
            }

            Header.nodesReorderOffset = handler.Tell();
            handler.Skip(RootTracks.Count * sizeof(long));

            Header.tracksOffset = handler.Tell();
            foreach (var node in AllTracks)
            {
                node.WriteName(handler);
                node.Write(handler);

                var rootIndex = RootTracks.IndexOf(node);
                if (rootIndex != -1)
                {
                    handler.Write(Header.rootTracksOffset + rootIndex * 8, node.Start);
                    handler.Write(Header.nodesReorderOffset + rootIndex * 8, node.Start);
                    handler.Write(Header.nodesReorderOffset2 + rootIndex * 8, node.Start);
                }
            }

            Clip.WriteProperties(handler);

            Header.namesOffset = handler.Tell();
            handler.AsciiStringTableFlush();

            handler.Align(16);
            Header.unicodeNamesOffset = handler.Tell();
            handler.StringTableFlush();

            handler.Align(16);
            Header.owordOffset = handler.Tell();
            handler.OffsetContentTableFlush();

            Header.dataOffset = handler.Tell(); // TODO wtf

            Header.Write(handler, Header.Start);
            return true;
        }
    }
}