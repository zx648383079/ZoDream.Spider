using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    /// <summary>
    /// 这是一个爬虫程序，需要装载网址容器和规则容器
    /// </summary>
    public interface ISpider: ILoader
    {
        public SpiderOption Option { get; set; }
        public IUrlProvider UrlProvider { get; set; }

        public IRuleProvider RuleProvider { get; set; }

        public IProxyProvider ProxyProvider {  get; set; }


        public void Start();

        public void Stop();

        public void Pause();

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
    }
}
