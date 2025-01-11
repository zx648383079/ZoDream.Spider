using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ZoDream.Shared.Events;
using ZoDream.Shared.Http;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    public interface IWebViewRequest
    {
        public string Url { get; }

        public RequestMethod Method { get; }
        public string GetHeader(string name);
    }

    public interface IWebViewResponse
    {
        public int StatusCode { get; }

        public string ContentType { get; }

        public ContentRangeHeaderValue? ContentRange { get; }

        public long ContentLength { get; }
        /// <summary>
        /// 当前想要内容的文件名
        /// </summary>
        public string FileName { get; }

        public string GetHeader(string name);

        public Task<Stream> GetContentAsync();
    }

    public interface IWebView
    {

        public event WebViewResponseReceivedEventHandler? ResponseReceived;

        public event WebViewDocumentChangedEventHandler? DocumentLoaded;
        public event WebViewDocumentChangedEventHandler? DocumentReady;
        public event WebViewDocumentChangedEventHandler? DocumentUnLoaded;

        public string DocumentTitle { get; }
        /// <summary>
        /// 获取当前网址
        /// </summary>
        public string Source { get; }

        public Task<string> GetDocumentAsync();

        public Task<CookieCollection> GetCookiesAsync();

        public Task<string?> ExecuteScriptAsync(string script);

        public Task<bool> OpenAsync(RequestData request);

        public Task ReadyAsync();

        public void Destroy();
    }
}
