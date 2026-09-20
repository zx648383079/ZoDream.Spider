using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    public interface IRequestHost
    {
        /// <summary>
        /// 根据Rules判断网址是否符合规则
        /// </summary>
        /// <param name="url"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool Cannable(string url, UriType type);
        /// <summary>
        /// 判断网址是否已经被处理过
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool IsInvoked(string url);
        /// <summary>
        /// 根据响应内容进行处理
        /// </summary>
        /// <param name="url"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public Task InvokeAsync(string url, IHttpResponse response);
        
    }
}
