using AngleSharp.Io;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZoDream.Shared.Http;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Spider.Controls;
using ZoDream.Spider.JsObjects;

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
