using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RszTool.App.Common;
using RszTool.App.Resources;
using RszTool.App.ViewModels;

namespace RszTool.App.Controls
{
    public class ConsoleBox : UserControl
    {
        public RichTextBox RichTextBox { get; }

        public ConsoleBox()
        {
            RichTextBox = new RichTextBox
            {
                IsReadOnly = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Padding = new Thickness(2),
                ContextMenu = new ContextMenu
                {
                    Items =
                    {
                        new MenuItem { Header = Texts.Clear, Command = new RelayCommand(Clear) }
                    }
                }
            };
            RichTextBox.Document.LineHeight = 1;
            Content = RichTextBox;
            Console.SetOut(new RichTextBoxWriter(RichTextBox)
            {
                SelectionColor = ThemeManager.Instance.LightForeground,
                DarkSelectionColor = ThemeManager.Instance.DarkForeground,
            });
            Console.SetError(new RichTextBoxWriter(RichTextBox)
            {
                SelectionColor = ThemeManager.Instance.LightErrorBrush,
                DarkSelectionColor = ThemeManager.Instance.DarkErrorBrush,
            });
        }

        private void Clear(object arg)
        {
            RichTextBox.Document.Blocks.Clear();
        }
    }
}
