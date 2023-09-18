using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Spider.Rules
{
    public class RegexReplaceRule : IRule
    {
        private string Search = string.Empty;
        private string Replace = string.Empty;
        public PluginInfo Info()
        {
            return new PluginInfo("正则替换");
        }

        public IFormInput[]? Form()
        {
            return new IFormInput[] {
                Input.Text(nameof(Search), "正则表达式", true),
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
            var regex = new Regex(Search, RegexOptions.IgnoreCase);
            container.Data = container.Data.Select(i => new RuleString(regex.Replace(i.ToString(), 
                Replace)));
            await container.NextAsync();
        }
    }
}
