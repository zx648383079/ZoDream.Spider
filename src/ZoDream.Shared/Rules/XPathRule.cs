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
    public class XPathRule : IRule
    {
        private readonly string tag;
        private readonly bool isHtml;

        public XPathRule(RuleItem option)
        {
            tag = option.Param1.Trim();
            var v = option.Param2.Trim().ToUpper();
            isHtml = v != "0" && v != "F" && v != "N" && v != "FALSE";
        }

        public void Render(ISpiderContainer container)
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
                    items.Add(new RuleString(isHtml ? node.InnerHtml : node.InnerText));
                }
            }
            container.Data = new RuleArray(items);
            container.Next();
        }
    }
}
