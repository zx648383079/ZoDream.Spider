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
using ZoDream.Shared.Models;
using ZoDream.Spider.Models;
using ZoDream.Spider.ViewModels;

namespace ZoDream.Spider.Pages
{
    /// <summary>
    /// SettingView.xaml 的交互逻辑
    /// </summary>
    public partial class SettingView : Window
    {
        public SettingView()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public SettingViewModel ViewModel = new();

        private void InstallBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PluginInstall((sender as Button)!.DataContext as PluginInfoItem);
        }

        private void UnInstallBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PluginUnInstall((sender as Button)!.DataContext as PluginInfoItem);
        }

        private void ImportBtn_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                RestoreDirectory = true,
                Filter = "DLL|*.dll|All|*.*",
                Multiselect = true,
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            ViewModel.PluginImport(picker.FileNames);
        }
    }
}
