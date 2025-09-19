using AngleSharp.Io;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Spider.Pages;

namespace ZoDream.Spider.Providers
{
    public class WebViewResponse(BrowserDebugView host, 
        int statusCode, CoreWebView2HttpResponseHeaders headers, CoreWebView2WebResourceResponseReceivedEventArgs args) : IHttpResponse
    {
        public HttpStatusCode StatusCode => (HttpStatusCode)statusCode;

        public string RedirectLocation => string.Empty;

        public string ContentDispositionFileName => ParseContentDisposition(GetHeader(headers, "Content-Disposition"));

        public string ContentTypeMediaType => MediaTypeHeaderValue.TryParse(GetHeader(headers, "Content-Type"), out var res) && !string.IsNullOrEmpty(res.MediaType) ? res.MediaType : string.Empty;

        public async Task<string> ReadAsync()
        {
            /// 编码问题
            using var input = await args.Response.GetContentAsync();
            return new StreamReader(input).ReadToEnd();
        }

        public async Task<bool> SaveAsync(string file, Action<long, long>? progress = null, CancellationToken token = default)
        {
            using var input = await args.Response.GetContentAsync();
            if (input is null)
            {
                return false;
            }
            using var output = new FileStream(file, FileMode.Create);
            var bArr = new byte[1024];
            var byteReceived = 0L;
            progress?.Invoke(byteReceived, input.Length);
            int size;
            do
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }
                size = await input.ReadAsync(bArr, token);
                if (size > 0)
                {
                    await output.WriteAsync(bArr.AsMemory(0, size), token);
                    byteReceived += size;
                    progress?.Invoke(byteReceived, input.Length);
                }
            } while (size > 0);
            return true;
        }

        public void Dispose()
        {

        }

        internal static string GetHeader(CoreWebView2HttpResponseHeaders headers, string name)
        {
            if (!headers.Contains(name))
            {
                return string.Empty;
            }
            return headers.GetHeader(name);
        }

        internal static string ParseContentDisposition(string header)
        {
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
}
