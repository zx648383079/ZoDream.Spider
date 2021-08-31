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
    public class ReplaceRule : IRule
    {
        private readonly string search;
        private readonly string replace;
        public ReplaceRule(RuleItem option)
        {
            search = option.Param1;
            replace = option.Param2;
        }
        public void Render(ISpiderContainer container)
        {
            container.Data = container.Data.Select(i => new RuleString(i.ToString().Replace(search, replace)));
            container.Next();
        }
    }
}
