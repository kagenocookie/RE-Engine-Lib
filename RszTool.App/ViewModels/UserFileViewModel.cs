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

        public override IEnumerable<object> GetTreeViewItems()
        {
            return [
                new TreeItemViewModel("Resources", UserFile.ResourceInfoList),
                new TreeItemViewModel("Instances", RszViewModel.Instances),
                new TreeItemViewModel("Objects", RszViewModel.Objects),
            ];
        }
    }
}
