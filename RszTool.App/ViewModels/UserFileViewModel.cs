namespace RszTool.App.ViewModels
{
    public class UserFileViewModel(UserFile file) : BaseRszFileViewModel
    {
        public override BaseRszFile File => UserFile;
        public UserFile UserFile { get; } = file;

        public bool ResourceChanged
        {
            get => UserFile.ResourceChanged;
            set => UserFile.ResourceChanged = value;
        }

        public RszViewModel RszViewModel => new(UserFile.RSZ!);

        private TreeItemViewModel[]? treeViewItems;
        public override IEnumerable<object> TreeViewItems
        {
            get
            {
                return treeViewItems ??= [
                    new TreeItemViewModel("Resources", UserFile.ResourceInfoList),
                    new TreeItemViewModel("Instances", RszViewModel.Instances),
                    new TreeItemViewModel("Objects", RszViewModel.Objects),
                ];
            }
        }
    }
}
