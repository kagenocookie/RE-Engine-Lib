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
    }

    public class GuiFile : BaseFile
    {
        public HeaderStruct Header { get; } = new();
        public DisplayElement? RootView { get; set; }

        public List<GuiContainer> Containers { get; } = new();
        public Element RootViewElement { get; set; } = null!;

        public List<ResourceAttribute> ResourceAttributes { get; } = new();
        public List<string> Resources { get; } = new();
        public List<string> ChildGUIs { get; } = new();

        public List<ChildGuiOverride> ChildGuiOverries { get; } = new();
        public List<AdditionalData2> Additional2 { get; } = new();
        public List<AdditionalData3> Additional3 { get; } = new();

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
            ResourceAttributes.Clear();
            Resources.Clear();
            ChildGUIs.Clear();
            ChildGuiOverries.Clear();
            Additional2.Clear();
            Additional3.Clear();

            var offsets = handler.ReadArray<long>((int)handler.Read<long>());

            for (int i = 0; i < offsets.Length; ++i)
            {
                handler.Seek(offsets[i]);
                var control = new GuiContainer();
                control.Info.Read(handler);
                Containers.Add(control);
            }

            foreach (var container in Containers)
            {
                handler.Seek(container.Info.elementsOffset);
                var elementOffsets = handler.ReadArray<long>((int)handler.Read<long>());

                handler.Seek(container.Info.clipsOffset);
                var totalClipCount = handler.Read<int>();
                var clipCount = handler.Read<int>();
                DataInterpretationException.DebugThrowIf(clipCount != 0 && totalClipCount % clipCount != 0);
                var clipOffsets = handler.ReadArray<long>(totalClipCount);

                foreach (var subOffset in elementOffsets)
                {
                    handler.Seek(subOffset);
                    var sub = new Element(header.GuiVersion);
                    sub.Read(handler);
                    container.Elements.Add(sub);
                }

                foreach (var clipOffset in clipOffsets)
                {
                    handler.Seek(clipOffset);
                    var gclip = new GuiClip();
                    gclip.Read(handler);
                    container.Clips.Add(gclip);
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

            handler.Seek(header.resourceAttributesOffset);
            count = (int)handler.Read<long>();
            for (int i = 0; i < count; ++i)
            {
                var attr = new ResourceAttribute(header.GuiVersion);
                attr.Read(handler);
                ResourceAttributes.Add(attr);
            }

            if (header.additionalDataOffset > 0)
            {
                handler.Seek(header.additionalDataOffset);
                var offs1 = handler.Read<long>();
                var offs2 = handler.Read<long>();
                var offs3 = handler.Read<long>();
                handler.Seek(offs1);
                count = (int)handler.Read<long>();

                for (int i = 0; i < count; ++i)
                {
                    var pr = new ChildGuiOverride();
                    pr.Read(handler);
                    ChildGuiOverries.Add(pr);
                }

                handler.Seek(offs2);
                count = (int)handler.Read<long>();
                for (int i = 0; i < count; ++i)
                {
                    var pr = new AdditionalData2();
                    pr.Read(handler);
                    Additional2.Add(pr);
                }

                handler.Seek(offs3);
                count = (int)handler.Read<long>();
                for (int i = 0; i < count; ++i)
                {
                    var pr = new AdditionalData3();
                    pr.Read(handler);
                    Additional3.Add(pr);
                }
            }

            handler.Seek(header.guiFilesOffset);
            count = (int)handler.Read<long>();
            for (int i = 0; i < count; ++i) ChildGUIs.Add(handler.ReadOffsetWString());

            handler.Seek(header.resourcePathsOffset);
            count = (int)handler.Read<long>();
            for (int i = 0; i < count; ++i) Resources.Add(handler.ReadOffsetWString());

            ConstructElementHierarchy();
            return true;
        }

        private void ConstructElementHierarchy()
        {
            var containersDict = Containers.ToDictionary(c => c.Info.guid);
            RootView = new DisplayElement(RootViewElement, containersDict[RootViewElement.ContainerId]);
            SetupDisplayElementChildren(RootView, containersDict);
        }

        private void SetupDisplayElementChildren(DisplayElement element, Dictionary<Guid, GuiContainer> containersDict)
        {
            if (element.Container == null) return;

            foreach (var child in element.Container.Elements)
            {
                // note: ContainerId can reference a ChildGui's container, that's not handled yet
                var displayChild = new DisplayElement(child, containersDict.GetValueOrDefault(child.ContainerId));
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
            var elementsStartOffset = handler.Tell();
            handler.Skip(Containers.Count * 8);

            header.offsetsStart = handler.Tell();
            for (int i = 0; i < Containers.Count; ++i)
            {
                handler.Write(elementsStartOffset + i * 8, handler.Tell());
                Containers[i].Info.Write(handler);
            }

            foreach (var elem in Containers)
            {
                elem.Info.elementsOffset = handler.Tell();
                handler.Write((long)elem.Elements.Count);
                handler.Skip(elem.Elements.Count * 8);

                elem.Info.clipsOffset = handler.Tell();
                handler.Write(elem.Clips.Count);
                handler.Write(elem.Clips.DistinctBy(c => c.name).Count()); // maybe
                handler.Skip(elem.Clips.Count * 8);
                elem.Info.Write(handler, elem.Info.Start);
            }

            header.viewOffset = handler.Tell();
            RootViewElement.Write(handler);

            foreach (var elem in Containers)
            {
                for (int i = 0; i < elem.Elements.Count; ++i)
                {
                    handler.Write(elem.Info.elementsOffset + 8 + i * 8, handler.Tell());
                    elem.Elements[i].Write(handler);
                }
            }

            foreach (var elem in Containers)
            {
                for (int i = 0; i < elem.Clips.Count; ++i)
                {
                    handler.Write(elem.Info.clipsOffset + 8 + i * 8, handler.Tell());
                    elem.Clips[i].Write(handler);
                }
            }

            var endSectionOffset = handler.Tell();

            foreach (var elem in Containers)
            {
                foreach (var gclip in elem.Clips)
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

            header.resourceAttributesOffset = handler.Tell();
            handler.Write((long)ResourceAttributes.Count);
            ResourceAttributes.Write(handler);

            if (header.GuiVersion >= GuiVersion.RE_RT)
            {
                header.additionalDataOffset = handler.Tell();
                handler.Skip(24);

                handler.Write(header.additionalDataOffset, handler.Tell());
                handler.Write((long)ChildGuiOverries.Count);
                ChildGuiOverries.Write(handler);
                handler.Align(16);

                handler.Write(header.additionalDataOffset + 8, handler.Tell());
                handler.Write((long)Additional2.Count);
                Additional2.Write(handler);
                handler.Align(16);

                handler.Write(header.additionalDataOffset + 16, handler.Tell());
                handler.Write((long)Additional3.Count);
                Additional3.Write(handler);
                handler.Align(16);
            }

            header.guiFilesOffset = handler.Tell();
            handler.Write((long)ChildGUIs.Count);
            foreach (var str in ChildGUIs) handler.WriteOffsetWString(str);

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
        internal long resourceAttributesOffset;
        internal long guiFilesOffset;
        internal long resourcePathsOffset;
        internal long additionalDataOffset;
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
            handler.Read(ref resourceAttributesOffset);
            if (GuiVersion >= GuiVersion.RE_RT) handler.Read(ref additionalDataOffset);
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
            handler.Write(ref resourceAttributesOffset);
            if (GuiVersion >= GuiVersion.RE_RT) handler.Write(ref additionalDataOffset);
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

    public class GuiClip : BaseModel
    {
        public Guid guid;
        public string name = "";
        public ClipEntry? clip;
        public bool IsDefault;

        public GuiClip? NextClip { get; set; }

        internal long transitionAfterEndClipOffset;

        public override string ToString() => (NextClip == null ? name : $"{name} [=> {NextClip.name}]");

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read<Guid>(ref guid);
            var uknNum = handler.Read<long>();
            DataInterpretationException.ThrowIf(uknNum != 0 && uknNum != 1, "It's no bool");
            IsDefault = uknNum != 0;

            name = handler.ReadOffsetWString();
            handler.Read(ref transitionAfterEndClipOffset);

            clip = new ClipEntry();
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
            handler.Write<Guid>(ref guid);
            handler.Write(IsDefault ? 1L : 0L);

            handler.WriteOffsetWString(name);
            handler.Write(ref transitionAfterEndClipOffset);
        }
    }

    public class Attribute : BaseModel
    {
        private GuiVersion Version { get; set; }

        public string Name { get; set; } = "";
        public PropertyType propertyType;
        public int uknInt;

        public int OrderIndex;

        public long valueOffset;
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

    public class ResourceAttribute(GuiVersion version) : Attribute(version)
    {
        public string TargetPath { get; set; } = "";
        public string TargetField { get; set; } = "";

        protected override bool DoRead(FileHandler handler)
        {
            TargetPath = handler.ReadOffsetWString();
            TargetField = handler.ReadOffsetAsciiString();
            return base.DoRead(handler);
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.WriteOffsetWString(TargetPath);
            handler.WriteOffsetAsciiString(TargetField);
            return base.DoWrite(handler);
        }

        public override string ToString() => $"{TargetPath} | {base.ToString()}";
    }

    public class ContainerInfo : BaseModel
    {
        public Guid guid;
        public string Name { get; set; } = "";
        public string ClassName { get; set; } = "";

        internal long elementsOffset;
        internal long clipsOffset;

        public override string ToString() => ClassName;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guid);
            Name = handler.ReadOffsetWString();
            ClassName = handler.ReadOffsetAsciiString();
            handler.Read(ref elementsOffset);
            handler.Read(ref clipsOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref guid);
            handler.WriteOffsetWString(Name);
            handler.WriteOffsetAsciiString(ClassName);
            handler.Write(ref elementsOffset);
            handler.Write(ref clipsOffset);
            return true;
        }
    }

    public class GuiContainer
    {
        public ContainerInfo Info { get; } = new();
        public List<Element> Elements { get; } = new();
        public List<GuiClip> Clips { get; } = new();

        public override string ToString()
        {
            return $"{Info.Name}: {Info.ClassName}";
        }
    }

    public class Element(GuiVersion version) : BaseModel
    {
        public string Name { get; set; } = "";
        public string ClassName { get; set; } = "";

        private GuiVersion Version { get; } = version;

        public Guid ID;
        public Guid ContainerId;
        public Guid guid3; // could be some sort of "group" or "tag" guid
        private ulong uknNum; // seems like a set of bools

        public List<Attribute> Attributes { get; } = new();
        public List<Attribute> ExtraAttributes { get; } = new();

        public byte[] ElementData = [];

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref ID);
            handler.Read(ref ContainerId);
            if (Version >= GuiVersion.RE8)
            {
                handler.Read(ref guid3);
            }
            Name = handler.ReadOffsetWString();
            ClassName = handler.ReadOffsetAsciiString();
            var attributesOffset = handler.Read<long>();
            var reordersOffset = Version >= GuiVersion.RE4 ? handler.Read<long>() : 0;
            var extraAttributesOffset = handler.Read<long>();
            if (Version >= GuiVersion.RE_RT) handler.ReadNull(8);
            var elementDataOffset = handler.Read<long>();
            if (Version >= GuiVersion.RE_RT) handler.Read(ref uknNum);

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

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref ID);
            handler.Write(ref ContainerId);
            if (Version >= GuiVersion.RE8)
            {
                handler.Write(ref guid3);
            }
            handler.WriteOffsetWString(Name);
            handler.WriteOffsetAsciiString(ClassName);

            var attributesOffsetStart = handler.Tell();
            handler.Skip(8);
            if (Version >= GuiVersion.RE4) handler.Skip(8);
            handler.Skip(8);
            if (Version >= GuiVersion.RE_RT) handler.WriteNull(8);

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
            WriteAttributes(handler, attributesOffsetStart);
            return true;
        }

        private void WriteAttributes(FileHandler handler, long offsetStart)
        {
            handler.Write(offsetStart, handler.Tell());
            handler.Write((long)Attributes.Count);

            foreach (var attr in Attributes)
            {
                attr.Write(handler);
            }

            if (Version >= GuiVersion.RE4)
            {
                handler.Write(offsetStart += 8, handler.Tell());
                foreach (var attr in Attributes) handler.Write((short)attr.OrderIndex);
                handler.Align(8);
            }

            handler.Write(offsetStart + 8, handler.Tell());
            handler.Write((long)ExtraAttributes.Count);
            ExtraAttributes.Write(handler);

        }

        public override string ToString() => $"{Name}: {ClassName}";
    }

    public class ChildGuiOverride : BaseModel
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

    public class AdditionalData2 : BaseModel
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

    public class AdditionalData3 : BaseModel
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
    }
}
