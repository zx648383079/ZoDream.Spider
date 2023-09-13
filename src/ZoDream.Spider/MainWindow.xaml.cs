using Microsoft.VisualBasic.Logging;
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
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Loggers;
using ZoDream.Shared.Models;
using ZoDream.Shared.Routes;
using ZoDream.Spider.Loggers;
using ZoDream.Spider.Pages;
using ZoDream.Spider.ViewModels;

namespace ZoDream.Spider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShellManager.BindFrame(InnerFrame);
            App.ViewModel.BaseWindow = this;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            ShellManager.UnBind();
            App.ViewModel.Dispose();
        }
    }
}
