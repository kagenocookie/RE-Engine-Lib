namespace ReeLib;

using System;
using ReeLib.Common;
using ReeLib.Pak;

/// <summary>
/// Provides fast entry lookup of any resources inside a set of PAK files. Can read and keep track of all PAK entries.<br/>
/// Thread safe except for methods noted otherwise. Base class methods may not be thread safe either.
/// </summary>
public class CachedMemoryPakReader : PakReader, IDisposable
{
    private sealed record class PakEntryCache(PakFile file, PakEntry entry);

    private volatile Dictionary<ulong, PakEntryCache>? cachedEntries;
    private MemoryStream memoryStream = new();
    public int MatchedEntryCount => cachedEntries?.Count ?? 0;
    public IEnumerable<string> CachedPaths => cachedEntries?.Values.Where(e => e.entry.path != null).Select(e => e.entry.path!) ?? Array.Empty<string>();

    public bool ContainsUnknownFiles => _unknownFiles != null || cachedEntries?.Any(kv => kv.Value.entry.path == null) == true;

    private Dictionary<ulong, KnownFileFormats>? _unknownFiles;
    private string[]? _unknownFilePaths;
    public Dictionary<ulong, KnownFileFormats> UnknownFiles => _unknownFiles ??= CacheUnknownFiles();
    public string[] UnknownFilePaths => _unknownFilePaths ??= UnknownFiles
        .OrderBy(kv => kv.Key)
        .Select(kv => PakReader.UnknownFilePathPrefix + kv.Key.ToString("X16") + "." + FileFormatExtensions.FormatToFileExtension(kv.Value)).ToArray();

    private Dictionary<ulong, KnownFileFormats> CacheUnknownFiles()
    {
        var list = new Dictionary<ulong, KnownFileFormats>();
        if (cachedEntries == null) return list;
        foreach (var e in cachedEntries)
        {
            if (e.Value.entry.path != null) continue;

            var hash = e.Value.entry.CombinedHash;
            try {
                var file = GetFile(hash);
                if (file != null)
                {
                    if (file.Length >= 4)
                    {
                        uint magic1 = 0;
                        file.Read(MemoryUtils.StructureAsBytes(ref magic1));
                        var fmt = GuessFileFormatFromMagic(magic1);
                        if (fmt != KnownFileFormats.Unknown)
                        {
                            list[hash] = fmt;
                            continue;
                        }
                    }

                    if (file.Length >= 8)
                    {
                        uint magic2 = 0;
                        file.Read(MemoryUtils.StructureAsBytes(ref magic2));
                        var fmt = GuessFileFormatFromMagic(magic2);
                        if (fmt != KnownFileFormats.Unknown)
                        {
                            list[hash] = fmt;
                            continue;
                        }
                    }

                    list[hash] = KnownFileFormats.Unknown;
                }
            }
            catch (Exception)
            {
                // ignore
            }
        }

        return list;
    }

    private readonly object _lock = new();
    private const int PakCacheTimeout = 30000;

    /// <summary>
    /// Reads a file into a MemoryStream.
    /// </summary>
    public MemoryStream? GetFile(string filepath)
    {
        var hash = PakUtils.GetFilepathHash(filepath);
        return GetFile(hash);
    }

    public int GetSize(string filepath)
    {
        if (cachedEntries?.TryGetValue(PakUtils.GetFilepathHash(filepath), out var entry) == true) {
            return (int)entry.entry.decompressedSize;
        }
        return 0;
    }

    /// <summary>
    /// Reads a file into a MemoryStream.
    /// </summary>
    public MemoryStream? GetFile(ulong filepathHash)
    {
        if (cachedEntries == null)
        {
            CacheEntries();
            if (cachedEntries == null) throw new Exception("Failed to generate cache of PAK entries");
        }

        if (cachedEntries.TryGetValue(filepathHash, out var entry))
        {
            var memoryStream = new MemoryStream();
            using var fs = File.OpenRead(entry.file.filepath);
            PakFile.ReadEntry(entry.entry, fs, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        return null;
    }

    public bool FileExists(string filepath) => FileExists(PakUtils.GetFilepathHash(filepath));
    public bool FileExists(ulong filepathHash)
    {
        if (cachedEntries == null)
        {
            CacheEntries();
            if (cachedEntries == null) throw new Exception("Failed to generate cache of PAK entries");
        }

        return cachedEntries.ContainsKey(filepathHash);
    }

    /// <summary>
    /// Reads a file into a MemoryStream using a reused file stream.<br/>
    /// Not thread safe.<br/><br/>
    /// The returned stream is transient, do not hold a permanent reference to it - copy any data you need somewhere else.
    /// </summary>
    public MemoryStream? GetFileCached(ulong filepathHash)
    {
        if (cachedEntries == null)
        {
            CacheEntries();
            if (cachedEntries == null) throw new Exception("Failed to generate cache of PAK entries");
        }

        if (cachedEntries.TryGetValue(filepathHash, out var entry))
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.SetLength(0);
            entry.file.Read(entry.entry, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        return null;
    }

    /// <summary>
    /// Scan all PAK files and cache lookup info for every file entry hash.
    /// </summary>
    public void CacheEntries(bool assignEntryPaths = false)
    {
        lock(_lock)  {
            if (cachedEntries?.Count > 0) {
                return;
            }

            cachedEntries = new();
            foreach (var pak in EnumeratePaks(assignEntryPaths))
            {
                foreach (var entry in pak.Entries)
                {
                    var hash = entry.CombinedHash;
                    if (!cachedEntries.ContainsKey(hash)) {
                        cachedEntries.Add(hash, new PakEntryCache(pak, entry));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Creates a new reader instance based on the currently cached data. Use for access to non-thread-safe operations when you need thread safety.
    /// Must call <see cref="CacheEntries"/> before to ensure a consistent state.
    /// </summary>
    public CachedMemoryPakReader Clone()
    {
        if (!(cachedEntries?.Count > 0))
        {
            throw new Exception("PAK file entry cache has not been built yet");
        }

        var reader = new CachedMemoryPakReader();
        reader.cachedEntries = cachedEntries;
        reader.PakFilePriority = PakFilePriority;
        reader.Filter = Filter;
        reader.MaxThreads = MaxThreads;
        reader.EnableConsoleLogging = EnableConsoleLogging;
        return reader;
    }

    /// <summary>
    /// Clears the current pak and entry cache.
    /// Not thread safe.
    /// </summary>
    public void Clear()
    {
        cachedEntries?.Clear();
        searchedPaths.Clear();
    }

    public void Dispose()
    {
        Clear();
        GC.SuppressFinalize(this);
    }

    private static KnownFileFormats GuessFileFormatFromMagic(uint magic)
    {
        return magic switch {
            AimapAttrFile.Magic => KnownFileFormats.AIMapAttribute,
            CcbkFile.Magic => KnownFileFormats.CharacterColliderBank,
            CdefFile.Magic => KnownFileFormats.CollisionDefinition,
            CfilFile.Magic => KnownFileFormats.CollisionFilter,
            CmatFile.Magic => KnownFileFormats.CollisionMaterial,
            CocoFile.Magic => KnownFileFormats.CompositeCollision,
            DefFile.Magic => KnownFileFormats.DynamicsDefinition,
            EfxFile.Magic => KnownFileFormats.Effect,
            FolFile.Magic => KnownFileFormats.Foliage,
            GcfFile.Magic => KnownFileFormats.GUIConfig,
            GmlFile.Magic => KnownFileFormats.GroundMaterialList,
            GrndFile.Magic => KnownFileFormats.Ground,
            GtlFile.Magic => KnownFileFormats.GroundTextureList,
            HFFile.Magic => KnownFileFormats.HeightField,
            CHFFile.Magic => KnownFileFormats.CollisionHeightField,
            JmapFile.Magic => KnownFileFormats.JointMap,
            McolFile.Magic => KnownFileFormats.CollisionMesh,
            MdfFile.Magic => KnownFileFormats.MaterialDefinition,
            MeshFile.Magic => KnownFileFormats.Mesh,
            MeshFile.MagicMply => KnownFileFormats.Mesh,
            MsgFile.Magic => KnownFileFormats.Message,
            TerrFile.Magic => KnownFileFormats.Terrain,
            TexFile.Magic => KnownFileFormats.Texture,
            UvsFile.Magic => KnownFileFormats.UVSequence,

            AimpFile.Magic => KnownFileFormats.AIMap,
            BhvtFile.Magic => KnownFileFormats.BehaviorTree, // same as FSMv2
            ClipFile.Magic => KnownFileFormats.Clip, // same as tml and ucurve
            GuiFile.Magic => KnownFileFormats.GUI,
            MotbankFile.Magic => KnownFileFormats.MotionBank,
            MotFile.Magic => KnownFileFormats.Motion,
            Motfsm2File.Magic => KnownFileFormats.MotionFsm2,
            MotlistFile.Magic => KnownFileFormats.MotionList,
            MotTreeFile.Magic => KnownFileFormats.MotionTree,
            PfbFile.Magic => KnownFileFormats.Prefab,
            RcolFile.Magic => KnownFileFormats.RequestSetCollider,
            ScnFile.Magic => KnownFileFormats.Scene,
            UserFile.Magic => KnownFileFormats.UserData,
            UVarFile.Magic => KnownFileFormats.UserVariables,


            0x00464453 => KnownFileFormats.MasterMaterial, // same as VfxShader, Shader
            0x4F4C4347 => KnownFileFormats.GpuCloth,
            0x58455452 => KnownFileFormats.RenderTexture,
            0x4D534648 => KnownFileFormats.Fsm,
            0x6D73666D => KnownFileFormats.MotionFsm,
            0x6E616863 => KnownFileFormats.Chain,
            0x326E6863 => KnownFileFormats.Chain2,
            0x4252504E => KnownFileFormats.LightProbes,
            0x59444F42 => KnownFileFormats.RigidBodySet, // same as Ragdoll
            0x54464453 => KnownFileFormats.SDFTexture,
            0x204D4252 => KnownFileFormats.RigidBodyMesh,
            0x5443504D => KnownFileFormats.MaterialPointCloud,
            0x736C6375 => KnownFileFormats.UserCurveList,
            0x67727472 => KnownFileFormats.RetargetRig,
            0x736E636A => KnownFileFormats.JointConstraints,
            0x7E7C6B73 => KnownFileFormats.Skeleton,
            0x3267656C => KnownFileFormats.IkLeg2,
            0x326C6B69 => KnownFileFormats.IkLookAt2,
            0x736C6B69 => KnownFileFormats.IkLegSpine,
            0x73626B69 => KnownFileFormats.IkBodyRig,
            0x64686B69 => KnownFileFormats.IkHand2,
            0x6C6D6B69 => KnownFileFormats.IkMultiBone,
            0x32746B69 => KnownFileFormats.IkTrain2,
            0x61646B69 => KnownFileFormats.IkDamageAction,
            0x6B696266 => KnownFileFormats.FullBodyIKRig,
            0x64637273 => KnownFileFormats.VibrationSource,
            0x6E6C6B73 => KnownFileFormats.FbxSkeleton,
            0x4D534C43 => KnownFileFormats.CollisionSkinningMesh,
            0x50534C43 => KnownFileFormats.CollisionShapePreset,
            0x50415257 => KnownFileFormats.WrapDeformer,
            0x52554653 => KnownFileFormats.ShellFur,
            0x464E4946 => KnownFileFormats.FilterInfo,
            0x52504347 => KnownFileFormats.GUIColorPreset,
            0x59545347 => KnownFileFormats.GUIStyleList,
            0x00535353 => KnownFileFormats.SSSProfile,
            0x00444F4C => KnownFileFormats.Lod,
            0x384D5453 => KnownFileFormats.SpeedTreeMesh,
            0x4854444D => KnownFileFormats.AlembicMesh,
            0x6D61636D => KnownFileFormats.MotionCamera,
            0x74736C63 => KnownFileFormats.MotionCameraList,
            0x6B6E6263 => KnownFileFormats.MotionCameraBank,
            0x32736674 => KnownFileFormats.TimelineFsm2,
            0x4F464246 => KnownFileFormats.OutlineFont,
            0x544C5346 => KnownFileFormats.FontSlot,
            0x544E4649 => KnownFileFormats.IconFont,
            0x4F49434F => KnownFileFormats.OpenColorIOConfig,
            0x52415453 => KnownFileFormats.StarCatalogue,
            0x70797466 => KnownFileFormats.Movie,
            0x44484B42 => KnownFileFormats.SoundBank,
            0x4B504B41 => KnownFileFormats.SoundPackage,
            0x494C5356 => KnownFileFormats.VfxShaderList,
            0x7261666C => KnownFileFormats.Lensflare,
            0x726D6565 => KnownFileFormats.EffectEmitMask,
            0x00534549 => KnownFileFormats.IESLight,
            0x67636C6A => KnownFileFormats.JointLODGroup,
            0x45445046 => KnownFileFormats.FreePolygon,
            0x504F5350 => KnownFileFormats.PSOPatch,
            0x21545353 => KnownFileFormats.SetStateList,
            0x54435846 => KnownFileFormats.EffectCollision,

            // TODO
            // 0x00000000 => KnownFileFormats.GpuMotionList,
            // 0x00006B69 => KnownFileFormats.IkTrain,
            // 0x00006B69 => KnownFileFormats.IkLizard,
            // 0x00006B69 => KnownFileFormats.IkWagon,
            // 0x00000000 => KnownFileFormats.RagdollController,
            // 0x00000000 => KnownFileFormats.RebeConfig,
            // 0x00000000 => KnownFileFormats.TimelineBlend,
            // 0x00000000 => KnownFileFormats.JointExpressionGraph,
            // 0x00000000 => KnownFileFormats.MeshVertexMapping,
            // 0x00000000 => KnownFileFormats.ZivaRT,
            // 0x00000000 => KnownFileFormats.ZivaCombiner,
            // 0x00000000 => KnownFileFormats.Dialogue,
            // 0x00000000 => KnownFileFormats.DialogueConfig,
            // 0x00000000 => KnownFileFormats.DialogueList,
            // 0x00000000 => KnownFileFormats.DialogueTimeline,
            // 0x00000000 => KnownFileFormats.DialogueTimelineList,
            // 0x00000000 => KnownFileFormats.ClothResetPose,
            // 0x00000000 => KnownFileFormats.ColliderSet,
            // 0x00000000 => KnownFileFormats.Strands,
            // 0x00000000 => KnownFileFormats.HeightTexture,
            // 0x00000000 => KnownFileFormats.ChainWind,
            // 0x00000000 => KnownFileFormats.PointGraph,
            // 0x00000000 => KnownFileFormats.PointGraphList,
            // 0x00000000 => KnownFileFormats.CfxShader,
            // 0x00000000 => KnownFileFormats.OodleNetworkRuntimeData,

            // no usable magic bytes
            // ByteBuffer,
            // Probes,
            // AudioMixer,
            // RayTraceMaterialRedirect,
            // MemorySettings,
            // EffectCsv,
            // NetworkConfig,
            // RenderConfigAsset,

            _ => KnownFileFormats.Unknown,
        };
    }
}
