using System;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Input;
using ZoDream.Helper.Http;
using ZoDream.Spider.Helper;
using ZoDream.Spider.Helper.Cookie;
using ZoDream.Spider.Model;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZoDream.Spider.View
{
    /// <summary>
    /// Description for WebView.
    /// </summary>
    public partial class WebView : Window
    {
        private NotificationMessageAction<List<HeaderItem>> _callBack;

        public Action<string> HtmlCallback { get; set; }

        /// <summary>
        /// Initializes a new instance of the WebView class.
        /// </summary>
        public WebView()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessageAction<List<HeaderItem>>>(this, "web", m =>
            {
                _callBack = m;
            });
            Closing += WebView_Closing;
        }

        private void CoreWebView2_NewWindowRequested(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
            Browser.Source = new Uri(e.Uri);
        }

        private void WebView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SpiderHelper.Browser = null;
            HtmlCallback = null;
        }

        private void EnterBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateUrl(UrlTb.Text);
        }

        public void NavigateUrl(string url)
        {
            url = Url.GetUrl(url, (SearchKind)SearchCb.SelectedIndex);
            UrlTb.Text = url;
            Browser.Source = new Uri(url);
            //Browser.Navigate(url);
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
            var rules = new List<HeaderItem> {
                new HeaderItem(HttpRequestHeader.Accept, Accepts.Html),
                new HeaderItem(HttpRequestHeader.Cookie, FullWebBrowserCookie.GetCookieInternal(Browser.Source, false)),
                new HeaderItem(HttpRequestHeader.Referer, Browser.Source.ToString()),
                new HeaderItem(HttpRequestHeader.UserAgent, UserAgents.Chrome)
            };
            _callBack.Execute(rules);
            Close();
        }

        public async Task<string> GetHtmlAsync()
        {
            var html = await Browser.ExecuteScriptAsync("document.getElementsByTagName('html')[0].innerHTML");
            if (string.IsNullOrWhiteSpace(html))
            {
                return html;
            }
            return "<!DOCTYPE html><html>" + JsonConvert.DeserializeObject(html) + "</html>";
        }

        private void Browser_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            
        }

        private void Browser_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            Title = Browser.CoreWebView2.DocumentTitle;
            BeforeBtn.Visibility = Browser.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            ForwardBtn.Visibility = Browser.CanGoForward ? Visibility.Visible : Visibility.Collapsed;

            _ = DealHtmlAsync();
        }

        private async Task DealHtmlAsync()
        {
            var html = await GetHtmlAsync();
            if (string.IsNullOrWhiteSpace(html))
            {
                return;
            }
            HtmlCallback?.Invoke(html);
            // _ = Browser.ExecuteScriptAsync("!function(){function c(a){var b,c,d;for(b=0;b<a.length;b++)c=a[b],d=c.getAttribute(\"target\"),\"_blank\"==d&&c.setAttribute(\"target\",\"_self\")}var a=document.getElementsByTagName(\"a\"),b=document.getElementsByTagName(\"form\");c(a),c(b)}();");
        }

        private void Browser_SourceChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            UrlTb.Text = Browser.Source.ToString();
        }

        private void Browser_CoreWebView2Ready(object sender, EventArgs e)
        {
            Browser.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
        }
    }
}