using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using RszTool.App.ViewModels;

namespace RszTool.App.Controls
{
    public class TextBoxWriter : TextWriter
    {
        protected TextBoxBase textBox;
        public delegate void WriteFunc(string value);

        public TextBoxWriter(TextBoxBase textBox)
        {
            this.textBox = textBox;
        }

        /// <summary>
        /// 使用UTF-16避免不必要的编码转换
        /// </summary>
        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }

        public virtual void DoWrite(string? value)
        {
            textBox.AppendText(value);
        }

        /// <summary>
        /// 最低限度需要重写的方法
        /// </summary>
        /// <param name="value"></param>
        public override void Write(string? value)
        {
            if (!textBox.CheckAccess())
            {
                textBox.Dispatcher.BeginInvoke((WriteFunc)Write, value);
            }
            else
            {
                DoWrite(value);
            }
        }

        /// <summary>
        /// 其他直接调用WriteLine(value)的情况会调用此函数
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine()
        {
            Write(NewLine);
        }

        /// <summary>
        /// 为提高效率直接处理一行的输出
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(string? value)
        {
            if (!textBox.CheckAccess())
            {
                textBox.Dispatcher.BeginInvoke((WriteFunc)WriteLine, value);
            }
            else
            {
                DoWrite(value);
                DoWrite(NewLine);
            }
        }
    }


    public class RichTextBoxWriter : TextBoxWriter
    {
        public RichTextBox RichTextBox => (textBox as RichTextBox)!;
        public Brush? SelectionColor { get; set; }
        public Brush? DarkSelectionColor { get; set; }

        public RichTextBoxWriter(RichTextBox textBox) : base(textBox)
        {
        }

        public override void DoWrite(string? value)
        {
            if (value == NewLine)
            {
                RichTextBox.AppendText(value);
            }
            else
            {
                TextRange tr = new(RichTextBox.Document.ContentEnd, RichTextBox.Document.ContentEnd)
                {
                    Text = value
                };
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, ThemeManager.Instance.IsDarkTheme ? DarkSelectionColor : SelectionColor);
            }
        }
    }
}
