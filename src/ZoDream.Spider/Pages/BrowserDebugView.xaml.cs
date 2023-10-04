using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Spider.JsObjects;

namespace ZoDream.Spider.Pages
{
    /// <summary>
    /// BrowserDebugView.xaml 的交互逻辑
    /// </summary>
    public partial class BrowserDebugView : Window, IRequest
    {
        public BrowserDebugView()
        {
            InitializeComponent();
        }

        public BrowserDebugView(BrowserFlags flag): this()
        {
            InitializeComponent();
            BrowserFlag = flag;
        }

        private readonly SpiderBridge bridge = new();
        public bool SupportTask { get; } = false;

        private BrowserFlags browserFlag;

        public BrowserFlags BrowserFlag {
            get { return browserFlag; }
            set {
                browserFlag = value;
                switch (value)
                {
                    case BrowserFlags.NONE:
                        YesBtn.Visibility = Visibility.Collapsed;
                        break;
                    case BrowserFlags.CONFIRM:
                        YesBtn.Content = "确定";
                        YesBtn.IsEnabled = true;
                        YesBtn.Visibility = Visibility.Visible;
                        break;
                    case BrowserFlags.DEBUG:
                        YesBtn.Content = "执行调试";
                        YesBtn.IsEnabled = true;
                        YesBtn.Visibility = Visibility.Visible;
                        break;
                    case BrowserFlags.DOING:
                        YesBtn.Content = "执行中";
                        YesBtn.Visibility = Visibility.Visible;
                        YesBtn.IsEnabled = false;
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 当前页面是否加载成功
        /// </summary>
        public bool LoadSuccess { get; set; } = false;

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

        private bool IsRunning = false;
        private RequestData? RequestData = null;

        public IList<HeaderItem>? HeaderItems { get; private set; }

        public event ConfirmEventHandler? OnConfirm;


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
            UrlTb.Text = RequestData is not null ? RequestData.GetUrlByHostMap(url) : url;
            Browser.Source = new Uri(url);
            IsLoading = true;
        }

        public async Task NavigateUrlAsync(string url, RequestData? request)
        {
            RequestData = request;
            await AddProxyAsync(request?.Proxy);
            Dispatcher.Invoke(() => {
                NavigateUrl(url);
            });
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
            OnConfirm?.Invoke(this);
            if (BrowserFlag == BrowserFlags.CONFIRM)
            {
                _ = LoadHeaderAsync();
            }
        }

        private async Task LoadHeaderAsync()
        {
            var cookie = await Browser.CoreWebView2.CookieManager.GetCookiesAsync(Browser.Source.ToString());
            HeaderItems = new List<HeaderItem> {
                new("Accept", HttpAccept.Html),
                new("Cookie", string.Join(';', cookie.Select(i => i.ToSystemNetCookie().ToString()))),
                new("Referer", Browser.Source.ToString()),
                new("User-Agent", HttpUserAgent.Chrome)
            };
            DialogResult = true;
        }

        public async Task<string> GetHtmlAsync()
        {
            var html = await ExecuteScriptAsync("document.documentElement.outerHTML");
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }
            html = Regex.Unescape(html);
            html = html.Remove(0, 1);
            return html.Remove(html.Length - 1, 1);
        }

        private void Browser_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            IsLoading = true;
            LoadSuccess = false;
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
            Title = Browser.CoreWebView2.DocumentTitle;
            LoadSuccess = e.IsSuccess;
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
            coreWebView.AddHostObjectToScript("zreSpider", bridge);
            coreWebView.AddScriptToExecuteOnDocumentCreatedAsync("var zreSpider = window.chrome.webview.hostObjects.zreSpider;");
            coreWebView.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            coreWebView.DownloadStarting += CoreWebView_DownloadStarting;
            // coreWebView.DOMContentLoaded += CoreWebView_DOMContentLoaded;
            // 过滤网址
            // coreWebView.AddWebResourceRequestedFilter("https://zodream.cn/*", CoreWebView2WebResourceContext.All);
            // coreWebView.WebResourceResponseReceived += CoreWebView_WebResourceResponseReceived;
        }

        private async void CoreWebView_WebResourceResponseReceived(object? sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            // 获取页面所有请求的资源
            if (e.Response.StatusCode == 206)
            {
                var header = e.Response.Headers.GetHeader("Content-Type");
                if (header == "video/mp4")
                {
                    // 保存响应内容
                    var stream = await e.Response.GetContentAsync();
                }
            }
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
            Title = Browser.CoreWebView2.DocumentTitle;
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
        /// <summary>
        /// 设置代理服务器, 这一步必须放在浏览器初始化完成之前
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public async Task AddProxyAsync(object? proxy)
        {
            while (IsRunning)
            {
                Thread.Sleep(100);
            }
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

        public Task<string?> GetAsync(string url)
        {
            return ExecuteAsync(() => {
                NavigateUrl(url);
            }, GetHtmlAsync);
        }

        public async Task<string> ExecuteScriptAsync(string script)
        {
            var isCallback = script.IndexOf("zreSpider") >= 0;
            string funcRes = string.Empty;
            var isRet = false;
            ContentReadyEventHandler func = (string res) => {
                funcRes = res;
                isRet = true;
            };
            await Browser.EnsureCoreWebView2Async();
            if (isCallback)
            {
                bridge.ContentReady += func;
            }
            var res = await Browser.ExecuteScriptAsync(script);
            if (!isCallback)
            {
                return res == "null" ? string.Empty : res;
            }
            await Task.Factory.StartNew(() => {
                while (!isRet)
                {
                    Thread.Sleep(100);
                }
            });
            bridge.ContentReady -= func;
            return funcRes;
        }

        public Task<Tout?> ExecuteAsync<Tout>(Action action, Func<Task<Tout>> func)
        {
            return Task.Factory.StartNew(() => {
                while (IsRunning)
                {
                    Thread.Sleep(100);
                }
                Dispatcher.Invoke(() => {
                    action.Invoke();
                    IsRunning = true;
                });
                while (IsLoading)
                {
                    Thread.Sleep(100);
                }
                Tout? res = default;// default(Tout);
                Dispatcher.Invoke(async () => {
                    res = await func.Invoke();
                    IsRunning = false;
                });
                while (IsRunning)
                {
                    Thread.Sleep(100);
                }
                return res;
            });
        }

        public Task<string?> ExecuteScriptAsync(string url, string script)
        {
            return ExecuteAsync(() => {
                NavigateUrl(url);
            }, async () => {
                return await ExecuteScriptAsync(script);
            });
        }

        public async Task<string?> GetAsync(RequestData request)
        {
            string? res;
            RequestData = request;
            var url = request.RealUrl;
            var maxRetries = request.RetryCount;
            await AddProxyAsync(request.Proxy);
            do
            {
                res = await GetAsync(url);
                if (LoadSuccess)
                {
                    return res;
                }
                maxRetries--;
                Thread.SpinWait(request.Timeout * 1000);
            } while (maxRetries > 0 && request.Timeout > 0);
            return res;
        }

        public IHttpClient Create(RequestData request)
        {
            return new HttpRequest().Create(request);
        }
    }

    public delegate void ConfirmEventHandler(object sender);

    public enum BrowserFlags
    {
        NONE,
        CONFIRM,
        DEBUG,
        DOING,
    }
}
