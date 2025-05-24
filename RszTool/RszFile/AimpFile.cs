using System.Numerics;
using System.Runtime.InteropServices;
using RszTool.Aimp;


namespace RszTool.Aimp
{
    public enum AimpFormat
    {
        Invalid,
        // unknown: RE3, RE7

        /// <summary>
        /// DMC5, RE2(27)
        /// </summary>
        Format28 = 28,

        /// <summary>
        /// RE3 RT, RE2 RT, RE7 RT, RE8, MHRise
        /// </summary>
        Format41 = 41,

        /// <summary>
        /// RE4, SF6
        /// </summary>
        Format43 = 43,

        /// <summary>
        /// DD2, MHWilds
        /// </summary>
        Format46 = 46,
    }

    public enum ContentGroupType
    {
        ContentGroupMapPoint,
        ContentGroupTriangle,
        ContentGroupPolygon,
        ContentGroupMapBoundary,
        ContentGroupMapAABB,
    }

    public class AimpHeader : BaseModel
    {
        public uint magic;
        public string? name;
        public string? hash;
        public int flags;
        public Guid guid;
        public int ukn1;
        public uint hash1;
        public uint hash2;
        public int ukn2;

        public long layersOffset;
        public long rszOffset;
        public long embeddedContentOffset;
        public long contentGroup1Offset;

        public long indexDataOffset;
        public long secondContentGroupOffset;
        public long secondNodeTableOffset;

        public AimpFormat Version { get; }

        public AimpHeader(AimpFormat version)
        {
            Version = version;
            if (version == AimpFormat.Invalid) {
                throw new Exception("Invalid AIMP file format");
            }
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref magic);
            name = handler.ReadInlineWString();
            if (Version > AimpFormat.Format41) {
                hash = handler.ReadInlineWString();
            }

            handler.Read(ref flags);
            if (Version < AimpFormat.Format46) {
                handler.Read(ref guid);
                handler.Read(ref ukn1);
            } else {
                handler.Read(ref ukn1);
                handler.Read(ref hash1);
                handler.Read(ref hash2);
                handler.Read(ref ukn2);
            }

            handler.Read(ref layersOffset);
            handler.Read(ref rszOffset);
            handler.Read(ref embeddedContentOffset);

            handler.Read(ref contentGroup1Offset);
            handler.Read(ref indexDataOffset);
            handler.Read(ref secondContentGroupOffset);
            handler.Read(ref secondNodeTableOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            return true;
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 40)]
    public struct ContentGroupBounds
    {
        [FieldOffset(0)]
        public float ukn1;
        [FieldOffset(4)]
        public float ukn2;
        [FieldOffset(8)]
        public Vector3 min;
        [FieldOffset(24)]
        public Vector3 max;
    }

    public struct TriangleNode
    {
        public uint index1;
        public uint index2;
        public uint index3;
        public byte flag1;
        public byte flag2;
        public byte flag3;
        public byte flag4;
        public Vector3 position;
    }

    public class PolygonNode
    {
        public int pointCount;
        public uint[]? numbers;
        public byte[]? bytes;
        public float[]? floats;
        public Vector3 min;
        public Vector3 max;

        public void Read(FileHandler handler)
        {
            handler.Read(ref pointCount);
            numbers = handler.ReadArray<uint>(pointCount);
            bytes = handler.ReadArray<byte>(pointCount);
            handler.Align(4);
            floats = handler.ReadArray<float>(pointCount);
            handler.Read(ref min);
            handler.Read(ref max);
        }

        public void Write(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }

    public struct MapBoundaryNode
    {
        public int[] indices;
        public Vector3 min;
        public Vector3 max;
    }

    public struct AABBNode
    {
        public byte[] indices;
        public int[] data;
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct PaddedVec3
    {
        [FieldOffset(0)] public float x;
        [FieldOffset(4)] public float y;
        [FieldOffset(8)] public float z;

        public Vector3 Vector3 => new Vector3(x, y, z);
        public override string ToString() => $"{x}, {y}, {z}";
    }

    public class IndexQuadData
    {
        public int[]? type1;
        public int[]? type2;
        public int[]? type3;
        public int[]? type4;

        public void Read(FileHandler handler)
        {
            var c1 = handler.Read<int>();
            var c2 = handler.Read<int>();
            var c3 = handler.Read<int>();
            var c4 = handler.Read<int>();
            type1 = handler.ReadArray<int>(c1);
            type2 = handler.ReadArray<int>(c2);
            type3 = handler.ReadArray<int>(c3);
            type4 = handler.ReadArray<int>(c4);
        }
    }

    public abstract class ContentGroup
    {
        public AimpFormat format;

        public abstract bool ReadNodes(FileHandler handler, int count);
        public abstract bool ReadData(FileHandler handler);

        public static ContentGroup Create(string classname, AimpFormat format)
        {
            ContentGroup content = classname switch {
                "via.navigation.map.ContentGroupMapPoint" => new ContentGroupPoints(),
                "via.navigation.map.ContentGroupTriangle" => new ContentGroupTriangles(),
                "via.navigation.map.ContentGroupPolygon" => new ContentGroupPolygons(),
                "via.navigation.map.ContentGroupMapBoundary" => new ContentGroupMapBoundary(),
                "via.navigation.map.ContentGroupMapAABB" => new ContentGroupAABB(),
                "via.navigation.map.ContentGroupWall" => new ContentGroupWall(),
                _ => throw new NotImplementedException("Unknown content group type " + classname),
            };
            content.format = format;
            return content;
        }
    }

    public struct TriangleInfo
    {
        public int uknId;
        public int index;
        public int id, n1, n2, n3, neighborCount, n4;
    }

    public struct TriangleConnection
    {
        public int index;
        public int triangleIndex, neighborIndex, n3, n4, n5, n6;
    }

    public struct IndexSet
    {
        public int[]? indices;
    };

    public class TriangleData
    {
        public TriangleInfo[]? Triangles;
        public TriangleConnection[]? Connections;
        public int maxIndex;
        public int minIndex;

        public void Read(FileHandler handler, AimpFormat format)
        {
            var triangleCount = handler.Read<int>();
            handler.Read(ref maxIndex);
            if (format >= AimpFormat.Format41) {
                handler.Read(ref minIndex);
            }

            Triangles = new TriangleInfo[triangleCount];
            int connectionCount = 0;
            for (int i = 0; i < triangleCount; ++i) {
                var tri = handler.Read<TriangleInfo>();
                Triangles[i] = tri;
                connectionCount += tri.neighborCount;
            }

            Connections = new TriangleConnection[connectionCount];
            int conn_i = 0;
            for (int i = 0; i < triangleCount; ++i) {
                var tri = Triangles[i];
                for (int k = 0; k < tri.neighborCount; ++k) {
                    Connections[conn_i++] = handler.Read<TriangleConnection>();
                }
            }
        }
    }

    public class TriangleContent
    {
        public PaddedVec3[]? Positions;
        public TriangleData? TriangleData;

        public void Read(FileHandler handler, AimpFormat format)
        {
            Positions = handler.ReadArray<PaddedVec3>(handler.Read<int>());
            TriangleData ??= new();
            TriangleData.Read(handler, format);
        }
    }

    public class ContentGroupPoints : ContentGroup
    {
        public Point[]? points;
        public Connection[]? connections;
        public ConnectionInfo[]? connectionInfos;
        public IndexQuadData? QuadData;
        public int[]? indexData;

        public struct Point
        {
            public Vector3 pos;
            public Vector3 normal;
        }

        public struct Connection
        {
            public int id;
            public int n1, n2, n3, n4, n5, n6, n7;
        }

        public struct ConnectionInfo
        {
            public int id;
            public int connectionId;
            public int n1, n2, n3, n4, n5;
        }

        public override bool ReadNodes(FileHandler handler, int count)
        {
            points = handler.ReadArray<Point>(count);
            return true;
        }

        public override bool ReadData(FileHandler handler)
        {
            indexData = handler.ReadArray<int>(points!.Length);
            return true;
        }
    }

    public class ContentGroupTriangles : ContentGroup
    {
        public TriangleNode[]? nodes;
        public TriangleContent? body;
        public int[]? data;

        public override bool ReadNodes(FileHandler handler, int count)
        {
            nodes = handler.ReadArray<TriangleNode>(count);
            return true;
        }

        public override bool ReadData(FileHandler handler)
        {
            data = handler.ReadArray<int>(nodes!.Length);
            return true;
        }
    }

    public class ContentGroupPolygons : ContentGroup
    {
        public PolygonNode[]? nodes;
        public TriangleContent? body;
        public IndexSet[]? triangleIndices;

        public override bool ReadNodes(FileHandler handler, int count)
        {
            nodes = new PolygonNode[count];
            for (int i = 0; i < count; ++i) {
                var n = new PolygonNode();
                n.Read(handler);
                nodes[i] = n;
            }
            return true;
        }

        public override bool ReadData(FileHandler handler)
        {
            triangleIndices = new IndexSet[nodes!.Length];
            for (int i = 0; i < triangleIndices.Length; ++i) {
                triangleIndices[i] = new IndexSet() {
                    indices = handler.ReadArray<int>(handler.Read<int>()),
                };
            }
            return true;
        }
    }

    public class ContentGroupMapBoundary : ContentGroup
    {
        public Data[]? boundaries;
        public int[]? integers;

        public class Data
        {
            public int[] indices = new int[8];
            public Vector3 min, max;
        }

        public override bool ReadNodes(FileHandler handler, int count)
        {
            boundaries = new Data[count];
            for (int i = 0; i < count; ++i) {
                boundaries[i] = new Data() {
                    indices = handler.ReadArray<int>(8),
                    min = handler.Read<Vector3>(),
                    max = handler.Read<Vector3>(),
                };
            }
            return true;
        }

        public override bool ReadData(FileHandler handler)
        {
            integers = handler.ReadArray<int>(boundaries!.Length);
            return true;
        }
    }

    public class ContentGroupAABB : ContentGroup
    {
        public Data[]? AABBs;
        public TriangleContent? body;
        public IndexSet[]? data;

        public class Data
        {
            public byte[] indices = new byte[8];
            public uint[] values = new uint[13];
        }

        public override bool ReadNodes(FileHandler handler, int count)
        {
            AABBs = new Data[count];
            for (int i = 0; i < count; ++i) {
                AABBs[i] = new Data() {
                    indices = handler.ReadArray<byte>(8),
                    values = format == AimpFormat.Format28 ? Array.Empty<uint>() : handler.ReadArray<uint>(13),
                };
            }
            return true;
        }


        public override bool ReadData(FileHandler handler)
        {
            data = new IndexSet[AABBs!.Length];
            for (int i = 0; i < data.Length; ++i) {
                data[i] = new IndexSet() {
                    indices = handler.ReadArray<int>(handler.Read<int>()),
                };
            }
            return true;
        }
    }
    public class ContentGroupWall : ContentGroup
    {
        public Data[]? AABBs;
        public OffsetData[]? data;

        public class Data
        {
            public via.mat4 matrix;
            public Vector4 scale;
            public Vector4 rotation;
            public Vector4 position;
            public int[] values = new int[8];
        }

        public struct OffsetData { public int a, b; };

        public override bool ReadNodes(FileHandler handler, int count)
        {
            AABBs = new Data[count];
            for (int i = 0; i < count; ++i) {
                AABBs[i] = new Data() {
                    matrix = handler.Read<via.mat4>(),
                    scale = handler.Read<Vector4>(),
                    rotation = handler.Read<Vector4>(),
                    position = handler.Read<Vector4>(),
                    values = handler.ReadArray<int>(8),
                };
            }
            return true;
        }

        public override bool ReadData(FileHandler handler)
        {
            data = handler.ReadArray<OffsetData>(AABBs!.Length);
            return true;
        }
    }

    public class ContentGroupContainer : BaseModel
    {
        public int contentCount;
        public ContentGroup[]? contents;
        public TriangleContent? body;
        public ContentGroupBounds bounds;
        public IndexQuadData? QuadData;
        public AimpFormat version;

        public ContentGroupContainer(AimpFormat version)
        {
            this.version = version;
        }

        private ContentGroup ReadContentHeader(FileHandler handler)
        {
            var classname = handler.ReadInlineWString();
            int count = handler.Read<int>();
            var content = ContentGroup.Create(classname, version);
            content.ReadNodes(handler, count);
            return content;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref contentCount);
            if (contentCount == 0) {
                contents = Array.Empty<ContentGroup>();
            } else {
                contents = new ContentGroup[contentCount];

                for (int i = 0; i < contentCount; ++i) {
                    contents[i] = ReadContentHeader(handler);
                }
            }

            var ukn = handler.Read<int>();
            if (ukn != 0) throw new Exception("Unexpected ukn != 0");
            body ??= new();
            body.Read(handler, version);

            // var boundsPos = handler.Tell();
            // var sizeToDate = boundsPos - Start;

            // I have no idea what this is
            // I have no idea how to determine whether it would need to be there
            // I have no idea how the game would know either
            // The header.ukn value is irrelevant
            // alignment is irrelevant (see DD2 aimapwaypoint_event_11, ch259rootwaypoint)
            // happens with all content group types
            // mostly consistent per game, but not always
            // using the bounds Vector4's paddings (both vectors because there's some cases with x,y,0 coords) as an indicator for whether it's there or not

            // DD2: padding only present for MapPoint groups, sometimes not: worldway.aiwayp - no extra padding
            // could it be there just to fuck with people trying to reverse engineer the format? lol
            var hasMysteryPadding = handler.Read<int>(handler.Tell() + 36) != 0 || handler.Read<int>(handler.Tell() + 20) != 0;
            if (hasMysteryPadding) {
                var padding = handler.Read<int>();
                if (padding != 0) throw new Exception("Mysterious padding is not actually padding!??");
            }

            // var expected = false;
            // var possiblyRelevantData = $"{boundsPos}/align:{boundsPos%16}/line:{boundsPos/16}/size:{sizeToDate}/{sizeToDate%16} {(contentCount == 0 ? "EMPTY" : contents[0].GetType().ToString())}";
            // Console.WriteLine($"Mysterious: {hasMysteryPadding} - {handler.FilePath} - {possiblyRelevantData}");
            // if (hasMysteryPadding != expected) {
            //     // Console.WriteLine($"{possiblyRelevantData}  Expectation: {expected} reality: {hasMysteryPadding} - {handler.FilePath}");
            // } else {
            //     // Console.WriteLine($"{possiblyRelevantData}  Expectation: {expected} reality: {bounds.hasMysteryPadding}");
            // }

            handler.Read(ref bounds);
            QuadData ??= new();
            QuadData.Read(handler);

            return true;
        }

        public void ReadData(FileHandler handler)
        {
            for (int i = 0; i < contentCount; ++i) {
                contents![i].ReadData(handler);
            }
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref contentCount);
            return true;
        }
    }

    public class AimpLayers
    {
        public uint nameHash;
        public string? name;
        public uint flags;

        public void Read(FileHandler handler, AimpFormat format)
        {
            if (format == AimpFormat.Format28) {
                handler.Read(ref nameHash);
            } else {
                name = handler.ReadInlineWString();
            }
            handler.Read(ref flags);
        }

        public override string ToString() => $"{name ?? nameHash.ToString()}: {flags:x}";
    }

    public class EmbeddedGroupData
    {
        public struct EmbedIndexedPosition
        {
            public short a;
            public byte b1, b2;
            public int c, d, e;
            public PaddedVec3 position;
        }

        public struct EmbedIndexedTriangle
        {
            public int index;

            public short b1;
            public byte b2, b3;

            public short c1;
            public byte c2, c3;

            public int d, e, f, g;
        }

        public struct Data2
        {
            public int a, b;
        }

        public EmbedIndexedPosition[]? positions1;
        public PaddedVec3[]? positions2;
        public PaddedVec3 min;
        public PaddedVec3 max;
        public EmbedIndexedTriangle[]? data1;
        public EmbedIndexedTriangle[]? data2;
        public EmbedIndexedTriangle[]? data3;
        public Data2[]? data4;
        public int[]? data5; // unknown
        public ContentGroup? contentGroup;

        public void Read(FileHandler handler, AimpFormat format)
        {
            var nodeCount = handler.Read<int>();
            if (nodeCount > 0) {
                var classname = handler.ReadInlineWString();
                contentGroup = ContentGroup.Create(classname, format);
                contentGroup.ReadNodes(handler, nodeCount);
            }

            positions1 = handler.ReadArray<EmbedIndexedPosition>(handler.ReadInt());
            positions2 = handler.ReadArray<PaddedVec3>(handler.ReadInt());
            handler.Read(ref min);
            handler.Read(ref max);
            data1 = handler.ReadArray<EmbedIndexedTriangle>(handler.ReadInt());
            data2 = handler.ReadArray<EmbedIndexedTriangle>(handler.ReadInt());
            data3 = handler.ReadArray<EmbedIndexedTriangle>(handler.ReadInt());
            if (format > AimpFormat.Format28) {
                data4 = handler.ReadArray<Data2>(handler.ReadInt());
                data5 = handler.ReadArray<int>(handler.ReadInt());
                if (data5.Length > 0) throw new Exception("Found unhandled data5 embed data");
            }
        }
    }

    public class EmbeddedMap
    {
        public uint hash1, hash2;
        public uint hash3, hash4;

        public string? key;

        public int[]? preData1, preData2;

        public EmbeddedGroupData? data1;
        public EmbeddedGroupData? data2;

        public void Read(FileHandler handler, AimpFormat format)
        {
            handler.Read(ref hash1);
            handler.Read(ref hash2);
            if (format == AimpFormat.Format28) {
                handler.Read(ref hash3);
                handler.Read(ref hash4);
            }
            preData1 = handler.ReadArray<int>(handler.ReadInt());
            preData2 = handler.ReadArray<int>(handler.ReadInt());
            data1 ??= new();
            data1.Read(handler, format);
            if (format >= AimpFormat.Format41) {
                data2 ??= new();
                data2.Read(handler, format);
                key = handler.ReadInlineWString();
            }
        }

        public override string ToString() => key ?? $"{hash1} {hash2}";
    }
}


namespace RszTool
{
    public class AimpFile : BaseRszFile
    {
        public enum AimpType
        {
            Invalid,
            Map,
            Navmesh,
            Waypoint,
            VolumeSpace,
            WaypointManager,
            NavmeshManager,
        }

        public AimpHeader Header { get; }
        public RSZFile RSZ { get; }

        public ContentGroupContainer? mainContent;
        public ContentGroupContainer? secondaryContent;
        public AimpLayers[]? layers;

        public int embedHash1;
        public int embedHash2;
        public int embedHash3;
        public int embedHash4;
        public int embedCount;

        public EmbeddedMap[] embeds = Array.Empty<EmbeddedMap>();

        public AimpFile(RszFileOption option, FileHandler fileHandler, AimpType type) : base(option, fileHandler)
        {
            Header = new(DetermineFileFormatFromFileType(type, fileHandler.FileVersion));
            RSZ = new RSZFile(option, fileHandler);
        }

        public AimpFile(RszFileOption option, FileHandler fileHandler)
            : this(option, fileHandler, DetermineMapTypeFromFilename(fileHandler.FilePath))
        {
        }

        private static AimpFormat DetermineFileFormatFromFileType(AimpType type, int fileVersion)
        {
            // the file format itself is identical between several files, but they have different versioning
            // map file versions into a common enum to simplify later versioning logic
            return type switch {
                AimpType.Map => fileVersion switch {
                    27 => AimpFormat.Format28,
                    28 => AimpFormat.Format28,
                    34 => AimpFormat.Format28, // need verify - RE3
                    41 => AimpFormat.Format41,
                    43 => AimpFormat.Format43,
                    > 43 => AimpFormat.Format46,
                    _ => throw new Exception($"Unsupported AIMP file format {type} {fileVersion}"),
                },
                AimpType.Navmesh => fileVersion switch {
                    8 => AimpFormat.Format41,
                    14 => AimpFormat.Format41,
                    17 => AimpFormat.Format43,
                    18 => AimpFormat.Format43,
                    >= 30 => AimpFormat.Format46,
                    _ => throw new Exception($"Unsupported AIMP file format {type} {fileVersion}"),
                },
                AimpType.Waypoint => fileVersion switch {
                    3 => AimpFormat.Format41,
                    5 => AimpFormat.Format43,
                    >= 6 => AimpFormat.Format46,
                    _ => throw new Exception($"Unsupported AIMP file format {type} {fileVersion}"),
                },
                AimpType.VolumeSpace => fileVersion switch {
                    4 => AimpFormat.Format41,
                    6 => AimpFormat.Format43,
                    > 6 => AimpFormat.Format46,
                    _ => throw new Exception($"Unsupported AIMP file format {type} {fileVersion}"),
                },
                AimpType.WaypointManager => fileVersion switch {
                    >= 8 => AimpFormat.Format46,
                    _ => throw new Exception($"Unsupported AIMP file format {type} {fileVersion}"),
                },
                AimpType.NavmeshManager => fileVersion switch {
                    >= 8 => AimpFormat.Format46,
                    _ => throw new Exception($"Unsupported AIMP file format {type} {fileVersion}"),
                },
                _ => throw new Exception($"Invalid AIMP file format {type} {fileVersion}"),
            };
        }

        private static AimpType DetermineMapTypeFromFilename(string? filename)
        {
            RszUtils.GetFileExtension(filename!, out var ext, out _);
            return ext switch {
                ".aimap" => AimpType.Map,
                ".aiwayp" => AimpType.Waypoint,
                ".aiwaypmgr" => AimpType.WaypointManager,
                ".aivspc" => AimpType.VolumeSpace,
                ".ainvm" => AimpType.Navmesh,
                ".ainvmmgr" => AimpType.NavmeshManager,
                _ => AimpType.Invalid,
            };
        }

        public const uint Magic = 0x504D4941;

        public override RSZFile? GetRSZ() => RSZ;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic) {
                throw new InvalidDataException($"{handler.FilePath} Not a AIMP file");
            }
            var format = header.Version;

            if (header.contentGroup1Offset > 0) {
                handler.Seek(header.contentGroup1Offset);
                // note: there are cases with no content groups (DMC5)
                mainContent ??= new(format);
                mainContent.Read(handler);
            }

            if (header.secondContentGroupOffset > 0 && header.secondNodeTableOffset > 0) {
                secondaryContent ??= new(format);
                handler.Seek(header.secondContentGroupOffset);
                secondaryContent.Read(handler);
            }

            var dataOffset = handler.Tell();
            if (header.indexDataOffset > 0) {
                handler.Seek(header.indexDataOffset);
                dataOffset = (dataOffset % 16) == 0 ? dataOffset : dataOffset + (16 - dataOffset % 16);
                if (dataOffset != header.indexDataOffset) {
                    throw new Exception($"Mismatch from expected data offset: {header.indexDataOffset}, expected {dataOffset}");
                }
                if (mainContent != null) {
                    handler.Align(16);
                    mainContent.ReadData(handler);
                }
                if (secondaryContent != null) {
                    handler.Align(16);
                    secondaryContent.ReadData(handler);
                }
            }

            if (header.layersOffset > 0) {
                handler.Seek(header.layersOffset);
                layers = new AimpLayers[64];
                for (int i = 0; i < 64; ++i) {
                    layers[i] = new AimpLayers();
                    layers[i].Read(handler, format);
                }
            }

            if (header.rszOffset > 0) {
                ReadRsz(RSZ, header.rszOffset);
            }

            if (header.embeddedContentOffset > 0) {
                handler.Seek(header.embeddedContentOffset);

                handler.Read(ref embedHash1);
                handler.Read(ref embedHash2);
                if (format <= AimpFormat.Format28) {
                    handler.Read(ref embedHash3);
                    handler.Read(ref embedHash4);
                }

                handler.Read(ref embedCount);
                embeds = new EmbeddedMap[embedCount];
                for (int i = 0; i < embedCount; ++i) {
                    var embed = new EmbeddedMap();
                    embed.Read(handler, format);
                    embeds[i] = embed;
                }
            }
            return true;
        }

        protected override bool DoWrite()
        {
            return true;
        }
    }
}
