using AngleSharp;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Shared.Rules
{
    public class JQueryRule : IRule
    {
        private readonly string tag;
        private readonly bool isHtml;

        public JQueryRule(RuleItem option)
        {
            tag = option.Param1.Trim();
            var v = option.Param2.Trim().ToUpper();
            isHtml = v != "0" && v != "F" && v != "N" && v != "FALSE";
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var items = new List<IRuleValue>();
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            foreach (var item in container.Data)
            {
                var doc = await context.OpenAsync(req => req.Content(item.ToString()));
                var nodes = doc.QuerySelectorAll(tag);
                if (nodes == null || nodes.Length == 0)
                {
                    return;
                }
                foreach (var node in nodes)
                {
                    items.Add(new RuleString(isHtml ? node.InnerHtml : node.TextContent));
                }
            }
            container.Data = new RuleArray(items);
            container.Next();
        }

        public void Render(ISpiderContainer container)
        {
            _ = RenderAsync(container);
        }
    }
}
