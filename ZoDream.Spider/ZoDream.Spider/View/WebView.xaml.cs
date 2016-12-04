using System;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using ZoDream.Helper.Http;
using ZoDream.Spider.Helper;
using ZoDream.Spider.Helper.Cookie;
using ZoDream.Spider.Model;

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
            Browser.Navigate(url);
        }

        private void UrlTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateUrl(UrlTb.Text);
            }
        }

        private void Browser_Navigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
        {
            UrlTb.Text = Browser.Url.ToString();
            Title = Browser.DocumentTitle;
            BeforeBtn.Visibility = Browser.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            ForwardBtn.Visibility = Browser.CanGoForward ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Browser_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Browser.Url = new Uri(((System.Windows.Forms.WebBrowser)sender).StatusText);
            e.Cancel = true;
        }

        private void Browser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            //将所有的链接的目标，指向本窗体
            if (Browser.Document == null) return;
            HtmlCallback?.Invoke(GetHtml());

            foreach (System.Windows.Forms.HtmlElement archor in Browser.Document.Links)
            {
                archor.SetAttribute("target", "_self");
            }

            //将所有的FORM的提交目标，指向本窗体
            foreach (System.Windows.Forms.HtmlElement form in Browser.Document.Forms)
            {
                form.SetAttribute("target", "_self");
            }
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
                new HeaderItem(HttpRequestHeader.Cookie, FullWebBrowserCookie.GetCookieInternal(Browser.Url, false)),
                new HeaderItem(HttpRequestHeader.Referer, Browser.Url.ToString()),
                new HeaderItem(HttpRequestHeader.UserAgent, UserAgents.Chrome)
            };
            _callBack.Execute(rules);
            //_callBack.Execute(Browser.Document.Cookie);
            Close();
        }

        public string GetHtml()
        {
            if (Browser.Document == null || Browser.Document.Body == null)
            {
                return "";
            }
            return "<!DOCTYPE html><html>" + Browser.Document.GetElementsByTagName("html")[0].InnerHtml + "</html>";
                //Encoding.UTF8.GetString(Encoding.GetEncoding(Browser.Document.Encoding).GetBytes(Browser.Document.Body.OuterHtml));
        }
    }
}