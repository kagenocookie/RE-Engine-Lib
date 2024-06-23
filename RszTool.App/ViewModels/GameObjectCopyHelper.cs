using RszTool.App.Views;
using RszTool.Pfb;
using RszTool.Scn;

namespace RszTool.App.ViewModels
{
    public static class GameObjectCopyHelper
    {
        private static PfbGameObject? CopiedPfbGameObject { get; set; }
        private static ScnGameObject? CopiedScnGameObject { get; set; }

        public static void CopyGameObject(PfbGameObject gameObject)
        {
            CopiedPfbGameObject = gameObject;
            CopiedScnGameObject = null;
        }

        public static void CopyGameObject(ScnGameObject gameObject)
        {
            CopiedScnGameObject = gameObject;
            CopiedPfbGameObject = null;
        }

        public static PfbGameObject? GetCopiedPfbGameObject()
        {
            if (CopiedPfbGameObject != null) return CopiedPfbGameObject;
            if (CopiedScnGameObject != null)
                return PfbGameObject.FromScnGameObject(CopiedScnGameObject);
            return null;
        }

        public static ScnGameObject? GetCopiedScnGameObject()
        {
            if (CopiedScnGameObject != null) return CopiedScnGameObject;
            if (CopiedPfbGameObject != null)
                return ScnGameObject.FromPfbGameObject(CopiedPfbGameObject);
            return null;
        }

        public static void AutoIncreaseIndex(GameObjectContextID[] contextIDs)
        {
            int indexIncrementOffset = App.Instance.SaveData.ContextIDIndexIncrementOffset;
            if (indexIncrementOffset > 0)
            {
                foreach (var item in contextIDs)
                {
                    if ((int)item.IndexViewModel.Value > 0)
                    {
                        item.IndexViewModel.Value = (int)item.IndexViewModel.Value + indexIncrementOffset;
                    }
                }
            }
        }

        /// <summary>
        /// Update re4 chainsaw.ContextID
        /// </summary>
        /// <param name="fileOption"></param>
        /// <param name="gameObjectData"></param>
        public static void UpdateContextID(RszFileOption fileOption, IGameObject gameObjectData)
        {
            if (fileOption.GameName == GameName.re4)
            {
                var contextIDs = IterGameObjectContextID(gameObjectData).ToArray();
                if (contextIDs.Length > 0)
                {
                    UpdateContextIDWindow dialog = new()
                    {
                        TreeViewItems = contextIDs
                    };
                    dialog.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Update re4 chainsaw.ContextID
        /// </summary>
        /// <param name="fileOption"></param>
        /// <param name="gameObjectData"></param>
        public static void UpdateInstanceContextID(RszFileOption fileOption, RszInstance instance)
        {
            if (fileOption.GameName == GameName.re4)
            {
                var contextIDs = IterInstanceContextID(instance).ToArray();
                if (contextIDs.Length > 0)
                {
                    UpdateContextIDWindow dialog = new()
                    {
                        TreeViewItems = contextIDs
                    };
                    dialog.ShowDialog();
                }
            }
        }

        /// <summary>
        /// 迭代GameObject以及子物体的组件，查找re4 chainsaw.ContextID
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static IEnumerable<GameObjectContextID> IterGameObjectContextID(IGameObject gameObject)
        {
            foreach (var component in gameObject.Components)
            {
                if (component.RSZUserData != null) continue;
                var fields = component.Fields;
                for (int i = 0; i < component.Values.Length; i++)
                {
                    if (fields[i].IsReference && component.Values[i] is RszInstance instanceField)
                    {
                        if (instanceField.RszClass.name == "chainsaw.ContextID")
                        {
                            yield return new GameObjectContextID($"{gameObject}/{component}.{fields[i].name}", instanceField);
                        }
                    }
                }
            }
            foreach (var child in gameObject.GetChildren())
            {
                foreach (var item in IterGameObjectContextID(child))
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<GameObjectContextID> IterInstanceContextID(RszInstance instance)
        {
            List<RszInstanceFieldRecord> paths = new();
            foreach (var item in instance.Flatten(new(){ Paths = paths }))
            {
                if (item.RSZUserData != null) continue;
                var fields = item.Fields;
                string? parentPath = null;
                for (int i = 0; i < item.Values.Length; i++)
                {
                    if (fields[i].IsReference && item.Values[i] is RszInstance instanceField)
                    {
                        if (instanceField.RszClass.name == "chainsaw.ContextID")
                        {
                            parentPath ??= string.Join("/", paths.Select(x => x.ToString()));
                            yield return new GameObjectContextID($"{parentPath}/{item}.{fields[i].name}", instanceField);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// For re4 chainsaw.ContextID, when paste game object, update the ContextID
    /// </summary>
    public class GameObjectContextID
    {
        public GameObjectContextID(string name, RszInstance instance)
        {
            Name = name;
            if (instance.RszClass.name != "chainsaw.ContextID")
            {
                throw new ArgumentException("Expected chainsaw.ContextID");
            }
            int groupIndex = instance.RszClass.IndexOfField("_Group");
            int indexIndex = instance.RszClass.IndexOfField("_Index");
            GroupViewModel = new(instance, groupIndex);
            IndexViewModel = new(instance, indexIndex);
            Items = [
                GroupViewModel,
                IndexViewModel,
            ];

            GroupViewModel.PropertyChanged += (o, e) => {
                App.Instance.SaveData.LastContextID.Group = (int)GroupViewModel.Value;
            };
            IndexViewModel.PropertyChanged += (o, e) => {
                App.Instance.SaveData.LastContextID.Index = (int)IndexViewModel.Value;
            };
        }

        public string Name { get; set; }
        public RszFieldNormalViewModel[] Items { get; set; }

        public RszFieldNormalViewModel GroupViewModel { get; }
        public RszFieldNormalViewModel IndexViewModel { get; }
    }
}
