using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Http
{
    public class HttpContentResponse(string content) : IHttpResponse
    {
        public HttpStatusCode StatusCode => HttpStatusCode.OK;

        public string RedirectLocation => string.Empty;

        public string ContentDispositionFileName => string.Empty;

        public string ContentTypeMediaType => "text/html";



        public Task<string> ReadAsync()
        {
            return Task.FromResult(content);
        }

        public Task<bool> SaveAsync(string file, Action<long, long>? progress = null, CancellationToken token = default)
        {
            File.WriteAllText(file, content, new UTF8Encoding(false));
            return Task.FromResult(true);
        }

        public void Dispose()
        {
        }
    }
}
