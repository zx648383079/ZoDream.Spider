using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Shared.Rules
{
    public class RegexRule : IRule
    {
        private string pattern;

        private string tag;

        public PluginInfo Info()
        {
            return new PluginInfo("正则截取");
        }

        public void Ready(RuleItem option)
        {
            pattern = option.Param1;
            tag = option.Param2.Trim();
        }
        public async Task RenderAsync(ISpiderContainer container)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var isEmptyTag = string.IsNullOrWhiteSpace(tag);
            container.Data = container.Data.Select(i => new RuleString(
                isEmptyTag ? regex.Match(i.ToString()).Value : regex.Match(i.ToString()).Groups[tag].Value
                ));
            await container.NextAsync();
        }
    }
}
