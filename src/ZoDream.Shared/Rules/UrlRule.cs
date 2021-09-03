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
    public class UrlRule : IRule
    {
        private string pattern = string.Empty;

        private string tag = string.Empty;

        public PluginInfo Info()
        {
            return new PluginInfo("自定义网址");
        }

        public void Ready(RuleItem option)
        {
            pattern = option.Param1;
            tag = option.Param2.Trim();
        }

        public void Render(ISpiderContainer container)
        {
            container.Data = container.Data.Select(i => RenderOne(container, i));
            container.Next();
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
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var isEmptyTag = string.IsNullOrWhiteSpace(tag);
            var matches = regex.Matches(html);
            foreach (Match item in matches)
            {
                var value = isEmptyTag ? item.Value : item.Groups[tag].Value;
                if (value.IndexOf("base64,") >= 0)
                {
                    continue;
                }
                var uri = container.AddUri(value, UriType.File);
                html = html.Replace(item.Value,
                    isEmptyTag ? uri : item.Value.Replace(item.Groups[tag].Value, uri));
            }
        }
    }
}
