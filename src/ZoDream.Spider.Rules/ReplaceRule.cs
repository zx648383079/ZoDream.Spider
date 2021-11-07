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
    public class ReplaceRule : IRule
    {
        private string search = string.Empty;
        private string replace = string.Empty;
        public PluginInfo Info()
        {
            return new PluginInfo("普通替换");
        }

        public void Ready(RuleItem option)
        {
            search = option.Param1;
            replace = option.Param2;
        }
        public async Task RenderAsync(ISpiderContainer container)
        {
            container.Data = container.Data.Select(i => new RuleString(i.ToString().Replace(search, replace)));
            await container.NextAsync();
        }
    }
}
