using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Rules
{
    public class RegexSetRule : IRule
    {
        private string pattern = string.Empty;
        private string name = string.Empty;
        public PluginInfo Info()
        {
            return new PluginInfo("提取属性");
        }

        public void Ready(RuleItem option)
        {
            pattern = option.Param1.Trim();
            name = option.Param2.Trim();
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var regex = new Regex(pattern);
            var match = regex.Match(container.Data.ToString());
            if (match == null)
            {
                await container.NextAsync();
                return;
            }
            if (!string.IsNullOrEmpty(name))
            {
                container.SetAttribute(name, match.Value);
            }
            var tags = regex.GetGroupNames();
            foreach (var tag in tags)
            {
                container.SetAttribute(tag, match.Groups[tag].Value);
            }
            await container.NextAsync();
        }
    }
}
