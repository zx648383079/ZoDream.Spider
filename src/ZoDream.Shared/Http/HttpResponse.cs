using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Http
{
    public class HttpResponse(Client client, HttpResponseMessage? response) : IHttpResponse
    {
        public HttpStatusCode StatusCode => response is null ? HttpStatusCode.NotFound : response.StatusCode;

        public string RedirectLocation => response?.Headers?.Location?.ToString() ?? string.Empty;

        public string ContentDispositionFileName => response?.Content?.Headers?.ContentDisposition?.FileName ?? string.Empty;

        public string ContentTypeMediaType => response?.Content?.Headers?.ContentType?.MediaType ?? string.Empty;

        public async Task<string> ReadAsync()
        {
            if (response is null)
            {
                return string.Empty;
            }
            return await client.ReadAsync(response) ?? string.Empty;
        }

        public async Task<bool> SaveAsync(string file, Action<long, long>? progress = null, CancellationToken token = default)
        {
            if (response is null)
            {
                return false;
            }
            return await client.SaveAsync(response, file, progress, token);
        }

        public void Dispose()
        {
            client.Dispose();
            response?.Dispose();
        }


    }
}
