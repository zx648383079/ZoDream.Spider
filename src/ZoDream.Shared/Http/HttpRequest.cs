﻿using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Http
{
    public class HttpRequest : IRequest, IDownloadRequest
    {
        public bool SupportTask { get; } = true;

        public Task<string> GetAsync(string url)
        {
            return Task.Factory.StartNew(() =>
            {
                var res = new Client().Get(url);
                return res == null ? string.Empty : res;
            });
        }

        public Task<string> GetAsync(string url, IList<HeaderItem> headers)
        {
            return GetAsync(url, headers, null);
        }

        public Task<string> GetAsync(string url, IList<HeaderItem> headers, ProxyItem? proxy)
        {
            return Task.Factory.StartNew(() =>
            {
                var client = new Client();
                client.Headers = headers;
                client.Proxy = proxy;
                var res = client.Get(url);
                return res == null ? string.Empty : res;
            });
        }

        public Task GetAsync(string file, string url)
        {
            return Task.Factory.StartNew(() =>
            {
                new Client().ReadAsFile(url, file);
            });
        }

        public Task GetAsync(string file, string url, IList<HeaderItem> headers)
        {
            return GetAsync(file, url, headers, null);
        }

        public Task GetAsync(string file, string url, IList<HeaderItem> headers, ProxyItem? proxy)
        {
            return Task.Factory.StartNew(() =>
            {
                var client = new Client();
                client.Headers = headers;
                client.Proxy = proxy;
                client.ReadAsFile(url, file);
            });
        }
    }
}