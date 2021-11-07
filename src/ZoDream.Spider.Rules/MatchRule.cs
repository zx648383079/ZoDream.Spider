using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Spider.Rules
{
    public class MatchRule : IRule
    {
        private string pattern = string.Empty;

        private string tag = string.Empty;

        public PluginInfo Info()
        {
            return new PluginInfo("正则匹配");
        }

        public void Ready(RuleItem option)
        {
            pattern = option.Param1;
            tag = option.Param2;
        }
        public async Task RenderAsync(ISpiderContainer container)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var items = new List<IRuleValue>();
            var isEmpty = string.IsNullOrWhiteSpace(tag);
            var tagNum = !isEmpty && Regex.IsMatch(tag, "^[0-9]+$") ? int.Parse(tag) : -1;
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
                    items.Add(new RuleString(match.Groups[tag].Value));
                }
            }
            container.Data = new RuleArray(items);
            await container.NextAsync();
        }


    }
}
