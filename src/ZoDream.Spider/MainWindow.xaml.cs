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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZoDream.Spider.Pages;
using ZoDream.Spider.ViewModels;

namespace ZoDream.Spider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel = new MainViewModel();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void AddUrlBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new EditUrlView();
            if (page.ShowDialog() != true)
            {
                return;
            }
            var items = page.UrlItems;
            
        }

        private void EditTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new EditView();
            if (page.ShowDialog() != true)
            {
                return;
            }
        }
    }
}
