﻿using System.Windows;
using System.Windows.Input;
using ZoDream.Helper.Http;

namespace ZoDream.Spider.View
{
    /// <summary>
    /// Description for WebView.
    /// </summary>
    public partial class WebView : Window
    {
        /// <summary>
        /// Initializes a new instance of the WebView class.
        /// </summary>
        public WebView()
        {
            InitializeComponent();
        }

        private void EnterBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateUrl(UrlTb.Text);
        }

        private void NavigateUrl(string url)
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
            Browser.Url = new System.Uri(((System.Windows.Forms.WebBrowser)sender).StatusText);
            e.Cancel = true;
        }

        private void Browser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            //将所有的链接的目标，指向本窗体
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

        }
    }
}