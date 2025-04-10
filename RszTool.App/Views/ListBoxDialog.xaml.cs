using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace RszTool.App.Views
{
    /// <summary>
    /// ListBoxDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ListBoxDialog : Window
    {
        public string Message { get; set; } = "";
        public string InputText { get; set; } = "";
        private ICollectionView? collectionView;
        public IEnumerable ItemsSource
        {
            get => ListBox.ItemsSource;
            set => ListBox.ItemsSource = collectionView = CollectionViewSource.GetDefaultView(value);
        }
        public object SelectedItem => ListBox.SelectedItem;

        public ListBoxDialog()
        {
            InitializeComponent();
        }

        private void OnTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && e.OriginalSource is TextBox textBox)
            {
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                OnSearch(sender, e);
            }
        }

        private void OnSearch(object sender, EventArgs e)
        {
            if (collectionView == null) return;
            if (string.IsNullOrEmpty(InputText))
            {
                collectionView.Filter = null;
            }
            else
            {
                collectionView.Filter = (o) => o?.ToString()?.Contains(InputText) == true;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
