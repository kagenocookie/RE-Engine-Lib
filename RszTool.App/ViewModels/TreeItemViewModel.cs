using System.ComponentModel;

namespace RszTool.App.ViewModels
{
    public abstract class BaseTreeItemViewModel(string name) : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public string Name { get; set; } = name;
        public abstract IEnumerable<object>? Items { get; }

        public override string ToString()
        {
            return Name;
        }
    }


    public class TreeItemViewModel(string name, IEnumerable<object>? items) : BaseTreeItemViewModel(name)
    {
        public override IEnumerable<object>? Items { get; } = items;
    }


    public class GameObjectsHeader(string name, IEnumerable<object>? items) : TreeItemViewModel(name, items);
    public class FoldersHeader(string name, IEnumerable<object>? items) : TreeItemViewModel(name, items);
}
