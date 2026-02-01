using System.Collections.ObjectModel;
using ReeLib.Common;
using ReeLib.Data;
using ReeLib.Pfb;
using ReeLib.via;

namespace ReeLib.Pfb
{
    public class PfbHeaderStruct : BaseModel {
        public uint magic;
        public int infoCount;
        public int resourceCount;
        public int gameObjectRefInfoCount;
        public long userdataCount;
        public long gameObjectRefInfoOffset;
        public long resourceInfoOffset;
        public long userdataInfoOffset;
        public long dataOffset;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref magic);
            handler.Read(ref infoCount);
            handler.Read(ref resourceCount);
            handler.Read(ref gameObjectRefInfoCount);
            if (handler.FileVersion > 16) handler.Read(ref userdataCount);
            handler.Read(ref gameObjectRefInfoOffset);
            handler.Read(ref resourceInfoOffset);
            if (handler.FileVersion > 16) handler.Read(ref userdataInfoOffset);
            handler.Read(ref dataOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref magic);
            handler.Write(ref infoCount);
            handler.Write(ref resourceCount);
            handler.Write(ref gameObjectRefInfoCount);
            if (handler.FileVersion > 16) handler.Write(ref userdataCount);
            handler.Write(ref gameObjectRefInfoOffset);
            handler.Write(ref resourceInfoOffset);
            if (handler.FileVersion > 16) handler.Write(ref userdataInfoOffset);
            handler.Write(ref dataOffset);
            return true;
        }
    }

    public struct PfbGameObjectInfo {
        public int objectId;
        public int parentId;
        public int componentCount;
    }

    public struct PfbGameObjectRefInfo {
        public int objectId;
        public int propertyId;
        public int arrayIndex;
        public int targetId;

        public override string ToString() => $"<{objectId}>.{propertyId}[{arrayIndex}] = {targetId}";
    }

    public class PfbGameObject : IGameObject
    {
        private WeakReference<PfbGameObject>? parentRef;
        public PfbGameObjectInfo Info;
        public ObservableCollection<RszInstance> Components { get; private set; } = new();
        public ObservableCollection<PfbGameObject> Children { get; private set; } = new();
        public RszInstance? Instance { get; set; }

        IList<RszInstance> IGameObject.Components => Components;

        /// <summary>
        /// 从ScnFile.GameObject生成GameObject
        /// </summary>
        /// <param name="scnGameObject"></param>
        /// <returns></returns>
        public static PfbGameObject FromScnGameObject(Scn.ScnGameObject scnGameObject)
        {
            PfbGameObject gameObject = new()
            {
                Info = new()
                {
                    objectId = -1,
                    parentId = -1,
                    componentCount = scnGameObject.Components.Count,
                },
                Components = new(scnGameObject.Components.Select(item => item.CloneCached())),
                Instance = scnGameObject.Instance?.CloneCached()
            };
            foreach (var child in scnGameObject.Children)
            {
                var newChild = FromScnGameObject(child);
                newChild.Parent = gameObject;
                gameObject.Children.Add(newChild);
            }
            RszInstance.CleanCloneCache();
            return gameObject;
        }

        public PfbGameObject? Parent
        {
            get => parentRef?.GetTarget();
            set => parentRef = value != null ? new(value) : null;
        }

        public string? Name => Instance?.Values[0] as string;

        public int? ObjectId => Info.objectId;

        public string? GetHierarchyPath()
        {
            string? path = Name;
            var nextChild = this;
            while (nextChild.Parent != null && nextChild.Parent != this) {
                path = $"{nextChild.Parent.Name}.{path}";
                nextChild = nextChild.Parent;
            }

            return path;
        }

        public PfbGameObject Clone()
        {
            PfbGameObject gameObject = new()
            {
                Info = Info,
                Components = new(Components.Select(item => item.CloneCached())),
                Instance = Instance?.CloneCached(),
            };
            foreach (var child in Children)
            {
                var newChild = (PfbGameObject)child.Clone();
                newChild.Parent = gameObject;
                gameObject.Children.Add(newChild);
            }
            return gameObject;
        }

        public override string ToString()
        {
            return Name ?? "";
        }

        public IEnumerable<IGameObject> GetChildren() => Children;
    }
}


namespace ReeLib
{
    public class PfbFile : BaseRszFile
    {
        public PfbHeaderStruct Header { get; } = new();
        public List<PfbGameObjectInfo> GameObjectInfoList { get; } = new();
        public List<PfbGameObjectRefInfo> GameObjectRefInfoList { get; } = new();
        public List<ResourceInfo> ResourceInfoList { get; } = new();
        public List<UserdataInfo> UserdataInfoList { get; } = new();
        public RSZFile RSZ { get; }
        public ObservableCollection<PfbGameObject> GameObjects { get; } = new();
        public bool ResourceChanged { get; set; } = false;
        public PfbGameObject? Root => GameObjects.FirstOrDefault();

        public PfbFile(RszFileOption option, FileHandler fileHandler) : base(option, fileHandler)
        {
            RSZ = new RSZFile(option, fileHandler);
        }

        public const uint Magic = 0x424650;

        public override RSZFile? GetRSZ() => RSZ;

        protected override bool DoRead()
        {
            GameObjectInfoList.Clear();
            GameObjectRefInfoList.Clear();
            ResourceInfoList.Clear();
            UserdataInfoList.Clear();
            GameObjects?.Clear();

            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a PFB file");
            }

            GameObjectInfoList.ReadStructList(handler, header.infoCount);

            handler.Seek(header.gameObjectRefInfoOffset);
            GameObjectRefInfoList.ReadStructList(handler, header.gameObjectRefInfoCount);

            handler.Seek(header.resourceInfoOffset);
            // ResourceInfoList.Read(handler, header.resourceCount);
            for (int i = 0; i < header.resourceCount; i++)
            {
                ResourceInfo item = new(Option.Version, true);
                if (!item.Read(handler)) return false;
                ResourceInfoList.Add(item);
            }

            handler.Seek(header.userdataInfoOffset);
            if (handler.FileVersion > 16)
            {
                UserdataInfoList.Read(handler, (int)header.userdataCount);
            }

            ReadRsz(RSZ, header.dataOffset);
            return true;
        }

        protected override bool DoWrite()
        {
            if (StructChanged)
            {
                RebuildInfoTables();
            }
            else if (ResourceChanged)
            {
                RszUtils.SyncResourceFromRsz(ResourceInfoList, RSZ);
                ResourceChanged = false;
            }
            FileHandler handler = FileHandler;
            handler.Clear();
            var header = Header;
            if (header.Size == 0) {
                header.Write(handler);
            } else {
                handler.Seek(header.Size);
            }
            GameObjectInfoList.Write(handler);

            if (GameObjectRefInfoList.Count > 0)
            {
                // handler.Align(16);
                header.gameObjectRefInfoOffset = handler.Tell();
                GameObjectRefInfoList.Write(handler);
            }

            handler.Align(16);
            header.resourceInfoOffset = handler.Tell();
            // ResourceInfoList.Write(handler);
            foreach (var item in ResourceInfoList)
            {
                if (handler.FileVersion <= 16) item.HasOffset = false;
                if (!item.Write(handler)) return false;
            }

            if (handler.FileVersion > 16 && UserdataInfoList.Count > 0)
            {
                handler.Align(16);
                header.userdataInfoOffset = handler.Tell();
                UserdataInfoList.Write(handler);
            }

            handler.StringTableFlush();

            handler.Align(16);
            header.dataOffset = handler.Tell();
            WriteRsz(RSZ, header.dataOffset);

            header.magic = Magic;
            header.infoCount = GameObjectInfoList.Count;
            header.resourceCount = ResourceInfoList.Count;
            header.gameObjectRefInfoCount = GameObjectRefInfoList.Count;
            header.userdataCount = UserdataInfoList.Count;
            header.Write(handler, 0);

            return true;
        }

        /// <summary>
        /// Go through the read game object data and set up the game object hierarchy and references.
        /// </summary>
        public void SetupGameObjects()
        {
            Dictionary<int, PfbGameObject> gameObjectMap = new();
            foreach (var info in GameObjectInfoList)
            {
                PfbGameObject gameObjectData = new()
                {
                    Info = info,
                    Instance = RSZ.ObjectList[info.objectId],
                };
                for (int i = 0; i < info.componentCount; i++)
                {
                    gameObjectData.Components.Add(RSZ.ObjectList[info.objectId + 1 + i]);
                }
                gameObjectMap[info.objectId] = gameObjectData;
                if (info.parentId == -1)
                {
                    GameObjects.Add(gameObjectData);
                }
            }

            // add children and set parent
            foreach (var info in GameObjectInfoList)
            {
                var gameObject = gameObjectMap[info.objectId];
                if (gameObjectMap.TryGetValue(info.parentId, out var parent))
                {
                    parent.Children.Add(gameObject);
                    gameObject.Parent = parent;
                }
            }

            var props = RszParser.PrefabGameObjectRefProps;
            if (props == null) return;

            foreach (var pfbref in GameObjectRefInfoList) {
                var src = RSZ.ObjectList[pfbref.objectId];
                if (!props.TryGetValue(src.RszClass.name, out var reflist)) {
                    Log.Error($"Could not resolve prefab game object references for class {src.RszClass.name}");
                    continue;
                }
                var target = RSZ.ObjectList[pfbref.targetId];
                var targetGo = IterGameObjects(null, true).First(cf => cf.Instance!.ObjectTableIndex == target.ObjectTableIndex);
                var found = false;
                for (int i = 0; i < src.Fields.Length; i++) {
                    var field = src.Fields[i];
                    if (reflist.TryGetValue(field.name, out var refdata) && refdata.PropertyId == pfbref.propertyId) {
                        found = true;
                        if (field.array) {
                            var list = (List<object>)src.Values[i];
                            ((GameObjectRef)list[pfbref.arrayIndex]).target = targetGo;
                        } else {
                            ((GameObjectRef)src.Values[i]).target = targetGo;
                        }
                        break;
                    }
                }
                if (!found) {
                    Log.Error($"Could not resolve prefab game object reference {pfbref}");
                }
            }
        }

        /// <summary>
        /// 收集GameObject以及子物体的实例和组件实例
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="rszInstances"></param>
        public static void CollectGameObjectInstances(PfbGameObject gameObject, List<RszInstance> rszInstances)
        {
            rszInstances.Add(gameObject.Instance!);
            foreach (var item in gameObject.Components)
            {
                rszInstances.Add(item);
            }
            foreach (var child in gameObject.Children)
            {
                CollectGameObjectInstances(child, rszInstances);
            }
        }

        /// <summary>
        /// 迭代GameObject以及子物体的实例和组件实例
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static IEnumerable<RszInstance> IterGameObjectInstances(PfbGameObject gameObject)
        {
            yield return gameObject.Instance!;
            foreach (var item in gameObject.Components)
            {
                yield return item;
            }
            foreach (var child in gameObject.Children)
            {
                foreach (var item in IterGameObjectInstances(child))
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<PfbGameObject> IterGameObjects(PfbGameObject? parent = null, bool includeChildren = false)
        {
            var items = parent?.Children ?? GameObjects;
            if (items == null)
            {
                yield break;
            }
            foreach (var item in items)
            {
                yield return item;
                if (includeChildren)
                {
                    foreach (var child in IterGameObjects(item, includeChildren))
                    {
                        yield return child;
                    }
                }
            }
        }

        public IEnumerable<PfbGameObject> IterAllGameObjects(bool includeChildren = false)
        {
            return IterGameObjects(includeChildren: includeChildren);
        }

        public void RebuildInfoTables()
        {
            // Reconstruct instance list
            List<RszInstance> rszInstances = new();
            if (GameObjects != null)
            {
                foreach (var gameObjectData in GameObjects)
                {
                    CollectGameObjectInstances(gameObjectData, rszInstances);
                }
            }

            RSZ.RebuildInstanceList(rszInstances);
            RSZ.RebuildInstanceInfo(false, false);
            RSZ.ClearObjects();

            // Reconstruct game object infos
            GameObjectInfoList.Clear();
            if (GameObjects != null)
            {
                foreach (var gameObjectData in GameObjects)
                {
                    AddGameObjectInfoRecursion(gameObjectData);
                }
            }

            RszUtils.SyncUserDataFromRsz(UserdataInfoList, RSZ);
            RszUtils.SyncResourceFromRsz(ResourceInfoList, RSZ);
            RebuildPfbRefs();
            StructChanged = false;
            ResourceChanged = false;
        }

        private void RebuildPfbRefs()
        {
            var props = RszParser.PrefabGameObjectRefProps;
            if (props == null) return;

            GameObjectRefInfoList.Clear();
            foreach (var instance in RSZ.InstanceList) {
                if (!props.TryGetValue(instance.RszClass.name, out var pfbRefs)) continue;

                for (int i = 0; i < instance.Fields.Length; i++) {
                    var field = instance.Fields[i];
                    if (!field.IsGameObjectRef) continue;

                    if (field.array) {
                        var list = (List<object>)instance.Values[i];
                        for (int index = 0; index < list.Count; index++) {
                            GameObjectRef? item = (GameObjectRef?)list[index];
                            if (item?.target?.Instance == null) continue;
                            TryAddRef(instance, pfbRefs, field, index, item.target.Instance);
                        }
                    } else {
                        var gref = (GameObjectRef)instance.Values[i];
                        if (gref?.target?.Instance == null) continue;

                        TryAddRef(instance, pfbRefs, field, 0, gref.target.Instance);
                    }
                }
            }

            bool TryAddRef(RszInstance instance, Dictionary<string, PrefabGameObjectRefProperty> pfbRefs, RszField field, int index, RszInstance target)
            {
                if (instance.ObjectTableIndex == -1) {
                    RSZ.AddToObjectTable(instance);
                }

                if (!pfbRefs.TryGetValue(field.name, out var refdata)) {
                    Log.Error("Unknown PFB game object ref field " + field.name);
                    return false;
                }

                GameObjectRefInfoList.Add(new ReeLib.Pfb.PfbGameObjectRefInfo() {
                    arrayIndex = index,
                    objectId = instance.ObjectTableIndex,
                    propertyId = refdata.PropertyId,
                    targetId = target.ObjectTableIndex,
                });
                return true;
            }
        }

        private void AddGameObjectInfoRecursion(PfbGameObject gameObject)
        {
            var instance = gameObject.Instance!;
            if (instance.ObjectTableIndex != -1) return;
            ref var infoData = ref gameObject.Info;
            RSZ.AddToObjectTable(instance);
            infoData.objectId = instance.ObjectTableIndex;
            infoData.parentId = gameObject.Parent?.ObjectId ?? -1;
            infoData.componentCount = (short)gameObject.Components.Count;

            GameObjectInfoList.Add(gameObject.Info);
            foreach (var item in gameObject.Components)
            {
                RSZ.AddToObjectTable(item);
            }
            foreach (var child in gameObject.Children)
            {
                AddGameObjectInfoRecursion(child);
            }
        }

        /// <summary>
        /// 从ScnFile.GameObject生成Pfb
        /// </summary>
        /// <param name="scnGameObject"></param>
        public void PfbFromScnGameObject(Scn.ScnGameObject scnGameObject)
        {
            PfbGameObject gameObject = PfbGameObject.FromScnGameObject(scnGameObject);
            GameObjects.Clear();
            GameObjects.Add(gameObject);
            RebuildInfoTables();
        }

        public void RemoveGameObject(PfbGameObject gameObject)
        {
            if (gameObject.Parent != null)
            {
                gameObject.Parent.Children.Remove(gameObject);
                gameObject.Parent = null;
            }
            else
            {
                GameObjects?.Remove(gameObject);
            }
            StructChanged = true;
        }

        /// <summary>
        /// Add a fully constructed game object to the file.
        /// </summary>
        /// <param name="gameObject">The game object to add.</param>
        /// <param name="parent">The parent under which the game object should be added.</param>
        /// <param name="insertIndex">The child index at which to insert the new game object relative to its parent.</param>
        /// <returns>The imported game object (new instance if shouldClone was true, otherwise the same instance that was passed in).</returns>
        public PfbGameObject AddGameObject(PfbGameObject gameObject,
                                     PfbGameObject? parent = null, int insertIndex = -1)
        {
            PfbGameObject newGameObject = gameObject;

            newGameObject.Parent = null;
            ObservableCollection<PfbGameObject> collection;
            if (parent != null)
            {
                newGameObject.Parent = parent;
                collection = parent.Children;
            }
            else
            {
                collection = GameObjects;
            }

            // Preliminary inserts so the RszInstances have some indices, not really necessary
            RSZ.InsertInstances(IterGameObjectInstances(newGameObject));

            if (insertIndex != -1)
            {
                collection.Insert(insertIndex, newGameObject);
            }
            else
            {
                collection.Add(newGameObject);
            }
            StructChanged = true;
            RszInstance.CleanCloneCache();
            return newGameObject;
        }

        public void AddGameObjects(
            IEnumerable<PfbGameObject> gameObjects,
            PfbGameObject? parent = null)
        {
            foreach (var gameObject in gameObjects)
            {
                AddGameObject(gameObject, parent);
            }
        }

        public PfbGameObject DuplicateGameObject(PfbGameObject gameObject)
        {
            return AddGameObject(gameObject.Clone(), gameObject.Parent, (gameObject.Parent?.Children ?? GameObjects).IndexOf(gameObject));
        }

        public void AddComponent(IGameObject gameObject, string className)
        {
            var component = RSZ.CreateInstance(className);
            gameObject.Components.Add(component);
            StructChanged = true;
        }

        public void AddComponent(IGameObject gameObject, RszInstance component)
        {
            gameObject.Components.Add(component);
            StructChanged = true;
        }

        public void RemoveComponent(IGameObject gameObject, RszInstance component)
        {
            gameObject.Components.Remove(component);
            StructChanged = true;
        }

        public void ClearGameObjects()
        {
            GameObjects?.Clear();
            GameObjectInfoList.Clear();
            GameObjectRefInfoList.Clear();
            RSZ.ClearInstances();
        }
    }
}
