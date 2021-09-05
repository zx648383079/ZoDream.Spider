using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ZoDream.Shared.Http;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Spider.JsObjects;

namespace ZoDream.Spider.Pages
{
    /// <summary>
    /// BrowserView.xaml 的交互逻辑
    /// </summary>
    public partial class BrowserView : Window, IRequest
    {
        public BrowserView(bool showConfirm = false)
        {
            InitializeComponent();
            YesBtn.Visibility = showConfirm ? Visibility.Visible : Visibility.Collapsed;
        }

        private SpiderBridge bridge = new SpiderBridge();
        public bool SupportTask { get; } = false;

        public bool IsLoading
        {
            get { return StopBtn.Visibility == Visibility.Visible; }
            set {
                StopBtn.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                RefreshBtn.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public IList<HeaderItem> HeaderItems { get; private set; }


        private void WebView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void EnterBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateUrl(UrlTb.Text);
        }

        public void NavigateUrl(string url)
        {
            url = UriRender.Render(url, SearchCb.SelectedIndex);
            UrlTb.Text = url;
            Browser.Source = new Uri(url);
        }

        private void UrlTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateUrl(UrlTb.Text);
            }
        }


        private void Browser_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Browser.Url = new Uri(((System.Windows.Forms.WebBrowser)sender).StatusText);
            e.Cancel = true;
        }


        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateUrl("https://www.baidu.com");
        }

        private void BeforeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoBack)
            {
                Browser.GoBack();
            }
        }

        private void ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoForward)
            {
                Browser.GoForward();
            }
        }

        private void YesBtn_Click(object sender, RoutedEventArgs e)
        {
            _ = LoadHeaderAsync();
        }

        private async Task LoadHeaderAsync()
        {
            var cookie = await Browser.CoreWebView2.CookieManager.GetCookiesAsync(Browser.Source.ToString());
            HeaderItems = new List<HeaderItem> {
                new HeaderItem("Accept", HttpAccept.Html),
                new HeaderItem("Cookie", string.Join(';', cookie.Select(i => i.ToSystemNetCookie().ToString()))),
                new HeaderItem("Referer", Browser.Source.ToString()),
                new HeaderItem("User-Agent", HttpUserAgent.Chrome)
            };
            DialogResult = true;
        }

        public async Task<string> GetHtmlAsync()
        {
            var html = await Browser.ExecuteScriptAsync("document.documentElement.outerHTML");
            html = Regex.Unescape(html);
            html = html.Remove(0, 1);
            return html.Remove(html.Length - 1, 1);
        }

        private void Browser_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            IsLoading = true;
        }

        private void Browser_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            Title = Browser.CoreWebView2.DocumentTitle;
            BeforeBtn.Visibility = Browser.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            ForwardBtn.Visibility = Browser.CanGoForward ? Visibility.Visible : Visibility.Collapsed;
            IsLoading = false;
        }

        private async Task DealHtmlAsync()
        {
            var html = await GetHtmlAsync();
            if (string.IsNullOrWhiteSpace(html))
            {
                return;
            }
            // HtmlCallback?.Invoke(html);
            // _ = Browser.ExecuteScriptAsync("!function(){function c(a){var b,c,d;for(b=0;b<a.length;b++)c=a[b],d=c.getAttribute(\"target\"),\"_blank\"==d&&c.setAttribute(\"target\",\"_self\")}var a=document.getElementsByTagName(\"a\"),b=document.getElementsByTagName(\"form\");c(a),c(b)}();");
        }

        private void Browser_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            UrlTb.Text = Browser.Source.ToString();
        }

        private void Browser_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            Browser.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            Browser.CoreWebView2.AddHostObjectToScript("zreSpider", bridge);
            Browser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("var zreSpider = window.chrome.webview.hostObjects.zreSpider;");
        }

        private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
            Browser.Source = new Uri(e.Uri);
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            Browser.Reload();
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            Browser.Stop();
        }

        public Task<string> GetAsync(string url)
        {
            NavigateUrl(url);
            return Task.Factory.StartNew(() =>
            {
                while(true)
                {
                    if (!IsLoading)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
                return GetHtmlAsync().GetAwaiter().GetResult();
            });
        }

        public Task<string> ExecuteScriptAsync(string url, string script)
        {
            NavigateUrl(url);
            return Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (!IsLoading)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
                return Browser.ExecuteScriptAsync(script).GetAwaiter().GetResult();
            });
        }

        public Task<string> GetAsync(string url, IList<HeaderItem> headers)
        {
            return GetAsync(url);
        }

        public Task<string> GetAsync(string url, IList<HeaderItem> headers, ProxyItem? proxy)
        {
            return GetAsync(url);
        }
    }

    public delegate void DocumentReadyEventHandler(object sender, string html);
}
