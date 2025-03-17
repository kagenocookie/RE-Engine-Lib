using System.Collections.ObjectModel;
using RszTool.App.Common;

namespace RszTool.App.ViewModels
{
    public class RcolFileViewModel(RcolFile file) : BaseRszFileViewModel
    {
        public override BaseRszFile File => RcolFile;
        public RcolFile RcolFile { get; } = file;

        public RszViewModel RszViewModel => new(RcolFile.RSZ);

        public override IEnumerable<object> TreeViewItems
        {
            get
            {
                yield return new TreeItemViewModel("Instances", RszViewModel.Instances);
                yield return new TreeItemViewModel("Objects", RszViewModel.Objects);
                yield return new TreeItemViewModel("Groups", RcolFile.GroupInfoList);
                yield return new TreeItemViewModel("Request Sets", RcolFile.RequestSetInfoList);
            }
        }
    }

    public class RcolShapeViewModel(RcolFile.RcolGroup group, RcolFile.RcolShape shape) : BaseViewModel
    {
        public RcolFile.RcolGroup Group { get; } = group;
        public RcolFile.RcolShape Shape { get; } = shape;

        public string Name => Shape.name ?? Shape.Guid.ToString();
        public IEnumerable<object> Items => Group.Shapes;

        public void NotifyItemsChanged()
        {
            OnPropertyChanged(nameof(Items));
        }

        public override string ToString() => Shape.shapeType + " shape: " + Name;

        public static ObservableCollection<RcolShapeViewModel> MakeList(RcolFile.RcolGroup group)
        {
            ObservableCollection<RcolShapeViewModel> list = new();
            foreach (var item in group.Shapes)
            {
                list.Add(new(group, item));
            }
            // group.Components.CollectionChanged += (_, e) =>
            // {
            //     ObjectModelUtils.SyncObservableCollection(list,
            //         obj => new RcolShapeComponentViewModel(gameObject), e);
            // };
            return list;
        }
    }
}
