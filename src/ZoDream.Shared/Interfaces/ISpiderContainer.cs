using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    public interface ISpiderContainer
    {
        public ISpider Application { get; set; }
        public CancellationToken Token { get; }

        public ILogger? Logger { get; }

        public bool IsDebug { get; }

        public UriItem Url { get; set; }

        public IList<IRule> Rules { get; set; }
        public int RuleIndex { get; }
        /// <summary>
        /// 原始内容
        /// </summary>
        public string? OriginData { get; set; }
        public IRuleValue? Data { get; set; }

        public IEnumerable<string> AttributeKeys { get; }
        /// <summary>
        /// 添加网址
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>新的相对路径</returns>
        public string AddUri(string uri);

        public string AddUri(string uri, UriType uriType);

        public void SetAttribute(string name, string value);

        public void UnsetAttribute(string name);

        public string GetAttribute(string name);

        public Task NextAsync();

        public string RenderData(string content);

        public void EmitProgress(long step, long count);
        /// <summary>
        /// 获取页面内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<string?> GetAsync(string url);
        /// <summary>
        /// 下载页面内容
        /// </summary>
        /// <param name="fileName">保存地址</param>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task GetAsync(string fileName, string url);

        /// <summary>
        /// 保存内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Task SaveAsync(string fileName, string content);
    }
}
