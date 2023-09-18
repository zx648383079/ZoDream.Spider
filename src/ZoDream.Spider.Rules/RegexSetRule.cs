using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.Rules
{
    public class RegexSetRule : IRule
    {
        private string Pattern = string.Empty;
        private string Name = string.Empty;
        public PluginInfo Info()
        {
            return new PluginInfo("提取属性");
        }

        public IFormInput[]? Form()
        {
            return new IFormInput[] {
                Input.Text(nameof(Pattern), "正则表达式", true),
                Input.Text(nameof(Name), "新属性名"),
            };
        }

        public void Ready(RuleItem option)
        {
            Pattern = option.Get<string>(nameof(Pattern)) ?? string.Empty;
            Name = option.Get<string>(nameof(Name)) ?? string.Empty;
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var regex = new Regex(Pattern);
            var match = regex.Match(container.Data.ToString());
            if (match == null)
            {
                await container.NextAsync();
                return;
            }
            if (!string.IsNullOrEmpty(Name))
            {
                container.SetAttribute(Name, match.Value);
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
