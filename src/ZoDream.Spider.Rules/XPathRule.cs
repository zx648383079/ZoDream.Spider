using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Spider.Rules
{
    public class XPathRule : IRule
    {
        private string tag = string.Empty;
        private string tagFunc = "";

        public PluginInfo Info()
        {
            return new PluginInfo("XPath查询");
        }

        public void Ready(RuleItem option)
        {
            tag = option.Param1.Trim();
            var v = option.Param2.Trim().ToUpper();
            tagFunc = v != "0" && v != "F" && v != "N" && v != "FALSE" ? "html" : "text";
            JQueryRule.SplitTag(ref tag, ref tagFunc);
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var items = new List<IRuleValue>();
            var doc = new HtmlDocument();
            foreach (var item in container.Data)
            {
                doc.LoadHtml(item.ToString());
                var nodes = doc.DocumentNode.SelectNodes(tag);
                if (nodes == null || nodes.Count == 0)
                {
                    return;
                }
                foreach (var node in nodes)
                {
                    items.Add(new RuleString(FormatNode(node, tagFunc)));
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
            switch (func)
            {
                case "text":
                    return node.InnerText;
                case "html":
                    return node.InnerHtml;
                default:
                    return node.GetAttributeValue(func, string.Empty);
            }
        }
    }
}
