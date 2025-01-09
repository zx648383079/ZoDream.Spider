﻿using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.VideoDownloaderRule
{
    public class Ruler : IRule, IRuleSaver, IWebViewRule
    {
        private string BinFolder = string.Empty;
        public bool ShouldPrepare => false;
        public bool CanNext => false;

        public PluginInfo Info()
        {
            return new("视频下载");
        }

        public IFormInput[]? Form()
        {
            return [
                Input.File(nameof(BinFolder), "FFMpeg Bin 文件夹", true, false, true) 
            ];
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

        public void Ready(IWebView loader, ISpiderContainer container)
        {
            loader.ResponseReceived += CoreView_WebResourceResponseReceived;
        }

        private async void CoreView_WebResourceResponseReceived(IWebViewRequest request, IWebViewResponse response)
        {
            if (response.StatusCode != 206)
            {
                return;
            }
            var header = response.ContentType;
            if (header.StartsWith("video/"))
            {
                // 保存响应内容
                var range = response.ContentRange;
                var stream = await response.GetContentAsync();
            }
            else if (header == "application/octet-stream" || header.StartsWith("audio/"))
            {
                // 保存音频
                var stream = await response.GetContentAsync();
            }
        }

        public void Destroy(IWebView loader)
        {
            loader.ResponseReceived -= CoreView_WebResourceResponseReceived;
        }
    }
}
