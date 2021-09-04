using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    /// <summary>
    /// 这是一个网址请求
    /// </summary>
    public interface IRequest
    {
        public bool SupportTask { get; }
        public Task<string> GetAsync(string url);
        public Task<string> GetAsync(string url, IList<HeaderItem> headers);
        public Task<string> GetAsync(string url, IList<HeaderItem> headers, ProxyItem? proxy);
    }
}
