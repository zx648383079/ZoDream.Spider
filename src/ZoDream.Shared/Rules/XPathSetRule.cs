using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Shared.Rules
{
    public class XPathSetRule : IRule
    {
        private string tag = string.Empty;
        private string name = string.Empty;

        public PluginInfo Info()
        {
            return new PluginInfo("XPath提取属性");
        }

        public void Ready(RuleItem option)
        {
            tag = option.Param1.Trim();
            name = option.Param2.Trim();
        }

        public void Render(ISpiderContainer container)
        {
            var doc = new HtmlDocument();
            foreach (var item in container.Data)
            {
                doc.LoadHtml(item.ToString());
                var nodes = doc.DocumentNode.SelectNodes(tag);
                if (nodes == null || nodes.Count == 0)
                {
                    continue;
                }
                foreach (var node in nodes)
                {
                    if (node == null)
                    {
                        continue;
                    }
                    var val = string.IsNullOrWhiteSpace(node.InnerText) ?
                    node.GetAttributeValue(name, string.Empty) : node.InnerText;
                    if (string.IsNullOrWhiteSpace(val))
                    {
                        continue;
                    }
                    container.SetAttribute(name, val);
                }
            }
            container.Next();
        }
    }
}
