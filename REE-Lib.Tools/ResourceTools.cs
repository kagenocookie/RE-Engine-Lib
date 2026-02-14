
namespace ReeLib.Tools;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using ReeLib.Common;
using ReeLib.Data;
using ReeLib.Il2cpp;

public static class FileExtensionTools
{
    public static void ExtractAllFileExtensionCacheData(IEnumerable<GameIdentifier> games, Func<GameIdentifier, string?>? listFileProvider = null)
    {
        Dictionary<string, FileExtensionCache> dict = new();
        foreach (var item in games) {
            if (item.hash == 0) continue;

            Log.Info("Handling file extensions for game " + item);
            var config = GameConfig.CreateFromRepository(item.ToString());
            var listOverride = listFileProvider?.Invoke(item);
            if (listOverride != null && File.Exists(listOverride)) {
                config.Resources.LocalPaths.FileList = listOverride;
            }
            var env = new Workspace(config);
            if (env.ListFile == null) {
                Log.Info("No file list available for game " + item);
                continue;
            }
            var cache = env.GenerateFileExtensionCache();
            if (cache != null) {
                dict[item.ToString()] = cache;
                // var path = $"output/{item.name}/file_extensions.json";
                // Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                // using var singleFs = File.Create(path);
                // JsonSerializer.Serialize(singleFs, dict, jsonOptions);
                Log.Info($"File extension cache for {item.name} successfully generated");
            }
        }

        var cacheFilepath = Path.GetFullPath("output/file_extensions.json");
        Directory.CreateDirectory(Path.GetDirectoryName(cacheFilepath)!);
        using var fs = File.Create(cacheFilepath);
        JsonSerializer.Serialize(fs, dict, jsonOptions);
        Log.Info($"Combined file extension cache successfully generated to {cacheFilepath}");
    }

    private static readonly JsonSerializerOptions jsonOptions = new() {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
    };
}

public class ResourceTools(Workspace workspace)
{
    /// <summary>
    /// The folder to which to output the generated data.
    /// </summary>
    public string BaseOutputPath { get; set; } = $"{workspace.Config.Game.name}";

    private Dictionary<string, RszClassPatch> rszTypePatches = new();

    public static readonly JsonSerializerOptions jsonOptions = new() {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
    };

    /// <summary>
    /// Attempts to read every file that uses RSZ data to determine their real field types.
    /// Uses the already configured RszPatchFiles from the workspace as a base.
    /// Output is written to {game}/rsz_patch.json
    /// </summary>
    public void InferRszData()
    {
        _ = workspace.RszParser; // ensure the parser is setup and rsz patches are loaded
        foreach (var p in workspace.Config.RszPatchFiles.Where(x => !string.IsNullOrEmpty(x)).Skip(1)) {
            var patches = JsonSerializer.Deserialize<Dictionary<string, RszClassPatch>>(File.ReadAllText(p));
            if (patches == null) continue;

            foreach (var (k, v) in patches) {
                if (rszTypePatches.TryGetValue(k, out var patch)) {
                    patch.FieldPatches = (patch.FieldPatches ?? []).Concat(v.FieldPatches ?? []).ToArray();
                } else {
                    rszTypePatches[k] = v;
                }
            }
        }
        // workspace.RszParser.ApplyPatches(rszTypePatches);

        DeduplicateFieldNames(workspace.RszParser.ClassDict.Values);
        var resourceFinder = new ResourceFieldFinder(workspace, this);
        var dupeHandler = new DuplicateInstanceRefHandler(workspace);

        static (int succ, int fail) HandleFiles(Workspace workspace, string extension, Action<string, Stream> callback)
        {
            int success = 0;
            int fails = 0;
            foreach (var (path, stream) in workspace.GetFilesWithExtension(extension)) {
                try {
                    callback.Invoke(path, stream);
                    if (++success % 100 == 0) {
                        Log.Info($"Handled {success} files...");
                    }
                } catch (Exception e) {
                    fails++;
                    Log.Error($"Failed to handle file {path}: {e.Message}");
                }
            }
            Log.Info($"Finished {success} {extension} files with {fails} failures");
            return (success, fails);
        }

        Log.Info("Handling scn files ...");
        HandleFiles(workspace, "scn", (path, stream) => {
            var file = new ScnFile(workspace.RszFileOption, new FileHandler(stream, path));
            file.Read();
            dupeHandler.FindDuplicateRszObjectInstances(file.RSZ, path);
            resourceFinder.CheckInstances(file.RSZ, file.ResourceInfoList);
        });

        resourceFinder.ApplyRszPatches();
        StoreInferredRszTypes(workspace.RszParser.ClassDict.Values);

        Log.Info("Handling pfb files ...");
        HandleFiles(workspace, "pfb", (path, stream) => {
            var file = new PfbFile(workspace.RszFileOption, new FileHandler(stream, path));
            file.Read();
            dupeHandler.FindDuplicateRszObjectInstances(file.RSZ, path);
            resourceFinder.CheckInstances(file.RSZ, file.ResourceInfoList);
            CheckGameObjectRefInstances(file);
        });
        resourceFinder.ApplyRszPatches();
        StoreInferredRszTypes(workspace.RszParser.ClassDict.Values);

        Log.Info("Handling user files ...");
        HandleFiles(workspace, "user", (path, stream) => {
            var file = new UserFile(workspace.RszFileOption, new FileHandler(stream, path));
            file.Read();
            dupeHandler.FindDuplicateRszObjectInstances(file.RSZ, path);
            resourceFinder.CheckInstances(file.RSZ, file.ResourceInfoList);
        });
        resourceFinder.ApplyRszPatches();
        StoreInferredRszTypes(workspace.RszParser.ClassDict.Values);

        Log.Info("Handling rcol files ...");
        HandleFiles(workspace, "rcol", (path, stream) => {
            var file = new RcolFile(workspace.RszFileOption, new FileHandler(stream, path));
            file.Read();
            dupeHandler.FindDuplicateRszObjectInstances(file.RSZ, path);
        });
        StoreInferredRszTypes(workspace.RszParser.ClassDict.Values);
    }

    private void CheckGameObjectRefInstances(PfbFile pfb)
    {
        var game = workspace.Config.Game;
        foreach (var refinfo in pfb.GameObjectRefInfoList) {
            var src = pfb.RSZ.ObjectList[(int)refinfo.objectId];
            var refFields = src.RszClass.fields.Where(f => f.IsGameObjectRef).ToArray();

            var propInfoDict = workspace.PfbRefProps.GetValueOrDefault(src.RszClass.name);
            if (propInfoDict?.Count == refFields.Length) {
                // already resolved
                continue;
            }

            var refValues = pfb.GameObjectRefInfoList.Where(rf =>
                rf.objectId == src.ObjectTableIndex &&
                (rf.arrayIndex == 0 || 1 == pfb.GameObjectRefInfoList.Count(rf2 => rf2.objectId == src.ObjectTableIndex))
            ).OrderBy(b => b.propertyId);

            if (refFields.Length == refValues.Count()) {
                propInfoDict = new();
                int i = 0;
                foreach (var propId in refValues.Select(r => r.propertyId)) {
                    var refField = refFields[i++];
                    var prop = new PrefabGameObjectRefProperty() { PropertyId = propId, AutoDetected = true };
                    propInfoDict[refField.name] = prop;
                    workspace.PfbRefProps[src.RszClass.name] = propInfoDict;
                    Log.Info($"Auto-detected GameObjectRef property {src.RszClass.name} {refField.name} as propertyId {propId}.");
                }

                var fn = $"{BaseOutputPath}/pfb_ref_props.json";
                using var fs = File.Create(fn);
                JsonSerializer.Serialize<Dictionary<string, Dictionary<string, PrefabGameObjectRefProperty>>>(fs, workspace.PfbRefProps, jsonOptions);
            } else {
                Log.Error($"Failed to resolve all GameObjectRef properties in class {src.RszClass.name}, file: {pfb.FileHandler.FilePath}.");
            }
        }
    }

    private void DeduplicateFieldNames(IEnumerable<RszClass> classlist)
    {
        var dupeDict = new Dictionary<string, int>();
        RszClassPatch? patch;
        foreach (var cls in classlist) {
            dupeDict.Clear();
            patch = null;
            foreach (var f in cls.fields) {
                if (dupeDict.TryGetValue(f.name, out var count)) {
                    patch ??= FindOrCreateClassPatch(cls.name);
                    if (count == 1) {
                        var prev = cls.fields.First(p => p.name == f.name);
                        var entryPrev = new RszFieldPatch() { Name = f.name, ReplaceName = f.name + "1" };
                        prev.name = entryPrev.ReplaceName;
                        patch.FieldPatches = patch.FieldPatches == null ? [entryPrev] : patch.FieldPatches.Append(entryPrev).ToArray();
                    }
                    var nameOverride = f.name + (dupeDict[f.name] = ++count);
                    var entry = new RszFieldPatch() { Name = f.name, ReplaceName = nameOverride };
                    patch.FieldPatches = patch.FieldPatches == null ? [entry] : patch.FieldPatches.Append(entry).ToArray();
                    f.name = nameOverride;
                } else {
                    dupeDict[f.name] = 1;
                }
            }
        }
    }

    private void StoreInferredRszTypes(RSZFile rsz)
    {
        var handled = new HashSet<RszClass>();
        foreach (var inst in rsz.InstanceList) {
            handled.Add(inst.RszClass);
        }

        StoreInferredRszTypes(handled);
    }

    internal bool TryFindClassPatch(string classname, [MaybeNullWhen(false)] out RszClassPatch patch) => rszTypePatches.TryGetValue(classname, out patch);
    internal RszClassPatch FindOrCreateClassPatch(string classname)
    {
        if (!rszTypePatches.TryGetValue(classname, out var patch)) {
            rszTypePatches[classname] = patch = new();
        }
        return patch;
    }

    private void StoreInferredRszTypes(IEnumerable<RszClass> classlist)
    {
        int changes = 0;
        foreach (var cls in classlist) {

            foreach (var f in cls.fields) {
                if (!f.IsTypeInferred) continue;
                if (f.type <= RszFieldType.Undefined) {
                    continue;
                }
                var props = FindOrCreateClassPatch(cls.name);

                var entry = props.FieldPatches?.FirstOrDefault(patch => patch.Name == f.name || patch.ReplaceName == f.name);
                if (entry == null) {
                    entry = new RszFieldPatch() { Name = f.name, Type = f.type };
                    props.FieldPatches = props.FieldPatches == null ? [entry] : props.FieldPatches.Append(entry).ToArray();
                    changes++;
                } else if (entry.Type != f.type) {
                    entry.Type = f.type;
                    changes++;
                }
            }
        }

        SaveRszPatchFile();
        Log.Info($"Updating RSZ inferred field type cache with {changes} changes");
    }

    internal void SaveRszPatchFile()
    {
        var path = Path.GetFullPath($"{BaseOutputPath}/rsz_patch.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        using var file = File.Create(path);
        JsonSerializer.Serialize(file, rszTypePatches, jsonOptions);
    }
}

internal sealed class ResourceFieldFinder(Workspace env, ResourceTools resourceTools) : IDisposable
{
    private ResourceList list = new();

    private sealed class ResourceList
    {
        public HashSet<(string cls, string field, string ext, bool? shouldBeInResourceList)> resources = new();
        public HashSet<(string cls, string field)> nonResources = new();
    }

    public void ApplyRszPatches()
    {
        MarkRSZResourceFields(list.resources
            .Where(item => !list.nonResources.Contains((item.cls, item.field)))
            .Select(item => (item.cls, item.field, item.ext)));
    }

    private void MarkRSZResourceFields(IEnumerable<(string classname, string field, string resourceExtension)> fields)
    {
        var resourceHolders = new Dictionary<KnownFileFormats, string>();
        foreach (var cls in env.RszParser.ClassDict.Values) {
            if (cls.name.EndsWith("Holder")) {
                var format = TypeCache.GetResourceFormat(cls.name);
                if (format != KnownFileFormats.Unknown) {
                    resourceHolders[format] = cls.name;
                }
            }
        }
        int changes = 0;
        foreach (var (cls, field, ext) in fields) {
            var typeinfo = env.RszParser.GetRSZClass(cls);
            var rszField = typeinfo?.fields.FirstOrDefault(f => f.name == field);
            if (rszField != null) {
                if (rszField.type is not RszFieldType.String and not RszFieldType.Resource) {
                    Log.Error($"Attempted to change non-string field into resource: {cls} field {field} of type {rszField.type}");
                    continue;
                }
                var fileFormat = PathUtils.GetFileFormatFromExtension(ext);

                resourceTools.TryFindClassPatch(cls, out var patch);
                var fieldPatch = patch?.FieldPatches?.FirstOrDefault(f => f.Name == field);
                // var shouldAdd = rszField.type == RszFieldType.String ||
                //     fileFormat != KnownFileFormats.Unknown && fileFormat != fieldPatch?.FileFormat;
                var shouldAdd = true;

                if (shouldAdd) {
                    patch ??= resourceTools.FindOrCreateClassPatch(cls);
                    if (fieldPatch == null) {
                        fieldPatch = new RszFieldPatch() { Name = field };
                        patch.AddFieldPatch(fieldPatch);
                    }
                    if (fileFormat == KnownFileFormats.Unknown) fileFormat = fieldPatch.FileFormat;
                    if (fieldPatch.FileFormat != KnownFileFormats.Unknown && fieldPatch.FileFormat != fileFormat) {
                        // handle conflicts
                        static bool IsTextureSubtype(KnownFileFormats format) => format is KnownFileFormats.Texture or KnownFileFormats.RenderTexture;
                        static bool IsTimelineSubtype(KnownFileFormats format) => format is KnownFileFormats.Timeline or KnownFileFormats.Clip or KnownFileFormats.TimelineBase;
                        static bool IsMotionSubtype(KnownFileFormats format) => format is KnownFileFormats.Motion or KnownFileFormats.MotionList or KnownFileFormats.GpuMotionList or ReeLib.KnownFileFormats.MotionCamera or KnownFileFormats.MotionCameraList or KnownFileFormats.MotionBase;
                        static bool IsDynamicsSubtype(KnownFileFormats format) => format is KnownFileFormats.HeightField or KnownFileFormats.RigidBodyMesh or KnownFileFormats.DynamicsBase;
                        static bool IsSkeletonSubtype(KnownFileFormats format) => format is KnownFileFormats.Skeleton or KnownFileFormats.FbxSkeleton;
                        static bool IsBehaviorTreeSubtype(KnownFileFormats format) => format is KnownFileFormats.BehaviorTree or KnownFileFormats.Fsm2 or KnownFileFormats.BehaviorTreeBase;

                        if (IsTextureSubtype(fileFormat) && IsTextureSubtype(fieldPatch.FileFormat)) {
                            fileFormat = KnownFileFormats.Texture;
                        } else if (IsTimelineSubtype(fileFormat) && IsTimelineSubtype(fieldPatch.FileFormat)) {
                            fileFormat = KnownFileFormats.TimelineBase;
                        } else if (IsMotionSubtype(fileFormat) && IsMotionSubtype(fieldPatch.FileFormat)) {
                            fileFormat = KnownFileFormats.MotionBase;
                        } else if (IsDynamicsSubtype(fileFormat) && IsDynamicsSubtype(fieldPatch.FileFormat)) {
                            fileFormat = KnownFileFormats.DynamicsBase;
                        } else if (IsSkeletonSubtype(fileFormat) && IsSkeletonSubtype(fieldPatch.FileFormat)) {
                            fileFormat = KnownFileFormats.Skeleton;
                        } else if (IsBehaviorTreeSubtype(fileFormat) && IsBehaviorTreeSubtype(fieldPatch.FileFormat)) {
                            fileFormat = KnownFileFormats.BehaviorTreeBase;
                        } else {
                            Log.Error($"Warning: Resource type conflict on field {cls} {field}: {fieldPatch.FileFormat} and {fileFormat}. Manually verify please.");
                        }
                    }
                    fieldPatch.FileFormat = fileFormat;
                    if (fileFormat is KnownFileFormats.Scene or KnownFileFormats.Prefab) {
                        // leave these as string
                        fieldPatch.Type = RszFieldType.String;
                    } else {
                        fieldPatch.Type = RszFieldType.Resource;
                        if (fileFormat != KnownFileFormats.Unknown && (string.IsNullOrEmpty(rszField.original_type) || rszField.original_type == "via.resource_handle")) {
                            if (resourceHolders.TryGetValue(fileFormat, out var holderClassname)) {
                                fieldPatch.OriginalType = holderClassname;
                            } else {
                                Log.Error($"Failed to map resource file format {fileFormat} to classname");
                            }
                        }
                    }
                    changes++;
                }
            }
        }
    }

    public void CheckInstances(RSZFile rszFile, List<ResourceInfo> resourceList)
    {
        foreach (var inst in rszFile.InstanceList) {
            if (inst != null && inst.HasValues) {
                FindResourceFields(resourceList, inst, list);
            }
        }
    }

    private void FindResourceFields(List<ResourceInfo> resourceList, RszInstance instance, ResourceList resourceFields)
    {
        for (var i = 0; i < instance.RszClass.fields.Length; i++) {
            var field = instance.RszClass.fields[i];
            if (field.type is RszFieldType.String or RszFieldType.Resource) {
                if (instance.Values[i] is string value) {
                    CheckStringForResource(resourceList, instance, resourceFields, field, value);
                } else if (instance.Values[i] is List<object> list && list.FirstOrDefault() is string) {
                    foreach (var element in list.OfType<string>()) {
                        if (CheckStringForResource(resourceList, instance, resourceFields, field, element)) {
                            break;
                        }
                    }
                }
            }
        }
    }

    private static bool CheckStringForResource(List<ResourceInfo> resourceList, RszInstance instance, ResourceList resourceFields, RszField field, string? value)
    {
        if (string.IsNullOrEmpty(value) || value.EndsWith(".json")) return false; // .json == re7/re2/dmc5 userdata json paths, not resources
        var cls = instance.RszClass.name;
        if (resourceFields.nonResources.Contains((cls, field.name))) return false;

        var seemsLikePath = RszUtils.IsResourcePath(value);
        if (!seemsLikePath) {
            // there are fields that can seem like paths (e.g. developer comments) but turns out aren't, this will cross those out
            resourceFields.nonResources.Add((cls, field.name));
            return false;
        }

        var isInResourceList = string.IsNullOrEmpty(value) ? (bool?)null : resourceList.Any(r => r.Path?.Equals(value, StringComparison.OrdinalIgnoreCase) == true);

        // better not store these as resources, so we don't require all folders and subfolders to always be imported
        if (cls == "via.Folder") return false;

        var ext = Path.GetExtension(value);
        if (string.IsNullOrEmpty(ext)) ext = string.Empty;
        else ext = ext.Substring(1);
        resourceFields.resources.Add((cls, field.name, ext, isInResourceList));

        return true;
    }

    public void Dispose()
    {
    }
}

internal sealed class DuplicateInstanceRefHandler(Workspace env)
{
    private static Dictionary<GameName, Dictionary<string, HashSet<string>>> whitelistedDuplicates = new () {
        { GameName.dmc5, new Dictionary<string, HashSet<string>>() {
            { "app.MaterialChangeController.MaterialInfo", ["Materials"] },
        } },
        { GameName.re2, new Dictionary<string, HashSet<string>>() {
            { "app.ropeway.environment.EnvironmentBoundaryClassifier", ["MyselfMap", "TargetMap"] },
            { "app.ropeway.posteffect.param.ColorCorrect", ["LinearParamsAt"] },
        } },
        { GameName.re8, new Dictionary<string, HashSet<string>>() {
            { "app.FsmStateManager.FsmStateData", ["StateID"] },
        } }
    };

    private HashSet<string>? components = new();

    public void FindDuplicateRszObjectInstances(RSZFile file, string filepath)
    {
        var game = env.Config.Game.GameEnum;
        components ??= env.TypeCache.GetSubclasses("via.Component").ToHashSet();
        var instances = new Dictionary<RszInstance, RszInstance>();
        foreach (var instance in file.InstanceList) {
            if (!instance.HasValues) {
                if (instance.RSZUserData is RSZUserDataInfo_TDB_LE_67 embed && embed.EmbeddedRSZ != null) {
                    FindDuplicateRszObjectInstances(embed.EmbeddedRSZ, filepath + " (Embedded RSZ)");
                }
                continue;
            }

            for (var fieldIndex = 0; fieldIndex < instance.RszClass.fields.Length; fieldIndex++) {
                var field = instance.RszClass.fields[fieldIndex];
                if (field.type == RszFieldType.Object) {
                    var value = instance.Values[fieldIndex];
                    var values = field.array ? ((List<object>)value).OfType<RszInstance>().ToArray() : [(RszInstance)value];
                    for (var i = 0; i < values.Length; i++) {
                        var val = values[i];
                        if (components.Contains(val.RszClass.name)) {
                            Log.Error($"Found direct reference to component - this is almost definitely wrong! Object {instance} field {fieldIndex} {field.name} referenced {val}");
                            Log.Error($"Filepath: {filepath}");
                            Log.Error($"Adding to the {instance.RszClass.name} patch list: {{ \"Name\": \"{field.name}\", \"Type\": \"S32\" }}");
                            field.type = RszFieldType.S32;
                            field.IsTypeInferred = true;
                            continue;
                        }
                        if (val.Index != 0 && !instances.TryAdd(val, instance)) {
                            var isWhitelisted = whitelistedDuplicates.GetValueOrDefault(game)?.GetValueOrDefault(instance.RszClass.name)?.Contains(field.name);
                            if (isWhitelisted != true) {
                                var valueIndex = field.array ? $"[{i}]" : "";
                                Log.Error($"Found duplicate rsz instance reference - likely read error, verify correctness please.\nObject {instance} field {fieldIndex} {field.name}{valueIndex}: value {val} previously referenced from {instances[val]}.");
                                Log.Error($"Filepath: {filepath}");
                                Log.Error("If the reference is correct, add it to the whitelist");
                                Log.Error("If the reference is not correct, add or modify the field patch list of the class in the rsz patch JSON:");
                                Log.Error($"{{\n  \"Name\": \"{field.name}\",\n  \"Type\": \"S32\"\n}}");
                            }
                        }
                    }
                }
            }
        }
    }
}
