using System.Collections.Generic;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Http
{
    public class HttpRequest : IRequest, IDownloadRequest
    {
        public bool SupportTask { get; } = true;

        public Task<string?> GetAsync(string url)
        {
            return new Client().GetAsync(url);
        }

        public Task<string?> GetAsync(string url, IList<HeaderItem> headers)
        {
            return GetAsync(url, headers, null);
        }

        public async Task<string?> GetAsync(string url, IList<HeaderItem> headers, ProxyItem? proxy)
        {
            var client = new Client();
            foreach (var item in headers)
            {
                client.Headers.Add(item.Name, item.Value);
            }
            client.Proxy = proxy;
            return await client.GetAsync(url);
        }

        public Task GetAsync(string file, string url)
        {
            return new Client(url).SaveAsync(file); ;
        }

        public Task GetAsync(string file, string url, IList<HeaderItem> headers)
        {
            return GetAsync(file, url, headers, null);
        }

        public Task GetAsync(string file, string url, IList<HeaderItem> headers, ProxyItem? proxy)
        {
            var client = new Client(url);
            foreach (var item in headers)
            {
                client.Headers.Add(item.Name, item.Value);
            }
            client.Proxy = proxy;
            return client.SaveAsync(file);
        }

        public Task<string?> ExecuteScriptAsync(string url, string script)
        {
            return Task.FromResult((string?)string.Empty);
        }
    }
}
