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
    /// RuleView.xaml 的交互逻辑
    /// </summary>
    public partial class RuleView : Window
    {

        public RuleViewModel ViewModel = new RuleViewModel();
        public RuleView()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
