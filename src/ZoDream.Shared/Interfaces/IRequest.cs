using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Interfaces
{
    /// <summary>
    /// 这是一个网址请求
    /// </summary>
    public interface IRequest
    {
        public Task<string> GetAsync(string url);
    }
}
