using System;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Http
{
    public class HttpRequest : IHttpRequest, IDownloadRequest
    {
        public bool SupportTask { get; } = true;


        private Client Create(RequestData request)
        {
            var client = new Client()
            {
                AllowAutoRedirect = request.AllowAutoRedirect,
                MaxRetries = request.RetryCount,
                RetryTime = request.RetryTime,
                TimeOut = request.Timeout * 1000,
                Proxy = request.Proxy,
                Url = request.RealUrl,
            };
            if (request.Headers is not null)
            {
                foreach (var item in request.Headers)
                {
                    client.Headers.Add(item.Name, item.Value);
                }
            }
            if (request.HostMap is not null)
            {
                client.Headers.Add("Host", request.HostMap.Host);
            }
            return client;
        }

        public async Task SendAsync(RequestData request, IRequestHost host)
        {
            var client = Create(request);
            using var response = new HttpResponse(client, await client.ReadResponseAsync());
            await host.InvokeAsync(request.Url, response);
        }

        public Task<string?> GetAsync(string url)
        {
            return new Client().GetAsync(url);
        }

        public async Task<string?> GetAsync(RequestData request)
        {
            var client = Create(request);
            try
            {
                return await client.GetAsync(request.RealUrl);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Task GetAsync(string file, string url)
        {
            return new Client(url).SaveAsync(file, null); ;
        }

        public Task GetAsync(string file, RequestData request, 
            Action<long, long>? progress = null, CancellationToken token = default)
        {
            var client = Create(request);
            return client.SaveAsync(file, progress, token);
        }

        public Task<string?> ExecuteScriptAsync(string url, string script)
        {
            return Task.FromResult((string?)string.Empty);
        }
    }
}
