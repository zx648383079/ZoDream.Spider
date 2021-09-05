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
    public class SplitRule : IRule
    {
        public PluginInfo Info()
        {
            return new PluginInfo("拆分");
        }

        public void Ready(RuleItem option)
        {
            
        }

        public void Render(ISpiderContainer container)
        {
            if (container.Data is not RuleArray)
            {
                container.Next();
                return;
            }
            var data = container.Data as RuleArray;
            if (data.Items.Count < 1)
            {
                return;
            }
            if (data.Items.Count == 1)
            {
                container.Data = data.Items[0];
                container.Next();
                return;
            }
            var rules = new List<IRule>();
            for (int i = container.RuleIndex + 1; i < container.Rules.Count; i++)
            {
                rules.Add(container.Rules[i]);
            }
            if (rules.Count < 0)
            {
                return;
            }
            foreach (var item in data.Items)
            {
                var con = container.Application.GetContainer(container.Url, rules);
                con.Data = item;
                con.Next();
            }
        }
    }
}
