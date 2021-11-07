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
    public class SplitRule : IRule
    {
        public PluginInfo Info()
        {
            return new PluginInfo("拆分");
        }

        public void Ready(RuleItem option)
        {
            
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            if (container.Data is not RuleArray)
            {
                await container.NextAsync();
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
                await container.NextAsync();
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
                await con.NextAsync();
            }
        }
    }
}
