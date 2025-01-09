using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Threading.Tasks;
using ZoDream.Shared.Events;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.Providers
{
    public class BrowserControl(WebView2 control) : IWebView
    {
        public event WebViewResponseReceivedEventHandler? ResponseReceived;


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
        }

        public void Destroy()
        {
            control.CoreWebView2.WebResourceResponseReceived -= CoreView_WebResourceResponseReceived;
        }

        private void CoreView_WebResourceResponseReceived(object? sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            ResponseReceived?.Invoke(new BrowserRequest(e.Request), new BrowserResponse(e.Response));
        }
    }
}
