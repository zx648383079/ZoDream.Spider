using System.Windows;
using System.Windows.Controls;
using ZoDream.Shared.Models;
using ZoDream.Spider.Controls;

namespace ZoDream.Spider.Pages
{
    /// <summary>
    /// BrowserView.xaml 的交互逻辑
    /// </summary>
    public partial class BrowserView : Window
    {
        public BrowserView()
        {
            InitializeComponent();
        }

        public void Navigate(string source, RequestData? requestData)
        {
            var tab = new BrowserTabItem(source, requestData);
            var tabItem = new TabItem()
            {
                Header = "Loading...",
                Content = tab
            };
            tab.TitleChanged += (_, title) => {
                tabItem.Header = title;
            };
            tab.NewTabRequested += (_, url) => {
                Navigate(url, requestData);
            };
            TabPanel.Items.Add(tabItem);
            TabPanel.SelectedItem = tabItem;
        }
    }
}
