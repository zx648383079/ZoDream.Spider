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
    public class UrlRule : IRule
    {
        private string Pattern = string.Empty;

        private string Tag = string.Empty;

        public PluginInfo Info()
        {
            return new PluginInfo("自定义网址");
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
            container.Data = container.Data.Select(i => RenderOne(container, i));
            await container.NextAsync();
        }

        private IRuleValue RenderOne(ISpiderContainer container, IRuleValue value)
        {
            var content = value.ToString();
            GetUrlFromCustom(container, ref content);
            return new RuleString(content);
        }

        public void GetUrlFromCustom(ISpiderContainer container, ref string html)
        {
            var items = new List<string>();
            var regex = new Regex(Pattern, RegexOptions.IgnoreCase);
            var isEmptyTag = string.IsNullOrWhiteSpace(Tag);
            var matches = regex.Matches(html);
            foreach (Match item in matches)
            {
                var value = isEmptyTag ? item.Value : item.Groups[Tag].Value;
                if (value.IndexOf("base64,") >= 0)
                {
                    continue;
                }
                var uri = container.AddUri(value, UriType.File);
                html = html.Replace(item.Value,
                    isEmptyTag ? uri : item.Value.Replace(item.Groups[Tag].Value, uri));
            }
        }
    }
}
