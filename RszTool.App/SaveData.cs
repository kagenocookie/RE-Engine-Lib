using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json.Serialization;
using RszTool.App.Resources;
using RszTool.Common;

namespace RszTool.App
{
    public class SaveData : INotifyPropertyChanged
    {
        public const string JsonPath = "RszTool.App.SaveData.json";

        [JsonConverter(typeof(EnumJsonConverter<GameName>))]
        public GameName GameName { get; set; } = GameName.re4;
        public ObservableCollection<string> RecentFiles { get; set; } = new();
        public List<string> OpenedFolders { get; set; } = new();
        public List<string> OpenedFiles { get; set; } = new();
        public ContextIDData LastContextID { get; set; } = new();
        // 更新ContextID时自动更新+n
        public int ContextIDIndexIncrementOffset { get; set; } = 100;
        public bool IsDarkTheme { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void AddRecentFile(string path)
        {
            int recentIndex = RecentFiles.IndexOf(path);
            if (recentIndex >= 0)
            {
                RecentFiles.Move(recentIndex, 0);
            }
            else
            {
                RecentFiles.Insert(0, path);
            }
        }

        public string ContextIDText => $"{LastContextID.Text}, {Texts.ContextIDIndexIncrementOffset}: {ContextIDIndexIncrementOffset}";
    }


    public class ContextIDData : INotifyPropertyChanged
    {
        public int Group { get; set; }
        public int Index { get; set; }

        public string Text => $"Last ContextID Index: {Index} Group: {Group}";

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
