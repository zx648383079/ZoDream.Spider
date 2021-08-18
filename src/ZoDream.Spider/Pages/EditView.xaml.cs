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
using ZoDream.Spider.ViewModels;

namespace ZoDream.Spider.Pages
{
    /// <summary>
    /// EditView.xaml 的交互逻辑
    /// </summary>
    public partial class EditView : Window
    {

        public EditViewModel ViewModel = new EditViewModel();
        public EditView()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void AddRuleBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new RuleView();
            if (page.ShowDialog() != true)
            {
                return;
            }
            ViewModel.RuleItems.Add(page.RuleGroup);
        }

        private void BrowserBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new BrowserView(true);
            if (page.ShowDialog() != true)
            {
                return;
            }
            var items = page.HeaderItems;
            foreach (var item in items)
            {
                ViewModel.AddHeader(item);
            }
        }

        private void HeaderSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(HeaderTb.Text) || string.IsNullOrWhiteSpace(HeaderValueTb.Text))
            {
                return;
            }
            var item = new HeaderItem(HeaderTb.Text, HeaderValueTb.Text);
            ViewModel.AddHeader(item);
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            var folder = new System.Windows.Forms.FolderBrowserDialog
            {
                SelectedPath = AppDomain.CurrentDomain.BaseDirectory,
                ShowNewFolderButton = false
            };
            if (folder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            FolderTb.Text = folder.SelectedPath;
        }
    }
}
