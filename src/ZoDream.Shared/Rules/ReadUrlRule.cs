using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Rules
{
    public class ReadUrlRule : IRule
    {
        private string name = string.Empty;

        public PluginInfo Info()
        {
            return new PluginInfo("载入二级页面");
        }

        public void Ready(RuleItem option)
        {
            name = option.Param1.Trim();
        }

        public void Render(ISpiderContainer container)
        {
            var uri = string.IsNullOrWhiteSpace(name) ? container.Data.ToString() : container.GetAttribute(name);
            if (string.IsNullOrWhiteSpace(uri) || uri.Length > 500)
            {
                container.Next();
                return;
            }
            var fromUri = new Uri(container.Url.Source);
            var toUri = new Uri(fromUri, uri);
            var fullUri = toUri.ToString();
            container.Application.UrlProvider.Add(fullUri);
            var item = container.Application.UrlProvider.Get(fullUri);
            if (item == null)
            {
                container.Next();
                return;
            }
            LoadNext(container, item);
        }

        private async Task LoadNext(ISpiderContainer container, UriItem url)
        {
            var spider = container.Application;
            spider.UrlProvider.UpdateItem(url, UriStatus.DOING);
            var content = await spider.RequestProvider.Getter().GetAsync(url.Source,
                spider.Option.HeaderItems,
                spider.ProxyProvider.Get());
            if (content == null)
            {
                spider.UrlProvider.UpdateItem(url, UriStatus.DONE);
                container.Next();
                return;
            }
            var rules = spider.RuleProvider.Get(url.Source);
            var keys = container.AttributeKeys;
            foreach (var item in rules)
            {
                var con = spider.GetContainer(url, spider.RuleProvider.Render(item.Rules));
                foreach (var key in keys)
                {
                    con.SetAttribute(key, con.GetAttribute(key));
                }
                con.Next();
            }
            spider.UrlProvider.UpdateItem(url, UriStatus.DONE);
            container.Next();
        }
    }
}
