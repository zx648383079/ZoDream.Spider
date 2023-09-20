using Microsoft.Web.WebView2.Wpf;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.VideoDownloaderRule
{
    public class Ruler : IRule, IRuleSaver, IRuleCustomLoader<WebView2>
    {
        private string BinFolder = string.Empty;
        public bool ShouldPrepare => false;
        public bool CanNext => false;

        public PluginInfo Info()
        {
            return new PluginInfo("视频下载");
        }

        public IFormInput[]? Form()
        {
            return new IFormInput[] { Input.File(nameof(BinFolder), "FFMpeg Bin 文件夹", true, false, true) };
        }

        public string GetFileName(string url)
        {
            return string.Empty;
        }


        public void Ready(RuleItem option)
        {
            BinFolder = option.Get<string>(nameof(BinFolder)) ?? string.Empty;
        }

        public async Task RenderAsync(ISpiderContainer container)
        {

        }

        public void Ready(WebView2 loader)
        {
            var coreView = loader.CoreWebView2;
            coreView.WebResourceResponseReceived += CoreView_WebResourceResponseReceived;
        }

        private async void CoreView_WebResourceResponseReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            if (e.Response.StatusCode != 206)
            {
                return;
            }
            var header = e.Response.Headers.GetHeader("Content-Type");
            if (header == "video/mp4")
            {
                // 保存响应内容
                var stream = await e.Response.GetContentAsync();
            } else if (header == "application/octet-stream")
            {
                // 保存音频
                var stream = await e.Response.GetContentAsync();
            }
        }

        public void Destroy(WebView2 loader)
        {
            loader.CoreWebView2.WebResourceResponseReceived -= CoreView_WebResourceResponseReceived;
        }
    }
}
