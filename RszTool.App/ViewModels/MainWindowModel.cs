using System.ComponentModel;
using System.IO;
using System.Windows;
using AvalonDock;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using AvalonDock.Themes;
using Microsoft.Win32;
using RszTool.App.Common;
using RszTool.App.Resources;
using RszTool.App.Views;

namespace RszTool.App.ViewModels
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        private const string LayoutConfigFileName = "RszTool.App.Layout.config";
        public event PropertyChangedEventHandler? PropertyChanged;
        public DockingManager? DockingManager { get; set; }
        public LayoutContent? SelectedTabItem => DockingManager!.Layout.ActiveContent;

        public IEnumerable<LayoutDocumentPane> GetLayoutDocumentPanes()
        {
            return DockingManager!.Layout.Descendents().OfType<LayoutDocumentPane>();
        }

        public IEnumerable<FileTabItemViewModel> GetFileTabs()
        {
            return DockingManager!.Layout.Descendents().OfType<FileTabItemViewModel>();
        }

        public FileExplorerViewModel FileExplorerViewModel { get; } = new();
        public Theme DockingTheme { get; set; }

        private BaseRszFileViewModel? CurrentFile =>
            SelectedTabItem is FileTabItemViewModel fileTabItemViewModel ?
            fileTabItemViewModel.FileViewModel : null;

        public bool IsDarkTheme
        {
            get => ThemeManager.Instance.IsDarkTheme;
            set
            {
                SaveData.IsDarkTheme = value;
                ThemeManager.Instance.IsDarkTheme = value;
                DockingTheme = value ? new Vs2013DarkTheme() : new Vs2013LightTheme();
            }
        }

        public static SaveData SaveData => App.Instance.SaveData;

        public RelayCommand OpenCommand => new(OnOpen);
        public RelayCommand SaveCommand => new(OnSave);
        public RelayCommand SaveAsCommand => new(OnSaveAs);
        public RelayCommand ReopenCommand => new(OnReopen);
        public RelayCommand AddFolderCommand => new(OnAddFolder);
        public RelayCommand CloseCommand => new(OnClose);
        public RelayCommand QuitCommand => new(OnQuit);
        public RelayCommand PreferenceCommand => new(OnPreference);
        public RelayCommand ClearRecentFilesHistory => new(OnClearRecentFilesHistory);
        public RelayCommand RemoveNonExistedRecentFilesHistory => new(OnRemoveNonExistedRecentFilesHistory);
        public RelayCommand OpenRecentFile => new(OnOpenRecentFile);
        public RelayCommand OpenAbout => new(OnOpenAbout);
        public RelayCommand OpenChangeLog => new(OnOpenChangeLog);

        public MainWindowModel()
        {
            foreach (var path in SaveData.OpenedFolders)
            {
                FileExplorerViewModel.Folders.Add(new(path));
            }
            if (FileExplorerViewModel.Folders.Count == 0)
            {
                FileExplorerViewModel.Folders.Add(new(Directory.GetCurrentDirectory()));
            }
            FileExplorerViewModel.OnFileSelected += f => OpenFile(f.Path);
            DockingTheme = IsDarkTheme ? new Vs2013DarkTheme() : new Vs2013LightTheme();
        }

        public void PostInit()
        {
            if (SaveData.OpenedFiles.Count > 0)
            {
                List<string> files = SaveData.OpenedFiles;
                SaveData.OpenedFiles = new();
                foreach (var path in files)
                {
                    if (File.Exists(path))
                    {
                        OpenFile(path);
                    }
                }
            }
            if (File.Exists(LayoutConfigFileName))
            {
                try
                {
                    var serializer = new XmlLayoutSerializer(DockingManager);
                    using var stream = new StreamReader(LayoutConfigFileName);
                    serializer.Deserialize(stream);

                    SaveData.OpenedFiles.Clear();
                    foreach (var fileTab in GetFileTabs())
                    {
                        fileTab.PostInit();
                        SaveData.OpenedFiles.Add(fileTab.FileViewModel.FilePath!);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path"></param>
        public bool OpenFile(string path)
        {
            if (!File.Exists(path)) return false;
            // check file opened
            foreach (var fileTab in GetFileTabs())
            {
                if (fileTab.FileViewModel.FilePath == path)
                {
                    return true;
                }
            }

            string rszJsonFile = $"rsz{SaveData.GameName}.json";
            if (!File.Exists(rszJsonFile))
            {
                MessageBoxUtils.Warning(string.Format(Texts.RszJsonNotFound, rszJsonFile));
                return false;
            }

            var content = RszFileViewFactory.Create(SaveData.GameName, path);
            if (content != null)
            {
                LayoutDocument document = new FileTabItemViewModel(content)
                {
                    IsSelected = true
                };
                if (GetLayoutDocumentPanes().FirstOrDefault() is LayoutDocumentPane pane)
                {
                    pane.Children.Add(document);
                }
                else
                {
                    throw new InvalidOperationException("No LayoutDocumentPaneGroup");
                }

                SaveData.AddRecentFile(path);
                SaveData.OpenedFiles.Add(path);
                return true;
            }
            else
            {
                MessageBoxUtils.Info(Texts.NotSupportedFormat);
            }
            return false;
        }

        public void TryOpenFile(string path)
        {
            AppUtils.TryAction(() => OpenFile(path));
        }

        public void OnDropFile(string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                if (Directory.Exists(path))
                {
                    FileExplorerViewModel.AddFolder(path);
                }
                else
                {
                    TryOpenFile(path);
                }
            }
        }

        private static readonly (string, string)[] SupportedFile = {
            ("User file", "*.user.*"),
            ("Scene file", "*.scn.*"),
            ("Prefab file", "*.pfb.*"),
        };

        private static readonly string OpenFileFilter =
            $"All file|{string.Join(";", SupportedFile.Select(item => item.Item2))}|" +
            string.Join("|", SupportedFile.Select(item => $"{item.Item1}|{item.Item2}"));

        private void OnOpen(object arg)
        {
            var dialog = new OpenFileDialog
            {
                Filter = OpenFileFilter
            };
            if (dialog.ShowDialog() == true)
            {
                TryOpenFile(dialog.FileName);
            }
        }

        private void OnSave(object arg)
        {
            AppUtils.TryAction(() => CurrentFile?.Save());
        }

        private void OnSaveAs(object arg)
        {
            var currentFile = CurrentFile;
            if (currentFile == null) return;
            var dialog = new SaveFileDialog
            {
                FileName = currentFile.FileName,
                Filter = OpenFileFilter
            };
            if (dialog.ShowDialog() == true)
            {
                // Open document
                string? path = dialog.FileName;
                if (path != null)
                {
                    AppUtils.TryAction(() =>
                    {
                        if(currentFile.SaveAs(path))
                        {
                            SaveData.AddRecentFile(path);
                        }
                    });
                }
            }
        }

        private void OnReopen(object arg)
        {
            AppUtils.TryAction(() => CurrentFile?.Reopen());
        }

        public static bool OnTabClose(FileTabItemViewModel fileTab)
        {
            if (fileTab.FileViewModel.Changed)
            {
                // Check changed
                var result = MessageBoxUtils.YesNoCancel(
                    $"{Texts.FileChangedPrompt}\n{fileTab.FileViewModel.FilePath}");
                if (result == MessageBoxResult.Yes)
                {
                    AppUtils.TryAction(() => fileTab.FileViewModel.Save());
                }
                else if (result == MessageBoxResult.Cancel) return false;
            }
            SaveData.OpenedFiles.Remove(fileTab.FileViewModel.FilePath!);
            return true;
        }

        private void OnClose(object arg)
        {
            if (SelectedTabItem is FileTabItemViewModel fileTab)
            {
                fileTab.Close();
            }
        }

        private void OnQuit(object arg)
        {
            if (OnExit())
            {
                Application.Current.Shutdown();
            }
        }

        private void OnPreference(object arg)
        {
            PreferenceWindow preferenceWindow = new();
            preferenceWindow.DataContext = SaveData;
            preferenceWindow.ShowDialog();
        }

        private void OnClearRecentFilesHistory(object arg)
        {
            SaveData.RecentFiles.Clear();
        }

        private void OnRemoveNonExistedRecentFilesHistory(object arg)
        {
            List<string> newList = new();
            foreach (var item in SaveData.RecentFiles)
            {
                if (File.Exists(item))
                {
                    newList.Add(item);
                }
            }
            SaveData.RecentFiles = new(newList);
        }

        private void OnOpenRecentFile(object arg)
        {
            if (arg is string path)
            {
                TryOpenFile(path);
            }
        }

        private void OnAddFolder(object arg)
        {
            var dialog = new OpenFolderDialog();
            if (dialog.ShowDialog() == true && dialog.FolderName != null)
            {
                FileExplorerViewModel.AddFolder(dialog.FolderName);
            }
        }

        public bool OnExit()
        {
            foreach (var fileTab in GetFileTabs())
            {
                if (!OnTabClose(fileTab)) return false;
            }
            var serializer = new XmlLayoutSerializer(DockingManager);
            using var stream = new StreamWriter(LayoutConfigFileName);
            serializer.Serialize(stream);
            return true;
        }

        private void OnOpenAbout(object arg)
        {
            var about = new AboutWindow();
            about.ShowDialog();
        }

        private void OnOpenChangeLog(object arg)
        {
            var window = new LongTextWindow
            {
                Title = Texts.ChangeLog,
                Text = Texts.ChangeLogContent
            };
            window.ShowDialog();
        }
    }


    public class FileTabItemViewModel : LayoutDocument
    {
        public FileTabItemViewModel()
        {
        }

        public FileTabItemViewModel(FrameworkElement content)
        {
            Content = content;
            PostInit();
        }

        public void PostInit()
        {
            var fileViewModel = FileViewModel;
            fileViewModel.HeaderChanged += UpdateHeader;
            Title = fileViewModel.FileName;
            ContentId = fileViewModel.FilePath;
        }

        public BaseRszFileViewModel FileViewModel => (BaseRszFileViewModel)((FrameworkElement)Content).DataContext;

        public void UpdateHeader()
        {
            var fileViewModel = FileViewModel;
            Title = fileViewModel.FileName + (fileViewModel.Changed ? "*" : "");
        }
    }


    public class RszFileViewFactory
    {
        public static FrameworkElement? Create(GameName gameName, string path)
        {
            BaseRszFileViewModel? fileViewModel = null;
            RszFileOption option = new(gameName);
            FrameworkElement? content = null;
            switch (RszUtils.GetFileType(path))
            {
                case FileType.user:
                    fileViewModel = new UserFileViewModel(new(option, new(path)));
                    content = new RszUserFileView();
                    break;
                case FileType.pfb:
                    fileViewModel = new PfbFileViewModel(new(option, new(path)));
                    content = new RszPfbFileView();
                    break;
                case FileType.scn:
                    fileViewModel = new ScnFileViewModel(new(option, new(path)));
                    content = new RszScnFileView();
                    break;
            }
            if (fileViewModel != null && content != null)
            {
                bool readSuccess = false;
                for (int tryCount = 0; tryCount < 5; tryCount++)
                {
                    try
                    {
                        readSuccess = fileViewModel.Read();
                        break;
                    }
                    catch (RszRetryOpenException e)
                    {
                        fileViewModel.File.FileHandler.Seek(0);
                        MessageBoxUtils.Info(string.Format(Texts.TryReopen, e.Message));
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                if (!readSuccess)
                {
                    MessageBoxUtils.Error(Texts.ReadFailed);
                    return null;
                }
                content.DataContext = fileViewModel;
                return content;
            }
            return null;
        }
    }
}
