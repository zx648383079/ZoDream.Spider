using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using ZoDream.Shared.Http;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.Pages
{
    /// <summary>
    /// ProxyView.xaml 的交互逻辑
    /// </summary>
    public partial class ProxyView : Window
    {
        public ProxyView()
        {
            InitializeComponent();
        }

        public IList<ProxyItem> ProxyItems
        {
            get
            {
                var items = new List<ProxyItem>();
                var data = ContentTb.Text.Split(new char[] { '\r', '\n' });
                foreach (var item in data)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        items.Add(new ProxyItem(item.Trim()));
                    }
                }
                return items;
            }
            set
            {
                var sb = new StringBuilder();
                foreach (var item in value)
                {
                    sb.AppendLine(item.ToString());
                }
                ContentTb.Text = sb.ToString();
            }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            var items = ProxyItems;
            foreach (var item in items)
            {
                Debug.WriteLine(item.Host);
                Debug.WriteLine(HttpProxy.Test(item));
            }
        }
    }
}
