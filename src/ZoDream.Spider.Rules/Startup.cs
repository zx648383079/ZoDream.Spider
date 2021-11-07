using System;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Spider.Rules
{
    public class Startup
    {
        public void Boot(IServiceCollection service)
        {
            service.AddRule(new Type[] {
                typeof(DownloadRule),
                typeof(HtmlToTextRule),
                typeof(HtmlUrlRule),
                typeof(JoinRule),
                typeof(JQueryRule),
                typeof(MatchRule),
                typeof(NarrowRule),
                typeof(RegexReplaceRule),
                typeof(RegexRule),
                typeof(ReplaceRule),
                typeof(RestRule),
                typeof(SaveRule),
                typeof(TraditionalToSimplifiedRule),
                typeof(UrlRule),
                typeof(XPathRule),
            });
        }
    }
}
