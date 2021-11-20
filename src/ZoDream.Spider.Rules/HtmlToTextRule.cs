using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.Rules
{
    public class HtmlToTextRule : IRule
    {

        public PluginInfo Info()
        {
            return new PluginInfo("去HTML化");
        }

        public void Ready(RuleItem option)
        {
            
        }
        public async Task RenderAsync(ISpiderContainer container)
        {
            container.Data = container.Data.Select(i => new RuleString(Html.ToText(i.ToString())));
            await container.NextAsync();
        }
    }
}
