using ReeLib.Common;

namespace ReeLib
{
    public class RcfFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public List<(string name, object value)> Properties { get; } = new();
        public int headerNum;

        private Dictionary<uint, (string name, Type type)> KnownProperties = new() {
            { 2157998135, ("AppName", typeof(string)) },
            { 3391752931, ("VersionStr", typeof(string)) },
            { 1288450746, ("HjmUriStr", typeof(string)) },
            { 538003547, ("DetectCidLinkChanges", typeof(bool)) },
            { 2425294260, ("FailOnCidLinkChanged", typeof(bool)) },
            { 1795464471, ("GssArgsStr", typeof(string[])) },
            { 1430395780, ("1430395780", typeof(bool)) },
            { 1572474904, ("IgnoreRegionState", typeof(bool)) },
        };

        protected override bool DoRead()
        {
            var handler = FileHandler;
            handler.Read<int>(ref headerNum);
            Properties.Clear();
            for (int i = 0; i < headerNum && !handler.IsEnd; i++) {
                var propHash = handler.Read<uint>();
                if (KnownProperties.TryGetValue(propHash, out var propInfo)) {
                    if (propInfo.type == typeof(string)) {
                        Properties.Add((propInfo.name, handler.ReadWString(jumpBack: false)));
                    } else if (propInfo.type == typeof(string[])) {
                        var strCount = handler.Read<int>();
                        var strs = new string[strCount];
                        for (int k = 0; k < strCount; k++) strs[k] = handler.ReadWString(jumpBack: false);
                        Properties.Add((propInfo.name, strs));
                    } else if (propInfo.type == typeof(bool)) {
                        Properties.Add((propInfo.name, handler.Read<bool>()));
                    } else if (propInfo.type == typeof(uint)) {
                        Properties.Add((propInfo.name, handler.Read<uint>()));
                    }
                } else {
                    Log.Error("Unknown property hash " + propHash);
                }
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var version = handler.FileVersion;
            handler.Write(headerNum);
            foreach (var (name, value) in Properties) {
                if (uint.TryParse(name, out var hash)) {
                    handler.Write(hash);
                } else {
                    handler.Write(MurMur3HashUtils.GetHash(name));
                }
                switch (value) {
                    case string str: handler.WriteWString(str); break;
                    case string[] strList:
                        handler.Write(strList.Length);
                        foreach (var str in strList) {
                            handler.WriteWString(str);
                        }
                        break;
                    case bool bb: handler.Write(bb); break;
                    case uint nn: handler.Write(nn); break;
                }
            }

            return true;
        }
    }
}