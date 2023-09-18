using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Spider.Rules
{
    public class TraditionalToSimplifiedRule : IRule
    {
        public PluginInfo Info()
        {
            return new PluginInfo("简繁转换");
        }

        public IFormInput[]? Form()
        {
            return null;
        }

        public void Ready(RuleItem option)
        {
        }
        public async Task RenderAsync(ISpiderContainer container)
        {
            container.Data = container.Data.Select(i => new RuleString(Format(i.ToString(), true)));
            await container.NextAsync();
        }

        public string Format(string content, bool isTc)
        {
            return ChineseConverter.Convert(content, isTc ? ChineseConversionDirection.TraditionalToSimplified : ChineseConversionDirection.SimplifiedToTraditional);
        }
    }
}
