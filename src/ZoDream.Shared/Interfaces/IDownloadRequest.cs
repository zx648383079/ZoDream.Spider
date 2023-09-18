using System;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    /// <summary>
    /// 这是一个网址请求
    /// </summary>
    public interface IDownloadRequest
    {
        public Task GetAsync(string file, string url);
        public Task GetAsync(string file, RequestData request, 
            Action<long, long>? progress = null, CancellationToken token = default);
    }
}
