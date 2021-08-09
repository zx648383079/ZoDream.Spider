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
        }

        private void BrowserBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new BrowserView(true);
            if (page.ShowDialog() != true)
            {
                return;
            }
            var items = page.HeaderItems;
        }
    }
}
