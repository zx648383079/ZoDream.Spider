using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Shared.Rules
{
    public class RegexReplaceRule : IRule
    {
        private string search = string.Empty;
        private string replace = string.Empty;
        public PluginInfo Info()
        {
            return new PluginInfo("正则替换");
        }

        public void Ready(RuleItem option)
        {
            search = option.Param1;
            replace = option.Param2;
        }
        public async Task RenderAsync(ISpiderContainer container)
        {
            var regex = new Regex(search, RegexOptions.IgnoreCase);
            container.Data = container.Data.Select(i => new RuleString(regex.Replace(i.ToString(), replace)));
            await container.NextAsync();
        }
    }
}
