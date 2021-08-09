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

namespace ZoDream.Spider.Pages
{
    /// <summary>
    /// EditUrlView.xaml 的交互逻辑
    /// </summary>
    public partial class EditUrlView : Window
    {
        public EditUrlView()
        {
            InitializeComponent();
        }

        public IList<string> UrlItems
        {
            get {
                return ContentTb.Text.Split(new char[] { '\r', '\n' }).Where(i => !string.IsNullOrWhiteSpace(i)).Distinct().ToList();
            }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
