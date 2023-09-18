using AngleSharp;
using AngleSharp.Dom;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Spider.Rules
{
    public class JQueryRule : IRule
    {
        private string Tag = string.Empty;
        private string TagFunc = "";

        public PluginInfo Info()
        {
            return new PluginInfo("JQuery查询");
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
            SplitTag(ref Tag, ref TagFunc);
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var items = new List<IRuleValue>();
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            foreach (var item in container.Data)
            {
                var doc = await context.OpenAsync(req => req.Content(item.ToString()));
                var nodes = doc.QuerySelectorAll(Tag);
                if (nodes == null || nodes.Length == 0)
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

        public static void SplitTag(ref string tag, ref string func)
        {
            var match = Regex.Match(tag, @"\.(\S+?)\(\)$");
            if (!match.Success)
            {
                return;
            }
            func = match.Groups[1].Value;
            tag = tag.Substring(0, tag.Length - match.Value.Length);
        }

        public static string FormatNode(IElement node, string func)
        {
            if (string.IsNullOrWhiteSpace(func))
            {
                func = "html";
            }
            switch (func)
            {
                case "text":
                    return node.TextContent;
                case "html":
                    return node.InnerHtml;
                default:
                    var res = node.GetAttribute(func);
                    return res ?? string.Empty;
            }
        }
    }
}
