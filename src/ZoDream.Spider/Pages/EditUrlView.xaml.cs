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
using ZoDream.Shared.Utils;

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
                var items = new List<string>();
                foreach (var item in ContentTb.Text.Split(new char[] { '\r', '\n' }))
                {
                    if (string.IsNullOrWhiteSpace(item))
                    {
                        continue;
                    }
                    var real = item.Trim();
                    if (items.Contains(real))
                    {
                        continue;
                    }
                    items.AddRange(Html.GenerateUrl(real));
                }
                return items;
            }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ContentTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            AmountTb.Text = $"预计 {UrlItems.Count} 条";
        }
    }
}
