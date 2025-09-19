using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    /// <summary>
    /// 这是一个网址请求
    /// </summary>
    public interface IHttpRequest
    {

        public bool SupportTask { get; }

        public Task SendAsync(RequestData request, IRequestHost host);

        public Task<string?> GetAsync(RequestData request);

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="url"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        public Task<string?> ExecuteScriptAsync(string url, string script);
    }

    public interface IHttpResponse : IDisposable
    {
        public HttpStatusCode StatusCode { get; }

        public string RedirectLocation { get; }
        public string ContentDispositionFileName{ get; }

        public string ContentTypeMediaType { get; }

        public Task<string> ReadAsync();

        public Task<bool> SaveAsync(string file, Action<long, long>? progress = null, CancellationToken token = default);
    }
}
