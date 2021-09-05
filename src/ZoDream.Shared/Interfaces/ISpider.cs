using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Events;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    /// <summary>
    /// 这是一个爬虫程序，需要装载网址容器和规则容器
    /// </summary>
    public interface ISpider: ILoader
    {
        public bool IsDebug { get; set; }

        public bool Paused { get; }
        public SpiderOption Option { get; set; }
        public IUrlProvider UrlProvider { get; set; }

        public IRuleProvider RuleProvider { get; set; }

        public IProxyProvider ProxyProvider {  get; set; }

        public IRequestProvider RequestProvider {  get; set; }

        public ILogger Logger {  get; set; }

        public event PausedEventHandler PausedChanged;

        public void Start();

        public void Stop();

        public void Pause();

        /// <summary>
        /// 重新开始
        /// </summary>
        public void Resume();

        /// <summary>
        /// 加载爬虫任务
        /// </summary>
        /// <param name="file"></param>
        public void Load(string file);

        public Task LoadAsync(string file);

        /// <summary>
        /// 保存爬虫任务
        /// </summary>
        /// <param name="file"></param>
        public void Save(string file);
        public Task SaveAsync(string file);

        public ISpiderContainer GetContainer(UriItem url, IList<IRule> rules);

        public string GetAbsoluteFile(string fileName);
    }
}
