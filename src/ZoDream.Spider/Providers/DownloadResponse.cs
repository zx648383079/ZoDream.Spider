using Microsoft.Web.WebView2.Core;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Spider.Providers
{
    public class DownloadResponse(CoreWebView2DownloadStartingEventArgs args) : IHttpResponse
    {
        public HttpStatusCode StatusCode => HttpStatusCode.OK;

        public string RedirectLocation => string.Empty;

        public string ContentDispositionFileName => string.Empty;

        public string ContentTypeMediaType => "application/force-download";

       

        public Task<string> ReadAsync()
        {
            args.Cancel = true;
            return Task.FromResult(string.Empty);
        }

        public Task<bool> SaveAsync(string file, Action<long, long>? progress = null, CancellationToken token = default)
        {
            args.ResultFilePath = file;
            if (progress is not null)
            {
                args.DownloadOperation.BytesReceivedChanged += (_, _) => {
                    progress.Invoke(args.DownloadOperation.BytesReceived, (long)(args.DownloadOperation.TotalBytesToReceive ?? 0));
                };
            }
            //while (args.DownloadOperation.State == CoreWebView2DownloadState.InProgress)
            //{
            //    Thread.Sleep(100);
            //}
            return Task.FromResult(true);
        }

        public void Dispose()
        {

        }
    }
}
