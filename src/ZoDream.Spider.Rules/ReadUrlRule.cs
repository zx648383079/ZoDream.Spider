using System;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.Rules
{
    public class ReadUrlRule : IRule
    {
        private string? Name;

        public PluginInfo Info()
        {
            return new PluginInfo("载入二级页面");
        }

        public IFormInput[]? Form()
        {
            return new IFormInput[] {
                Input.Text(nameof(Name), "属性名"),
            };
        }

        public void Ready(RuleItem option)
        {
            Name = option.Get<string>(nameof(Name));
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var uri = string.IsNullOrWhiteSpace(Name) ? container.Data.ToString() : 
                container.GetAttribute(Name);
            if (string.IsNullOrWhiteSpace(uri) || uri.Length > 500)
            {
                await container.NextAsync();
                return;
            }
            var fromUri = new Uri(container.Url.Source);
            var toUri = new Uri(fromUri, uri);
            var fullUri = toUri.ToString();
            container.Application.UrlProvider.Add(fullUri);
            var item = container.Application.UrlProvider.Get(fullUri);
            if (item == null)
            {
                await container.NextAsync();
                return;
            }
            await LoadNext(container, item);
        }

        private async Task LoadNext(ISpiderContainer container, UriItem url)
        {
            var spider = container.Application;
            spider.UrlProvider.EmitUpdate(url, UriCheckStatus.Doing);
            var content = await container.GetAsync(url.Source);
            if (content == null)
            {
                spider.UrlProvider.EmitUpdate(url, UriCheckStatus.Done);
                await container.NextAsync();
                return;
            }
            var rules = spider.RuleProvider.Get(url.Source);
            var keys = container.AttributeKeys;
            foreach (var item in rules)
            {
                var con = spider.GetContainer(url, spider.PluginLoader.Render(item.Rules));
                foreach (var key in keys)
                {
                    con.SetAttribute(key, con.GetAttribute(key));
                }
                await con.NextAsync();
            }
            spider.UrlProvider.EmitUpdate(url, UriCheckStatus.Done);
            await container.NextAsync();
        }
    }
}
