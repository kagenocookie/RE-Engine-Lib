using ReeLib.Clip;
using ReeLib.InternalAttributes;
using ReeLib.MotTree;
using ReeLib.Tml;

namespace ReeLib.Tml
{
    public class Header : ReadWriteModel, IKeyValueContainer
    {
        public uint magic = TmlFile.Magic;
        public ClipVersion version;
        public float totalFrames;
        public int rootNodeCount;
        public int trackCount;
        public int nodeGroupCount;
        public int nodeReorderCount;
        public int nodeCount;
        public int propertyCount;
        public int keyCount;
        public Guid guid;

        public long nodesOffset;
        public long trackTableOffset;
        public long nodeGroupsOffset;
        public long nodesReorderOffset;
        public long nodeTableOffset;
        public long propertyOffset;
        public long keyOffset;
        public long speedPointOffset;
        public long hermiteDataOffset;
        public long bezier3DDataOffset;
        public long uknOffset;
        public long clipInfoOffset;

        public long stringsOffset;
        public long unicodeStringsOffset;
        public long owordOffset;
        public long dataOffset;

        long IKeyValueContainer.AsciiStringOffset => stringsOffset;
        long IKeyValueContainer.UnicodeStringOffset => unicodeStringsOffset;
        long IKeyValueContainer.DataOffset16B => owordOffset;

        protected override sealed bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref magic);
            action.Do(ref version);
            action.Do(ref totalFrames);
            action.Do(ref rootNodeCount);
            action.Do(ref trackCount);
            action.Do(version >= ClipVersion.RE8, ref nodeGroupCount);
            action.Do(ref nodeReorderCount);
            action.Do(ref nodeCount);
            action.Do(ref propertyCount);
            action.Do(ref keyCount);
            if(version < ClipVersion.RE8) action.Null(4);
            action.Do(version <= ClipVersion.RE2_DMC5, ref guid);

            action.Do(ref nodesOffset);
            action.Do(ref trackTableOffset);
            action.Do(version >= ClipVersion.RE8, ref nodeGroupsOffset);
            action.Do(ref nodesReorderOffset);
            action.Do(ref nodeTableOffset);
            action.Do(ref propertyOffset);
            action.Do(ref keyOffset);
            action.Do(ref speedPointOffset);
            action.Do(ref hermiteDataOffset);
            action.Do(version != ClipVersion.RE4, ref bezier3DDataOffset);
            action.Do(version <= ClipVersion.RE2_DMC5, ref uknOffset);
            action.Do(ref clipInfoOffset);
            DataInterpretationException.DebugThrowIf(uknOffset != 0 && uknOffset != clipInfoOffset, "It's actually used??");
            DataInterpretationException.DebugThrowIf(version > ClipVersion.RE2_DMC5 && speedPointOffset != hermiteDataOffset, "Speedpoint does still exist");

            action.Do(ref stringsOffset);
            action.Do(ref unicodeStringsOffset);
            action.Do(ref owordOffset);
            action.Do(ref dataOffset);

            return true;
        }
    }

    public class TimelineTrack(ClipVersion version)
    {
        public bool enabled;
        public int ukn;
        public int clipCount;
        public int nodeCount;
        public string type = "";
        public string name = "";
        public int nodeStartOffset;
        public long uknOffset;

        public List<TimelineNode> Nodes { get; } = new();
        public ClipVersion Version { get; set; } = version;

        internal void Read(FileHandler handler, long asciiOffset, long unicodeOffset)
        {
            handler.Read(ref enabled);
            handler.ReadNull(3);
            handler.Read(ref ukn);
            handler.Read(ref clipCount);
            handler.Read(ref nodeCount);
            // note: type is stored in both ascii and utf16 string tables, should be safe to ignore one of them
            type = handler.ReadAsciiString(asciiOffset + handler.Read<long>());
            type = handler.ReadWString(unicodeOffset + handler.Read<long>() * 2);
            name = handler.ReadWString(unicodeOffset + handler.Read<long>() * 2);
            handler.Read(ref nodeStartOffset);
            handler.ReadNull(4);
            if (Version >= ClipVersion.RE_RT)
            {
                handler.Read(ref uknOffset);
            }
        }

        internal void Write(FileHandler handler)
        {
            handler.Write(ref enabled);
            handler.WriteNull(3);
            handler.Write(ref ukn);
            handler.Write(ref clipCount);
            handler.Write(ref nodeCount);
            handler.Write((long)handler.AsciiStringTableAdd(type, false).TableOffset);
            handler.Write((long)handler.StringTableAdd(type, false).TableOffset);
            handler.Write((long)handler.StringTableAdd(name, false).TableOffset);
            handler.Write(ref nodeStartOffset);
            handler.WriteNull(4);
            if (Version >= ClipVersion.RE_RT)
            {
                handler.Write(ref uknOffset);
            }
        }

        public override string ToString() => type + ": " + name;
    }

    public class TimelineNode : ReadWriteModel
    {
        public int childCount;
        public int propCount;
        public float startFrame;
        public float endFrame;
        public Guid guid;

        public Guid guid2;

        public MotionTreeNodeType nodeType;
        public byte uknByte;
        public byte uknByte2;

        public ulong nameHash;
        public long nameOffset;
        public long nodeTagOffset;
        public long childIndex;
        public long propertyIndex;

        public List<TimelineNode> ChildNodes { get; } = new();
        public List<Property> Properties { get; } = new();

        public string Name = "";
        public string Tag = "";

        public ClipVersion Version { get; set; }

        public TimelineNode(ClipVersion version)
        {
            Version = version;
        }


        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref childCount);
            action.Do(ref propCount);
            action.Do(ref startFrame);
            action.Do(ref endFrame);
            action.Do(ref guid);
            if (Version <= ClipVersion.RE_RT)
            {
                action.Do(ref guid2);
            }

            action.Do(ref nodeType);
            action.Do(ref uknByte);
            action.Do(ref uknByte2);
            action.Null(5);

            action.Do(ref nameHash);

            if (Version == ClipVersion.RE7 || Version == ClipVersion.RE3)
            {
                action.Do(ref nameOffset);
            }
            else
            {
                action.Do(ref nameOffset);
                action.Do(ref nodeTagOffset);
            }
            action.Do(ref childIndex);
            action.Do(ref propertyIndex);

            return true;
        }

        public void ReadName(FileHandler handler, long unicodeOffset, long asciiOffset)
        {
            if (Version == ClipVersion.RE7 || Version == ClipVersion.RE3)
            {
                Name = handler.ReadWString(unicodeOffset + nameOffset * 2);
            }
            else if (Version == ClipVersion.RE2_DMC5)
            {
                Name = handler.ReadUTF8String(asciiOffset + nameOffset);
                var name2 = handler.ReadWString(unicodeOffset + nodeTagOffset * 2);
            }
            else
            {
                Name = handler.ReadWString(unicodeOffset + nameOffset * 2);
                Tag = handler.ReadWString(unicodeOffset + nodeTagOffset * 2);
            }
        }

        public void WriteName(FileHandler handler)
        {
            if (Version == ClipVersion.RE7 || Version == ClipVersion.RE3)
            {
                nameOffset = handler.StringTableAdd(Name, false).TableOffset;
            }
            else if (Version == ClipVersion.RE2_DMC5)
            {
                // note: this is not _fully_ correct, as it's actually utf8, but there's only one dmc5 file that uses utf8 characters so it's whatever
                nameOffset = handler.AsciiStringTableAdd(Name, false).TableOffset;
                nodeTagOffset = handler.StringTableAdd(Name, false).TableOffset;
            }
            else
            {
                nameOffset = handler.StringTableAdd(Name, false).TableOffset;
                nodeTagOffset = handler.StringTableAdd(Tag, false).TableOffset;
            }
        }

        public override string ToString() => !string.IsNullOrEmpty(Name) ? Name : guid.ToString();
    }

    public class TmlNodeGroup : BaseModel
    {
        public int ukn;
        public float frameCount;
        public int ukn2;
        public float frameCount2;
        public long nameOffset;

        public string Name = "";

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref ukn);
            handler.Read(ref frameCount);
            handler.Read(ref ukn2);
            handler.Read(ref frameCount2);
            handler.Read(ref nameOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref ukn);
            handler.Write(ref frameCount);
            handler.Write(ref ukn2);
            handler.Write(ref frameCount2);
            handler.Write(nameOffset = handler.StringTableAdd(Name).TableOffset);
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
    public class TmlFile : BaseFile
    {
        public Header Header { get; } = new();
        public List<TimelineTrack> Tracks { get; } = new();
        public List<TmlNodeGroup> NodeGroups { get; } = new();
        public List<TimelineNode> RootNodes { get; } = new();
        public List<TimelineNode> Nodes { get; } = new();
        public List<Property> Properties { get; } = new();
        public List<Key> Keys { get; } = new();

        public List<SpeedPointData> SpeedPointData { get; } = new();
        public List<HermiteInterpolationData> HermiteData { get; } = new();
        public List<Bezier3DKeys> Bezier3DData { get; } = new();
        public List<ClipInfoStruct> ClipInfo { get; } = new();

        public const uint Magic = 0x50494C43;

        public TmlFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);

            if (Header.magic != Magic)
            {
                throw new InvalidDataException("Not a valid TML file");
            }

            Tracks.Clear();
            Keys.Clear();
            Nodes.Clear();
            RootNodes.Clear();
            Properties.Clear();
            HermiteData.Clear();
            SpeedPointData.Clear();
            Bezier3DData.Clear();
            ClipInfo.Clear();

            handler.Seek(Header.nodesOffset);
            var rootOffsets = handler.ReadArray<long>(Header.rootNodeCount);

            if (Header.trackCount > 0)
            {
                handler.Seek(Header.trackTableOffset);
                for (int i = 0; i < Header.trackCount; ++i)
                {
                    var track = new TimelineTrack(Header.version);
                    track.Read(handler, Header.stringsOffset, Header.unicodeStringsOffset);
                    Tracks.Add(track);
                }
            }

            if (Header.nodeGroupCount > 0)
            {
                handler.Seek(Header.nodeGroupsOffset);
                for (int i = 0; i < Header.nodeGroupCount; ++i)
                {
                    var item = new TmlNodeGroup();
                    item.Read(handler);
                    item.ReadName(handler, Header.unicodeStringsOffset);
                    NodeGroups.Add(item);
                }
            }

            if (Header.nodeReorderCount > 0)
            {
                handler.Seek(Header.nodesReorderOffset);
                var childOffsets = handler.ReadArray<long>(Header.nodeReorderCount);
                DataInterpretationException.ThrowIfArraysNotEqual<long>(childOffsets!.Order().ToArray(), rootOffsets!.Order().ToArray());
            }

            handler.Seek(Header.nodeTableOffset);
            for (int i = 0; i < Header.nodeCount; ++i)
            {
                var node = new TimelineNode(Header.version);
                node.Read(handler);
                node.ReadName(handler, Header.unicodeStringsOffset, Header.stringsOffset);
                Nodes.Add(node);
            }

            foreach (var off in rootOffsets)
            {
                // one dmc5 file has duplicate nodes where this fails - ignore, surely nobody's gonna edit this one
                var rootNode = Nodes.First(n => n.Start == off);
                RootNodes.Add(rootNode);
            }

            var speedPointCount = 0;
            var clipInfoCount = 0;
            handler.Seek(Header.propertyOffset);
            for (int i = 0; i < Header.propertyCount; i++)
            {
                Property property = new(Header.version);
                property.Info.Read(handler);
                property.Info.FunctionName = handler.ReadAsciiString(Header.stringsOffset + property.Info.nameOffset);
                DataInterpretationException.ThrowIf(string.IsNullOrEmpty(property.Info.FunctionName));
                Properties.Add(property);
                if (Header.version <= ClipVersion.RE2_DMC5)
                {
                    speedPointCount += (int)property.Info.nameAsciiHash;
                }
            }

            var hermiteKeys = 0;
            var bezier3dCount = 0;
            handler.Seek(Header.keyOffset);
            for (int i = 0; i < Header.keyCount; i++)
            {
                Key key = new(Header.version);
                key.Read(handler);
                Keys.Add(key);
                if (key.interpolation == InterpolationType.Hermite) hermiteKeys++;
                if (key.interpolation == InterpolationType.Bezier3D) bezier3dCount++;
            }

            if (speedPointCount > 0)
            {
                handler.Seek(Header.speedPointOffset);
                SpeedPointData.ReadStructList(handler, speedPointCount);
                DataInterpretationException.DebugThrowIf((Header.hermiteDataOffset - Header.speedPointOffset) / 24 != speedPointCount);
            }
            else if (Header.version == ClipVersion.RE7 && Header.speedPointOffset != Header.hermiteDataOffset)
            {
                // I have no idea why it's like this here, re7 being re7 I guess
                // the struct looks identical so it's _probably_ the same thing as in newer games
                handler.Seek(Header.speedPointOffset);
                SpeedPointData.ReadStructList(handler, Properties.Count);
                DataInterpretationException.DebugThrowIf((Header.hermiteDataOffset - Header.speedPointOffset) / 24 != speedPointCount);
            }
            else
            {
                DataInterpretationException.DebugThrowIf(Header.speedPointOffset != Header.hermiteDataOffset);
            }

            if (hermiteKeys > 0)
            {
                handler.Seek(Header.hermiteDataOffset);
                HermiteData.ReadStructList(handler, hermiteKeys);
            }

            if (bezier3dCount > 0)
            {
                handler.Seek(Header.bezier3DDataOffset);
                Bezier3DData.ReadStructList(handler, bezier3dCount);
            }

            // TODO figure out where the count actually comes from
            // not key interpolation and not linked to properties directly
            handler.Seek(Header.clipInfoOffset);
            clipInfoCount = (int)(Header.stringsOffset - Header.clipInfoOffset) / ClipInfoStruct.GetSize(Header.version);
            DataInterpretationException.DebugThrowIf((Header.stringsOffset - Header.clipInfoOffset) % ClipInfoStruct.GetSize(Header.version) != 0);
            for (int i = 0; i < clipInfoCount; ++i)
            {
                var clip = new ClipInfoStruct() { Version = Header.version };
                clip.Read(handler);
                ClipInfo.Add(clip);
            }

            // note: copied from ClipEntry due to otherwise different file contents
            // consider merging the two together somehow
            foreach (var property in Properties)
            {
                if (property.Info.ChildMembershipCount == 0) continue;
                if (property.IsPropertyContainer)
                {
                    property.ChildProperties ??= new();
                    for (long i = property.Info.ChildStartIndex; i < property.Info.ChildMembershipCount + property.Info.ChildStartIndex; i++)
                    {
                        property.ChildProperties.Add(Properties[(int)i]);
                    }
                }
                else
                {
                    property.Keys ??= new();
                    for (long i = property.Info.ChildStartIndex; i < property.Info.ChildMembershipCount + property.Info.ChildStartIndex; i++)
                    {
                        Key key = Keys[(int)i];
                        property.Keys.Add(key);
                        key.ReadValue(handler, property, Header);
                    }
                }
            }

            // setup node children
            foreach (var node in Nodes)
            {
                for (int i = (int)node.childIndex; i < node.childIndex + node.childCount; ++i)
                {
                    node.ChildNodes.Add(Nodes[i]);
                }

                for (int i = (int)node.propertyIndex; i < node.propertyIndex + node.propCount; ++i)
                {
                    node.Properties.Add(Properties[i]);
                }
            }

            foreach (var track in Tracks)
            {
                for (int i = track.nodeStartOffset; i < track.nodeStartOffset + track.nodeCount; ++i)
                {
                    track.Nodes.Add(RootNodes[i]);
                }
            }

            return true;
        }

        protected override bool DoWrite()
        {
            Header.trackCount = Tracks.Count;
            Header.nodeGroupCount = NodeGroups.Count;
            Header.rootNodeCount = Header.nodeReorderCount = RootNodes.Count;
            Header.nodeCount = Nodes.Count;
            Header.propertyCount = Properties.Count;
            Header.keyCount = Keys.Count;

            var handler = FileHandler;
            Header.Write(handler);

            Header.nodesOffset = handler.Tell();
            handler.Skip(sizeof(long) * RootNodes.Count);

            Header.trackTableOffset = handler.Tell();
            foreach (var track in Tracks)
            {
                track.Write(handler);
            }

            Header.nodeGroupsOffset = handler.Tell();
            if (NodeGroups.Count > 0)
            {
                NodeGroups.Write(handler);
            }

            Header.nodesReorderOffset = handler.Tell();
            handler.Skip(sizeof(long) * RootNodes.Count);

            // TODO re-flatten nodes
            Header.nodeTableOffset = handler.Tell();
            foreach (var node in Nodes)
            {
                node.WriteName(handler);
                node.Write(handler);

                var rootIndex = RootNodes.IndexOf(node);
                if (rootIndex != -1)
                {
                    handler.Write(Header.nodesOffset + rootIndex * 8, node.Start);
                    handler.Write(Header.nodesReorderOffset + rootIndex * 8, node.Start);
                }
            }

            Header.propertyOffset = handler.Tell();
            foreach (var property in Properties)
            {
                var stringItem = handler.AsciiStringTableAdd(property.Info.FunctionName, false);
                property.Info.nameOffset = stringItem.TableOffset;

                if (Header.version <= ClipVersion.RE3)
                {
                    stringItem = handler.StringTableAdd(property.Info.FunctionName, false);
                    property.Info.unicodeNameOffset = stringItem.TableOffset;
                }

                property.Info.Write(handler);
            }

            Header.keyOffset = handler.Tell();
            foreach (var key in Keys)
            {
                key.Write(handler);
            }

            Header.speedPointOffset = handler.Tell();
            if (SpeedPointData.Count > 0)
            {
                SpeedPointData.Write(handler);
            }

            Header.hermiteDataOffset = handler.Tell();
            HermiteData.Write(handler);

            Header.bezier3DDataOffset = handler.Tell();
            Bezier3DData.Write(handler);

            Header.uknOffset = handler.Tell(); // this one never has content

            Header.clipInfoOffset = handler.Tell();
            ClipInfo.Write(handler);

            Header.stringsOffset = handler.Tell();
            handler.AsciiStringTableFlush();

            handler.Align(16);
            Header.unicodeStringsOffset = handler.Tell();
            handler.StringTableFlush();

            handler.Align(16);
            Header.owordOffset = handler.Tell();
            handler.OffsetContentTableFlush();

            Header.dataOffset = handler.Tell();

            Header.Write(handler, 0);
            return true;
        }
    }
}