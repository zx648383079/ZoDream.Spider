using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZoDream.Spider.Controls
{
    /// <summary>
    /// FileInput.xaml 的交互逻辑
    /// </summary>
    public partial class FileInput : UserControl
    {
        public FileInput()
        {
            InitializeComponent();
        }

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(FileInput), new PropertyMetadata(string.Empty));



        public bool IsFile
        {
            get { return (bool)GetValue(IsFileProperty); }
            set { SetValue(IsFileProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFileProperty =
            DependencyProperty.Register("IsFile", typeof(bool), typeof(FileInput), new PropertyMetadata(true));



        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Filter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(FileInput), new PropertyMetadata(string.Empty));




        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsFile)
            {
                OpenFile();
            } else
            {
                OpenFolder();
            }
        }

        private void FileTb_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Link;
            e.Handled = true;
        }

        private void FileTb_PreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var file = ((Array)e.Data.GetData(DataFormats.FileDrop))?.GetValue(0)?.ToString();
                if (string.IsNullOrEmpty(file))
                {
                    return;
                }
                FileName = file;
            }
        }

        private void OpenFolder()
        {
            var folder = new System.Windows.Forms.FolderBrowserDialog
            {
                SelectedPath = !string.IsNullOrWhiteSpace(FileName) && Directory.Exists(FileName) ? FileName :  AppDomain.CurrentDomain.BaseDirectory,
                ShowNewFolderButton = true
            };
            if (folder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            FileName = folder.SelectedPath;
        }

        private void OpenFile()
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择文件"
            };
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                picker.Filter = Filter;
            }
            if (picker.ShowDialog() != true)
            {
                return;
            }
            FileName = picker.FileName;
        }
    }
}
