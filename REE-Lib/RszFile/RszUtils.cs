using ReeLib.Common;

namespace ReeLib
{
    public static class RszUtils
    {
        public static bool IsResourcePath(string path)
        {
            if (!path.Contains('/')) return false;
            var ext = Path.GetExtension(path.AsSpan()).Trim();
            if (ext.IsEmpty) return false;

            // there are fields that can seem like paths (e.g. developer comments) but turns out aren't, this should cross those out
            // example: re7 app.EPVStandardData.Element "Comment"
            // 初壁登場/モーション【Enemy/em4000/fbx/em4000_Slow_Appear_first.mot.fbx】
            if (!char.IsAscii(path[0])) return false;
            foreach (var ch in ext) {
                if (!char.IsAscii(ch)) return false;
            }

            return true;
        }

        public static void AddUserDataFromRsz(List<UserdataInfo> userdataInfos, RSZFile rsz, int userDataStart = 0, int length = -1)
        {
            if (length == -1) length = rsz.RSZUserDataInfoList.Count - userDataStart;
            for (int i = 0; i < length; i++)
            {
                var item = rsz.RSZUserDataInfoList[i + userDataStart];
                if (item is RSZUserDataInfo info)
                {
                    userdataInfos.Add(info.ToUserdataInfo(rsz.RszParser));
                }
            }
        }

        public static void SyncUserDataFromRsz(List<UserdataInfo> userdataInfos, RSZFile rsz)
        {
            userdataInfos.Clear();
            AddUserDataFromRsz(userdataInfos, rsz);
        }

        public static void ScanRszForResources(List<ResourceInfo> resourcesInfos, RSZFile rsz, int instanceStart = 0, int length = -1)
        {
            if (length == -1) length = rsz.InstanceList.Count - instanceStart;
            HashSet<string> addedPath = new();
            foreach (var item in resourcesInfos)
            {
                if (item.Path != null)
                {
                    addedPath.Add(item.Path);
                }
            }
            void CheckResource(string path)
            {
                if (IsResourcePath(path) && !addedPath.Contains(path))
                {
                    addedPath.Add(path);
                    resourcesInfos.Add(new ResourceInfo { Path = path });
                }
            }

            for (int i = 0; i < length; i++)
            {
                var instance = rsz.InstanceList[i + instanceStart];
                if (instance.RSZUserData != null) continue;
                var fields = instance.RszClass.fields;
                // avoid reference unused resource
                if (instance.RszClass.name == "via.Folder" && !Convert.ToBoolean(instance.Values[4]))
                {
                    continue;
                }
                else if (instance.RszClass.name == "via.Prefab" && !Convert.ToBoolean(instance.Values[0]))
                {
                    continue;
                }
                for (int j = 0; j < fields.Length; j++)
                {
                    var field = fields[j];
                    if (field.IsString)
                    {
                        if (field.array)
                        {
                            foreach (var item in (List<object>)instance.Values[j])
                            {
                                CheckResource((string)item);
                            }
                        }
                        else
                        {
                            CheckResource((string)instance.Values[j]);
                        }
                    }
                }
            }
        }

        public static void SyncResourceFromRsz(List<ResourceInfo> resourcesInfos, RSZFile rsz)
        {
            resourcesInfos.Clear();
            ScanRszForResources(resourcesInfos, rsz);
        }
    }
}
