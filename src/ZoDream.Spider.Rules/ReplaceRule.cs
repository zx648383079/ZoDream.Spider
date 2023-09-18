using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Spider.Rules
{
    public class ReplaceRule : IRule
    {
        private string Search = string.Empty;
        private string Replace = string.Empty;
        public PluginInfo Info()
        {
            return new PluginInfo("普通替换");
        }

        public IFormInput[]? Form()
        {
            return new IFormInput[] {
                Input.Text(nameof(Search), "查找的字符", true),
                Input.Text(nameof(Replace), "替换的值"),
            };
        }

        public void Ready(RuleItem option)
        {
            Search = option.Get<string>(nameof(Search)) ?? string.Empty;
            Replace = option.Get<string>(nameof(Replace)) ?? string.Empty;
        }
        public async Task RenderAsync(ISpiderContainer container)
        {
            container.Data = container.Data.Select(i => new RuleString(i.ToString().Replace(Search, Replace)));
            await container.NextAsync();
        }
    }
}
