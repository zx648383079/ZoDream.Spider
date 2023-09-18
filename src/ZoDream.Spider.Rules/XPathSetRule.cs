using HtmlAgilityPack;
using System;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.Rules
{
    public class XPathSetRule : IRule
    {
        private string Tag = string.Empty;
        private string TagFunc = string.Empty;
        private string Name = string.Empty;

        public PluginInfo Info()
        {
            return new PluginInfo("XPath提取属性");
        }

        public IFormInput[]? Form()
        {
            return new IFormInput[] {
                Input.Text(nameof(Tag), "选择器"),
                Input.Text(nameof(TagFunc), "属性"),
                Input.Text(nameof(Name), "新属性名"),
            };
        }

        public void Ready(RuleItem option)
        {
            Tag = option.Get<string>(nameof(Tag)) ?? string.Empty;
            Name = option.Get<string>(nameof(Name)) ?? string.Empty;
            TagFunc = option.Get<string>(nameof(TagFunc)) ?? string.Empty;
            JQueryRule.SplitTag(ref Tag, ref TagFunc);
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var doc = new HtmlDocument();
            foreach (var item in container.Data)
            {
                doc.LoadHtml(item.ToString());
                var nodes = doc.DocumentNode.SelectNodes(Tag);
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
                    var val = XPathRule.FormatNode(node, TagFunc);
                    if (string.IsNullOrWhiteSpace(val))
                    {
                        continue;
                    }
                    container.SetAttribute(Name, val);
                }
            }
            await container.NextAsync();
        }
    }
}
