using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Spider.Rules
{
    public class NarrowRule : IRule
    {
        private string Begin = string.Empty;

        private string End = string.Empty;

        public PluginInfo Info()
        {
            return new PluginInfo("截断");
        }

        public IFormInput[]? Form()
        {
            return new IFormInput[] {
                Input.Text(nameof(Begin), "开始字符", true),
                Input.Text(nameof(End), "结束字符", true),
            };
        }

        public void Ready(RuleItem option)
        {
            Begin = option.Get<string>(nameof(Begin)) ?? string.Empty;
            End = option.Get<string>(nameof(End)) ?? string.Empty;
        }
        public async Task RenderAsync(ISpiderContainer container)
        {
            container.Data = container.Data.Select(i => new RuleString(
                Narrow(i.ToString())
                ));
            await container.NextAsync();
        }

        public string Narrow(string val)
        {
            var index = val.IndexOf(Begin, StringComparison.Ordinal);
            if (index < 0)
            {
                index = 0;
            }
            else
            {
                index += Begin.Length;
            }
            var next = Math.Min(val.IndexOf(End, index, StringComparison.Ordinal), val.Length - index);
            return val.Substring(index, next);
        }
    }
}
