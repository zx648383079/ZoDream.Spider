using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Events;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.Providers
{
    public class BrowserControl(WebView2 control) : IWebView
    {
        public event WebViewResponseReceivedEventHandler? ResponseReceived;

        public event WebViewDocumentChangedEventHandler? DocumentLoaded;
        public event WebViewDocumentChangedEventHandler? DocumentReady;
        public event WebViewDocumentChangedEventHandler? DocumentUnLoaded;

        private string _lastSource = string.Empty;
        public string DocumentTitle => control.CoreWebView2.DocumentTitle;
        /// <summary>
        /// 获取当前网址
        /// </summary>
        public string Source => control.CoreWebView2.Source;

        public async Task<string> GetDocumentAsync()
        {
            var html = await ExecuteScriptAsync("document.documentElement.outerHTML");
            if (string.IsNullOrEmpty(html))
            {
                return html ?? string.Empty;
            }
            html = Regex.Unescape(html);
            html = html.Remove(0, 1);
            return html.Remove(html.Length - 1, 1);
        }

        public async Task<CookieCollection> GetCookiesAsync()
        {
            var items = await control.CoreWebView2.CookieManager.GetCookiesAsync(Source);
            var res = new CookieCollection();
            foreach (var cookie in items)
            {
                res.Add(cookie.ToSystemNetCookie());
            }
            return res;
        }

        public async Task<string?> ExecuteScriptAsync(string script)
        {
            return await control.ExecuteScriptAsync(script);
        }

        public Task<bool> OpenAsync(RequestData request)
        {
            control.Source = new Uri(request.Url);
            return Task.FromResult(true);
        }

        public async Task ReadyAsync()
        {
            await control.EnsureCoreWebView2Async();
            var coreView = control.CoreWebView2;
            coreView.WebResourceResponseReceived += CoreView_WebResourceResponseReceived;
            coreView.SourceChanged += CoreView_SourceChanged;
            coreView.NavigationCompleted += CoreView_NavigationCompleted;
        }

        private void CoreView_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            DocumentReady?.Invoke(this, Source);
        }


        private void CoreView_SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
        {
            DocumentUnLoaded?.Invoke(this, _lastSource);
            DocumentLoaded?.Invoke(this, _lastSource = Source);
        }

        public void Destroy()
        {
            var coreView = control.CoreWebView2;
            coreView.WebResourceResponseReceived -= CoreView_WebResourceResponseReceived;
            coreView.SourceChanged -= CoreView_SourceChanged;
            coreView.NavigationCompleted -= CoreView_NavigationCompleted;
        }

        private void CoreView_WebResourceResponseReceived(object? sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            ResponseReceived?.Invoke(this, new BrowserRequest(e.Request), new BrowserResponse(e.Response));
        }
    }
}
