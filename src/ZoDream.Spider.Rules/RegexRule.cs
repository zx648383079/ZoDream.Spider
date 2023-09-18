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
    public class RegexRule : IRule
    {
        private string Pattern = string.Empty;

        private string Tag = string.Empty;

        public PluginInfo Info()
        {
            return new PluginInfo("正则截取");
        }
        public IFormInput[]? Form()
        {
            return new IFormInput[] {
                Input.Text(nameof(Pattern), "正则表达式", true),
                Input.Text(nameof(Tag), "匹配组"),
            };
        }

        public void Ready(RuleItem option)
        {
            Pattern = option.Get<string>(nameof(Pattern)) ?? string.Empty;
            Tag = option.Get<string>(nameof(Tag)) ?? string.Empty;
        }
        public async Task RenderAsync(ISpiderContainer container)
        {
            var regex = new Regex(Pattern, RegexOptions.IgnoreCase);
            var isEmptyTag = string.IsNullOrWhiteSpace(Tag);
            container.Data = container.Data.Select(i => new RuleString(
                isEmptyTag ? regex.Match(i.ToString()).Value : regex.Match(i.ToString()).Groups[Tag].Value
                ));
            await container.NextAsync();
        }
    }
}
