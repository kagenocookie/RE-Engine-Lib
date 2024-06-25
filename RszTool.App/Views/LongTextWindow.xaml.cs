using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RszTool.App.Views
{
    /// <summary>
    /// LongTextWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LongTextWindow : Window
    {
        public LongTextWindow()
        {
            InitializeComponent();
        }

        public string Text { get => TextBox.Text; set => TextBox.Text = value; }
    }
}
