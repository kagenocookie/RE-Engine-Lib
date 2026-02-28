using System.Runtime.InteropServices;
using ReeLib.Clip;
using ReeLib.Common;
using ReeLib.Gui;

namespace ReeLib
{
    public enum GuiVersion
    {
        RE7 = 14,
        RE2_DMC5 = 20,
        RE8 = 23,
        RE_RT = 27,
        MHRise = 33,
        RE4 = 34,
        DD2 = 35,
        MHWilds = 41,
        Pragmata = 43,
        RE9 = Pragmata,
    }

    public class GuiFile : BaseFile
    {
        public HeaderStruct Header { get; } = new();
        public DisplayElement? RootView { get; set; }

        public List<GuiContainer> Containers { get; } = new();
        public Element RootViewElement { get; set; } = null!;

        public List<AttributeOverride> AttributeOverrides { get; } = new();
        public List<string> Resources { get; } = new();
        public List<string> LinkedGUIs { get; } = new();

        public List<GuiParameter> Parameters { get; } = new();
        public List<GuiParameterReference> ParameterReferences { get; } = new();
        public List<GuiParameterOverride> ParameterOverrides { get; } = new();

        public GuiFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        public const uint Magic = 0x52495547;  // GUIR
        public const string Extension2 = ".gui";

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a GUI file");
            }

            Containers.Clear();
            AttributeOverrides.Clear();
            Resources.Clear();
            LinkedGUIs.Clear();
            Parameters.Clear();
            ParameterReferences.Clear();
            ParameterOverrides.Clear();

            var containerCount = (int)handler.Read<long>();

            if (header.GuiVersion < GuiVersion.Pragmata)
            {
                var offsets = handler.ReadArray<long>(containerCount);

                for (int i = 0; i < containerCount; ++i)
                {
                    handler.Seek(offsets[i]);
                    var control = new GuiContainer(header.GuiVersion);
                    control.Info.Read(handler);
                    Containers.Add(control);
                }
            }
            else
            {
                handler.Seek(header.offsetsStart);
                for (int i = 0; i < containerCount; ++i)
                {
                    var control = new GuiContainer(header.GuiVersion);
                    control.Info.Read(handler);
                    Containers.Add(control);
                }
            }

            foreach (var container in Containers)
            {
                if (container.Info.elementsOffset > 0)
                {
                    handler.Seek(container.Info.elementsOffset);
                    var elemCount = (int)handler.Read<long>();

                    if (header.GuiVersion >= GuiVersion.Pragmata)
                    {
                        for (int i = 0; i < elemCount; ++i)
                        {
                            var sub = new Element(header.GuiVersion);
                            sub.Read(handler);
                            container.Elements.Add(sub);
                        }
                    }
                    else
                    {
                        var elementOffsets = handler.ReadArray<long>(elemCount);
                        foreach (var subOffset in elementOffsets)
                        {
                            handler.Seek(subOffset);
                            var sub = new Element(header.GuiVersion);
                            sub.Read(handler);
                            container.Elements.Add(sub);
                        }
                    }
                }

                handler.Seek(container.Info.clipsOffset);
                var totalClipCount = handler.Read<int>();
                var clipCount = handler.Read<int>();
                DataInterpretationException.DebugThrowIf(clipCount != 0 && totalClipCount % clipCount != 0);
                var clipOffsets = handler.ReadArray<long>(totalClipCount);

                foreach (var clipOffset in clipOffsets)
                {
                    handler.Seek(clipOffset);
                    var gclip = new GuiClip(header.GuiVersion);
                    gclip.Read(handler);
                    container.Clips.Add(gclip);
                }

                if (container.Info.attributesOffset1 > 0)
                {
                    handler.Seek(container.Info.attributesOffset1);
                    var attrCount = (int)handler.Read<long>();
                    container.Attributes1.Clear();
                    for (int i = 0; i < attrCount; ++i)
                    {
                        var c = new ContainerAttribute1(header.GuiVersion);
                        c.Read(handler);
                        container.Attributes1.Add(c);
                    }
                }

                if (container.Info.attributesOffset2 > 0)
                {
                    handler.Seek(container.Info.attributesOffset2);
                    var attrCount = (int)handler.Read<long>();
                    container.Attributes2.Clear();
                    for (int i = 0; i < attrCount; ++i)
                    {
                        var c = new ContainerAttribute2(header.GuiVersion);
                        c.Read(handler);
                        container.Attributes2.Add(c);
                    }
                }
            }

            foreach (var elem in Containers)
            {
                foreach (var gclip in elem.Clips)
                {
                    if (gclip.transitionAfterEndClipOffset != 0)
                    {
                        // the "next" clip doesn't need to be on the same element
                        gclip.NextClip = Containers.SelectMany(e => e.Clips).First(clip => clip.Start == gclip.transitionAfterEndClipOffset);
                    }
                }
            }

            handler.Seek(header.viewOffset);
            RootViewElement = new Element(header.GuiVersion);
            RootViewElement.Read(handler);
            DataInterpretationException.DebugThrowIf(RootViewElement.ClassName != "via.gui.View");

            int count;
            if (header.uknOffset != 0)
            {
                handler.Seek(header.uknOffset);
                count = (int)handler.Read<long>();
                DataInterpretationException.DebugThrowIf(count != 0);
            }

            handler.Seek(header.attributeOverridesOffset);
            count = (int)handler.Read<long>();
            for (int i = 0; i < count; ++i)
            {
                var attr = new AttributeOverride(header.GuiVersion);
                attr.Read(handler);
                AttributeOverrides.Add(attr);
            }

            if (header.parameterDataOffset > 0)
            {
                handler.Seek(header.parameterDataOffset);
                var offs1 = handler.Read<long>();
                var offs2 = handler.Read<long>();
                var offs3 = handler.Read<long>();
                handler.Seek(offs1);
                count = (int)handler.Read<long>();

                for (int i = 0; i < count; ++i)
                {
                    var pr = new GuiParameter();
                    pr.Read(handler);
                    Parameters.Add(pr);
                }

                handler.Seek(offs2);
                count = (int)handler.Read<long>();
                for (int i = 0; i < count; ++i)
                {
                    var pr = new GuiParameterReference();
                    pr.Read(handler);
                    ParameterReferences.Add(pr);
                }

                handler.Seek(offs3);
                count = (int)handler.Read<long>();
                for (int i = 0; i < count; ++i)
                {
                    var pr = new GuiParameterOverride();
                    pr.Read(handler);
                    ParameterOverrides.Add(pr);
                }
            }

            handler.Seek(header.guiFilesOffset);
            count = (int)handler.Read<long>();
            for (int i = 0; i < count; ++i) LinkedGUIs.Add(handler.ReadOffsetWString());

            handler.Seek(header.resourcePathsOffset);
            count = (int)handler.Read<long>();
            for (int i = 0; i < count; ++i) Resources.Add(handler.ReadOffsetWString());

            ConstructElementHierarchy();
            return true;
        }

        private void ConstructElementHierarchy()
        {
            var containersDict = Containers.ToDictionary(c => c.Info.ID);
            RootView = new DisplayElement(RootViewElement, containersDict[RootViewElement.ContainerID]);
            SetupDisplayElementChildren(RootView, containersDict);
        }

        private void SetupDisplayElementChildren(DisplayElement element, Dictionary<GuiObjectID, GuiContainer> containersDict)
        {
            if (element.Container == null) return;

            foreach (var child in element.Container.Elements)
            {
                // note: ContainerId can reference a ChildGui's container, that's not handled yet
                var displayChild = new DisplayElement(child, containersDict.GetValueOrDefault(child.ContainerID));
                SetupDisplayElementChildren(displayChild, containersDict);
                element.Children.Add(displayChild);
            }
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var header = Header;
            header.Write(handler);

            handler.Write((long)Containers.Count);
            if (header.GuiVersion >= GuiVersion.Pragmata)
            {
                header.offsetsStart = handler.Tell();
                for (int i = 0; i < Containers.Count; ++i)
                {
                    Containers[i].Info.Write(handler);
                }
            }
            else
            {
                var elementsStartOffset = handler.Tell();
                handler.Skip(Containers.Count * 8);

                header.offsetsStart = handler.Tell();
                for (int i = 0; i < Containers.Count; ++i)
                {
                    handler.Write(elementsStartOffset + i * 8, handler.Tell());
                    Containers[i].Info.Write(handler);
                }
            }

            if (header.GuiVersion >= GuiVersion.Pragmata)
            {
                header.viewOffset = handler.Tell();
                RootViewElement.Write(handler);
                RootViewElement.WriteAttributes(handler);

                foreach (var container in Containers)
                {
                    handler.Align(16);
                    container.Info.elementsOffset = handler.Tell();
                    handler.Write((long)container.Elements.Count);
                    for (int i = 0; i < container.Elements.Count; ++i)
                    {
                        container.Elements[i].Write(handler);
                    }

                    for (int i = 0; i < container.Elements.Count; ++i)
                    {
                        container.Elements[i].WriteAttributes(handler);
                    }

                    container.Info.clipsOffset = handler.Tell();
                    handler.Write(container.Clips.Count);
                    handler.Write(container.Clips.DistinctBy(c => c.name).Count()); // maybe
                    handler.Skip(container.Clips.Count * 8);
                    for (int i = 0; i < container.Clips.Count; ++i)
                    {
                        handler.Write(container.Info.clipsOffset + 8 + i * 8, handler.Tell());
                        container.Clips[i].Write(handler);
                    }

                    if (container.Attributes1.Count > 0)
                    {
                        container.Info.attributesOffset1 = handler.Tell();
                        handler.Write((long)container.Attributes1.Count);
                        foreach (var attr in container.Attributes1) attr.Write(handler);
                    }
                    else container.Info.attributesOffset1 = 0;

                    if (container.Attributes2.Count > 0)
                    {
                        container.Info.attributesOffset2 = handler.Tell();
                        handler.Write((long)container.Attributes2.Count);
                        foreach (var attr in container.Attributes2) attr.Write(handler);
                    }
                    else container.Info.attributesOffset2 = 0;
                }
            }
            else
            {
                foreach (var container in Containers)
                {
                    container.Info.elementsOffset = handler.Tell();
                    handler.Write((long)container.Elements.Count);
                    handler.Skip(container.Elements.Count * 8);

                    container.Info.clipsOffset = handler.Tell();
                    handler.Write(container.Clips.Count);
                    handler.Write(container.Clips.DistinctBy(c => c.name).Count()); // maybe
                    handler.Skip(container.Clips.Count * 8);
                }

                header.viewOffset = handler.Tell();
                RootViewElement.Write(handler);
                RootViewElement.WriteAttributes(handler);

                foreach (var container in Containers)
                {
                    for (int i = 0; i < container.Elements.Count; ++i)
                    {
                        handler.Write(container.Info.elementsOffset + 8 + i * 8, handler.Tell());
                        container.Elements[i].Write(handler);
                        container.Elements[i].WriteAttributes(handler);
                    }
                }

                foreach (var container in Containers)
                {
                    for (int i = 0; i < container.Clips.Count; ++i)
                    {
                        handler.Write(container.Info.clipsOffset + 8 + i * 8, handler.Tell());
                        container.Clips[i].Write(handler);
                    }
                }
            }

            var endSectionOffset = handler.Tell();

            foreach (var container in Containers)
            {
                container.Info.Rewrite(handler);
                foreach (var gclip in container.Clips)
                {
                    if (gclip.NextClip != null)
                    {
                        gclip.transitionAfterEndClipOffset = gclip.NextClip.Start;
                        handler.Seek(gclip.Start);
                        gclip.WriteHeader(handler);
                    }
                }
            }

            handler.Seek(endSectionOffset);
            handler.Align(16);

            if (header.GuiVersion < GuiVersion.DD2)
            {
                header.uknOffset = handler.Tell();
                handler.WriteNull(8);
            }

            header.attributeOverridesOffset = handler.Tell();
            handler.Write((long)AttributeOverrides.Count);
            AttributeOverrides.Write(handler);

            if (header.GuiVersion is >= GuiVersion.RE_RT and < GuiVersion.Pragmata)
            {
                // NOTE: with pragmata, this data seems to be stored as an offset in elements/containers
                header.parameterDataOffset = handler.Tell();
                handler.Skip(24);

                handler.Write(header.parameterDataOffset, handler.Tell());
                handler.Write((long)Parameters.Count);
                Parameters.Write(handler);
                handler.Align(16);

                handler.Write(header.parameterDataOffset + 8, handler.Tell());
                handler.Write((long)ParameterReferences.Count);
                ParameterReferences.Write(handler);
                handler.Align(16);

                handler.Write(header.parameterDataOffset + 16, handler.Tell());
                handler.Write((long)ParameterOverrides.Count);
                ParameterOverrides.Write(handler);
                handler.Align(16);
            }

            header.guiFilesOffset = handler.Tell();
            handler.Write((long)LinkedGUIs.Count);
            foreach (var str in LinkedGUIs) handler.WriteOffsetWString(str);

            header.resourcePathsOffset = handler.Tell();
            handler.Write((long)Resources.Count);
            foreach (var str in Resources) handler.WriteOffsetWString(str);

            handler.StringTableFlush();
            handler.AsciiStringTableFlush();
            handler.OffsetContentTableFlush();

            header.Write(handler, 0);
            return true;
        }
    }
}


namespace ReeLib.Gui
{
    public class HeaderStruct : BaseModel {
        public uint version;
        public uint magic;
        internal long offsetsStartOffset;
        internal long uknOffset;
        internal long attributeOverridesOffset;
        internal long guiFilesOffset;
        internal long resourcePathsOffset;
        internal long parameterDataOffset;
        internal long offsetsStart;
        internal long viewOffset;

        public GuiVersion GuiVersion { get; private set; }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref version);
            GuiVersion = (GuiVersion)(version % 100);
            handler.Read(ref magic);
            handler.Read(ref offsetsStartOffset);

            if (GuiVersion < GuiVersion.DD2) handler.Read(ref uknOffset);
            handler.Read(ref attributeOverridesOffset);
            if (GuiVersion is >= GuiVersion.RE_RT and < GuiVersion.Pragmata) handler.Read(ref parameterDataOffset);
            handler.Read(ref guiFilesOffset);
            handler.Read(ref resourcePathsOffset);

            handler.Seek(offsetsStartOffset);
            handler.Read(ref offsetsStart);
            handler.Read(ref viewOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref version);
            handler.Write(ref magic);
            var addrOffset = handler.Tell();
            handler.Write(ref offsetsStartOffset);

            if (GuiVersion < GuiVersion.DD2) handler.Write(ref uknOffset);
            handler.Write(ref attributeOverridesOffset);
            if (GuiVersion is >= GuiVersion.RE_RT and < GuiVersion.Pragmata) handler.Write(ref parameterDataOffset);
            handler.Write(ref guiFilesOffset);
            handler.Write(ref resourcePathsOffset);

            handler.Write(addrOffset, offsetsStartOffset = handler.Tell());
            handler.Write(ref offsetsStart);
            handler.Write(ref viewOffset);
            return true;
        }
    }

    public class DisplayElement(Element element, GuiContainer? container)
    {
        public Element Element { get; set; } = element;
        public GuiContainer? Container { get; set; } = container;
        public List<DisplayElement> Children { get; } = new();

        public override string ToString() => Element.ToString();
    }

    public class GuiClip(GuiVersion version) : BaseModel
    {
        public GuiObjectID ID;
        public string name = "";
        public EmbeddedClip? clip;
        public bool IsDefault;

        public GuiVersion version = version;

        public GuiClip? NextClip { get; set; }

        internal long transitionAfterEndClipOffset;

        public override string ToString() => (NextClip == null ? name : $"{name} [=> {NextClip.name}]");

        protected override bool DoRead(FileHandler handler)
        {
            ID = GuiObjectID.Read(handler, version);
            var uknNum = handler.Read<long>();
            DataInterpretationException.ThrowIf(uknNum != 0 && uknNum != 1, "It's no bool");
            IsDefault = uknNum != 0;

            name = handler.ReadOffsetWString();
            handler.Read(ref transitionAfterEndClipOffset);

            clip = new EmbeddedClip();
            clip.Read(handler.WithOffset(handler.Tell()));
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            WriteHeader(handler);
            clip!.Write(handler.WithOffset(handler.Tell()));
            return true;
        }

        internal void WriteHeader(FileHandler handler)
        {
            ID.Write(handler, version);
            handler.Write(IsDefault ? 1L : 0L);

            handler.WriteOffsetWString(name);
            handler.Write(ref transitionAfterEndClipOffset);
        }
    }

    public class ContainerAttribute1(GuiVersion version) : Attribute(version)
    {
        public GuiObjectID ID;
        public string Tag { get; set; } = "";
        public string State { get; set; } = "";

        protected override bool DoRead(FileHandler handler)
        {
            ID = GuiObjectID.Read(handler, Version);
            Name = handler.ReadOffsetWString();
            Tag = handler.ReadOffsetAsciiString();
            State = handler.ReadOffsetWString();
            handler.Read<int>(); // nameUtf16hash
            handler.Read(ref propertyType);
            uknInt = handler.Read<byte>();
            handler.ReadNull(2);

            long jumpBack = handler.Tell() + 8;
            Value = propertyType.Read(handler);
            handler.Seek(jumpBack);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            ID.Write(handler, Version);
            handler.WriteOffsetWString(Name);
            handler.WriteOffsetAsciiString(Tag);
            handler.WriteOffsetWString(State);
            handler.Write(MurMur3HashUtils.GetHash(Name));
            handler.Write(ref propertyType);
            handler.Write((byte)uknInt);
            handler.WriteNull(2);
            var valueEndOffset = handler.Tell() + 8;
            propertyType.Write(Value, handler, false);
            handler.Seek(valueEndOffset);
            return true;
        }
    }

    public class ContainerAttribute2(GuiVersion version) : BaseModel
    {
        public GuiObjectID ID;
        public List<GuiObjectID> Items { get; } = new();
        public string Classname { get; set; } = "";
        public string StateName { get; set; } = "";
        public GuiVersion Version { get; } = version;
        public PropertyType propertyType;
        public int uknInt;

        protected override bool DoRead(FileHandler handler)
        {
            ID = GuiObjectID.Read(handler, Version);
            var listOffset = handler.Read<long>();
            handler.Read(ref propertyType);
            handler.ReadNull(3);
            handler.Read(ref uknInt);

            Classname = handler.ReadOffsetAsciiString();
            StateName = handler.ReadOffsetAsciiString();

            using var _ = handler.SeekJumpBack(listOffset);
            var count = (int)handler.Read<long>();
            for (int i = 0; i < count; ++i) Items.Add(GuiObjectID.Read(handler, Version));
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            ID.Write(handler, Version);
            handler.WriteOffsetContent(h => {
                h.Write((long)Items.Count);
                foreach (var id in Items) id.Write(handler, Version);
            });
            handler.Write(ref propertyType);
            handler.WriteNull(3);
            handler.Write(ref uknInt);
            handler.WriteOffsetAsciiString(Classname);
            handler.WriteOffsetAsciiString(StateName);
            return true;
        }
    }

    public class Attribute : BaseModel
    {
        protected GuiVersion Version { get; set; }

        public string Name { get; set; } = "";
        public PropertyType propertyType;
        public int uknInt;

        public int OrderIndex;

        private long nameHash;

        public object Value { get; set; } = 0;

        public Attribute(GuiVersion version)
        {
            Version = version;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref propertyType);
            handler.ReadNull(3);
            handler.Read(ref uknInt);
            Name = handler.ReadOffsetAsciiString();

            long jumpBack = handler.Tell() + 8;
            Value = propertyType.Read(handler);
            handler.Seek(jumpBack);
            if (Version > GuiVersion.RE_RT) handler.Read(ref nameHash);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref propertyType);
            handler.WriteNull(3);
            handler.Write(ref uknInt);
            handler.WriteOffsetAsciiString(Name);

            var valueEndOffset = handler.Tell() + 8;
            propertyType.Write(Value, handler, false);
            handler.Seek(valueEndOffset);
            if (Version > GuiVersion.RE_RT) handler.Write(nameHash = MurMur3HashUtils.GetHash(Name));
            return true;
        }

        public override string ToString() => $"[{Name}]: {Value}";
    }

    public class StateReference : BaseModel
    {
        public GuiObjectID StateId { get; set; }
        public string AsciiName = "";
        public long offsetMaybe;
        public string TypeName = "";

        protected override bool DoRead(FileHandler handler)
        {
            StateId = GuiObjectID.Read(handler, GuiVersion.RE9);
            AsciiName = handler.ReadOffsetAsciiString();
            handler.Read(ref offsetMaybe);
            TypeName = handler.ReadOffsetWString();
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            StateId.Write(handler, GuiVersion.RE9);
            handler.WriteOffsetAsciiString(AsciiName);
            handler.Write(ref offsetMaybe);
            handler.WriteOffsetWString(TypeName);
            return true;
        }
    }

    public class AttributeOverride(GuiVersion version) : Attribute(version)
    {
        public string TargetPath { get; set; } = "";
        public List<GuiObjectID> TargetIDs { get; } = new List<GuiObjectID>();
        public string TargetClassname { get; set; } = "";

        protected override bool DoRead(FileHandler handler)
        {
            if (Version >= GuiVersion.MHWilds)
            {
                using var _ = handler.SeekJumpBack(handler.Read<long>());
                var count = (int)handler.Read<long>();
                TargetIDs.Clear();
                for (int i = 0; i < count; ++i) TargetIDs.Add(new GuiObjectID(handler.Read<long>()));
            }
            else
            {
                TargetPath = handler.ReadOffsetWString();
            }
            TargetClassname = handler.ReadOffsetAsciiString();
            return base.DoRead(handler);
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (Version >= GuiVersion.MHWilds)
            {
                handler.WriteOffsetContent((h) => {
                    h.Write((long)TargetIDs.Count);
                    foreach (var id in TargetIDs) h.Write(id.AsID);
                });
            }
            else
            {
                handler.WriteOffsetWString(TargetPath);
            }
            handler.WriteOffsetAsciiString(TargetClassname);
            return base.DoWrite(handler);
        }

        public override string ToString() => $"{TargetPath} | {base.ToString()}";
    }

    public class ContainerInfo : BaseModel
    {
        public GuiObjectID ID;
        public string Name { get; set; } = "";
        public string ClassName { get; set; } = "";

        internal long elementsOffset;
        internal long clipsOffset;
        internal long attributesOffset1;
        internal long attributesOffset2;

        public GuiVersion version;

        public override string ToString() => ClassName;

        protected override bool DoRead(FileHandler handler)
        {
            ID = GuiObjectID.Read(handler, version);
            Name = handler.ReadOffsetWString();
            ClassName = handler.ReadOffsetAsciiString();
            handler.Read(ref elementsOffset);
            handler.Read(ref clipsOffset);
            if (version >= GuiVersion.Pragmata)
            {
                handler.Read(ref attributesOffset1);
                handler.Read(ref attributesOffset2);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            ID.Write(handler, version);
            handler.WriteOffsetWString(Name);
            handler.WriteOffsetAsciiString(ClassName);
            handler.Write(ref elementsOffset);
            handler.Write(ref clipsOffset);
            if (version >= GuiVersion.Pragmata)
            {
                handler.Write(ref attributesOffset1);
                handler.Write(ref attributesOffset2);
            }
            return true;
        }
    }

    public class GuiContainer
    {
        public GuiContainer(GuiVersion version)
        {
            Info = new ContainerInfo() { version = version };
        }

        public ContainerInfo Info { get; }
        public List<Element> Elements { get; } = new();
        public List<GuiClip> Clips { get; } = new();
        public List<ContainerAttribute1> Attributes1 { get; } = new();
        public List<ContainerAttribute2> Attributes2 { get; } = new();

        public override string ToString()
        {
            return $"{Info.Name}: {Info.ClassName}";
        }
    }

    public struct GuiObjectID : IEquatable<Guid>, IEquatable<GuiObjectID>
    {
        public Guid guid;

        public readonly long AsID
        {
            get
            {
                Span<byte> bytes = stackalloc byte[16];
                guid.TryWriteBytes(bytes);
                return MemoryMarshal.Read<long>(bytes.Slice(8));
            }
        }

        public GuiObjectID(Guid guid)
        {
            this.guid = guid;
        }

        public GuiObjectID(long id)
        {
            Span<byte> bytes = stackalloc byte[16];
            bytes.Clear();
            MemoryMarshal.Write<long>(bytes.Slice(8), id);
            guid = new Guid(bytes);
        }

        public static GuiObjectID Read(FileHandler handler, GuiVersion version)
        {
            if (version >= GuiVersion.Pragmata) {
                return new GuiObjectID(handler.Read<long>());
            } else {
                return new GuiObjectID(handler.Read<Guid>());
            }
        }

        public readonly void Write(FileHandler handler, GuiVersion version)
        {
            if (version >= GuiVersion.Pragmata) {
                handler.Write(AsID);
            } else {
                handler.Write<Guid>(guid);
            }
        }

        public override string ToString() => guid.ToString();

        public bool Equals(Guid other) => guid == other;
        public override bool Equals(object? obj) => obj is GuiObjectID go ? go.guid == guid : false;
        public static bool operator==(GuiObjectID id, GuiObjectID other) => id.guid == other.guid;
        public static bool operator!=(GuiObjectID id, GuiObjectID other) => id.guid != other.guid;
        public static bool operator==(GuiObjectID id, Guid other) => id.guid == other;
        public static bool operator!=(GuiObjectID id, Guid other) => id.guid != other;
        public override int GetHashCode() => guid.GetHashCode();
        public bool Equals(GuiObjectID other) => other == this;
    }

    public class Element(GuiVersion version) : BaseModel
    {
        public string Name { get; set; } = "";
        public string ClassName { get; set; } = "";

        private GuiVersion Version { get; } = version;

        public GuiObjectID ID;
        public GuiObjectID ContainerID;
        public Guid guid3; // could be some sort of "group" or "tag" guid
        private ulong uknNum; // seems like a set of bools
        private long extraStateRefOffset;

        public List<Attribute> Attributes { get; } = new();
        public List<Attribute> ExtraAttributes { get; } = new();
        public List<StateReference> ExtraStateRefs { get; } = new();

        public byte[] ElementData = [];

        protected override bool DoRead(FileHandler handler)
        {
            ID = GuiObjectID.Read(handler, Version);
            ContainerID = GuiObjectID.Read(handler, Version);
            if (Version >= GuiVersion.RE8)
            {
                handler.Read(ref guid3);
            }
            Name = handler.ReadOffsetWString();
            ClassName = handler.ReadOffsetAsciiString();
            var attributesOffset = handler.Read<long>();
            var reordersOffset = Version >= GuiVersion.RE4 ? handler.Read<long>() : 0;
            var extraAttributesOffset = handler.Read<long>();
            if (Version >= GuiVersion.RE_RT) handler.Read(ref extraStateRefOffset);
            var elementDataOffset = handler.Read<long>();
            if (Version >= GuiVersion.RE_RT) handler.Read(ref uknNum);

            var end = handler.Tell();
            handler.Seek(attributesOffset);
            var attributeCount = (int)handler.Read<long>();

            Attributes.Clear();
            for (int i = 0; i < attributeCount; i++)
            {
                Attribute attribute = new(Version);
                attribute.Read(handler);
                Attributes.Add(attribute);
            }

            if (reordersOffset != 0)
            {
                handler.Seek(reordersOffset);
                var indices = handler.ReadArray<short>(attributeCount);
                for (int i = 0; i < attributeCount; ++i)
                {
                    Attributes[i].OrderIndex = indices[i];
                }
                handler.Align(8);
            }

            handler.Seek(extraAttributesOffset);
            var extraAttributeCount = (int)handler.Read<long>();
            for (int i = 0; i < extraAttributeCount; i++)
            {
                Attribute attribute = new(Version);
                attribute.Read(handler);
                ExtraAttributes.Add(attribute);
            }

            if (elementDataOffset != 0)
            {
                // Log.Warn(handler.FilePath + " | " + ClassName + " | Found extra gui subelem data: " + uknExtraOffset);
                handler.Seek(elementDataOffset);
                switch (ClassName) {
                    case "via.gui.TextureSet":
                        ElementData = handler.ReadArray<byte>(handler.Read<int>() * 24);
                        break;
                    case "via.gui.Scale9Grid":
                    case "via.gui.BlurFilter":
                        ElementData = handler.ReadArray<byte>(160);
                        break;
                    default:
                        throw new NotImplementedException("Unhandled GUI element data for type " + ClassName);
                }
            }
            DataInterpretationException.DebugThrowIf(elementDataOffset == 0 && (ClassName == "via.gui.TextureSet" || ClassName == "via.gui.Scale9Grid" || ClassName == "via.gui.BlurFilter"));
            if (extraStateRefOffset > 0) {
                handler.Seek(extraStateRefOffset);
                var count = (int)handler.Read<long>();
                ExtraStateRefs.Read(handler, count);
            }
            handler.Seek(end);

            return true;
        }

        private long attributesOffsetStart;
        protected override bool DoWrite(FileHandler handler)
        {
            ID.Write(handler, Version);
            ContainerID.Write(handler, Version);
            if (Version >= GuiVersion.RE8)
            {
                handler.Write(ref guid3);
            }
            handler.WriteOffsetWString(Name);
            handler.WriteOffsetAsciiString(ClassName);

            attributesOffsetStart = handler.Tell();
            handler.Skip(8); // attributesOffset
            if (Version >= GuiVersion.RE4) handler.Skip(8); // reordersOffset
            handler.Skip(8); // extraAttributesOffset
            if (Version >= GuiVersion.RE_RT) handler.WriteNull(8); // extraStateRefOffset

            switch (ClassName) {
                case "via.gui.TextureSet":
                    handler.WriteOffsetContent((h) => {
                        h.Write(ElementData.Length / 24);
                        h.WriteArray(ElementData);
                    });
                    break;
                case "via.gui.Scale9Grid":
                case "via.gui.BlurFilter":
                    handler.WriteOffsetContent((h) => h.WriteArray(ElementData));
                    break;
                default:
                    handler.WriteNull(8);
                    break;
            }

            if (Version >= GuiVersion.RE_RT) handler.Write(ref uknNum);
            return true;
        }

        internal void WriteAttributes(FileHandler handler)
        {
            var offset = attributesOffsetStart;
            handler.Write(offset, handler.Tell());
            handler.Write((long)Attributes.Count);

            foreach (var attr in Attributes)
            {
                attr.Write(handler);
            }

            if (Version >= GuiVersion.RE4)
            {
                handler.Write(offset += 8, handler.Tell());
                foreach (var attr in Attributes) handler.Write((short)attr.OrderIndex);
                handler.Align(8);
            }

            handler.Write(offset + 8, handler.Tell()); // extraAttributesOffset
            handler.Write((long)ExtraAttributes.Count);
            ExtraAttributes.Write(handler);

            if (ExtraAttributes.Count > 0) // extraStateRefOffset
            {
                handler.Write(offset + 16, handler.Tell());
                handler.Write((long)ExtraStateRefs.Count);
                ExtraStateRefs.Write(handler);
            }
        }

        public override string ToString() => $"{Name}: {ClassName}";
    }

    public class GuiParameter : BaseModel
    {
        public Guid guid;
        public string name = "";
        public string classname = "";
        public string targetType = "";
        public PropertyType type;
        public byte uknByte;
        public object? value;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guid);
            name = handler.ReadOffsetWString();
            classname = handler.ReadOffsetAsciiString();
            targetType = handler.ReadOffsetWString();
            handler.Read(ref type);
            handler.Read(ref uknByte);
            handler.Skip(6);
            value = type.Read(handler);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref guid);
            handler.WriteOffsetWString(name);
            handler.WriteOffsetAsciiString(classname);
            handler.WriteOffsetWString(targetType);
            handler.Write(ref type);
            handler.Write(ref uknByte);
            handler.WriteNull(6);
            type.Write(value ?? 0L, handler, false);
            return true;
        }

        public override string ToString() => name;
    }

    public class GuiParameterReference : BaseModel
    {
        public Guid guid;
        public string path = "";

        public PropertyType type;
        public int uknInt;
        public string name = "";
        public string field = "";

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guid);
            path = handler.ReadOffsetWString();
            handler.Read(ref type);
            handler.ReadNull(3);
            handler.Read(ref uknInt);
            name = handler.ReadOffsetAsciiString();
            field = handler.ReadOffsetAsciiString();
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref guid);
            handler.WriteOffsetWString(path);
            handler.Write(ref type);
            handler.WriteNull(3);
            handler.Write(ref uknInt);
            handler.WriteOffsetAsciiString(name);
            handler.WriteOffsetAsciiString(field);
            return true;
        }

        public override string ToString() => path;
    }

    public class GuiParameterOverride : BaseModel
    {
        public Guid guid;
        public string path = "";
        public PropertyType type;
        public byte uknByte;
        public string? str;
        public object? value;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guid);
            path = handler.ReadOffsetWString();
            handler.Read(ref type);
            handler.Read(ref uknByte);
            handler.Skip(6);
            var strOffset = handler.Read<long>();
            str = strOffset == 0 ? null : handler.ReadOffsetAsciiString();
            value = type.Read(handler);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref guid);
            handler.WriteOffsetWString(path);
            handler.Write(ref type);
            handler.Write(ref uknByte);
            handler.Skip(6);
            if (str == null) handler.WriteNull(8);
            else handler.WriteOffsetAsciiString(str);
            type.Write(value ?? 0L, handler, false);
            return true;
        }

        public override string ToString() => $"{guid}: {value}";
    }
}
