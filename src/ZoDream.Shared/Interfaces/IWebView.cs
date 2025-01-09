using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ZoDream.Shared.Events;
using ZoDream.Shared.Http;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    public interface IWebViewRequest
    {
        public string Uri { get; }

        public RequestMethod Method { get; }
        public string GetHeader(string name);
    }

    public interface IWebViewResponse
    {
        public int StatusCode { get; }

        public string ContentType { get; }

        public ContentRangeHeaderValue? ContentRange { get; }

        public long ContentLength { get; }
        public string GetHeader(string name);

        public Task<Stream> GetContentAsync();
    }

    public interface IWebView
    {

        public event WebViewResponseReceivedEventHandler? ResponseReceived;

        public Task<bool> OpenAsync(RequestData request);

        public Task ReadyAsync();

        public void Destroy();
    }
}
