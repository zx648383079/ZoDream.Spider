using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Shared.Http
{
    public class HttpRequest : IRequest, IDownloadRequest
    {
        public bool SupportTask { get; } = true;

        public Task<string?> GetAsync(string url)
        {
            return new Client().GetAsync(url);
        }

        public async Task<string?> GetAsync(RequestData request)
        {
            var client = new Client()
            {
                MaxRetries = request.RetryCount,
                RetryTime = request.RetryTime,
                TimeOut = request.Timeout * 1000,
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
            client.Proxy = request.Proxy;
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
            return new Client(url).SaveAsync(file); ;
        }

        public Task GetAsync(string file, RequestData request)
        {
            var client = new Client();
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
            client.Proxy = request.Proxy;
            client.Url = request.RealUrl;
            return client.SaveAsync(file);
        }

        public Task<string?> ExecuteScriptAsync(string url, string script)
        {
            return Task.FromResult((string?)string.Empty);
        }
    }
}
