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
    public class NarrowRule : IRule
    {
        private string begin = string.Empty;

        private string end = string.Empty;

        public PluginInfo Info()
        {
            return new PluginInfo("截断");
        }

        public void Ready(RuleItem option)
        {
            begin = option.Param1;
            end = option.Param2.Trim();
        }
        public void Render(ISpiderContainer container)
        {
            var isEmptyTag = string.IsNullOrWhiteSpace(tag);
            container.Data = container.Data.Select(i => new RuleString(
                Narrow(i.ToString())
                ));
            container.Next();
        }

        public string Narrow(string val)
        {
            var index = val.IndexOf(begin, StringComparison.Ordinal);
            if (index < 0)
            {
                index = 0;
            }
            else
            {
                index += begin.Length;
            }
            var next = Math.Min(val.IndexOf(end, index, StringComparison.Ordinal), val.Length - index);
            return val.Substring(index, next);
        }
    }
}
