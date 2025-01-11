using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Http;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Spider.Providers
{
    public class BrowserRequest(CoreWebView2WebResourceRequest request) : IWebViewRequest
    {

        public string Url => request.Uri;

        public RequestMethod Method => Enum.TryParse<RequestMethod>(request.Method, true, out var res) ? res : RequestMethod.Get;

        public string GetHeader(string name)
        {
            return request.Headers.GetHeader(name);
        }
    }

    public class BrowserResponse(CoreWebView2WebResourceResponseView response) : IWebViewResponse
    {
        public int StatusCode => response.StatusCode;

        public string ContentType => GetHeader("Content-Type");
        public ContentRangeHeaderValue? ContentRange => 
            ContentRangeHeaderValue.TryParse(GetHeader("Content-Range"), out var res) ? res : null;

        public long ContentLength => long.TryParse(GetHeader("Content-Length"), out var res) ? res : 0;

        public string FileName {
            get {
                var header = GetHeader("Content-Disposition");
                if (string.IsNullOrWhiteSpace(header))
                {
                    return string.Empty;
                }
                var match = Regex.Match(header, @"filename=""?([^"";]+)");
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
                return string.Empty;
            }
        }
        public string GetHeader(string name)
        {
            return response.Headers.GetHeader(name);
        }

        public Task<Stream> GetContentAsync()
        {
            return response.GetContentAsync();
        }
    }
}
