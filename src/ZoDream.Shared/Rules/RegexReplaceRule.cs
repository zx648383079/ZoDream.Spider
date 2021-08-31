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
        private readonly string search;
        private readonly string replace;
        public RegexReplaceRule(RuleItem option)
        {
            search = option.Param1;
            replace = option.Param2;
        }
        public void Render(ISpiderContainer container)
        {
            var regex = new Regex(search, RegexOptions.IgnoreCase);
            container.Data = container.Data.Select(i => new RuleString(regex.Replace(i.ToString(), replace)));
            container.Next();
        }
    }
}
