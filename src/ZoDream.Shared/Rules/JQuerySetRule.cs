using AngleSharp;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Shared.Rules
{
    public class JQuerySetRule : IRule
    {
        private string tag = string.Empty;
        private string name = string.Empty;

        public PluginInfo Info()
        {
            return new PluginInfo("JQuery提取属性");
        }

        public void Ready(RuleItem option)
        {
            tag = option.Param1.Trim();
            name = option.Param2.Trim();
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            foreach (var item in container.Data)
            {
                var doc = await context.OpenAsync(req => req.Content(item.ToString()));
                var node = doc.QuerySelector(tag);
                if (node == null)
                {
                    continue;
                }
                var val = string.IsNullOrWhiteSpace(node.TextContent) ?
                    node.GetAttribute(name) : node.TextContent;
                if (string.IsNullOrWhiteSpace(val))
                {
                    continue;
                }
                container.SetAttribute(name, val);
            }
            container.Next();
        }

        public void Render(ISpiderContainer container)
        {
            _ = RenderAsync(container);
        }
    }
}
