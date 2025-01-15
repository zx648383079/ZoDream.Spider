using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.IO;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.VideoDownloaderRule
{
    public class Ruler : IRule, IRuleSaver, IWebViewRule
    {

        private ISpiderContainer? _container;
        private readonly Dictionary<string, MultipartStream> _outputItems = [];
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
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return Disk.RenderFile(url);
            }
            return GetFileName(uri);
        }

        public string GetFileName(Uri uri)
        {
            if (uri.Host.Contains("bilibili.") || uri.Host.Contains("bilivideo"))
            {
                return Disk.RenderFile(uri.AbsolutePath);
            }
            return Disk.RenderFile(uri.AbsolutePath);
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
            _container = container;
            loader.ResponseReceived += WebView_WebResourceResponseReceived;
        }

        private async void WebView_WebResourceResponseReceived(IWebView sender, IWebViewRequest request, IWebViewResponse response)
        {
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var url))
            {
                return;
            }
            if (url.Host.Contains("bilivideo"))
            {
                DownloadBili(url, response);
                return;
            }
            if (url.Host.Contains("googlevideo.com"))
            {
                DownloadYtb(url, response);
                return;
            }
        }



        public void Destroy(IWebView loader)
        {
            loader.ResponseReceived -= WebView_WebResourceResponseReceived;
            foreach (var item in _outputItems)
            {
                item.Value.Dispose();
            }
            _outputItems.Clear();
        }

        private async void DownloadBili(Uri url, IWebViewResponse response)
        {
            if (response.StatusCode != 206)
            {
                return;
            }
            _container?.Logger?.Info($"Request: {url}");
            var header = response.ContentType;
            if (header.StartsWith("video/"))
            {
                // 保存响应内容
                var range = response.ContentRange;
                _container?.Logger?.Info($"Response: {header}; {range}");
                var stream = await response.GetContentAsync();
                AddStream(GetFileName(url, response), range, stream);
            }
            else if (header == "application/octet-stream" || header.StartsWith("audio/"))
            {
                // 保存音频
                var range = response.ContentRange;
                var stream = await response.GetContentAsync();
                AddStream(GetFileName(url, response), range, stream);
            }
        }

        private async void DownloadYtb(Uri url, IWebViewResponse response)
        {
            if (response.StatusCode != 200)
            {
                return;
            }
            if (response.ContentType == "application/vnd.yt-ump")
            {
                var stream = await response.GetContentAsync();
                var queries = HttpUtility.ParseQueryString(url.Query);
                AddStream(queries.Get("id") ?? url.AbsolutePath, null, stream);
            }
        }

        private async void AddStream(string fileName, ContentRangeHeaderValue? range, Stream input)
        {
            if (_container is null)
            {
                return;
            }
            if (!_outputItems.TryGetValue(fileName, out var output))
            {
                _container.Logger?.Info($"Save: {fileName}");
                var fs = await _container.Application.Storage.CreateStreamAsync(fileName);
                output = new MultipartStream(fs, range?.Length ?? 0);
                _outputItems.Add(fileName, output);
            }
            if (range is null || !range.HasRange)
            {
                output.Write(input);
            } else
            {
                output.Write(range.From ?? 0, (range.To - range.From) ?? input.Length, input);
            }
            if (output.IsCompleted)
            {
                _container.Logger?.Info($"Download Completed: {fileName}");
            }
        }

        private string GetFileName(Uri url, IWebViewResponse response)
        {
            var fileName = response.FileName;
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                return fileName;
            }
            return GetFileName(url);
        }

    }
}
