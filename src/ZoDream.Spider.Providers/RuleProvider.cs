using System.Collections.Generic;
using System.Linq;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.Providers
{
    public class RuleProvider : IRuleProvider
    {
        public List<RuleGroupItem> Items { get; private set; } = new();

        
        private readonly ISpider Application;

        public RuleProvider(ISpider spider)
        {
            Application = spider;
            Add(spider.Project.RuleItems);
        }

        public bool Cannable(string uri, UriType uriType)
        {
            foreach (var item in Items)
            {
                if (item.IsAllow(uri, uriType))
                {
                    return true;
                }
            }
            return false;
        }
        public IList<RuleGroupItem> Get(string uri)
        {
            return Items.Where(item => item.IsMatch(uri)).ToList();
        }

        public IList<RuleGroupItem> GetEvent(string name)
        {
            return Items.Where(item => item.MatchType == RuleMatchType.Event && item.MatchValue == name).ToList();
        }

        public void Add(RuleGroupItem rule)
        {
            Items.Add(rule);
        }

        public void Add(IList<RuleGroupItem> rules)
        {
            Items = rules.ToList();
        }
        public IList<RuleGroupItem> All()
        {
            return Items.ToList();
        }

        
        /// <summary>
        /// 返回的不一定是绝对路径
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string GetFileName(string uri)
        {
            var items = Get(uri);
            foreach (var item in items)
            {
                foreach (var it in item.Rules)
                {
                    if (it == null)
                    {
                        continue;
                    }
                    if (!Application.PluginLoader.IsSaver(it))
                    {
                        continue;
                    }
                    var rule = Application.PluginLoader.Render(it);
                    if (rule is not IRuleSaver)
                    {
                        continue;
                    }
                    var file = (rule as IRuleSaver).GetFileName(uri);
                    if (!string.IsNullOrEmpty(file))
                    {
                        return file;
                    }
                }
            }
            return string.Empty;
        }
    }
}
