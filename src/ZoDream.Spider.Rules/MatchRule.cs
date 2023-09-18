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
    public class MatchRule : IRule
    {
        private string Pattern = string.Empty;

        private string Tag = string.Empty;

        public PluginInfo Info()
        {
            return new PluginInfo("正则匹配");
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
            var items = new List<IRuleValue>();
            var isEmpty = string.IsNullOrWhiteSpace(Tag);
            var tagNum = !isEmpty && Regex.IsMatch(Tag, "^[0-9]+$") ? int.Parse(Tag) : -1;
            var tags = regex.GetGroupNames();
            foreach (var item in container.Data)
            {
                var match = regex.Match(item.ToString());
                if (isEmpty)
                {
                    items.Add(new RuleMap(tags, match));
                }
                else if (tagNum >= 0)
                {
                    items.Add(new RuleString(match.Groups[tagNum].Value));
                }
                else
                {
                    items.Add(new RuleString(match.Groups[Tag].Value));
                }
            }
            container.Data = new RuleArray(items);
            await container.NextAsync();
        }


    }
}
