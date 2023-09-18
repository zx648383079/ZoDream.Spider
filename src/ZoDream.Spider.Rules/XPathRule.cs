using HtmlAgilityPack;
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
    public class XPathRule : IRule
    {
        private string Tag = string.Empty;
        private string TagFunc = "";

        public PluginInfo Info()
        {
            return new PluginInfo("XPath查询");
        }

        public IFormInput[]? Form()
        {
            return new IFormInput[] {
                Input.Text(nameof(Tag), "选择器"),
                Input.Text(nameof(TagFunc), "属性"),
            };
        }
        public void Ready(RuleItem option)
        {
            Tag = option.Get<string>(nameof(Tag)) ?? string.Empty;
            var v = option.Get<string>(nameof(TagFunc))?.Trim().ToUpper();
            TagFunc = v != "0" && v != "F" && v != "N" && v != "FALSE" ? "html" : "text";
            JQueryRule.SplitTag(ref Tag, ref TagFunc);
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var items = new List<IRuleValue>();
            var doc = new HtmlDocument();
            foreach (var item in container.Data)
            {
                doc.LoadHtml(item.ToString());
                var nodes = doc.DocumentNode.SelectNodes(Tag);
                if (nodes == null || nodes.Count == 0)
                {
                    return;
                }
                foreach (var node in nodes)
                {
                    items.Add(new RuleString(FormatNode(node, TagFunc)));
                }
            }
            container.Data = new RuleArray(items);
            await container.NextAsync();
        }

        public static string FormatNode(HtmlNode node, string func)
        {
            if (string.IsNullOrWhiteSpace(func))
            {
                func = "html";
            }
            return func switch
            {
                "text" => node.InnerText,
                "html" => node.InnerHtml,
                _ => node.GetAttributeValue(func, string.Empty),
            };
        }
    }
}
