using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Shared.Rules
{
    public class TraditionalToSimplifiedRule : IRule
    {
        public void Render(ISpiderContainer container)
        {
            container.Data = container.Data.Select(i => new RuleString(Format(i.ToString(), true)));
            container.Next();
        }

        public string Format(string content, bool isTc)
        {
            return ChineseConverter.Convert(content, isTc ? ChineseConversionDirection.TraditionalToSimplified : ChineseConversionDirection.SimplifiedToTraditional);
        }
    }
}
