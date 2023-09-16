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
        public Task<string?> GetAsync(RequestData request);

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="url"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        public Task<string?> ExecuteScriptAsync(string url, string script);
    }
}
