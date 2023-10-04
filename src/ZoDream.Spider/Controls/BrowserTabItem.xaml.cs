using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZoDream.Shared.Http;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.Controls
{
    /// <summary>
    /// BrowserTabItem.xaml 的交互逻辑
    /// </summary>
    public partial class BrowserTabItem : UserControl
    {
        public BrowserTabItem()
        {
            InitializeComponent();
        }

        public BrowserTabItem(RequestData request): this()
        {
            RequestData = request;
            _ = AddProxyAsync(request.Proxy);
            NavigateUrl(request.Url);
        }

        public BrowserTabItem(string url, RequestData? request) : this()
        {
            RequestData = request;
            _ = AddProxyAsync(request?.Proxy);
            NavigateUrl(url);
        }

        private readonly RequestData? RequestData = null;

        public event EventHandler<string>? TitleChanged;
        public event EventHandler<string>? NewTabRequested;

        public bool IsLoading {
            get { return StopBtn.Visibility == Visibility.Visible; }
            set {
                StopBtn.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                RefreshBtn.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public string Source {
            get { return Browser.Source.ToString(); }
            set { NavigateUrl(value); }
        }

        private void EnterBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateUrl(UrlTb.Text);
        }

        public void NavigateUrl(string url)
        {
            url = UriRender.Render(url, SearchCb.SelectedIndex);
            UrlTb.Text = RequestData is not null ? RequestData.GetUrlByHostMap(url) : url;
            Browser.Source = new Uri(url);
            IsLoading = true;
        }

        private void UrlTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateUrl(UrlTb.Text);
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

        private void Browser_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            IsLoading = true;
            // 可以自定义请求头
            if (RequestData is not null)
            {
                if (RequestData.Headers is not null)
                {
                    foreach (var item in RequestData.Headers)
                    {
                        e.RequestHeaders.SetHeader(item.Name, item.Value);
                    }
                }
                if (RequestData.HostMap is not null && e.Uri.Contains(RequestData.HostMap.Ip))
                {
                    e.RequestHeaders.SetHeader("Host", RequestData.HostMap.Host);
                }
            }
        }

        private void Browser_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            // Title = Browser.CoreWebView2.DocumentTitle;
            // LoadSuccess = e.IsSuccess;
            BeforeBtn.Visibility = Browser.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            ForwardBtn.Visibility = Browser.CanGoForward ? Visibility.Visible : Visibility.Collapsed;
            IsLoading = false;
        }

        private void Browser_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            var uri = Browser.Source;
            UrlTb.Text = RequestData is not null ? RequestData.GetUrlByHostMap(uri) : uri.ToString();
        }

        private void Browser_CoreWebView2InitializationCompleted(object sender,
            CoreWebView2InitializationCompletedEventArgs e)
        {
            var coreWebView = Browser.CoreWebView2;
            coreWebView.NewWindowRequested += CoreWebView2_NewWindowRequested;
            // coreWebView.AddHostObjectToScript("zreSpider", bridge);
            // coreWebView.AddScriptToExecuteOnDocumentCreatedAsync("var zreSpider = window.chrome.webview.hostObjects.zreSpider;");
            coreWebView.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            coreWebView.DownloadStarting += CoreWebView_DownloadStarting;
            // coreWebView.DOMContentLoaded += CoreWebView_DOMContentLoaded;
            // 过滤网址
            // coreWebView.AddWebResourceRequestedFilter("https://zodream.cn/*", CoreWebView2WebResourceContext.All);
            // coreWebView.WebResourceResponseReceived += CoreWebView_WebResourceResponseReceived;
        }

        private void CoreWebView_DownloadStarting(object? sender, CoreWebView2DownloadStartingEventArgs e)
        {
            // e.Cancel = true;
            // e.DownloadOperation.Uri;
            // e.DownloadOperation.BytesReceivedChanged
            // 可以获取下载文件的进度及相关信息
        }

        private void CoreWebView_DOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            IsLoading = false;
        }

        private void CoreWebView2_DocumentTitleChanged(object? sender, object e)
        {
            TitleChanged?.Invoke(this, Browser.CoreWebView2.DocumentTitle);
        }

        private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
            NewTabRequested?.Invoke(this, RequestData is not null ? RequestData.GetUrlByHostMap(e.Uri) : e.Uri.ToString());
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            Browser.Reload();
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            Browser.Stop();
        }

        /// <summary>
        /// 设置代理服务器, 这一步必须放在浏览器初始化完成之前
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public async Task AddProxyAsync(object? proxy)
        {
            var data = proxy?.ToString();
            var options = new CoreWebView2EnvironmentOptions();
            if (!string.IsNullOrWhiteSpace(data))
            {
                options.AdditionalBrowserArguments = "--proxy-server=" + data.ToString();
            }
            var env = await CoreWebView2Environment.CreateAsync(null, null, options);
            try
            {
                await Browser.EnsureCoreWebView2Async(env);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
